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
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.Build.Pipeline;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Scripting.ScriptCompilation;
using UnityEditor.Sprites;

public enum BuildEnvironment
{
    Local  = 0,
    Debug  = 1,
    Release= 2,
}
public class BuildBundle
{
    private static string bundleVersion;
    private static BuildEnvironment environment;
    private static string version;
    private static string versionCode;
    
    [MenuItem("Build/BuildContentUpdate")]
    private static void BuildContentUpdate()
    {
        
        
        //初始化编译参数
        InitializeBuildParams();
       
        Debug.Log("BuildEnvironment:" + environment);
        
        //SaveAssetConfigController();
        
        //编译前做清理工作
        PreBuildAction();
          
        //编译热更新代码
        BuildGameModuleAssembly();
        
        BuildContentUpdateBundle();
    }


    [MenuItem("Build/BuildContentUpdate(AllBundle)")]
    private static void BuildContentUpdateAllBundle()
    {

        BuildContentUpdate();
    }


//     public static void SaveAssetConfigController()
//     {
//         PlayerSettings.bundleVersion = version;
//         PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);
// #if UNITY_ANDROID
//         AssetConfigController.Instance.RootVersion = version;
//         AssetConfigController.Instance.VersionCode = versionCode;
// #elif UNITY_IOS
//             AssetConfigController.Instance.IOSRootVersion = version;
//             AssetConfigController.Instance.IOSVersionCode = versionCode;
// #endif
//         UnityEditor.EditorUtility.SetDirty(AssetConfigController.Instance);
//         AssetDatabase.Refresh();
//         AssetDatabase.SaveAssets();
//     }
     
    private static bool isAllBundleBuild = false;
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
            
