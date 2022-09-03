using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class FreeGameProxy11012 : FreeGameProxy
	{
		public FreeGameProxy11012(MachineContext context)
		:base(context)
		{

	
		}
		
		
		protected override void RecoverCustomFreeSpinState()
		{
			base.RecoverCustomFreeSpinState();
		}


		protected async override Task ShowFreeSpinStartCutSceneAnimation()
		{
			machineContext.view.Get<TransitionView11012>().OpenFreeCut();

			await machineContext.WaitSeconds(3.26f);
			
			machineContext.state.Get<WheelsActiveState11012>().UpdateRunningWheelState(false,true);
			
			await machineContext.WaitSeconds(4.13f - 3.26f);
			
			await base.ShowFreeSpinStartCutSceneAnimation();
		}


		protected async override Task ShowFreeSpinFinishCutSceneAnimation()
		{ 
			machineContext.view.Get<TransitionView11012>().OpenFreeCut();

			await machineContext.WaitSeconds(3.26f);
			
			machineContext.state.Get<WheelsActiveState11012>().UpdateRunningWheelState(false,false);
			RestoreTriggerWheelElement();
			Constant11012.ClearDoor(machineContext);
			
			await machineContext.WaitSeconds(4.13f - 3.26f);
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