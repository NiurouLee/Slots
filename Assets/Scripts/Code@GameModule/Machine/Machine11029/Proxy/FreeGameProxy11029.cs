using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using GameModule;

namespace GameModule
{
	public class FreeGameProxy11029 : FreeGameProxy
	{
		public bool isAvgBet = false;
		ElementConfigSet elementConfigSet = null;
		private MapGamePopup11029 _mapGamePopup11029;
		public FreeGameProxy11029(MachineContext context)
		:base(context)
		{
			 elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
		}

		protected override void RegisterInterestedWaitEvent()
        {
            base.RegisterInterestedWaitEvent();
            waitEvents.Add(WaitEvent.WAIT_SPECIAL_EFFECT);
	        waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }
        
		
		public override bool UseAverageBet()
        {
	        if (freeSpinState.freeSpinId >= 2 && freeSpinState.freeSpinId <= 8)
            {
                isAvgBet = true;
                return true;
            }
            else
            {
                isAvgBet = false;
                return false;
            }
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
	            if (freeSpinState.freeSpinId == 2)
                {
	                 UpdateFreeSpinUIState(false, UseAverageBet());
                }
                else
                {
	                 UpdateFreeSpinUIState(true, UseAverageBet());
                }
                Proceed();
            }
        }
		
		protected override void RecoverLogicState()
        {
	        if (IsFreeSpinTriggered())
            {
                HandleFreeStartLogic();
            }
            else
            {
	            PlayBgMusic();
	            machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(false);

	            if (freeSpinState.freeSpinId == 0)
	            {
		            machineContext.view.Get<MeiDuSha11029>().PlayInFree();
	            }
             
                RecoverFreeSpinStateWhenRoomSetup();
             
                if (freeSpinState.freeSpinId == 2)
                {
	                 UpdateFreeSpinUIState(false, UseAverageBet());
                }
                else
                {
	                 UpdateFreeSpinUIState(true, UseAverageBet());
                }
                if (freeSpinState.freeSpinId == 6 || freeSpinState.freeSpinId == 7 ||
                         freeSpinState.freeSpinId == 8)
                {
	                var wheels = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();
	                for (int i = 0; i < wheels.Count; i++)
	                {
		                wheels[i].transform.Find("Wild").gameObject.SetActive(true);
	                }
	                for (int i = 0; i < wheels.Count; i++)
	                {
		                machineContext.state.Get<WheelsActiveState11029>().HideTwoRoll(wheels[i]);
	                }
                }

                if (freeSpinState.freeSpinId == 1)
                {
	                machineContext.view.Get<MeiDuSha11029>().PlayIdle();

	                machineContext.view.Get<LightView11029>().ShowLight(true);
                }
                else
                {
	                machineContext.view.Get<MeiDuSha11029>().PlayInFree();
                }

                if (NeedSettle())
                {
	                HandleFreeFinishLogic();
                }
                else
                {
	                Proceed();
                }
            }
        }

		protected override async Task ShowFreeGameFinishPopUp()
		{
			if (freeSpinState.freeSpinId == 0)
			{
				await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp11029>("UIFreeGameFinish11029");
			}
			else if (freeSpinState.freeSpinId == 1)
			{
				await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp11029>("UIMagicBonusGameFinish11029");
			}
			else if (freeSpinState.freeSpinId == 2)
			{
				await ShowFreeGameFinishPopUp<MapFreeFinishPopup11029>("UIMiniGameFinish11029");
			}
			else if (freeSpinState.freeSpinId >= 3)
			{
				await ShowFreeGameFinishPopUp<MapFreeFinishPopup11029>("UIMapGameFinish11029");
			}
		}
		protected override async Task ShowFreeGameStartPopUp()
		{
			if (freeSpinState.freeSpinId == 0)
			{
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp11029>("UIFreeGameStart11029");
			}
			else if (freeSpinState.freeSpinId == 1)
			{
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp11029>("UIMagicBonusGameStart11029");
				machineContext.view.Get<LightView11029>().ShowLight(true);
			}
			else if (freeSpinState.freeSpinId == 2)
			{
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp11029>("UIMiniGameStart11029");
			}
			else if (freeSpinState.freeSpinId == 3 || freeSpinState.freeSpinId == 4 || freeSpinState.freeSpinId == 5)
			{
				await ShowMapGamePopUp();
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp11029>("UIMapGamePoint135Start11029");
			}
			else
			{
				await ShowMapGamePopUp();
				await ShowFreeGameStartPopUp<FreeSpinStartPopUp11029>("UIMapGamePoint246Start11029");
			}
		}
		
