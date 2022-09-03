using System;
using UnityEngine;

namespace GameModule
{
    public class ElementTrain11030:Element
    {
        [ComponentBinder("IntegralGroup/IntegralText")] protected TextMesh integralText;
        public ElementTrain11030(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
            if (!inIsStatic)
            {
                integralText = transform.Find("Root/IntegralGroup/IntegralText").GetComponent<TextMesh>();
            }
        }

        public long trainWinRate = 0; 
        public void SetSymbolUpperValue(long inWinRate)
        {
            trainWinRate = inWinRate;
            if (integralText)
            {
                integralText.gameObject.SetActive(true);
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(inWinRate);
                integralText.text = integralText.text = Constant11030.ChipNum2String(Convert.ToDouble(chips));
            }
        }
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            UpdateElementContent();
        }
        

        public virtual void UpdateElementContent()
        {
            if (integralText)
            {
                integralText.gameObject.SetActive(false);
            }
            trainWinRate = 0;
        }

        public long GetTrainWinRate()
        {
            return trainWinRate;
        }

        public override void DoRecycle()
        {
            if (integralText)
            {
                integralText.gameObject.SetActive(false);
            }
            trainWinRate = 0;
            base.DoRecycle();
        }
    }
}