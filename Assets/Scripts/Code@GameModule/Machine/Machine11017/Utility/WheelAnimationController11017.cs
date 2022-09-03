using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11017: WheelAnimationController
    {
        protected Animator animatorZhenping;
        public override void ShowAnticipationAnimation(int rollIndex)
        {
            base.ShowAnticipationAnimation(rollIndex);
            animatorZhenping = wheel.GetContext().transform.Find("ZhenpingAnim").GetComponent<Animator>();
            XUtility.PlayAnimationAsync(animatorZhenping, "Free", wheel.GetContext());
        }
    }
}