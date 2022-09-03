using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11025 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11025()
		{

		}
		
		public override  int GetElementSortingOffset(uint id)
		{
			if (Constant11025.ValueList.Contains(id) || Constant11025.JackpotList.Contains(id))
				return 101;
			if (Constant11025.ScatterList.Contains(id))
				return 150;
			return 0;
		}

		public override int GetElementActiveSortingOffset(uint id)
		{
			if (Constant11025.ValueList.Contains(id) || Constant11025.JackpotList.Contains(id))
				return 600;
			if (Constant11025.ScatterList.Contains(id))
				return 6500;
			return 500;
		}
		
		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11025.ScatterList.Contains(id))
				return BlinkAnimationPlayStyleType.IdleCondition;
			return BlinkAnimationPlayStyleType.Default;
		}
		public override void  ParseElementExtra(ElementConfig elementConfig, string extraInfo)
		{
			if (Constant11025.ValueList.Contains(elementConfig.id))
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
			else if (Constant11025.JackpotList.Contains(elementConfig.id))
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
		public override Type GetElementClassType(uint id)
		{
			if (Constant11025.ValueList.Contains(id))
				return typeof(ElementValue11025);
			return base.GetElementClassType(id);
			// throw new Exception("存在异常symbol_id="+id);
		}
		
		public override string GetElementBlinkSoundName(uint id, string name)
		{
			if (Constant11025.ValueList.Contains(id) || Constant11025.JackpotList.Contains(id))
				return "J01_Blink";
			return base.GetElementBlinkSoundName(id, name);
		}
	}
}