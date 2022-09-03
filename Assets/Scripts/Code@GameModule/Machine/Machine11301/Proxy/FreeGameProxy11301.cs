using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class FreeGameProxy11301 : FreeGameProxy
	{
		public FreeGameProxy11301(MachineContext context)
		:base(context)
		{

	
		}
		
		public override bool UseAverageBet()
        {
            return (freeSpinState as FreeSpinState11301).IsAvgBet;
        }
		protected override void RecoverCustomFreeSpinState()
		{
			base.RecoverCustomFreeSpinState();
			machineContext.state.Get<BetState11301>().UpdateTotalBetForState();
			machineContext.view.Get<TransitionView11301>().UpdateBgAnim(true);
			machineContext.view.Get<JackPotPanel>().transform.localPosition = new Vector3(0.88f,machineContext.view.Get<JackPotPanel>().transform.localPosition.y,0);
		}

		protected override async Task ShowFreeGameStartPopUp()
        {
			if(freeSpinState.freeSpinId>0){
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UISuperFreeStart11301");
			}else{
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>();
			}
        }
		protected override async Task ShowFreeGameFinishPopUp()
        {
			if(freeSpinState.freeSpinId>0){
				await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>("UISuperFreeFinish11301");
			}else{
				await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>();
			}
        }

		protected async override Task ShowFreeSpinStartCutSceneAnimation()
		{
			
			// List<Task> listTask = new List<Task>();
			machineContext.WaitSeconds(2, () =>
			{
				machineContext.state.Get<BetState11301>().UpdateTotalBetForState();
				machineContext.view.Get<TransitionView11301>().UpdateBgAnim(true);
				machineContext.view.Get<JackPotPanel>().transform.localPosition = new Vector3(0.88f,machineContext.view.Get<JackPotPanel>().transform.localPosition.y,0);
				machineContext.state.Get<WheelsActiveState11301>().UpdateRunningWheelState(false,true);
                var extraState = machineContext.state.Get<ExtraState11301>();
                if (freeSpinState.NextIsFreeSpin && freeSpinState.freeSpinId > 0 && freeSpinState.LeftCount == freeSpinState.TotalCount && extraState.SuperFreePattern != null)
                {
                    machineContext.view.Get<DoorView11301>().LockFreeDoorLogic(extraState.SuperFreePattern);
                }

            });
			machineContext.view.Get<TransitionView11301>().OpenFreeCut();
            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
			await machineContext.WaitSeconds(3.26f);

            // await machineContext.WaitSeconds(4.13f - 3.26f);
       
			// if (listTask.Count > 0)
            //     await Task.WhenAll(listTask);

            machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
			
			await base.ShowFreeSpinStartCutSceneAnimation();
		}


		protected async override Task ShowFreeSpinFinishCutSceneAnimation()
		{
			
			UpdateFreeSpinUIState(true, UseAverageBet());
		
            machineContext.WaitSeconds(2, () =>
            {
				machineContext.state.Get<BetState11301>().UpdateTotalBetForState();
				RestoreBet();
				
				UpdateFreeSpinUIState(false, false);
				
                machineContext.view.Get<TransitionView11301>().UpdateBgAnim(false);
				machineContext.view.Get<JackPotPanel>().transform.localPosition = new Vector3(0.35f,machineContext.view.Get<JackPotPanel>().transform.localPosition.y,0);
				machineContext.state.Get<WheelsActiveState11301>().UpdateRunningWheelState(false,false);
				RestoreTriggerWheelElement();
				Constant11301.ClearDoor(machineContext);
				Constant11301.IsShowMapFeature = machineContext.state.Get<ExtraState11301>().IsMapFeature();
				
            });
			machineContext.view.Get<TransitionView11301>().OpenFreeCut();
            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);

			await machineContext.WaitSeconds(3.26f);
            machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
			
			// await machineContext.WaitSeconds(4.13f - 3.26f);
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}
		
		protected override async void HandleFreeReTriggerLogic()
		{
			var wheel = machineContext.state.Get<WheelsActiveState11013>().GetRunningWheel()[0];

			await wheel.winLineAnimationController.BlinkFreeSpinTriggerLine();

			base.HandleFreeReTriggerLogic();
		}

	}
}