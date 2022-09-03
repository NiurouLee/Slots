using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DragonU3DSDK.Audio;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using UnityEngine.Rendering;

namespace GameModule
{
	public class BonusProxy11030 : BonusProxy,IUpdateable
	{
		public ExtraState11030 extraState;
		public WheelsActiveState11030 wheelsActiveState;
		public JackpotInfoState11030 jpState;
		public ElementExtraInfoProvider11030 elementProvider;
		public TrainView11030 trainView;
		public TrainGoldView11030 trainGoldView;
		public bool isInChooseInitSpin;
		public TaskCompletionSource<bool> starAddValueTask;
		public WheelTrain11030 chooseWheel;
		public bool canReconnectFlag = true;
		public BonusProxy11030(MachineContext context)
		:base(context)
		{
			extraState = machineContext.state.Get<ExtraState11030>();
			wheelsActiveState = machineContext.state.Get<WheelsActiveState11030>();
			jpState = machineContext.state.Get<JackpotInfoState11030>();
			trainView = machineContext.view.Get<TrainView11030>();
			trainGoldView = machineContext.view.Get<TrainGoldView11030>();
			elementProvider = (ElementExtraInfoProvider11030)machineContext.elementExtraInfoProvider;
			chooseWheel = machineContext.view.Get<WheelTrain11030>(Constant11030.WheelLineFeatureGameName);
			isInChooseInitSpin = false;
		}
		protected override void RegisterInterestedWaitEvent()
		{
			base.RegisterInterestedWaitEvent();
			waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
		}
		public async Task PerformTrainInterlude()
		{
			AudioUtil.Instance.PlayAudioFx("Video1");
			var trainId = extraState.GetNextRunningTrain().Id;
			var interludeName = "";
			switch (trainId)
			{
				case 15:
					interludeName = "Red";
					break;
				case 16:
					interludeName = "Green";
					break;
				case 17:
					interludeName = "Blue";
					break;
				case 18:
					interludeName = "Purple";
					break;
			}

			var interludeAnimator = machineContext.assetProvider.InstantiateGameObject("TrainTransition" + interludeName);
			var sortingGroup = interludeAnimator.AddComponent<SortingGroup>();
			sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
			sortingGroup.sortingOrder = 100;
			interludeAnimator.transform.SetParent(machineContext.transform);
			var tempAnimator = interludeAnimator.GetComponent<Animator>();
			XUtility.PlayAnimation(tempAnimator,"Free", () =>
			{
				GameObject.Destroy(interludeAnimator);
			});

			await machineContext.WaitSeconds(1.5f);
			ChangeViewToTrain();
			await machineContext.WaitSeconds(0.5f);
			await Task.CompletedTask;
		}

		public async Task PerformGoldTrainInterlude()
		{
			AudioUtil.Instance.PlayAudioFx("Video1");
			var interludeAnimator = machineContext.assetProvider.InstantiateGameObject("TrainTransition");
			var sortingGroup = interludeAnimator.AddComponent<SortingGroup>();
			sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
			sortingGroup.sortingOrder = 100;
			interludeAnimator.transform.SetParent(machineContext.transform);
			var tempAnimator = interludeAnimator.GetComponent<Animator>();
			XUtility.PlayAnimation(tempAnimator,"Free", () =>
			{
				GameObject.Destroy(interludeAnimator);
			});
			await machineContext.WaitSeconds(1.5f);
			ChangeViewToGoldTrain();
			await machineContext.WaitSeconds(1.5f);
			await Task.CompletedTask;
		}
		public async Task PerformTrainOutInterlude()
		{
			await machineContext.WaitSeconds(1f);
			ChangeViewFromTrain();
			await machineContext.WaitSeconds(1f);
			await Task.CompletedTask;
		}

		public async Task PerformGoldTrainOutInterlude()
		{
			await machineContext.WaitSeconds(1f);
			ChangeViewFromGoldTrain();
			await machineContext.WaitSeconds(1f);
			await Task.CompletedTask;
		}

