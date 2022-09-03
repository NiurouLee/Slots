using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using GameModule;
using UnityEngine.Rendering;
using UnityEngine.UI;

// using Google.Protobuf.Collections;

namespace GameModule
{
	public class LinkLogicProxy11022 : LinkLogicProxy
	{
		private List<Item> _listNewDrop;
		private uint _respinLimitCount;
		private uint _respinCurCount;
		private Dictionary<int, LinkPerformBigElement11022> performElementDictionary = new Dictionary<int, LinkPerformBigElement11022>();
		// private Dictionary<uint, AmalgamationGameResultExtraInfo.Types.ShapeItem> lastViewState = new Dictionary<uint, AmalgamationGameResultExtraInfo.Types.ShapeItem>();
		private ExtraState11022 extraState;
		private Wheel lastSingleSpinWheel;
		private int lastCollectShapeIndex;
		private ElementConfigSet tempElementConfigSet;
		IndependentWheel linkWheel;
		Transform rollsRoot;
		LinkWheelState11022 linkWheelState;
		private Animator machineContextAnimator;
		private AudioSource singleWheelSpinAudioSource;
		public LinkLogicProxy11022(MachineContext context)
		:base(context)
		{
			strLinkTriggerSound = "J01_Trigger";
			machineContextAnimator = machineContext.transform.GetComponent<Animator>();
			extraState = context.state.Get<ExtraState11022>();
			_listNewDrop = new List<Item>();
			tempElementConfigSet = machineContext.machineConfig.GetElementConfigSet();
			linkWheel =  machineContext.view.Get<IndependentWheel>();
			rollsRoot = linkWheel.transform.Find("Rolls");
			linkWheelState = machineContext.state.Get<LinkWheelState11022>();
			// var tempSpinningController = (WheelSpinningController<IndependentWheelAnimationController11022>) linkWheel.spinningController;
			// var tempAnimationController = (IndependentWheelAnimationController11022)tempSpinningController.animationController;
			// tempAnimationController.BindLinkProxy(this);
			((IndependentWheelAnimationController11022)((WheelSpinningController<IndependentWheelAnimationController11022>)linkWheel.spinningController).animationController).BindLinkProxy(this);
		}
		protected override async Task HandleReSpinStartLogic()
		{
			await Task.CompletedTask;
		}
		protected override async Task HandleLinkGame()
		{
			if (extraState.IsLastLinkNeedInitialized() && !extraState.IsLinkNeedInitialized())
			{
				var linkWheel = machineContext.view.Get<IndependentWheel>("WheelLinkGame");
				var wheelsActiveState = machineContext.state.Get<WheelsActiveState11022>();
				linkWheel.wheelState.UpdateCurrentActiveSequence(wheelsActiveState.GetReelNameForWheel(linkWheel));
			}
			
			_listNewDrop.Clear();
			if (IsFromMachineSetup())
			{
				InitLinkUI();
			}
			else
			{
				UpdateLinkWheelLockElements();	
			}
			var runningWheel = machineContext.state.Get<WheelsActiveState11022>().GetRunningWheel()[0];
			if (!extraState.IsLinkNeedInitialized() && !runningWheel.wheelName.Contains("X") && !IsFromMachineSetup())
			{
				await PerformCompositeElement();	
				await machineContext.WaitSeconds(1);
			}

			if (!IsLinkSpinFinished() && !extraState.IsLinkNeedInitialized())
			{
				UpdateRespinCount(reSpinState.ReSpinCount, reSpinState.ReSpinLimit);
				machineContext.view.Get<LinkSpinTitleView11022>().ShowLight();
			}
		}

