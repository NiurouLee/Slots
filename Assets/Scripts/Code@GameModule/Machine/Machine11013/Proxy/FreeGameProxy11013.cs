using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;
using GameModule.PopUp;

namespace GameModule
{
	public class FreeGameProxy11013 : FreeGameProxy
	{

		protected Animator animatorTranslation;
		public FreeGameProxy11013(MachineContext context)
		:base(context)
		{

			animatorTranslation = context.transform.Find("TranslationFree").GetComponent<Animator>();
		}
		 
		protected override async Task ShowFreeGameStartPopUp()
		{

			if (!this.IsFromMachineSetup())
			{
				var popUp = PopUpManager.Instance.ShowPopUp<UITotalFreeGameStart11013>($"UITotalFreeGameStart{machineContext.assetProvider.AssetsId}");
				await popUp.SetExtraCount(freeSpinState.LeftCount);
			}
			else
			{
				RestoreTriggerWheelElement();
			}
 
			await ShowFreeGameStartPopUp<FreeSpinStartPopUp>();
		}
		
		
		protected async override Task ShowFreeSpinStartCutSceneAnimation()
		{
			AudioUtil.Instance.PlayAudioFx("FeeGame_Transition");
			animatorTranslation.gameObject.SetActive(true);
			animatorTranslation.Play("Free");
			await machineContext.WaitSeconds(0.76f);
			
			machineContext.state.Get<WheelsActiveState11013>().UpdateRunningWheelState(true);
			Constant11013.ClearStar(machineContext);
			
			await machineContext.WaitSeconds(2.5f - 0.76f);
			animatorTranslation.gameObject.SetActive(false);
			
			await machineContext.view.Get<LockView11013>().ScatterToWild();
			
			await base.ShowFreeSpinStartCutSceneAnimation();
		}


		protected async override Task ShowFreeSpinFinishCutSceneAnimation()
		{ 
			AudioUtil.Instance.PlayAudioFx("FeeGame_Transition");
			animatorTranslation.gameObject.SetActive(true);
			animatorTranslation.Play("Free");
			await machineContext.WaitSeconds(0.76f);
			
			machineContext.state.Get<WheelsActiveState11013>().UpdateRunningWheelState(false);
			RestoreTriggerWheelElement();
			
			
			await machineContext.WaitSeconds(2.5f - 0.76f);
			animatorTranslation.gameObject.SetActive(false);
			
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}
		
		
		protected override void RestoreTriggerWheelElement()
		{
			base.RestoreTriggerWheelElement();
			Constant11013.ClearStar(machineContext);
		}
        
		protected override async void HandleFreeReTriggerLogic()
		{
			var wheel = machineContext.state.Get<WheelsActiveState11013>().GetRunningWheel()[0];

			await wheel.winLineAnimationController.BlinkFreeSpinTriggerLine();

			var assetName = "UIFreeGameExtraNotice" + machineContext.assetProvider.AssetsId;
			if (machineContext.assetProvider.GetAsset<GameObject>(assetName) != null)
			{
				var popUp = PopUpManager.Instance.ShowPopUp<FreeSpinReTriggerPopUp11013>(assetName);
				popUp.SetPopUpCloseAction(() =>
				{
					UpdateFreeSpinUIState(true, UseAverageBet());
					ChangeToWild();
				});
				popUp.SetExtraCount(freeSpinState.NewCount);
			}
			else
			{
				UpdateFreeSpinUIState(true, UseAverageBet());
				ChangeToWild();
			}
		}


		protected async Task ChangeToWild()
		{
			await machineContext.view.Get<LockView11013>().ScatterToWild();
			Proceed();
		}



	}
}