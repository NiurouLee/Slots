using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class FreeGameProxy11024 : FreeGameProxy
	{
		private MapView11024 _map;
		public MapView11024 map
		{
			get
			{
				if (_map == null)
				{
					_map =  machineContext.view.Get<MapView11024>();
				}
				return _map;
			}
		}
		private WheelsActiveState11024 _activeWheelState;
		public WheelsActiveState11024 activeWheelState
		{
			get
			{
				if (_activeWheelState == null)
				{
					_activeWheelState =  machineContext.state.Get<WheelsActiveState11024>();
				}
				return _activeWheelState;
			}
		}
		private ExtraState11024 _extraState;
		public ExtraState11024 extraState
		{
			get
			{
				if (_extraState == null)
				{
					_extraState =  machineContext.state.Get<ExtraState11024>();
				}
				return _extraState;
			}
		}

		private Wheel _baseWheel;

		public Wheel baseWheel
		{
			get
			{
				if (_baseWheel == null)
				{
					_baseWheel = machineContext.view.Get<Wheel>("WheelBaseGame");
				}
				return _baseWheel;
			}
		}
		private CollectBarView11024 _collectBar;

		public CollectBarView11024 collectBar
		{
			get
			{
				if (_collectBar == null)
				{
					_collectBar = machineContext.view.Get<CollectBarView11024>();
				}
				return _collectBar;
			}
		}
		
		private PigGroupView11024 _pigGroup;

		public PigGroupView11024 pigGroup
		{
			get
			{
				if (_pigGroup == null)
				{
					_pigGroup = machineContext.view.Get<PigGroupView11024>();
				}
				return _pigGroup;
			}
		}
		public FreeGameProxy11024(MachineContext context)
		:base(context)
		{

	
		}
		protected override void HandleCommonLogic()
		{
			if (IsFromMachineSetup())
				return;
            
			StopWinCycle();
            
			if (IsFreeSpinTriggered())
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

		protected override void HandleFreeStartLogic()
		{
			base.HandleFreeStartLogic();
		}
		protected override async Task ShowFreeGameStartPopUp<T>(string address = null)
		{
			var freeType = extraState.GetMapLevel() / 5;
			address = "UIMapGameStar" + freeType;
            
			if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
			{
				XDebug.LogError($"ShowFreeGameStartPopUp:{address} is Not Exist" );    
				return;
			}
 
			TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(waitTask, null);

			var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
			startPopUp.SetPopUpCloseAction(() =>
			{
				machineContext.RemoveTask(waitTask);
				waitTask.SetResult(true);
			});
			if (startPopUp.IsAutoClose())
			{
				await machineContext.WaitSeconds(GetStartPopUpDuration());
				startPopUp.Close();    
			}
			await waitTask.Task;
		}
		protected override async Task ShowFreeGameFinishPopUp<T>(string address = null)
		{
			if (address == null)
			{
				address = "UIMapGame";
			}

			if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
			{
				XDebug.LogError($"ShowFreeGameFinishPopUp:{address} is Not Exist" );    
				return;
			}

			TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(waitTask, null);

			var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
			startPopUp.SetPopUpCloseAction(() =>
			{
				machineContext.RemoveTask(waitTask);
				waitTask.SetResult(true);
			});

			await waitTask.Task;
		}
		protected override async Task ShowFreeGameStartPopUp()
		{
			await ShowFreeGameStartPopUp<FreeSpinStartPopUp11024>();
		}
		protected override async Task ShowFreeGameFinishPopUp()
		{
			await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp11024>();
		}
		public override bool UseAverageBet()
		{
			return true;
		}
		protected override async Task ShowFreeSpinStartCutSceneAnimation()
		{
			if (map.isOpen)
			{
				await map.CloseMap();
				// machineContext.view.Get<MachineSystemWidgetView>().SetActive(true);
				// AudioUtil.Instance.UnPauseMusic();
				// machineContext.view.Get<ControlPanel>().UpdateControlPanelState(false, false);
				// machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
			}
			var task = GetWaitTask();
			AudioUtil.Instance.PlayAudioFx("LinkGame_Video2");
			var interlude = machineContext.assetProvider.InstantiateGameObject("TransitionPig");
			interlude.transform.SetParent(machineContext.transform,false);
			XUtility.PlayAnimation(interlude.GetComponent<Animator>(), "TransitionPig",()=>{
				GameObject.Destroy(interlude);
				task.SetResult(true);
			},machineContext);
			await machineContext.WaitSeconds(1f);
			activeWheelState.UpdateFreeWheelState();
			machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
			UpdateSpinUiViewTotalBet(false,false);
			UpdateFreeSpinUIState(true, UseAverageBet());
			controlPanel.ShowSpinButton(false);
			await task.Task;
		}
		public override void RecoverFreeSpinStateWhenRoomSetup()
		{
			activeWheelState.UpdateFreeWheelState();
			machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
			UpdateSpinUiViewTotalBet(false,false);
			UpdateFreeSpinUIState(true, UseAverageBet());
			controlPanel.ShowSpinButton(false);
		}
		protected override async void HandleFreeFinishLogic()
		{
			var winState = machineContext.state.Get<WinState>();
            
			if (winState.displayCurrentWin == 0)
			{
				await machineContext.WaitSeconds(0.8f);
			}
			StopBackgroundMusic();
			await ShowFreeGameFinishPopUp();
			RestoreBet();
			UpdateFreeSpinUIState(false);
			await ShowFreeSpinFinishCutSceneAnimation();
			await ShowFreeSpinBigWinEffect();
			//UnPauseBackgroundMusic();
            
			OnHandleFreeFinishLogicEnd();
		}
		protected override async Task ShowFreeSpinFinishCutSceneAnimation()
		{
			var task = GetWaitTask();
			AudioUtil.Instance.PlayAudioFx("LinkGame_Video2");
			var interlude = machineContext.assetProvider.InstantiateGameObject("TransitionPig");
			interlude.transform.SetParent(machineContext.transform,false);
			XUtility.PlayAnimation(interlude.GetComponent<Animator>(), "TransitionPig",()=>{
				GameObject.Destroy(interlude);
				task.SetResult(true);
			},machineContext);
			await machineContext.WaitSeconds(1f);
			activeWheelState.UpdateBaseWheelState();
			map.InitState();
			await task.Task;
		}
		protected override void OnHandleFreeFinishLogicEnd()
		{
			freeSpinState.backFromFree = true;
			if (machineContext.GetLogicStepProxy(LogicStepType.STEP_RE_SPIN).CheckCurrentStepHasLogicToHandle())
			{
				machineContext.JumpToLogicStep(LogicStepType.STEP_RE_SPIN,LogicStepType.STEP_FREE_GAME);
			}
			else
			{
				Proceed();	
			}
		}
	}
}