		public override bool NeedSettle()
		{
			return reSpinState.ReSpinNeedSettle && extraState.GetNextCollectShapeIndex() == -1;
		}
		protected override async void HandleCustomLogic()
		{
			//处理触发Link：开始弹板或者过场动画
			if (IsLinkTriggered())
			{
				StopBackgroundMusic();
				await HandleReSpinStartLogic();
				await HandleLinkGameTrigger();
				await HandleLinkBeginPopup();
				await HandleLinkBeginCutSceneAnimation();
			}

			//处理Link逻辑：锁图标或者其他
			if (IsInRespinProcess())
			{
				await HandleLinkGame();
			}

			//是否Link结束：处理结算过程
			if (IsLinkSpinFinished())
			{
				// StopBackgroundMusic();
				await HandleLinkReward();
			}
			//Link结算完成，恢复Normal
			if (NeedSettle())
			{
				StopBackgroundMusic();
				await HandleLinkFinishPopup();
				await HandleLinkFinishCutSceneAnimation();
				await HandleLinkHighLevelEffect();
			}
			Proceed();
		}
		protected override async  Task HandleLinkReward()
		{
			// AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
			var nowShapeData = extraState.GetShapeItems();
			int lastCollectShapeIndex = -1;
			int nextSpinShapeIndex = -1;
			for (int i = 0; i < nowShapeData.Count; i++)
			{
				var tempShape = nowShapeData[i];
				if (tempShape.IsOver)
				{
					lastCollectShapeIndex = i;
				}
				else
				{
					nextSpinShapeIndex = i;
					break;
				}
			}

			var runningWheel = machineContext.state.Get<WheelsActiveState11022>().GetRunningWheel()[0];

			if (runningWheel.wheelName.Contains("X") && lastCollectShapeIndex > -1)
			{
				if (singleWheelSpinAudioSource != null)
				{
					singleWheelSpinAudioSource.Stop();
				}
				AudioUtil.Instance.PlayAudioFx("B02_Blink01");
				AudioUtil.Instance.PlayAudioFx("Wheel_Stop");
				XUtility.PlayAnimation(machineContextAnimator, "Win");
				var tempShape = nowShapeData[lastCollectShapeIndex];
				var tempSequenceElement = new SequenceElement(tempElementConfigSet.GetElementConfig(tempShape.SymbolId), machineContext);
				var performElement = performElementDictionary[(int) tempShape.PositionId];
				performElement.UpdateElement(tempSequenceElement,true);
				performElement.containerView.transform.gameObject.SetActive(true);
				// var tempSortingOrder = performElement.containerView.GetBaseSortOrder();
				// int tempSortingOrder = -1;
				performElement.containerView.UpdateBaseSortingOrder(200);
				performElement.containerView.transform.GetComponent<SortingGroup>().enabled = true;
				performElement.containerView.PlayElementAnimation("Win",false, () =>
				{
					performElement.containerView.transform.GetComponent<SortingGroup>().sortingOrder = -1;
					// performElement.containerView.UpdateBaseSortingOrder(tempSortingOrder);
				});
				await machineContext.WaitSeconds(0.5f);
				await CollectShape(tempShape);
			}

			if (extraState.IsInLinkCollect())
			{
				for (int i = 0; i <= lastCollectShapeIndex; i++)
				{
					var tempShape = nowShapeData[i];
					var tempSequenceElement =
						new SequenceElement(tempElementConfigSet.GetElementConfig(tempShape.SymbolId), machineContext);
					var performElement = performElementDictionary[(int) tempShape.PositionId];
					// if (performElement.IsPerformEnd())
					// {
					// 	break;
					// }
					performElement.PerformOpenBox();
					machineContext.WaitSeconds(1.0f, () =>
					{
						XUtility.PlayAnimation(machineContextAnimator, "Zhenping1");
					});
					performElement.UpdateElement(tempSequenceElement,false);
					performElement.containerView.transform.GetComponent<SortingGroup>().enabled = true;
					// performElement.containerView.UpdateBaseSortingOrder(-1);
					performElement.containerView.transform.GetComponent<SortingGroup>().sortingOrder = -1;
					await machineContext.WaitSeconds(1.5f);
					await CollectShape(tempShape,true);
				}
				var waitTask = new TaskCompletionSource<bool>();
				bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
				machineContext.serviceProvider.GetSpinResult(machineContext.state.Get<BetState>().totalBet, isAutoSpin,machineContext,(sSpin) =>
				{
					// XDebug.Log($"<color=yellow>=======11022LinkSettleSpinGameResult: Response: {LitJson.JsonMapper.ToJsonField(sSpin.GameResult)}</color>"); 
					// machineContext.state.UpdateMachineStateOnSpinResultReceived(sSpin);
					machineContext.state.Get<ReSpinState11022>().UpdateStateOnReceiveSpinResult(sSpin);
					waitTask.SetResult(true);
				});
				await waitTask.Task;
			}

			if (nextSpinShapeIndex > -1)
			{
				var tempShape = nowShapeData[nextSpinShapeIndex];
				var performElement = performElementDictionary[(int) tempShape.PositionId];
				var singleWheelName = Constant11022.GetSingleWheelName((int)tempShape.Width,(int)tempShape.Height);
				var playNode = performElement.containerView.GetElement().transform.Find("container/Play");
				playNode.gameObject.SetActive(true);
				
				var nextRunningWheel = machineContext.view.Get<Wheel>(singleWheelName);
				nextRunningWheel.wheelMask.frontSortingLayerID = SortingLayer.NameToID("SoloElement");
				nextRunningWheel.wheelMask.backSortingLayerID = SortingLayer.NameToID("SoloElement");
				
				var btn = new GameObject("PressBtn");
				btn.AddComponent<Transform>();
				btn.AddComponent<BoxCollider2D>();
				btn.GetComponent<BoxCollider2D>().size = new Vector2(nextRunningWheel.contentWidth,nextRunningWheel.contentHeight);
				btn.transform.SetParent(performElement.containerView.transform,false);
				var tempBtnTask = new TaskCompletionSource<bool>();
				var pointerEventCustomHandler = btn.AddComponent<PointerEventCustomHandler>();
				pointerEventCustomHandler.BindingPointerClick((pointerData) =>
				{
					AudioUtil.Instance.PlayAudioFx("AJ01_Click"); 
					GameObject.Destroy(btn);
					tempBtnTask.SetResult(true);
				});
				if (Constant11022.debugType)
				{
					GameObject.Destroy(btn);
					tempBtnTask.SetResult(true);
				}
				machineContext.view.Get<LinkSpinTitleView11022>().Hide();
				machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);    
				await tempBtnTask.Task;
				machineContext.WaitSeconds(1.0f, () =>
				{
					XUtility.PlayAnimation(machineContextAnimator, "Zhenping2");
				});
				playNode.gameObject.SetActive(false);
				performElement.PerformOpenBox();
				performElement.containerView.transform.gameObject.SetActive(false);
				machineContext.state.Get<WheelsActiveState11022>().UpdateSingleWheelState(singleWheelName);
				nextRunningWheel.transform.position = performElement.containerView.transform.position;
				machineContext.WaitSeconds(0.5f, () =>
				{
					singleWheelSpinAudioSource = AudioUtil.Instance.PlayAudioFx("Wheel_Spin");
				});
				await machineContext.WaitSeconds(1f);
			}
			// else if (extraState.IsInLinkCollect())
			// {
			// 	var shapeDictionary = new Dictionary<int,AmalgamationGameResultExtraInfo.Types.ShapeItem>();
			// 	for (int i = 0; i < nowShapeData.Count; i++)
			// 	{
			// 		var tempShape = nowShapeData[i];
			// 		shapeDictionary[(int)tempShape.PositionId] = tempShape;
			// 	}
   //
			// 	for (int i = 0; i < 15; i++)
			// 	{
			// 		if (shapeDictionary.ContainsKey(i))
			// 		{
			// 			var tempShape = shapeDictionary[i];
			// 			var performElement = performElementDictionary[(int) tempShape.PositionId];
			// 			await CollectShape(tempShape);
			// 		}
			// 	}
			// 	var waitTask = new TaskCompletionSource<bool>();
			// 	bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
			// 	machineContext.serviceProvider.GetSpinResult(machineContext.state.Get<BetState>().totalBet, isAutoSpin,machineContext,(sSpin) =>
   //                          {
	  //                           XDebug.Log($"<color=yellow>=======11022LinkSettleSpinGameResult: Response: {LitJson.JsonMapper.ToJsonField(sSpin.GameResult)}</color>"); 
   //                              // machineContext.state.UpdateMachineStateOnSpinResultReceived(sSpin);
   //                              machineContext.state.Get<ReSpinState11022>().UpdateStateOnReceiveSpinResult(sSpin);
   //                              waitTask.SetResult(true);
   //                          });
			// 	await waitTask.Task;
			// }
			// await Task.CompletedTask;
		}

