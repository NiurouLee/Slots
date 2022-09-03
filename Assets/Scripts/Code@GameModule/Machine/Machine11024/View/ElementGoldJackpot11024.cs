using System;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class ElementGoldJackpot11024:ElementCoin11024
    {
        [ComponentBinder("Root/Num")] protected TextMesh integralText;
        [ComponentBinder("Root/Nomal")] protected Transform normalSprite;
        [ComponentBinder("Root/AddValue")] protected Transform addValueSprite;
        public ElementGoldJackpot11024(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
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
            integralText.text = "";
            normalSprite.gameObject.SetActive(true);
            addValueSprite.gameObject.SetActive(false);
        }

        public virtual void SetWinRate(long inWinRate)
        {
            if (integralText)
            {
                if (inWinRate > 0)
                {
                    var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(inWinRate);
                    integralText.text = Constant11024.ChipNum2String(Convert.ToDouble(chips));   
                    normalSprite.gameObject.SetActive(false);
                    addValueSprite.gameObject.SetActive(true);
                }
                else
                {
                    integralText.text = "";
                    normalSprite.gameObject.SetActive(true);
                    addValueSprite.gameObject.SetActive(false);
                }
            }
        }

        public override void DoRecycle()
        {
            integralText.text = "";
            normalSprite.gameObject.SetActive(true);
            addValueSprite.gameObject.SetActive(false);
            base.DoRecycle();
        }
        public void JumpNum()
        {
            if (integralText)
            {
                var sequence = DOTween.Sequence();
                sequence.Append(integralText.transform.DOScale(1.2f,0.2f));
                sequence.Append(integralText.transform.DOScale(1f,0.2f));
                sequence.target = sequenceElement.machineContext.transform;
            }
        }
    }
}