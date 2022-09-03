using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11025 : MachineContextBuilder
	{
		public MachineContextBuilder11025(string inMachineId)
		:base(inMachineId)
		{
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11025();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return new SequenceElementConstructor11025(machineContext);
		}
		public override void SetUpCommonMachineState(MachineState machineState)
		{
			
			machineState.Add<BetState>();
			machineState.Add<AdStrategyState>();
			machineState.Add<FreeSpinState>();
			machineState.Add<AutoSpinState>();
			machineState.Add<ReSpinState>();
			machineState.Add<JackpotInfoState>();
			machineState.Add<WinState>();
			SetUpWheelActiveState(machineState);
		}
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11025>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			machineState.Add<ExtraState11025>();
			machineState.Add<WheelState11025>();
			machineState.Add<WheelState11025>();
		}

		public override MachineConfig SetUpMachineConfig(MachineContext machineContext)
		{
			return base.SetUpMachineConfig(machineContext);
		}

		public override void AdaptMachineView(MachineContext machineContext)
		{
			var wheels = machineContext.transform.Find("Wheels");

			var uiTotalHeight = 1150;
			var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;

			layoutHelper = new VerticalMachineLayoutHelper();

			layoutHelper.AddElement(wheels,uiTotalHeight, 0.05f);

			layoutHelper.DoLayout();

			var storeBackGround = machineContext.transform.Find("Background/StoreBg");
			var storeNode = machineContext.transform.Find("Wheels/WheelStoreGame/Root");
			storeBackGround.SetParent(storeNode,true);
			storeBackGround.gameObject.SetActive(true);

			float normalScale = 16f / 9;
			float longScale = 19f / 9;
			var heightWidthScale = ViewResolution.referenceResolutionPortrait.y /
			                       ViewResolution.referenceResolutionPortrait.x;
			if (heightWidthScale >= longScale)
			{
				var tempLocalPosition = wheels.localPosition;
				tempLocalPosition.y = -5;
				wheels.localPosition = tempLocalPosition;
			}
		}
		public override void BindingWheelView(MachineContext machineContext)
		{
			FrameData11025.SetContext(machineContext);
			List<int> rowCountInEachRoll = Constant11025.RollHeightList;
			List<Vector3> roolPositions = new List<Vector3> { };
			var elementSize = new Vector2(1.35f, 1.00f);
			roolPositions = IrregularWheel.GetIrregularRollPosition(5, rowCountInEachRoll,elementSize, IrregularWheel.ElementAlignType.AlignBottom);
			
			var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
			var baseWheel = machineContext.view.Add<WheelBase11025>(baseWheelTransform);
			baseWheel.SetWheelExtraConfig(rowCountInEachRoll, roolPositions,elementSize);
			baseWheel.BuildWheel<SpecialContainerRoll<FlowerElementContainer11025>, ElementSupplier, WheelSpinningController<WheelAnimationController11025>>(machineContext.state.Get<WheelState>());
			baseWheel.SetUpStickyNode();
			baseWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var wheelFreeTransform = machineContext.transform.Find("Wheels/WheelFreeGame");
			var wheelFree = machineContext.view.Add<WheelFree11025>(wheelFreeTransform);
			wheelFree.SetWheelExtraConfig(rowCountInEachRoll, roolPositions,elementSize);
			wheelFree.BuildWheel<SpecialContainerRoll<FlowerElementContainer11025>, ElementSupplier, WheelSpinningController<WheelAnimationController11025>>(machineContext.state.Get<WheelState>(1));
			wheelFree.SetUpStickyNode();
			wheelFree.SetUpWinLineAnimationController<WinLineAnimationController>();
		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotView = machineContext.transform.Find("Wheels/JackpotPanel");
			machineContext.view.Add<JackPotPanel11025>(jackpotView);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			var shopView = machineContext.transform.Find("Wheels/WheelStoreGame");
			var shopViewInstance = machineContext.view.Add<ShopView11025>(shopView);
			shopViewInstance.InitAfterBindingContext();
			
			var freeMultiWheel = machineContext.transform.Find("Wheels/WheelFreeGame/Turntable");
			var freeMultiWheelInstance = machineContext.view.Add<FreeMultiWheel11025>(freeMultiWheel);
			freeMultiWheelInstance.InitAfterBindingContext();
		}
		public override void BindingBackgroundView(MachineContext machineContext)
		{
			var background = machineContext.transform.Find("Background");
			machineContext.view.Add<BackgroundView11025>(background);
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11025);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11025);
			logicProxy[LogicStepType.STEP_ROUND_START] = typeof(RoundStartProxy11025);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11025);
			logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11025);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11025);
			// logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11025);
			// logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11025);
			// logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11025);
			// logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11025);
			// logicProxy[LogicStepType.STEP_LATE_HIGH_LEVEL_WIN_EFFECT] = typeof(LateHighLevelWinEffectProxy11025);
			// logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11025);
			// logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(ReSpinLogicProxy11025);
			// logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11025);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11025);
			// logicProxy[LogicStepType.STEP_ROUND_FINISH] = typeof(RoundFinishProxy11025);
			return logicProxy;
		}
	}
}