		public async Task CollectShape(AmalgamationGameResultExtraInfo.Types.ShapeItem collectShape,bool quickProcess = false)
		{
			if (Constant11022.JackpotList.Contains(collectShape.SymbolId))
			{
				long jackpotWinRate = (long)collectShape.JackpotPay;
				var chips = machineContext.state.Get<BetState>().GetPayWinChips(jackpotWinRate);
				AddWinChipsToControlPanel((ulong)chips, 0.2f,false,false);
				await machineContext.WaitSeconds(1f);
				await ShowJackpotPopUp<UIJackpotBase11022>((int) collectShape.JackpotId,chips);
			}
			else
			{
				long winRate = (long)collectShape.WinRate;
				var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
				AddWinChipsToControlPanel((ulong)chips, 0.2f,false,false);
				if (quickProcess)
				{
					await machineContext.WaitSeconds(0.2f);	
				}
				else
				{
					await machineContext.WaitSeconds(1f);
				}
			}	
		}
		// protected override void Proceed()
		// {
		// 	// HandleToNextStep();
		// }

		public void InitCompositeElement()
		{
			if (extraState.IsLinkNeedInitialized())
			{
				return;
			}

			if (IsFromMachineSetup())
			{
				var nowShapeData = extraState.GetShapeItems();
				var rollRowCount = linkWheelState.GetWheelConfig().rollRowCount;
				for (int i = 0; i < nowShapeData.Count; i++)
				{
					var tempShape = nowShapeData[i];
					int startColumnIdx = (int)tempShape.PositionId / rollRowCount;
					int startRowIdx = (int)tempShape.PositionId % rollRowCount;
					int emdColumnIdx = startColumnIdx + (int)tempShape.Width - 1;
					int endRowIdx = startRowIdx + (int) tempShape.Height - 1;
					int endPositionId = emdColumnIdx * rollRowCount + endRowIdx;
					var tempSequenceElement = new SequenceElement(tempElementConfigSet.GetElementConfig(tempShape.SymbolId), machineContext);
					Vector3 centerPosition = (linkWheel.GetRoll((int) tempShape.PositionId).initialPosition + linkWheel.GetRoll(endPositionId).initialPosition)/2;
					for (int columnIdx = startColumnIdx; columnIdx <= emdColumnIdx; columnIdx++)
					{
						for (int rowIdx = startRowIdx; rowIdx <= endRowIdx; rowIdx++)
						{
							int tempPositionId = columnIdx * rollRowCount + rowIdx;
							var tempRoll = linkWheel.GetRoll(tempPositionId);
							var visibleContainer = tempRoll.GetContainer(1);
							if (tempRoll.transform.gameObject.activeSelf)
							{
								tempRoll.transform.gameObject.SetActive(false);
							}
						}
					}
					var bigElement = CreatePerformElement((int) tempShape.PositionId,centerPosition,tempSequenceElement,(int)tempShape.Width,(int)tempShape.Height);
					// performElementDictionary[(int) tempShape.PositionId] = new LinkPerformBigElement11022(rollsRoot,centerPosition, tempSequenceElement,(int)tempShape.Width,(int)tempShape.Height);
					if (tempShape.Width == 1)
					{
						if (!extraState.IsInLinkCollect() && IsLinkSpinFinished())
						{
							bigElement.UpdateElement(tempSequenceElement,false);
							bigElement.containerView.transform.GetComponent<SortingGroup>().enabled = true;
							bigElement.containerView.transform.GetComponent<SortingGroup>().sortingOrder = -1;
							// bigElement.containerView.UpdateBaseSortingOrder(-1);
						}
						else
						{
							bigElement.containerView.PlayElementAnimation("Idle");
						}
					}
					else if (!tempShape.IsOver)
					{
						bigElement.containerView.PlayElementAnimation("Idle");
					}
					else
					{
						bigElement.UpdateElement(tempSequenceElement,false);
						bigElement.containerView.transform.GetComponent<SortingGroup>().enabled = true;
						// bigElement.containerView.UpdateBaseSortingOrder(-1);
						bigElement.containerView.transform.GetComponent<SortingGroup>().sortingOrder = -1;
					}
				}	
			}
			else
			{
				var nowShapeData = extraState.GetShapeItems();
				var rollRowCount = linkWheelState.GetWheelConfig().rollRowCount;
				for (int i = 0; i < nowShapeData.Count; i++)
				{
					var tempShape = nowShapeData[i];
					int startColumnIdx = (int)tempShape.PositionId / rollRowCount;
					int startRowIdx = (int)tempShape.PositionId % rollRowCount;
					int emdColumnIdx = startColumnIdx + (int)tempShape.Width - 1;
					int endRowIdx = startRowIdx + (int) tempShape.Height - 1;
					for (int columnIdx = startColumnIdx; columnIdx <= emdColumnIdx; columnIdx++)
					{
						for (int rowIdx = startRowIdx; rowIdx <= endRowIdx; rowIdx++)
						{
							int tempPositionId = columnIdx * rollRowCount + rowIdx;
							ChangeToPerformLayer(tempPositionId);
						}
					}
				}
			}
		}
		public async Task PerformCompositeElement()
		{
			var performTime = 0.5f;
			var outTime = 0.5f;
			var allTime = 1.5f;
			var nowShapeData = extraState.GetShapeItems();
			var rollRowCount = linkWheelState.GetWheelConfig().rollRowCount;
			bool playAudioFlag = false;
			for (int i = 0; i < nowShapeData.Count; i++)
			{
				var tempShape = nowShapeData[i];
				if (performElementDictionary.ContainsKey((int)tempShape.PositionId) && (tempShape.Height==performElementDictionary[(int)tempShape.PositionId].height && tempShape.Width == performElementDictionary[(int)tempShape.PositionId].width))
				{
					
				}
				// else if ((tempShape.Height==1 && tempShape.Width == 1) && !performElementDictionary.ContainsKey((int)tempShape.PositionId))
				// {
				// 	int startColumnIdx = (int)tempShape.PositionId / rollRowCount;
				// 	int startRowIdx = (int)tempShape.PositionId % rollRowCount;
				// 	int startPositionId = (int) tempShape.PositionId;
				// 	var tempSequenceElement = new SequenceElement(tempElementConfigSet.GetElementConfig(tempShape.SymbolId), machineContext);
				// 	Vector3 centerPosition = linkWheel.GetRoll(startPositionId).initialPosition;
				// 	var tempRoll = linkWheel.GetRoll(startPositionId);
				// 	var visibleContainer = tempRoll.GetContainer(1);
				// 	if (tempRoll.transform.gameObject.activeSelf)
				// 	{
				// 		tempRoll.transform.gameObject.SetActive(false);
				// 	}
				// 	var bigElement = CreatePerformElement(startPositionId,centerPosition,tempSequenceElement,(int)tempShape.Width,(int)tempShape.Height);
				// 	// performElementDictionary[startPositionId] = new LinkPerformBigElement11022(rollsRoot,centerPosition, tempSequenceElement,(int)tempShape.Width,(int)tempShape.Height);
				// 	bigElement.containerView.PlayElementAnimation("Idle");
				// }
				else
				{
					playAudioFlag = true;
					int startColumnIdx = (int)tempShape.PositionId / rollRowCount;
					int startRowIdx = (int)tempShape.PositionId % rollRowCount;
					int emdColumnIdx = startColumnIdx + (int)tempShape.Width - 1;
					int endRowIdx = startRowIdx + (int) tempShape.Height - 1;
					int endPositionId = emdColumnIdx * rollRowCount + endRowIdx;
					var tempSequenceElement = new SequenceElement(tempElementConfigSet.GetElementConfig(tempShape.SymbolId), machineContext);
					Vector3 centerPosition = (linkWheel.GetRoll((int) tempShape.PositionId).initialPosition + linkWheel.GetRoll(endPositionId).initialPosition)/2;
					for (int columnIdx = startColumnIdx; columnIdx <= emdColumnIdx; columnIdx++)
					{
						for (int rowIdx = startRowIdx; rowIdx <= endRowIdx; rowIdx++)
						{
							int tempPositionId = columnIdx * rollRowCount + rowIdx;
							var tempRoll = linkWheel.GetRoll(tempPositionId);
							// var visibleContainer = tempRoll.GetContainer(1);
							// if (tempRoll.transform.gameObject.activeSelf)
							// {
							// 	machineContext.WaitSeconds(performTime, () =>
							// 	{
							// 		tempRoll.transform.gameObject.SetActive(false);
							// 	});
							// }
							if (tempRoll.transform.gameObject.activeSelf)
							{
								XDebug.LogError("roll still active while composite");
							}

							if (performElementDictionary.ContainsKey(tempPositionId))
							{
								var tempPerformElement = performElementDictionary[tempPositionId];
								performElementDictionary.Remove(tempPositionId);
								tempPerformElement.containerView.PlayElementAnimation("Out",false,() =>
								{
									tempPerformElement.DestorySelf();
								});
							}
						}
					}
					machineContext.WaitSeconds(outTime, () =>
					{
						var bigElement = CreatePerformElement((int) tempShape.PositionId,centerPosition,tempSequenceElement,(int)tempShape.Width,(int)tempShape.Height);
						// performElementDictionary[(int) tempShape.PositionId] = new LinkPerformBigElement11022(rollsRoot,centerPosition, tempSequenceElement,(int)tempShape.Width,(int)tempShape.Height);
						bigElement.containerView.PlayElementAnimation("In");
					});
				}
			}

			if (playAudioFlag)
			{
				AudioUtil.Instance.PlayAudioFx("J01_Complex");
			}
			// lastViewState.Clear();
			// for (int i = 0; i < nowShapeData.Count; i++)
			// {
			// 	var tempShape = nowShapeData[i];
			// 	lastViewState.Add(tempShape.PositionId,tempShape);
			// }
			await machineContext.WaitSeconds(allTime);
		}

