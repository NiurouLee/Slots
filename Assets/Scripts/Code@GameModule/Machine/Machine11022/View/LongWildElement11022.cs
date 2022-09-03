// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/23/12:19
// Ver : 1.0.0
// Description : PigAndCoinElement.cs
// ChangeLog :
// **********************************************

using System;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class LongWildElement11022 : Element
    {
        [ComponentBinder("SymbolSprite")] protected Transform _Picture;

        public LongWildElement11022(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            UpdateElementContent();
        }

        public virtual void UpdateElementContent()
        { 
            
        }
        public override void PlayAnimation(string animationName, bool maskByWheelMask, Action endCallback = null)
        {
            base.PlayAnimation(animationName, true, endCallback);
        }
    }
}