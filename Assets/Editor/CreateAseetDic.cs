#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

public class CreateAseetDic
{


    [MenuItem("Assets/Utils/目录创建")]
    public static void CreateSlotDic()
    {
        string[] strs = Selection.assetGUIDs;

        string path = AssetDatabase.GUIDToAssetPath(strs[0]);



        string[] pathArray = new string[] {
            @"Animation/Bonus",
            @"Animation/Free",
            @"Animation/Panel",
            @"Animation/Symbol",
            @"Atlas",
            @"Config",
            @"Font",
            @"Materials",
            @"Model",
            @"Config",
            @"Prefab/Bonus",
            @"Prefab/Free",
            @"Prefab/Panel",
            @"Prefab/Symbol",
            @"Textures/Bonus",
            @"Textures/Free",
            @"Textures/Panel",
            @"Textures/Particle",
            @"Textures/Symbol",
        };


        if (!path.Contains(@"Slot/Slot_"))
        {
            return;
        }
        path=path.Substring(path.IndexOf(@"Slot/Slot_"));

        int slotId = 0;

        string[] temp = path.Split('_');

        if (temp.Length > 1 && int.TryParse(temp[1], out slotId))
        {
            string dicPath = Path.Combine(Application.dataPath, path+@"\");

            foreach (var dicName in pathArray)
            {
                string tempPath = Path.Combine(dicPath, dicName);

                if(!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                    Debug.Log(string.Format("创建文件夹{0}成功", tempPath));
                }
                else
                {
                    Debug.Log(string.Format("创建文件夹{0}失败,文件夹已存在", tempPath));
                    continue;
                }
            }

        }
        else
        {
            Debug.LogError(string.Format("Assets目录下文件路径{0}不合法",temp));
        }


    }


    [MenuItem("Assets/Utils/截屏")]
    public static void CaptureScreen()
    {
        string path = @"D://ScreenShot";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        try
        {
            ScreenCapture.CaptureScreenshot(path + "/" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png");
            Debug.Log("截屏成功!" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss"));
        }       
        catch (Exception e)
        {

            Debug.LogError(e);
            throw;
        }

    }


}
#endif