using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class ElementExtraInfoProvider11312 : ElementExtraInfoProvider
    {
		public override int GetElementSortingOffset(uint id)
		{
			if (Constant11312.ScSymbolId == id)
			{
				return 100;
			}
			if(Constant11312.AllListCoinElementId.Contains(id)){
				return 50;
			}

			return base.GetElementSortingOffset(id);
		}
        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
		{
			if (Constant11312.AllListCoinElementId.Contains(elementConfig.id) || Constant11312.AllListSmallBlueCoinElementId.Contains(elementConfig.id) ||Constant11312.AllListSmallGoldCoinElementId.Contains(elementConfig.id))
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
			if (Constant11312.AllListCoinElementId.Contains(id) || Constant11312.AllListSmallBlueCoinElementId.Contains(id) || Constant11312.AllListSmallGoldCoinElementId.Contains(id))
			{
				return typeof(ElementCoin11312);
			}
			return typeof(Element);
		}

		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
			if(id == Constant11312.ScSymbolId)
				return BlinkAnimationPlayStyleType.Idle;
			if(Constant11312.AllListCoinElementId.Contains(id))
				return BlinkAnimationPlayStyleType.Idle;
            return BlinkAnimationPlayStyleType.Default;
        }

		public override SpriteMaskInteraction GetElementActiveInteraction(uint id)
        {
			if(id == Constant11312.ScSymbolId)
				return SpriteMaskInteraction.VisibleInsideMask;
			// if(Constant11312.AllListCoinElementId.Contains(id))
			// 	return SpriteMaskInteraction.VisibleInsideMask;
            return SpriteMaskInteraction.None;
        }
    }
}

