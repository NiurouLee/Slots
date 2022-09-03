using UnityEngine;
using SimpleJson;
using UnityEditor;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using System.Collections.Generic;
using System;
using System.Reflection;
using Spine;

public class QuickPrase
{
    public static void ResolveEntity<T>(T entity, JsonObject jsonObject)
    {
        Type type = entity.GetType();
        FieldInfo[] fieldInfo = type.GetFields();

        for (var i = 0; i < fieldInfo.Length; i++)
        {
            if (jsonObject.ContainsKey(fieldInfo[i].Name))
            {
                Type fieldType = fieldInfo[i].FieldType;
                if (fieldType.IsPrimitive || fieldType == typeof(string))
                {
                    fieldInfo[i].SetValue(entity,
                        Convert.ChangeType(jsonObject[fieldInfo[i].Name], fieldInfo[i].FieldType));
                }
                else
                {
                    var fieldValue = Activator.CreateInstance(fieldType);
                    fieldInfo[i].SetValue(entity, fieldValue);
                    ResolveEntity(fieldInfo[i].GetValue(entity), jsonObject[fieldInfo[i].Name] as JsonObject);
                }
            }
        }
    }
}

public class EasingConfig
{
    public string easingType;

    public virtual JsonObject ToJsonObject()
    {
        var jsonObject = new JsonObject();
        jsonObject.Add("easingType", easingType);

        return jsonObject;
    }
}

public class CubicEasingConfig : EasingConfig
{
    public double control1X = 0;
    public double control1Y = 0;
    public double control2X = 1;
    public double control2Y = 1;
    public double easingStep = 10;
    public double easingDuration = 2;
    public double timeEnterSteady = 1.33333;
    public double steadyLeastStep = 5;
    public double steadyIncreaseStep = 5;
    public double drumIncreaseStep = 40;
    public double drumSpeedMultiplier = 2;
    public double earlyStopSpeedMultiplier = 5;
    public double enterSteadyRate = 0.5;
 
    private CubicBezier bezierA;
    private CubicBezier bezierB;
    public CubicEasingConfig()
    {
        easingType = "Cubic";
    }

    public override JsonObject ToJsonObject()
    {
        var jsonObject = base.ToJsonObject();
        jsonObject.Add("control1X", control1X);
        jsonObject.Add("control1Y", control1Y);
        jsonObject.Add("control2X", control2X);
        jsonObject.Add("control2Y", control2Y);
        jsonObject.Add("easingStep", easingStep);
        jsonObject.Add("easingDuration", easingDuration);
        jsonObject.Add("timeEnterSteady", enterSteadyRate * easingDuration);
        jsonObject.Add("steadyLeastStep", steadyLeastStep);
        jsonObject.Add("steadyIncreaseStep", steadyIncreaseStep);
        jsonObject.Add("drumIncreaseStep", drumIncreaseStep);
        jsonObject.Add("drumSpeedMultiplier", drumSpeedMultiplier);
        jsonObject.Add("earlyStopSpeedMultiplier", earlyStopSpeedMultiplier);

        return jsonObject;
    }
 
    public void UpdateConfig()
    {
        bezierA = new CubicBezier(control1X,control1Y,control2X,control2Y);
    }

    public void UpdateRate()
    {
        enterSteadyRate = timeEnterSteady / easingDuration;
    }
    
    public double GetSpeed(double time)
    {
        return bezierA.Slope(time) * easingStep / easingDuration;
    }
    public double GetY(double time)
    {
        return bezierA.Solve(time);
    }
    
}

public class WinEffectConfig
{
    public int winLevel;
    public string audioName;
    public string freeAudioName;
    public string stopAudioName;
    public string freeStopAudioName;
    
    public WinEffectConfig()
    {
        winLevel = 1;
        audioName = "win_effect_smallwin";
        freeAudioName = "win_effect_smallwin";
    }
    
    public WinEffectConfig(int inWinLevel, string inAudioName)
    {
        winLevel = inWinLevel;
        audioName = inAudioName;
        stopAudioName = "Term";
    }

    public JsonObject ToJsonObject()
    {
        var jsonObject = new JsonObject();
        jsonObject.Add("winLevel", winLevel);
        jsonObject.Add("audioName", audioName);
        
        if(freeAudioName != null)
            jsonObject.Add("freeAudioName", freeAudioName);
        jsonObject.Add("stopAudioName", stopAudioName);
      
        if(freeStopAudioName != null)
            jsonObject.Add("freeStopAudioName", freeStopAudioName);
        return jsonObject;
    }
}

public class PanelEasingConfig
{
    public int NormalEasingIndex = 0;
    public int FreeEasingIndex = 0;
    public int LinkEasingIndex  = 0;
    public int ExtraEasingIndex  = 0;

    public bool foldOut = true;
}

public class SlotRunTimeTools : MonoBehaviour
{
    public JsonObject machineConfig;
    private int subjectId = -1;

    public List<EasingConfig> easingConfig;

