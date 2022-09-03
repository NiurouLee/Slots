namespace GameModule
{
    public class ControlPanelWinUpdateProxy11035 : ControlPanelWinUpdateProxy
    {
        private UIComboView _uiComboView;

        private UIComboNoticeView _uiComboNoticeView;

        private ExtraState11035 _extraState;
        private WheelsActiveState11035 _activeState;

        public ControlPanelWinUpdateProxy11035(MachineContext context)
            : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();

            _uiComboView = machineContext.view.Get<UIComboView>();

            _uiComboNoticeView = machineContext.view.Get<UIComboNoticeView>();

            _extraState = machineContext.state.Get<ExtraState11035>();

            _activeState = machineContext.state.Get<WheelsActiveState11035>();
        }

        protected override async void HandleCustomLogic()
        {
            if (_activeState.GetRunningWheel()[0].wheelName == "WheelBaseGame")
            {
                _uiComboView.Set(true);

                await _uiComboNoticeView.Set();
            }
            else
            {
                _uiComboView.Set();

                _uiComboNoticeView.Hide();
            }
 
            base.HandleCustomLogic();
        }
    }
}