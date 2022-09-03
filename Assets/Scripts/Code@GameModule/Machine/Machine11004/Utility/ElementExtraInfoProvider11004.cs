using System;
using System.Collections.Generic;

namespace GameModule.Utility
{
    public class ElementExtraInfoProvider11004: ElementExtraInfoProvider
    {
        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            if (Constant11004.ListLionElementIds.Contains(id) ||
                Constant11004.ScatterElementId == id)
            {
                
                return true;
            }

            return false;
        }

        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11004.ScatterElementId == id)
            {
                return BlinkAnimationPlayStyleType.IdleCondition;
            }
            else if (Constant11004.DicIngotsCoinIds.ContainsKey(id))
            {
                return BlinkAnimationPlayStyleType.Idle;
            }

            return base.GetElementBlinkAnimationPlayStyleType(id);
        }


        public override int GetElementSortingOffset(uint id)
        {
            if (Constant11004.ScatterElementId == id ||
                Constant11004.ListLionElementIds.Contains(id))
            {
                return 100;
            }
            return base.GetElementSortingOffset(id);
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11004.ScatterElementId == id )
            {
                return 312;
            }
            else if (Constant11004.ScatterElementId == id ||
                Constant11004.ListLionElementIds.Contains(id))
            {
                return 200;
            }
            else if(Constant11004.DicIngotsCoinIds.ContainsKey(id) || 
                Constant11004.DicIngotsJackpotIds.ContainsKey(id))
            {
                return 3000;
            }

            return 0;
        }
        
        
        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11004.DicIngotsCoinIds.ContainsKey(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("winRate"))
                    {
                        elementConfig.extraInfo["winRate"] = (ulong)(int) extraInfoObject["winRate"];
                    }
                }
            }
        }
        
        
        public override Type GetElementClassType(uint id)
        {
            if (Constant11004.DicIngotsCoinIds.ContainsKey(id))
            {
                return typeof(ElementLink11004);
            }

            return typeof(Element11004);   
        }


        public override (float, float, float) GetElementStaticGrayLayerPrams(uint id)
        {
            if (!Constant11004.DicIngotsCoinIds.ContainsKey(id) &&
                !Constant11004.DicIngotsJackpotIds.ContainsKey(id))
            {
                return (0.65f, 0.58f, 1);
            }

            return base.GetElementStaticGrayLayerPrams(id);
        }


        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11004.ListLinkAllElementIds.Contains(id))
            {
                name = "J01";
            }

            return base.GetElementBlinkSoundName(id, name);
        }


        public override List<uint> GetElementBlinkVariantList(uint id)
        {
            if (Constant11004.ListLinkAllElementIds.Contains(id))
            {
                return Constant11004.ListLinkAllElementIds;
            }
			
            return base.GetElementBlinkVariantList(id);
        }


        
    }
}