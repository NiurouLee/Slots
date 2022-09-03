using UnityEngine;

namespace GameModule
{
    public class WheelsSpinningProxy11002 : WheelsSpinningProxy
    {
        private Animator _animatorSpoiler;

        public WheelsSpinningProxy11002(MachineContext context)
            : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();
            _animatorSpoiler = machineContext.view.Get<Wheel>().transform.GetComponent<Animator>();
        }


        public override async void OnSpinResultReceived()
        {
            var preWheelHasAnticipation = false;

            for (var index = 0; index < runningWheel.Count; index++)
            {
                preWheelHasAnticipation = runningWheel[index].spinningController.OnSpinResultReceived(preWheelHasAnticipation);
            }

            var wheelState = machineContext.state.Get<WheelState11002>();
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.HasBonusGame() && wheelState.spoiler)
            {
                _animatorSpoiler.Play("Wheel", 0, 0);
                AudioUtil.Instance.PlayAudioFx("WheelFeature_Notice");

                var charater = machineContext.view.Get<CharacterView11002>();
                charater.AnimAppear();

                await XUtility.WaitSeconds(3, machineContext);
            }

            if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin
                || machineContext.state.Get<ReSpinState>().IsInRespin
                || !machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<ControlPanel>().ShowStopButton(true);
            }
        }
    }
}
