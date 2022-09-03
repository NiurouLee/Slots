using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.Build.Pipeline.Utilities;
using System.Text.RegularExpressions;
using System.Threading;
using DragonU3DSDK.Asset;
using UnityEditor.AddressableAssets;

public class AndroidBuild
{
    private static string version;
    private static string versionCode;
    private static string releaseBuild;
    private static string GameModule;
    private static string buildPackageType;
    private static string printLog;

    private static BuildEnvironment environment;
    
    private static bool UseGameModule
    {
        get
        {
            return GameModule == "true";
        }
    }
    
    private static bool IsReleaseBuild
    {
        get
        {
            return releaseBuild == "true";
        }
    }
    [MenuItem("Build/BuildApk")]
    public static void BuildApk()
    {
        InitializeBuildParams();
        
        UpdateBuildSetting();
        
        CLRBinding();
        
        BuildBundle();

        BuildPlayer();
    }

    public static void InitializeBuildParams()
    {
        
        version = GetProjectParameter("version-");
        if (!string.IsNullOrEmpty(version))
        {
            versionCode = GetProjectParameter("versionCode-");
            releaseBuild = GetProjectParameter("releaseBuild-");
            GameModule = GetProjectParameter("GameModule-");
            buildPackageType = GetProjectParameter("buildPackageType-");
 
            environment = releaseBuild == "true"
                ? BuildEnvironment.Release
                : BuildEnvironment.Debug;
        }
        else
        {
            //取不到ProjectParameter的就认为是打本地包
            version = "1.0.0";
            versionCode = "10";
            releaseBuild = "false";
            GameModule = "true";
            buildPackageType = "0";
            environment = BuildEnvironment.Local;
        }
         
        Debug.Log($"BuildParams#######version:{version};versionCode:{versionCode}; releaseBuild{releaseBuild};GameModule{GameModule};");
    }
      
    private static void UpdateBuildSetting()
    {
        if (version != null)
        {
            PlayerSettings.bundleVersion = version;
            PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);
            
#if UNITY_ANDROID
            AssetConfigController.Instance.RootVersion = version;
            AssetConfigController.Instance.VersionCode = versionCode;
#elif UNITY_IOS
          //  AssetConfigController.Instance.IOSRootVersion = version;
            AssetConfigController.Instance.IOSVersionCode = versionCode;
#endif
            UnityEditor.EditorUtility.SetDirty(AssetConfigController.Instance);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        string defineSymbolString = "IAP";
       
        if (UseGameModule)
        {
            defineSymbolString = "HOT_FIX;" + defineSymbolString;
        }
        
        if (IsReleaseBuild)
        {
            defineSymbolString = "PRODUCTION_PACKAGE;" + defineSymbolString;
        }      
        
        else if(environment == BuildEnvironment.Debug || environment == BuildEnvironment.Local)
        {
            defineSymbolString = "DEVELOPMENT_BUILD;" + defineSymbolString;
        }

        WritePackageVersionToPackageSetting(version);
        
  #if  PRODUCTION_PACKAGE
          Debug.Log("PRODUCTION_PACKAGE1");
  #endif      
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbolString);
 
        PlayerSettings.Android.useCustomKeystore = ConfigurationController.Instance.AndroidKeyStoreUseConfiguration;
     
        PlayerSettings.Android.keystoreName = ConfigurationController.Instance.AndroidKeyStorePath;
        
        PlayerSettings.Android.keystorePass = ConfigurationController.Instance.AndroidKeyStorePass;

        PlayerSettings.Android.keyaliasName = ConfigurationController.Instance.AndroidKeyStoreAlias;
        
