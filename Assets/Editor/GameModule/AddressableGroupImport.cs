// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Reflection;
// using UnityEditor;
// using UnityEditor.AddressableAssets.Settings;
// using UnityEditor.AddressableAssets.Settings.GroupSchemas;
// using UnityEngine;
// using UnityEngine.ResourceManagement.ResourceProviders;
// using UnityEngine.ResourceManagement.Util;
// using UnityEngine.U2D;
//
// public class AddressableGroupImport
// {
//
//     private static string MachineStr = "Machine";
//     private static List<string> LocalList = new List<string>(){ "Slot_111001"};
//     private static List<string> ExcludeList = new List<string>() {"Machine11003"};
//
//
//
//     [MenuItem("AssetBundle/刷新所有Machine到Addressable")]
//     static void RefreshMachineFolder()
//     {
//         DirectoryInfo directoryInfo = new DirectoryInfo($"{Application.dataPath}/RemoteAssets/Machine/LazyLoad");
//
//         bool isLocal = false;
//         foreach (var directory in directoryInfo.GetDirectories())
//         {
//             isLocal = false;
//             if (LocalList.Contains(directory.Name))
//             {
//                 isLocal = true;
//             }
//
//             if (ExcludeList.Contains(directory.Name))
//             {
//                 continue;
//             }
//
//             string slotId = directory.Name.Replace(MachineStr,"");
//             
//             string unityPath = WindowsTools.WindowsPath2CommonPath(directory.FullName).Replace( Application.dataPath , "");
//             unityPath = $"Assets{unityPath}";
//             if (!CreateSlotAddressableGroup(isLocal,unityPath,slotId))
//             {
//                 EditorUtility.ClearProgressBar();
//                 return;   
//             }
//         }
//         
//         EditorUtility.ClearProgressBar();
//     }
//
//
//     static void ClearGroup(AddressableAssetSettings settings,AddressableAssetGroup slotGroup)
//     {
//         HashSet<AddressableAssetGroup> modifiedGroups = new HashSet<AddressableAssetGroup>();
//
//         
//          List<AddressableAssetEntry> list  = new List<AddressableAssetEntry>();
//          slotGroup.GatherAllAssets(list, true, true,true);
//          foreach (var addressableAssetEntry in list)
//          {
//              //slotGroup.RemoveAssetEntry(addressableAssetEntry,true);
//              modifiedGroups.Add(addressableAssetEntry.parentGroup);
//              settings.RemoveAssetEntry(addressableAssetEntry.guid,false);
//          }
//          foreach (var g in modifiedGroups)
//          {
//              g.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, list, false, true);
//              //AddressableAssetUtility.OpenAssetIfUsingVCIntegration(g);
//          }
//          settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, list, true, false);
//         
//       
//     }
//
//
//     private static int TotalStep = 5;
//     private static string BannerAndIcon_Group = "BannerAndIcon";
//     private static string configFolder = "Assets/AddressableAssetsData";
//     private static string configName = "AddressableAssetSettings.asset";
//     static bool CreateSlotAddressableGroup(bool isLocal,string slotResPath,string slotId)
//     {
//         try
//         {
//             // string assetGuid = Selection.assetGUIDs[0];
//             // string slotResPath = AssetDatabase.GUIDToAssetPath(assetGuid);
//             // string folderName = slotResPath.Substring(slotResPath.LastIndexOf("/")+1);
//             // string[] strList = folderName.Split('_');
//             // if (!CheckFolderValid(strList))
//             // {
//             //     EditorUtility.DisplayDialog("警告","请选择关卡根目录", "确定");
//             //     return;
//             // }
//
//             bool isCancel;
//
//             #region 创建关卡的Addressable Group
//             string groupLabel = $"{MachineStr}{slotId}";
//             string groupName = $"{MachineStr}{slotId}";
//             isCancel = EditorUtility.DisplayCancelableProgressBar(groupName+"Group","创建中...",0);
//             if (isCancel) return false;
//
//
//             AddressableAssetSettings settings = GetAddressableAssetSettings();
//
//             AddressableAssetGroup slotGroup = GetGroup(settings, groupName+"Group");
//             ClearGroup(settings,slotGroup);
//
//             //return false;
//             
//             BundledAssetGroupSchema slotGroupScheme = slotGroup.GetSchema<BundledAssetGroupSchema>();
//             if (isLocal)
//             {
//                 slotGroupScheme.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
//                 slotGroupScheme.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
//                 slotGroupScheme.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
//             }
//             else
//             {
//                 slotGroupScheme.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.AppendHash;
//                 slotGroupScheme.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteBuildPath);
//                 slotGroupScheme.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kRemoteLoadPath);
//             }
//             slotGroupScheme.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
//             slotGroupScheme.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
//             
//             
//             
//             var fieldInfo2 = slotGroupScheme.GetType().GetField("m_BundledAssetProviderType", BindingFlags.Instance |BindingFlags.NonPublic);
//             fieldInfo2?.SetValue(slotGroupScheme, new SerializedType { Value = typeof(BundledAssetProvider) });
//             var fieldInfo = slotGroupScheme.GetType().GetField("m_AssetBundleProviderType", BindingFlags.Instance |BindingFlags.NonPublic);
//             fieldInfo?.SetValue(slotGroupScheme, new SerializedType { Value = typeof(AssetBundleProvider) });
//             
//             AddSettingsLabels(settings, groupLabel);
//
//             string strTitle = groupName + "Group";
//             string strProgressTip = "{0} 创建中 {1}/{2}";
//             string txtShow = string.Format(strProgressTip, strTitle + "Prefab", 1, TotalStep);
//             Debug.Log(txtShow);
//             isCancel = EditorUtility.DisplayCancelableProgressBar(strTitle,txtShow,0.1f);
//             if (isCancel) return false;
//             AddAsset<GameObject>("t:Prefab", Path.Combine(slotResPath,"Prefab"), groupLabel, settings, slotGroup, 
//                 new Dictionary<string, string>{
//                     {"Prefab/Elements/Active","Active_Element_"+slotId},
//                     {"Prefab/Elements/Static","Static_Element_"+slotId},
//                     {"Prefab/PopUp","PopUp_"+slotId},
//                     {"Prefab/Misc","Misc_"+slotId},
//                     {"Prefab/View","View_"+slotId},
//                 });
//             
//             // AddAsset<GameObject>("t:Prefab", Path.Combine(slotResPath,"Particle"), groupLabel, settings, slotGroup, 
//             //     new Dictionary<string, string>{
//             //         {"Particle","Particle_"+slotId},
//             //     });
//             
//             txtShow = string.Format(strProgressTip, strTitle + " Atlas", 2, TotalStep);
//             Debug.Log(txtShow);
//             isCancel = EditorUtility.DisplayCancelableProgressBar(strTitle,txtShow,0.5f);
//             if (isCancel) return false;
//
//             if (isLocal)
//             {
//                 AddressableAssetGroup slotAtlasGroup = GetGroup(settings, $"{groupName} Atlas Group");
//                 BundledAssetGroupSchema slotAtlasGroupScheme = slotAtlasGroup.GetSchema<BundledAssetGroupSchema>();
//                 
//                 slotAtlasGroupScheme.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
//                 slotAtlasGroupScheme.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
//                 slotAtlasGroupScheme.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
//                 slotAtlasGroupScheme.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
//                 slotAtlasGroupScheme.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
//             
//             
//                 var fieldAtlasInfo2 = slotAtlasGroupScheme.GetType().GetField("m_BundledAssetProviderType", BindingFlags.Instance |BindingFlags.NonPublic);
//                 fieldAtlasInfo2?.SetValue(slotAtlasGroupScheme, new SerializedType { Value = typeof(BundledAssetProvider) });
//                 var fieldAtlasInfo = slotAtlasGroupScheme.GetType().GetField("m_AssetBundleProviderType", BindingFlags.Instance |BindingFlags.NonPublic);
//                 fieldAtlasInfo?.SetValue(slotAtlasGroupScheme, new SerializedType { Value = typeof(AssetBundleProvider) });
//
//                 AddAsset<SpriteAtlas>("t:SpriteAtlas", Path.Combine(slotResPath,"Atlas"), groupLabel, settings, slotAtlasGroup,
//                     new Dictionary<string, string>{
//                         {"Atlas","Atlas_"+slotId},
//                     });
//
//             }
//             else
//             {
//                 AddAsset<SpriteAtlas>("t:SpriteAtlas", Path.Combine(slotResPath,"Atlas"), groupLabel, settings, slotGroup,
//                     new Dictionary<string, string>{
//                         {"Atlas","Atlas_"+slotId},
//                     });
//             }
//             
//             txtShow = string.Format(strProgressTip, strTitle + "Config", 3, TotalStep);
//             Debug.Log(txtShow);
//             isCancel = EditorUtility.DisplayCancelableProgressBar(strTitle,txtShow,0.7f);
//             if (isCancel) return false;
//             AddAsset<TextAsset>("t:TextAsset", Path.Combine(slotResPath,"Config"), groupLabel, settings, slotGroup,
//                 new Dictionary<string, string>{
//                     {"Config","Config_"+slotId},
//                 });
//
//             txtShow = string.Format(strProgressTip, strTitle + "AudioClip", 4, TotalStep);
//             Debug.Log(txtShow);
//             isCancel = EditorUtility.DisplayCancelableProgressBar(strTitle,txtShow,0.9f);
//             if (isCancel) return false;
//             AddAsset<AudioClip>("t:AudioClip", Path.Combine(slotResPath,"Audio"), groupLabel, settings, slotGroup,
//                 new Dictionary<string, string>{
//                     {"Audio","Audio_"+slotId},
//                 });
//             AddAsset<AudioClip>("t:AudioClip", Path.Combine(slotResPath,"AudioOpt"), groupLabel, settings, slotGroup,
//                 new Dictionary<string, string>{
//                     {"AudioOpt","AudioOpt_"+slotId},
//                 });
//             #endregion
//
//             #region 添加机器PreLoad
//             
//             AddressableAssetGroup preLoadGroup = GetGroup(settings, BannerAndIcon_Group);
//             string machinePath = Directory.GetParent(Directory.GetParent(slotResPath).FullName).FullName;
//             machinePath = WindowsTools.WindowsPath2CommonPath(machinePath);
//             machinePath =   machinePath.Replace(Application.dataPath, "Assets");
//             string preLoadDir = Path.Combine(machinePath, $"PreLoad/Machine{slotId}/Prefab");
//             Debug.Log(preLoadDir);
//             if (Directory.Exists(preLoadDir))
//             {
//                 AddAsset<GameObject>("t:Prefab", preLoadDir, string.Empty, settings,
//                     preLoadGroup);   
//             }
//
//             #endregion
//             
//             
//             Debug.Log(string.Format("{0} Group 创建完成", groupName));
//             isCancel = EditorUtility.DisplayCancelableProgressBar(strTitle,"创建完成",1.0f);
//             if (isCancel) return false;
//             
//             
//             
//             
//             AssetDatabase.SaveAssets();
//             AssetDatabase.Refresh();
//             
//             //EditorUtility.DisplayDialog(groupName+" Group","创建完成", "确定");
//
//             return true;
//         }
//         catch (Exception e)
//         {
//             Debug.LogException(e);
//             EditorUtility.ClearProgressBar();
//             return false;
//         }
//
//     }
//
//     private static bool CheckFolderValid(string[] strList)
//     {
//         if (strList.Length < 2)
//         {
//             return false;
//         }
//
//         int num = 0;
//         bool isNumber = int.TryParse(strList[1], out num);
//         if (strList[0] != "Slot" || !isNumber)
//         {
//             return false;
//         }
//
//         return true;
//     }
//
//     private static AddressableAssetSettings GetAddressableAssetSettings()
//     {
//         AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath(Path.Combine(configFolder,configName), typeof(AddressableAssetSettings)) as AddressableAssetSettings;
//         if (settings == null)
//         {
//             settings = AddressableAssetSettings.Create("Assets/AddressableAssetsData", "AddressableAssetSettings",true,true);   
//         }
//
//         return settings;
//     }
//     
//     private static AddressableAssetGroup GetGroup(AddressableAssetSettings settings, string groupName)
//     {
//         AddressableAssetGroup slotGroup = null;
//         foreach (var addressableAssetGroup in settings.groups)
//         {
//             if (addressableAssetGroup!=null && addressableAssetGroup.name == groupName)
//             {
//                 slotGroup = addressableAssetGroup;
//                 break;
//             }
//         }
//
//         if (slotGroup == null)
//         {
//             slotGroup = settings.CreateGroup(groupName, false, false, true, null, new Type[]
//             {
//                 typeof(BundledAssetGroupSchema),
//                 typeof(ContentUpdateGroupSchema)
//             });
//         }
//         return slotGroup;
//     }
//     
//     private static void AddSettingsLabels(AddressableAssetSettings setting, string slotLabel)
//     {
//         List<string> labels = setting.GetLabels();
//         if (labels != null && labels.IndexOf(slotLabel)<0)
//         {
//             setting.AddLabel(slotLabel);
//         }
//     }
//     
//     private static  void AddAsset<T>(string filter, string slotResPath, string groupLabel, AddressableAssetSettings settings, AddressableAssetGroup slotGroup, Dictionary<string, string> dicFolderGroup=null)
//     {
//         List<string> listDirectoryName = new List<string>();
//         Dictionary<string, bool> dicHasCreateFolderGroup = null;
//         if (dicFolderGroup != null)
//         {
//             dicHasCreateFolderGroup = new Dictionary<string, bool>();
//             foreach (var item in dicFolderGroup)
//             {
//                 dicHasCreateFolderGroup[item.Key] = false;
//             }
//         }
//         string[] allPath = AssetDatabase.FindAssets(filter, new string[] { slotResPath});
//
//         for (int i = 0; i < allPath.Length; i++)
//         {
//             allPath[i] = AssetDatabase.GUIDToAssetPath(allPath[i]);
//         }
//         
//         for (int i = 0; i < allPath.Length; i++)
//         {
//             //string path = AssetDatabase.GUIDToAssetPath(allPath[i]);
//             string path = allPath[i];
//             AddressableAssetEntry slotAssetEntry = null;
//
//             bool isCreated = false;
//             
//             if (dicFolderGroup != null)
//             {
//                 bool shouldContinue = false;
//                 foreach (var item in dicHasCreateFolderGroup)
//                 {
//                     if (path.IndexOf(item.Key)>=0)
//                     {
//                         shouldContinue = true;
//                         //string directoryName = Path.GetDirectoryName(path);
//                         int index = item.Key.IndexOf("/")+1;
//                         string strKey = string.Empty;
//                         if (index != 0)
//                         {
//                             strKey = item.Key.Substring(index, item.Key.Length - index);
//                         }
//                         
//                         string directoryName = Path.Combine(slotResPath, strKey);
//                          if (!item.Value)
//                          {
//                              listDirectoryName.Add(directoryName);
//                             dicHasCreateFolderGroup[item.Key] = true;
//                             CreateSlotAssetEntry(settings, slotGroup,directoryName ,
//                                 groupLabel, dicFolderGroup[item.Key]);
//                             //isCreated = true;
//                             break;
//                          }
//                     }
//                 }
//
//                 if (shouldContinue) continue;
//             }
//
//             // if (!isCreated)
//             // {
//                 CreateSlotAssetEntry(settings, slotGroup, path, groupLabel, Path.GetFileNameWithoutExtension(path));
//             // }
//         }
//     }
//
//     private static void CreateSlotAssetEntry(AddressableAssetSettings settings, AddressableAssetGroup slotGroup, string filePath, string groupLabel, string address)
//     {
//         try
//         {
//             AddressableAssetEntry slotAssetEntry =  settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(filePath), slotGroup);
//             if (slotAssetEntry != null)
//             {
//                 if (!string.IsNullOrEmpty(groupLabel))
//                 {
//                     slotAssetEntry.SetLabel(groupLabel,true);   
//                 }
//                 slotAssetEntry.address = address;
//             }
//         }
//         catch (Exception e)
//         {
//             Debug.LogException(e);
//         }
//         
//     }
// }
