using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Rendering;
using GameModule;

namespace GameModule
{
	public class LinkLogicProxy11026 : LinkLogicProxy
	{
		private WheelsActiveState11026 wheelsActiveState;
		private LinkLockView11026 linkLockView;
        ElementConfigSet elementConfigSet = null;
		public LinkLogicProxy11026(MachineContext context)
			: base(context)
		{
			wheelsActiveState = machineContext.state.Get<WheelsActiveState11026>();
			linkLockView = machineContext.view.Get<LinkLockView11026>();
			elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
		}

		protected override async void HandleCustomLogic()
		{
			//处理触发Link：开始弹板或者过场动画
			if (IsLinkTriggered())
			{
				StopBackgroundMusic();
				
				machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
				
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
				StopBackgroundMusic();
				await HandleLinkReward();
			}
            //Link结算完成，恢复Normal
			if (NeedSettle())
			{
				StopBackgroundMusic();
				await machineContext.state.Get<ReSpinState>().SettleReSpin();
				await HandleLinkFinishCutSceneAnimation();
				await HandleLinkHighLevelEffect();
			}
			Proceed();
		}

		protected override async Task ShowReSpinTriggerLineAnimation()
		{
			var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
			//retrigger时先清除覆盖在scatter上的wild
			var wheel = machineContext.state.Get<WheelsActiveState11026>().GetRunningWheel()[0];
			var extraState = machineContext.state.Get<ExtraState11026>();
			var listWildPos = extraState.GetFreeStickyWildIds();
			var listRandomWildPos = extraState.GetFreeRandomWildIds();
			if (listWildPos.Count > 0 || listRandomWildPos.Count >0)
			{
				var reSpinWinLine = wheel.wheelState.GetReSpinWinLine();
				for (int i = 0; i < reSpinWinLine.Count; i++)
				{
					for (var index = 0; index < reSpinWinLine[i].Positions.Count; index++)
					{
						var pos = reSpinWinLine[i].Positions[index];
						var container =  wheel.GetWinLineElementContainer((int) pos.X,(int) pos.Y);
						var rollIndex = (int) pos.X;
						var rowIndex = (int) pos.Y;
						uint id = machineContext.state.Get<WheelState11026>().GetElementId(rollIndex,rowIndex);
						var elementConfig = elementConfigSet.GetElementConfig(id);
						container.UpdateElement(new SequenceElement(elementConfig,machineContext));
					}
				}
			}
			if (wheels.Count > 0)
					await wheels[0].winLineAnimationController.BlinkReSpinLine();
		}

		protected void ResetLinkLockState()
		{
			var states = machineContext.state.GetAll<WheelState>();
			for (var i = 0; i < states.Count; i++)
			{
				if (states[i] is WheelStateLeft11026 left11026)
				{
					left11026.ResetLinkWheelState();
				}

				else if (states[i] is WheelStateCenter11026 center11026)
				{
					center11026.ResetLinkWheelState();
				}

				else if (states[i] is WheelStateRight11026 right11026)
				{
					right11026.ResetLinkWheelState();
				}
			}
		}

