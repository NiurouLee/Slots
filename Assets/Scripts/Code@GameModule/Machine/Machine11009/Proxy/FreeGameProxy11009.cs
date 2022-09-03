using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class FreeGameProxy11009 : FreeGameProxy
	{

		protected WheelsActiveState11009 wheelsActiveState;
		protected ExtraState11009 extraState;

		
		public FreeGameProxy11009(MachineContext context)
		:base(context)
		{
			wheelsActiveState = machineContext.state.Get<WheelsActiveState11009>();
			extraState = machineContext.state.Get<ExtraState11009>();
		}

		


		protected override async void HandleFreeFinishLogic()
		{
			

			if (extraState.HasBoxActivate())
			{

				//不结算，开始下一轮free
				if (freeSpinState.FreeNeedSettle)
				{
					await freeSpinState.SettleFreeSpin();
				}

				await ShowFreeGameStartPopUp();
				await ShowFreeSpinStartCutSceneAnimation();
				Proceed();
			}
			else
			{
				base.HandleFreeFinishLogic();
			}
			
		}


		protected override async Task ShowFreeGameStartPopUp()
		{
			var extraInfo = machineContext.state.Get<ExtraState11009>();
			bool isGreen = extraInfo.GetFreeGreenState();
			bool isRed = extraInfo.GetFreeRedState();
			bool isPurple = extraInfo.GetFreePurpleState();

            
            
			if (isGreen && isRed && isPurple)
			{
				//绿红紫
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameSuperFeatureStart11009");
			}
			else if(isGreen && isRed)
			{
				//绿红
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameJPAndProsperityStart11009");
			}
			else if(isRed && isPurple)
			{
				//红紫
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameJPAndLongevityStart11009");

			}
			else if(isGreen && isPurple)
			{
				//绿紫
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameProsperityAndLongevityStart11009");

			}
			else if(isGreen)
			{
				//绿
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameProsperityStart11009");

			}
			else if(isRed)
			{
				//红
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameJPStart11009");

			}
			else if (isPurple)
			{
				//紫
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameLongevityStart11009");

			}

			
		}

		protected async override Task ShowFreeSpinStartCutSceneAnimation()
		{

			machineContext.view.Get<TransitionView11009>().RefreshUI();

			await machineContext.WaitSeconds(1);
			
			wheelsActiveState.UpdateRunningWheelState(true);
			
			
			
			await machineContext.WaitSeconds(1.43f);
			
			await base.ShowFreeSpinStartCutSceneAnimation();
		}


		protected override async Task ShowFreeGameFinishPopUp()
		{
			await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>("UIFreeGameFinish11009");
		}

		protected async override Task ShowFreeSpinFinishCutSceneAnimation()
		{
			machineContext.view.Get<TransitionView11009>().RefreshUI();
			
			await machineContext.WaitSeconds(1);
			
			wheelsActiveState.UpdateRunningWheelState(false);
			
			RestoreTriggerWheelElement();
			wheelsActiveState.CloseElementMask();
			
			await machineContext.WaitSeconds(1.43f);
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}

	}
}