            isAllBundleBuild = GetProjectParameter("isAllBundleBuild-")=="true";
        }
        else
        {
            environment = BuildEnvironment.Local;
            bundleVersion = "v6";
        }

        //BundleFolderSetting.bundleFolderName = bundleFolderName;
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
    
    private static void BuildContentUpdateBundle()
    {
        var stateDataPath = ContentUpdateScript.GetContentStateDataPath(false);
        
        Debug.Log("DefaultStatePath:" + stateDataPath);
       
        //Release版本需要为不同的包都编译bundle
        if (environment == BuildEnvironment.Release || isAllBundleBuild)
        {
            var directoryName = Path.GetDirectoryName(stateDataPath);
             
            if (!string.IsNullOrEmpty(directoryName))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directoryName);
                
                var fileInfo = dirInfo.GetFiles("*.bin");
             
                for (var i = 0; i < fileInfo.Length; i++)
                {
                    Debug.Log("NeedBuildBundleForFollowStateBin:" + fileInfo[i].FullName);
                }

                if (fileInfo.Length > 0)
                {
                    BuildBundleForPackageBuyIndex(0, fileInfo);
                }
            }
        }
        else if(environment == BuildEnvironment.Debug)
        {
            BuildBundleForPackageVersion(stateDataPath, null);
            
        }
        else if (environment == BuildEnvironment.Local)
        {
            //本地打热更新代码包，先将 stateDataPath 从 AddressableAssetsData拷贝出来
            //放在 Assets目录下，避免打完bundle之后，下次再打bundle的时候addressables_content_state被重置
            //为debug版本的 addressables_content_state
            
            var localStateDataPath = Application.dataPath + "/addressables_content_state.bin";
            if (File.Exists(localStateDataPath))
                BuildBundleForPackageVersion(stateDataPath, null);
            else
            {
                File.Copy(stateDataPath, localStateDataPath);
                BuildBundleForPackageVersion(stateDataPath, null);
            }
        }
        
    }
     
    private static void BuildBundleForPackageBuyIndex(int buildIndex, FileInfo[] fileInfos)
    {
        if (fileInfos.Length > buildIndex)
        {
            var stateDataPath = "Assets/" + fileInfos[buildIndex].FullName.Replace(Application.dataPath + "/", "");
           
            Debug.Log("BuildBundleForPackageVersion:" + stateDataPath);
         
            while (!stateDataPath.Contains("-"))
            {
                buildIndex++;
                
                if(fileInfos.Length > buildIndex)
                    stateDataPath = "Assets/" + fileInfos[buildIndex].FullName.Replace(Application.dataPath + "/", "");
                else
                {
                    break;
                }
            }

            if (stateDataPath.Contains("-"))
            {
                Debug.Log("BuildBundleForPackageVersion:" + stateDataPath);
              
                BuildBundleForPackageVersion(stateDataPath,
                    () =>
                    {
                        Debug.Log("BuildBundleForPackage:Done" + stateDataPath);
                        
                        BuildBundleForPackageBuyIndex( buildIndex + 1, fileInfos);
                    });
            }
            else
            {
                PushBundleBuildInfoToGit();
            }
        }
        else
        {
            PushBundleBuildInfoToGit();
        }
    }

    private static void PushBundleBuildInfoToGit()
    {
        var addCommand = "git add " + "Assets/Scripts/Code@GameModule/Common/GlobalSetting.cs";
        var commitCommand = "git commit -m" + "####BuildOnlineBundle-" + bundleVersion + "#########";
        var pushCommand = "git push";
        
        var combineCommand = addCommand + "&&" + commitCommand + "&&" + pushCommand;
        
        ShellHelper.ProcessCommand(combineCommand, "");
    }
    
    private static void BuildBundleForPackageVersion(string stateDataPath, Action buildBundleEndCallback)
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

            if (environment == BuildEnvironment.Release || 
                environment == BuildEnvironment.Debug)
            {
                //如果是正式包，'-'后面是包的版本号
               
                var index = stateDataPath.IndexOf('-');
                if (index > 0)
                {
                    var startIndex = index + 1;
                    var endIndex = stateDataPath.IndexOf(".bin");
                    var version = stateDataPath.Substring(startIndex, endIndex - startIndex);
                    settings.OverridePlayerVersion = version;
                    settings.profileSettings.SetValue(id, "PackageVersion", version);
                    settings.profileSettings.SetValue(id, "RemoteBuildPath", $"ServerData/{BundleFolderSetting.BundleFolderName}/v{version}/{bundleVersion}");

                    CreateResVersionTxt(version);
                }
                else
                {
                    settings.profileSettings.SetValue(id, "RemoteBuildPath", $"ServerData/{BundleFolderSetting.BundleFolderName}/{BundleFolderSetting.BundleRootFolderName}/{bundleVersion}");
                    CreateResVersionTxt(ResVersionManager.Version);
                }

                settings.profileSettings.SetValue(id, "BundleVersion", bundleVersion);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            //如果需要不同的包的bundle放不同的目录，就在设置RemoteBuildPath 后面加上 PackageVersion
            //settings.profileSettings.SetValue(id, "RemoteBuildPath", "ServerData/" + bundleFolderName + bundleVersion);
            var entries = ContentUpdateScript.GatherModifiedEntries(settings, stateDataPath);

            if (entries.Count > 0)
            {
                var localAtlasEntry = new List<AddressableAssetEntry>();
                for (var i = entries.Count- 1; i >= 0; i--)
                {
                  //  if (entries[i].parentGroup.Name.Contains("Atlas"))
                    if (entries[i].AssetPath.EndsWith(".spriteatlas"))
                    {
                        localAtlasEntry.Add(entries[i]);
                        entries.RemoveAt(i);
                    }
                }

                if (entries.Count > 0)
                {
                    ContentUpdateScript.CreateContentUpdateGroup(settings, entries, "Content Update");
                }

                if (localAtlasEntry.Count > 0)
                {
                    string atlasContentUpdateName = "Atlas Content Update";
                    ContentUpdateScript.CreateContentUpdateGroup(settings, localAtlasEntry, atlasContentUpdateName);
                    var group = settings.FindGroup(atlasContentUpdateName);
                    
                    if (group != null)
                    {
                        group.GetSchema<BundledAssetGroupSchema>().BundleMode =
                            BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                    }
                }
            }

            ContentUpdateScript.BuildContentUpdate(settings, stateDataPath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
 
            //正式环境，打包环境重置AddressableAssetsData中的修改，为下一个包打bundle 做准备
            if (environment == BuildEnvironment.Release|| 
                environment == BuildEnvironment.Debug)
            {
                
                ShellHelper.ProcessCommand("git checkout Assets/AddressableAssetsData/", "");
                Debug.Log("BuildBundleForPackageVersion: CheckOut");

                //等待Shell执行完成
                Thread.Sleep(5000);
                Debug.Log("BuildBundleForPackageVersion:Call callback");
                //刷新资源，避免用上一包的资源来打一下包的热更新内容
                AssetDatabase.Refresh();
                buildBundleEndCallback?.Invoke();
            }
        }
    }
    private static void WriteBundleVersionToGameModuleCode()
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

    private static void CreateResVersionTxt(string version)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
        
        string rootPath = directoryInfo.Parent.ToString();
        
        rootPath = $"{rootPath}/ServerData/{BundleFolderSetting.BundleFolderName}/";
       
        if (!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }
        
        var cmd = $"ln -s {version} current_version"; 
        ShellHelper.ProcessCommand(cmd,rootPath);
        
        // string path = $"{rootPath}ResVersion.{version}.txt";
        // string config = ResVersionManager.CreateResVersionConfig(bundleVersion);
        // FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        // byte[] bytes = new UTF8Encoding().GetBytes(config.ToString());
        // fs.Write(bytes, 0, bytes.Length);
        // fs.Close();
    }
}
