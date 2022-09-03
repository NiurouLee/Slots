using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class ElementGoldValue11024:ElementValue11024
    {
        public ElementGoldValue11024(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
        {
            
        }

        public void JumpNum()
        {
            if (integralText)
            {
                var sequence = DOTween.Sequence();
                sequence.Append(integralText.transform.DOScale(1.3f,0.2f));
                sequence.Append(integralText.transform.DOScale(1.4f,0.2f));
                sequence.Append(integralText.transform.DOScale(1f,0.2f));
                sequence.target = sequenceElement.machineContext.transform;
            }
        }
    }
}