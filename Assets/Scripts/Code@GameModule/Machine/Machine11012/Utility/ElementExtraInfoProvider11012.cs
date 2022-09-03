using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11012 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11012()
		{
			
			// listAllCoins.AddRange(Constant11012.ListNormalCoins);
			// listAllCoins.AddRange(Constant11012.ListNormalJackpotCoins);
			listAllCoins.AddRange(Constant11012.ListDoorCoins);
			listAllCoins.AddRange(Constant11012.ListDoorJackpotCoins);
		}
		
		
		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11012.ScatterElementId == id)
			{
				return BlinkAnimationPlayStyleType.IdleCondition;
			}
			else if (Constant11012.ListNormalCoins.Contains(id) ||
			         Constant11012.ListNormalJackpotCoins.Contains(id)||
			         Constant11012.ListDoorCoins.Contains(id)||
			         Constant11012.ListDoorJackpotCoins.Contains(id))
			{
				return BlinkAnimationPlayStyleType.Idle;
			}

			return base.GetElementBlinkAnimationPlayStyleType(id);
		}
		
		
		
		public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
		{
			if (Constant11012.ListNormalCoins.Contains(elementConfig.id) ||
			    Constant11012.ListDoorCoins.Contains(elementConfig.id))
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
			if (Constant11012.ListNormalCoins.Contains(id) ||
			    Constant11012.ListNormalJackpotCoins.Contains(id)||
			    Constant11012.ListDoorCoins.Contains(id)||
			    Constant11012.ListDoorJackpotCoins.Contains(id))
			{
				return typeof(ElementCoin11012);
			}
			else if (Constant11012.ScatterElementId != id &&
			         !Constant11012.ListDoorElementIds.Contains(id))
			{
				return typeof(Element11012);  
			}

			return typeof(Element);
		}

		public override (float, float, float) GetElementStaticGrayLayerPrams(uint id)
		{
			if (!Constant11012.ListDoorCoins.Contains(id) &&
			    !Constant11012.ListNormalCoins.Contains(id) &&
			    Constant11012.ScatterElementId != id &&
			    !Constant11012.ListDoorElementIds.Contains(id) &&
			    !Constant11012.ListNormalJackpotCoins.Contains(id) &&
			    !Constant11012.ListDoorJackpotCoins.Contains(id))
			{
				return (0.62f, 1.12f, 0.46f);
			}

			return base.GetElementStaticGrayLayerPrams(id);
		}


		public override int GetElementSortingOffset(uint id)
		{
			if (Constant11012.ScatterElementId == id)
			{
				return 100;
			}

			return base.GetElementSortingOffset(id);
		}

		public override int GetElementActiveSortingOffset(uint id)
		{
			if (Constant11012.ListWildElementId.Contains(id))
			{
				return 300;
			}
			else if (Constant11012.ListNormalCoins.Contains(id) ||
			    Constant11012.ListNormalJackpotCoins.Contains(id) ||
			    Constant11012.ListDoorCoins.Contains(id) ||
			    Constant11012.ListDoorJackpotCoins.Contains(id))
			{
				return 3000;
			}
			
			// if (Constant11012.ListDoorElementIds.Contains(id))
			// {
			// 	return 200;
			// }

			return 200;
		}


		public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
		{
			if (Constant11012.ListDoorElementIds.Contains(id))
			{
				return true;
			}

			return base.GetNeedKeepStateWhenStopAllAnimation(id);
		}


		

		public override string GetElementBlinkSoundName(uint id, string name)
		{
			if (Constant11012.ListNormalCoins.Contains(id) || Constant11012.ListDoorJackpotCoins.Contains(id))
			{
				name = "J01"; 
			}
			else if (listAllCoins.Contains(id))
			{
				name = "B01";
			}

			return base.GetElementBlinkSoundName(id, name);
		}

		private List<uint> listAllCoins = new List<uint>();
		public override List<uint> GetElementBlinkVariantList(uint id)
		{
			if (listAllCoins.Contains(id))
			{
				return listAllCoins;
			}
			return base.GetElementBlinkVariantList(id);
		}
	}
}