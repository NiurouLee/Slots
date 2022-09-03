using System;

namespace GameModule
{
    public class ElementExtraInfoProvider11021: ElementExtraInfoProvider
    {
        
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11021.ElementWild == id ||
                Constant11021.ElementScatter == id)
            {
                return BlinkAnimationPlayStyleType.Idle;
            }

            return base.GetElementBlinkAnimationPlayStyleType(id);
        }
        
        
        
        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            if (Constant11021.ElementWild == id ||
                Constant11021.ElementScatter == id)
            {
                return true;
            }

            return base.GetNeedKeepStateWhenStopAllAnimation(id);
        }


        public override int GetElementSortingOffset(uint id)
        {
            if (Constant11021.ElementWild == id)
            {
                return 30;
            }
            else if ( Constant11021.ElementScatter == id)
            {
                return 40;
            }
			
            return base.GetElementSortingOffset(id);
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11021.ElementWild == id)
            {
                return 300;
            }
            else if ( Constant11021.ElementScatter == id)
            {
                return 400;
            }
            return base.GetElementActiveSortingOffset(id);
        }

        public override Type GetElementClassType(uint id)
        {
            if (Constant11021.ElementScatter == id)
            {
                return typeof(ElementScatter11021);
            }
            return base.GetElementClassType(id);
        }
    }
}