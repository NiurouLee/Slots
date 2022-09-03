using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class NextSpinPrepareProxy11035 : NextSpinPrepareProxy
    {
        private UIComboView _uiComboView;

        private UIComboNoticeView _uiComboNoticeView;

        private UIFireJackpotNoticeView _uiFireJackpotNoticeView;


        private ExtraState11035 _extraState;

        private WheelsActiveState11035 _activeState;

        private ReSpinState _respinState;

        private AutoSpinState _autoSpinState;

        private Wheel _wheel;

        private Wheel _jackpotWheel;

        private UIMachineLineGroupView11035 _lineGroupView;

        private ControlPanel _controlPanel;


        public NextSpinPrepareProxy11035(MachineContext context) : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();

            _uiComboView = machineContext.view.Get<UIComboView>();

            _uiComboNoticeView = machineContext.view.Get<UIComboNoticeView>();

            _uiFireJackpotNoticeView = machineContext.view.Get<UIFireJackpotNoticeView>();

            _extraState = machineContext.state.Get<ExtraState11035>();

            _activeState = machineContext.state.Get<WheelsActiveState11035>();

            _respinState = machineContext.state.Get<ReSpinState>();

            _autoSpinState = machineContext.state.Get<AutoSpinState>();

            _wheel = machineContext.view.Get<Wheel>();

            _lineGroupView = machineContext.view.Get<UIMachineLineGroupView11035>();

            _jackpotWheel = machineContext.view.Get<Wheel>(1);

            _controlPanel = machineContext.view.Get<ControlPanel>();
        }

        protected override async void HandleCommonLogic()
        {
            // if (_extraState.HasJackpotWheel())
            // {
            //     machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            //
            //     if (preLogicStep != LogicStepType.STEP_MACHINE_SETUP)
            //     {
            //         _controlPanel.ShowSpinButton(false);
            //
            //         await XUtility.WaitSeconds(3.0f, machineContext);
            //
            //         _uiComboNoticeView.Hide();
            //
            //         _uiFireJackpotNoticeView.Open();
            //
            //         AudioUtil.Instance.PlayAudioFxIfNotPlaying("Jackpot_Open");
            //
            //         await XUtility.WaitSeconds(2.0f, machineContext);
            //
            //         _uiFireJackpotNoticeView.Hide();
            //
            //         _activeState.UpdateRunningWheel(new List<string>() { "WheelJackpotGame" });
            //
            //         _controlPanel.ShowSpinButton(true);
            //     }
            //
            //     Proceed();
            // }
            // else
            // {
            //     TryBackToRespin();
            // }



            //    _wheel.wheelState.SetRollLockState(1, false);

            base.HandleCommonLogic();
        }

        protected override void OnBetChange()
        {
            _uiComboView.Set();
            _uiComboNoticeView.Hide();
            
            betChangedInCurrentStep = true;
 
            machineContext.view.Get<ControlPanel>().StopWinAnimation(false, 0);
             
            var runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if (runningWheel[0].wheelName != "WheelJackpotGame")
            {
                _lineGroupView.PlayIdle();
                StopWinCycle();
            }
            
            SendBiBetChangeLog();
        }
    }
}