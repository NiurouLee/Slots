using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;
using UnityEngine.Rendering;

namespace GameModule
{
	public class FreeGameProxy11030 : FreeGameProxy
	{
		public FreeGameProxy11030(MachineContext context)
		:base(context)
		{

	
		}
		protected override async void HandleFreeStartLogic()
		{
			machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
			// StopBackgroundMusic();
			await ShowFreeSpinStartCutSceneAnimation();
			//UnPauseBackgroundMusic();
			Proceed();
		}
		protected override void HandleFreeReTriggerLogic()
		{
			DealFreeGameReTrigger();
		}
		public async void DealFreeGameReTrigger()
		{
			await ShowFreeSpinTriggerLineAnimation();
			var assetName = "UIFreeGameExtraNotice";
			var popUp = PopUpManager.Instance.ShowPopUp<FreeSpinReTriggerPopUp11030>(assetName);
			popUp.SetPopUpCloseAction(() =>
			{
				UpdateFreeSpinUIState(true, UseAverageBet());
				HandleFreeReTriggerEnd();
			});
			popUp.SetExtraCount(freeSpinState.NewCount);
		}
		protected override void RecoverCustomFreeSpinState()
		{
			// machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
			machineContext.state.Get<WheelsActiveState11030>().UpdateFreeWheelState();
		}
		protected override async Task ShowFreeSpinStartCutSceneAnimation()
		{
		
			// var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("FreeGameStartCutPopup");
			// var sortingGroup = transitionAnimation.GetComponent<SortingGroup>();
			// sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
			// sortingGroup.sortingOrder = 100;
			// transitionAnimation.transform.SetParent(machineContext.transform);

			if (!IsFromMachineSetup())
			{
				AudioUtil.Instance.PlayAudioFx("SwitchBoard");
			}
			RecoverCustomFreeSpinState();
			// await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionFG", machineContext);
			await machineContext.WaitSeconds(0.5f);
			// GameObject.Destroy(transitionAnimation);   
			await base.ShowFreeSpinStartCutSceneAnimation();
		}
		protected override async Task ShowFreeGameFinishPopUp<T>(string address = null)
		{
			if (address == null)
			{
				address = "UIFreeGameFinish" + machineContext.assetProvider.AssetsId;
			}

			if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
			{
				XDebug.LogError($"ShowFreeGameFinishPopUp:{address} is Not Exist" );    
				return;
			}

			TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(waitTask, null);

			var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
			startPopUp.BindFinishAction(() =>
			{
				machineContext.RemoveTask(waitTask);
				waitTask.SetResult(true);
			});

			await waitTask.Task;
		}
		protected override async Task ShowFreeSpinFinishCutSceneAnimation()
		{
			// var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("FreeGameStartCutPopup");
			// var sortingGroup = transitionAnimation.GetComponent<SortingGroup>();
			// sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
			// sortingGroup.sortingOrder = 100;
			// transitionAnimation.transform.SetParent(machineContext.transform);
			machineContext.state.Get<WheelsActiveState11030>().UpdateBaseWheelState();
			AudioUtil.Instance.PlayAudioFx("SwitchBoard");
			RestoreTriggerWheelElement();
			// machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
			AudioUtil.Instance.StopMusic();

			// await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionFG", machineContext);
			// GameObject.Destroy(transitionAnimation);
			await machineContext.WaitSeconds(0.5f);
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}
		protected override void RestoreTriggerWheelElement()
		{
			var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
			if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
			{
				var runningWheel = machineContext.state.Get<WheelsActiveState11030>().GetRunningWheel()[0];
				runningWheel.wheelState.UpdateWheelStateInfo(triggerPanels[0]);
				runningWheel.ForceUpdateElementOnWheel();
				// for (var i = 0; i < runningWheel.GetMaxSpinningUpdaterCount(); i++)
				// {
				// 	var roll = runningWheel.GetRoll(i);
				// 	for (var j = 0; j < runningWheel.GetRollRowCount(0,runningWheel.wheelState.GetWheelConfig()); j++)
				// 	{
				// 		var container = roll.GetVisibleContainer(j);
				// 		if (container.sequenceElement.config.id == 13)
				// 		{
				// 			container.UpdateElement(container.sequenceElement,true);
				// 			container.UpdateElementMaskInteraction(true);
				// 			container.ShiftSortOrder(true);
				// 		}
				// 	}
				// }
			}
		}
	}
}