//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-24 20:38
//  Ver : 1.0.0
//  Description : FreeGameProxy11010.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;

namespace GameModule
{
    public class FreeGameProxy11010: FreeGameProxy
    {
        public FreeGameProxy11010(MachineContext context) : base(context)
        {
        }

        protected override async Task ShowFreeGameStartPopUp()
        {
            if (IsFromMachineSetup())
            {
                RestoreTriggerWheelElement();
            }
            await base.ShowFreeGameStartPopUp();
        }

        protected override void RecoverCustomFreeSpinState()
        {
            machineContext.state.Get<WheelsActiveState11010>().UpdateFreeWheelState();
        }

        protected override Task ShowFreeSpinStartCutSceneAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            machineContext.state.Get<WheelsActiveState11010>().UpdateFreeWheelState();
            return base.ShowFreeSpinStartCutSceneAnimation();
        }

        protected override Task ShowFreeSpinFinishCutSceneAnimation()
        {
            machineContext.state.Get<WheelsActiveState11010>().UpdateBaseWheelState();
            RestoreTriggerWheelElement();
            return base.ShowFreeSpinFinishCutSceneAnimation();
        }
        
        
        protected override async void HandleFreeReTriggerLogic()
        {
            AudioUtil.Instance.PauseMusic();
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            await wheels[0].winLineAnimationController.BlinkFreeSpinTriggerLine();
            base.HandleFreeReTriggerLogic();
        }
    }
}