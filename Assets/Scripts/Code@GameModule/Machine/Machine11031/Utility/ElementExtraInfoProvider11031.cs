using System.Collections.Generic;
using System;
using System.Linq;

namespace GameModule
{
    public class ElementExtraInfoProvider11031 : ElementExtraInfoProvider
    {
        public ElementExtraInfoProvider11031()
        {
        }

        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11031.TruckElementId == id)
            {
                return BlinkAnimationPlayStyleType.IdleCondition;
            }

            return base.GetElementBlinkAnimationPlayStyleType(id);
        }

        public override Type GetElementClassType(uint id)
        {
            if (Constant11031.TruckElementId == id)
            {
                return typeof(ElementTruck11031);
            }

            return typeof(Element11031);
        }

        public override (float, float, float) GetElementStaticGrayLayerPrams(uint id)
        {
            if (!Constant11031.ListChilliIds.Contains(id))
            {
                return (0.692f, 0.641f, 0.205f);
            }

            return base.GetElementStaticGrayLayerPrams(id);
        }

        public override List<uint> GetElementBlinkVariantList(uint id)
        {
            if (Constant11031.ListYellowChilli.Contains(id))
            {
                return Constant11031.ListYellowChilli;
            }

            return base.GetElementBlinkVariantList(id);
        }

        // public override int GetElementSortingOffset(uint id)
        // {
        //     if (Constant11031.TruckElementId == id)
        //         return 10;
        //     return 0;
        // }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11031.TruckElementId == id)
            {
                return 350;
            }

            if (Constant11031.ListChilliIds.Contains(id))
            {
                return 201;
            }

            return 200;
        }

        public override string GetElementBlinkSoundName(uint id, string name)
        {
            //红辣椒带jackpot
            if (Constant11031.ListRedChilliWithJackpot.Contains(id))
            {
                return "J06-J10_Blink";
            }

            if (Constant11031.ListGreenChilliWithJackpot.Contains(id))
            {
                return "J16-J20_Blink";
            }

            //所有黄辣椒 播放带渐变的J05_Blink01-J05_Blink05
            if (Constant11031.ListYellowChilli.Contains(id))
            {
                return "J05_Blink";
            }

            return base.GetElementBlinkSoundName(id, name);
        }

        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11031.ListChilliIds.Contains(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("jackpotId"))
                    {
                        elementConfig.extraInfo["jackpotId"] = (int) extraInfoObject["jackpotId"];
                    }
                }
            }
        }
    }
}