#if !SDK_ASSEMBLY && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SDKPostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, 
        string[] deletedAssets, 
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (var variaImportedAsset in importedAssets)
        {
            //Debug.LogError($"=========imported:{variaImportedAsset}");

            if (variaImportedAsset.Contains("DragonSDK.asmdef"))
            {
                AssetDatabase.DeleteAsset(variaImportedAsset);
                Debug.LogError($"=======DragonSDK.asmdef已被自动删除，想要启用，可以在项目预定义中添加SDK_ASSEMBLY,或者修改SDKPostprocessor脚本,然后重新导入");
                break;
            }

            
        }
        // foreach (var variaImportedAsset in deletedAssets)
        // {
        //     Debug.LogError($"=========deleted:{variaImportedAsset}");
        // }
        // foreach (var variaImportedAsset in movedAssets)
        // {
        //     Debug.LogError($"=========moved:{variaImportedAsset}");
        // }
        
        AssetDatabase.Refresh();
    }
}
#endif