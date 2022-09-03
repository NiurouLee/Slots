// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/19/14:56
// Ver : 1.0.0
// Description : AtlasTool.cs
// ChangeLog :
// **********************************************


using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasTool
{
    
    //默认的PackSetting 
    private static SpriteAtlasPackingSettings GetDefaultPackSetting()
    {
        var packSetting = new SpriteAtlasPackingSettings()
        {
            blockOffset = 1,
            enableRotation = false,
            enableTightPacking = false,
            padding = 4,
        };
        return packSetting;
    }
    /// 默认的TextureSetting
    private static SpriteAtlasTextureSettings GetDefaultTextureSetting()
    {
        var textureSetting = new SpriteAtlasTextureSettings()
        {
            readable = false,
            generateMipMaps = false,
            sRGB = true,
            filterMode = FilterMode.Bilinear,
        };
        return textureSetting;
    }

    /// <summary>
    /// 默认平台设置
    /// </summary>
    /// <returns></returns>
    private static TextureImporterPlatformSettings GetDefaultPlatformSetting()
    {
        var platformSetting = new TextureImporterPlatformSettings()
        {
            maxTextureSize = 2048,
            format = TextureImporterFormat.Automatic,
            crunchedCompression = false,
            textureCompression = TextureImporterCompression.Compressed,
            compressionQuality = 50,
        };
        return platformSetting;
    }
    /// <summary>
    /// 默认Ios平台设置
    /// </summary>
    /// <returns></returns>
    private static TextureImporterPlatformSettings GetIosOverridePlatformSetting()
    {
        var platformSetting = new TextureImporterPlatformSettings()
        {
            name = "iPhone", 
            overridden = true,
            maxTextureSize = 2048,
            format = TextureImporterFormat.ASTC_4x4,
            crunchedCompression = false,
            compressionQuality = 50,
        };
        
        return platformSetting;
    }
    
    /// <summary>
    /// 默认Ios平台设置
    /// </summary>
    /// <returns></returns>
    private static TextureImporterPlatformSettings GetHighQualityPlatformSetting()
    {
        var platformSetting = new TextureImporterPlatformSettings()
        {
            overridden = false,
            maxTextureSize = 2048,
            crunchedCompression = false,
            compressionQuality = 50,
            textureCompression = TextureImporterCompression.Uncompressed,
        };
        
        return platformSetting;
    }
  
    static SpriteAtlas CreateNormalAtlas(string assetPath)
    {
        SpriteAtlas atlas = new SpriteAtlas();
        // 设置参数 可根据项目具体情况进行设置
        atlas.SetPackingSettings(GetDefaultPackSetting());

        atlas.SetTextureSettings(GetDefaultTextureSetting());
        
        atlas.SetPlatformSettings(GetDefaultPlatformSetting());

        atlas.SetPlatformSettings(GetIosOverridePlatformSetting());

        AssetDatabase.CreateAsset(atlas, assetPath);

        return atlas;
    }

    static SpriteAtlas GetOrCreateAtlas(string atlasPath, bool highQuality)
    {
        var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
        if (spriteAtlas == null)
        {
            spriteAtlas = CreateNormalAtlas(atlasPath);
        }
 
        if (highQuality)
        {
            spriteAtlas.SetPlatformSettings(GetHighQualityPlatformSetting());
            var iosOverride = GetIosOverridePlatformSetting();
            iosOverride.overridden = false;
            spriteAtlas.SetPlatformSettings(iosOverride);
        }
        
        return spriteAtlas;
    }
    
    
    private static string GetRootFolder(string assetPath)
    {
        var destFolder = "";
       
        if (assetPath.Contains("/Texture"))
        {
            destFolder = assetPath.Substring(0, assetPath.LastIndexOf("/Texture"));
        }
        
        return destFolder;
    }
    
    [MenuItem("Assets/打包成图集", true)]
    private static bool FindValidate()
    {
        if (Selection.assetGUIDs.Length > 0)
        {
            var guid = Selection.assetGUIDs[0];
            if (!string.IsNullOrEmpty(guid))
            {
                string clickedPath = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.IsValidFolder(clickedPath);
            }
        }

        return false;
    }

    [MenuItem("Assets/打包成图集",false)]
   
    static void PackTexture()
    {
        string clickedAssetGuid = Selection.assetGUIDs[0];
        string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
        string clickedPathFull = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);

        FileAttributes attr = File.GetAttributes(clickedPathFull);
        bool isDirectory = attr.HasFlag(FileAttributes.Directory);
        if (!isDirectory)
        {
            EditorUtility.DisplayDialog("Attention Please",
                $"只针对文件夹生效:{clickedPath}不是文件，或没有包含图片文件", "Ok");
        }
        else
        {
           PackTexture(clickedPath,false);
        }
    }
    
    [MenuItem("Assets/打包成高质量图集",false)]
   
    static void PackHighQualityTexture()
    {
        string clickedAssetGuid = Selection.assetGUIDs[0];
        string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
        string clickedPathFull = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);

        FileAttributes attr = File.GetAttributes(clickedPathFull);
        bool isDirectory = attr.HasFlag(FileAttributes.Directory);
        if (!isDirectory)
        {
            EditorUtility.DisplayDialog("Attention Please",
                $"只针对文件夹生效:{clickedPath}不是文件，或没有包含图片文件", "Ok");
        }
        else
        {
            PackTexture(clickedPath,true);
        }
    }

    public static void PackTexture(string clickedPath, bool highQuality)
    {
        var rootFolder = GetRootFolder(clickedPath);
        if (rootFolder != "")
        {
            var pathName = clickedPath.Substring(clickedPath.LastIndexOf("/") + 1);

            var atlasPath = rootFolder + "/Atlas/" + pathName + "Atlas.spriteAtlas";
            CreateDirectoryFromAssetPath(atlasPath);

            PackingTextureInDirectory(clickedPath, atlasPath, highQuality);
        }
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
 
    static void PackingTextureInDirectory(string directory, string atlasPath, bool highQuality)
    {
        var atlas = GetOrCreateAtlas(atlasPath, highQuality);
        var packables = atlas.GetPackables();
      
        if (packables.Length > 0)
        {
            atlas.Remove(packables);
        }

        if (Directory.Exists($"{directory}/PackInAtlas"))
        {
            Object obj = AssetDatabase.LoadAssetAtPath($"{directory}/PackInAtlas", typeof(Object));
            atlas.Add(new[] {obj});
            AssetDatabase.SaveAssets();
            return;
        }
        else
        {
            Object obj = AssetDatabase.LoadAssetAtPath($"{directory}", typeof(Object));
            atlas.Add(new[] {obj});
            AssetDatabase.SaveAssets();
            
            // DirectoryInfo dir = new DirectoryInfo(directory);
            //
            // FileInfo[] files = dir.GetFiles("*.png");
            //
            // foreach (FileInfo file in files)
            // {
            //     atlas.Add(new[] {AssetDatabase.LoadAssetAtPath<Sprite>($"{directory}/{file.Name}")});
            // }
            //AssetDatabase.SaveAssets();
        }
        
      
    }
}