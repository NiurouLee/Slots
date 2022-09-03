using System.Collections.Generic;
using System;

namespace GameModule
{
    public class ElementExtraInfoProvider11029 : ElementExtraInfoProvider
    {
        public ElementExtraInfoProvider11029()
        {
        }
        
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11029.ListJSymbolElementIds.Contains(id))
            {
                return BlinkAnimationPlayStyleType.Idle;
            }

            return base.GetElementBlinkAnimationPlayStyleType(id);
        }

        public override int GetElementSortingOffset(uint id)
        {
            if (id == Constant11029.ScatterElementId)
            {
                return 350;
            }
            else if (Constant11029.ListJSymbolElementIds.Contains(id))
            {
                return 300;
            }
            else
            {
                return 0;
            }
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (id == Constant11029.ScatterElementId)
            {
                return 550;
            }
            else if (Constant11029.ListJSymbolElementIds.Contains(id))
            {
                return 500;
            }
            else
            {
                return 200;
            }
        }


        public override Type GetElementClassType(uint id)
        {
            if (Constant11029.ListJSymbolElementIds.Contains(id))
            {
                return typeof(ElementCoin11029);
            }

            return typeof(Element);
        }

        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11029.ListJSymbolElementIds.Contains(elementConfig.id))
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

        public override List<uint> GetElementBlinkVariantList(uint id)
        {
            if (Constant11029.ListJSymbolElementIds.Contains(id))
            {
                return Constant11029.ListJSymbolElementIds;
            }

            return base.GetElementBlinkVariantList(id);
        }
        
        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11029.ListJSymbolElementIds.Contains(id))
            {
                return "J02_Blink";
            }
            
            return base.GetElementBlinkSoundName(id, name);
        }
    }
}