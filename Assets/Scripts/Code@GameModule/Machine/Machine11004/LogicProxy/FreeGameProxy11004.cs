using System.Threading.Tasks;

namespace GameModule
{
    public class FreeGameProxy11004: FreeGameProxy
    {
        public FreeGameProxy11004(MachineContext context) : base(context)
        {
        }


        protected override Task ShowFreeSpinStartCutSceneAnimation()
        {
            
            machineContext.state.Get<WheelsActiveState11004>().UpdateRunningWheelState(false,true);
            return base.ShowFreeSpinStartCutSceneAnimation();
        }


        protected override Task ShowFreeSpinFinishCutSceneAnimation()
        {
            
            machineContext.state.Get<WheelsActiveState11004>().UpdateRunningWheelState(false,false);

            RestoreTriggerWheelElement();
            return base.ShowFreeSpinFinishCutSceneAnimation();
        }

        protected override async void HandleFreeReTriggerLogic()
        {
            await machineContext.view.Get<Wheel>().winLineAnimationController.BlinkFreeSpinTriggerLine();

            base.HandleFreeReTriggerLogic();
        }
    }
}