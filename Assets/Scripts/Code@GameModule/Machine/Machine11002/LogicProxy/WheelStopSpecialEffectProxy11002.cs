namespace GameModule
{
    public class WheelStopSpecialEffectProxy11002 : WheelStopSpecialEffectProxy
    {
        private ChillView _baseChillView;
        private ChillView _freeChillView;
        private WheelsActiveState11002 _wheelsActiveState;


        public WheelStopSpecialEffectProxy11002(MachineContext machineContext) : base(machineContext)
        {
            _baseChillView = machineContext.view.Get<ChillView>();
            _freeChillView = machineContext.view.Get<ChillView>(1);
            _wheelsActiveState = machineContext.state.Get<WheelsActiveState11002>();

        }

        protected override async void HandleCustomLogic()
        {
            if (_wheelsActiveState.GetRunningWheel()[0].wheelName == "Wheel1")
            {
                _freeChillView.StopAllFullWildAnimation();
                await _freeChillView.RefreshUI(false);
            }
            else
            {
                _baseChillView.StopAllFullWildAnimation();
                await _baseChillView.RefreshUI(false);
            }

            base.HandleCustomLogic();
        }
    }
}