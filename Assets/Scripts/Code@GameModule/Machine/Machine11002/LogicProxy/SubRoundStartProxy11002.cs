namespace GameModule
{
    public class SubRoundStartProxy11002 : SubRoundStartProxy
    {
        private WheelsActiveState11002 _wheelsActiveState;
        private CharacterView11002 _characterView;
        private ChillView _baseChillView;
        private ChillView _freeChillView;


        public SubRoundStartProxy11002(MachineContext context) : base(context)
        {
            _wheelsActiveState = machineContext.state.Get<WheelsActiveState11002>();
            _baseChillView = machineContext.view.Get<ChillView>();
            _freeChillView = machineContext.view.Get<ChillView>(1);
            _characterView = machineContext.view.Get<CharacterView11002>();
        }


        protected override async void HandleCustomLogic()
        {
            var extraState = machineContext.state.Get<ExtraState11002>();
            var batState = machineContext.state.Get<BetState>();


            if (_wheelsActiveState.GetRunningWheel()[0].wheelName == "Wheel1")
            {
                await _freeChillView.RefreshUI(true);
                _characterView.Hide();
            }
            else
            {
                await _baseChillView.RefreshUI(true);
                _characterView.Show();
                _characterView.AnimIdle();
            }

            base.HandleCustomLogic();
        }
    }
}