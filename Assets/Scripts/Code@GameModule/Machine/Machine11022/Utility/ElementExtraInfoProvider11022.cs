using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11022 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11022()
		{

		}

		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11022.LinkList.Contains(id))
				return BlinkAnimationPlayStyleType.Idle;
			return BlinkAnimationPlayStyleType.Default;
		}
		public bool CheckElementJackpot(uint elementId)
		{
			return Constant11022.JackpotList.Contains(elementId);
		}
		public bool CheckElementValue(uint elementId)
		{
			return Constant11022.ValueList.Contains(elementId);
		}
		public bool CheckElementLongWild(uint elementId)
		{
			return Constant11022.LongWildList.Contains(elementId);
		}

		public bool CheckElementBox(uint elementId)
		{
			return Constant11022.BoxList.Contains(elementId);
		}


		public override Type GetElementClassType(uint id)
		{
			if (CheckElementJackpot(id))
			{
				return typeof(JackpotElement11022);
			}

			if (CheckElementValue(id))
			{
				return typeof(CoinElement11022);
			}

			if (CheckElementLongWild(id))
			{
				return typeof(LongWildElement11022);
			}

			return typeof(GrayElement11022);
		}
		public override SpriteMaskInteraction GetElementActiveInteraction(uint id)
		{
			if (Constant11022.NormalList.Contains(id) || Constant11022.LongWildList.Contains(id))
			{
				return SpriteMaskInteraction.VisibleInsideMask;   
			}
			return base.GetElementActiveInteraction(id);
		}
		public override int GetElementSortingOffset(uint id)
		{
			if (CheckElementJackpot(id) || CheckElementValue(id) || CheckElementBox(id) || id == 13)
			{
				return 60;
			}

			if (id == 12)
			{
				return 52;
			}

			if (CheckElementLongWild(id))
			{
				return 20;
			}
			return 0;
		}
		
		public override int GetElementActiveSortingOffset(uint id)
		{
			if (id == 12)
			{
				return 310;
			}
			if (CheckElementJackpot(id) || CheckElementValue(id) || CheckElementBox(id) || id == 13)
			{
				return 300;
			}

			return 40;
		}
		
		public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
		{
			if (CheckElementJackpot(id) || CheckElementValue(id))
			{
				return true;
			}
            
			return base.GetNeedKeepStateWhenStopAllAnimation(id);
		}
		
		public override string GetElementBlinkSoundName(uint id, string name)
		{
			if (CheckElementJackpot(id) || CheckElementValue(id))
			{
				return "B02_Blink";
			}

			return base.GetElementBlinkSoundName(id, name);
		}
		
		public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
		{
			if (CheckElementJackpot(elementConfig.id))
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
		public override (float,float,float) GetElementStaticGrayLayerPrams(uint id)
		{
			if (Constant11022.NormalList.Contains(id) || id == 12)
			{
				return (0f, 0.52f, 0.4f);
			}
			return (0,0,0);
		}
	}
}