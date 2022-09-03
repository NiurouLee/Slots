namespace GameModule
{
    public class MachineSetUpProxy11004: MachineSetUpProxy
    {
        public MachineSetUpProxy11004(MachineContext context) : base(context)
        {
            
        }
        
        protected override void HandleCustomLogic()
        {
            base.HandleCustomLogic();
            
            machineContext.view.Get<LockElementView11004>().RefreshLockNoAnim();
            machineContext.view.Get<LockElementView11004>().ClearLock();
        }
    }
}