using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Dlugin;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class NewBuild
{
    [MenuItem("NewBuild/AssetBundle/Debug")]
    public static void BuildAssetBundle_Debug()
    {
        BuildAssetBundle(EditorUserBuildSettings.activeBuildTarget, true);
    }
    
    [MenuItem("NewBuild/AssetBundle/Release")]
    public static void BuildAssetBundle_Release()
    {
        BuildAssetBundle(EditorUserBuildSettings.activeBuildTarget, false);
    }
    
    [MenuItem("NewBuild/Player/Debug")]
    public static void BuildPlayer_Debug()
    {
        BuildPlayer(EditorUserBuildSettings.activeBuildTarget ,true, true, false);
    }
    
    [MenuItem("NewBuild/Player/Debug_AAB")]
    public static void BuildPlayer_Debug_AAB()
    {
        BuildPlayer(EditorUserBuildSettings.activeBuildTarget ,true, true, true);
    }
    
    [MenuItem("NewBuild/Player/Release")]
    public static void BuildPlayer_Release()
    {
        BuildPlayer(EditorUserBuildSettings.activeBuildTarget ,false, true, false);
    }
    
    [MenuItem("NewBuild/Player/Release_AAB")]
    public static void BuildPlayer_Release_AAB()
    {
        BuildPlayer(EditorUserBuildSettings.activeBuildTarget ,false, true, true);
    }
    
    
    
    
    private static void BuildAssetBundle(BuildTarget target, bool isDebug)
    {
        //Symbol
        string defineSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (!defineSymbolString.EndsWith(";"))
        {
            defineSymbolString += ";";
        }
        if (defineSymbolString.IndexOf("IAP", StringComparison.Ordinal) < 0)
        {
            defineSymbolString += "IAP;";
        }
        if (defineSymbolString.IndexOf("HOT_FIX", StringComparison.Ordinal) < 0)
        {
            defineSymbolString += "HOT_FIX;";
        }
        
        if (!isDebug && defineSymbolString.IndexOf("PRODUCTION_PACKAGE", StringComparison.Ordinal) < 0)
        {
            defineSymbolString += "PRODUCTION_PACKAGE;";
        }  
        else if(isDebug)
        {
            defineSymbolString = "DEVELOPMENT_BUILD;" + defineSymbolString;
        }
        
        if (target == BuildTarget.Android)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbolString);
        }
        else if (target == BuildTarget.iOS)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbolString);
        }

        if (!isDebug)
        {
            ConfigurationController.Instance.version = VersionStatus.RELEASE;
        }

        
        //更新GlobalSetting 的 bundleVersion
        WriteBundleVersionToGameModuleCode();
        
        
        //ILRuntime
        AssemblyBuilderEditor.BuildAssemblySync(isDebug);
        AssetDatabase.ImportAsset("Assets/GameModuleDll/GameModule.bytes", ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
        
        
        
        //Addressable
        AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath("Assets/AddressableAssetsData/AddressableAssetSettings.asset", typeof(AddressableAssetSettings)) as AddressableAssetSettings;

        if (settings == null)
        {
            Debug.LogError("Can't Load AddressableAssetSettings" ); 
            return;
        }
        
        var profileName = "Default";

        if (!isDebug)
        {
            profileName = "Local";
                
        } else
        {
            profileName = "Debug";
        }



        string id = settings.profileSettings.GetProfileId(profileName);
         
        settings.activeProfileId = id;
        
        
        settings.profileSettings.SetValue(id, "RootVersion", BundleFolderSetting.BundleRootFolderName);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        AddressableAssetSettingsDefaultObject.Settings = settings;
        settings.profileSettings.SetValue(id, "BundleVersion", VersionSetting.BundleVersion);
           
        settings.profileSettings.SetValue(id, "RemoteBuildPath", $"ServerData/{BundleFolderSetting.BundleFolderName}/{BundleFolderSetting.BundleRootFolderName}/{VersionSetting.BundleVersion}");

        
        
        PackagePlayerBuildProcessor.RemoteBuildPath = settings.RemoteCatalogBuildPath.GetValue(settings);
        
        
        
        
        AddressableAssetSettings.CleanPlayerContent();
        BuildCache.PurgeCache(false);
        if (Directory.Exists(Application.dataPath + "/../ServerData"))
            Directory.Delete(Application.dataPath + "/../ServerData", true);
        AddressableAssetSettings.BuildPlayerContent();
        
        //CreateSoftLinkForLatestBundle(VersionSetting.BundleVersion);
        
        CreateResVersionTxt();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        PushBundleBuildInfoToGit(isDebug);
        
        Debug.Log("BuildAssetBundle......Done" ); 
    }
    
    
    public static void WriteBundleVersionToGameModuleCode()
    {
        var globalSettingText = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts/Code@GameModule/Common/GlobalSetting.cs"));
        
        globalSettingText = Regex.Replace(globalSettingText, "public\\s+static\\s+string\\s+bundleVersion\\s+=\\s+\"v[\\d]*\";", $"public static string bundleVersion = \"{VersionSetting.BundleVersion}\";");
        
        File.WriteAllText(Path.Combine(Application.dataPath, "Scripts/Code@GameModule/Common/GlobalSetting.cs"), globalSettingText);
       
        AssetDatabase.SaveAssets();
      
        AssetDatabase.ImportAsset("Assets/Scripts/Code@GameModule/Common/GlobalSetting.cs", ImportAssetOptions.ForceSynchronousImport);
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
    }
    
    public static void PushBundleBuildInfoToGit(bool isDebug)
    {
        // if (!isDebug)
        // {
        //     var addCommand = "git add " + "Assets/Scripts/Code@GameModule/Common/GlobalSetting.cs";
        //     var commitCommand = "git commit -m" + "####BuildOnlineBundle-" + VersionSetting.BundleVersion + "#########";
        //     var pushCommand = "git push";
        //
        //     var combineCommand = addCommand + "&&" + commitCommand + "&&" + pushCommand;
        //
        //     ShellHelper.ProcessCommand(combineCommand, "");
        // }
    }
    

    private static void BuildPlayer(BuildTarget target, bool isDebug, bool CLRBinding, bool aab)
    {
        //AssetBundle
        BuildAssetBundle(target, isDebug);
        if(CLRBinding) ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis();
     //   CopyInpackageAssetBundle();
        
        //DragonSDK
        SetupDragonSDK();

        //Mass Setting
        PlayerSettings.bundleVersion = VersionSetting.Version;
        PlayerSettings.Android.bundleVersionCode = int.Parse(VersionSetting.VersionCode);
        PlayerSettings.iOS.buildNumber = VersionSetting.VersionCode;
        
        
        try
        {
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }
        catch (Exception e)
        {
            // ignored
        }

        if (target == BuildTarget.Android)
        {
            AssetConfigController.Instance.RootVersion = VersionSetting.Version;
            AssetConfigController.Instance.VersionCode = VersionSetting.VersionCode;
            // if (!aab)
            // {
            //     ConfigurationController.Instance.Res_Server_URL_Release =
            //         ConfigurationController.Instance.Res_Server_URL_AdHoc;
            // }
        }
        else if (target == BuildTarget.iOS)
        {
            AssetConfigController.Instance.IOSRootVersion = VersionSetting.Version;
            AssetConfigController.Instance.IOSVersionCode = VersionSetting.VersionCode;
        }
        EditorUtility.SetDirty(AssetConfigController.Instance);
        
        System.IO.Directory.Move(Path.Combine(Application.dataPath, "Scripts/Code@GameModule") + "", Path.Combine(Application.dataPath, "Scripts/Code@GameModule~"));
        System.IO.Directory.Move(Path.Combine(Application.dataPath, "Editor/GameModule") + "", Path.Combine(Application.dataPath, "Editor/GameModule~"));
        
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        
        if (target == BuildTarget.Android)
        {
            PlayerSettings.Android.useCustomKeystore = ConfigurationController.Instance.AndroidKeyStoreUseConfiguration;
            PlayerSettings.Android.keystoreName = ConfigurationController.Instance.AndroidKeyStorePath;
            PlayerSettings.Android.keystorePass = ConfigurationController.Instance.AndroidKeyStorePass;
            PlayerSettings.Android.keyaliasName = ConfigurationController.Instance.AndroidKeyStoreAlias;
            PlayerSettings.Android.keyaliasPass = ConfigurationController.Instance.AndroidKeyStoreAliasPass;
            
            
            //Installation package
            string fileNameSuffix = aab ? ".aab" : ".apk";
            EditorUserBuildSettings.buildAppBundle = aab;
            string dirPath = Application.dataPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            string dir = directoryInfo.Parent.ToString();
            string fileName = "FortuneX" + fileNameSuffix;
            string outputPath = Path.Combine(dir, "AndroidExport/" + fileName);
            string outputDir = Path.Combine(dir, "AndroidExport/");
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            Debug.Log($"==========outputPath:{outputPath}");
        
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i) if (EditorBuildSettings.scenes[i].enabled) levels.Add(EditorBuildSettings.scenes[i].path);
            BuildReport report = BuildPipeline.BuildPlayer(levels.ToArray(), outputPath, target, isDebug ? BuildOptions.Development : BuildOptions.None);

            BuildSummary summary = report.summary;
            DebugUtil.Log(summary.result.ToString());
            
        }
        else if(target == BuildTarget.iOS)
        {
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            string outputPath = Path.GetFullPath(Application.dataPath + "/../iOS/build/");
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            
            Debug.Log($"==========outputPath:{outputPath}");
            
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i) if (EditorBuildSettings.scenes[i].enabled) levels.Add(EditorBuildSettings.scenes[i].path);
            BuildReport report = BuildPipeline.BuildPlayer(levels.ToArray(), outputPath, target, isDebug ? BuildOptions.Development : BuildOptions.None);
            BuildSummary summary = report.summary;
            DebugUtil.Log(summary.result.ToString());
        }
        else
        {
            Debug.LogError($"暂时不支持平台：{target}");
        }
        
        System.IO.Directory.Move(Path.Combine(Application.dataPath, "Scripts/Code@GameModule~") + "", Path.Combine(Application.dataPath, "Scripts/Code@GameModule"));
        System.IO.Directory.Move(Path.Combine(Application.dataPath, "Editor/GameModule~") + "", Path.Combine(Application.dataPath, "Editor/GameModule"));

        Debug.Log("BuildPlayer......Done" );
    }
    
    private static void SetupDragonSDK()
    {
        SDKEditor.SetFirebase();
        PluginConfigInfo info = PluginsInfoManager.LoadPluginConfig();

        if (info != null && info.m_Map.ContainsKey(Constants.FaceBook))
        {
            FacebookConfigInfo fbInfo = info.m_Map[Constants.FaceBook] as FacebookConfigInfo;
            SDKEditor.SetFacebook(fbInfo.AppID);
        }
    }

    private static void CopyInpackageAssetBundle()
    {
        string PackageBuildInAssetBundle = Application.streamingAssetsPath + "/" + ContentUpdater.InPackageAssetBundleFolderName;
        
        //clear
        {
            if (Directory.Exists(PackageBuildInAssetBundle))
            {
                Directory.Delete(PackageBuildInAssetBundle, true);
                File.Delete(PackageBuildInAssetBundle + ".meta");
            }
        
            string inPackageBundleAssetInfo = Path.Combine(Application.dataPath, "Resources/PackageAssetBundlesInfo.txt");
    
            if (File.Exists(inPackageBundleAssetInfo))
            {
                File.Delete(inPackageBundleAssetInfo);
                File.Delete(inPackageBundleAssetInfo + ".meta");
            }
        }
        
        //copy
        {
            AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath("Assets/AddressableAssetsData/AddressableAssetSettings.asset", 
                typeof(AddressableAssetSettings)) as AddressableAssetSettings;
            string remoteBuildPath = settings.RemoteCatalogBuildPath.GetValue(settings);
            string inPackageBundleSourcePath = Path.GetDirectoryName(Application.dataPath);
            if (!string.IsNullOrEmpty(inPackageBundleSourcePath))
            {
                string sourceFolder = Path.Combine(inPackageBundleSourcePath, remoteBuildPath);
                if (Directory.Exists(sourceFolder))
                {
                
                    if (Directory.Exists(PackageBuildInAssetBundle))
                    {
                        Debug.LogWarning($"Found and deleting directory \"{PackageBuildInAssetBundle}\", directory is managed By App.");
                        Directory.Delete(PackageBuildInAssetBundle, true);
                    }

                    string parentDir = Path.GetDirectoryName(Addressables.BuildPath);

                    if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
                        Directory.CreateDirectory(parentDir);


                    if (!Directory.Exists(PackageBuildInAssetBundle))
                    {
                        Directory.CreateDirectory(PackageBuildInAssetBundle);
                    }
 
                    string[] bundleFiles = Directory.GetFiles(sourceFolder, "inpackage_*.bundle");

                    string bundleFileInfo = "";
                    for (var i = 0; i < bundleFiles.Length; i++)
                    {
                        var targetFileName = Path.GetFileName(bundleFiles[i]);
                        bundleFileInfo += targetFileName + ";";
                        File.Copy(bundleFiles[i], Path.Combine(PackageBuildInAssetBundle, targetFileName));
                    }

                    string inPackageBundleAssetInfo = Path.Combine(Application.dataPath, "Resources/PackageAssetBundlesInfo.txt");
                
                    File.WriteAllText(inPackageBundleAssetInfo, bundleFileInfo);
                }
            }
        }
    }

    [MenuItem("NewBuild/Config/CreateResVersionTxt")]
    private static void CreateResVersionTxt()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
        string rootPath = directoryInfo.Parent.ToString();
        rootPath = $"{rootPath}/ServerData//{BundleFolderSetting.BundleFolderName}/{BundleFolderSetting.BundleRootFolderName}/";
        if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
        string path = $"{rootPath}ResVersion.txt";
        string config = ResVersionManager.CreateResVersionConfig(VersionSetting.BundleVersion);
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        byte[] bytes = new UTF8Encoding().GetBytes(config.ToString());
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }
}