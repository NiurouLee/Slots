using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScriptableBase : SerializedMonoBehaviour
{
    [ReadOnly]
    [LabelText("data数据")]
    public string dataJson;
    
    private void Awake()
    {
#if UNITY_EDITOR
        InitEditor();
#endif
    }
    
    
#if UNITY_EDITOR

    protected virtual void InitEditor()
    {
        GetSaveDuringPlay();
    }


    [OnValueChanged("SetSaveDuringPlay")]
    public bool isSaveDuringPlay = false;


    private void SetSaveDuringPlay()
    {
        UnityEditor.EditorPrefs.SetBool("SaveDuringPlay_Enabled", isSaveDuringPlay);
    }

    private void GetSaveDuringPlay()
    {
        isSaveDuringPlay = UnityEditor.EditorPrefs.GetBool("SaveDuringPlay_Enabled", false);
    }

    

#endif


}
