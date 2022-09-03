using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class FreeGameProxy11019 : FreeGameProxy
	{
		public FreeGameProxy11019(MachineContext context)
		:base(context)
		{

	
		}


		protected override void RegisterInterestedWaitEvent()
		{
			waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
		}
		


		protected override void RecoverCustomFreeSpinState()
		{
			var winState = machineContext.state.Get<WinState>();
			machineContext.view.Get<BonusWinView11019>(Constant11019.FreeBonusWinViewName).RefreshWin((long)winState.displayTotalWin);

			base.RecoverCustomFreeSpinState();
		}


		protected async override Task ShowFreeSpinStartCutSceneAnimation()
		{
			 machineContext.view.Get<TransitionsView11019>().PlayFreeTransition();
			 await machineContext.WaitSeconds(1.6f);
			machineContext.state.Get<WheelsActiveState11019>().UpdateRunningWheelState(false,true);
			
			var winState = machineContext.state.Get<WinState>();
			machineContext.view.Get<BonusWinView11019>(Constant11019.FreeBonusWinViewName).RefreshWin((long)winState.displayTotalWin);

			
			await machineContext.WaitSeconds(3 - 1.6f);
			await base.ShowFreeSpinStartCutSceneAnimation();
		}


		protected async override Task ShowFreeSpinFinishCutSceneAnimation()
		{ 
			machineContext.view.Get<TransitionsView11019>().PlayFreeTransition();
			await machineContext.WaitSeconds(1.6f);
			machineContext.state.Get<WheelsActiveState11019>().UpdateRunningWheelState(false,false);
			RestoreTriggerWheelElement();
			
			await machineContext.WaitSeconds(3 - 1.6f);
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}

		protected override async void HandleFreeReTriggerLogic()
		{
			var wheel = machineContext.state.Get<WheelsActiveState11019>().GetRunningWheel()[0];
			await wheel.winLineAnimationController.BlinkFreeSpinTriggerLine();

			base.HandleFreeReTriggerLogic();
		}
	}
}