		protected override async Task HandleLinkBeginCutSceneAnimation()
		{
			machineContext.view.Get<TransitionsView11026>().PlayLinkTransition();
			await machineContext.WaitSeconds(2.3f);
			machineContext.view.Get<TransitionsView11026>().HideLink();
			wheelsActiveState.UpdateRunningWheelState(true, false, true);

			machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
			
			ResetLinkLockState();
			
			RefreshLinkWheelInitElement(false);
			
			Animator animatorWheel = machineContext.view.Get<Wheel>("WheelLinkGame1").transform
							.GetComponent<Animator>();
			animatorWheel.Play("Idle");
			//刷新剩余次数
			machineContext.view.Get<LinkLockView11026>().ShowRotationTips();
			machineContext.view.Get<LinkLockView11026>().ShowReSpinCount();
			//显示右边行数
			machineContext.view.Get<LinkRowMore11026>().ShowRowsMore();
			machineContext.view.Get<LinkRowMore11026>().PlayBeginIdle();
			//显示左边倍数
			machineContext.view.Get<LinkMultiplier11026>().ShowAllWins();
			machineContext.view.Get<LinkMultiplier11026>().PlayBeginIdle();
			//更换link背景
			machineContext.view.Get<BackGroundView11026>().ShowLinkBackGround();
			await machineContext.WaitSeconds(3.0f - 2.3f);
			machineContext.view.Get<TransitionsView11026>().HideDragon();
			await machineContext.WaitSeconds(0.5f);
			//link转场弹窗
			AudioUtil.Instance.PlayAudioFx("LinkGameStart_Open");
			machineContext.view.Get<FeatureGameTips11026>().FeatureTipShow();
			await machineContext.WaitSeconds(3.0f);
			machineContext.view.Get<FeatureGameTips11026>().FeatureTipHide();
			await machineContext.WaitSeconds(0.6f);
		}

		protected override void RecoverLogicState()
		{
			machineContext.view.Get<TransitionsView11026>().HideDragon();
			ResetLinkLockState();
			UpdateLinkCenterWheelLockElements(true);
			machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
			machineContext.view.Get<BackGroundView11026>().ShowLinkBackGround();
			uint rowNumber = machineContext.state.Get<ExtraState11026>().GetRowsMore();
			wheelsActiveState.UpdateRunningWheel(Constant11026.GetListLink(rowNumber), false);
			HandleRecoverWheelState();
			RefreshLinkWheelInitElement(false);
			
			//显示左边倍数
			var view = machineContext.view.Get<LinkMultiplier11026>();
			view.ShowAllWinsImage();
			view.PLayAllWinsIdle();
			
			//显示右边行数
			var moreView = machineContext.view.Get<LinkRowMore11026>();
			moreView.ShowAllWinsImage();
			moreView.PLayRowMoreIdle();
			//刷新剩余次数
			// machineContext.view.Get<LinkLockView11026>().ShowReSpinCount();
		}

