namespace GameModule
{
    public class NextSpinPrepareProxy11006: NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11006(MachineContext context) : base(context)
        {
        }
        
        
        protected override async void HandleCommonLogic()
        {
            base.HandleCommonLogic();
 
            if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                await machineContext.WaitSeconds(1.0f);
               
                CheckAndStartAutoSpin();
            }
            else
            {
                machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
                var freeSpinState = machineContext.state.Get<FreeSpinState>();
                if (freeSpinState.IsInFreeSpin && !freeSpinState.NextIsFreeSpin)
                {
                    
                }
                else
                {
                    CheckAndPlayWinCycle();
                }
            }
        }
    }
}