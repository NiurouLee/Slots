// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/14/18:05
// Ver : 1.0.0
// Description : MachineConfigScriptable.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class MachineConfigScriptable : ScriptableBase
{
    public int machineId;
    
#if UNITY_EDITOR  
    //private Dictionary<string, int> dd;
    public event Action OnMachineConfigSet;

    private SimpleRollUpdaterEasingScriptable _simpleRollUpdaterEasingScriptable;
    // [LabelText("节奏集合")] 
    // private MachineConfigView machineConfigView;

    private string machineConfigJson;
   
    private Dictionary<string, object> _machineConfigObj;
    public void Awake()
    {
        var wheelConfigScriptable = transform.GetComponentsInChildren<WheelConfigScriptable>(true);
        
        if (wheelConfigScriptable != null)
        {
            for (var i = 0; i < wheelConfigScriptable.Length; i++)
            {
                wheelConfigScriptable[i].machineConfig = this;
                OnMachineConfigSet += wheelConfigScriptable[i].OnBtnRead;
            }
        }
        
        _simpleRollUpdaterEasingScriptable = transform.GetComponentInChildren<SimpleRollUpdaterEasingScriptable>();

        if (_simpleRollUpdaterEasingScriptable)
        {
            _simpleRollUpdaterEasingScriptable.machineConfig = this;
            OnMachineConfigSet += _simpleRollUpdaterEasingScriptable.OnBtnRead;
        }

        var machineConfigAsset = 
            AssetDatabase.LoadAssetAtPath($"Assets/RemoteAssets/Machine/LazyLoad/Machine{machineId}/Config/Machine{machineId}Config.txt",
                typeof(TextAsset)) as TextAsset;

         if (machineConfigAsset != null)
         {
             machineConfigJson = machineConfigAsset.text;
             _machineConfigObj = JsonConvert.DeserializeObject<Dictionary<string,object>>(machineConfigJson);
             OnMachineConfigSet?.Invoke();
         }
        else
         {
             _machineConfigObj = new Dictionary<string, object>();
             _machineConfigObj["wheelConfigs"] = new JObject();
             
            if (wheelConfigScriptable != null)
            {
                for (var i = 0; i < wheelConfigScriptable.Length; i++)
                {
                    wheelConfigScriptable[i].OnBtnSave();
                }
            }
            
            if (_simpleRollUpdaterEasingScriptable != null)
                _simpleRollUpdaterEasingScriptable.OnBtnSave();
        }
    }
 
    public void SaveMachineConfig(bool notify = true)
    {
        var configJson = JsonConvert.SerializeObject(_machineConfigObj);
        dataJson = configJson;
        
        string assetPath = $"Assets/RemoteAssets/Machine/LazyLoad/Machine{machineId}/Config/Machine{machineId}Config.txt";
    
        TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

        if (asset == null)
        {
            TextAsset configAsset = new TextAsset(configJson);
            AssetDatabase.CreateAsset(configAsset, assetPath);
        }
        else
        {
            File.WriteAllText(assetPath, configJson);
            EditorUtility.SetDirty(asset);
        }
         
        AssetDatabase.Refresh();

        if (notify)
        {
            var assembly = Assembly.Load("GameModuleAssembly");
            var type = assembly.GetType("GameModule.MachineEditorHelper");
            var method = type.GetMethod("ApplyMachineConfig", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
                method.Invoke(null, new object[] {dataJson});
        }
    }
    
    public string GetWheelConfig(string wheelName)
    {
        var wheelConfigs =  ((JObject) _machineConfigObj["wheelConfigs"]).ToObject<Dictionary<string, object>>();
        if (wheelConfigs != null)
        {
            if (wheelConfigs.ContainsKey(wheelName))
            {
                return JsonConvert.SerializeObject(wheelConfigs[wheelName]);
            }
        }
        return null;
    }

    public SimpleRollUpdaterEasingScriptable GetEasingScriptable()
    {
        return _simpleRollUpdaterEasingScriptable;
    }

    public void UpdateWheelConfig(string wheelName, string wheelConfigJson)
    {
        var wheelConfig = ((JObject) _machineConfigObj["wheelConfigs"]).ToObject<Dictionary<string, object>>();;
        if (wheelConfig != null)
        {
       
            if(wheelConfig.ContainsKey(wheelName))
                wheelConfig[wheelName] = JsonConvert.DeserializeObject<JObject>(wheelConfigJson);
            else
            {
                wheelConfig.Add(wheelName, JsonConvert.DeserializeObject<JObject>(wheelConfigJson));
            }

            _machineConfigObj["wheelConfigs"] = JObject.FromObject(wheelConfig);
         //   machineConfigObj["wheelConfigs"] = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(wheelConfig));
        }
    }

    public string GetEasingConfig()
    {
        var easingConfigs = _machineConfigObj["easingConfigs"];
        if (easingConfigs != null)
        {
            return JsonConvert.SerializeObject(easingConfigs);
        }
        
        return null;
    }
    
    public void UpdateEasingConfig(string easingConfigsJson)
    {
        if (_machineConfigObj.ContainsKey("easingConfigs"))
        {
            _machineConfigObj["easingConfigs"] = JsonConvert.DeserializeObject(easingConfigsJson);
        }
        else
        {
            _machineConfigObj.Add("easingConfigs",JsonConvert.DeserializeObject(easingConfigsJson));
        }
    }
#endif
}

public class MachineConfigView
{
    [LabelText("顶条资源名")]
    public string topPanelAssetName = "TopPanel";
    [LabelText("控制面板资源名")]
    public string controlPanelAssetName = "ControlPanel";
}
 