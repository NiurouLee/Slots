using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class NextSpinPrepareProxy11027 : NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11027(MachineContext machineContext) : base(machineContext)
        {
        }
        
        protected override void OnBetChange()
        {
            base.OnBetChange();
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            wheel.GetContext().state.Get<WheelsActiveState11027>().FadeOutRollMask(wheel);
            machineContext.view.Get<WheelRollingView11027>().InitializeWheelView(false,false);
        }
    }
}