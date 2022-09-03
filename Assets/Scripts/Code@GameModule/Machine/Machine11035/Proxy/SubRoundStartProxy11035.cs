using System.Collections.Generic;

namespace GameModule
{
    public class SubRoundStartProxy11035 : SubRoundStartProxy
    {
        private UIComboView _uiComboView;

        private UIComboNoticeView _uiComboNoticeView;

        private ReSpinState _respinState;

        private ExtraState11035 _extraState;
        private WheelsActiveState11035 _activeState;

        private UIMachineLineGroupView11035 _lineGroupView;

        public SubRoundStartProxy11035(MachineContext context)
            : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();

            _uiComboView = machineContext.view.Get<UIComboView>();

            _uiComboNoticeView = machineContext.view.Get<UIComboNoticeView>();

            _respinState = machineContext.state.Get<ReSpinState>();

            _extraState = machineContext.state.Get<ExtraState11035>();

            _activeState = machineContext.state.Get<WheelsActiveState11035>();

            _lineGroupView = machineContext.view.Get<UIMachineLineGroupView11035>();
        }

        protected override void HandleCustomLogic()
        {
            if (!_extraState.HasJackpotWheel())
            {
                if (_activeState.GetRunningWheel()[0].wheelName != "WheelBaseGame")
                {
                    _activeState.UpdateRunningWheel(new List<string>() {"WheelBaseGame"});

                    machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
                    machineContext.view.Get<JackPotPanel>().UnlockJackpotPanel();
                }
            }
            else
            {
                AudioUtil.Instance.PlayAudioFxIfNotPlaying("11035_JackpotRolling");
            }

            _uiComboView.Set();

            _uiComboNoticeView.Hide();

            _lineGroupView.PlayIdle();

            if (_respinState.NextIsReSpin || _respinState.IsInRespin || _extraState.HasJackpotWheel())
            {
                machineContext.view.Get<ControlPanel>().StopWinAnimation(false, 0);
            }

            AudioUtil.Instance.StopAudioFx("Jackpot_Win");

            base.HandleCustomLogic();
        }

        protected override void PlayBgMusic()
        {
            if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
            }
            else if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
            }
            else
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
            }
        }
    }
}