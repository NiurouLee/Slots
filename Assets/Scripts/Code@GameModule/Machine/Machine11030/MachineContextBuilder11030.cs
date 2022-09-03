using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11030 : MachineContextBuilder
	{
		public MachineContextBuilder11030(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11030();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return new SequenceElementConstructor11030(machineContext);
		}
		public override void SetUpCommonMachineState(MachineState machineState)
		{
			
			machineState.Add<BetState11030>();
			machineState.Add<AdStrategyState>();
			machineState.Add<FreeSpinState11030>();
			machineState.Add<AutoSpinState11030>();
			machineState.Add<ReSpinState11030>();
			machineState.Add<JackpotInfoState11030>();
			machineState.Add<WinState11030>();
			
			SetUpWheelActiveState(machineState);
		}
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11030>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			TrainCabin11030.CleanCabinPool();
			machineState.Add<WheelState11030>();
			machineState.Add<WheelState11030>();
			machineState.Add<WheelState11030>();
			machineState.Add<ExtraState11030>();
		}
		public override void BindingWheelView(MachineContext machineContext)
		{
			var baseWheelTransform = machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame");
			var baseWheel = machineContext.view.Add<WheelTrain11030>(baseWheelTransform);
			baseWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11030>>(machineContext.state.Get<WheelState>());
			baseWheel.SetUpWinLineAnimationController<WinLineAnimationController11030>();

			var wheelFreeTransform = machineContext.transform.Find("WheelFeature/Wheels/WheelFreeGame");
			var wheelFree = machineContext.view.Add<WheelTrain11030>(wheelFreeTransform);
			wheelFree.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController11030>>(machineContext.state.Get<WheelState>(1));
			wheelFree.SetUpWinLineAnimationController<WinLineAnimationController11030>();
			
			var wheelLinkTransform = machineContext.transform.Find("WheelFeature/Wheels/WheelLineFeatureGame");
			var wheelLink = machineContext.view.Add<WheelTrain11030>(wheelLinkTransform);
			wheelLink.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController11030>>(machineContext.state.Get<WheelState>(2));
			wheelLink.SetUpWinLineAnimationController<WinLineAnimationController11030>();
		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotView = machineContext.transform.Find("WheelFeature/JackpotPanel");
			machineContext.view.Add<JackPotPanel11030>(jackpotView);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			var trainView = machineContext.transform.Find("WheelFeature/Wheels/WheelTrainNormal");
			var trainViewInstance = machineContext.view.Add<TrainView11030>(trainView);
			trainViewInstance.InitAfterBindingContext();
			var trainGoldView = machineContext.transform.Find("WheelFeature/Wheels/WheelTrainGold");
			var trainGoldViewInstance = machineContext.view.Add<TrainGoldView11030>(trainGoldView);
			trainGoldViewInstance.InitAfterBindingContext();
			base.BindingExtraView(machineContext);
		}
		public override void BindingBackgroundView(MachineContext machineContext)
		{
			var background = machineContext.transform.Find("Background");
			machineContext.view.Add<BackgroundView11030>(background);
		}
		public override void AdaptMachineView(MachineContext machineContext)
		{
			if (!Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
			{
				float x = Mathf.Clamp(ViewResolution.referenceResolutionLandscape.x, 1024, ViewResolution.designSize.x);
				float lerp = (x - 1024f) / (ViewResolution.designSize.x - 1024f);
				Vector2 padScaler = GetPadMinXScaler();
				float lerpScalerX = Mathf.Lerp(padScaler.x, 1, lerp);
				float lerpScalerY = Mathf.Lerp(padScaler.y, 1, lerp);

				var wheels = machineContext.transform.GetComponentInChildren<SimpleRollUpdaterEasingScriptable>()?.transform;
				Vector3 lerpScalerVec = new Vector3(lerpScalerX, lerpScalerY, 1);

				if (wheels == null)
				{
					wheels = machineContext.transform.Find("Wheels");
				}
				if (wheels == null)
				{
					wheels = machineContext.transform.Find("ZhenpingAnim/Wheels");
				}

				if (wheels == null)
				{
					wheels = machineContext.transform.Find("WheelFeature/Wheels");
				}
				if (wheels == null)
				{
					return;
				}
				var tranJackpotPanel = wheels.transform.parent.Find("JackpotPanel");
				if (tranJackpotPanel == null)
				{
					tranJackpotPanel = wheels.transform.parent.Find("ZhenpingAnim/JackpotPanel");
				}

				Transform parentJackpot = null;
				if (tranJackpotPanel != null)
				{
					parentJackpot = tranJackpotPanel.parent;
					tranJackpotPanel.parent = wheels;
				}

				wheels.localScale = lerpScalerVec;

				if (tranJackpotPanel != null)
				{
					tranJackpotPanel.parent = parentJackpot;
				}
			}
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11030);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11030);
			logicProxy[LogicStepType.STEP_ROUND_START] = typeof(RoundStartProxy11030);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11030);
			logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11030);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11030);
			logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11030);
			logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11030);
			logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11030);
			logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11030);
			logicProxy[LogicStepType.STEP_LATE_HIGH_LEVEL_WIN_EFFECT] = typeof(LateHighLevelWinEffectProxy11030);
			logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11030);
			logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(ReSpinLogicProxy11030);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11030);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11030);
			logicProxy[LogicStepType.STEP_ROUND_FINISH] = typeof(RoundFinishProxy11030);
			logicProxy[LogicStepType.STEP_BONUS_WIN_NUM_INTERRUPT] = typeof(BonusWinNumInterruptProxy11030);
			return logicProxy;
		}
	}
}