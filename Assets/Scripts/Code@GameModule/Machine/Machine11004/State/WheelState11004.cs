using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelState11004: WheelState
    {
        public WheelState11004(MachineState state) : base(state)
        {
        }


        protected SSpin nowSpinResult;
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            nowSpinResult = spinResult;
            base.UpdateStateOnReceiveSpinResult(spinResult);
        }

        public SSpin GetSpinResult()
        {
            return nowSpinResult;
        }
    }
}