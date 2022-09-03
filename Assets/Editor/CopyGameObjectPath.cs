using UnityEditor;
using UnityEngine;

public class CopyGameObjectPath
{
    [MenuItem("GameObject/复制层级路径", false, 0)]
    static public void CopyPath(MenuCommand menuCommand)
    {
        var path = Selection.activeGameObject.name;
        var parent = Selection.activeGameObject.transform.parent;
        while (parent != null)
        {
            if (parent.name.Equals("Canvas (Environment)"))
            {
                parent = null;
            }
            else
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
        }

        GUIUtility.systemCopyBuffer = path;
    }
    
    
    [MenuItem("GameObject/复制层级路径(NoRoot)", false, 0)]
    static public void CopyPathNoRoot(MenuCommand menuCommand)
    {
        var path = Selection.activeGameObject.name;
        var parent = Selection.activeGameObject.transform.parent;
        while (parent != null)
        {
            if (parent.name.Equals("Canvas (Environment)"))
            {
                parent = null;
            }
            else if(parent.parent==null)
            {
                break;
            }
            else
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
        }

        GUIUtility.systemCopyBuffer = path;
    }
    

    [MenuItem("Assets/复制资源路径", false, 0)]
    static public void CopyAssetPath()
    {
        var path = string.Empty;
        if (Selection.assetGUIDs != null && Selection.assetGUIDs.Length == 1)
        {
            path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        }

        GUIUtility.systemCopyBuffer = path;
    }
}