		public async Task PerformTrainWheelInterlude()
		{
			// await machineContext.WaitSeconds(1f);
			AudioUtil.Instance.PlayAudioFx("SwitchBoard");
			ChangeViewToChooseTrainWheel();
			// await machineContext.WaitSeconds(1f);
			await Task.CompletedTask;
		}
		public async Task PerformTrainWheelOutInterlude()
		{
			await machineContext.WaitSeconds(1f);
			AudioUtil.Instance.PlayAudioFx("SwitchBoard");
			ChangeViewFromChooseTrainWheel();
			await machineContext.WaitSeconds(1f);
			await Task.CompletedTask;
		}
		public void ChangeViewToChooseTrainWheel()
		{
			AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
			wheelsActiveState.UpdateChooseTrainWheelState();
			// jpState.LockJackpot = true;
		}
		public void ChangeViewFromChooseTrainWheel()
		{
			
			wheelsActiveState.UpdateBaseWheelState();
			AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
			// jpState.LockJackpot = false;
			var runningWheel = wheelsActiveState.GetRunningWheel()[0];
			runningWheel.wheelState.UpdateWheelStateInfo(extraState.GetChooseTriggeringPanel());
			runningWheel.ForceUpdateElementOnWheel(); 
		}
		public void ChangeViewToTrain()
		{
			wheelsActiveState.UpdateTrainWheelState();
			// jpState.LockJackpot = true;
			trainView.Show();
			trainView.RefreshViewState();
		}

		public void ChangeViewToGoldTrain()
		{
			wheelsActiveState.UpdateTrainWheelState();
			// jpState.LockJackpot = true;
			trainGoldView.Show();
			trainGoldView.RefreshViewState();
		}

		public void ChangeViewFromTrain()
		{
			wheelsActiveState.UpdateBackFromTrainWheelState();
			trainView.Hide();
		}
		public void ChangeViewFromGoldTrain()
		{
			wheelsActiveState.UpdateBackFromTrainWheelState();
			trainGoldView.Hide();
		}

		public async Task SpinChooseTrainWheel()
		{
			isInChooseInitSpin = true;
			var spinTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(spinTask,null);
			chooseWheel.spinningController.StartSpinning(
				(Wheel runningWheel) =>
				{
					machineContext.RemoveTask(spinTask);
					spinTask.SetResult(true);
				},
				(Wheel runningWheel) =>
				{
					// chooseWheel.spinningController.CheckAndShowAnticipationAnimation();
				},
				null,
				0
			);
			UpdateScheduler.HookUpdate(this);
			await machineContext.state.Get<ExtraState11030>().SendBonusProcess((bonusResponse) =>
			{
				((WheelState11030)chooseWheel.wheelState).UpdateStateOnReceiveBonusInitSpin(bonusResponse);
			});
			await machineContext.WaitSeconds(0.5f);
			chooseWheel.spinningController.OnSpinResultReceived(true);
			await spinTask.Task;
			UpdateScheduler.UnhookUpdate(this);
			isInChooseInitSpin = false;
		}

		public void Update()
		{
			chooseWheel.spinningController.OnLogicUpdate();
		}
		protected bool CheckIsTriggerElement(ElementContainer container)
		{
			var elementId = container.sequenceElement.config.id;
			return elementProvider.CheckElementGoldTrain(elementId) || 
			       elementProvider.CheckElementStar(elementId) || 
			       elementProvider.CheckElementTrain(elementId) || 
			       elementProvider.CheckElementValue(elementId);
		}
		public async Task PerformChooseTrainWinAnimation()
		{
			var runningWheel = wheelsActiveState.GetRunningWheel()[0];
			
			var triggerElementContainers = runningWheel.GetElementMatchFilter((container) =>
			{
				if (CheckIsTriggerElement(container))
				{
					return true;
				}

				return false;
			});

			if (triggerElementContainers.Count > 0)
			{
				for (var j = 0; j < triggerElementContainers.Count; j++)
				{
					triggerElementContainers[j].PlayElementAnimation("Trigger");
					triggerElementContainers[j].ShiftSortOrder(true);
				}

				await XUtility.WaitSeconds(2f, machineContext);
				// for (var j = 0; j < triggerElementContainers.Count; j++)
				// {
				// 	triggerElementContainers[j].UpdateAnimationToStatic();
				// }
			}
			else
			{
				throw new Exception("火车feature没有触发element");
			}
		}

