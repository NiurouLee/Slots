using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine.U2D;

public class SpriteAtlasExporter : ScriptableObject
{
    static int currentMaxTextureSize;
    static TextureImporterFormat currentTIFormat;
    static string logTitle = "ChangeTextureImportSettings. ";


    [MenuItem ("Tools/设置关卡iPhone图集格式")]
    static void ChangeTextureFormat_Android_ASTC_4x4()
    { 
       SelectedChangeAnyPlatformSettings(TextureImporterFormat.ETC2_RGBA8,false,"Android");
       SelectedChangeAnyPlatformSettings(TextureImporterFormat.ASTC_RGB_4x4,true,"iPhone");
    }

    /// <summary>
    /// Main work method
    /// </summary>
    static void SelectedChangeAnyPlatformSettings(TextureImporterFormat newFormat, bool overridden, string platform)
    {
       int processingTexturesNumber;
       var spriteAtlases = GetSelectedTextures();
       processingTexturesNumber = spriteAtlases.Count;
       if (processingTexturesNumber == 0)
       {
         Debug.LogWarning(logTitle + "Nothing to do. Please select objects/folders with 2d textures in Project tab");
         return;
       }
       AssetDatabase.StartAssetEditing();
       foreach (var item in spriteAtlases)
       {
          TextureImporterPlatformSettings platformSettings = SpriteAtlasExtensions.GetPlatformSettings(item.Key, platform);
          platformSettings.format = newFormat;
          platformSettings.overridden = overridden;
          item.Key.SetPlatformSettings(platformSettings);
          
          EditorUtility.SetDirty(item.Key);
          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();
          AssetDatabase.ImportAsset(item.Value, ImportAssetOptions.ForceUpdate);
       }
       AssetDatabase.StopAssetEditing();
       Debug.Log("Textures processed: " + processingTexturesNumber);
    }
 
    static Dictionary<SpriteAtlas,string> GetSelectedTextures()
    {
       Dictionary<SpriteAtlas, string> atlasAssets = new Dictionary<SpriteAtlas, string>();
       string[] atFindAssets = AssetDatabase.FindAssets("t:Spriteatlas", new[] { "Assets/RemoteAssets" });
       for(int i=0; i<atFindAssets.Length; i++)
       {
          string path = AssetDatabase.GUIDToAssetPath(atFindAssets[i]);
          atlasAssets.Add(AssetDatabase.LoadMainAssetAtPath(path) as SpriteAtlas, path);
       }
       return atlasAssets; 
    }
}