using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using UnityEngine.Rendering;

namespace GameModule
{
	public class FreeGameProxy11025 : FreeGameProxy
	{
		
		private ExtraState11025 _extraState;
		public ExtraState11025 extraState
		{
			get
			{
				if (_extraState == null)
				{
					_extraState =  machineContext.state.Get<ExtraState11025>();
				}
				return _extraState;
			}
		}
		public FreeGameProxy11025(MachineContext context)
		:base(context)
		{

	
		}

		private WheelsActiveState11025 _wheelsActiveState;
		public WheelsActiveState11025 wheelsActiveState
		{
			get
			{
				if (_wheelsActiveState == null)
				{
					_wheelsActiveState =  machineContext.state.Get<WheelsActiveState11025>();
				}
				return _wheelsActiveState;
			}
		}
		protected override void HandleCommonLogic()
		{
			if (IsFromMachineSetup())
				return;
            
			StopWinCycle();
            
			if (IsFreeSpinTriggered() && !IsFromMachineSetup())
			{
				HandleFreeStartLogic();
				return;
			}
          
			if (!NextSpinIsFreeSpin())
			{
				HandleFreeFinishLogic();
				return;
			}

			if (IsFreeSpinReTriggered())
			{
				HandleFreeReTriggerLogic();
				return;
			}
            
			{
				UpdateFreeSpinUIState(true, UseAverageBet());
				Proceed();
			}
		}
		protected override void RecoverLogicState()
		{
			// if (IsFreeSpinTriggered())
			// {
			// 	HandleFreeStartLogic();
			// }
			// else
			// {
				RecoverFreeSpinStateWhenRoomSetup();

				UpdateFreeSpinUIState(true, UseAverageBet());

				if (NeedSettle())
				{
					HandleFreeFinishLogic();
				}
				else
				{
					Proceed();
				}
			// }
		}
		protected override void RecoverCustomFreeSpinState()
		{
			wheelsActiveState.UpdateFreeWheelState();
			if (extraState.GetFreeData().IsSuper)
			{
				var multiWheel = machineContext.view.Get<FreeMultiWheel11025>();
				multiWheel.ChangeToMulti();
			}
			machineContext.view.Get<MachineSystemWidgetView>().SetActive(false);
		}
		
