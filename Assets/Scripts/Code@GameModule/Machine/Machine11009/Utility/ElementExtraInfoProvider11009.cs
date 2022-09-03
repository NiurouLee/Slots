using System;
using System.Collections.Generic;

namespace GameModule.Utility
{
    public class ElementExtraInfoProvider11009: ElementExtraInfoProvider
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
                    if (extraInfoObject.Keys.Contains("winRate"))
                    {
                        elementConfig.extraInfo["winRate"] = (ulong)(int) extraInfoObject["winRate"];
                    }
                }
            }
        }
        
        
        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            if (Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                Constant11009.ListElementIdPurpleVaraint.Contains(id))
            {
                return true;
            }

            return false;
        }
        
        
        
        public override int GetElementSortingOffset(uint id)
        {
            
            if (Constant11009.ListCollectElementId.Contains(id) ||
                Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                Constant11009.ListElementIdPurpleVaraint.Contains(id) )
            {
                return 12;
            }

            return 0;
        }

        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11009.ListCollectElementId.Contains(id) ||
                Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                Constant11009.ListElementIdPurpleVaraint.Contains(id) )
            {
                return BlinkAnimationPlayStyleType.Idle;
            }
            return BlinkAnimationPlayStyleType.Default;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11009.ListCollectElementId.Contains(id) ||
                Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                Constant11009.ListElementIdPurpleVaraint.Contains(id) )
            {
                return 312;
            }

            return 300;
        }

        public override Type GetElementClassType(uint id)
        {
            if (Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                Constant11009.ListElementIdPurpleVaraint.Contains(id))
            {
                return typeof(ElementCoins11009);
            }

            return base.GetElementClassType(id);
        }
    }
    
}