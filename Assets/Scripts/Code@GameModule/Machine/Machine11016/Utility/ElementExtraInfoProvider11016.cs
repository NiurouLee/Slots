//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 15:27
//  Ver : 1.0.0
//  Description : ElementExtraInfoProvider11016.cs
//  ChangeLog :
//  **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class ElementExtraInfoProvider11016:ElementExtraInfoProvider
    {
        public override Type GetElementClassType(uint id)
        {
            return typeof(Element11016);   
        }   
        
        public override  int GetElementSortingOffset(uint id)
        {
            if (Constant11016.IsFeaturedElement(id))
            {
                return 10;
            }

            if (Constant11016.IsFeaturedSlotElement(id))
            {
                return 5;
            }
            return 0;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11016.IsFeaturedElement(id))
            {
                return 1;
            }

            if (Constant11016.IsFeaturedSlotElement(id))
            {
                return 2;
            }
            return 200;
        }
        
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11016.IsFeaturedSlotElement(id))
            {
                return BlinkAnimationPlayStyleType.Idle;
            }
            return base.GetElementBlinkAnimationPlayStyleType(id);
        }
        
        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11016.IsFeaturedSlotElement(id))
            {
                return "J01_Blink";
            }
            if (Constant11016.IsFeaturedElement(id))
            {
                return "B01_Blink";
            }

            return base.GetElementBlinkSoundName(id, name);
        }

        public override int GetElementBlinkSoundOrderId(uint id)
        {
            if (Constant11016.IsFeaturedSlotElement(id))
            {
                return 2;
            }
            if (Constant11016.IsFeaturedElement(id))
            {
                return 1;
            }

            return base.GetElementBlinkSoundOrderId(id);
        }
    }
}