//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 19:16
//  Ver : 1.0.0
//  Description : ElementExtraInfoProvider11011.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class ElementExtraInfoProvider11011: ElementExtraInfoProvider
    {
        public override Type GetElementClassType(uint id)
        {
            return typeof(Element11011);   
        }   
        
        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11011.IsLinkElement(id) || Constant11011.IsWrapElement(id))
            {
                return 50;
            }
            return 10;
        }
        
        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11011.IsLinkElement(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("winRate"))
                    {
                        elementConfig.extraInfo["winRate"] = (int) extraInfoObject["winRate"];
                    }
                }
            }
        }
        
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11011.IsLinkElement(id) || Constant11011.IsWrapElement(id))
            {
                return BlinkAnimationPlayStyleType.Idle;
            }
            return base.GetElementBlinkAnimationPlayStyleType(id);
        }
        
        public override (float, float, float) GetElementStaticGrayLayerPrams(uint id)
        {
            if (Constant11011.IsNormalElementId(id))
            {
                return (0.65f, 0.58f, 1);
            }
            return base.GetElementStaticGrayLayerPrams(id);
        }
        
        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11011.IsAddSpinElement(id))
            {
                return "+1Spin_Blink";
            }
            if (Constant11011.IsWrapAllElement(id))
            {
                return "B03_Blink";
            }
            if (Constant11011.IsWrapLockElement(id))
            {
                return "B02_Blink";
            }
            if (Constant11011.IsLinkElement(id))
            {
                return  "B01_Blink"; 
            }
            return base.GetElementBlinkSoundName(id, name);
        }
    }
}