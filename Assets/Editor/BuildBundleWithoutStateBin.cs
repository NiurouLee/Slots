
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using System.Text;
using System.Threading;
using DragonU3DSDK.Asset;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Scripting.ScriptCompilation;
using UnityEditor.Sprites;

// public enum BuildEnvironment
// {
//     Local  = 0,
//     Debug  = 1,
//     Release= 2,
// }
public class BuildBundleWithoutStateBin
{
    public static string bundleVersion;
    public static BuildEnvironment environment;
    private static string version;
    private static string versionCode;
     
    [MenuItem("Build/BuildLastestBundle")]
    public static void BuildLatestBundle()
    {
        //初始化编译参数
        InitializeBuildParams();
       
        Debug.Log("BuildEnvironment:" + environment);
        
      //  SaveAssetConfigController();
        
        //编译前做清理工作
        PreBuildAction();
          
        //编译热更新代码
        BuildGameModuleAssembly();
        
        BuildBundleContent();
    }
    
      
    private static void InitializeBuildParams()
    {
        var releaseBuild = GetProjectParameter("releaseBuild-");
        
        version = GetProjectParameter("version-");
        versionCode = GetProjectParameter("versionCode-");
         
        if (releaseBuild != null)
        {
            bundleVersion = "v" + GetProjectParameter("bundleVersion-");
        
            environment = releaseBuild == "true"
                ? BuildEnvironment.Release
                : BuildEnvironment.Debug;
        }
        else
        {
            environment = BuildEnvironment.Local;
            bundleVersion = "v6";
            versionCode = "1";
        }
    }
  
    private static void BuildGameModuleAssembly()
    {
        //更新GlobalSetting 的 bundleVersion
        WriteBundleVersionToGameModuleCode();
     
        Debug.Log("StartBuild-GameModuleAssembly");
        
        string defineSymbolString = "HOT_FIX;IAP";
        
        if (environment == BuildEnvironment.Release)
        {
            defineSymbolString = "PRODUCTION_PACKAGE;" + defineSymbolString;
        }
        else if(environment == BuildEnvironment.Debug || environment == BuildEnvironment.Local)
        {
            defineSymbolString = "DEVELOPMENT_BUILD;" + defineSymbolString;
        }
#if UNITY_ANDROID
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbolString);
#elif UNITY_IOS
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbolString);
#endif
        Debug.Log("Update DefineSymbols:" + defineSymbolString);
        AssetDatabase.SaveAssets();
        //编译GameModule代码
        
        Debug.Log("BuildAssemblySync....");
        AssemblyBuilderEditor.BuildAssemblySync(environment == BuildEnvironment.Debug || environment == BuildEnvironment.Local);
        
        Debug.Log("BuildAssemblySync Done....");
        
        AssetDatabase.ImportAsset("Assets/GameModuleDll/GameModule.bytes", ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
    }

    private static void PreBuildAction()
    {
        if (environment != BuildEnvironment.Local)
        {
            Debug.Log("Clean Cache");
            //先做清理工作，清楚cache
            AddressableAssetSettings.CleanPlayerContent(null);
            BuildCache.PurgeCache(false);

            if (Directory.Exists(Application.dataPath + "/../ServerData"))
                Directory.Delete(Application.dataPath + "/../ServerData", true);
        }

#if UNITY_ANDROID
        Debug.Log("PackAllAtlases Begin" + DateTime.UtcNow.ToString("s"));
        UnityEditor.U2D.SpriteAtlasUtility.PackAllAtlases(BuildTarget.Android);
        Debug.Log("PackAllAtlases End" +  DateTime.UtcNow.ToString("s"));
#elif UNITY_IOS
        UnityEditor.U2D.SpriteAtlasUtility.PackAllAtlases(BuildTarget.iOS);
#endif
    }
    
    private static void BuildBundleContent()
    {
        BuildPlayerContent();
        
        if(environment == BuildEnvironment.Release)
            PushBundleBuildInfoToGit();
    }
    public static void PushBundleBuildInfoToGit()
    {
        if (environment == BuildEnvironment.Release)
        {
            var addCommand = "git add " + "Assets/Scripts/Code@GameModule/Common/GlobalSetting.cs";
            var commitCommand = "git commit -m" + "####BuildOnlineBundle-" + bundleVersion + "#########";
            var pushCommand = "git push";
        
            var combineCommand = addCommand + "&&" + commitCommand + "&&" + pushCommand;
        
            ShellHelper.ProcessCommand(combineCommand, "");
        }
    }
    
    private static void BuildPlayerContent()
    {
        var settings = AssetDatabase.LoadAssetAtPath("Assets/AddressableAssetsData/AddressableAssetSettings.asset", typeof(AddressableAssetSettings)) as AddressableAssetSettings;
        
        if (settings)
        {
            var profileName = "Default";
           
            if (environment == BuildEnvironment.Local)
            {
                profileName = "Local";
                
            } else if (environment == BuildEnvironment.Debug)
            {
                profileName = "Debug";
            }
            
            string id = settings.profileSettings.GetProfileId(profileName);
            
            settings.activeProfileId = id;
            
            AddressableAssetSettingsDefaultObject.Settings = settings;
            settings.profileSettings.SetValue(id, "BundleVersion", bundleVersion);
           
            settings.profileSettings.SetValue(id, "RemoteBuildPath", $"ServerData/{BundleFolderSetting.BundleFolderName}/{BundleFolderSetting.BundleRootFolderName}/{bundleVersion}");
         
            

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            AddressableAssetSettings.BuildPlayerContent();
            
            CreateSoftLinkForLatestBundle(bundleVersion);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    public static void WriteBundleVersionToGameModuleCode()
    {
        var globalSettingText = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts/Code@GameModule/Common/GlobalSetting.cs"));
        
        globalSettingText = Regex.Replace(globalSettingText, "public\\s+static\\s+string\\s+bundleVersion\\s+=\\s+\"v[\\d]*\";", $"public static string bundleVersion = \"{bundleVersion}\";");
        
        File.WriteAllText(Path.Combine(Application.dataPath, "Scripts/Code@GameModule/Common/GlobalSetting.cs"), globalSettingText);
       
        AssetDatabase.SaveAssets();
      
        AssetDatabase.ImportAsset("Assets/Scripts/Code@GameModule/Common/GlobalSetting.cs", ImportAssetOptions.ForceSynchronousImport);
    }

    private static string GetProjectParameter(string preStr)
    {
        var args = System.Environment.GetCommandLineArgs();
        foreach (string arg in System.Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith(preStr))
            {
                return arg.Substring(preStr.Length);
            }
        }
        return null;
    }

    public static void CreateSoftLinkForLatestBundle(string latestBundleVersion)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
        string rootPath = directoryInfo.Parent.ToString();
        
        rootPath = $"{rootPath}/ServerData/{BundleFolderSetting.BundleFolderName}/{BundleFolderSetting.BundleRootFolderName}";
      
        if (!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }
        
        var cmd = $"ln -s {latestBundleVersion} current_version"; 
        ShellHelper.ProcessCommand(cmd,rootPath);
        
        //
        // string path = $"{rootPath}ResVersion.{version}.txt";
        // string config = ResVersionManager.CreateResVersionConfig(bundleVersion);
        // FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        // byte[] bytes = new UTF8Encoding().GetBytes(config.ToString());
        // fs.Write(bytes, 0, bytes.Length);
        // fs.Close();
    }
}