		public async Task PerformTrainLine()
		{
			var runningWheel = (WheelTrain11030)wheelsActiveState.GetRunningWheel()[0];
			await runningWheel.SetTrainLine(true);
			jpState.LockJackpot = true;
		}
		public async Task PerformTrainLineEnd()
		{
			var runningWheel = (WheelTrain11030)wheelsActiveState.GetRunningWheel()[0];
			await runningWheel.SetTrainLine(false);
			jpState.LockJackpot = false;
		}

		protected override void HandleCustomLogic()
		{
			DealCustomLogic();
		}

		public async void DealCustomLogic()
		{
			if (extraState.IsInChoose())
			{
				machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
				await StartChooseGame();
				if (!extraState.HasBonusGame())
				{
					Proceed();
					return;
				}
			}
			if (extraState.IsTrainFromChoose())
			{
				if (extraState.IsSelectLinePanelNeedInit())
				{
					if (IsInReconnect())
					{
						canReconnectFlag = false;
						ChangeViewToChooseTrainWheel();
					}
					else
					{
						await PerformTrainWheelInterlude();
					}
					machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
					await SpinChooseTrainWheel();
				}
				else if (IsInReconnect())
				{
					AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
				}
			}
			if (!IsInReconnect())
			{
				if (extraState.GetLastGoldTrain() != null)
				{
					// await AudioUtil.Instance.PlayAudioFxAsync("B01_Trigger");
				}
				// await PerformChooseTrainWinAnimation();
			}
			if (IsInReconnect())
			{
				var trainList = extraState.GetCompleteRunningTrains();
				for (int i = 0; i < trainList.Count; i++)
				{
					var tempTrain = trainList[i];
					RefreshSymbolUpperValue(tempTrain);
				}
			}
			if (extraState.GetNextRunningTrain() != null)
			{
				machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
				if (IsInReconnect())
				{
					ChangeViewToTrain();
					var trainList = extraState.GetCompleteRunningTrains();
					for (int i = 0; i < trainList.Count; i++)
					{
						var tempTrain = trainList[i];
						trainView.ReconnectCollectValue(tempTrain);
					}
					trainView.ReconnectCollectValueView();
					canReconnectFlag = false;
					await PerformTrainLine();
				}
				else
				{
					await PerformTrainLine();
					await PerformTrainInterlude();
				}
				while (extraState.GetNextRunningTrain() != null)
				{
					await trainView.StartPerform();
					RefreshSymbolUpperValue(extraState.GetNextRunningTrain());
					await machineContext.state.Get<ExtraState11030>().SendBonusProcess();
					await machineContext.WaitSeconds(0.01f);
					// GC.Collect();
				}
				await machineContext.WaitSeconds(0.5f);
				await trainView.PerformCollectToCenterBoard();
				await machineContext.WaitSeconds(0.5f);
				await PerformTrainOutInterlude();
			}

			var isTrainFromChoose = extraState.IsTrainFromChoose();
			if (extraState.GetLastGoldTrain() != null)
			{
				machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
				await PerformTrainLine();
				await PerformCollectValueToTopFrame();
				await PerformGoldTrainInterlude();
				await trainGoldView.StartPerform();
				var totalWinRate = extraState.GetLastGoldTrain().TotalWinRate;
				var totalWinChip = machineContext.state.Get<BetState>().GetPayWinChips(totalWinRate);
				uint winLevel = 0;
				for (var i = Constant11030.WinLevelDictionary.Count-1; i >= 0; i--)
				{
					if (totalWinRate >= Constant11030.WinLevelDictionary[i])
					{
						winLevel = (uint)i + 1;
						break;
					}
				}

				if (winLevel >= (int) WinLevel.BigWin)
				{
					await WinEffectHelper.ShowBigWinEffectAsync(winLevel, totalWinChip, machineContext);
					AudioUtil.Instance.PlayAudioFx("GoldenTrain_CollectResponse");
					trainGoldView.ShowCollectEFX();
					AddWinChipsToControlPanel(totalWinChip);
				}
				else
				{
					AudioUtil.Instance.PlayAudioFx("GoldenTrain_CollectResponse");
					var waitTime = 0f;
					switch (winLevel)
					{
						case 3:
							waitTime = 6f;
							break;
						case 2:
							waitTime = 2f;
							break;
						case 1:
							waitTime = 1f;
							break;
						default:
							throw new Exception("金火车winLevel异常");
					}

					if (winLevel == 3)
					{
						await trainGoldView.ShowCollectEFX();
					}
					else
					{
						trainGoldView.ShowCollectEFX();	
					}
					AddWinChipsToControlPanel(totalWinChip);
					await machineContext.WaitSeconds(waitTime);
				}
				await machineContext.state.Get<ExtraState11030>().SendBonusProcess();
				await machineContext.state.Get<ExtraState11030>().SettleBonusProgress();
				await machineContext.WaitSeconds(0.01f);
				await PerformGoldTrainOutInterlude();
				await PerformTrainLineEnd();
			}
			else
			{
				await PerformTrainLineEnd();
				await machineContext.WaitSeconds(0.5f);
				await PerformCollectValueToGround();
				await machineContext.state.Get<ExtraState11030>().SettleBonusProgress();
			}
			if (isTrainFromChoose)
			{
				await PerformTrainWheelOutInterlude();
			}
			Proceed();
		}

