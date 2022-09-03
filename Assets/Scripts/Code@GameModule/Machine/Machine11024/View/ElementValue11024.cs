using System;
using UnityEngine;

namespace GameModule
{
    public class ElementValue11024:ElementCoin11024
    {
        [ComponentBinder("Root/Num")] protected TextMesh integralText;
        public ElementValue11024(Transform transform, bool inIsStatic)
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
            long winRate = sequenceElement.config.GetExtra<int>("winRate");
            SetWinRate(winRate);
        }

        public virtual void SetWinRate(long inWinRate)
        {
            if (integralText)
            {
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(inWinRate);
                integralText.text = Constant11024.ChipNum2String(Convert.ToDouble(chips));
            }
        }

        public override void DoRecycle()
        {
            // transform.gameObject.SetActive(true);
            base.DoRecycle();
        }
    }
}