		public async Task ShowFreeMultiWheel()
		{
			var multiWheel = machineContext.view.Get<FreeMultiWheel11025>();
			var wheelTrans = machineContext.assetProvider.InstantiateGameObject("TransitionWheel");
			var sortingGroup = wheelTrans.AddComponent<SortingGroup>();
			sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
			sortingGroup.sortingOrder = 5000;
			wheelTrans.transform.SetParent(machineContext.transform.Find("Wheels"),false);
			wheelTrans.transform.localPosition = new Vector3(0, 4.81f, 0);
			wheelTrans.SetActive(true);
			XUtility.PlayAnimation(wheelTrans.GetComponent<Animator>(), "Transition", () =>
			{
				GameObject.Destroy(wheelTrans);
			},machineContext);
			AudioUtil.Instance.PlayAudioFx("Store_SuperFree_Wheel_Video");
			await machineContext.WaitSeconds(1.2f);
			await machineContext.WaitSeconds(0.8f);
			AudioUtil.Instance.PlayAudioFx("Store_SuperFree_Wheel_Boom");
			await multiWheel.PerformMulti();
		}
		protected override async Task ShowFreeGameFinishPopUp()
		{
			if (Constant11025.NormalFreeId.Contains(freeSpinState.freeSpinId) || Constant11025.ShopNormalFreeId.Contains(freeSpinState.freeSpinId))
			{
				await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>("UIFreeGameFinish11025");
			}
			else if (Constant11025.ShopSuperFreeId.Contains(freeSpinState.freeSpinId))
			{
				await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>("UIStoreGameFinish11025");
			}
			else if (Constant11025.ShopSpecialFreeId.Contains(freeSpinState.freeSpinId))
			{
				await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>("UIStoreGameSpecialFinish11025");
			}
		}
		protected override async Task ShowFreeGameStartPopUp()
		{
			if (Constant11025.NormalFreeId.Contains(freeSpinState.freeSpinId))
			{
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp11025>();
			}
		}
		protected override async void HandleFreeStartLogic()
		{
			StopBackgroundMusic();
			if (!IsFromMachineSetup() && Constant11025.NormalFreeId.Contains(freeSpinState.freeSpinId))
			{
				await ShowFreeSpinTriggerLineAnimation();
			}

			await ShowFreeGameStartPopUp();
            
			await ShowFreeSpinStartCutSceneAnimation();

			if (extraState.GetFreeData().IsSuper)
			{
				await ShowFreeMultiWheel();
			}
			//UnPauseBackgroundMusic();
			Proceed();
		}
		protected override async Task ShowFreeSpinStartCutSceneAnimation()
		{
			//更新Bet到FreeSpin的TriggerBet
			//正常情况下FreeSpin的Bet和Base的Bet是一致的，但是有点关卡会使用平均BET
			//所以这里要将Bet在触发的时候设置成FreeSpinBet，保证jackpot等其他依赖BET的View上的数值计算正确
			//如果使用AverageBet，这里ControlPanel上面的会显示AverageBet的文字，不显示具体的数字，
			GameObject transitionAnimation;
			if (Constant11025.NormalFreeId.Contains(freeSpinState.freeSpinId))
			{
				transitionAnimation = machineContext.assetProvider.InstantiateGameObject("Transition");	
			}
			else
			{
				transitionAnimation = machineContext.assetProvider.InstantiateGameObject("Transition");
			}

			AudioUtil.Instance.PlayAudioFx("Video1");
			// var sortingGroup = transitionAnimation.AddComponent<SortingGroup>();
			// sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
			// sortingGroup.sortingOrder = 5000;
			transitionAnimation.transform.SetParent(machineContext.transform);
			transitionAnimation.SetActive(true);
			XUtility.PlayAnimation(transitionAnimation.GetComponent<Animator>(),"Transition", () =>
			{
				GameObject.Destroy(transitionAnimation);
			},machineContext);
			await machineContext.WaitSeconds(1.5f);
			machineContext.view.Get<MachineSystemWidgetView>().SetActive(false);
			machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
			UpdateSpinUiViewTotalBet(false,false);
			UpdateFreeSpinUIState(true, UseAverageBet());
			controlPanel.ShowSpinButton(false);
			wheelsActiveState.UpdateFreeWheelState();

			await machineContext.WaitSeconds(3f);
		}
		public override bool UseAverageBet()
		{
			if (freeSpinState.freeSpinId != 0)
				return true;
			return false;
		}

