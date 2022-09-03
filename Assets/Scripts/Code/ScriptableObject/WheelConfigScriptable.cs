using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;


public class WheelConfigScriptable : ScriptableBase
{
    public string GetWheelName()
    {
        return transform.gameObject.name;
    }
    
    
    #if UNITY_EDITOR
    
    public MachineConfigScriptable machineConfig;
    
    [LabelText("数据")]
    public WheelConfigItem wheelConfigItem;
    
    [Button("读取")]
    public void OnBtnRead()
    {
        if (machineConfig != null)
        {
            var wheelConfigJson = machineConfig.GetWheelConfig(transform.gameObject.name);
            if (wheelConfigJson == null)
            {
                wheelConfigJson = JsonConvert.SerializeObject(wheelConfigItem);
                machineConfig.UpdateWheelConfig(transform.gameObject.name, wheelConfigJson);
            }
            wheelConfigItem = JsonConvert.DeserializeObject<WheelConfigItem>(wheelConfigJson);
            wheelConfigItem.mapEasing = machineConfig.GetEasingScriptable();
        }
        else
        {
            wheelConfigItem = JsonConvert.DeserializeObject<WheelConfigItem>(dataJson);
        }
    }

    [Button("保存")]
    public void OnBtnSave()
    {
        
        wheelConfigItem.wheelName = GetWheelName();
        
        dataJson = JsonConvert.SerializeObject(wheelConfigItem);

        if (machineConfig != null)
        {
            machineConfig.UpdateWheelConfig(transform.gameObject.name, dataJson);
            machineConfig.SaveMachineConfig();
        }
    }
    
#endif
}


#if UNITY_EDITOR
[Serializable]
public class WheelConfigItem
{
    
    [JsonIgnore]
    [LabelText("节奏集合")]
    public SimpleRollUpdaterEasingScriptable mapEasing;
    
    [LabelText("轮盘名字")]
    public string wheelName = string.Empty;
    
    [LabelText("轮盘的列数")]
    public int rollCount = 5;

    [LabelText("轮盘的行数")]
    public int rollRowCount = 3;
    
    [LabelText("轮盘上图标最大高度")]
    public int elementMaxHeight = 1;
    
    [LabelText("轮带上方额外冗余数")]
    public int extraTopElementCount = 0;
 
    [LabelText("是否创建ReSpin轮盘")]
    public bool buildReSpinWheel = false;

    [LabelText("normal使用节奏索引")]
    [ValueDropdown("GetMapEasingNames")]
    public string normalEasingName;
    
    [LabelText("free使用节奏索引")]
    [ValueDropdown("GetMapEasingNames")]
    public string freeEasingName;
    
    [LabelText("reSpin使用节奏索引")]
    [ValueDropdown("GetMapEasingNames")]
    public string reSpinEasingName;
    
    [LabelText("special使用节奏索引")]
    [ValueDropdown("GetMapEasingNames")]
    public string specialEasingName;

    [LabelText("图标赢钱框的资源名")]
    public string winFrameAssetName = "WinFrame";
    
    [LabelText("赢钱线播放时长")]
    public float winLineBlinkDuration = 2;
    
    [LabelText("Bonus线动画播放时长")]
    public float bonusLineBlinkDuration = 2;
    
    [LabelText("动画资源名")]
    public string anticipationAnimationAssetName = "AnticipationAnimation";
    
    [LabelText("声音资源名")]
    public string anticipationSoundAssetName = "AnticipationSound";
     
    [LabelText("图标是否为上压下")]
    public bool topRowHasHighSortOrder = false;
    [LabelText("图标是否为左压右")]
    public bool leftColHasHighSortOrder = false;
    
    [LabelText("是各个格子独立转的轮盘")]
    public bool isIndependentWheel = false;
    [LabelText("赢钱线是否显示在图标上")]
    public bool isPayLineUpElement = false;
    
    public IEnumerable<string> GetMapEasingNames()
    {
        List<string> list = new List<string>();
        if (mapEasing != null)
        {
            foreach (var key in mapEasing.mapItem.Keys)
            {
                list.Add(key);
            }
        }
        return list;
    }
}

#endif
