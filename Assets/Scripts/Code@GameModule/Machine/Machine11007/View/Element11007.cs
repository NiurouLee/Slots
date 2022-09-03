//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-18 20:21
//  Ver : 1.0.0
//  Description : Element11007.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class Element11007: Element
    {
        [ComponentBinder("IntegralText")] private TextMesh _integralText;
        [ComponentBinder("GrayLayer")] private SpriteRenderer _spriteGray;
        public Element11007(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
            if (animator)
            {
                animator.keepAnimatorControllerStateOnDisable = true;
            }
        }
        
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            UpdateElementContent(element);
        }

        private void UpdateElementContent(SequenceElement element)
        {
            if (sequenceElement.machineContext.state.Get<WheelsActiveState11007>().IsLinkWheel)
            {
                if (_spriteGray)
                {
                    _spriteGray.color = Constant11007.IsCoinElement(element.config.id)
                        ? Color.white
                        : Constant11007.LinkGrayColor;
                }
            }
            else
            {
                if (_spriteGray)
                {
                    _spriteGray.color = Color.white;
                }
            }

            if (Constant11007.IsCoinElement(element.config.id))
            {
                UpdateCoinElementContent(element);
            }
        }

        private void UpdateCoinElementContent(SequenceElement element)
        {
            if (_integralText)
            {
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(100*(element.config.id - 20));
                _integralText.text = chips.GetAbbreviationFormat();
            }
        }
        
    }
}