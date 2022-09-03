using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class NextSpinPrepareProxy11025 : NextSpinPrepareProxy
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
		private FreeSpinState _freeState;
		public FreeSpinState freeState
		{
			get
			{
				if (_freeState == null)
				{
					_freeState = machineContext.state.Get<FreeSpinState>();
				}
				return _freeState;
			}
		}
		private BetState _betState;
		public BetState betState
		{
			get
			{
				if (_betState == null)
				{
					_betState =  machineContext.state.Get<BetState>();
				}
				return _betState;
			}
		}
		private ShopView11025 _shopView;
		public ShopView11025 shopView
		{
			get
			{
				if (_shopView == null)
				{
					_shopView =  machineContext.view.Get<ShopView11025>();
				}
				return _shopView;
			}
		}
		private WheelBase11025 _baseWheel;
		public WheelBase11025 baseWheel
		{
			get
			{
				if (_baseWheel == null)
				{
					_baseWheel =  machineContext.view.Get<WheelBase11025>();
				}
				return _baseWheel;
			}
		}
		public NextSpinPrepareProxy11025(MachineContext context)
		:base(context)
		{
		}
		protected override void OnUnlockBetFeatureConfigChanged()
		{
			baseWheel.RefreshShopLockState();
		}
		public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
		{
			base.OnMachineInternalEvent(internalEvent, args);
			if (internalEvent == MachineInternalEvent.EVENT_CUSTOM_CLICK &&  !machineContext.state.Get<AutoSpinState>().IsAutoSpin)
			{
				var btnType = (BtnType11025) args[0];
				if (btnType == BtnType11025.Box)
				{
					var pageId = (int) args[1];
					var boxId = (int) args[2];
					DealAboutBoxBtn(pageId,boxId);
				}
				else if (btnType == BtnType11025.ChangePage)
				{
					var direction = (MoveDirection) args[1];
					DealAboutChangePageBtn(direction);
				}
				else if (btnType == BtnType11025.CloseShop)
				{
					DealAboutCloseShopBtn();
				}
				else if (btnType == BtnType11025.OpenShop)
				{
					DealAboutOpenShopBtn();
				}
			}
		}
		protected override void HandleCustomLogic()
		{
			HandleCustomLogicAsync();
		}

		public async void HandleCustomLogicAsync()
		{
			if (IsFromMachineSetup())
			{
				if (shopView.CheckMapGame())
				{
					if (!shopView.isOpen)
					{
						// machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
						await shopView.OpenShop();	
					}
					PerformMapGame();
				}
				else
				{
					machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
					await baseWheel.ShowShopTip();
					machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
				}
			}

			if (freeState.backFromFree && freeState.freeSpinId > 0)
			{
				shopView.SetShopData(extraState.GetShopData());
				shopView.SetPageToPlayingPage();
				shopView.RefreshAll();
				if (!shopView.isOpen)
				{
					// machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
					await shopView.OpenShop();	
				}
				if (shopView.CheckMapGame())
				{
					PerformMapGame();
				}
				else
				{
					shopView.SetBtnEnable(true);
				}
			}
		}
		protected override void OnBetChange()
		{
			base.OnBetChange();
			baseWheel.RefreshShopLockState();
			baseWheel.CleanWheel();
			var normalData = extraState.GetNowNormalData();
			baseWheel.wheelState.UpdateWheelStateInfo(normalData.Panels[0]);
			baseWheel.ForceUpdateElementOnWheel();
			baseWheel.UpdateChameleonState(normalData.StickyColumnReSpinCounts,true);
			baseWheel.UpdateNormalDataPanel(normalData.StickyItems,normalData.FullIndexes,normalData.FailedIndexes,true);
		}
		public async void DealAboutOpenShopBtn()
		{
			if (betState.IsFeatureUnlocked(0))
			{
				if (!shopView.isOpen)
				{
					machineContext.view.Get<ControlPanel>().StopWinAnimation(false,0);
					shopView.SetPageToPlayingPage();
					shopView.RefreshAll();
					await shopView.OpenShop();	
					shopView.SetBtnEnable(true);
				}
			}
			else
			{
				machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, 0);
			}
		}
		public async void DealAboutCloseShopBtn()
		{
			if (shopView.isOpen)
			{
				await shopView.CloseShop();
				AudioUtil.Instance.UnPauseMusic();
				machineContext.view.Get<ControlPanel>().UpdateControlPanelState(false, false);
				machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
			}
		}
		
		public async void DealAboutChangePageBtn(MoveDirection direction)
		{
			await shopView.PageMove(direction);
		}
		public async void DealAboutBoxBtn(int pageId,int boxId)
		{
			if (pageId == extraState.GetPlayingPageIndex())
			{
				var tempBox = shopView.GetPage(pageId).GetBox(boxId);
				var boxData = tempBox.GetBoxData();
				if (!boxData.Open)
				{
					if (boxData.Price <= extraState.GetNowShopCoins())
					{
						machineContext.state.Get<WinState>().SetCurrentWin(0);
						machineContext.view.Get<ControlPanel>().UpdateWinLabelChips(0);
						shopView.SetBtnEnable(false);
						await extraState.SendSpecialProcess(boxId.ToString());
						shopView.SetShopData(extraState.GetShopData());
						shopView.RefreshCoins();
						baseWheel.SetStoreCoin(extraState.GetNowShopCoins());
						boxData = tempBox.GetBoxData();
						tempBox.RefreshText();
						// if (tempBox.GetBoxData().WinRate > 0)
						// {
						//     var chips = betState.GetPayWinChips(tempBox.GetBoxData().WinRate);
						//     context.state.Get<WinState>().AddCurrentWin(chips);
						//     context.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long) context.state.Get<WinState>().currentWin,0.5f, false);
						// }
						await tempBox.PerformOpen();
						if (tempBox.GetBoxData().IsSuper)
						{
							await ShowFreeGamePopUp<FreeSpinStartPopUp11025>("UIStoreGameTrigger11025");
							await shopView.CloseShop();
							machineContext.JumpToLogicStep(LogicStepType.STEP_FREE_GAME,LogicStepType.STEP_NEXT_SPIN_PREPARE);
							//进superFG
						}
						else if (tempBox.GetBoxData().IsFree)
						{
							await ShowFreeGamePopUp<FreeSpinStartPopUp11025>("UIFreeGameStart11025");
							await shopView.CloseShop();
							machineContext.JumpToLogicStep(LogicStepType.STEP_FREE_GAME,LogicStepType.STEP_NEXT_SPIN_PREPARE);
							//进FG
						}
						else
						{
							var chips = boxData.TotalWin;//boxData.WinRate * extraState.GetAverageBet() / 100;
							var winLevel = AddWinChipsToControlPanel(chips,0,true,false);
							var waitTime = 2f;
							if (winLevel == WinLevel.SmallWin)
							{
								waitTime = 1f;
							}
							await machineContext.WaitSeconds(waitTime);
							EventBus.Dispatch(new EventBalanceUpdate((long) chips, "boxValue"));
							//
							if (shopView.CheckMapGame())
							{
								await machineContext.WaitSeconds(0.5f);
								PerformMapGame();
							}
							else
							{
								shopView.SetBtnEnable(true);	
							}
						}	
					}
					else
					{
						for (var i = 0; i < 9; i++)
						{
							shopView.GetPage(pageId).GetBox(i).HideTip();	
						}
						tempBox.ShowTip();
					}
				}
			}
		}
		public override WinLevel AddWinChipsToControlPanel(ulong winChips, float configDuration = 0f,
            bool withAudio = true, bool withAutoAni = true, string specifiedAudioName = null,bool audioLoop = false,string specifiedWinAudioName = null)
        {
            if (winChips > 0)
            {
                //Debug.LogError($"=======aaaa currentWin:{machineContext.state.Get<WinState>().currentWin}");

                machineContext.state.Get<WinState>().AddCurrentWin(winChips);
                //Debug.LogError($"=======bbbb currentWin:{machineContext.state.Get<WinState>().currentWin} add:{winChips} origin:{machineContext.state.Get<WinState>().currentWin - winChips}");
                
                var winLevel = machineContext.state.Get<BetState>().GetSmallWinLevel((long) winChips);

                //     var machineConfig = MachineState.GetMachineConfig();

                string audioName = null;
                string stopAudioName = null;
                float effectDuration = 0.5f;

                bool needPlayWinOutAnimation = winLevel == WinLevel.NiceWin && withAutoAni;

                effectDuration = 0.5f;

               
                if (winLevel == WinLevel.SmallWin)
                {
                    audioName = "Symbol_SmallWin_" + machineContext.assetProvider.AssetsId;
                    stopAudioName = "Symbol_SmallWinEnding_" + machineContext.assetProvider.AssetsId;
                    effectDuration = 1.0f;
                    if (!string.IsNullOrEmpty(specifiedWinAudioName))
                    {
                        audioName = specifiedWinAudioName;
                        stopAudioName = "";
                    }
                }
                else
                {
                    audioName = "Symbol_Win_" + machineContext.assetProvider.AssetsId;
                    stopAudioName = "Symbol_WinEnding_" + machineContext.assetProvider.AssetsId;
                    effectDuration = 2.0f;
                    if (!string.IsNullOrEmpty(specifiedWinAudioName))
                    {
                        audioName = specifiedWinAudioName; 
                        stopAudioName = "";
                    }
                }
                machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation(
	                (long) machineContext.state.Get<WinState>().currentWin,
	                effectDuration, needPlayWinOutAnimation, audioName, stopAudioName,audioLoop);
       
                return winLevel;
            }

            return WinLevel.NoWin;
        }
		public async void PerformMapGame()
		{
			machineContext.state.Get<WinState>().SetCurrentWin(0);
			machineContext.view.Get<ControlPanel>().UpdateWinLabelChips(0);
			shopView.SetBtnEnable(false);
			var popupAssetName = "UIStoreGameSpecialOpen" + (extraState.GetPlayingPageIndex() + 1) + "_11025";
			await extraState.SendSpecialProcess();
			await shopView.PerformCollectMapGame();
			await ShowFreeGamePopUp<FreeSpinStartPopUp11025>(popupAssetName);
			await shopView.CloseShop();
			machineContext.JumpToLogicStep(LogicStepType.STEP_FREE_GAME,LogicStepType.STEP_NEXT_SPIN_PREPARE);
		}
		protected async Task ShowFreeGamePopUp<T>(string address)where T:FreeSpinStartPopUp
		{
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
		protected override bool NeedPlayWinCycle()
		{
			return base.NeedPlayWinCycle() && !IsFromMachineSetup();
		}
	}
}