		protected override async Task HandleLinkFinishCutSceneAnimation()
		{
			machineContext.view.Get<LinkLockView11026>().Reset();
			var transitionsView = machineContext.view.Get<TransitionsView11026>();
			transitionsView.PlayFinishLinkTransition();
			await machineContext.WaitSeconds(2.3f);
			ResetText();
			machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
			bool isFree = machineContext.state.Get<FreeSpinState>().IsInFreeSpin;
			wheelsActiveState.UpdateRunningWheelState(false, isFree);
			if (isFree)
			{
				if (machineContext.state.Get<ExtraState11026>().GetIsSuper())
				{
					machineContext.view.Get<LockElementLayer11026>().ShowStickyWildElement();
				}
			}
			var superFreeProgress = machineContext.view.Get<SuperFreeProgressView11026>();
			superFreeProgress.UpdateProgress();
			superFreeProgress.LockSuperFreeIdle(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
			transitionsView.ShowDragon();
			await machineContext.WaitSeconds(3.0f - 2.3f);
			transitionsView.PlayIdleransition();
			if (isFree)
			{
				machineContext.view.Get<BackGroundView11026>().ShowFreeBackGround();
			}
			else
			{
				machineContext.view.Get<BackGroundView11026>().ShowBaseBackFround();
			}
		}

		private void  HandleRecoverWheelState()
		{
			uint rowNumber = machineContext.state.Get<ExtraState11026>().GetRowsMore();
			Animator animatorWheel = machineContext.view.Get<Wheel>("WheelLinkGame1").transform
				.GetComponent<Animator>();
			if (rowNumber > 0)
			{
				animatorWheel.Play($"LinkGane{rowNumber}Idle");
			}
			else
			{
				animatorWheel.Play("Idle");
			}
		}

		protected async override Task HandleLinkGame()
		{
			bool isFromMachineSetup = IsFromMachineSetup();
		
			if (!isFromMachineSetup)
			{
				await ShowMultiplierFly();
				await ShowShowNewAddRow();
			}

			UpdateLinkCenterWheelLockElements(isFromMachineSetup);
		}

		private void UpdateLinkCenterWheelLockElements(bool isSetupRoom, bool updateOnlyCenter = false)
		{
			var listWheels = wheelsActiveState.GetRunningWheel();

			for (var i = 0; i < listWheels.Count; i++)
			{
				List<int> newLinkItem = null;
				if (!updateOnlyCenter && listWheels[i].wheelState is WheelStateLeft11026 linkLeftWheelState)
				{
					newLinkItem = linkLeftWheelState.GetNewAppearLinkItemIndex();
				}
				else if (!updateOnlyCenter && listWheels[i].wheelState is WheelStateCenter11026 linkCenterWheelState)
				{
					newLinkItem = linkCenterWheelState.GetNewAppearLinkItemIndex();
				}
				else if (listWheels[i].wheelState is WheelStateRight11026 linkRightWheelState)
				{
					newLinkItem = linkRightWheelState.GetNewAppearLinkItemIndex();
				}

				if (newLinkItem != null)
				{
					for (var c = 0; c < newLinkItem.Count; c++)
					{
						listWheels[i].wheelState.SetRollLockState(newLinkItem[c], true);
						if (!isSetupRoom)
						{
							var container = listWheels[i].GetRoll(newLinkItem[c]).GetVisibleContainer(0);
						//	var sortingGroup = container.transform.GetComponent<SortingGroup>();
						//	sortingGroup.sortingOrder = 9999 + newLinkItem[c];
							container.ShiftSortOrder(true);
							// container.PlayElementAnimation("Idle");
						}
					}
				}
			}
		}
		
		protected async Task ShowShowNewAddRow()
		{
			var wheel = machineContext.state.Get<WheelsActiveState11026>().GetRunningWheel()[2];

			var wheelStateRight11026 = (WheelStateRight11026) wheel.wheelState;

			var listNewItemIndex = wheelStateRight11026.GetNewAppearLinkItemIndex();

			long rowNumber = machineContext.state.Get<ExtraState11026>().GetRowsMore();
			var oldRowNumber = rowNumber - listNewItemIndex.Count;
			
			Animator animatorWheel = machineContext.view.Get<Wheel>("WheelLinkGame1").transform
				.GetComponent<Animator>();
			
			for (var i = 0; i < listNewItemIndex.Count; i++)
			{ 
				//轮盘上升
				XUtility.PlayAnimation(animatorWheel, (oldRowNumber + i+1).ToString());
				
				var container = wheel.GetRoll(listNewItemIndex[i]).GetVisibleContainer(0);
				
				var endPos = machineContext.view.Get<LinkAnimationNode11026>().GetIntegralAnimationNodePos(oldRowNumber + i);
                AudioUtil.Instance.PlayAudioFx("J01Reel_Rise");
				//0.4秒后开始拖尾
				await machineContext.WaitSeconds(0.4f);

				//粉色拖尾，拖尾时间0.5秒
				var objFly = machineContext.assetProvider.InstantiateGameObject("J01_Row", true);
				objFly.transform.parent = machineContext.transform;
				var startPos = container.transform.position;
				objFly.transform.position = startPos;
				XUtility.Fly(objFly.transform, startPos, endPos, 0, 0.5f, null);
				AudioUtil.Instance.PlayAudioFx("J01Multiplier_Fly");
				await machineContext.WaitSeconds(0.5f);
				//爆炸动画
				AudioUtil.Instance.PlayAudioFx("J01Reel_Unlock");
				await machineContext.WaitSeconds(2.33f - 1.2f);
				machineContext.assetProvider.RecycleGameObject("J01_Row", objFly);
				//更新轮盘
				machineContext.state.Get<WheelsActiveState11026>()
					.UpdateRunningLinkWheel(Constant11026.GetListLink(oldRowNumber + i + 1));
				RefreshLinkWheelInitElement(true);
				UpdateLinkCenterWheelLockElements(false, true);
				wheelStateRight11026.SetRollLockState(listNewItemIndex[i], true);
				container.UpdateElementMaskInteraction(true);
				//更换数字
				AudioUtil.Instance.PlayAudioFx("J01Multiplier_Fresh");
				machineContext.view.Get<LinkRowMore11026>().ShowAllWinsImage(oldRowNumber + i + 1);
				machineContext.view.Get<LinkRowMore11026>().PlayRowMore(oldRowNumber + i + 1);
				await machineContext.WaitSeconds(1f);
				machineContext.view.Get<LinkRowMore11026>().ShowRowMoreFinishImage(oldRowNumber + i + 1);
			}
		}

		protected async Task ShowMultiplierFly()
		{
			var wheel = machineContext.state.Get<WheelsActiveState11026>().GetRunningWheel()[0];

			var wheelStateLeft11026 = (WheelStateLeft11026) wheel.wheelState;

			var listNewItemIndex = wheelStateLeft11026.GetNewAppearLinkItemIndex();
			
			var endPos = machineContext.view.Get<LinkMultiplier11026>().GetIntegralPos();

			var multiplier = machineContext.state.Get<ExtraState11026>().GetAllWinMultiplier();
			var oldMultiplier = multiplier - listNewItemIndex.Count;
			
			for (var i = 0; i < listNewItemIndex.Count; i++)
			{
				var container = wheel.GetRoll(listNewItemIndex[i]).GetVisibleContainer(0);
				
				wheelStateLeft11026.SetRollLockState(listNewItemIndex[i], true); 
				var sortingGroup = container.transform.GetComponent<SortingGroup>();
				sortingGroup.sortingOrder = 10001 + listNewItemIndex[i];;
				container.UpdateElementMaskInteraction(true);
				
				var objFly =
					machineContext.assetProvider.InstantiateGameObject("ep_Active_J01_Trail", true);
				objFly.transform.parent = machineContext.transform;
				var startPos = container.transform.position;
				objFly.transform.position = startPos;
				AudioUtil.Instance.PlayAudioFx("J01Multiplier_Fly");
				await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.3f, Ease.Linear,
					machineContext);
				machineContext.view.Get<LinkMultiplier11026>().ShowAllWinsImage(oldMultiplier + i + 1); 
				AudioUtil.Instance.PlayAudioFx("J01Multiplier_Fresh");
				machineContext.view.Get<LinkMultiplier11026>().PLayAllWins(oldMultiplier + i + 1);
				await machineContext.WaitSeconds(1.67f);
				machineContext.view.Get<LinkMultiplier11026>().ShowAllWinsFinishImage(oldMultiplier + i + 1);
				machineContext.assetProvider.RecycleGameObject("ep_Active_J01_Trail", objFly);
			}
		}

