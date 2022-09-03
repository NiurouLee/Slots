using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11030 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11030()
		{

		}

        public bool CheckElementNormal(uint id)
        {
            return Constant11030.NormalList.Contains(id);
        }
        public bool CheckElementTrain(uint id)
        {
            return Constant11030.TrainList.Contains(id);
        }
        public bool CheckElementValue(uint id)
        {
            return Constant11030.ValueList.Contains(id);
        }
        public bool CheckElementWild(uint id)
        {
            return Constant11030.WildList.Contains(id);
        }
        public bool CheckElementScatter(uint id)
        {
            return Constant11030.ScatterList.Contains(id);
        }
        public bool CheckElementStar(uint id)
        {
            return Constant11030.StarList.Contains(id);
        }
        public bool CheckElementGoldTrain(uint id)
        {
            return Constant11030.GoldTrainList.Contains(id);
        }
	    public override Type GetElementClassType(uint id)
        {
            if (CheckElementNormal(id))
                return typeof(ElementGray11030);
            if (CheckElementWild(id))
                return typeof(ElementWild11030); 
            if (CheckElementValue(id))
                return typeof(ElementValue11030);
            if (CheckElementTrain(id))
                return typeof(ElementTrain11030);
            if (CheckElementStar(id))
                return typeof(ElementStar11030);
            if (CheckElementGoldTrain(id))
                return typeof(ElementGoldTrain11030);
            if (CheckElementScatter(id))
                return typeof(ElementScatter11030); 
            throw new Exception("存在异常symbol_id="+id);
        }

        public override  int GetElementSortingOffset(uint id)
        {
            int extraSortingOrder = 0;
            if (CheckElementNormal(id))
                extraSortingOrder= 0;
            else if (CheckElementWild(id))
                extraSortingOrder= 10; 
            else if (CheckElementValue(id))
                extraSortingOrder= 70;
            else if (CheckElementTrain(id))
                extraSortingOrder= 80;
            else if (CheckElementStar(id))
                extraSortingOrder= 90;
            else if (CheckElementGoldTrain(id))
                extraSortingOrder= 100;
            else if (CheckElementScatter(id))
                extraSortingOrder= 110;
            else
                throw new Exception("存在异常symbol_id="+id);
            return 0 + extraSortingOrder;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            int extraSortingOrder = 0;
            if (CheckElementNormal(id))
                extraSortingOrder= 0;
            else if (CheckElementWild(id))
                extraSortingOrder= 10; 
            else if (CheckElementValue(id))
                extraSortingOrder= 70;
            else if (CheckElementTrain(id))
                extraSortingOrder= 80;
            else if (CheckElementStar(id))
                extraSortingOrder= 90;
            else if (CheckElementGoldTrain(id))
                extraSortingOrder= 100;
            else if (CheckElementScatter(id))
                extraSortingOrder= 110;
            else
                throw new Exception("存在异常symbol_id="+id);
            return 200 + extraSortingOrder;
        }
        public override string GetElementActiveAssetAddress(uint id, string name)
        {
            return "Active_" + name;
        }

        public override string GetElementStaticAssetAddress(uint id, string name)
        {
            return "Static_" + name;
        }

        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (CheckElementValue(id))
            {
                return "J05_Blink";
            }
            if (CheckElementTrain(id))
            {
                return "J01234_Blink";
            }
            if (CheckElementWild(id))
            {
                return "B01_Blink";
            }
            return base.GetElementBlinkSoundName(id, name);
        }
        
        
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11030.ScatterList.Contains(id))
                return BlinkAnimationPlayStyleType.IdleCondition;
            return BlinkAnimationPlayStyleType.Default;
        }
        
        public override SpriteMaskInteraction GetElementDefaultInteraction(uint id)
        {
            return SpriteMaskInteraction.VisibleInsideMask;
        }
        
        public override SpriteMaskInteraction GetElementActiveInteraction(uint id)
        {
            return SpriteMaskInteraction.None;
        }

        public override (float,float,float) GetElementStaticGrayLayerPrams(uint id)
        {
            if (CheckElementNormal(id) || CheckElementScatter(id) || CheckElementWild(id))
            {
                return (0f, 0.52f, 0.4f);
            }
            return (0,0,0);
        }

        public override void  ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (CheckElementTrain(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("trainId"))
                    {
                        elementConfig.extraInfo["trainId"] = (int) extraInfoObject["trainId"];
                    }
                }
            } else if (CheckElementValue(elementConfig.id))
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
        
        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            return false;
        }

        public override List<uint> GetElementBlinkVariantList(uint id)
        {
            if (Constant11030.ScatterAndWildList.Contains(id))
            {
                return Constant11030.ScatterAndWildList;
            }
            return base.GetElementBlinkVariantList(id);
        }

        public override int GetElementBlinkSoundOrderId(uint id)
        {
            return 0;
        }
        public override bool CanShowElementAnticipation(uint id)
        {
            return false;
        }

	}
}