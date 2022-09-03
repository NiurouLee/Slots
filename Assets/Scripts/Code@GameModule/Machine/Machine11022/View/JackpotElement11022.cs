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

using TMPro;
using UnityEngine;

namespace GameModule
{
    public class JackpotElement11022 : Element
    {

        public JackpotElement11022(Transform transform, bool inIsStatic)
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
        public override void DoRecycle()
        {
            transform.gameObject.SetActive(true);
            base.DoRecycle();
        }
    }
}