		protected override async  Task HandleLinkReward()
        {
	       // if (!IsFromMachineSetup())
	       {
		       uint multiplierNumber = machineContext.state.Get<ExtraState11026>().GetAllWinMultiplier();
		       if (multiplierNumber > 1)
		       {
			       machineContext.view.Get<LinkMultiplierFinish11026>().HideMultiplierImage();
			       machineContext.view.Get<LinkMultiplier11026>().HideAllWins();
			       machineContext.view.Get<LinkRowMore11026>().HideRowsMore();
			       machineContext.view.Get<LinkLockView11026>().HideRotationTips();
			       await AllWins();
			       await HandleJ01Refresh();
		       }
		       else
		       {
			       machineContext.view.Get<LinkMultiplierFinish11026>().HideMultiplierImage();
			       machineContext.view.Get<LinkMultiplier11026>().HideAllWins();
			       machineContext.view.Get<LinkRowMore11026>().HideRowsMore();
			       machineContext.view.Get<LinkLockView11026>().HideRotationTips();
		       }
		       await BonusFly();
	       }
        }
		
		
        //1.75s
		private async Task AllWins()
		{
			machineContext.view.Get<LinkMultiplierFinish11026>().setMultiplierImage();
			await machineContext.view.Get<LinkMultiplierFinish11026>().PLayAllWinsFisnish();
		}

