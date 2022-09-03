using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using Spine;
using Spine.Unity;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace GameModule
{
	public class LinkLogicProxy11024 : LinkLogicProxy
	{
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
		public LinkLogicProxy11024(MachineContext context)
		:base(context)
		{
			
		}
		protected override void RecoverLogicState()
		{
			if (IsLinkTriggered())
			{
				activeWheelState.UpdateBaseWheelState();
				RestoreTriggerWheelElement();
			}
			else if (NeedSettle())
			{
				activeWheelState.UpdateLinkWheelState();
				UpdateLinkWheelLockElements();
				var runningWheelList = activeWheelState.GetRunningWheel();
				WheelLink11024 collectBoardWheel = (WheelLink11024) runningWheelList[runningWheelList.Count - 1];
				for (var i = 0; i < runningWheelList.Count; i++)
				{
					var linkWheel = (WheelLink11024) runningWheelList[i];
					linkWheel.ShowWinValueBoard(i == runningWheelList.Count - 1);
				}

				long totalWinNum = 0;
				for (var i = 0; i < runningWheelList.Count; i++)
				{
					var linkWheel = (WheelLink11024) runningWheelList[i];
					var linkData = linkWheel.GetLinkData().Items;
					for (var i1 = 0; i1 < linkData.Count; i1++)
					{
						var itemData = linkData[i1];
						if (itemData.SymbolId > 0)
						{
							var winRate = itemData.WinRate + itemData.JackpotPay;
							var winValue = (long) betState.GetPayWinChips(winRate);
							var stickyElement = linkWheel.GetStickyElement((int) itemData.PositionId);
							stickyElement.ShowCollectAnimationComplete();
							totalWinNum += winValue;
						}
					}
				}
				collectBoardWheel.SetCollectValue(totalWinNum);
			}
			else
			{
				activeWheelState.UpdateLinkWheelState();
				UpdateLinkWheelLockElements();
			}
		}
		protected override async void HandleCustomLogic()
		{
			//处理触发Link：开始弹板或者过场动画
			if (IsLinkTriggered())
			{
				StopBackgroundMusic();
				await HandleLinkGameTrigger();
				await HandleLinkBeginPopup();
				await HandleLinkBeginCutSceneAnimation();
				await PerformBeforeLinkStart();
			}

			//处理Link逻辑：锁图标或者其他
			if (IsInRespinProcess())
			{
				await HandleLinkGame();
			}

			//是否Link结束：处理结算过程
			if (IsLinkSpinFinished())
			{
				StopBackgroundMusic();
				await HandleLinkReward();
			}
			//Link结算完成，恢复Normal
			if (NeedSettle())
			{
				StopBackgroundMusic();
				await HandleLinkFinishPopup();
				await HandleLinkFinishCutSceneAnimation();
				await HandleLinkHighLevelEffect();
				await AddValueToFloor();
			}
			Proceed();
		}
		protected override string GetLinkBeginAddress()
		{
			var pigType = "";
			for (var i = 0; i < 3; i++)
			{
				if (extraState.HasReSpinType(i))
				{
					pigType += i + 1;
				}
			}
			return "UIlinkGame" + pigType;
		}
		protected override string GetLinkFinishAddress()
		{
			return "UIlinkGameSettlement";
		}
		protected override async Task HandleLinkGameTrigger()
		{
			await pigGroup.PerformExplode();
		}
		protected override async Task HandleLinkFinishCutSceneAnimation()
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
			var wheelList = activeWheelState.GetRunningWheel();
			for (var i = 0; i < wheelList.Count; i++)
			{
				var linkWheel = (WheelLink11024)wheelList[i];
				linkWheel.CleanWheel();
			}
			activeWheelState.UpdateBaseWheelState();
			RestoreTriggerWheelElement();
			await task.Task;
		}
		protected override async  Task HandleLinkBeginCutSceneAnimation()
		{
			var task = GetWaitTask();
			AudioUtil.Instance.PlayAudioFx("LinkGame_Video1");
			var interlude = machineContext.assetProvider.InstantiateGameObject("Transition");
			interlude.transform.SetParent(machineContext.transform,false);
			var pigTypeList = new List<int>();
			for (var i = 0; i < 3; i++)
			{
				if (extraState.HasReSpinType(i))
					pigTypeList.Add(i);
			}
			// var transformPigType = pigTypeList[Random.Range(0,pigTypeList.Count)];
			var nameList = new List<string>
			{
				"Purple",
				"Red",
				"Green",
			};

			var animatorStateName = "";
			for (var i = 0; i < pigTypeList.Count; i++)
			{
				animatorStateName += nameList[pigTypeList[i]];
			}
			XUtility.PlayAnimation(interlude.GetComponent<Animator>(), "Transition"+animatorStateName,()=>{
				GameObject.Destroy(interlude);
				task.SetResult(true);
			},machineContext);
			// if (extraState.HasReSpinType(0) && extraState.HasReSpinType(1) && extraState.HasReSpinType(2))
			// {
			// 	animatorStateName = "PurpleLeRedGreen";
			// }
			// else if (extraState.HasReSpinType(0) && extraState.HasReSpinType(1))
			// {
			// 	animatorStateName = "PurpleLeRed";
			// }
			// else if (extraState.HasReSpinType(0) && extraState.HasReSpinType(2))
			// {
			// 	animatorStateName = "PurpleLeGreen";
			// }
			// var skeletonGroup = interlude.transform.Find("Node/Object/" + animatorStateName);
			// for (var i = 0; i < pigTypeList.Count; i++)
			// {
			// 	var pigType = nameList[pigTypeList[i]];
			// 	var skeleton = skeletonGroup.Find(pigType);
			// 	var skeletonAnimation = skeleton.GetComponent<SkeletonAnimation>();
			// 	machineContext.WaitSeconds(0.1f, () =>
			// 	{
			// 		var skeletonSkeleton = skeletonAnimation.skeleton;
			// 		skeletonSkeleton.SetSkin(pigType);
			// 	});
			// }

			await machineContext.WaitSeconds(1f);
			var coinPosList = new Dictionary<int,uint>();
			for (var i = 0; i < baseWheel.rollCount; i++)
			{
				var roll = baseWheel.GetRoll(i);
				for (var i1 = 0; i1 < roll.rowCount; i1++)
				{
					var container = roll.GetVisibleContainer(i1);
					if (Constant11024.IsAnyPigId(container.sequenceElement.config.id) >= 0)
					{
						coinPosList[i * 3 + i1] = container.sequenceElement.config.id;
					}
				}
			}
			activeWheelState.UpdateLinkWheelState();
			var keyList = new List<int>(coinPosList.Keys);
			WheelLink11024 initWheel;
			if (extraState.HasReSpinType(1))
			{
				initWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame2_2");
			}
			else
			{
				initWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame1");
			}
			
			for (var i = 0; i < keyList.Count; i++)
			{
				var key = keyList[i];
				initWheel.UpdateRunningElement(coinPosList[key], key, 0, true);
			}

			if (extraState.HasReSpinType(1))
			{
				var extraWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame2_1");
				var animator = extraWheel.transform.GetComponent<Animator>();
				XUtility.PlayAnimation(animator,"OpenIdle");
				initWheel.transform.GetComponent<SortingGroup>().enabled = true;
				extraWheel.transform.GetComponent<SortingGroup>().enabled = true;
			}
			//
			// if (extraState.HasReSpinType(2))
			// {
				var runningWheel = activeWheelState.GetRunningWheel();
				for (var i = 0; i < runningWheel.Count; i++)
				{
					var linkWheel = (WheelLink11024)runningWheel[i];
					linkWheel.SetLeftSpinTimes(0);
					linkWheel.HideSpinTimes();
				}
			// }
			await task.Task;
		}
		private void UpdateLinkWheelLockElements()
		{
			var runningWheel = activeWheelState.GetRunningWheel();
			for (var i1 = 0; i1 < runningWheel.Count; i1++)
			{
				var linkWheel = (WheelLink11024)runningWheel[i1];
				var linkWheelState = linkWheel.wheelState;
				var items = extraState.GetLinkData(linkWheel.linkWheelIndex).Items;
				for (int i = 0; i < items.count; i++)
				{
					var item = items[i];
					int id = (int) item.PositionId;
					// var lockRoll = wheel.GetRoll(id) as SoloRoll11022;
					if (!linkWheelState.IsRollLocked(id) && item.SymbolId>0)
					{
						linkWheel.UpdateRunningElement(item.SymbolId, id,0,true);
						linkWheel.wheelState.SetRollLockState(id, true);
						linkWheel.GetRoll(id).transform.gameObject.SetActive(false);
						linkWheel.GetStickyElement(id).SetStickyData(linkWheel.GetItemData(id),true);
					}
				}
			}
		}

		public async Task GlowExtraWheel()
		{
			AudioUtil.Instance.PlayAudioFx("LinkGame_SwitchDouble");
			var glowWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame2_1");
			var animator = glowWheel.transform.GetComponent<Animator>();
			await XUtility.PlayAnimationAsync(animator,"Open",machineContext);
			var initWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame2_2");
			initWheel.transform.GetComponent<SortingGroup>().enabled = false;
			glowWheel.transform.GetComponent<SortingGroup>().enabled = false;
		}

		public async Task ChangePigToGold()
		{
			WheelLink11024 initWheel;
			AudioUtil.Instance.PlayAudioFx("LinkGame_SwitchColors");
			if (extraState.HasReSpinType(1))
			{
				initWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame2_2");
			}
			else
			{
				initWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame1");
			}
			var linkData = extraState.GetLinkData(initWheel.linkWheelIndex);
			for (var i = 0; i < initWheel.rollCount; i++)
			{
				var roll = initWheel.GetRoll(i);
				var container = roll.GetVisibleContainer(0);
				var pigType = Constant11024.IsAnyPigId(container.sequenceElement.config.id);
				if (pigType >= 0)
				{
					var itemData = initWheel.GetItemData(i);
					var localI = i;
					var coverLight = machineContext.assetProvider.InstantiateGameObject("Fx_hit", true);
					coverLight.SetActive(false);
					coverLight.SetActive(true);
					coverLight.transform.SetParent(initWheel.transform,false);
					coverLight.transform.position = container.transform.position;
					machineContext.WaitSeconds(2f, () =>
					{
						machineContext.assetProvider.RecycleGameObject("Fx_hit", coverLight);
					});
					machineContext.WaitSeconds(0.2f, () =>
					{
						initWheel.UpdateRunningElement(itemData.SymbolId, localI, 0, false);
						initWheel.wheelState.SetRollLockState(localI, true);
						roll.transform.gameObject.SetActive(false);
						initWheel.GetStickyElement(localI).SetStickyData(itemData,true);
					});
				}
			}
			await machineContext.WaitSeconds(1f);
		}

		public async Task RaiseSpinTimes()
		{
			var taskList = new List<Task>();
			var wheelList = activeWheelState.GetRunningWheel();
			for (var i = 0; i < wheelList.Count; i++)
			{
				var linkWheel = (WheelLink11024) wheelList[i];
				taskList.Add(linkWheel.RaiseSpinTimes());
			}
			await Task.WhenAll(taskList);
		}
		public async Task CopyGoldElement()
		{
			var bottomWheel =  machineContext.view.Get<WheelLink11024>("WheelLinkGame2_2");
			var extraWheel = machineContext.view.Get<WheelLink11024>("WheelLinkGame2_1");
			for (var i = 0; i < bottomWheel.rollCount; i++)
			{
				var container = bottomWheel.GetRoll(i).GetVisibleContainer(0);
				var id = container.sequenceElement.config.id;
				if (Constant11024.IsGoldId(id))
				{
					var localI = i;
					var itemData = extraWheel.GetItemData(localI);
					var flyObject = machineContext.assetProvider.InstantiateGameObject("ActiveFly2", true);
					flyObject.SetActive(false);
					flyObject.SetActive(true);
					var sortingGroup = flyObject.GetComponent<SortingGroup>();
					sortingGroup.sortingLayerName = "LocalUI";
					sortingGroup.sortingOrder = 1;
					var flyObjectContainer = new GameObject("flyObjectContainer");
					flyObjectContainer.transform.SetParent(bottomWheel.transform,false);
					flyObjectContainer.transform.position = container.transform.position;
					flyObject.transform.SetParent(flyObjectContainer.transform,false);
					flyObject.transform.localPosition = Vector3.zero;
					AudioUtil.Instance.PlayAudioFx("LinkGame_J04Fly");
					machineContext.WaitSeconds(0.5f, () =>
					{
						AudioUtil.Instance.PlayAudioFx("LinkGame_J04FlyStop");
					});
					await XUtility.PlayAnimationAsync(flyObject.GetComponent<Animator>(),"ActiveFly1",machineContext);
					machineContext.assetProvider.RecycleGameObject("ActiveFly2", flyObject);
					GameObject.Destroy(flyObjectContainer);
					var elementContainer = extraWheel.UpdateRunningElement(id, localI, 0, false);
					extraWheel.wheelState.SetRollLockState(localI, true);
					extraWheel.GetRoll(localI).transform.gameObject.SetActive(false);
					extraWheel.GetStickyElement(localI).SetStickyData(itemData,true);
						
					// var flyEndAnimator = machineContext.assetProvider.InstantiateGameObject("Fx_GlowBlink", true);
					// flyEndAnimator.SetActive(false);
					// flyEndAnimator.SetActive(true);
					// flyEndAnimator.transform.SetParent(extraWheel.transform,false);
					// flyEndAnimator.transform.position = elementContainer.transform.position;
					// machineContext.WaitSeconds(2f, () =>
					// {
					// 	machineContext.assetProvider.RecycleGameObject("Fx_GlowBlink", flyEndAnimator);
					// });
				}
			}
			await machineContext.WaitSeconds(1.5f);
		}

		public async Task PerformExtraSpinTimes()
		{
			LinkBigPigGroupView11024 pigGroup;
			if (extraState.HasReSpinType(1))
			{
				pigGroup = machineContext.view.Get<LinkSmallPigGroupView11024>();
			}
			else
			{
				pigGroup = machineContext.view.Get<LinkBigPigGroupView11024>();
			}

			var startPosition = pigGroup.GetPigBoardPosition(2);
			var runningWheel = activeWheelState.GetRunningWheel();
			for (var i = 0; i < runningWheel.Count; i++)
			{
				var linkWheel = (WheelLink11024)runningWheel[i];
				var targetPosition = linkWheel.GetCountTextPosition();
				var flyObject = machineContext.assetProvider.InstantiateGameObject("Fly_link", true);
				flyObject.SetActive(false);
				flyObject.SetActive(true);
				flyObject.transform.SetParent(linkWheel.transform,false);
				flyObject.transform.position = startPosition;
				var targetLocalPosition = flyObject.transform.parent.InverseTransformPoint(targetPosition);
				AudioUtil.Instance.PlayAudioFx("LinkGame_ExtraSpinFly");
				flyObject.transform.DOLocalMove(targetLocalPosition,Constant11024.CollectCoinTime).OnComplete(() =>
				{
					linkWheel.RefreshLeftSpinTimes();
					machineContext.WaitSeconds(2f,()=>
					{
						machineContext.assetProvider.RecycleGameObject("Fly_link", flyObject);
					});
				});
			}
			await machineContext.WaitSeconds(Constant11024.CollectCoinTime + 1f);
		}
		public async Task PerformBeforeLinkStart()
		{
			if (extraState.HasReSpinType(1))
			{
				await GlowExtraWheel();//升盘面
				await machineContext.WaitSeconds(0.5f);
			}

			await ChangePigToGold();//变金
			await machineContext.WaitSeconds(0.5f);
			if (extraState.HasReSpinType(1))
			{
				await CopyGoldElement();//翻金币
			}
			await RaiseSpinTimes();
			if (extraState.HasReSpinType(2))
			{
				await PerformExtraSpinTimes();
			}
			// UpdateLinkWheelLockElements();
		}
		protected override async Task HandleLinkGame()
		{
			if (extraState.HasReSpinType(1) && !IsLinkSpinFinished() && !NeedSettle())
			{
				var linkWheels = activeWheelState.GetRunningWheel();
				for (var i = 0; i < linkWheels.Count; i++)
				{
					var linkWheel = (WheelLink11024) linkWheels[i];
					if (linkWheel.GetLeftSpinTimes() == 0)
					{
						linkWheel.ChangeToFeatureEnd();
					}
				}
			}
		}
		protected override bool IsLinkSpinFinished()
		{
			return !NextIsLinkSpin() && reSpinState.ReSpinNeedSettle && !extraState.HasCollectLink();
		}
		public override bool NeedSettle()
		{
			return reSpinState.ReSpinNeedSettle && extraState.HasCollectLink();
		}
		protected override async Task HandleLinkReward()
		{
			var runningWheelList = activeWheelState.GetRunningWheel();
			WheelLink11024 collectBoardWheel = (WheelLink11024)runningWheelList[runningWheelList.Count - 1];
			for (var i = 0; i < runningWheelList.Count; i++)
			{
				var linkWheel = (WheelLink11024)runningWheelList[i];
				linkWheel.ShowWinValueBoard(i == runningWheelList.Count-1);
			}
			if (!extraState.HasReSpinType(1))
			{
				machineContext.view.Get<LinkBigPigGroupView11024>().HidePigBoard();
			}
			for (var i = 0; i < runningWheelList.Count; i++)
			{
				var linkWheel = (WheelLink11024)runningWheelList[i];
				var grandWinRate = linkWheel.GetGrandValue();
				if (grandWinRate > 0)
				{
					var winValue = (long)betState.GetPayWinChips(grandWinRate);
					machineContext.view.Get<JackPotSmallPanel11024>().SetForceShowJpType(3);
					await machineContext.WaitSeconds(0.5f);
					await ShowJackpotPopup<UIJackpotBase11024>(3,winValue);
					collectBoardWheel.AddCollectValue(winValue);
					machineContext.view.Get<JackPotSmallPanel11024>().SetForceShowJpType(0);
				}
			}

			// float collectCoinTime = 0.5f;
			float collectInterval = 1f;
			await machineContext.WaitSeconds(1f);
			for (var i = 0; i < runningWheelList.Count; i++)
			{
				var linkWheel = (WheelLink11024)runningWheelList[i];
				var linkData = linkWheel.GetLinkData().Items;
				for (var i1 = 0; i1 < linkData.Count; i1++)
				{
					var itemData = linkData[i1];
					if (itemData.SymbolId > 0)
					{
						var winRate = itemData.WinRate + itemData.JackpotPay;
						var winValue = (long)betState.GetPayWinChips(winRate);
						var stickyElement = linkWheel.GetStickyElement((int)itemData.PositionId);
						stickyElement.ShowCollectAnimation();
						await machineContext.WaitSeconds(0.3f);
						var flyObject = machineContext.assetProvider.InstantiateGameObject("Fly_link",true);
						flyObject.SetActive(false);
						flyObject.SetActive(true);
						flyObject.transform.SetParent(linkWheel.transform,false);
						flyObject.transform.position = stickyElement.transform.position;
						var targetPosition = collectBoardWheel.GetCollectBoardPosition();
						var targetLocalPosition = flyObject.transform.parent.InverseTransformPoint(targetPosition);
						AudioUtil.Instance.PlayAudioFx("LinkGame_SettlementFly");
						flyObject.transform.DOLocalMove(targetLocalPosition, Constant11024.CollectCoinTime).OnComplete(() =>
						{
							AudioUtil.Instance.PlayAudioFx("LinkGame_SettlementFlyStop");
							collectBoardWheel.AddCollectValue(winValue);
							machineContext.WaitSeconds(2f,()=>
							{
								machineContext.assetProvider.RecycleGameObject("Fly_link", flyObject);
							});
						});
						if (itemData.JackpotPay > 0)
						{
							machineContext.view.Get<JackPotSmallPanel11024>().SetForceShowJpType((int)itemData.JackpotId);
							await machineContext.WaitSeconds(collectInterval);
							await ShowJackpotPopup<UIJackpotBase11024>((int)itemData.JackpotId,winValue);
							machineContext.view.Get<JackPotSmallPanel11024>().SetForceShowJpType(0);
						}
						else
						{
							await machineContext.WaitSeconds(collectInterval);	
						}
					}
				}
			}
			
			var waitTask = new TaskCompletionSource<bool>();
			bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
			machineContext.serviceProvider.GetSpinResult(machineContext.state.Get<BetState>().totalBet, isAutoSpin,machineContext,(sSpin) =>
			{
				extraState.UpdateStateOnReceiveSpinResult(sSpin);
				waitTask.SetResult(true);
			});
			await waitTask.Task;
		}

		public async Task AddValueToFloor()
		{
			// var runningWheelList = activeWheelState.GetRunningWheel();
			// WheelLink11024 collectBoardWheel = (WheelLink11024)runningWheelList[runningWheelList.Count - 1];
			// var winValue = collectBoardWheel.collectValue;
			var winState = machineContext.state.Get<WinState>();
			var winValue = winState.displayCurrentWin;
			var audioName = "Symbol_SmallWin_" + machineContext.assetProvider.AssetsId;
			var addWinValue = winValue - winState.currentWin;
			AddWinChipsToControlPanel((ulong)addWinValue,0.5f,true ,false,audioName);
			await machineContext.WaitSeconds(1f);
		}

		protected virtual async Task ShowJackpotPopup<T>(int jpType, long chips) where T : UIJackpotBase11024
		{
			string address = "UIJackpot" + Constant11024.JackpotName[jpType] + machineContext.assetProvider.AssetsId;

			if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
			{
				XDebug.LogError($"ShowJackmpotPopUp:{address} is Not Exist");
				return;
			}

			TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(waitTask, null);
			AudioUtil.Instance.PlayAudioFx("Jackpot");
			var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
			startPopUp.SetJackpotWinNum((ulong) chips);
			startPopUp.SetPopUpCloseAction(() =>
			{
				machineContext.RemoveTask(waitTask);
				waitTask.SetResult(true);
			});

			await waitTask.Task;
		}
		
		protected virtual async Task HandleLinkFinishPopup()
		{
			if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkFinishAddress()) != null)
			{
				var task = GetWaitTask();
				var finishLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinFinishPopUp11024>(GetLinkFinishAddress());
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
					}
				}
				await task.Task;
			}
			await Task.CompletedTask;
		}
		protected override async Task HandleLinkBeginPopup()
		{
			if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkBeginAddress()) != null)
			{
				var task = GetWaitTask();
				var startLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinStartPopUp11024>(GetLinkBeginAddress());
				if (startLinkPopup != null)
				{
					startLinkPopup.SetPopUpCloseAction(() =>
					{
						SetAndRemoveTask(task);
					});
					// if (startLinkPopup.IsAutoClose())
					// {
					// 	await machineContext.WaitSeconds(GetLinkBeginPopupDuration());
					// 	startLinkPopup.Close();     
					// }
				}
				else
				{
					SetAndRemoveTask(task);
				}

				await task.Task;
			}
			await Task.CompletedTask;
		}
	}
}