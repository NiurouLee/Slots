using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;


/// <summary>
/// 查找引用工具
/// </summary>
public class FindReferencesEditorWindow : EditorWindow
{
    private static Object findObj;
    private List<Object> findResults = new List<Object>();

    private Vector2 scrollPos = new Vector2();
    private bool checkPrefab = true;
    private bool checkScene = true;
    private bool checkMaterial = true;

    /// <summary>
    /// 查找引用
    /// </summary>
    [MenuItem("Tools/Find References")]
    public static void FindReferences()
    {
        FindReferencesEditorWindow window =
            (FindReferencesEditorWindow) EditorWindow.GetWindow(typeof(FindReferencesEditorWindow), false,
                "Find References", true);
        window.Show();
    }


    private void OnEnable()
    {
        if (Selection.objects.Length > 0)
        {
            findObj = Selection.objects[0];
        }
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        findObj = EditorGUILayout.ObjectField(findObj, typeof(Object), true);
        if (GUILayout.Button("Find", GUILayout.Width(100)))
        {
            findResults.Clear();
            if (findObj == null)
            {
                return;
            }

            //查找物件的路径及GUID
            string assetPath = AssetDatabase.GetAssetPath(findObj);
            string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);

            //过滤格式
            string filter = "";
            if (checkPrefab)
            {
                filter += "t:Prefab ";
            }

            if (checkScene)
            {
                filter += "t:Scene ";
            }

            if (checkMaterial)
            {
                filter += "t:Material ";
            }

            filter = filter.Trim();
            Debug.Log("Filter = " + filter);
            if (!string.IsNullOrEmpty(filter))
            {
                string[] guids = AssetDatabase.FindAssets(filter, new[] {"Assets"});
                int len = guids.Length;
                for (int i = 0; i < len; i++)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    bool cancel =
                        EditorUtility.DisplayCancelableProgressBar("Finding ...", filePath, i * 1.0f / len);
                    if (cancel)
                    {
                        break;
                    }

                    // 检查是否包含guid
                    try
                    {
                        // 某些文件读取会抛出异常
                        string content = File.ReadAllText(filePath);
                        if (content.Contains(assetGuid))
                        {
                            Object fileObject = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                            findResults.Add(fileObject);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning(filePath + "\n" + e.ToString());
                    }
                }

                EditorUtility.ClearProgressBar();
            }
        }

        EditorGUILayout.EndHorizontal();
        checkPrefab = EditorGUILayout.Toggle("查找 Prefab : ", checkPrefab);
        checkScene = EditorGUILayout.Toggle("查找 Scene : ", checkScene);
        checkMaterial = EditorGUILayout.Toggle("查找 Material : ", checkMaterial);
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

        EditorGUILayout.EndVertical();
    }
}