		public async Task BonusFly()
		{
			var wheel = wheelsActiveState.GetRunningWheel()[1];
			var endPos = machineContext.view.Get<ControlPanel>().GetWinTextRefWorldPosition(Vector3.zero);
			int rollCount = wheel.rollCount;
			uint rowNumber = machineContext.state.Get<ExtraState11026>().GetRowsMore();
			uint multiplierNumber = machineContext.state.Get<ExtraState11026>().GetAllWinMultiplier();
			
			for (int x = 0; x < rollCount; x++)
			{
				var container = wheel.GetRoll(x).GetVisibleContainer(0);
			
				if (Constant11026.ListBonusTextElementIds.Contains(container.sequenceElement.config.id) ||
				    Constant11026.ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
				{
					container.PlayElementAnimation("Settlement");
					ulong chips = 0;
					var items = machineContext.state.Get<ExtraState11026>().GetCenterLinkData().Items;
					for (int q = items.count - 1; q >= 0; q--)
					{
						var item = items[q];
						if ((item.PositionId % 7) < (3 - rowNumber))
						{
							items.Remove(item);
						}
					}

					if (Constant11026.ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
					{
						chips = machineContext.state.Get<BetState>().GetPayWinChips(items[x].JackpotPay);
						await Constant11026.ShowJackpot(machineContext, this, items[x].JackpotId, items[x].JackpotPay);
					}

					if (Constant11026.ListBonusTextElementIds.Contains(container.sequenceElement.config.id))
					{
						chips = machineContext.state.Get<BetState>().GetPayWinChips(items[x].WinRate);
					}

					var objFly = machineContext.assetProvider.InstantiateGameObject("ep_Active_J01_Trail", true);
					objFly.transform.parent = machineContext.transform;
					var startPos = container.transform.position;
					objFly.transform.position = startPos;
					XUtility.Fly(objFly.transform, startPos, endPos, 0, 0.5f, null);
					AudioUtil.Instance.PlayAudioFx("J01_Settlement");
					await machineContext.WaitSeconds(0.4f);
					AudioUtil.Instance.PlayAudioFx("J01_Settlement2");
					var objFinish = machineContext.assetProvider.InstantiateGameObject("ep_Active_J01_Hit", true);
					objFinish.transform.parent = machineContext.transform;
					objFinish.transform.position = endPos;
					await XUtility.FlyAsync(objFinish.transform, endPos, endPos, 0, 0.5f, Ease.Linear, machineContext);
					AddWinChipsToControlPanel(chips * multiplierNumber, 0.5f, false, false);
					machineContext.assetProvider.RecycleGameObject("ep_Active_J01_Hit", objFinish);

					machineContext.assetProvider.RecycleGameObject("ep_Active_J01_Trail", objFly);
					await machineContext.WaitSeconds(0.5f);
				}
			}
		}

		public void RefreshText()
        {
	        var wheel = wheelsActiveState.GetRunningWheel()[1];
	        uint rowNumber = machineContext.state.Get<ExtraState11026>().GetRowsMore();
	        uint multiplierNumber = machineContext.state.Get<ExtraState11026>().GetAllWinMultiplier();

	        var matchFilterList = wheel.GetElementMatchFilter((container) =>
	        {
		        if (Constant11026.ListBonusElementIds.Contains(container.sequenceElement.config.id) ||
		            Constant11026.ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
		        {
			        return true;
		        }

		        return false;
	        });

	        for (int x = 0; x < matchFilterList.Count; x++)
	        {
		        var container = matchFilterList[x];

		        var items = machineContext.state.Get<ExtraState11026>().GetCenterLinkData().Items;
		        for (int q = items.count - 1; q >= 0; q--)
		        {
			        var item = items[q];
			        if ((item.PositionId % 7) < (3 - rowNumber))
			        {
				        items.Remove(item);
			        }
		        }

		        if (Constant11026.ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
		        {
			        if (multiplierNumber > 1)
			        {
				        var animatorRoot = container.transform.GetChild(0).Find("AnimRoot");
				        animatorRoot.Find("Sprite").gameObject.SetActive(false);
				        animatorRoot.Find("Sprite1").gameObject.SetActive(true);
				        animatorRoot.Find("Sprite1").Find("3x").gameObject.SetActive(multiplierNumber == 3);
				        animatorRoot.Find("Sprite1").Find("4x").gameObject.SetActive(multiplierNumber == 4);
				        animatorRoot.Find("Sprite1").Find("2x").gameObject.SetActive(multiplierNumber == 2);
			        }
		        }

		        if (Constant11026.ListBonusElementIds.Contains(container.sequenceElement.config.id))
		        {
			        if (container.GetElement() is ElementCoin11026 element)
			        {
				        element.UpdateWinChipsWithMultiplier(multiplierNumber);
			        }
		        }
	        }
        }

		private async Task HandleJ01Refresh()
        {
	        var wheel = machineContext.state.Get<WheelsActiveState11026>().GetRunningWheel()[1];
	      
	        wheel.GetElementMatchFilter((container) =>
	        {
		        var elementId = container.sequenceElement.config.id;
		       
		        if (Constant11026.ListBonusElementIds.Contains(elementId) ||
		            Constant11026.ListBonusJackpotElementIds.Contains(elementId))
		        {
			        container.PlayElementAnimation("Refresh");
			        return true;
		        }
		        return false;
	        });
	        AudioUtil.Instance.PlayAudioFx("J01_increase");
	        await machineContext.WaitSeconds(0.13f);
	        RefreshText();
	        await machineContext.WaitSeconds(1.0f - 0.13f);
        }
		
		protected void RefreshLinkWheelInitElement(bool onlyMiddle)
		{
			var runningWheels = wheelsActiveState.GetRunningWheel();
			
			if (onlyMiddle)
			{
				((LinkWheel11206) runningWheels[1]).RefreshWheelInitElement();
			}
			else
			{
				for (var i = 0; i < runningWheels.Count; i++)
				{
					if (runningWheels[i] is LinkWheel11206 wheel)
					{
						wheel.RefreshWheelInitElement();
					}
				}
			}
		}

		public void ResetText()
		{
			var wheel = machineContext.state.Get<WheelsActiveState11026>().GetRunningWheel()[1];
			uint rowNumber = machineContext.state.Get<ExtraState11026>().GetRowsMore();
			var matchFilterList = wheel.GetElementMatchFilter((container) =>
			{
				if (Constant11026.ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
				{
					return true;
				}

				return false;
			});
			for (int x = 0; x < matchFilterList.Count; x++)
			{
				var container = matchFilterList[x];

				var items = machineContext.state.Get<ExtraState11026>().GetCenterLinkData().Items;
				for (int q = items.count - 1; q >= 0; q--)
				{
					var item = items[q];
					if ((item.PositionId % 7) < (3 - rowNumber))
					{
						items.Remove(item);
					}
				}
				if (Constant11026.ListBonusJackpotElementIds.Contains(container.sequenceElement.config.id))
				{
					var animatorRoot = container.transform.GetChild(0).Find("AnimRoot");
					animatorRoot.Find("Sprite").gameObject.SetActive(true);
					animatorRoot.Find("Sprite1").gameObject.SetActive(false);
					animatorRoot.Find("Sprite1").Find("3x").gameObject.SetActive(false);
					animatorRoot.Find("Sprite1").Find("4x").gameObject.SetActive(false);
					animatorRoot.Find("Sprite1").Find("2x").gameObject.SetActive(false);
				}
			}
		}
	}
}