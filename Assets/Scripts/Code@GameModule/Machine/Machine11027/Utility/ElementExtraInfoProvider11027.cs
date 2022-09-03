using System.Collections.Generic;
using System;

namespace GameModule
{
    public class ElementExtraInfoProvider11027 : ElementExtraInfoProvider
    {
        public ElementExtraInfoProvider11027()
        {
        }
        
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11027.ScatterElementId == id)
            {
                return BlinkAnimationPlayStyleType.Idle;
            }
        
            return base.GetElementBlinkAnimationPlayStyleType(id);
        }

        public override Type GetElementClassType(uint id)
        {
            if (Constant11027.ListWildElementIds.Contains(id))
            {
                return typeof(ElementCoin11027);
            }

            return typeof(Element11027);
        }
        
        public override bool CanShowElementAnticipation(uint id)
        {
            if (Constant11027.ScatterElementId == id)
            {
                 return true;
            }

            return false;
        }
        
        public override int GetElementSortingOffset(uint id)
        {
            if (Constant11027.ScatterElementId == id || Constant11027.ListWildElementIds.Contains(id))
            {
                return 51;
            }
        
            return 0;
        }
        
        public override int GetElementActiveSortingOffset(uint id)
        {
            return 101;
        }

        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11027.ListWildElementIds.Contains(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("winRate"))
                    {
                        elementConfig.extraInfo["winRate"] = (ulong) (int) extraInfoObject["winRate"];
                    }
                }
            }
        }
    }
}