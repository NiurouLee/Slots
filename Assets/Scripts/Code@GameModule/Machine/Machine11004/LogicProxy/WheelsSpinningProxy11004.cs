namespace GameModule
{
    public class WheelsSpinningProxy11004: WheelsSpinningProxy
    {
        public WheelsSpinningProxy11004(MachineContext context) : base(context)
        {
        }


        public override async void OnSpinResultReceived()
        {
            var lockElementView = machineContext.view.Get<LockElementView11004>();
            await lockElementView.StartGetLock();
            await lockElementView.RefreshRollingLock();
            
            base.OnSpinResultReceived();
        }
        
        
    }
}