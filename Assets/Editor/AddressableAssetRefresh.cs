// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/15/14:06
// Ver : 1.0.0
// Description : AddressableAssetRefresh.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.U2D;
 
public class AddressableAssetConfigRefresh
{
    public static string systemPackageFolderPath = "/PackageAssets";
    public static string systemRemoteFolderPath = "/RemoteAssets/UISystem";
    
    public static string activityFolderPath = "/RemoteAssets/UIActivities";
    
    public static string machineLazyLoadFolderPath = "/RemoteAssets/Machine/LazyLoad";
 
    public static string machineAssetFolderPrefix = "Machine";
 
    public static int assetPriorityMaxLevel = 4;

    public static List<string> localMachineIds = new List<string>() {"Machine11003"};

    public static List<string> assetFolders = new List<string>()
    {
        "Textures", "Prefabs", "Atlas", "Animations", "Materials","Audio"
    };

    [MenuItem("Assets/Addressable配置/刷新所有目录配置",false)]
    public static void RefreshAssetGroupConfig()
    {
        RefreshMachine();
        
        RefreshInPackageSystemAssets();
        
        RefreshSystemRemoteAssets();
        
        RefreshActivitiesAssets();
        
        RemoveEmptyAssetGroup();

        SortingAssetsGroup();
    }

    /// <summary>
    /// 系统功能资源按照使用优先级分为三类:
    /// 进大厅之前必须要下载好的资源
    /// P1
    /// 进入大厅之后才需要开始下载的资源
    /// P2
    /// P2下载之后可以下载的资源，用于优化玩家的体验
    /// P3 
    /// 使用时候再下载的资源
    /// P4
    /// Prefab
    /// Atlas
 
    /// Audio
    /// Audio Opt
    /// </summary>
 
    [MenuItem("Assets/Addressable配置/刷新包内系统目录配置",false)]
    public static void RefreshInPackageSystemAssets()
    {
        var directoryInfo = new DirectoryInfo($"{Application.dataPath}{systemPackageFolderPath}");
            
        // List<string> ignoreFolders = new List<string>()
        // {
        //     "Common"
        // };
        
        foreach (var directory in directoryInfo.GetDirectories())
        {
            Debug.Log("Directory:" + directory.Name);
          //  if (!ignoreFolders.Contains(directory.Name))
            {
                string directoryPathInUnity = WindowsTools.WindowsPath2CommonPath(directory.FullName)
                    .Replace(Application.dataPath, "");
                
                directoryPathInUnity = $"Assets{directoryPathInUnity}";
                
                RefreshOrCreateSystemFolderAddressableConfig(directoryPathInUnity, true);
                
                bool isCancel = EditorUtility.DisplayCancelableProgressBar(directoryPathInUnity, "刷新完成", 1.0f);
          
                if (isCancel) 
                    return;
            }
        }
        EditorUtility.ClearProgressBar();
        SortingAssetsGroup();
    }
    
