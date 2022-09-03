using UnityEngine;

namespace GameModule
{
    public class UIActivity_ValentinesDay_MapPopup_RewardCellView : UIActivity_ValentinesDay_RewardCellView
    {
        [ComponentBinder("StateGroup/FinishState")]
        public Transform transformFinishState;

        [ComponentBinder("")]
        public Animator animator;

        public void SetChecked(bool isChecked, bool withAnimation)
        {
            var stateName = isChecked ? "Collect State" : "Emptry";
            if (animator != null)
            {
                animator.Play(stateName, 0, withAnimation ? 0 : 1);
            }
        }
    }
}