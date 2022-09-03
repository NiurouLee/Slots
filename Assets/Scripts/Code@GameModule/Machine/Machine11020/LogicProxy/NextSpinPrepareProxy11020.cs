

namespace GameModule
{
    public class NextSpinPrepareProxy11020 : NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11020(MachineContext context) 
            : base(context)
        {

        }

        protected override void OnBetChange()
        {
            base.OnBetChange();

            Constant11020.firstInitWheel = false;

            var state = machineContext.state.Get<WheelsActiveState11020>();

            var wheel = state.GetRunningWheel()[0];

            state.RemoveWheelWildElement(wheel);

            machineContext.view.Get<LockedFramesView11020>().StartWheel(wheel, false);
        }
    }
}
