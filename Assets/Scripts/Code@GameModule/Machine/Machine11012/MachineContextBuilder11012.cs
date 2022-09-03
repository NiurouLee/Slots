using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11012 : MachineContextBuilder
	{
		public MachineContextBuilder11012(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11012();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return base.GetSequenceElementConstructor(machineContext);
		}
		
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11012>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
			
			//Link
			machineState.Add<WheelState>();
			
			//Free
			machineState.Add<WheelState>();
			
			
            
			machineState.Add<ExtraState11012>();
			machineState.Replace<ReSpinState, ReSpinState11012>();

		}
		public override void BindingWheelView(MachineContext machineContext)
		{

			var WheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
			var WheelBaseGameWheel = machineContext.view.Add<Wheel>(WheelBaseGameTrans);
			WheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>());
			WheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var WheelLinkGameTrans = machineContext.transform.Find("Wheels/WheelLinkGame");
			var WheelLinkGameWheel = machineContext.view.Add<IndependentWheel>(WheelLinkGameTrans);
			WheelLinkGameWheel.BuildWheel<SoloRoll, ElementSupplier, IndependentSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(1));
			WheelLinkGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var WheelFreeGameTrans = machineContext.transform.Find("Wheels/WheelFreeGame");
			var WheelFreeGameWheel = machineContext.view.Add<Wheel>(WheelFreeGameTrans);
			WheelFreeGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11012>>(machineContext.state.Get<WheelState>(2));
			WheelFreeGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotTran = machineContext.transform.Find("JackpotPanel");
			machineContext.view.Add<JackPotPanel>(jackpotTran);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			base.BindingExtraView(machineContext);
			machineContext.view.Add<DoorView11012>(null);
			
			var tranTransitionView = machineContext.transform.Find("Wheels/TransitionView");
			machineContext.view.Add<TransitionView11012>(tranTransitionView);
			
			
			var LinkWinView = machineContext.transform.Find("Wheels/WheelLinkGame/WinGroup");
			machineContext.view.Add<BonusWinView11012>(LinkWinView);
			
			
			var linkLockView = machineContext.transform.Find("Wheels/WheelLinkGame/SpinRemainingGroup");
			machineContext.view.Add<LinkLockView11012>(linkLockView);
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11012);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11012);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11012);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11012);
			logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11012);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11012);
			logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11012);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11012);

			return logicProxy;
		}


		public override Vector2 GetPadMinXScaler()
		{
			return new Vector2(0.85f, 0.85f);
		}
	}
}