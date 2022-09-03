using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11301 : ElementExtraInfoProvider
	{
		private List<uint> listLinkAllCoins = new List<uint>();
		public ElementExtraInfoProvider11301()
		{
			
			listLinkAllCoins.AddRange(Constant11301.ListNormalCoins);
			listLinkAllCoins.AddRange(Constant11301.ListNormalJackpotCoins);
			listAllCoins.AddRange(Constant11301.ListDoorCoins);
			listAllCoins.AddRange(Constant11301.ListDoorJackpotCoins);
		}
		
		
		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11301.ScatterElementId == id)
			{
				return BlinkAnimationPlayStyleType.IdleCondition;
			}
			else if (Constant11301.ListNormalCoins.Contains(id) ||
			         Constant11301.ListNormalJackpotCoins.Contains(id)||
			         Constant11301.ListDoorCoins.Contains(id)||
			         Constant11301.ListDoorJackpotCoins.Contains(id))
			{
				return BlinkAnimationPlayStyleType.Idle;
			}

			return base.GetElementBlinkAnimationPlayStyleType(id);
		}
		
		
		
		public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
		{
			if (Constant11301.ListNormalCoins.Contains(elementConfig.id) ||
			    Constant11301.ListDoorCoins.Contains(elementConfig.id))
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
			if (Constant11301.ListNormalCoins.Contains(id) ||
			    Constant11301.ListNormalJackpotCoins.Contains(id)||
			    Constant11301.ListDoorCoins.Contains(id)||
			    Constant11301.ListDoorJackpotCoins.Contains(id))
			{
				return typeof(ElementCoin11301);
			}
			else if (Constant11301.ScatterElementId != id &&
			         !Constant11301.ListDoorElementIds.Contains(id))
			{
				return typeof(Element11301);  
			}

			return typeof(Element);
		}

		public override (float, float, float) GetElementStaticGrayLayerPrams(uint id)
		{
			if (!Constant11301.ListDoorCoins.Contains(id) &&
			    !Constant11301.ListNormalCoins.Contains(id) &&
			    Constant11301.ScatterElementId != id &&
			    !Constant11301.ListDoorElementIds.Contains(id) &&
			    !Constant11301.ListNormalJackpotCoins.Contains(id) &&
			    !Constant11301.ListDoorJackpotCoins.Contains(id))
			{
				return (0.62f, 1.12f, 0.46f);
			}

			return base.GetElementStaticGrayLayerPrams(id);
		}


		public override int GetElementSortingOffset(uint id)
		{
			if (Constant11301.ScatterElementId == id)
			{
				return 100;
			}

			return base.GetElementSortingOffset(id);
		}

		public override int GetElementActiveSortingOffset(uint id)
		{
			if (Constant11301.ListWildElementId.Contains(id))
			{
				return 300;
			}
			else if (Constant11301.ListNormalCoins.Contains(id) ||
			    Constant11301.ListNormalJackpotCoins.Contains(id) ||
			    Constant11301.ListDoorCoins.Contains(id) ||
			    Constant11301.ListDoorJackpotCoins.Contains(id))
			{
				return 3000;
			}
			
			// if (Constant11301.ListDoorElementIds.Contains(id))
			// {
			// 	return 200;
			// }

			return 200;
		}


		public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
		{
			if (Constant11301.ListDoorElementIds.Contains(id))
			{
				return true;
			}

			return base.GetNeedKeepStateWhenStopAllAnimation(id);
		}


		

		public override string GetElementBlinkSoundName(uint id, string name)
		{
			if (listAllCoins.Contains(id))
			{
				name = "B01";
			}
			if(listLinkAllCoins.Contains(id)){
				name = "J01";
			}

			return base.GetElementBlinkSoundName(id, name);
		}

		private List<uint> listAllCoins = new List<uint>();
		public override List<uint> GetElementBlinkVariantList(uint id)
		{
			if (listLinkAllCoins.Contains(id))
			{
				return listLinkAllCoins;
			}
			if (listAllCoins.Contains(id))
			{
				return listAllCoins;
			}
			return base.GetElementBlinkVariantList(id);
		}
	}
}