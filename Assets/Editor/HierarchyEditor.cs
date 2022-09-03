//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-07 11:00
//  Ver : 1.0.0
//  Description : HierarchyEditor.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyEditor
{
    static HierarchyEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
    }

    public static void HierarchyWindowItemCallback(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj != null && obj.CompareTag("Variable"))
        {
            Rect r = new Rect(selectionRect);
            r.x = r.width;
            var style = new GUIStyle();
            style.normal.textColor = Color.yellow;
            style.hover.textColor = Color.cyan;
            GUI.Label(r, "变量",style);
            EditorGUI.DrawRect(selectionRect, new Color(0.5f, 0.5f, 0f,0.3f));
        }
    }
}