// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/15/11:03
// Ver : 1.0.0
// Description : AssetsTools.cs
// ChangeLog :
// **********************************************


using System.IO;
using Newtonsoft.Json.Utilities;
using UnityEditor;
using UnityEngine;

public class AssetsTools
{
    // [MenuItem("Assets/移动资源到公共目录", false, 25)]
    // public static void MoveAssetToCommonFolder()
    // {
    //     string clickedAssetGuid = Selection.assetGUIDs[0];
    //     string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
    //     string clickedPathFull = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);
    //     
    //     
    // }
    //

    [MenuItem("Assets/图片尺寸减半", false, 25)]
    public static void HalfTextureSize()
    {
        ResizeTexture(0.5f);
    }
    
    [MenuItem("Assets/图片尺寸减80%", false, 26)]
    public static void ScaleToEightyPercentTextureSize()
    {
        ResizeTexture(0.8f);
    }
    
    [MenuItem("Assets/设置图片压缩格式为默认格式")]
    private static void SettingDefaultTextureFormat()
    {
        if (Selection.assetGUIDs.Length <= 0)
            return;
         
        for (var c = 0; c < Selection.assetGUIDs.Length; c++)
        {
            string clickedAssetGuid = Selection.assetGUIDs[c];
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

            for (var i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".png") || files[i].EndsWith(".jpg"))
                {
                    var selectPath = files[i].Substring(files[i].IndexOf("Assets/"));

                    if (selectPath.EndsWith(".png") || selectPath.EndsWith(".jpg"))
                    {
                        var tImporter = AssetImporter.GetAtPath(selectPath) as TextureImporter;

                        if (tImporter == null)
                        {
                            return;
                        }

                        tImporter.compressionQuality = 50;
                        tImporter.textureCompression =  TextureImporterCompression.Compressed;

                        TextureImporterPlatformSettings android = new TextureImporterPlatformSettings();
                        android.overridden = true;
                        android.name = "Android";
                        android.format = TextureImporterFormat.ETC2_RGBA8;
                        android.compressionQuality = 50;

                        tImporter.SetPlatformTextureSettings(android);

                        TextureImporterPlatformSettings ios = new TextureImporterPlatformSettings();
                        ios.overridden = true;
                        ios.name = "iPhone";
                        ios.format = TextureImporterFormat.ASTC_4x4;
                        ios.compressionQuality = 50;
                        tImporter.SetPlatformTextureSettings(ios);
                        
                        AssetDatabase.ImportAsset(selectPath);

                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }
    

    private static void ResizeTexture(float scaleFactor)
    {
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

        for (var i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".png") || files[i].EndsWith(".jpg"))
            {
                var selectPath = files[i].Substring(files[i].IndexOf("Assets/"));
                
                if (selectPath.EndsWith(".png") || selectPath.EndsWith(".jpg"))
                {
                    var tImporter = AssetImporter.GetAtPath(selectPath) as TextureImporter;

                    if (tImporter == null)
                    {
                        return;
                    }

                    TextureImporterType importerType = tImporter.textureType;
                    var warpMode = tImporter.wrapMode;

                    tImporter.textureType = TextureImporterType.Default;
                    tImporter.isReadable = true;
                    tImporter.spritePixelsPerUnit = (int)(tImporter.spritePixelsPerUnit * scaleFactor);

                    AssetDatabase.ImportAsset(selectPath);
                    AssetDatabase.Refresh();

                    var textureAsset = (Texture2D) AssetDatabase.LoadAssetAtPath<Texture2D>(selectPath);

                    Resize(textureAsset, (int) (textureAsset.width * scaleFactor), (int)(textureAsset.height * scaleFactor), false,
                        FilterMode.Trilinear);

                    byte[] bytes = selectPath.EndsWith(".png")
                        ? textureAsset.EncodeToPNG()
                        : textureAsset.EncodeToJPG();

                    var dirPath = Application.dataPath + selectPath.Substring(6);

                    System.IO.File.WriteAllBytes(dirPath, bytes);

                    tImporter.isReadable = false;
                    tImporter.textureType = importerType;
                    tImporter.wrapMode = warpMode;

                    AssetDatabase.ImportAsset(selectPath);
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                }
            }
        }
    }
    
    public static void Resize(Texture2D texture2D, int targetX, int targetY, bool mipmap =true, FilterMode filter = FilterMode.Bilinear)
    {
        //create a temporary RenderTexture with the target size
        RenderTexture rt = RenderTexture.GetTemporary(targetX, targetY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

        //set the active RenderTexture to the temporary texture so we can read from it
        RenderTexture.active = rt;
    
        //Copy the texture data on the GPU - this is where the magic happens [(;]
        Graphics.Blit(texture2D, rt);
        //resize the texture to the target values (this sets the pixel data as undefined)
        texture2D.Resize(targetX, targetY, texture2D.format, mipmap);
        texture2D.filterMode = filter;

        try
        {
            //reads the pixel values from the temporary RenderTexture onto the resized texture
            texture2D.ReadPixels(new Rect(0.0f, 0.0f, targetX, targetY), 0, 0);
            //actually upload the changed pixels to the graphics card
            texture2D.Apply();
        }
        catch
        {
            Debug.LogError("Read/Write is not enabled on texture "+ texture2D.name);
        }


        RenderTexture.ReleaseTemporary(rt);
    }
}
