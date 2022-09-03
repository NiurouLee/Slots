using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;
using Object = UnityEngine.Object;

public class FindReferenceAssets : EditorWindow
{
    private const string MenuItemName = "Assets/查找引用当前资源的资源";
    private const string MetaExtension = ".meta";
    private Vector2 scrollPos = new Vector2();
    private static List<Object> findResults = new List<Object>();

    private static Process process;

    [MenuItem(MenuItemName, false, 25)]
    public static void FindReferenceAsset()
    {
        findResults = new List<Object>();

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

            FindReferenceAssets window =
                (FindReferenceAssets) EditorWindow.GetWindow(typeof(FindReferenceAssets), false,
                    "Find References", true);
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
      
        if (files[index].EndsWith(".meta") || files[index].EndsWith(".spriteatlas") || files[index].EndsWith(".DS_Store"))
        {
            FindReferenceByIndex(files, index + 1, callback);
            return;
        }
        
        var cancel  = EditorUtility.DisplayCancelableProgressBar("Process", files[index], (float) index /files.Count);
              
        if (cancel)
        {
            callback?.Invoke();
            return;
        }
        
        var relativePath = files[index].Substring(Application.dataPath.Length - 6);
        var ob = AssetDatabase.LoadAssetAtPath(relativePath, typeof(UnityEngine.Object));
              
        FindReference(ob, list =>
        {
            for (var i = 0; i < list.Count; i++)
            {
                findResults.Add(AssetDatabase.LoadAssetAtPath(list[i], typeof(Object)));
            }
            
            FindReferenceByIndex(files, index + 1, callback);
        });
    }

    public static void FindReference(UnityEngine.Object objectToFInd, Action<List<string>> findEndAction, string pathToFind = null)
    {
        bool isMacOS = Application.platform == RuntimePlatform.OSXEditor;
        int totalWaitMilliseconds = isMacOS ? 2 * 1000 : 300 * 1000;
        int cpuCount = Environment.ProcessorCount;
        
        string appDataPath = string.IsNullOrEmpty(pathToFind) ?  Application.dataPath: pathToFind;

        var selectedObject = objectToFInd;
        string selectedAssetPath = AssetDatabase.GetAssetPath(selectedObject);
        string selectedAssetGUID = AssetDatabase.AssetPathToGUID(selectedAssetPath);
        string selectedAssetMetaPath = selectedAssetPath + MetaExtension;

        
        UnityEngine.Debug.Log("selectedAssetGUID:" + selectedAssetGUID);
        var references = new List<string>();
        var output = new System.Text.StringBuilder();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var psi = new ProcessStartInfo();
        psi.WindowStyle = ProcessWindowStyle.Minimized;

        if (isMacOS)
        {
            psi.FileName = "/usr/bin/mdfind";
            psi.Arguments = string.Format("-onlyin {0} {1}", appDataPath, selectedAssetGUID);
        }
        else
        {
            psi.FileName = Path.Combine(Environment.CurrentDirectory, @"Tools\rg.exe");
            psi.Arguments = string.Format("--case-sensitive --follow --files-with-matches --no-text --fixed-strings " +
                                          "--ignore-file Assets/Editor/FindReferencesInProject2/ignore.txt " +
                                          "--threads {0} --regexp {1} -- {2}",
                cpuCount, selectedAssetGUID, appDataPath);
        }

        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

         
        process = new Process();
        
        process.StartInfo = psi;

        DataReceivedEventHandler outputDataReceived = (sender, e) =>
        {
            if (string.IsNullOrEmpty(e.Data))
                return;
            UnityEngine.Debug.Log("selectedAssetGUID:" + e.Data);
            
            string relativePath = e.Data.Replace(appDataPath, "Assets").Replace("\\", "/");

            // skip the meta file of whatever we have selected
            if (relativePath == selectedAssetMetaPath)
                return;

            references.Add(relativePath);
        };

        process.OutputDataReceived += outputDataReceived;
        
        DataReceivedEventHandler errorDataReceived =  (sender, e) =>
        {
            if (string.IsNullOrEmpty(e.Data))
                return;

            output.AppendLine("Error: " + e.Data);
        };

        process.ErrorDataReceived += errorDataReceived;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        
        var findReferences = new List<string>();

        foreach (string file in references)
        {
            string guid = AssetDatabase.AssetPathToGUID(file);
            output.AppendLine(string.Format("{0} {1}", guid, file));

            string assetPath = file;
            if (file.EndsWith(MetaExtension))
            {
                assetPath = file.Substring(0, file.Length - MetaExtension.Length);
            }

            findReferences.Add(assetPath);

            // UnityEngine.Debug.Log(string.Format("{0}\n{1}", file, guid), AssetDatabase.LoadMainAssetAtPath(assetPath));
        }

       // EditorUtility.ClearProgressBar();
        stopwatch.Stop();

        process.OutputDataReceived -= outputDataReceived;
        process.ErrorDataReceived -= errorDataReceived;
        
        findEndAction.Invoke(findReferences);

        // string content = string.Format(
        //     "{0} {1} found for object: \"{2}\" path: \"{3}\" guid: \"{4}\" total time: {5}s\n\n{6}",
        //     references.Count, references.Count > 2 ? "references" : "reference", selectedObject.name, selectedAssetPath,
        //     selectedAssetGUID, stopwatch.ElapsedMilliseconds / 1000d, output);
        // UnityEngine.Debug.LogWarning(content, selectedObject);
    }

    [MenuItem(MenuItemName, true)]
    private static bool FindValidate()
    {
        var obj = Selection.activeObject;
        if (obj != null && AssetDatabase.Contains(obj))
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return !AssetDatabase.IsValidFolder(path);
        }

        return false;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Result Count = " + findResults.Count);

        EditorGUILayout.LabelField("注意: 以下结果, 不包括动态引用, 和代码引用");

        EditorGUILayout.Space();
        // 显示结果
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < findResults.Count; i++)
        {
            EditorGUILayout.ObjectField(findResults[i], typeof(Object), true);
        }

        EditorGUILayout.EndScrollView();
    }
}