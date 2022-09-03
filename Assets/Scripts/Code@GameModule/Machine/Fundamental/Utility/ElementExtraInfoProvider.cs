// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/06/11:57
// Ver : 1.0.0
// Description : ElementExtraInfoProvider.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class ElementExtraInfoProvider : IElementExtraInfoProvider
    {
        public virtual Type GetElementClassType(uint id)
        {
            return typeof(Element);
        }

        public virtual  int GetElementSortingOffset(uint id)
        {
            return 0;
        }

        public virtual int GetElementActiveSortingOffset(uint id)
        {
            return 200;
        }

        public virtual string GetElementActiveAssetAddress(uint id, string name)
        {
            return "Active_" + name;
        }

        public virtual string GetElementStaticAssetAddress(uint id, string name)
        {
            return "Static_" + name;
        }

        public virtual string GetElementBlinkSoundName(uint id, string name)
        {
            return name + "_Blink";
        }
        
        
        public virtual BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id)
        {
            return BlinkAnimationPlayStyleType.Default;
        }
        
        public virtual SpriteMaskInteraction GetElementDefaultInteraction(uint id)
        {
            return SpriteMaskInteraction.VisibleInsideMask;
        }
        
        public virtual SpriteMaskInteraction GetElementActiveInteraction(uint id)
        {
            return SpriteMaskInteraction.None;
        }

        public virtual (float,float,float) GetElementStaticGrayLayerPrams(uint id)
        {
            return (0,0,0);
        }
        public virtual Color GetElementStaticGrayLayerMultiBlendModePrams(uint id)
        {
            return new Color(0,0,0,0);
        }

        public virtual void  ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            
        }
        
        public virtual bool GetNeedKeepStateWhenStopAllAnimation(uint id)
        {
            return false;
        }

        public virtual List<uint> GetElementBlinkVariantList(uint id)
        {
            return new List<uint>(){id};
        }

        public virtual int GetElementBlinkSoundOrderId(uint id)
        {
            return 0;
        }

        public virtual bool CanShowElementAnticipation(uint id)
        {
            return false;
        }

        public virtual bool GetElementCreateBigElementParts(uint id)
        {
            return false;
        }
    }
}