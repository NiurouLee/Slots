using System;
using System.Collections.Generic;
using Machine.Machine11005.Utility;

namespace GameModule.Utility
{
    public class ElementExtraInfoProvider11005: ElementExtraInfoProvider
    {
        
        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (elementConfig.id >= 15 &&
                elementConfig.id <= 114)
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("letter"))
                    {
                        elementConfig.extraInfo["letter"] = (int) extraInfoObject["letter"];
                    }
                }
            }
        }

        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            if (id == Constant11005.ElementJackpot ||  Constant11005.ElementIdBoom == id)
            {
                return true;
            }

            return false;
        }

        public override Type GetElementClassType(uint id)
        {
            if (id == Constant11005.ElementJackpot)
            {
                return typeof(ElementJackpot11005);
            }
            return base.GetElementClassType(id);
        }
    }
}