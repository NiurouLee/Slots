using System;
using UnityEngine;

namespace GameModule
{
    public class ElementValue11030:Element
    {
        [ComponentBinder("IntegralGroup/IntegralText")] protected TextMesh integralText;
        public ElementValue11030(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
            if (!inIsStatic)
            {
                integralText = transform.Find("Root/IntegralGroup/IntegralText").GetComponent<TextMesh>();
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
                long winRate = sequenceElement.config.GetExtra<int>("winRate");
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                integralText.text = Constant11030.ChipNum2String(Convert.ToDouble(chips));
            }
        }

        public long GetValueWinRate()
        {
            return sequenceElement.config.GetExtra<int>("winRate");
        }

        public override void DoRecycle()
        {
            transform.gameObject.SetActive(true);
            base.DoRecycle();
        }
    }
}