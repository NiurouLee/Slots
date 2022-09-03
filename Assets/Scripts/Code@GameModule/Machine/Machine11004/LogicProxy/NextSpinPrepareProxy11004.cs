namespace GameModule
{
    public class NextSpinPrepareProxy11004 : NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11004(MachineContext context) : base(context)
        {
        }

        protected override void OnBetChange()
        {
            base.OnBetChange();
            
            var extraState = machineContext.state.Get<ExtraState11004>();
            var lockData = extraState.GetLockData();
            
            

            
            if (lockData != null && !lockData.IsOver)
            {
                machineContext.view.Get<LockElementView11004>().RefreshLockNoAnim();
                machineContext.view.Get<BaseTitleView11004>().RefreshUI();
            }
            else
            {
                machineContext.view.Get<LockElementView11004>().ClearLock();
                machineContext.view.Get<BaseTitleView11004>().ClearUI();
            }
        }
    }
}