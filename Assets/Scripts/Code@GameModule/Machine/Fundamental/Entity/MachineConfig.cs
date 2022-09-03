// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : MachineConfig.cs
// ChangeLog :
// **********************************************


using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class MachineConfig
    {
        public int machineId;
        public string machineName;

        public ElementConfigSet elementConfigSet;
        public Dictionary<string, WheelConfig> wheelConfigs;

        public IMachineAudioConfig audioConfig;
        
        /// <summary>
        /// 暂时使用SimpleRollUpdaterEasingConfig作为Value，如果以后有新的数学模型，再改成IRollUpdaterEasingConfig
        /// </summary>
        
        public Dictionary<string, SimpleRollUpdaterEasingConfig> easingConfigs;

        public MachineConfig()
        {
            elementConfigSet = new ElementConfigSet();
            easingConfigs = new Dictionary<string, SimpleRollUpdaterEasingConfig>();
            wheelConfigs = new Dictionary<string, WheelConfig>();
        }

        public MachineConfig(int inMachineId)
        {
            machineId = inMachineId;

            elementConfigSet = new ElementConfigSet();
            easingConfigs = new Dictionary<string, SimpleRollUpdaterEasingConfig>();
            wheelConfigs = new Dictionary<string, WheelConfig>();
        }

        public void PrepareElementConfig(RepeatedField<SymbolConfig> symbolConfigs, IElementExtraInfoProvider elementExtraInfoProvider)
        {
            if (symbolConfigs == null || symbolConfigs.Count == 0)
                return;
            
            for (var i = 0; i < symbolConfigs.count; i++)
            {
                if(symbolConfigs[i].Name.EndsWith("_wild"))
                    continue;
                
                var elementConfig = new ElementConfig(symbolConfigs[i].Id);
                
                //Config From Server;
                elementConfig.name = symbolConfigs[i].Name;
                elementConfig.height = symbolConfigs[i].Height;
                elementConfig.width = symbolConfigs[i].Width;
                elementConfig.position = symbolConfigs[i].Position;
                
                elementExtraInfoProvider.ParseElementExtra(elementConfig, symbolConfigs[i].Extras);
                 
                //Special Config From Client
                elementConfig.zOffset = elementExtraInfoProvider.GetElementSortingOffset(elementConfig.id);
 
                elementConfig.activeZOffset =
                    elementExtraInfoProvider.GetElementActiveSortingOffset(elementConfig.id);
                elementConfig.activeAssetName =
                    elementExtraInfoProvider.GetElementActiveAssetAddress(elementConfig.id, elementConfig.name);
                elementConfig.staticAssetName =
                    elementExtraInfoProvider.GetElementStaticAssetAddress(elementConfig.id, elementConfig.name);
                elementConfig.createBigElementParts =
                    elementExtraInfoProvider.GetElementCreateBigElementParts(elementConfig.id);
                elementConfig.elementClassType = elementExtraInfoProvider.GetElementClassType(elementConfig.id);

                elementConfig.defaultInteraction =
                    elementExtraInfoProvider.GetElementDefaultInteraction(elementConfig.id);

                elementConfig.blinkType =
                    elementExtraInfoProvider.GetElementBlinkAnimationPlayStyleType(elementConfig.id);

                elementConfig.activeStateInteraction =
                    elementExtraInfoProvider.GetElementActiveInteraction(elementConfig.id);

                elementConfig.keepStateWhenStopAllAnimation =
                    elementExtraInfoProvider.GetNeedKeepStateWhenStopAllAnimation(elementConfig.id);
                
                elementConfigSet.AddElementConfig(elementConfig);
                
                elementConfig.blinkSoundName = elementExtraInfoProvider.GetElementBlinkSoundName(elementConfig.id, elementConfig.name);

                elementConfig.staticGrayLayerPrams =
                    elementExtraInfoProvider.GetElementStaticGrayLayerPrams(elementConfig.id);
                elementConfig.staticGrayLayerMultiBlendModePrams = 
                    elementExtraInfoProvider.GetElementStaticGrayLayerMultiBlendModePrams(elementConfig.id);
                elementConfig.blinkSoundOrderId =  elementExtraInfoProvider.GetElementBlinkSoundOrderId(elementConfig.id);
            }
        }
        
        public ElementConfigSet GetElementConfigSet()
        {
            return elementConfigSet;
        }
        
        public SequenceElement GetSequenceElement(uint elementId, MachineContext context)
        {
            return new SequenceElement(elementConfigSet.GetElementConfig(elementId), context);
        }

        public void ClearEasingConfig()
        {
            easingConfigs.Clear();
        }
        
        public WheelConfig GetWheelConfig(string wheelName)
        {
            return wheelConfigs[wheelName];
        }
        
        public void AddEasingConfig(string key, IRollUpdaterEasingConfig easingConfig)
        {
            easingConfigs.Add(key, (SimpleRollUpdaterEasingConfig)easingConfig);
        }
    
        public IRollUpdaterEasingConfig GetEasingConfig(string configName)
        {
            return easingConfigs[configName] as IRollUpdaterEasingConfig;
        }
#if UNITY_EDITOR
        public void UpdateConfig(MachineConfig machineConfig)
        {
            this.easingConfigs = machineConfig.easingConfigs;
            this.wheelConfigs = machineConfig.wheelConfigs;
        }
#endif        
    }
}