		protected override async void HandleFreeStartLogic()
        {
            StopBackgroundMusic();
            if (!IsFromMachineSetup())
            {
                await ShowFreeSpinTriggerLineAnimation();
            }

            if (freeSpinState.freeSpinId == 2)
            {
	            await ShowFreeSpinStartCutSceneAnimation();
	            await ShowFreeGameStartPopUp();
            }else
            {
	            await ShowFreeGameStartPopUp();
	            await ShowFreeSpinStartCutSceneAnimation();
            }
            Proceed();
        }

		protected void PlayBgMusic()
		{
			if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
			{
				var freeSpinState = machineContext.state.Get<FreeSpinState>();
				if (freeSpinState.freeSpinId == 0)
				{
					AudioUtil.Instance.PlayMusic("Bg_FreeGame_11029",true);
				}
				else if (freeSpinState.freeSpinId == 1)
				{
					AudioUtil.Instance.PlayMusic("Bg_FreeGame_11029",true);
				}
				else if (freeSpinState.freeSpinId == 2)
				{
					AudioUtil.Instance.StopMusic();
				}
				else
				{
					AudioUtil.Instance.PlayMusic("Bg_FreeGame_Map_11029",true);
				}
			}
		}
		
		public async Task ShowMapGamePopUp()
		{
			 AudioUtil.Instance.PlayAudioFxOneShot("Map_Open");
			_mapGamePopup11029 = PopUpManager.Instance.ShowPopUp<MapGamePopup11029>("MapGame");
			_mapGamePopup11029.EnableButton(false);
			_mapGamePopup11029.ShowLevel(true);
			await machineContext.WaitSeconds(1.0f);
			await _mapGamePopup11029.ShowMapGame();
		} 
		
		//开始free转场动画
		protected override async  Task ShowFreeSpinStartCutSceneAnimation()
		{
			if (freeSpinState.freeSpinId == 2)
			{
				await ShowMapGamePopUp();
			}

			if (IsFromMachineSetup() && (freeSpinState.freeSpinId == 0 || freeSpinState.freeSpinId == 1))
			{
				machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(false);
				machineContext.state.Get<WheelsActiveState11029>().UpdateRunningWheelState(false, true);
				machineContext.view.Get<WheelBonus11029>().transform.gameObject.SetActive(false);
				if (freeSpinState.freeSpinId == 1)
				{
					var wheel = machineContext.view.Get<Wheel11029>();
					for (var i = 1; i < wheel.rollCount; i++)
					{
						wheel.GetRoll(i).ChangeRowCount(3);
						wheel.GetRoll(i).ChangeExtraTopElementCount(3);
					}

					wheel.transform.Find("spiningMask").gameObject.SetActive(false);
					wheel.transform.GetComponent<Animator>().Play("Idle1");
				}
				uint freeSpinId = freeSpinState.freeSpinId;
				if (freeSpinId == 1)
				{
					machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
					machineContext.view.Get<MeiDuSha11029>().PlayIdle();
					machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(true, false, false);
				}
				else if (freeSpinId == 0)
				{
					machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
					machineContext.view.Get<MeiDuSha11029>().PlayInFree();
					machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(true, false, false);
				}
				machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
				UpdateSpinUiViewTotalBet(false, false);
				UpdateFreeSpinUIState(true, UseAverageBet());
				// if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
				// {
				// 	machineContext.view.Get<ControlPanel>().ShowAutoStopButton(true);
				// }
				controlPanel.ShowSpinButton(false);
				await base.ShowFreeSpinStartCutSceneAnimation();
			}
			else
			{
				machineContext.view.Get<TransitionsView11029>().PlayFreeTransition();
				await machineContext.WaitSeconds(1.5f);
				//Map弹窗消失
				if (_mapGamePopup11029 != null)
				{
					PopUpManager.Instance.ClosePopUp(_mapGamePopup11029);
				}

				machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(false);
				machineContext.state.Get<WheelsActiveState11029>().UpdateRunningWheelState(false, true);
				if (freeSpinState.freeSpinId == 6 || freeSpinState.freeSpinId == 7 ||
				    freeSpinState.freeSpinId == 8)
				{
					var wheels = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();
					for (int i = 0; i < wheels.Count; i++)
					{
						wheels[i].transform.Find("Wild").gameObject.SetActive(true);
					}

					for (int i = 0; i < wheels.Count; i++)
					{
						machineContext.state.Get<WheelsActiveState11029>().HideTwoRoll(wheels[i]);
					}
				}
				else if (freeSpinState.freeSpinId == 0 || freeSpinState.freeSpinId == 1)
				{
					machineContext.view.Get<WheelBonus11029>().transform.gameObject.SetActive(false);
				}

				if (freeSpinState.freeSpinId == 1)
				{
					var wheel = machineContext.view.Get<Wheel11029>();
					for (var i = 1; i < wheel.rollCount; i++)
					{
						wheel.GetRoll(i).ChangeRowCount(3);
						wheel.GetRoll(i).ChangeExtraTopElementCount(3);
					}

					wheel.transform.Find("spiningMask").gameObject.SetActive(false);
					wheel.transform.GetComponent<Animator>().Play("Idle1");
				}

				uint freeSpinId = freeSpinState.freeSpinId;
				if (freeSpinId == 1)
				{
					machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
					machineContext.view.Get<MeiDuSha11029>().PlayIdle();
					machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(true, false, false);
				}
				else if (freeSpinId == 0)
				{
					machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
					machineContext.view.Get<MeiDuSha11029>().PlayInFree();
					machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(true, false, false);
				}
				else if (freeSpinId == 2)
				{
					controlPanel.UpdateControlPanelState(false, UseAverageBet());
					machineContext.view.Get<MiniGameView11029>().ShowWinGroup(freeSpinState.freeSpinBet);
					machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
					machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, false, true);
				}
				else if (freeSpinId > 2 && freeSpinId <= 8)
				{
					machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
					machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, true, false);
				}