		public async Task PerformCollectValueToGround()
		{
			var runningWheel = (WheelTrain11030)wheelsActiveState.GetRunningWheel()[0];
			long goldTrainCollectWinRate = 0;
			var showAnimationElementList = new List<ElementContainer>();
			for (int i = 0; i < runningWheel.wheelState.rollCount; i++)
			{
				var roll = runningWheel.GetRoll(i);
				for (int j = 0; j < runningWheel.GetRollRowCount(i, runningWheel.wheelState.GetWheelConfig());j++)
				{
					var container = roll.GetVisibleContainer(j);
					var element = container.GetElement();
					var elementId = element.sequenceElement.config.id;
					if (elementProvider.CheckElementTrain(elementId) || elementProvider.CheckElementValue(elementId))
					{
						long winRate;
						if (elementProvider.CheckElementTrain(elementId))
						{
							winRate = ((ElementTrain11030) element).GetTrainWinRate();
						}
						else
						{
							winRate = ((ElementValue11030) element).GetValueWinRate();
						}
						goldTrainCollectWinRate += winRate;
						container.PlayElementAnimation("Win");
						if (elementProvider.CheckElementTrain(elementId))
						{
							((ElementTrain11030)container.GetElement()).SetSymbolUpperValue(winRate);	
						}
						showAnimationElementList.Add(container);
					}
					else if (elementProvider.CheckElementStar(elementId))
					{
						container.PlayElementAnimation("Trigger");
						showAnimationElementList.Add(container);
					}
				}
			}
			var totalWinChip = machineContext.state.Get<BetState>().GetPayWinChips(goldTrainCollectWinRate);
			// AudioUtil.Instance.PlayAudioFx("J012345_Trigger",true);
			
			uint winLevel = 0;
			for (var i = Constant11030.WinLevelDictionary.Count-1; i >= 0; i--)
			{
				if (goldTrainCollectWinRate >= Constant11030.WinLevelDictionary[i])
				{
					winLevel = (uint)i + 1;
					break;
				}
			}
			var winTime = 0f;
			if (winLevel >= 3)
			{
				winTime = 6f;
			}
			if (winLevel == 2)
			{
				winTime = 4f;
			}
			if (winLevel == 1)
			{
				winTime = 2f;
			}

			// AddWinChipsToControlPanel((ulong)totalWinChip,winTime);
			machineContext.state.Get<WinState>().AddCurrentWin((ulong)totalWinChip);
			machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation(
				(long) machineContext.state.Get<WinState>().currentWin,
				winTime, false, "J012345_Trigger", "J012345_TriggerStop",true);
			
			starAddValueTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(starAddValueTask,null);
			machineContext.WaitSeconds(winTime, () =>
			{
				if (starAddValueTask != null)
				{
					starAddValueTask.SetResult(true);
				}
			});
			await starAddValueTask.Task;
			machineContext.RemoveTask(starAddValueTask);
			starAddValueTask = null;
			// AudioUtil.Instance.StopAudioFx("J012345_Trigger");
			// AudioUtil.Instance.PlayAudioFx("J012345_TriggerStop");
			for (int i = 0; i < showAnimationElementList.Count; i++)
			{
				var container = showAnimationElementList[i];
				var element = container.GetElement();
				var elementId = element.sequenceElement.config.id;
				long winRate = 0;
				if (elementProvider.CheckElementTrain(elementId))
				{
					winRate = ((ElementTrain11030) element).GetTrainWinRate();
				}
				container.UpdateAnimationToStatic();
				container.UpdateElementMaskInteraction(false);
				if (elementProvider.CheckElementTrain(elementId))
				{
					((ElementTrain11030)container.GetElement()).SetSymbolUpperValue(winRate);	
				}
			}
		}

