using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11015 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11015()
		{

		}
		
		
		
		public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11015.ScatterElementId == id)
			{
				return BlinkAnimationPlayStyleType.IdleCondition;
			}
			else if (Constant11015.ListWildAllElementId.Contains(id) ||
			         id == Constant11015.ShieldElementId)
			{
				return BlinkAnimationPlayStyleType.Idle;
			}

			return base.GetElementBlinkAnimationPlayStyleType(id);
		}
		
	



		public override int GetElementSortingOffset(uint id)
		{
			if (id == Constant11015.WildElementId)
			{
				return 10;
			}
			else if (id == Constant11015.ScatterElementId)
			{
				return 20;
			}
			else if (id == Constant11015.ShieldElementId)
			{
				return 30;
			}

			return base.GetElementSortingOffset(id);
		}

		public override int GetElementActiveSortingOffset(uint id)
		{
			if (id == Constant11015.WildElementId)
			{
				return 100;
			}
			else if (id == Constant11015.ScatterElementId)
			{
				return 200;
			}
			else if (id == Constant11015.ShieldElementId)
			{
				return 300;
			}

			return base.GetElementActiveSortingOffset(id);
		}


		public override Type GetElementClassType(uint id)
		{
			if (id == Constant11015.ShieldElementId)
			{
				return typeof(ElementShield11015);
			}
			else if (id == Constant11015.ZeusElementId)
			{
				return typeof(ElementZeus11015);
			}

			return base.GetElementClassType(id);
		}


		public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
		{
			if (id == Constant11015.ZeusElementId)
			{
				return true;
			}
			return base.GetNeedKeepStateWhenStopAllAnimation(id);
		}
	}
}