				await machineContext.WaitSeconds(2.0f - 1.5f);
				machineContext.view.Get<TransitionsView11029>().HideFreeTransition();
				machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
				UpdateSpinUiViewTotalBet(false, false);
				if (freeSpinState.freeSpinId == 2)
				{
					UpdateFreeSpinUIState(false, UseAverageBet());
				}
				else
				{
					UpdateFreeSpinUIState(true, UseAverageBet());
				}

				// if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
				// {
				// 	machineContext.view.Get<ControlPanel>().ShowAutoStopButton(true);
				// } 
				controlPanel.ShowSpinButton(false);
				await Task.CompletedTask;
			}
		}


		//结束free转场动画
		protected override async  Task ShowFreeSpinFinishCutSceneAnimation()
		{ 
			machineContext.view.Get<TransitionsView11029>().PlayFreeTransition();
			await machineContext.WaitSeconds(1.5f);
			
			if (freeSpinState.freeSpinId == 1)
			{
				var logicStepProxy = (WheelsSpinningProxy11029)machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING);
				logicStepProxy._layerBonusFree.RecyleBigScatterElement();
			}
			machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);
			if (freeSpinState.freeSpinId == 6 || freeSpinState.freeSpinId == 7 || freeSpinState.freeSpinId == 8)
			{
				var wheels = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();
				for (int i = 0; i < wheels.Count; i++)
				{
					var wheel = wheels[i];
					var roll = wheel.GetRoll((int) 2);
					roll.transform.gameObject.SetActive(true);
					for (int k = 0; k < roll.containerCount; k++)
					{
						var elementContainer = roll.GetContainer(k);
						elementContainer.transform.gameObject.SetActive(true);
					}
				}
			}
			machineContext.state.Get<WheelsActiveState11029>().UpdateRunningWheelState(false,false);
			machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(true);
			machineContext.view.Get<MeiDuSha11029>().PlayInBase();
			UpdateFreeSpinUIState(true, isAvgBet);
			RestoreFinishTriggerWheelElement();
			machineContext.view.Get<BackGroundView11029>().ShowFreeBackground(false, false, false);
			machineContext.view.Get<ProgressBar11029>().ChangeMapRightBtn();
			machineContext.view.Get<ProgressBar11029>().LockSuperFreeIdle(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
			await machineContext.view.Get<ProgressBar11029>().ChangeFill(false,false);
			await machineContext.WaitSeconds(2.0f-1.5f);
			machineContext.view.Get<TransitionsView11029>().HideFreeTransition();
			await base.ShowFreeSpinFinishCutSceneAnimation();
		}
		
		//free回到base，将牌面换成触发时的牌面。
		protected virtual void RestoreFinishTriggerWheelElement()
        {
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
            {
                if (freeSpinState.NextIsFreeSpin) 
                { 
                    machineContext.state.Get<BetState>().SetTotalBet(freeSpinState.freeSpinBet);
                    UpdateSpinUiViewTotalBet(false,false);
                }
                else 
                { 
                    RestoreBet();
                }
                controlPanel.UpdateControlPanelState(false, false);
                var baseWheel = machineContext.view.Get<Wheel>("WheelBaseGame");
                baseWheel.wheelState.UpdateWheelStateInfo(triggerPanels[0]);
                baseWheel.ForceUpdateElementOnWheel();   
            }
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

         //   UpdateFreeSpinUIState(false);
            await ShowFreeSpinFinishCutSceneAnimation();
            await ShowFreeSpinBigWinEffect();
        
            if (machineContext.state.Get<ExtraState11029>().GetIsWheel())
            {
	            // var bonusProxy11029 = machineContext.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11029;
	            // bonusProxy11029.StartWheelRollingGame();
	            machineContext.JumpToLogicStep(LogicStepType.STEP_BONUS,LogicStepType.STEP_FREE_GAME);
            }
            else
            { 
	            OnHandleFreeFinishLogicEnd();
            }
		}
	}
}