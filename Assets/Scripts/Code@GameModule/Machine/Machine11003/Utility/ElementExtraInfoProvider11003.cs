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
using System.Linq;

namespace GameModule
{
    public class ElementExtraInfoProvider11003 : ElementExtraInfoProvider
    {
        public override Type GetElementClassType(uint id)
        {
            if (Constant11003.coinElement.Contains(id))
            {
                return typeof(CoinElement11003);
            }

            if (Constant11003.piggyElement.Contains(id))
            {
                return typeof(PigElement);
            }

            if (Constant11003.jackPotElement.Contains(id))
            {
                return typeof(JackpotElement11003);
            }

            return typeof(Element);
        }


        public override int GetElementSortingOffset(uint id)
        {
            if (Constant11003.piggyElement.Contains(id) || Constant11003.coinElement.Contains(id))
            {
                return 12;
            }

            return 0;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11003.piggyElement.Contains(id))
            {
                return 312;
            }

            if (Constant11003.jackPotElement.Contains(id))
            {
                return 200;
            }

            return 300;
        }

        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            if (Constant11003.piggyElement.Contains(id))
            {
                return true;
            }
            
            return base.GetNeedKeepStateWhenStopAllAnimation(id);
        }

        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11003.piggyElement.Contains(id))
            {
                return "B02_Blink";
            }

            if (Constant11003.coinElement.Contains(id))
            {
                return "B01_Blink";
            }
            
            return base.GetElementBlinkSoundName(id, name);
        }

        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11003.coinElement.Contains(elementConfig.id))
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
            } else if (Constant11003.piggyElement.Contains(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("count"))
                    {
                        elementConfig.extraInfo["count"] = (int) extraInfoObject["count"];
                    }
                }
            }
        }
    }
}