		public void ChangeToPerformLayer(int changePosition)
		{
			var tempRoll = linkWheel.GetRoll(changePosition);
			if (!tempRoll.transform.gameObject.activeSelf)
			{
				return;
			}
			tempRoll.transform.gameObject.SetActive(false);
			var tempSequenceElement = new SequenceElement(tempElementConfigSet.GetElementConfig(14), machineContext);
			var centerPosition = tempRoll.initialPosition;
			var bigElement = CreatePerformElement(changePosition,centerPosition,tempSequenceElement,1,1);
			// performElementDictionary[(int) tempShape.PositionId] = new LinkPerformBigElement11022(rollsRoot,centerPosition, tempSequenceElement,(int)tempShape.Width,(int)tempShape.Height);
			bigElement.containerView.PlayElementAnimation("Idle");
		}

		public LinkPerformBigElement11022 CreatePerformElement(int position,Vector3 centerPosition,SequenceElement tempSequenceElement,int width,int height)
		{
			if (performElementDictionary.ContainsKey(position))
			{
				XDebug.LogError("Create Repeat Perform Element");
			}

			performElementDictionary[position] = new LinkPerformBigElement11022(rollsRoot,centerPosition, tempSequenceElement,width,height);
			performElementDictionary[position].containerView.UpdateBaseSortingOrder(position * 10);
			if (Constant11022.BoxList.Contains(tempSequenceElement.config.id))
			{
				performElementDictionary[position].containerView.transform.GetComponent<SortingGroup>().enabled = false;
				performElementDictionary[position].containerView.GetElement().transform.Find("container").GetComponent<SortingGroup>().sortingOrder = position;	
			}
			// performElementDictionary[position].containerView.PlayElementAnimation("Idle");
			return performElementDictionary[position];
		}
		protected override float GetElementTriggerDuration()
		{
			return 3f;
		}
		public override bool CheckCurrentStepHasLogicToHandle()
		{
			return reSpinState.ReSpinTriggered || reSpinState.IsInRespin || extraState.IsFullTrigger();
		}
		protected override async Task HandleLinkGameTrigger()
		{
			machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
			if (extraState.IsLinkNeedInitialized())
			{
				return;
			}
			if (machineContext.assetProvider.GetAsset<AudioClip>(strLinkTriggerSound))
			{
				AudioUtil.Instance.PlayAudioFx(strLinkTriggerSound);
			}
			var wheels = GetLinkGameTriggerWheels();
			for (int i = 0; i < wheels.Count; i++)
			{
				var triggerElementContainers = wheels[i].GetElementMatchFilter((container) =>
				{
					if (CheckIsTriggerElement(container))
					{
						return true;
					}

					return false;
				},GetElementTriggerOffsetRow());

				if (triggerElementContainers.Count > 0)
				{
					for (var j = 0; j < triggerElementContainers.Count; j++)
					{
						triggerElementContainers[j].PlayElementAnimation(GetElementTriggerAnimation());
						triggerElementContainers[j].ShiftSortOrder(true);
					}

					await XUtility.WaitSeconds(GetElementTriggerDuration(), machineContext);
					// for (var j = 0; j < triggerElementContainers.Count; j++)
					// {
					// 	triggerElementContainers[j].UpdateAnimationToStatic();
					// }
				}   
			}
			// machineContext.view.Get<ControlPanel>().ShowStopButton(true);
		}
		protected override async Task HandleLinkBeginCutSceneAnimation()
		{
			try
			{
				var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("FeatureStartCutPopup");

				var sortingGroup = transitionAnimation.GetComponent<SortingGroup>();
				sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
				sortingGroup.sortingOrder = 100;
 
				transitionAnimation.transform.SetParent(machineContext.transform);

				AudioUtil.Instance.PlayAudioFx("Lock_Transilation");
				if (extraState.IsLinkNeedInitialized())
				{
					XUtility.PlayAnimation(transitionAnimation.GetComponent<Animator>(), "Transition", () =>
					{
						GameObject.Destroy(transitionAnimation);
					},machineContext);
					await machineContext.WaitSeconds(2.5f);
					InitLinkUI();
				}
				else
				{
					machineContext.WaitSeconds(2.5f, () =>
					{
						InitLinkUI();
					});
					await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "Transition", machineContext);

					GameObject.Destroy(transitionAnimation);
				}
				await Task.CompletedTask;	
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
		protected override string GetLinkFinishAddress()
		{
			return "UIRespinGameFinish" + machineContext.assetProvider.AssetsId;
		}
		protected override async  Task HandleLinkFinishCutSceneAnimation()
		{
			try
			{
				var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("FeatureStartCutPopup");

				var sortingGroup = transitionAnimation.GetComponent<SortingGroup>();
				sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
				sortingGroup.sortingOrder = 100;
 
				transitionAnimation.transform.SetParent(machineContext.transform);

				machineContext.WaitSeconds(2.5f, () =>
				{
					machineContext.state.Get<WheelsActiveState11022>().UpdateLinkWheelState();
					machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
					// controlPanel.UpdateControlPanelState(false,false);
					var keys = new List<int>(performElementDictionary.Keys);
					for (int i = 0; i < keys.Count; i++)
					{
						performElementDictionary[keys[i]].DestorySelf();
					}
					performElementDictionary.Clear();
					// lastViewState.Clear();
					var linkWheel =  machineContext.view.Get<IndependentWheel>("WheelLinkGame");
					for (int i = 0; i < linkWheel.rollCount; i++)
					{
						linkWheel.GetRoll(i).transform.gameObject.SetActive(true);
					}
					ResetLinkWheels();
					if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
					{
						machineContext.state.Get<WheelsActiveState11022>().UpdateFreeWheelState();	
						AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
					}
					else
					{
						machineContext.state.Get<WheelsActiveState11022>().UpdateBaseWheelState();	
						AudioUtil.Instance.StopMusic();
						// AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
					}
					RestoreTriggerWheelElement();
				});

				AudioUtil.Instance.PlayAudioFx("Lock_Transilation");
				await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "Transition", machineContext);

				GameObject.Destroy(transitionAnimation);
  
				await Task.CompletedTask;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
		protected override void RestoreTriggerWheelElement()
		{
			var triggerPanels = reSpinState.triggerPanels;
			if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
			{
				var runningWheel = machineContext.state.Get<WheelsActiveState11022>().GetRunningWheel()[0];
				runningWheel.wheelState.UpdateWheelStateInfo(triggerPanels[0]);
				runningWheel.ForceUpdateElementOnWheel();
				for (var i = 0; i < runningWheel.GetMaxSpinningUpdaterCount(); i++)
				{
					var roll = runningWheel.GetRoll(i);
					for (var j = 0; j < runningWheel.GetRollRowCount(0,runningWheel.wheelState.GetWheelConfig()); j++)
					{
						var container = roll.GetVisibleContainer(j);
						if (container.sequenceElement.config.id == 13)
						{
							container.UpdateElement(container.sequenceElement,true);
							container.UpdateElementMaskInteraction(true);
							container.ShiftSortOrder(true);
						}
					}
				}
				// machineContext.state.Get<WheelState>().UpdateWheelStateInfo(triggerPanels[0]);
				// machineContext.view.Get<Wheel>().ForceUpdateElementOnWheel();   
			}
		}
		protected override bool IsLinkTriggered()
		{
			return reSpinState.ReSpinTriggered || (extraState.IsFullTrigger() && !IsFromMachineSetup());
		}
		protected override bool CheckIsTriggerElement(ElementContainer container)
		{
			return Constant11022.LinkList.Contains(container.sequenceElement.config.id);
		}
		
		private void InitLinkUI()
		{
			AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
			machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
			_respinLimitCount = reSpinState.ReSpinLimit;
			// controlPanel.UpdateControlPanelState(true,false);
			UpdateRespinCount(reSpinState.ReSpinCount, reSpinState.ReSpinLimit, true);
			machineContext.state.Get<WheelsActiveState11022>().UpdateLinkWheelState();
			UpdateLinkWheelLockElements();
			InitCompositeElement();
		}
		public void UpdateRespinCount(uint reSpinCount, uint reSpinLimit, bool isEnterRoom=false)
		{
			if (isEnterRoom)
			{
				machineContext.view.Get<LinkSpinTitleView11022>().UpdateContent((int)reSpinCount);  
			}
			else
			{
				machineContext.view.Get<LinkSpinTitleView11022>().UpdateContent((int)reSpinCount-1);  
			}
		}
		
		
		private void UpdateLinkWheelLockElements()
        {
	        var runningWheel = machineContext.state.Get<WheelsActiveState11022>().GetRunningWheel()[0];
	        if (extraState.IsLinkNeedInitialized() || runningWheel.wheelName.Contains("X"))
	        {
		        return;
	        }

	        // var wheel = machineContext.view.Get<IndependentWheel>();
            var linkWheelState = machineContext.state.Get<LinkWheelState11022>();
            var items = machineContext.state.Get<ExtraState11022>().GetLinkItems();
            var isInit = IsLinkTriggered() || IsFromMachineSetup();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                // var lockRoll = wheel.GetRoll(id) as SoloRoll11022;
                if (!linkWheelState.IsRollLocked(id) && item.SymbolId>0)
                {
                    linkWheelState.SetRollLockState(id, true);
                    UpdateRunningElement(item.SymbolId, id,0,true);
                    var elementContainer = GetRunningElementContainer(id);
                    var element = elementContainer.GetElement();
                    
                    elementContainer.ShiftSortOrder(true);
                    elementContainer.PlayElementAnimation("Idle");
                    // if (isInit)
                    // {
                    //     lockRoll.ShiftRollMaskAndSortOrder(2100);   
                    // }
                }
            }
        }
		
		private void ResetLinkWheels()
		{
			machineContext.view.Get<LinkSpinTitleView11022>().Show();
			var items = extraState.GetLinkItems();
			var linkWheelState = machineContext.state.Get<LinkWheelState11022>();
			linkWheelState.ResetSpinResultElementDef();
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				int id = (int) item.PositionId;
				UpdateRunningElement(Constant11022.GetRandomNormalElementId(), id);
				linkWheelState.SetRollLockState(id, false);
			}
		}
		
