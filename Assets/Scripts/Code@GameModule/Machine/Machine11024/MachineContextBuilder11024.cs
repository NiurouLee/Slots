using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11024 : MachineContextBuilder
	{
		public MachineContextBuilder11024(string inMachineId)
		:base(inMachineId)
		{
	
	
		}
	
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11024();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return new SequenceElementConstructor11024(machineContext);
		}
		public override void SetUpCommonMachineState(MachineState machineState)
		{
			machineState.Add<BetState11024>();
			machineState.Add<AdStrategyState11024>();
			machineState.Add<FreeSpinState11024>();
			machineState.Add<AutoSpinState11024>();
			machineState.Add<ReSpinState11024>();
			machineState.Add<JackpotInfoState11024>();
			machineState.Add<WinState11024>();
			SetUpWheelActiveState(machineState);
		}
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11024>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			machineState.Add<ExtraState11024>();
			machineState.Add<WheelState11024>();
			for (var i = 0; i < 3; i++)
			{
				machineState.Add<WheelStateFree11024>();
			}
	
			for (var i = 0; i < 3; i++)
			{
				machineState.Add<WheelStateLink11024>();	
			}
		}
	
		public override void AdaptMachineView(MachineContext machineContext)
		{
			var wheels = machineContext.transform.Find("Wheels");
	
			var uiTotalHeight = 1072;
			var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;
	
			layoutHelper = new VerticalMachineLayoutHelper();
	
			layoutHelper.AddElement(wheels,uiTotalHeight, 0.464f,0.2f,0);
	
			var storeBg = machineContext.transform.Find("Wheels/MapGame/Root/MapGameBg/MapGameBg4");
			storeBg.SetParent(machineContext.transform,true);
			layoutHelper.DoLayout();
			storeBg.SetParent(machineContext.transform.Find("Wheels/MapGame/Root/MapGameBg"), true);

			if (ViewResolution.referenceResolutionPortrait.y > 1400)
			{
				machineContext.view.Get<BackgroundView11024>().ShowLogo(true);
			}
			else
			{
				machineContext.view.Get<BackgroundView11024>().ShowLogo(false);
			}
		}
		public override void BindingWheelView(MachineContext machineContext)
		{
			var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
			var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
			baseWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11024>>(machineContext.state.Get<WheelState11024>());
			baseWheel.SetUpWinLineAnimationController<WinLineAnimationController11024>();
	
			for (var i = 1; i <= 3; i++)
			{
				var wheelFreeTransform = machineContext.transform.Find("Wheels/WheelFreeGame" + i);
				var wheelFree = machineContext.view.Add<WheelFree11024>(wheelFreeTransform);
				wheelFree.FreeWheelIndex = i-1;
				wheelFree.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11024>>(machineContext.state.Get<WheelStateFree11024>(i-1));
				wheelFree.SetUpWinLineAnimationController<WinLineAnimationController11024>();	
			}
	
			{
				var wheelLinkTransform = machineContext.transform.Find("Wheels/WheelLinkGame1");
				var wheelLink = machineContext.view.Add<WheelLink11024>(wheelLinkTransform);
				wheelLink.linkWheelIndex = 0;
				wheelLink
					.BuildWheel<SoloRoll11024, ElementSupplier,
						IndependentSpinningController11024<IndependentWheelAnimationController11024>>(machineContext.state
						.Get<WheelStateLink11024>(0));
				wheelLink.SetUpWinLineAnimationController<WinLineAnimationController11024>();
			}
			{
				var wheelLinkTransform = machineContext.transform.Find("Wheels/WheelLinkGame2_1");
				var wheelLink = machineContext.view.Add<WheelLink11024>(wheelLinkTransform);
				wheelLink.linkWheelIndex = 0;
				wheelLink
					.BuildWheel<SoloRoll11024, ElementSupplier,
						IndependentSpinningController11024<IndependentWheelAnimationController11024>>(machineContext.state
						.Get<WheelStateLink11024>(1));
				wheelLink.SetUpWinLineAnimationController<WinLineAnimationController11024>();
			}
			{
				var wheelLinkTransform = machineContext.transform.Find("Wheels/WheelLinkGame2_2");
				var wheelLink = machineContext.view.Add<WheelLink11024>(wheelLinkTransform);
				wheelLink.linkWheelIndex = 1;
				wheelLink
					.BuildWheel<SoloRoll11024, ElementSupplier,
						IndependentSpinningController11024<IndependentWheelAnimationController11024>>(machineContext.state
						.Get<WheelStateLink11024>(2));
				wheelLink.SetUpWinLineAnimationController<WinLineAnimationController11024>();
			}
		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotView = machineContext.transform.Find("Wheels/JackpotPanel");
			machineContext.view.Add<JackPotPanel11024>(jackpotView);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			var pigGroup = machineContext.transform.Find("Wheels/WheelBaseGame/Pig");
			var pigGroupInstance = machineContext.view.Add<PigGroupView11024>(pigGroup);
			pigGroupInstance.InitAfterBindingContext();
			
			var collectBar = machineContext.transform.Find("Wheels/WheelBaseGame/ProgressBarButton");
			var collectBarInstance = machineContext.view.Add<CollectBarView11024>(collectBar);
			collectBarInstance.InitAfterBindingContext();
	
			var map = machineContext.transform.Find("Wheels/MapGame");
			var mapInstance = machineContext.view.Add<MapView11024>(map);
			mapInstance.InitAfterBindingContext();
			
			var smallJackpot = machineContext.transform.Find("Wheels/WheelLinkGame2_1/SmallJackPotPanel");
			var smallJackpotInstance = machineContext.view.Add<JackPotSmallPanel11024>(smallJackpot);
			smallJackpotInstance.InitAfterBindingContext();
			
			var linkPigGroup = machineContext.transform.Find("Wheels/WheelLinkGame1/PigGroup");
			var linkPigGroupInstance = machineContext.view.Add<LinkBigPigGroupView11024>(linkPigGroup);
			linkPigGroupInstance.InitAfterBindingContext();
			
			var linkSmallPigGroup = machineContext.transform.Find("Wheels/WheelLinkGame2_1/PigGroup");
			var linkSmallPigGroupInstance = machineContext.view.Add<LinkSmallPigGroupView11024>(linkSmallPigGroup);
			linkSmallPigGroupInstance.InitAfterBindingContext();
		}
		public override void BindingBackgroundView(MachineContext machineContext)
		{
			var background = machineContext.transform.Find("Background");
			machineContext.view.Add<BackgroundView11024>(background);
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11024);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11024);
			logicProxy[LogicStepType.STEP_ROUND_START] = typeof(RoundStartProxy11024);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11024);
			logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11024);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11024);
			logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11024);
			logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11024);
			logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11024);
			logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11024);
			logicProxy[LogicStepType.STEP_LATE_HIGH_LEVEL_WIN_EFFECT] = typeof(LateHighLevelWinEffectProxy11024);
			logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11024);
			// logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(ReSpinLogicProxy11024);
			logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11024);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11024);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11024);
			logicProxy[LogicStepType.STEP_ROUND_FINISH] = typeof(RoundFinishProxy11024);
			return logicProxy;
		}
	
	}
}