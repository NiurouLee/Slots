using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditor.UI;

public class CreateAtlas : EditorWindow
{
    [MenuItem("Assets/Atlas Tools/Create Atlas")]
    public static void Create()
    {
        CreateAtlas window = new CreateAtlas();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.ShowUtility();
    }

    bool variant = false;
    string atlasName = "MainUI";
    void OnGUI()
    {
        EditorGUILayout.Space(50);
        
        GUIStyle myStyle = new GUIStyle(GUI.skin.textField);
        myStyle.alignment = TextAnchor.MiddleCenter;
        atlasName = EditorGUILayout.TextField("AtlasName:", atlasName, myStyle);
        EditorGUILayout.Space(20);
        
        variant = EditorGUILayout.Toggle("Generate Variant", variant);
        
        EditorGUILayout.Space(30);
        if (GUILayout.Button("Create Atlas"))
            ClickOk();
    }

    private void ClickOk()
    {
        SpriteAtlas atlas = new SpriteAtlas();
        atlas.SetIncludeInBuild(false);
        AssetDatabase.CreateAsset(atlas, "Assets/Atlas/" + atlasName + ".spriteatlas");
        foreach (var obj in Selection.objects)
        {
            SpriteAtlasExtensions.Add(atlas, new Object[] { obj });
        }

        if(variant)
        {
            SpriteAtlas variantAtlas = new SpriteAtlas();
            variantAtlas.SetIsVariant(true);
            variantAtlas.SetVariantScale(0.5f);
            variantAtlas.SetMasterAtlas(atlas);
            variantAtlas.SetIncludeInBuild(false);
            AssetDatabase.CreateAsset(variantAtlas, "Assets/Atlas/" + atlasName + "_SD.spriteatlas");
            foreach (var obj in Selection.objects)
            {
                SpriteAtlasExtensions.Add(variantAtlas, new Object[] { obj });
            }
        }
        AssetDatabase.SaveAssets();
    }
}