        PlayerSettings.Android.keyaliasPass = ConfigurationController.Instance.AndroidKeyStoreAliasPass;
    }



    private static void CLRBinding()
    {
        AssemblyBuilderEditor.BuildAssemblySync();
        ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis();
    }
 
    private static void BuildBundle()
    {
        Debug.Log("BuildBundle......" ); 
        Debug.Log("CleanCacheBundle......" ); 
        
        AddressableAssetSettings.CleanPlayerContent(null);
        
        BuildCache.PurgeCache(false);
       
        if(Directory.Exists(Application.dataPath + "/../ServerData"))
            Directory.Delete(Application.dataPath + "/../ServerData", true);

        Debug.Log("BuildAssembly......" ); 
        
        AssemblyBuilderEditor.BuildAssemblySync(environment == BuildEnvironment.Debug || environment == BuildEnvironment.Local);
        
        AssetDatabase.ImportAsset("Assets/GameModuleDll/GameModule.bytes", ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
 
        AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath("Assets/AddressableAssetsData/AddressableAssetSettings.asset", typeof(AddressableAssetSettings)) as AddressableAssetSettings;

        if (settings == null)
        {
            Debug.LogError("Can't Load AddressableAssetSettings" ); 
            return;
        }
        
        var profileName = "Default";

        if (environment == BuildEnvironment.Local)
        {
            profileName = "Local";
                
        } else if (environment == BuildEnvironment.Debug)
        {
            profileName = "Debug";
        }

        if (!UseGameModule)
        {
            profileName = "DebugBuild";
        }


        string id = settings.profileSettings.GetProfileId(profileName);
         
        settings.activeProfileId = id;
        
        
        settings.profileSettings.SetValue(id, "RootVersion", BundleFolderSetting.BundleRootFolderName);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        AddressableAssetSettingsDefaultObject.Settings = settings;
        
        PackagePlayerBuildProcessor.RemoteBuildPath = settings.RemoteCatalogBuildPath.GetValue(settings);
        
        Debug.Log("BuildPlayerContent----Started" ); 
     
        AddressableAssetSettings.BuildPlayerContent();
      
        Debug.Log("BuildPlayerContent-----Finished" );
        
        if (environment == BuildEnvironment.Release|| 
        environment == BuildEnvironment.Debug)
        {
            Debug.Log("Save 保存ContentStateData" ); 
            //保存ContentStateData
            var contentStatePath = ContentUpdateScript.GetContentStateDataPath(false);
            var newPackageStatePath = contentStatePath.Replace(".bin", "-" + version + ".bin");
            File.Copy(contentStatePath, newPackageStatePath, true);
           
            AssetDatabase.Refresh();
        }
    }
    
    private static void BuildPlayer()
    {
        Debug.Log("BuildPlayer:......" + IsReleaseBuild);
    
        string fileNameSuffix = ".apk";
     
        if (buildPackageType == "1")
        {
            EditorUserBuildSettings.buildAppBundle = true;
            fileNameSuffix = ".aab";
        }
        else
        {
            EditorUserBuildSettings.buildAppBundle = false;
        }
        string dirPath = Application.dataPath;
        DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
        string dir = directoryInfo.Parent.ToString();
 
        //string fileName = "FortuneX_"+ version + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "_" + (IsReleaseBuild ? "release" : "dev") + fileNameSuffix;
        
        string fileName = "FortuneX" + fileNameSuffix;
        string outputPath = Path.Combine(dir, "AndroidExport/" + fileName);

        string outputDir = Path.Combine(dir, "AndroidExport/");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        Debug.Log($"==========outputPath:{outputPath}");
        
        string[] levels = GetLevelsFromBuildSettings();

        if (environment == BuildEnvironment.Debug)
        {
            BuildPipeline.BuildPlayer(levels, outputPath, BuildTarget.Android, BuildOptions.Development);
        }
        else
        {
            BuildPipeline.BuildPlayer(levels, outputPath, BuildTarget.Android, BuildOptions.None);
        }
       
       // var contentStatePath = ContentUpdateScript.GetContentStateDataPath(false);
      //  var newPackageStatePath = contentStatePath.Replace(".bin", "-" + version + ".bin");

        Debug.Log("BuildPlayer......Done" ); 
        
        //上传 State Data文件到服务器
        // if (IsReleaseBuild && buildPackageType != "0")
        // {
        
        if (environment == BuildEnvironment.Release|| 
            environment == BuildEnvironment.Debug)
        {
            var addCommand = "git add Assets/Scripts/Code/PackageSetting.cs";
            var commitCommand = "git commit -m" + "####PackageBuild-" + version + "#########" + versionCode;
            var pushCommand = "git push";
            //var combineCommand = addCommand + " && " + commitCommand;
            var combineCommand = addCommand + " && " + commitCommand + " && " + pushCommand;
            
            ShellHelper.ProcessCommand(combineCommand, "");

            Thread.Sleep(50000);
            Debug.Log("ProductionBuild：Git Command:" + combineCommand); 
        }
    }

    private static void WritePackageVersionToPackageSetting(string packageVersion)
    {
        var packageSettingText = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts/Code/PackageSetting.cs"));
        
        Debug.Log("packageSettingText:" + packageSettingText);
        
        packageSettingText = Regex.Replace(packageSettingText, "public\\s+static\\s+string\\s+packageVersion\\s+=\\s+\"[\\w.\\d]+\";", $"public static string packageVersion = \"{packageVersion}\";");
      
        packageSettingText = Regex.Replace(packageSettingText, "public\\s+static\\s+long\\s+versionCode\\s+=\\s+\\d+;", $"public static long versionCode = {versionCode};");
        
        Debug.Log("packageSettingText:Modify" + packageSettingText);
        
        File.WriteAllText(Path.Combine(Application.dataPath, "Scripts/Code/PackageSetting.cs"), packageSettingText);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset("Assets/Scripts/Code/PackageSetting.cs", ImportAssetOptions.ForceSynchronousImport);
    }
 
    private static string[] GetLevelsFromBuildSettings()
    {
        List<string> levels = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
        {
            if (EditorBuildSettings.scenes[i].enabled)
                levels.Add(EditorBuildSettings.scenes[i].path);
        }

        return levels.ToArray();
    }

    private static string GetProjectParameter(string preStr)
    {
        //var args = System.Environment.GetCommandLineArgs();
        foreach (string arg in System.Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith(preStr))
            {
                return arg.Substring(preStr.Length);
            }
        }
        return null;
    }
}
