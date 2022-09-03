using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11024 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11024()
		{

		}
		// public override bool GetElementCreateBigElementParts(uint id)
		// {
		// 	if (Constant11024.IsEmptyElement(id))
		// 		return true;
		// 	return false;
		// }
		public override  int GetElementSortingOffset(uint id)
		{
			if (Constant11024.IsValueId(id) || Constant11024.IsJackpotId(id))
				return 150;
			if (Constant11024.IsWildId(id))
				return 100;
			if (Constant11024.IsS01Id(id))
				return 50;
			return 0;
		}
		
		public override int GetElementActiveSortingOffset(uint id)
		{
			if (Constant11024.IsValueId(id) || Constant11024.IsJackpotId(id))
				return 600;
			if (Constant11024.IsWildId(id))
				return 500;
			if (Constant11024.IsS01Id(id))
				return 400;
			return 300;
		}
		
		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			return BlinkAnimationPlayStyleType.Default;
		}
		
		public override void  ParseElementExtra(ElementConfig elementConfig, string extraInfo)
		{
			if (Constant11024.IsValueId(elementConfig.id))
			{
				var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
				elementConfig.extraInfo = new Dictionary<string, object>();
				if (extraInfoObject != null && extraInfoObject.IsObject)
				{
					if (extraInfoObject.Keys.Contains("winRate"))
					{
						elementConfig.extraInfo["winRate"] = (int) extraInfoObject["winRate"];
					}
					if (extraInfoObject.Keys.Contains("colour"))
					{
						elementConfig.extraInfo["colour"] = (int) extraInfoObject["colour"];
					}
				}
			} 
			else if (Constant11024.IsJackpotId(elementConfig.id))
			{
				var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
				elementConfig.extraInfo = new Dictionary<string, object>();
				if (extraInfoObject != null && extraInfoObject.IsObject)
				{
					if (extraInfoObject.Keys.Contains("jackpotId"))
					{
						elementConfig.extraInfo["jackpotId"] = (int) extraInfoObject["jackpotId"];
					}
					if (extraInfoObject.Keys.Contains("colour"))
					{
						elementConfig.extraInfo["colour"] = (int) extraInfoObject["colour"];
					}
				}
			} 
		}
		public override Type GetElementClassType(uint id)
		{
			if (Constant11024.IsValueId(id))
			{
				if (Constant11024.IsGoldValue(id))
					return typeof(ElementGoldValue11024);
				return typeof(ElementValue11024);
			}

			if (Constant11024.IsGoldJackpot(id))
			{
				return typeof(ElementGoldJackpot11024);
			}

			if (Constant11024.IsAnyPigId(id) >= 0 || Constant11024.IsGoldId(id))
			{
				return typeof(ElementCoin11024);
			}
			if (Constant11024.IsAnyPigId(id) == -1 && !Constant11024.IsGoldId(id) && !Constant11024.IsEmptyElement(id))
			{
				return typeof(GrayElement11024);
			}
			return base.GetElementClassType(id);
		}
		public override (float,float,float) GetElementStaticGrayLayerPrams(uint id)
		{
			if (Constant11024.IsAnyPigId(id) == -1 && !Constant11024.IsGoldId(id) && !Constant11024.IsEmptyElement(id))
			{
				return (0f, 0.52f, 0.4f);
			}
			return (0,0,0);
		}

		public override Color GetElementStaticGrayLayerMultiBlendModePrams(uint id)
		{
			if (Constant11024.IsAnyPigId(id) == -1 && !Constant11024.IsGoldId(id) && !Constant11024.IsEmptyElement(id))
			{
				return new Color(45f,54f,92f,216.75f);
			}
			return new Color(0, 0, 0, 0);
		}
		public override string GetElementBlinkSoundName(uint id, string name)
		{
			if (Constant11024.IsValueId(id) || Constant11024.IsJackpotId(id))
				return "J01234_Blink";
			return base.GetElementBlinkSoundName(id, name);
		}
	}
}