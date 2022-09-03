// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/07/14:47
// Ver : 1.0.0
// Description : FindAssetDependencies.cs
// ChangeLog :
// **********************************************
 
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class FindAssetDependencies : EditorWindow
{
    private const string MenuItemName = "Assets/查找当前资源引用的所有资源";
    private const string MetaExtension = ".meta";
    private Vector2 scrollPos = new Vector2();
    private static Dictionary<Object, List<Object>> findResults = new Dictionary<Object, List<Object>>();

    private static Process process;

    [MenuItem(MenuItemName, false, 25)]
    public static void FindReference()
    {
        findResults = new Dictionary<Object, List<Object>>();

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
        
        FindReferenceByIndex(files.ToList(), 0, () =>
        {
            EditorUtility.ClearProgressBar();

            FindAssetDependencies window =
                (FindAssetDependencies) EditorWindow.GetWindow(typeof(FindAssetDependencies), false,
                    "非法引用", true);
            window.Show();
        });
    }

    public static void FindReferenceByIndex(List<string> files, int index, Action callback)
    {
        if (index >= files.Count)
        {
            callback.Invoke();
            return;
        }
      
        if (files[index].EndsWith(".meta")){
            FindReferenceByIndex(files, index + 1, callback);
            return;
        }

        var assetPath = files[index].Replace(Application.dataPath,"Assets");

        var dependencies = AssetDatabase.GetDependencies(assetPath);

        if (dependencies.Length > 0)
        {
            var invalidObj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

            for (var i = 0; i < dependencies.Length; i++)
            {
                if (!dependencies[i].EndsWith(".cs") && !dependencies[i].EndsWith(".shader"))
                {
                    Debug.Log(dependencies[i]);
                    var refObj = AssetDatabase.LoadAssetAtPath(dependencies[i], typeof(UnityEngine.Object));
                    if (!findResults.ContainsKey(invalidObj))
                    {
                        findResults[invalidObj] = new List<Object>();
                    }

                    findResults[invalidObj].Add(refObj);
                }
            }
        }
        
        var cancel  = EditorUtility.DisplayCancelableProgressBar("Process", files[index], (float) index /files.Count);
              
        if (cancel)
        {
            callback?.Invoke();
            return;
        }
        
        FindReferenceByIndex(files, index + 1, callback);
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
            EditorGUILayout.ObjectField(item.Key, typeof(Object), true);
           
            EditorGUILayout.LabelField("Dependency:");
          
            for (var i = 0; i < item.Value.Count; i++)
            {
                EditorGUILayout.ObjectField(item.Value[i], typeof(Object), true);
            }
            if (GUILayout.Button("独立分组"))
            {
                SeparateAssetPath(item.Key);
            }
            EditorGUILayout.Space();
        }
        
        

        EditorGUILayout.EndScrollView();
    }

    // protected void GroupAssets(string assetsPath)
    // {
    //     foreach (var item in findResults)
    //     {
    //         EditorGUILayout.ObjectField(item.Key, typeof(Object), true);
    //        
    //         EditorGUILayout.LabelField("Dependency:");
    //       
    //         for (var i = 0; i < item.Value.Count; i++)
    //         {
    //             EditorGUILayout.ObjectField(item.Value[i], typeof(Object), true);
    //         }
    //
    //         if (GUILayout.Button("独立分组"))
    //         {
    //             SeparateAssetPath(item.Key);
    //         }
    //         EditorGUILayout.Space();
    //     }
    // }

    
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

    public void SeparateAssetPath(Object item)
    {
        var assetPath = AssetDatabase.GetAssetPath(item);
        var itemName = assetPath.Substring(assetPath.LastIndexOf("/"));

        var folderName = itemName.Substring(0, itemName.LastIndexOf("."));

        var rootPath = assetPath.Substring(0, assetPath.LastIndexOf("/Prefabs/"));
        string textureFolder = rootPath + "/Textures" + folderName;
        string materialFolder = rootPath + "/Material" + folderName;
        string animationFolder = rootPath + "/Animation" + folderName;

        var dependencies = findResults[item];

        for (var i = 0; i < dependencies.Count; i++)
        {
            var dPath = AssetDatabase.GetAssetPath(dependencies[i]);
            if (dPath.Contains(rootPath))
            {
                if(dPath.Contains("Spine"))
                    continue;
                
                if (dependencies[i] is Texture && !dPath.Contains("Font") && !dPath.Contains("Particle"))
                {
                    var name = dPath.Substring(dPath.LastIndexOf("/"));
                    var targetPath = textureFolder + name;
                    if (dPath != targetPath)
                    {
                        CreateDirectoryFromAssetPath(targetPath);
                        AssetDatabase.MoveAsset(dPath, targetPath);
                    }
                }
                else if (dependencies[i] is AnimationClip || dependencies[i] is RuntimeAnimatorController)
                {
                    var name = dPath.Substring(dPath.LastIndexOf("/"));
                    var targetPath = animationFolder + name;
                    if (dPath != targetPath)
                    {
                        CreateDirectoryFromAssetPath(targetPath);
                        AssetDatabase.MoveAsset(dPath, targetPath);
                    }
                }
                else if (dependencies[i] is Material)
                {
                    if (!dPath.Contains("Font"))
                    {
                        var name = dPath.Substring(dPath.LastIndexOf("/"));
                        var targetPath = materialFolder + name;
                        if (dPath != targetPath)
                        {
                            CreateDirectoryFromAssetPath(targetPath);
                            AssetDatabase.MoveAsset(dPath, targetPath);
                        }
                    }
                }
            }
        }
    }
    
    
}