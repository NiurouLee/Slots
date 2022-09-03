using System.Collections.Generic;

namespace GameModule
{
    public class MachineSetUpProxy11035 : MachineSetUpProxy
    {
        private UIComboView _uiComboView;

        private UIComboNoticeView _uiComboNoticeView;

        private UIFireJackpotNoticeView _uiFireJackpotNoticeView;

        private ExtraState11035 _extraState;

        private WheelsActiveState11035 _activeState;

        private ReSpinState _respinState;

        public MachineSetUpProxy11035(MachineContext machineContext) : base(machineContext)
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
        }

        protected override void HandleCustomLogic()
        {
            if (_respinState.IsInRespin || _respinState.NextIsReSpin || _extraState.HasJackpotWheel())
            {
                var triggerPanels = _respinState.triggerPanels;

                if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
                {
                    machineContext.state.Get<WheelState>().UpdateWheelStateInfo(triggerPanels[0]);

                    machineContext.view.Get<Wheel>().ForceUpdateElementOnWheel();
                }

                machineContext.JumpToLogicStep(LogicStepType.STEP_RE_SPIN, LogicStepType.STEP_MACHINE_SETUP);
            }
            else
            {
                base.HandleCustomLogic();
            }
        }

        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();

            _uiComboView.Set();

            _uiComboNoticeView.Hide();

            _uiFireJackpotNoticeView.Hide();
        }
    }
}