		protected virtual async Task ShowJackpotPopUp<T>(int jpType,long chips) where T : UIJackpotBase11022
		{
			string address = "UIJackpot"+ Constant11022.jackpotName[jpType] + machineContext.assetProvider.AssetsId;

			if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
			{
				XDebug.LogError($"ShowFreeGameFinishPopUp:{address} is Not Exist" );    
				return;
			}

			TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(waitTask, null);

			var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
			startPopUp.SetJackpotWinNum((ulong)chips);
			startPopUp.SetPopUpCloseAction(() =>
			{
				machineContext.RemoveTask(waitTask);
				waitTask.SetResult(true);
			});

			await waitTask.Task;
		}
		protected override async Task HandleLinkFinishPopup()
		{
			if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkFinishAddress()) != null)
			{
				var task = GetWaitTask();
				var finishLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinFinishPopUp11022>(GetLinkFinishAddress());
				if (!ReferenceEquals(finishLinkPopup, null))
				{
					finishLinkPopup.SetPopUpCloseAction(() =>
					{
						//finishLinkPopup.Close();
						SetAndRemoveTask(task);
					});
					finishLinkPopup.Initialize(machineContext);
					if (finishLinkPopup.IsAutoClose())
					{
						await machineContext.WaitSeconds(GetLinkFinishPopupDuration());
						finishLinkPopup.Close();
					}else if (Constant11022.debugType)
					{
						await machineContext.state.Get<ReSpinState>().SettleReSpin();
						finishLinkPopup.Close();
					}
				}
				await task.Task;
			}
			await Task.CompletedTask;
		}

		protected override void Proceed()
		{
			if (extraState.HasBonusGame())
			{
				machineContext.JumpToLogicStep(LogicStepType.STEP_BONUS,LogicStepType.STEP_RE_SPIN);
			}
			else
			{
				base.Proceed();	
			}
		}
	}
}