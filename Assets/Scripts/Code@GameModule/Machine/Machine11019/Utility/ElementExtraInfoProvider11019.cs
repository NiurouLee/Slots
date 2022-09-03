using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11019 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11019()
		{

		}
		
		
		
		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11019.ScatterElementId == id)
			{
				return BlinkAnimationPlayStyleType.IdleCondition;
			}
			else if (Constant11019.ListBonusElementIds.Contains(id) ||
			         Constant11019.ListBonusJackpotElementIds.Contains(id))
			{
				return BlinkAnimationPlayStyleType.Idle;
			}

			return base.GetElementBlinkAnimationPlayStyleType(id);
		}
		
		
		
		public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
		{
			if (Constant11019.ListBonusElementIds.Contains(elementConfig.id))
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
			if (Constant11019.ListBonusElementIds.Contains(id) ||
			    Constant11019.ListBonusJackpotElementIds.Contains(id))
			{
				return typeof(ElementBonus11019);
			}

			return typeof(Element11019);   
		}

		public override (float, float, float) GetElementStaticGrayLayerPrams(uint id)
		{
			if (!Constant11019.ListBonusElementIds.Contains(id) &&
			    !Constant11019.ListBonusJackpotElementIds.Contains(id))
			{
				return (0.62f, 0.85f, 0.975f);
			}

			return base.GetElementStaticGrayLayerPrams(id);
		}


		public override int GetElementSortingOffset(uint id)
		{
			if (Constant11019.ScatterElementId == id)
			{
				return 100;
			}

			return base.GetElementSortingOffset(id);
		}

		public override int GetElementActiveSortingOffset(uint id)
		{
			if (id == Constant11019.WildElementId || id == Constant11019.ScatterElementId)
			{
				return 300;
			}

			return 200;
		}


		public override string GetElementBlinkSoundName(uint id, string name)
		{
			if (Constant11019.ListBonusAllElementIds.Contains(id))
			{
				name = "J01";
			}

			return base.GetElementBlinkSoundName(id, name);
		}


		public override List<uint> GetElementBlinkVariantList(uint id)
		{
			if (Constant11019.ListBonusAllElementIds.Contains(id))
			{
				return Constant11019.ListBonusAllElementIds;
			}
			
			return base.GetElementBlinkVariantList(id);
		}
	}
}