//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-11 13:25
//  Ver : 1.0.0
//  Description : FindMissiongReferences.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class FindMissingReferences
{
    [MenuItem("Tools/Find Missing References")]
    public static void Find()
    {
        var go = Selection.activeGameObject;
        if (go == null)
        {
            Debug.LogError("Please select GameObject first");
            return;
        }

        var prefabType = PrefabUtility.GetPrefabAssetType(go);
        if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
        {
            AssetDatabase.OpenAsset(go);
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            go = Selection.activeGameObject;
        }

        var queue = new Queue<Transform>();
        queue.Enqueue(go.transform);

        while (queue.Count > 0)
        {
            var transform = queue.Dequeue();
            FindMissingReferencesInTransform(transform);

            for (int i = 0; i < transform.childCount; i++)
            {
                queue.Enqueue(transform.GetChild(i));
            }
        }
    }

    private static void FindMissingReferencesInTransform(Transform transform)
    {
        var components = transform.GetComponents<Component>();

        foreach (var component in components)
        {
            if (component == null)
            {
                ShowMissingScript(transform);
            }
            else
            {
                var so = new SerializedObject(component);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType != SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                    {
                        ShowMissingReference(transform, component, sp.name);
                    }
                }
            }
        }
    }

    private static void ShowMissingScript(Transform transform)
    {
        Debug.LogError("Missing script found on: " + GetGameObjectPath(transform), transform.gameObject);
    }

    private static void ShowMissingReference(Transform transform, Component component, string propertyName)
    {
        string message = string.Format("Missing reference found in: {0}, Component: {1}, Property : {2}",
            GetGameObjectPath(transform), component, propertyName);
        Debug.LogWarning(message, transform.gameObject);
    }

    private static string GetGameObjectPath(Transform transform)
    {
        var result = new StringBuilder(transform.name);
        while (transform.parent != null)
        {
            transform = transform.parent;
            result.Insert(0, transform.name + "/");
        }

        return result.ToString();
    }
}
