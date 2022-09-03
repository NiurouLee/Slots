namespace GameModule
{
    public class NextSpinPrepareProxy11021: NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11021(MachineContext context) : base(context)
        {
        }
        
        
        
        protected override  void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<JackpotPanel11021>().EnableButtonResponse(true);
            }
        }
        
        public override void OnAutoSpinStopClicked()
        {
            base.OnAutoSpinStopClicked();
            
            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<JackpotPanel11021>().EnableButtonResponse(true);
            }
        }

        public override void StartNextSpin()
        {
            machineContext.view.Get<JackpotPanel11021>().EnableButtonResponse(false);
          
            base.StartNextSpin();
        }
         
        protected override void OnBetChange()
        {
            base.OnBetChange();
            var prizeView = machineContext.view.Get<TitlePrizeView>();
            prizeView.RefreshInfo();
            prizeView.VisibleItemsAll();
            machineContext.view.Get<JackpotPanel11021>().UpdateJackpotLockState(true,false);
           
        }


        protected override void OnUnlockBetFeatureConfigChanged()
        {
            machineContext.view.Get<JackpotPanel11021>().UpdateJackpotLockState(false,false);

        }
    }
}