using UnityEngine;

namespace GameModule
{
    public class EarlyHighLevelWinEffectProxy11002 : EarlyHighLevelWinEffectProxy
    {
        private Animator _animator;

        public EarlyHighLevelWinEffectProxy11002(MachineContext context)
            : base(context)
        {

        }

        public override void SetUp()
        {
            base.SetUp();
            _animator = machineContext.transform.Find("ZhenpingAnim").GetComponent<Animator>();
        }

        protected override async void HandleCustomLogic()
        {
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState11002>();
            if (wheelsActiveState.GetRunningWheel()[0].wheelName == "Wheel0")
            {
                _animator.Play("Open", 0, 0);
            }
            else
            {
                _animator.Play("Open1", 0, 0);
            }
            AudioUtil.Instance.PlayAudioFx("BigWin_Notice");

            await XUtility.WaitSeconds(2.1f, machineContext);
            WinEffectHelper.ShowBigWinEffect(_winState.winLevel, _winState.displayCurrentWin, Proceed);
            // UpdateWinChipsToDisplayTotalWin();
        }
    }
}