    public Dictionary<string, double> customDoubleProperty;
    public Dictionary<string, bool> customBooleanProperty;
    public Dictionary<string, List<double>> customListDoubleProperty;
    
    public List<PanelEasingConfig> panelEasingIndexList;
    public List<WinEffectConfig> winEffectConfigs;

    public int selectEasingType;
    public int selectIndex;
    
    public void SetMachineConfig(int inSubjectId, JsonObject inMachineConfig)
    {
        subjectId = inSubjectId;
        machineConfig = inMachineConfig;
        ParseEasingConfig();
        
        ParseCustomConfig();

        ParseWinLevelEffectConfig();
    }

    public bool IsMachineConfigLoaded()
    {
        return subjectId > 0 && machineConfig != null;
    }

    private void ParseCustomConfig()
    {
        customBooleanProperty = new Dictionary<string, bool>();
        customDoubleProperty = new Dictionary<string, double>();
        customListDoubleProperty = new Dictionary<string, List<double>>();

        if (machineConfig.ContainsKey("customProperty"))
        {
            var customProperty = machineConfig["customProperty"] as JsonObject;
            if (customProperty != null)
            {
                foreach (var propertyPair in customProperty)
                {
                    if (propertyPair.Value is bool)
                    {
                        customBooleanProperty.Add(propertyPair.Key, (bool) propertyPair.Value);
                    }
                    else if (propertyPair.Value is double || propertyPair.Value is long)
                    {
                        customDoubleProperty.Add(propertyPair.Key, (double) Convert.ChangeType(propertyPair.Value,typeof(double)));
                    }
                    else if (propertyPair.Value.GetType().IsGenericType &&
                             propertyPair.Value.GetType().GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var arrayProperty = customProperty[propertyPair.Key] as JsonArray;

                        if (arrayProperty?.Count > 0)
                        {
                            if (arrayProperty[0] is double)
                            {
                                var ls = new List<double>();

                                for (var i = 0; i < ls.Count; i++)
                                {
                                    ls.Add((double) arrayProperty[i]);
                                }

                                customListDoubleProperty.Add(propertyPair.Key, ls);
                            }
                        }
                    }
                }
            }
        }
    }

    private void ParseWinLevelEffectConfig()
    {
        if (machineConfig.ContainsKey("audioConfig"))
        {
            JsonObject audioConfig = machineConfig["audioConfig"] as JsonObject;

            if (audioConfig != null)
            {
                if (audioConfig.ContainsKey("winAudioConfig"))
                {
                    var winAudioConfig = audioConfig["winAudioConfig"] as JsonArray;
                    if (winAudioConfig != null)
                    {
                        winEffectConfigs = new List<WinEffectConfig>();
                        for (var i = 0; i < winAudioConfig.Count; i++)
                        {
                            WinEffectConfig config = new WinEffectConfig();
                            QuickPrase.ResolveEntity(config, winAudioConfig[i] as JsonObject);
                            winEffectConfigs.Add(config);
                        }
                    }
                }
            }
        }
        
        if (winEffectConfigs == null || winEffectConfigs.Count <= 0)
        {
            winEffectConfigs = new List<WinEffectConfig>();
            
            winEffectConfigs.Add(new WinEffectConfig(1, "MacRiffsWin1"));
            winEffectConfigs.Add(new WinEffectConfig(2, "MacRiffsWin2"));
            winEffectConfigs.Add(new WinEffectConfig(3, "MacRiffsWin3"));
            winEffectConfigs.Add(new WinEffectConfig(4, "MacRiffsWin4"));
        }
    }
    
    private void ParseEasingConfig()
    {
        if (machineConfig.ContainsKey("easingConfig"))
        {
            JsonArray easingConfigJson = machineConfig["easingConfig"] as JsonArray;
            if (easingConfigJson != null)
            {
                easingConfig = new List<EasingConfig>();
                for (var i = 0; i < easingConfigJson.Count; i++)
                {
                    var config = easingConfigJson[i] as JsonObject;
                    if (config.ContainsKey("easingType") && config["easingType"] as string == "Cubic")
                    {
                        var cubicConfig = new CubicEasingConfig();
                        QuickPrase.ResolveEntity(cubicConfig, config);
                        cubicConfig.UpdateRate();
                        easingConfig.Add(cubicConfig);
                    }
                    else
                    {
                        //TODO OTHER TYPE;
                    }
                }
            }
        }
        else
        {
            easingConfig = new List<EasingConfig>();
            CubicEasingConfig config = new CubicEasingConfig();
            easingConfig.Add(config);
        }

        selectIndex = 0;
        
        panelEasingIndexList = new List<PanelEasingConfig>();
        JsonArray panelConfigJson  = machineConfig["panels"] as JsonArray;
        if (panelConfigJson != null && panelConfigJson.Count > 0)
        {
            for (var i = 0; i < panelConfigJson.Count; i++)
            {
                var panelEasingConfig = new PanelEasingConfig();
                QuickPrase.ResolveEntity(panelEasingConfig, panelConfigJson[i] as JsonObject);
                panelEasingIndexList.Add(panelEasingConfig);
            }
        }
    }

