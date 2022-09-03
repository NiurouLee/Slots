using static DragonU3DSDK.Network.API.ILProtocol.SGetValentineMainPageInfo.Types;
using static DragonU3DSDK.Network.API.ILProtocol.SGetValentineMainPageInfo.Types.ValentineStepReward.Types;

namespace GameModule
{
    public class UIActivity_ValentinesDay_LevelRewardsView : View
    {
        private UIActivity_ValentinesDay_LevelRewardView[] _rewardViews;

        private int[] _steps = Activity_ValentinesDay.steps;

        private int _currenStep = -1;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            _rewardViews = new UIActivity_ValentinesDay_LevelRewardView[_steps.Length];

            for (int i = 0; i < _steps.Length; i++)
            {
                var transformReward = transform.Find($"LevelReward{_steps[i]}");
                _rewardViews[i] = AddChild<UIActivity_ValentinesDay_LevelRewardView>(transformReward);
            }
        }

        public void SetLocked(bool locked)
        {
            for (int i = 0; i < _rewardViews.Length; i++)
            {
                _rewardViews[i].SetLocked(locked);
            }
        }

        // public void SetSpecialRewardChecked(int step, bool isChecked)
        // {
        //     if (step <= 0 || step >= _rewardViews.Length) { return; }
        //     _rewardViews[step].SetSpecialRewardChecked(isChecked, step > _currenStep);
        // }

        // public void SetNormalRewardChecked(int step, bool isChecked)
        // {
        //     if (step <= 0 || step >= _rewardViews.Length) { return; }
        //     _rewardViews[step].SetNormalRewardChecked(isChecked, step > _currenStep);
        // }

        public void Set(ValentineStepReward[] rewards)
        {
            if (rewards == null || rewards.Length != _rewardViews.Length) { return; }
            for (int i = 0; i < _rewardViews.Length; i++)
            {
                var reward = rewards[i];
                var rewardView = _rewardViews[i];
                rewardView.Set(reward);
            }
        }

        public void SetStep(int step)
        {
            for (int i = 0; i < _rewardViews.Length; i++)
            {
                _rewardViews[i].SetStepChecked(step >= _steps[i], false);
            }
            _currenStep = step;
        }

        public void AnimateSetStep(int step)
        {
            for (int i = 0; i < _rewardViews.Length; i++)
            {
                _rewardViews[i].SetStepChecked(step >= _steps[i], _steps[i] > _currenStep);
            }
            _currenStep = step;
        }
    }
}