		public bool IsInReconnect()
		{
			return IsFromMachineSetup() && canReconnectFlag;
		}

		public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params  object[] args)
		{
			switch (internalEvent)
			{
				case MachineInternalEvent.EVENT_CONTROL_STOP:
					machineContext.view.Get<ControlPanel>().StopWinAnimation();
					if (starAddValueTask != null)
					{
						starAddValueTask.SetResult(false);
					}
					break;
			}
            
			base.OnMachineInternalEvent(internalEvent, args);
		}
		public void RefreshSymbolUpperValue(GoldRushTrainGameResultExtraInfo.Types.Train train)
		{
			var chipValue = train.TotalWinRate;
			var backWheel = wheelsActiveState.GetRunningWheel()[0];
			var roll = backWheel.GetRoll((int) train.X);
			var container = roll.GetVisibleContainer((int) train.Y);
			((ElementTrain11030)container.GetElement()).SetSymbolUpperValue((long) chipValue);
		}

		public async Task PerformCollectValueToTopFrame()
		{
			var runningWheel = (WheelTrain11030)wheelsActiveState.GetRunningWheel()[0];
			var goldTrain = extraState.GetLastGoldTrain();
			var goldTrainContainer = runningWheel.GetRoll((int) goldTrain.X).GetVisibleContainer((int) goldTrain.Y);
			goldTrainContainer.PlayElementAnimation("Trigger");
			
			var goldTrainCollectNumAnimator = runningWheel.TrainLine;
			var goldTrainCollectNum = goldTrainCollectNumAnimator.transform.Find("IntegralGroup/IntegralText").GetComponent<TextMesh>();
			goldTrainCollectNum.text = "";
			long goldTrainCollectWinRate = 0;
			// bool firstUpdateFlag = true;
			// firstUpdateFlag = false;
			await XUtility.PlayAnimationAsync(goldTrainCollectNumAnimator,"NoticeOpen");
			void UpdateCollectNum(long nowNum)
			{
				goldTrainCollectNum.text = nowNum.GetCommaFormat();
				// if (firstUpdateFlag)
				// {
				// 	firstUpdateFlag = false;
				// 	XUtility.PlayAnimation(goldTrainCollectNumAnimator,"NoticeOpen");
				// }
				XUtility.PlayAnimationAsync(goldTrainCollectNumAnimator,"NoticeCollect");
			}
			for (int i = 0; i < runningWheel.wheelState.rollCount-1; i++)
			{
				var roll = runningWheel.GetRoll(i);
				for (int j = 0; j < runningWheel.GetRollRowCount(i, runningWheel.wheelState.GetWheelConfig());j++)
				{
					var container = roll.GetVisibleContainer(j);
					var element = container.GetElement();
					var elementId = element.sequenceElement.config.id;
					if (elementProvider.CheckElementTrain(elementId) || elementProvider.CheckElementValue(elementId))
					{
						long winRate;
						if (elementProvider.CheckElementTrain(elementId))
						{
							winRate = ((ElementTrain11030) element).GetTrainWinRate();
						}
						else
						{
							winRate = ((ElementValue11030) element).GetValueWinRate();
						}
						goldTrainCollectWinRate += winRate;
						var chips = machineContext.state.Get<BetState>().GetPayWinChips(goldTrainCollectWinRate);
					
						UpdateCollectNum(chips);
						container.PlayElementAnimation("Collect",false, () =>
						{
							container.UpdateAnimationToStatic();
							container.UpdateElementMaskInteraction(false);
							if (elementProvider.CheckElementTrain(elementId))
							{
								((ElementTrain11030)container.GetElement()).SetSymbolUpperValue(winRate);	
							}
						});
						if (elementProvider.CheckElementTrain(elementId))
						{
							((ElementTrain11030)container.GetElement()).SetSymbolUpperValue(winRate);	
						}
						AudioUtil.Instance.PlayAudioFx("J012345_Collect");
						await machineContext.WaitSeconds(1f);
					}
				}
			}
			await machineContext.WaitSeconds(1f);
			await XUtility.PlayAnimationAsync(goldTrainCollectNumAnimator,"NoticeClose");
			machineContext.WaitSeconds(3f, () =>
			{
				goldTrainContainer.PlayElementAnimation("Idle");
			});
		}

