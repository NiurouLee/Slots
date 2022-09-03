namespace GameModule
{
    public class WheelsSpinningProxy11035 : WheelsSpinningProxy
    {
        public WheelsSpinningProxy11035(MachineContext context) : base(context)
        {
        }

        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            base.OnMachineInternalEvent(internalEvent, args);

            if (internalEvent == MachineInternalEvent.EVENT_CONTROL_STOP)
            {
                AudioUtil.Instance.StopAudioFx("11035_JackpotRolling");
            }
        }
    }
}