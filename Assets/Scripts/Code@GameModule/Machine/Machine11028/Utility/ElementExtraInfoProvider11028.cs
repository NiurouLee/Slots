//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-17 14:22
//  Ver : 1.0.0
//  Description : ElementExtraInfoProvider11028.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class ElementExtraInfoProvider11028: ElementExtraInfoProvider
    {
        public override BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            if (Constant11028.B01 == id || Constant11028.RapidHit.Contains(id))
            {
                return BlinkAnimationPlayStyleType.Idle;
            }

            return base.GetElementBlinkAnimationPlayStyleType(id);
        }

        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11028.RapidHit.Contains(id))
            {
                return "J01_Blink";
            }
            return base.GetElementBlinkSoundName(id, name);
        }
        
        public override bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            if (Constant11028.RapidHit.Contains(id))
            {
                return true;
            }
            return base.GetNeedKeepStateWhenStopAllAnimation(id);
        }
        
        public override int GetElementBlinkSoundOrderId(uint id)
        {
            if (Constant11028.B01 == id)
            {
                return 2;
            }
            if (Constant11028.RapidHit.Contains(id))
            {
                return 1;
            }

            return base.GetElementBlinkSoundOrderId(id);
        }
        
        public override Type GetElementClassType(uint id)
        {
            if (Constant11028.B01 == id)
            {
                return typeof(Element11028);
            }

            return base.GetElementClassType(id);
        }
    }
}