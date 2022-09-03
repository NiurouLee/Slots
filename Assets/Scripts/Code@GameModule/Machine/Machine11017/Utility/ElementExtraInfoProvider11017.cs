using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class ElementExtraInfoProvider11017 : ElementExtraInfoProvider
    {
        public ElementExtraInfoProvider11017()
        {

        }
        
        public override int GetElementSortingOffset(uint id)
        {
            if (id == 12  || id == 13)
                return 16;
            return 0;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (id ==  12 || id == 13)
                return 216;
            return 200;
        }
        
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
		{
			if (Constant11017.ScatterElementId == id || Constant11017.SmallGoldElementId == id || Constant11017.PuePleElementId == id)
			{
				return BlinkAnimationPlayStyleType.IdleCondition;
			}
			return base.GetElementBlinkAnimationPlayStyleType(id);
		}
    }
}