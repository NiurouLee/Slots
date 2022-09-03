// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/16/11:04
// Ver : 1.0.0
// Description : ElementExtraInfoProvider11002.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class ElementExtraInfoProvider11001:ElementExtraInfoProvider
    {
        public override Type GetElementClassType(uint id)
        {
            if (Constant11001.coinElement.Contains(id))
            {
                return typeof(CoinElement11001);
            }
            
            return typeof(Element);
        }
        public override int GetElementSortingOffset(uint id)
        {
            if (id ==  14)
                return 16;
            return 0;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (id ==  14)
                return 6000;
            return 2999;
        }

        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11001.coinElement.Contains(id))
            {
                return "J01_Blink";
            }
            
            return base.GetElementBlinkSoundName(id, name);
        }


        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (id == Constant11001.b01)
                return BlinkAnimationPlayStyleType.IdleCondition;
            return BlinkAnimationPlayStyleType.Default;
        }

        public override int GetElementBlinkSoundOrderId(uint id)
        {
            if (id == Constant11001.b01)
            {
                return 1;
            }
            return base.GetElementBlinkSoundOrderId(id);
        }

        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11001.coinElement.Contains(elementConfig.id))
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
    }
}