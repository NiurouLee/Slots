using System.Threading.Tasks;

namespace GameModule
{
    public class FreeGameProxy11015:FreeGameProxy
    {
        public FreeGameProxy11015(MachineContext context) : base(context)
        {
        }
        


        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }
        
       


        protected async override Task ShowFreeSpinStartCutSceneAnimation()
        {

             
            AudioUtil.Instance.PlayAudioFxOneShot("FreeGame_Trantion");
            machineContext.view.Get<TransitionView11015>().OpenTransition();
            await machineContext.WaitSeconds(1.666f);
            
            machineContext.state.Get<WheelsActiveState11015>().UpdateRunningWheelState(true);

            await machineContext.WaitSeconds(2.766f - 1.666f);
            
            await base.ShowFreeSpinStartCutSceneAnimation();
        }


        protected async override Task ShowFreeSpinFinishCutSceneAnimation()
        { 
            AudioUtil.Instance.PlayAudioFxOneShot("FreeGame_Trantion");
            machineContext.view.Get<TransitionView11015>().OpenTransition();
            await machineContext.WaitSeconds(1.666f);
            
            machineContext.state.Get<WheelsActiveState11015>().UpdateRunningWheelState(false);
            
            await machineContext.WaitSeconds(2.766f - 1.666f);
            
            await base.ShowFreeSpinFinishCutSceneAnimation();
        }

        protected override async void HandleFreeReTriggerLogic()
        {
            var wheel = machineContext.state.Get<WheelsActiveState11015>().GetRunningWheel()[0];
            await wheel.winLineAnimationController.BlinkFreeSpinTriggerLine();

            base.HandleFreeReTriggerLogic();
        }
    }
}