    private JsonArray UnParseEasingConfig()
    {
        JsonArray easingConfigJson = new JsonArray();

        for (var i = 0; i < easingConfig.Count; i++)
        {
            easingConfigJson.Add(easingConfig[i].ToJsonObject());
        }

        return easingConfigJson;
    }
    
    private JsonArray UnParseWinEffectConfig()
    {
        var winEffectConfigsJson = new JsonArray();
        for (var i = 0; i < winEffectConfigs.Count; i++)
        {
            winEffectConfigsJson.Add(winEffectConfigs[i].ToJsonObject());
        }

        return winEffectConfigsJson;
    }

    private JsonObject UnParseCustomConfig()
    {
        var customProperty = machineConfig["customProperty"] as JsonObject;

        if (customProperty != null)
        {
            if (customBooleanProperty != null && customBooleanProperty.Count > 0)
            {
                foreach (var item in customBooleanProperty)
                {
                    customProperty[item.Key] = item.Value;
                }
            }

            if (customDoubleProperty != null && customDoubleProperty.Count > 0)
            {
                foreach (var item in customDoubleProperty)
                {
                    customProperty[item.Key] = item.Value;
                }
            }

            if (customListDoubleProperty != null && customListDoubleProperty.Count > 0)
            {
                foreach (var item in customListDoubleProperty)
                {
                    var jsonArray = new JsonArray();

                    for (var i = 0; i < item.Value.Count; i++)
                    {
                        jsonArray.Add(item.Value[i]);
                    }

                    customProperty[item.Key] = jsonArray;
                }
            }
        }

        return customProperty;
    }

    public void UpdatePanelEasingConfig()
    {
        JsonArray panelConfigJson  = machineConfig["panels"] as JsonArray;
        if (panelConfigJson != null)
        {
            for (var i = 0; i < panelEasingIndexList.Count; i++)
            {
                var panelConfig = panelConfigJson[i] as JsonObject;

                if (panelConfig != null)
                {
                    panelConfig["NormalEasingIndex"] = panelEasingIndexList[i].NormalEasingIndex;
                    panelConfig["FreeEasingIndex"] = panelEasingIndexList[i].FreeEasingIndex;
                    panelConfig["LinkEasingIndex"] = panelEasingIndexList[i].LinkEasingIndex;
                    panelConfig["ExtraEasingIndex"] = panelEasingIndexList[i].ExtraEasingIndex;
                }
            }
        }
    }

    public void UpdateConfig()
    {
        machineConfig["easingConfig"] = UnParseEasingConfig();
        machineConfig["customProperty"] = UnParseCustomConfig();
       
        if (machineConfig.ContainsKey("audioConfig"))
        {
            var audioConfig = (JsonObject) machineConfig["audioConfig"];
            if (audioConfig.ContainsKey("winAudioConfig"))
                audioConfig["winAudioConfig"] = UnParseWinEffectConfig();
            else
            {
                audioConfig.Add("winAudioConfig", UnParseWinEffectConfig());
            }
        }
        else
        {
            var audioConfig = new JsonObject();
            audioConfig.Add("winAudioConfig", UnParseWinEffectConfig());
            machineConfig.Add("audioConfig", audioConfig);
        }

        UpdatePanelEasingConfig();
        
        Debug.Log("New Config");
        Debug.Log(machineConfig.ToString());
    }
    
    public void ApplyNewConfig()
    {
#if HOT_FIX
            IType type = ILRuntimeHelp.appdomain.LoadedTypes["GameModule.SlotEditorGameModuleHelper"];
            //根据方法名称和参数个数获取方法
            IMethod method = type.GetMethod("ApplyMachineConfig", 0);

            int activeSubjectId = (int) ILRuntimeHelp.appdomain.Invoke(method, new object[]{machineConfig.ToString()});
#else
        var assembly = Assembly.Load("GameModuleAssembly");
        var type = assembly.GetType("GameModule.SlotEditorGameModuleHelper");
        var method = type.GetMethod("ApplyMachineConfig", BindingFlags.Public | BindingFlags.Static);
        method.Invoke(null, new object[]{machineConfig.ToString()});
#endif
    }
    
    public int GetMachineId()
    {
#if HOT_FIX
            IType type = ILRuntimeHelp.appdomain.LoadedTypes["GameModule.SlotEditorGameModuleHelper"];
            //根据方法名称和参数个数获取方法
            IMethod method = type.GetMethod("GetSubjectId", 0);

            int activeSubjectId = (int) ILRuntimeHelp.appdomain.Invoke(method, null);
#else
        var assembly = Assembly.Load("GameModuleAssembly");
        var type = assembly.GetType("GameModule.SlotEditorGameModuleHelper");
        var method = type.GetMethod("GetSubjectId", BindingFlags.Public | BindingFlags.Static);
        int activeSubjectId = (int) method.Invoke(null, null);
#endif
        return activeSubjectId;
    }
}