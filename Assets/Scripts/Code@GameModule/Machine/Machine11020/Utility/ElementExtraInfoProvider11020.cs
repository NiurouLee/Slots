

using System;
using UnityEngine;

namespace GameModule
{
    public class ElementExtraInfoProvider11020: ElementExtraInfoProvider
    {
        public override Type GetElementClassType(uint id)
        {
            if (Constant11020.lionElement == id)
            {
                return typeof(FireBallElement11020);
            }

            return typeof(Element);
        }

        public override  int GetElementSortingOffset(uint id)
        {
            if (id == Constant11020.bonusElement || id == Constant11020.lionElement)
            {
                return 50;
            }
            return 0;
        }

        // public override SpriteMaskInteraction GetElementDefaultInteraction(uint id)
        // {
        //     if (id == Constant11020.bonusElement 
        //         // || id == Constant11020.lionElement
        //         )
        //     {
        //         return SpriteMaskInteraction.None;
        //     }

        //     return SpriteMaskInteraction.VisibleInsideMask;
        // }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (
                // id == Constant11020.wildElement 
                // || id == Constant11020.buleWildElement || 
                id == Constant11020.bonusElement ||
                id == Constant11020.bonusWildElement || 
                id == Constant11020.lionElement)
            {
                return 2000;
            }
            return 10;
        }

        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (id == Constant11020.lionElement)
                return BlinkAnimationPlayStyleType.Idle;
            
            return BlinkAnimationPlayStyleType.Default;
        }

        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            if (
                id == Constant11020.bonusElement ||
                id == Constant11020.bonusWildElement)
            {
                return true;
            }

            return base.GetNeedKeepStateWhenStopAllAnimation(id);
        }
        
        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11020.bonusElement == id)
            {
                return "B01_Blink";
            }

            if (Constant11020.lionElement == id)
            {
                return "J01_Blink";  
            }
            return base.GetElementBlinkSoundName(id, name);
        }
    }
}