    [MenuItem("Assets/Addressable配置/刷新当前系统资源目录配置",true)]
    public static bool ValidateIsSystemAssetsFolder()
    {
        var guid = Selection.assetGUIDs[0];
        if (!string.IsNullOrEmpty(guid))
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(guid);
            string pattern = @"LazyLoad/Machine\d{5}";

            if (!Regex.IsMatch(clickedPath, pattern))
            {
                if (clickedPath.Contains("PackageAssets") || clickedPath.Contains("RemoteAssets/UISystem"))
                {
                    return AssetDatabase.IsValidFolder(clickedPath);
                }
            }
        }
        return false;
    }
    
    [MenuItem("Assets/Addressable配置/刷新当前系统资源目录配置",false)]
    public static void RefreshSystemAssetsFolder()
    {
        var guid = Selection.assetGUIDs[0];
        if (!string.IsNullOrEmpty(guid))
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(guid);
            string folderName = clickedPath.Substring(clickedPath.LastIndexOf("/") + 1);

            bool isOk = EditorUtility.DisplayDialog("Attention Please",
                $"是否需要刷新系统{folderName}资源的Addressable配置:{clickedPath}", "Ok", "Cancel");

            if (isOk)
            {
                RefreshOrCreateSystemFolderAddressableConfig(clickedPath, clickedPath.Contains("PackageAssets"));
            }
        }
    }
    
    [MenuItem("Assets/Addressable配置/刷新当前活动资源目录配置",true)]
    public static bool ValidateIsActivityAssetsFolder()
    {
        var guid = Selection.assetGUIDs[0];
        if (!string.IsNullOrEmpty(guid))
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(guid);
            string pattern = @"LazyLoad/Machine\d{5}";

            if (!Regex.IsMatch(clickedPath, pattern))
            {
                if (clickedPath.Contains("RemoteAssets/UIActivities"))
                {
                    return AssetDatabase.IsValidFolder(clickedPath);
                }
            }
        }
        return false;
    }
    
    [MenuItem("Assets/Addressable配置/刷新当前活动资源目录配置",false)]
    public static void RefreshActivityAssetsFolder()
    {
        var guid = Selection.assetGUIDs[0];
        if (!string.IsNullOrEmpty(guid))
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(guid);
            string folderName = clickedPath.Substring(clickedPath.LastIndexOf("/") + 1);

            bool isOk = EditorUtility.DisplayDialog("Attention Please",
                $"是否需要刷新活动{folderName}资源的Addressable配置:{clickedPath}", "Ok", "Cancel");

            if (isOk)
            {
                RefreshOrCreateSystemFolderAddressableConfig(clickedPath, false);
            }
        }
    }
     
    [MenuItem("Assets/Addressable配置/刷新RemoteAssets系统目录配置")]
    public static void RefreshSystemRemoteAssets()
    {
        var directoryInfo = new DirectoryInfo($"{Application.dataPath}{systemRemoteFolderPath}");
   
        foreach (var directory in directoryInfo.GetDirectories())
        {
            Debug.Log("Directory:" + directory.Name);
          //  if (!ignoreFolders.Contains(directory.Name))
            {
                string directoryPathInUnity = WindowsTools.WindowsPath2CommonPath(directory.FullName)
                    .Replace(Application.dataPath, "");
                
                directoryPathInUnity = $"Assets{directoryPathInUnity}";
                 
                RefreshOrCreateSystemFolderAddressableConfig(directoryPathInUnity, false);
                
                bool isCancel = EditorUtility.DisplayCancelableProgressBar(directoryPathInUnity, "刷新完成", 1.0f);
          
                if (isCancel) 
                    return;
            }
        }
        EditorUtility.ClearProgressBar();
    }
    
    [MenuItem("Assets/Addressable配置/刷新所有活动目录配置")]
    public static void RefreshActivitiesAssets()
    {
        var directoryInfo = new DirectoryInfo($"{Application.dataPath}{activityFolderPath}");
   
        foreach (var directory in directoryInfo.GetDirectories())
        {
            Debug.Log("Directory:" + directory.Name);
            //  if (!ignoreFolders.Contains(directory.Name))
            {
                string directoryPathInUnity = WindowsTools.WindowsPath2CommonPath(directory.FullName)
                    .Replace(Application.dataPath, "");
                
                directoryPathInUnity = $"Assets{directoryPathInUnity}";
                 
                RefreshOrCreateSystemFolderAddressableConfig(directoryPathInUnity, false);
                
                bool isCancel = EditorUtility.DisplayCancelableProgressBar(directoryPathInUnity, "刷新完成", 1.0f);
          
                if (isCancel) 
                    return;
            }
        }
        EditorUtility.ClearProgressBar();
        SortingAssetsGroup();
    }
     
    [MenuItem("Assets/Addressable配置/刷新或创建当前关卡的配置", false)]
    public static void CreateOrRefreshMachineAssets()
    {
        var guid = Selection.assetGUIDs[0];
        if (!string.IsNullOrEmpty(guid))
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(guid);
            string folderName = clickedPath.Substring(clickedPath.LastIndexOf("/") + 1);
            string machineAssetsId = folderName.Replace(machineAssetFolderPrefix, "");

            bool isOk = EditorUtility.DisplayDialog("Attention Please",
                $"是否需要刷新关卡{machineAssetsId}资源:{clickedPath}", "Ok", "Cancel");

            if (isOk)
            {
                CreateOrRefreshMachineAddressableConfig(localMachineIds.Contains(folderName), clickedPath,
                    machineAssetsId);
            }
        }
    }
    
    [MenuItem("Assets/Addressable配置/刷新或创建当前关卡的配置", true)]
    public static bool ValidateIsMachineAssetFolder()
    {
        var guid = Selection.assetGUIDs[0];
        if (!string.IsNullOrEmpty(guid))
        {
            string clickedPath = AssetDatabase.GUIDToAssetPath(guid);
            string pattern = @"LazyLoad/Machine\d{5}";

            if (Regex.IsMatch(clickedPath, pattern))
            {
                return AssetDatabase.IsValidFolder(clickedPath);
            }
        }
        return false;
    }

    [MenuItem("Assets/Addressable配置/刷新所有关卡的配置", false)]
    public static void RefreshMachine()
    {
        var directoryInfo = new DirectoryInfo($"{Application.dataPath}{machineLazyLoadFolderPath}");
        var localList = new List<string>() {"Machine11003"};

        foreach (var directory in directoryInfo.GetDirectories())
        {
            var isLocal = localList.Contains(directory.Name);

            string machineId = directory.Name.Replace(machineAssetFolderPrefix, "");

            string unityPath = WindowsTools.WindowsPath2CommonPath(directory.FullName)
                .Replace(Application.dataPath, "");
            unityPath = $"Assets{unityPath}";
            
            if (!CreateOrRefreshMachineAddressableConfig(isLocal, unityPath, machineId))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            
            string strTitle = "Machine" + machineId + "Group";
            bool isCancel = EditorUtility.DisplayCancelableProgressBar(strTitle, "创建完成", 1.0f);
          
            if (isCancel) 
                return;
        }
        
        RemoveEmptyAssetGroup();

        EditorUtility.ClearProgressBar();
        SortingAssetsGroup();
    }

    private static string machinePreloadGroup = "MachinePreLoadGroup";
    private static string configFolder = "Assets/AddressableAssetsData";
    private static string configName = "AddressableAssetSettings.asset";
  
    static bool CreateOrRefreshMachineAddressableConfig(bool isLocal, string machineResPath, string machineAssetId)
    {
        try
        {
            #region 创建关卡的Addressable Group

            string groupLabel = $"{machineAssetFolderPrefix}{machineAssetId}";
            string groupName = $"{machineAssetFolderPrefix}{machineAssetId}";

            List<string> groupLabels = new List<string>() {groupLabel};

            if (isLocal)
            {
                groupName = "In_Package_" + groupName;
                groupLabels.Add("AssetPriorityLevel1");
            }
 
            AddressableAssetSettings settings = GetAddressableAssetSettings();

            AddressableAssetGroup machineGroup = GetOrCreateAssetGroup(settings, groupName + "Group");
        
            ClearGroup(settings, machineGroup);
 
            var groupScheme = machineGroup.GetSchema<BundledAssetGroupSchema>();

            groupScheme.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.AppendHash;
            groupScheme.BuildPath.SetVariableByName(settings, "RemoteBuildPath");
            groupScheme.LoadPath.SetVariableByName(settings, "RemoteLoadPath");

            groupScheme.AssetBundledCacheClearBehavior =
                BundledAssetGroupSchema.CacheClearBehavior.ClearWhenWhenNewVersionLoaded;

            groupScheme.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            groupScheme.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
 
            var fieldInfo2 = groupScheme.GetType()
                .GetField("m_BundledAssetProviderType", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo2?.SetValue(groupScheme, new SerializedType {Value = typeof(BundledAssetProvider)});
            var fieldInfo = groupScheme.GetType()
                .GetField("m_AssetBundleProviderType", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo?.SetValue(groupScheme, new SerializedType {Value = typeof(AssetBundleProvider)});

            AddSettingsLabels(settings, groupLabel);
 
            AddAsset<GameObject>("t:Prefab", Path.Combine(machineResPath, "Prefab"), groupLabels, settings, machineGroup,
                new Dictionary<string, string>
                {
                    {"Prefab/Elements/Active", "Active_Element_" + machineAssetId},
                    {"Prefab/Elements/Static", "Static_Element_" + machineAssetId},
                    {"Prefab/PopUp", "PopUp_" + machineAssetId},
                    {"Prefab/Misc", "Misc_" + machineAssetId},
                    {"Prefab/View", "View_" + machineAssetId}});
          
            AddAsset<SpriteAtlas>("t:SpriteAtlas", Path.Combine(machineResPath, "Atlas"), groupLabels, settings,
                machineGroup,
                new Dictionary<string, string>
                {
                    {"Atlas", "Atlas_" + machineAssetId},
                });
             
            AddAsset<TextAsset>("t:TextAsset", Path.Combine(machineResPath, "Config"), groupLabels, settings, machineGroup,
                new Dictionary<string, string>
                {
                    {"Config", "Config_" + machineAssetId},
                });
        
            AddAsset<AudioClip>("t:AudioClip", Path.Combine(machineResPath, "Audio"), groupLabels, settings, machineGroup,
                new Dictionary<string, string>
                {
                    {"Audio", "Audio_" + machineAssetId},
                });
            
            AddAsset<Material>("t:Material", Path.Combine(machineResPath, "Material/AddToAddressable"), groupLabels, settings, machineGroup,
                null);
            
            AddAsset<AudioClip>("t:AudioClip", Path.Combine(machineResPath, "AudioOpt"), groupLabels, settings, machineGroup,
                new Dictionary<string, string>
                {
                    {"AudioOpt", "AudioOpt_" + machineAssetId},
                });

            #endregion

            #region 添加机器PreLoad

            AddressableAssetGroup preLoadGroup = GetOrCreateAssetGroup(settings, machinePreloadGroup);
            
            string machinePath = Directory.GetParent(Directory.GetParent(machineResPath).FullName).FullName;
            machinePath = WindowsTools.WindowsPath2CommonPath(machinePath);
            machinePath = machinePath.Replace(Application.dataPath, "Assets");
            
            string preLoadDir = Path.Combine(machinePath, $"PreLoad/Machine{machineAssetId}/Prefab");
            
            Debug.Log(preLoadDir);
            
            if (Directory.Exists(preLoadDir))
            {
                var p1Label = new List<string>() {"AssetPriorityLevel1"};
                
                AddSettingsLabels(settings, p1Label[0]);
                
                AddAsset<GameObject>("t:Prefab", preLoadDir, isLocal ? p1Label: null, settings,
                    preLoadGroup);
            }

            #endregion
            
            Debug.Log(string.Format("{0} Group 创建完成", groupName));
          
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
 
             //EditorUtility.DisplayDialog(groupName+" Group","创建完成", "确定");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            EditorUtility.ClearProgressBar();
            return false;
        }
    }

    public static void RefreshOrCreateSystemFolderAddressableConfig(string systemFolderPath, bool isLocal)
    {
            Debug.Log($"开始刷新目录[{systemFolderPath}]配置:");
            
            bool hasAssetsFolder = false;
            var systemFolderName = systemFolderPath.Substring(systemFolderPath.LastIndexOf("/") + 1);
            
            foreach (var folder in assetFolders)
            {
                if (AssetDatabase.IsValidFolder(systemFolderPath + "/" + folder))
                {
                    hasAssetsFolder = true;
                    break;
                }
            }

            bool hasPriorityFolder = false;
            
            for(int i = 1; i <= assetPriorityMaxLevel; i++)
            {
                if (AssetDatabase.IsValidFolder(systemFolderPath + $"/{systemFolderName}_P{i}"))
                {
                    hasPriorityFolder = true;
                }
            }

            if (hasAssetsFolder && hasPriorityFolder)
            {
                Debug.LogError($"目录[{systemFolderPath}]下子目录命名不符合规则:既存在资源类型目录，又存在资源优先级目录");
                return;
            }
            
            if (!hasAssetsFolder && !hasPriorityFolder)
            {
                Debug.LogError($"目录[{systemFolderPath}]下子目录命名不符合规则:既不存在资源类型目录，又不存在资源优先级目录");
                return;
            }

            if (hasAssetsFolder && !hasPriorityFolder)
            {
                
                CreateOrRefreshSystemFolderAssetGroup(systemFolderPath, systemFolderName,isLocal, 4);
            }
            else
            {
                var dataPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
                var directoryInfo = new DirectoryInfo($"{dataPath}/{systemFolderPath}");

                foreach (var directory in directoryInfo.GetDirectories())
                {
                    for (var i = 1; i <= assetPriorityMaxLevel; i++)
                        if (directory.Name.EndsWith($"_P{i}"))
                        {
                            CreateOrRefreshSystemFolderAssetGroup(systemFolderPath + "/" + directory.Name,
                                directory.Name, isLocal, i);
                        }
                }
            }
    }
    
    private static void CreateOrRefreshSystemFolderAssetGroup(string folderAssetPath, string folderName, bool isLocal, int assetsPriorityLevel)
    {
        string groupLabel = "SALabel_" + folderName;
        string collectionLabel  = "AssetPriorityLevel" + assetsPriorityLevel; 
        
        string groupName = $"SA_{folderName}";
        
        if(folderAssetPath.Contains("Activities"))
            groupName = $"AA_{folderName}";
        
        if (isLocal)
        {
            groupName = "In_Package_" + groupName;
        }

        bool isActivityFolder = folderAssetPath.Contains("UIActivities/");
        
        
        List<Tuple<string, string>> directlyReferenceAssetsFolders = new List<Tuple<string, string>>()
        {
            new Tuple<string, string>("Atlas", "t:SpriteAtlas"),
            new Tuple<string, string>("Audio", "t:AudioClip"),
            new Tuple<string, string>("Prefabs", "t:Prefab")
        };

        if (CheckFolderHasValidAssetsExist(folderAssetPath, directlyReferenceAssetsFolders))
        {
            AddressableAssetSettings settings = GetAddressableAssetSettings();
            AddressableAssetGroup assetGroup = GetOrCreateAssetGroup(settings, groupName + "_Group");
            
            ClearGroup(settings, assetGroup);
            
            
            var groupScheme = assetGroup.GetSchema<BundledAssetGroupSchema>();

            groupScheme.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.AppendHash;
            groupScheme.BuildPath.SetVariableByName(settings, "RemoteBuildPath");
            groupScheme.LoadPath.SetVariableByName(settings, "RemoteLoadPath");

            groupScheme.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
            groupScheme.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
            groupScheme.AssetBundledCacheClearBehavior =
                BundledAssetGroupSchema.CacheClearBehavior.ClearWhenWhenNewVersionLoaded;
 
            var fieldInfo2 = groupScheme.GetType()
                .GetField("m_BundledAssetProviderType", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo2?.SetValue(groupScheme, new SerializedType {Value = typeof(BundledAssetProvider)});
            var fieldInfo = groupScheme.GetType()
                .GetField("m_AssetBundleProviderType", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo?.SetValue(groupScheme, new SerializedType {Value = typeof(AssetBundleProvider)});

            AddSettingsLabels(settings, groupLabel);
            
            if(!isActivityFolder)
                AddSettingsLabels(settings, collectionLabel);

            var groupLabels = new List<string>(){groupLabel};
          
            if (!isActivityFolder)
            {
                groupLabels.Add(collectionLabel);
            }
            
            AddAsset<GameObject>("t:Prefab", Path.Combine(folderAssetPath, "Prefabs"), groupLabels, settings, assetGroup,
                null);
            AddAsset<SpriteAtlas>("t:SpriteAtlas", Path.Combine(folderAssetPath, "Atlas"), groupLabels, settings, assetGroup,
                null);
            AddAsset<AudioClip>("t:AudioClip", Path.Combine(folderAssetPath, "Audio"), groupLabels, settings, assetGroup,
                null);
            
            AddAsset<Texture>("t:Texture", Path.Combine(folderAssetPath, "Textures/AddToAddressable"),groupLabels, settings, assetGroup);
 
            if (AssetDatabase.IsValidFolder(Path.Combine(folderAssetPath, "Spine")))
            {
                CreateAssetEntry(settings, assetGroup, Path.Combine(folderAssetPath, "Spine"), groupLabels, $"{folderName}SpineAssets");
            }
            
            if (AssetDatabase.IsValidFolder(Path.Combine(folderAssetPath, "Animation")))
            {
                CreateAssetEntry(settings, assetGroup, Path.Combine(folderAssetPath, "Animation"), groupLabels, $"{folderName}AnimationAssets");
            }
 
            if (folderAssetPath != "Assets/PackageAssets/Common" && AssetDatabase.IsValidFolder(Path.Combine(folderAssetPath, "Particle")))
            {
                CreateAssetEntry(settings, assetGroup, Path.Combine(folderAssetPath, "Particle"), groupLabels, $"{folderName}Particle");
            }
            
            if (AssetDatabase.IsValidFolder(Path.Combine(folderAssetPath, "Font")))
            {
                CreateAssetEntry(settings, assetGroup, Path.Combine(folderAssetPath, "Font"), groupLabels, $"{folderName}Font");
            }
            
            if (folderAssetPath == "Assets/PackageAssets/Common")
            {
              //  CreateAssetEntry(settings, assetGroup, Path.Combine(folderAssetPath, "Font"), groupLabels, "FontCommon");
                
                var packIndex = 1;
                while (AssetDatabase.IsValidFolder(Path.Combine(folderAssetPath, "Particle/ParticlePack" + packIndex)))
                {
                    CreateAssetEntry(settings, assetGroup, Path.Combine(folderAssetPath, "Particle/ParticlePack" + packIndex), groupLabels, "ParticleCommonPack" + packIndex);
                    packIndex++;
                }
                
                groupLabels.Add("PlayerAvatar");
                
                AddAsset<Texture>("t:Texture", Path.Combine(folderAssetPath, "Textures/PlayerAvatar"), groupLabels, settings, assetGroup, null);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"目录{folderAssetPath}:没有有效的资源");
        }
    }

    private static bool CheckFolderHasValidAssetsExist(string folderAssetPath, List<Tuple<string, string>> searchArgs)
    {
        for (var i = 0; i < searchArgs.Count; i++)
        {
            var assetFolder = folderAssetPath + "/" + searchArgs[i].Item1;
            if (AssetDatabase.IsValidFolder(assetFolder))
            {
                string[] allPath = AssetDatabase.FindAssets(searchArgs[i].Item2, new string[] {assetFolder});
                
                if(allPath.Length > 0)
                {
                    return true;
                }

                // for (int c = 0; c < allPath.Length; c++)
                // {
                //     allPath[c] = AssetDatabase.GUIDToAssetPath(allPath[c]);
                //     Debug.Log(allPath[c]);
                // }
            }
        }
        
        return false;
    }
 
    private static AddressableAssetSettings GetAddressableAssetSettings()
    {
        AddressableAssetSettings settings =
            AssetDatabase.LoadAssetAtPath(Path.Combine(configFolder, configName), typeof(AddressableAssetSettings)) as
                AddressableAssetSettings;
        if (settings == null)
        {
            settings = AddressableAssetSettings.Create("Assets/AddressableAssetsData", "AddressableAssetSettings", true,
                true);
        }

        return settings;
    }

    private static AddressableAssetGroup GetOrCreateAssetGroup(AddressableAssetSettings settings, string groupName)
    {
        AddressableAssetGroup group = null;
        foreach (var addressableAssetGroup in settings.groups)
        {
            if (addressableAssetGroup != null && addressableAssetGroup.name == groupName)
            {
                group = addressableAssetGroup;
                break;
            }
        }

        if (group == null)
        {
            group = settings.CreateGroup(groupName, false, false, true, null, new Type[]
            {
                typeof(BundledAssetGroupSchema),
                typeof(ContentUpdateGroupSchema)
            });
        }
        
        return group;
    }


    [MenuItem("Assets/Addressable配置/清除空分组", false)]
    private static void RemoveEmptyAssetGroup()
    {
        var settings = GetAddressableAssetSettings();
        var groups = settings.groups;

        for (var i = groups.Count - 1; i >= 0; i--)
        {
            if (groups[i] != null && groups[i].entries.Count == 0)
            {
                settings.RemoveGroup(groups[i]);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void SortingAssetsGroup()
    {
        var settings = GetAddressableAssetSettings();
        var groups = settings.groups;
        groups.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });
    }


    private static void AddSettingsLabels(AddressableAssetSettings setting, string addressableLabel)
    {
        List<string> labels = setting.GetLabels();
        if (labels != null && labels.IndexOf(addressableLabel) < 0)
        {
            setting.AddLabel(addressableLabel);
        }
    }

    private static void AddAsset<T>(string filter, string resPath, List<string> groupLabels,
        AddressableAssetSettings settings, AddressableAssetGroup assetGroup,
        Dictionary<string, string> dicFolderGroup = null)
    {
        
        Dictionary<string, bool> dicHasCreateFolderGroup = null;

        if (dicFolderGroup != null)
        {
            dicHasCreateFolderGroup = new Dictionary<string, bool>();
            foreach (var item in dicFolderGroup)
            {
                dicHasCreateFolderGroup[item.Key] = false;
            }
        }

        string[] allPath = AssetDatabase.FindAssets(filter, new string[] {resPath});

        for (int i = 0; i < allPath.Length; i++)
        {
            allPath[i] = AssetDatabase.GUIDToAssetPath(allPath[i]);
        }

        for (int i = 0; i < allPath.Length; i++)
        {
            //string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
            string path = allPath[i];
            if(path.Contains("Ignore"))
                continue;
            if (dicFolderGroup != null)
            {
                bool shouldContinue = false;
            
                foreach (var item in dicHasCreateFolderGroup)
                {
                    if (path.IndexOf(item.Key) >= 0)
                    {
                        shouldContinue = true;
                        int index = item.Key.IndexOf("/") + 1;
                    
                        string strKey = string.Empty;
                        if (index != 0)
                        {
                            strKey = item.Key.Substring(index, item.Key.Length - index);
                        }

                        string directoryName = Path.Combine(resPath, strKey);
                    
                        if (!item.Value)
                        {
                            dicHasCreateFolderGroup[item.Key] = true;
                            CreateAssetEntry(settings, assetGroup, directoryName,
                                groupLabels, dicFolderGroup[item.Key]);
                            break;
                        }
                    }
                }
                
                if (shouldContinue) 
                        continue;
            }
            
            CreateAssetEntry(settings, assetGroup, path, groupLabels, Path.GetFileNameWithoutExtension(path));
        }
    }

    private static void CreateAssetEntry(AddressableAssetSettings settings, AddressableAssetGroup assetGroup,
        string filePath, List<string> groupLabels, string address)
    {
        try
        {
          
            var assetsEntries = assetGroup.entries;
           var iEnumerator = assetsEntries.GetEnumerator();
           while (iEnumerator.MoveNext())
           {
               if (iEnumerator.Current != null && iEnumerator.Current.address == address)
               {
                   assetGroup.RemoveAssetEntry(iEnumerator.Current);
                   break;
               }
           }
            
            AddressableAssetEntry assetEntry =
                settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(filePath), assetGroup);
            
            List<string> labels = settings.GetLabels();
              
            if (assetEntry != null)
            {
                for (var i = 0; i < labels.Count; i++)
                {
                    assetEntry.SetLabel(labels[i], false);
                }
                
                if (groupLabels != null && groupLabels.Count > 0)
                {
                    for (var i = 0; i < groupLabels.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(groupLabels[i]))
                        {
                            assetEntry.SetLabel(groupLabels[i], true);
                        }
                    }
                }
                
                assetEntry.address = address;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    static void ClearGroup(AddressableAssetSettings settings, AddressableAssetGroup assetGroup)
    {
        HashSet<AddressableAssetGroup> modifiedGroups = new HashSet<AddressableAssetGroup>();

        List<AddressableAssetEntry> list = new List<AddressableAssetEntry>();

        assetGroup.GatherAllAssets(list, true, true, true);

        foreach (var addressableAssetEntry in list)
        {
            //slotGroup.RemoveAssetEntry(addressableAssetEntry,true);
            modifiedGroups.Add(addressableAssetEntry.parentGroup);
            settings.RemoveAssetEntry(addressableAssetEntry.guid, false);
        }

        foreach (var g in modifiedGroups)
        {
            g.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, list, false, true);
            //AddressableAssetUtility.OpenAssetIfUsingVCIntegration(g);
        }

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, list, true, false);
    }
}