		protected override async void HandleFreeFinishLogic()
		{
			var freeWheel = machineContext.view.Get<WheelFree11025>();
			if (!IsFromMachineSetup())
			{
				var freeData = extraState.GetFreeData();
				var countList = new List<int>() {0, 0, 0, 0, 0};
				for (var i = 0; i < freeData.StickyItems.Count; i++)
				{
					var item = freeData.StickyItems[i];
					countList[(int)item.X]++;
				}

				machineContext.view.Get<FreeMultiWheel11025>().SetLower(true);
				await freeWheel.PrepareCollectAll(countList);
				for (var i = 0; i < countList.Count; i++)
				{
					if (countList[i] > 0)
					{
						await PerformFreeCollectRoll(i,i==countList.Count-1 || countList[i+1] == 0);
					}
				}
				machineContext.view.Get<FreeMultiWheel11025>().SetLower(false);
				for (var i = 0; i < countList.Count; i++)
				{
					if (countList[i] > 0)
					{
						await CollectToFloor(i);
						await machineContext.WaitSeconds(0.5f);
					}
				}
			
				await machineContext.WaitSeconds(1.8f);
				// var winState = machineContext.state.Get<WinState>();
            
				// if (winState.displayCurrentWin == 0)
				// {
					// await machineContext.WaitSeconds(0.8f);
				// }	
			}
			else
			{
				freeWheel.CleanWheel();
			}
			StopBackgroundMusic();
			await ShowFreeGameFinishPopUp();
			
			await ShowFreeSpinFinishCutSceneAnimation();
			await ShowFreeSpinBigWinEffect();
			await machineContext.WaitSeconds(0.5f);
			//UnPauseBackgroundMusic();
            
			OnHandleFreeFinishLogicEnd();
		}
		public async Task CollectToFloor(int rollIndex)
		{
			var freeWheel = machineContext.view.Get<WheelFree11025>();
			var collectValue = freeWheel.GetCollectValue(rollIndex);
			await freeWheel.CollectToFloor(rollIndex);
			AddWinChipsToControlPanel((ulong) collectValue, 0, false, false);
		}
		public async Task PerformFreeCollectRoll(int rollIndex,bool closeRightLight)
		{
			var freeWheel = machineContext.view.Get<WheelFree11025>();
			var freeData = extraState.GetFreeData();
			var rollItemList = new Dictionary<int,ChameleonGameResultExtraInfo.Types.StickyItem>();
			for (var i = 0; i < freeData.StickyItems.Count; i++)
			{
				var item = freeData.StickyItems[i];
				if (item.X == rollIndex)
				{
					rollItemList[(int) item.Y] = item;
				}
			}

			await freeWheel.PrepareCollectRoll(rollIndex);
			await machineContext.WaitSeconds(0.5f);
			var rollHeight = Constant11025.RollHeightList[rollIndex];

			bool openMouthFlag = true;
			var collectCount = 0;
			for (var i = rollHeight - 1; i >= 0; i--)
			{
				if (rollItemList.ContainsKey(i) && rollItemList[i].WinRate > 0)
				{
					if (openMouthFlag)
					{
						openMouthFlag = false;
						freeWheel.ChameleonPrepareCollectCoin(rollIndex);	
					}

					collectCount++;
					var chips = (long)machineContext.state.Get<BetState>().GetPayWinChips(rollItemList[i].WinRate);
					freeWheel.ChameleonCollectCoin(rollIndex,i,chips,collectCount);
					await machineContext.WaitSeconds(Constant11025.EatCoinInterval);
				}
			}

			await machineContext.WaitSeconds(0.5f);
			await freeWheel.FinishCollectRoll(rollIndex,closeRightLight);
		}
		protected override async Task ShowFreeSpinFinishCutSceneAnimation()
		{
			
			GameObject transitionAnimation;
			if (Constant11025.NormalFreeId.Contains(freeSpinState.freeSpinId))
			{
				transitionAnimation = machineContext.assetProvider.InstantiateGameObject("Transition");	
			}
			else
			{
				transitionAnimation = machineContext.assetProvider.InstantiateGameObject("Transition");
			}
			AudioUtil.Instance.PlayAudioFx("Video1");
			// var sortingGroup = transitionAnimation.AddComponent<SortingGroup>();
			// sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
			// sortingGroup.sortingOrder = 5000;
			transitionAnimation.transform.SetParent(machineContext.transform);
			transitionAnimation.SetActive(true);
			XUtility.PlayAnimation(transitionAnimation.GetComponent<Animator>(),"Transition", () =>
			{
				GameObject.Destroy(transitionAnimation);
			});
			await machineContext.WaitSeconds(1.5f);
			if (Constant11025.NormalFreeId.Contains(freeSpinState.freeSpinId))
			{
				machineContext.view.Get<MachineSystemWidgetView>().SetActive(true);
			}
			RestoreBet();
			machineContext.view.Get<WheelFree11025>().ResetFreeRollMaskOrder();
			UpdateFreeSpinUIState(false);
			wheelsActiveState.UpdateBaseWheelState();
			// RestoreTriggerWheelElement();
			// machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
			AudioUtil.Instance.StopMusic();

			// await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionFG", machineContext);
			// GameObject.Destroy(transitionAnimation);
			await machineContext.WaitSeconds(3f);
		}
		protected override void OnHandleFreeFinishLogicEnd()
		{
			base.OnHandleFreeFinishLogicEnd();
		}
	}
}