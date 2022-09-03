//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-18 19:53
//  Ver : 1.0.0
//  Description : ElementExtraInfoProvider11007.cs
//  ChangeLog :
//  **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class ElementExtraInfoProvider11007: ElementExtraInfoProvider
    {
        public override Type GetElementClassType(uint id)
        {
            if (!(Constant11007.IsJackpotElement(id) ||
                  id == Constant11007.ELEMENT_EMPTY || 
                  Constant11007.IsBonusElement(id)))
            {
                return typeof(Element11007);
            }
            
            return typeof(Element);
        }
        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11007.IsLinkElement(id))
            {
                return 30;
            }
            return 10;
        }

        public override SpriteMaskInteraction GetElementActiveInteraction(uint id)
        {
            if (Constant11007.IsBigElement(id) || Constant11007.ELEMENT_WILD == id)
            {
                return SpriteMaskInteraction.VisibleInsideMask;   
            }
            return base.GetElementActiveInteraction(id);
        }
        
        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11007.IsLinkElement(id))
            {
                return  "J01_Blink"; 
            }
            return name + "_Blink";
        }
    }
}