		private async Task BlinkScatterLine()
		{
			var runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
			var bonusWinLines =  runningWheel.wheelState.GetBonusWinLine();
			for (int i = 0; i < bonusWinLines.Count; i++)
			{
				for (var index = 0; index < bonusWinLines[i].Positions.Count; index++)
				{
					var pos = bonusWinLines[i].Positions[index];
					var container =  runningWheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
					//AudioUtil.Instance.PlayAudioFx(container.sequenceElement.config.name + "_Trigger");
					container.PlayElementAnimation("Trigger");
					container.ShiftSortOrder(true);
				}
			}
			// for (var i = 0; i < runningWheel.rollCount; i++)
			// {
			// 	var roll = runningWheel.GetRoll(i);
			// 	for (var j = 0; j < roll.rowCount; j++)
			// 	{
			// 		var container = roll.GetVisibleContainer(j);
			// 		if (Constant11030.ScatterAndWildList.Contains(container.sequenceElement.config.id))
			// 		{
			// 			container.PlayElementAnimation("Trigger");
			// 			container.ShiftSortOrder(true);
			// 		}
			// 	}
			// }
			AudioUtil.Instance.PlayAudioFx("B01_Trigger");
			await machineContext.WaitSeconds(runningWheel.wheelState.GetWheelConfig().bonusLineBlinkDuration);
		}
		private async Task StartChooseGame()
		{
			await machineContext.WaitSeconds(0.3f);
			AudioUtil.Instance.StopMusic();
			// await BlinkBonusLine();
			await BlinkScatterLine();
			var popup = PopUpManager.Instance.ShowPopUp<UISelectionFeature11030>("UISelectFeature");
			popup.Initialize(machineContext);
			var popCompleteTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(popCompleteTask,null);
			popup.SetChooseCallback(async (choice) =>
			{
				await ChooseCallback(choice);
				machineContext.RemoveTask(popCompleteTask);
				popCompleteTask.SetResult(true);
			});
			if (Constant11030.debugType)
			{
				machineContext.WaitSeconds(0.5f, () =>
				{
					popup.OnFreeBtnClicked();
				});
			}

			AudioUtil.Instance.UnPauseMusic();
			AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
			AudioUtil.Instance.PlayAudioFx("SelectFeature__Open");
			await popCompleteTask.Task;
		}
		
		private async Task ChooseCallback(int choice)
		{
			CBonusProcess cBonusProcess = new CBonusProcess();
			cBonusProcess.Json = "Link";
			await extraState.SendBonusProcess(choice == 0 ? cBonusProcess : null);
			// Proceed();
		}
		
		protected override void Proceed()
		{
			if (extraState.HasBonusGame())
			{
				machineContext.JumpToLogicStep(LogicStepType.STEP_BONUS,LogicStepType.STEP_BONUS);
			}
			else
			{
				base.Proceed();	
			}
		}
	}
}