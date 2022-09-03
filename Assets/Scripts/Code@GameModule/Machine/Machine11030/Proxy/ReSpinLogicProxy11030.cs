using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;
using UnityEngine.Rendering;

namespace GameModule
{
	public class ReSpinLogicProxy11030 : ReSpinLogicProxy
	{
		public ExtraState11030 extraState;
		public WheelsActiveState11030 wheelActiveState;
		public List<GameObject> lockList = new List<GameObject>();
		public ReSpinLogicProxy11030(MachineContext context)
		:base(context)
		{
			extraState = machineContext.state.Get<ExtraState11030>();
			wheelActiveState = machineContext.state.Get<WheelsActiveState11030>();
		}
		protected override async void HandleCustomLogic()
		{
			var freeSpinState = machineContext.state.Get<FreeSpinState11030>();
			if (freeSpinState.NewCount > 0)
			{
				var wheel = wheelActiveState.GetRunningWheel()[0];
				await wheel.winLineAnimationController.BlinkFreeSpinTriggerLine();
				var assetName = "UIFreeGameExtraNotice";
				var popUp = PopUpManager.Instance.ShowPopUp<FreeSpinReTriggerPopUp11030>(assetName);
				var popEndTask = new TaskCompletionSource<bool>();
				popUp.SetPopUpCloseAction(() =>
				{
					var controlPanel = machineContext.view.Get<ControlPanel>();
					controlPanel.UpdateControlPanelState(true,false);
					controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount-1, freeSpinState.TotalCount);
					popEndTask.SetResult(true);
				});
				popUp.SetExtraCount(freeSpinState.NewCount);
				await popEndTask.Task;
			}
			
			if (reSpinState.ReSpinTriggered)
			{
				await HandleReSpinStartLogic();
			}

			if (IsFromMachineSetup())
			{
				var runningWheel = wheelActiveState.GetRunningWheel()[0]; 
				runningWheel.wheelState.UpdateWheelStateInfo(reSpinState.triggerPanels[0]);
				runningWheel.ForceUpdateElementOnWheel();
				var stickyRollList = extraState.GetStickyColumnsIndex();
				var wheelState = runningWheel.wheelState;
				for (var i = 0; i < stickyRollList.Count; i++)
				{
					wheelState.SetRollLockState(stickyRollList[i], true);
					var roll = runningWheel.GetRoll(stickyRollList[i]);
					var lockObj = machineContext.assetProvider.InstantiateGameObject("FreeLock");
					lockList.Add(lockObj);
					lockObj.SetActive(true);
					var lockPosition = roll.transform.position;
					// lockPosition.y = 0;
					lockObj.transform.position = lockPosition;
					lockObj.transform.SetParent(runningWheel.transform);
					var lockAnimator = lockObj.GetComponent<Animator>();
					XUtility.PlayAnimation(lockAnimator,"FreeLock");
				}
			}

			if (reSpinState.ReSpinNeedSettle)
			{
				await SettleRespin();
			}
            
			Proceed();
		}
		protected override async Task HandleReSpinStartLogic()
		{

			if (!IsFromMachineSetup())
			{
				await ShowReSpinTriggerLineAnimation();
			}
			var stickyRollList = extraState.GetStickyColumnsIndex();
			var runningWheel = wheelActiveState.GetRunningWheel()[0];
			var wheelState = runningWheel.wheelState;
			for (var i = 0; i < stickyRollList.Count; i++)
			{
				wheelState.SetRollLockState(stickyRollList[i], true);
				var roll = runningWheel.GetRoll(stickyRollList[i]);
				var lockObj = machineContext.assetProvider.InstantiateGameObject("FreeLock");
				lockList.Add(lockObj);
				lockObj.SetActive(true);
				var lockPosition = roll.transform.position;
				// lockPosition.y = 0;
				lockObj.transform.SetParent(runningWheel.transform);
				lockObj.transform.position = lockPosition;
				lockObj.transform.localScale = Vector3.one;
				var lockAnimator = lockObj.GetComponent<Animator>();
				XUtility.PlayAnimation(lockAnimator,"Intro");
			}
			AudioUtil.Instance.PlayAudioFx("FreeSpin_LockReel");
			await machineContext.WaitSeconds(0.5f);
		}

		public async Task SettleRespin()
		{
			var runningWheel = wheelActiveState.GetRunningWheel()[0];
			var wheelState = runningWheel.wheelState;
			for (var i = 0; i < wheelState.GetWheelConfig().rollCount; i++)
			{
				wheelState.SetRollLockState(i, false);
				var roll = runningWheel.GetRoll(i);
			}

			while (lockList.Count > 0)
			{
				GameObject.Destroy(lockList.Pop());
			}
			await extraState.SettleBonusProgress();
		}
	}
}