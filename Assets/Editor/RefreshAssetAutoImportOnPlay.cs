//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-13 11:32
//  Ver : 1.0.0
//  Description : EnableAssetAutoImportOnPlay.cs
//  ChangeLog :
//  **********************************************

using UnityEditor;
using UnityEngine;

///Turn off AutoRefresh: Unity->Preferences->General->Auto Refresh
namespace UnityEditor.Utility
{
    [InitializeOnLoad]
    public class RefreshAssetAutoImportOnPlay
    {
        private const string kKeyOfAutoRefresh = "kAutoRefresh";

        static bool HasAutoRefresh () => EditorPrefs.GetBool(kKeyOfAutoRefresh);

        static RefreshAssetAutoImportOnPlay()
        {
            EditorApplication.playModeStateChanged += OnEditorApplicationPlayModeStateChanged;
        }

        private static void OnEditorApplicationPlayModeStateChanged(PlayModeStateChange playingState)
        {
            switch (playingState)
            {
                case PlayModeStateChange.ExitingEditMode:
                    if (!HasAutoRefresh())
                    {
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();   
                    }
                    break;
            }
        }
    }    
}
