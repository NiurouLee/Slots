namespace GameModule
{
    public class SpecialBonusProxy11021:SpecialBonusProxy
    {
        public SpecialBonusProxy11021(MachineContext context) : base(context)
        {
        }


        protected override void RegisterInterestedWaitEvent()
        {
            base.RegisterInterestedWaitEvent();
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }
     

        protected async override void HandleCustomLogic()
        {
            await machineContext.WaitSeconds(0.5f);

            await machineContext.view.Get<TitlePrizeView>().CollectItems(this);
            base.HandleCustomLogic();
        }
        
      
        
        
        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            switch (internalEvent)
            {
                case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
                {
                    var waitEvent = (WaitEvent) args[0];
                    if (waitEvent == WaitEvent.WAIT_WIN_NUM_ANIMTION)
                    {
                        machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
                        machineContext.view.Get<TitlePrizeView>().NiceWinComplet();
                        //Proceed();
                    }
                    // else
                    // {
                    //     SafeHandleLogic();
                    // }

                    break;
                }
                case MachineInternalEvent.EVENT_CONTROL_STOP:
                    machineContext.view.Get<ControlPanel>().StopWinAnimation();
                    break;
                
                case MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN_STOP:
                    if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    {
                        machineContext.state.Get<AutoSpinState>().OnDisableAutoSpin();
                        machineContext.view.Get<ControlPanel>().ShowStopButton(true);
                    }
                    break;
            }
        
            base.OnMachineInternalEvent(internalEvent, args);
        }
    }
}