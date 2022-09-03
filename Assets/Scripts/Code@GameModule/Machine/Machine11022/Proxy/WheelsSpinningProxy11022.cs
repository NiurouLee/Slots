namespace GameModule
{
    public class WheelsSpinningProxy11022:WheelsSpinningProxy
    {
        public WheelsSpinningProxy11022(MachineContext context)
            : base(context)
        {
        }
        public override void OnSpinResultReceived()
        {
            base.OnSpinResultReceived();
            if (Constant11022.debugType)
            {
                machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_STOP);
            }
        }
    }
}