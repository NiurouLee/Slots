using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11022 : MachineContextBuilder
	{
		public MachineContextBuilder11022(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11022();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return new SequenceElementConstructor11022(machineContext);
		}
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11022>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
			//Free
			machineState.Add<WheelState>();
			//Bonus 15X1
			machineState.Add<LinkWheelState11022>();
			//SingleWheel 2X2
			//SingleWheel 2X3
			//SingleWheel 2X4
			//SingleWheel 2X5
			//SingleWheel 3X2
			//SingleWheel 3X3
			//SingleWheel 3X4
			//SingleWheel 3X5
			for (int i = 0; i < 8; i++)
			{
				machineState.Add<SingleWheelState11022>();
			}

			machineState.Add<ExtraState11022>();
			machineState.Replace<ReSpinState, ReSpinState11022>();
		}
		
		public override Vector2 GetPadMinXScaler()
		{
			return new Vector2(0.85f, 0.85f);
		}
		
		public override void BindingBackgroundView(MachineContext machineContext)
		{
			var background = machineContext.transform.Find("Background");
			machineContext.view.Add<BackgroundView11022>(background);
		}
		
		public override void BindingWheelView(MachineContext machineContext)
		{
			var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
			var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
			baseWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11022>>(machineContext.state.Get<WheelState>());
			baseWheel.SetUpWinLineAnimationController<WinLineAnimationController11022>();

			var wheelFreeTransform = machineContext.transform.Find("Wheels/WheelFreeGame");
			var wheelFree = machineContext.view.Add<Wheel>(wheelFreeTransform);
			wheelFree.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController11022>>(machineContext.state.Get<WheelState>(1));
			wheelFree.SetUpWinLineAnimationController<WinLineAnimationController11022>();
			
			var wheelLinkTransform = machineContext.transform.Find("Wheels/WheelLinkGame");
			var wheelLink = machineContext.view.Add<IndependentWheel>(wheelLinkTransform);
			wheelLink.BuildWheel<SoloRoll11022, ElementSupplier,IndependentSpinningController<IndependentWheelAnimationController11022>>(machineContext.state.Get<LinkWheelState11022>());
			wheelLink.SetUpWinLineAnimationController<WinLineAnimationController11022>();
			
			var wheelsTransform = machineContext.transform.Find("Wheels");
			var wheelCount = wheelsTransform.childCount;
			
			for (var i = 3; i < wheelCount; i++)
			{
				if (wheelsTransform.GetChild(i).name.Contains("Wheel"))
				{
					var SingleWheel = machineContext.view.Add<Wheel>(wheelsTransform.GetChild(i));
					SingleWheel.BuildWheel<SingleRoll11022, ElementSupplier,WheelSpinningController<SingleWheelAnimationController11022>>(machineContext.state.Get<SingleWheelState11022>(i-3));
					SingleWheel.SetUpWinLineAnimationController<WinLineAnimationController11022>();
				}
			}
		}
		
		public override void BindingExtraView(MachineContext machineContext)
		{
			var freeTitleView = machineContext.transform.Find("Wheels/WheelFreeGame/TipsGroup");
			machineContext.view.Add<FreeSpinTitleView11022>(freeTitleView);
			var linkTitleView = machineContext.transform.Find("Wheels/WheelLinkGame/TipsGroup");
			machineContext.view.Add<LinkSpinTitleView11022>(linkTitleView);
		}
		
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotView = machineContext.transform.Find("JackpotPanel");
			machineContext.view.Add<JackPotPanel11022>(jackpotView);
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11022);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11022);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11022);
			logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11022);
			logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11022);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11022);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11022);
			logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11022);
			logicProxy[LogicStepType.STEP_ROUND_FINISH] = typeof(RoundFinishProxy11022);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11022);
			return logicProxy;
		}
	}
}