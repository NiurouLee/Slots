using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11013 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11013()
		{

		}
		
		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11013.StarElement == id ||
			         Constant11013.WildBaseElement == id ||
			         Constant11013.WildFreeElement == id)
			{
				return BlinkAnimationPlayStyleType.Idle;
			}

			return base.GetElementBlinkAnimationPlayStyleType(id);
		}
		
		
		public override Type GetElementClassType(uint id)
		{
			if (Constant11013.StarElement == id)
			{
				return typeof(ElementStar11013);
			}

			if (Constant11013.GoldenScatterElement == id ||
			    Constant11013.PinkScatterElement == id)
			{
				return typeof(ElementScatter11013);
			}

			return typeof(Element);   
		}


		public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
		{
			if (Constant11013.StarElement == id ||
			    Constant11013.WildBaseElement == id ||
			    Constant11013.WildFreeElement == id ||
			    Constant11013.GoldenScatterElement == id ||
			    Constant11013.PinkScatterElement == id)
			{
				return true;
			}

			return base.GetNeedKeepStateWhenStopAllAnimation(id);
		}


		public override int GetElementSortingOffset(uint id)
		{
			if (Constant11013.StarElement == id ||
			    Constant11013.WildBaseElement == id ||
			    Constant11013.WildFreeElement == id ||
			    Constant11013.GoldenScatterElement == id ||
			    Constant11013.PinkScatterElement == id)
			{
				return 30;
			}
			
			return base.GetElementSortingOffset(id);
		}

		public override int GetElementActiveSortingOffset(uint id)
		{
			if (Constant11013.StarElement == id ||
			    Constant11013.WildBaseElement == id ||
			    Constant11013.WildFreeElement == id ||
			    Constant11013.GoldenScatterElement == id ||
			    Constant11013.PinkScatterElement == id)
			{
				return 300;
			}
			return base.GetElementActiveSortingOffset(id);
		}
		
		
		public override string GetElementBlinkSoundName(uint id, string name)
		{
			if (Constant11013.ListAllScatterElements.Contains(id))
			{
				name = "B01";
			}

			return base.GetElementBlinkSoundName(id, name);
		}


		public override List<uint> GetElementBlinkVariantList(uint id)
		{
			if (Constant11013.ListAllScatterElements.Contains(id))
			{
				return Constant11013.ListAllScatterElements;
			}
			
			return base.GetElementBlinkVariantList(id);
		}
	}
}