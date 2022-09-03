// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/16/11:04
// Ver : 1.0.0
// Description : ElementExtraInfoProvider11002.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class ElementExtraInfoProvider11002 : ElementExtraInfoProvider
    {
        public override int GetElementSortingOffset(uint id)
        {
            if (id == 12 || id == 17)
                return 40;
            return 0;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (id == 12 || id == 17)
                return 400;
            return 50;
        }

        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (id == 12 || id == 17)
                return BlinkAnimationPlayStyleType.IdleCondition;
            return BlinkAnimationPlayStyleType.Default;
        }

        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (id == 14 || id == 15 || id == 16)
            {
                return "B02_Blink";
            }

            if (id == 17)
            {
                return "B05_BLINK";
            }

            return base.GetElementBlinkSoundName(id, name);
        }

    }
}