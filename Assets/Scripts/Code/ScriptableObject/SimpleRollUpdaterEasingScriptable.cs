using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;


public class SimpleRollUpdaterEasingScriptable : ScriptableBase
{
    
#if UNITY_EDITOR
    public MachineConfigScriptable machineConfig;
    
    [LabelText("节奏集合")]
    public Dictionary<string, SimpleReelUpdaterEasingItem> mapItem =
        new Dictionary<string, SimpleReelUpdaterEasingItem>();
     
    [Button("读取")]
    public void OnBtnRead()
    {
        if (machineConfig != null)
        {
            var easingConfigJson = machineConfig.GetEasingConfig();
            if (easingConfigJson == null)
            {
                easingConfigJson = JsonConvert.SerializeObject(mapItem);
                machineConfig.UpdateEasingConfig(easingConfigJson);
            }
            mapItem = JsonConvert.DeserializeObject<Dictionary<string, SimpleReelUpdaterEasingItem>>(easingConfigJson);
        }
        else
        {
            mapItem = JsonConvert.DeserializeObject<Dictionary<string, SimpleReelUpdaterEasingItem>>(dataJson);
        }
    }

    [Button("保存")]
    public void OnBtnSave()
    {
        dataJson = JsonConvert.SerializeObject(mapItem);
        
        if (machineConfig != null)
        {
            machineConfig.UpdateEasingConfig(dataJson);
            machineConfig.SaveMachineConfig();
        }
    }
#endif

}


#if UNITY_EDITOR

[Serializable]
public class SimpleReelUpdaterEasingItem
{
    [LabelText("滚轴滚动速度")]
    public float spinSpeed = 25f;
    [LabelText("期盼模式下的滚动速度")]
    public float anticipationSpeed = 25;
    [LabelText("期盼模式下额外滚动时间")]
    public float anticipationExtraTime = 1.0f;
    [LabelText("滚轴启动的加速时间")]
    public float speedUpDuration = 0.5f;
    [LabelText("滚轴停下减速的时间")]
    public float slowDownDuration = 0.5f;
    [LabelText("滚动时长")]
    public float leastSpinDuration = 2;
    [LabelText("减速需要滚动的步数")]
    public int slowDownStepCount = 2;
    [LabelText("滚轴启动时候回弹量")] 
    public float startOverShootAmount = 0f; 
    [LabelText("滚轴停下时候回弹量")]
    public float overShootAmount = 1.5f;
    [LabelText("相邻滚轴停下间隔时长")]
    public float stopIntervalTime = 0.2f;
    [LabelText("ReelStop音效果名称")]
    public string reelStopSoundName = "ReelStop";
    [LabelText("RollUpdater类型名")]
    public string updaterTypeName = "";
    [LabelText("停轮偏移距离最小值")]
    public float minStopMoveDistance = 0.3f;
    [LabelText("停轮偏移距离最大值")]
    public float maxStopMoveDistance = 0.7f;
    [LabelText("停轮偏移调回时间")]
    public float stopMoveDuration = 0.2f;
}

#endif