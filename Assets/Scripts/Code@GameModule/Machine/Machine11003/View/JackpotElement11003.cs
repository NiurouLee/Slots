// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/28/21:07
// Ver : 1.0.0
// Description : JackpotElement.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class JackpotElement11003:Element
    {
        [ComponentBinder("Lock_Img")] 
        private Transform _lockState;

        public JackpotElement11003(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            UpdateElementContent();
        }

        public void UpdateElementContent()
        {
            var featureIndex = (int) sequenceElement.config.id - 10;

            if (featureIndex > 1)
            {
                featureIndex += 1;
            }
            
            var isUnlocked = sequenceElement.machineContext.state.Get<BetState>().IsFeatureUnlocked(featureIndex);
            
            _lockState.gameObject.SetActive(!isUnlocked);
        }
    }
}