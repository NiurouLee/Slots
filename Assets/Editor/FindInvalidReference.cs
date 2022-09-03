using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Utilities;
using TMPro;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class FindInvalidReference : EditorWindow
{
    private const string MenuItemName = "Assets/查找非法引用";
    private const string MetaExtension = ".meta";
    private Vector2 scrollPos = new Vector2();
    private static Dictionary<Object, List<Object>> findResults = new Dictionary<Object, List<Object>>();

    /// <summary>
    /// key:oldGuid, value:newGuild,新旧guid映射表
    /// </summary>
    private static Dictionary<string, string> guidMapping;

    public string workFolder = "Machine11001";

    public string commonFolder = "Assets/PackageAssets/Common";

    private static List<string> checkAssetsName = new List<string>() { ".prefab", ".asset", ".mat", ".controller", ".anim", ".spriteatlas", ".FBX"};
    private static Process process;

    private static List<string> ignoreFolders = new List<string>()
    {
        "PackageAssets/Common/", "RemoteAssets/Machine/MachineCommon/", "/Spine/", "/spine/", "/UICommonReward/"
    };

    [MenuItem(MenuItemName, false, 25)]
    public static void FinInValidReference()
    {
        findResults = new Dictionary<Object, List<Object>>();

        guidMapping = new Dictionary<string, string>();
        
        string clickedAssetGuid = Selection.assetGUIDs[0];
        string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
        string clickedPathFull = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);

        FileAttributes attr = File.GetAttributes(clickedPathFull);
        bool isDirectory = attr.HasFlag(FileAttributes.Directory);

        string[] files;
        if (isDirectory)
        {
            files = Directory.GetFiles(clickedPathFull, "*", SearchOption.AllDirectories);
        }
        else
        {
            files = new[] {clickedPathFull};
        }

        string parttern = @"Machine\d{5}";

        if (Regex.IsMatch(clickedPath, parttern))
        {
            string subjectFolder = Regex.Match(clickedPath, parttern).Value;

            Debug.Log("SubjectFolder:" + subjectFolder);

            
            FindReferenceByIndex(files.ToList(), 0, () =>
            {
                EditorUtility.ClearProgressBar();

                FindInvalidReference window =
                    (FindInvalidReference) EditorWindow.GetWindow(typeof(FindInvalidReference), false,
                        "非法引用", true);
                window.workFolder = subjectFolder;
                window.Show();
            }, subjectFolder);
        }
       else
       {
           var destFolder = GetRootFolder(clickedPath);

           FindReferenceByIndex(files.ToList(), 0, () =>
           {
               EditorUtility.ClearProgressBar();

               FindInvalidReference window =
                   (FindInvalidReference) EditorWindow.GetWindow(typeof(FindInvalidReference), false,
                       "非法引用", true);
               window.workFolder = destFolder;
               window.Show();
           }, destFolder);
       }
    }

    public static bool IsIgnoreFolder(string assetsPath)
    {
        for (var i = 0; i < ignoreFolders.Count; i++)
        {
            if (assetsPath.Contains(ignoreFolders[i]))
            {
                return true;
            }
        }

        if (!assetsPath.Contains("RemoteAssets") && !assetsPath.Contains("PackageAssets"))
        {
            return true;
        }
        
        return false;
    }
    
    public static void FindReferenceByIndex(List<string> files, int index, Action callback, string folderName)
    {
        if (index >= files.Count)
        {
            callback.Invoke();
            return;
        }

        if (files[index].EndsWith(".meta"))
        {
            FindReferenceByIndex(files, index + 1, callback, folderName);
            return;
        }

        var assetPath = files[index].Replace(Application.dataPath, "Assets");

        var dependencies = AssetDatabase.GetDependencies(assetPath);


        if (dependencies.Length > 0)
        {
            var invalidObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

            bool isCheckAsset = false;
            foreach (var asset in checkAssetsName)
            {
                if (assetPath.EndsWith(asset))
                {
                    isCheckAsset = true;
                    break;
                }
            }

            if (isCheckAsset)
            {
                string text = System.IO.File.ReadAllText(files[index]);
                for (var i = 0; i < dependencies.Length; i++)
                {
                    if (!dependencies[i].Contains(folderName) && !IsIgnoreFolder(dependencies[i]))
                    {
                       
                        if (!dependencies[i].EndsWith(".cs") && !dependencies[i].EndsWith(".shader"))
                        {
                            Debug.Log(dependencies[i]);
                            var refObj = AssetDatabase.LoadAssetAtPath(dependencies[i], typeof(UnityEngine.Object));

                            var guid = AssetDatabase.AssetPathToGUID(dependencies[i]);

                            if (text == "" || text.Contains(guid))
                            {
                                if (!findResults.ContainsKey(invalidObj))
                                {
                                    findResults[invalidObj] = new List<Object>();
                                }

                                findResults[invalidObj].Add(refObj);
                            }
                        }
                    }
                }
            }
        }

        var cancel = EditorUtility.DisplayCancelableProgressBar("Process", files[index], (float) index / files.Count);

        if (cancel)
        {
            callback?.Invoke();
            return;
        }

        FindReferenceByIndex(files, index + 1, callback, folderName);
    }

    private void RefreshFindResult()
    {
        var tempDictionary = new Dictionary<Object, List<Object>>();

        foreach (var item in findResults)
        {
            var updateResult = RefreshDependencies(AssetDatabase.GetAssetPath(item.Key));
            if (updateResult.Key != null && updateResult.Value != null && updateResult.Value.Count > 0)
            {
                tempDictionary.Add(updateResult.Key, updateResult.Value);
            }
        }

        findResults = tempDictionary;
    }

    private KeyValuePair<Object, List<Object>> RefreshDependencies(string assetPath)
    {
        var dependencies = AssetDatabase.GetDependencies(assetPath);

        if (dependencies.Length > 0)
        {
            var invalidObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

            List<Object> invalidRefList = null;
            string text = "";

            if (assetPath.EndsWith(".mat") || assetPath.EndsWith(".prefab"))
            {
                text = System.IO.File.ReadAllText(Application.dataPath.Replace("/Assets", "/") + assetPath);
            }

            for (var i = 0; i < dependencies.Length; i++)
            {
                if (!dependencies[i].Contains(workFolder) &&  !IsIgnoreFolder(dependencies[i]))
                {
                    if (!dependencies[i].EndsWith(".cs") && !dependencies[i].EndsWith(".shader"))
                    {
                        Debug.Log(dependencies[i]);
                        var refObj = AssetDatabase.LoadAssetAtPath(dependencies[i], typeof(UnityEngine.Object));

                        var guid = AssetDatabase.AssetPathToGUID(dependencies[i]);

                        if (text == "" || text.Contains(guid))
                        {
                            if (invalidRefList == null)
                            {
                                invalidRefList = new List<Object>();
                            }

                            invalidRefList.Add(refObj);
                        }
                    }
                }
            }

            if (invalidRefList != null && invalidRefList.Count > 0)
            {
                return new KeyValuePair<Object, List<Object>>(invalidObj, invalidRefList);
            }
        }

        return default;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Result Count = " + findResults.Count);

        EditorGUILayout.LabelField("注意: 以下结果, 不包括动态引用, 和代码引用");

        EditorGUILayout.Space();
        // 显示结果
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var item in findResults)
        {
            var assetPath = AssetDatabase.GetAssetPath(item.Key);

            EditorGUILayout.ObjectField(item.Key, typeof(Object), true);
            EditorGUILayout.LabelField("Dependency:");

            for (var i = 0; i < item.Value.Count; i++)
            {
                EditorGUILayout.ObjectField(item.Value[i], typeof(Object), true);

                if (GUILayout.Button("解决依赖复制到当前目录"))
                {
                    bool result = ResolveInvalidDependencyByDuplicateObj(item.Key, item.Value[i]);
                    if (result)
                    {
                        RefreshFindResult();
                    }
                }

                if (GUILayout.Button("解决依赖移动到公共目录"))
                {
                    bool result = ResolveInvalidDependencyByMoveObj(item.Key, item.Value[i]);
                    if (result)
                    {
                        RefreshFindResult();
                    }
                }     
                
                if (GUILayout.Button("解决依赖移动到当前目录目录"))
                {
                    bool result = ResolveInvalidDependencyByMoveObjToDest(item.Key, item.Value[i]);
                    if (result)
                    {
                        RefreshFindResult();
                    }
                }
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();
    }

    private bool ResolveInvalidDependencyByMoveObj(Object mainObj, Object dependencyObj)
    {
        var assetPath = AssetDatabase.GetAssetPath(mainObj);

        string parttern = @"Machine\d{5}";

      //  if (Regex.IsMatch(assetPath, parttern))
        {
            var dependencyPath = AssetDatabase.GetAssetPath(dependencyObj);
            Debug.Log($"$ Move dependencyPath:{dependencyPath} to ${commonFolder}");
            return MoveObj(dependencyPath, commonFolder);
        }

        return false;
    }

    private static string GetRootFolder(string assetPath)
    {
        var destFolder = assetPath;

        if(assetPath.Contains("/Prefab"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Prefab"));
        }
        else if (assetPath.Contains("/Texture"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Texture"));
        }
        else if (assetPath.Contains("/Material"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Material"));
        }
        else if (assetPath.Contains("/Animation"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Animation"));
        }
        else if (assetPath.Contains("/Spine"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Spine"));
        }
        else if (assetPath.Contains("/spine"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Spine"));
        }
        else if (assetPath.Contains("/Audio"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Audio"));
        }

        return destFolder;
    }
    
    private bool ResolveInvalidDependencyByMoveObjToDest(Object mainObj, Object dependencyObj)
    {
        var assetPath = AssetDatabase.GetAssetPath(mainObj);

        string parttern = @"Machine\d{5}";

        var destFolder = GetRootFolder(assetPath);
 
        var dependencyPath = AssetDatabase.GetAssetPath(dependencyObj);

        return MoveObj(dependencyPath, destFolder);
        
       // return false;
    }


    private bool ResolveInvalidDependencyByDuplicateObj(Object mainObj, Object dependencyObj)
    {
        var assetPath = AssetDatabase.GetAssetPath(mainObj);
        var assetFullPath = Application.dataPath.Replace("/Assets", "/") + assetPath;

        string parttern = @"Machine\d{5}";

        if (Regex.IsMatch(assetPath, parttern))
        {
            var match = Regex.Match(assetPath, parttern);
            Debug.Log(match.Value);

            var targetRootFolder = assetPath.Substring(0, assetPath.IndexOf(match.Value) + match.Value.Length);

            var dependencyPath = AssetDatabase.GetAssetPath(dependencyObj);

            if (dependencyPath.EndsWith(".shader") || assetPath.EndsWith(".cs"))
            {
                //TODO 如果是shader暂时不复制，并且返回true来刷新
                return true;
            }
            return DuplicateObj(assetFullPath, dependencyPath, targetRootFolder);
        }
        else
        {
            var rootPath = GetRootFolder(assetPath);

            var dependencyPath = AssetDatabase.GetAssetPath(dependencyObj);

            return DuplicateObj(assetFullPath, dependencyPath, rootPath);
        }

        return false;
    }

    public static void CreateDirectoryFromAssetPath(string assetPath)
    {
        string directoryPath = Path.GetDirectoryName(assetPath);
        if (Directory.Exists(directoryPath))
            return;

        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }
    }

    private bool MoveObj(string assetPath, string destRootPath)
    {
        if (assetPath.EndsWith(".shader") || assetPath.EndsWith(".cs") || IsIgnoreFolder(assetPath))
        {
            return true;
        }
        
        var sourceAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        
        if (sourceAsset == null)
        {
            return true;
        }

        var destPath = GetDestPath(assetPath, destRootPath);

        if (GetRootFolder(assetPath) == GetRootFolder(destPath))
        {
            return true;
        }
        
        CreateDirectoryFromAssetPath(destPath);

        var originalGuid = AssetDatabase.AssetPathToGUID(destPath);

        if (string.IsNullOrEmpty(originalGuid))
        {
            var result = AssetDatabase.MoveAsset(assetPath, destPath);

            if (!string.IsNullOrEmpty(result))
            {
                Debug.Log($"$ Move assetPath:{assetPath} to ${destPath}:Failed Due To Asset Exist");
                return false;
            }
        }

        var dependencies = AssetDatabase.GetDependencies(destPath);

        var allAssetText = System.IO.File.ReadAllText(destPath);

        if (dependencies.Length > 0)
        {
            for (var i = 0; i < dependencies.Length; i++)
            {
                if (dependencies[i].EndsWith(".shader") || dependencies[i].EndsWith(".cs"))
                {
                    continue;
                }
                
                sourceAsset = AssetDatabase.LoadAssetAtPath<Object>(dependencies[i]);

                if (sourceAsset == null)
                {
                    continue;
                }
                
                
                var depGuid = AssetDatabase.AssetPathToGUID(dependencies[i]);

                if (allAssetText.Contains(depGuid))
                {
                    var depDestPath = GetDestPath(dependencies[i], destRootPath);
                    var guid = AssetDatabase.AssetPathToGUID(depDestPath);

                    if (string.IsNullOrEmpty(guid))
                    {
                        if (!MoveObj(dependencies[i], destRootPath))
                        {
                            Debug.Log($"$ Move assetPath:{assetPath} to ${destPath}:Failed Due To MoveObj");
                            return false;
                        }
                    }
                    else if(depDestPath != dependencies[i])
                    {
                        Debug.Log($"$ Move assetPath:{assetPath} to ${destPath}:Failed Due To depDestPath != dependencies[i]");
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private string GetDestPath(string assetPath, string destRootPath)
    {
        string destPath = "";

        var sourceAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        if (sourceAsset is Texture)
        {
            if (destRootPath.Contains("Machine/LazyLoad/"))
            {
                destPath = destRootPath + "/Texture";
            }
         
            if (assetPath.Contains("Particle"))
            {
                destPath = destRootPath + "/Particle";
            }
        }
        else if (sourceAsset is GameObject)
        {
            destPath = destRootPath + "/Prefabs";
        }
        else if (sourceAsset is Material)
        {
            destPath = destRootPath + "/Material";
        }
        else if (sourceAsset is AnimationClip || sourceAsset is RuntimeAnimatorController)
        {
            destPath = destRootPath + "/Animation";
        }
        else if (sourceAsset is Font || sourceAsset is TMP_FontAsset)
        {
            destPath = destRootPath + "/Font";
        }
        else if (sourceAsset is Shader)
        {
            destPath = destRootPath + "/Shader";
            //TODO 
        }
        else
        {
            Debug.LogError("NotSupported: Unknown Asset Type:" + assetPath);
            return null;
        }

        var fileName = assetPath.Substring(assetPath.LastIndexOf('/'));
        destPath += fileName;

        return destPath;
    }

    private bool DuplicateObj(string pathOfAssetReferenceCurrentAsset, string assetPath, string destRootPath)
    {
        var destPath = GetDestPath(assetPath, destRootPath);
         
        if (assetPath.EndsWith(".shader") || assetPath.EndsWith(".cs"))
        {
            //TODO 如果是shader暂时不复制，并且返回true来刷新
            return true;
        }
        if (string.IsNullOrEmpty(destPath))
            return false;

        var oldGuid = AssetDatabase.AssetPathToGUID(assetPath);
        if (guidMapping.ContainsKey(oldGuid))
        {
            //TODO 这里就不重复复制新资源，而是把已经复制出来的新资源guid挂在依赖资源上
            if (!string.IsNullOrEmpty(pathOfAssetReferenceCurrentAsset))
            {
                var text = System.IO.File.ReadAllText(pathOfAssetReferenceCurrentAsset);
                text = text.Replace(oldGuid, guidMapping[oldGuid]);
                System.IO.File.WriteAllText(pathOfAssetReferenceCurrentAsset, text);
                AssetDatabase.Refresh();
            }
            return true;
        }
        
        CreateDirectoryFromAssetPath(destPath);
        
        bool result = AssetDatabase.CopyAsset(assetPath, destPath);
        if (result)
        {
            if (!string.IsNullOrEmpty(pathOfAssetReferenceCurrentAsset))
            {
                var text = System.IO.File.ReadAllText(pathOfAssetReferenceCurrentAsset);
                var newGuid = AssetDatabase.AssetPathToGUID(destPath);

                guidMapping.Add(oldGuid, newGuid);
                text = text.Replace(oldGuid, newGuid);
                System.IO.File.WriteAllText(pathOfAssetReferenceCurrentAsset, text);
                AssetDatabase.Refresh();
            }
        }

        var dependencies = AssetDatabase.GetDependencies(destPath);

        var allAssetText = System.IO.File.ReadAllText(destPath);

        if (dependencies.Length > 0)
        {
            for (var i = 0; i < dependencies.Length; i++)
            {
                var depGuid = AssetDatabase.AssetPathToGUID(dependencies[i]);

                if (allAssetText.Contains(depGuid))
                {
                    if (!DuplicateObj(destPath, dependencies[i], destRootPath))
                    {
                        return false;
                    }
                }
            }
        }

        return result;
    }
}