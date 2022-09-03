namespace GameModule
{
    public class NextSpinPrepareProxy11015: NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11015(MachineContext context) : base(context)
        {
        }
        
        
        
        protected override bool NeedPlayWinCycle()
        {
            return !betChangedInCurrentStep && !machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
        }
    }
}