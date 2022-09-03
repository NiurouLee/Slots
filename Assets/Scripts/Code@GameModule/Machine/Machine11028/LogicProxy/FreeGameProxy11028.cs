//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-12 19:56
//  Ver : 1.0.0
//  Description : FreeGameProxy11028.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;

namespace GameModule
{
    public class FreeGameProxy11028:FreeGameProxy
    {
        public FreeGameProxy11028(MachineContext context)
            : base(context)
        {
        }
        
        protected override void RecoverCustomFreeSpinState()
        {
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
        }

        protected override async Task ShowFreeGameStartPopUp()
        {
            await base.ShowFreeGameStartPopUp();
            var bonusProxy11028 = machineContext.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11028;
            bonusProxy11028?.CloseWheelPopup();
            await machineContext.WaitSeconds(1);
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            machineContext.view.Get<TransitionView11028>(1).PlayFreeTransitionAnimation();
            await machineContext.WaitSeconds(1);
            RestoreTriggerWheelElement();
            machineContext.view.Get<BackgroundView11028>().UpdateFreeBackground(false,false);
            await machineContext.WaitSeconds(2f);
            machineContext.view.Get<TransitionView11028>(1).Hide();
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            await base.ShowFreeSpinFinishCutSceneAnimation();
        }

        public override void RecoverFreeSpinStateWhenRoomSetup()
        {
            machineContext.view.Get<BackgroundView11028>().UpdateFreeBackground(true,machineContext.state.Get<ExtraState11028>().IsNight);
            base.RecoverFreeSpinStateWhenRoomSetup();
        }

        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0]
                .ToggleAttachedGameObject("ElementAnticipation", false);
            machineContext.view.Get<TransitionView11028>(1).PlayFreeTransitionAnimation();
            await machineContext.WaitSeconds(1);
            machineContext.view.Get<BackgroundView11028>().UpdateFreeBackground(true,machineContext.state.Get<ExtraState11028>().IsNight);
            await machineContext.WaitSeconds(2f);
            machineContext.view.Get<TransitionView11028>(1).Hide();
            await base.ShowFreeSpinStartCutSceneAnimation();
           
        }

        protected override async void HandleFreeReTriggerLogic()
        {
            machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0]
                .ToggleAttachedGameObject("ElementAnticipation", false);
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            await wheels[0].winLineAnimationController.BlinkFreeSpinTriggerLine();
            base.HandleFreeReTriggerLogic();
        }
    }
}