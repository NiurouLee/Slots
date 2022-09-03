using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11021 : MachineContextBuilder
	{
		public MachineContextBuilder11021(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11021();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return base.GetSequenceElementConstructor(machineContext);
		}
		
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11021>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
            
			
            
            
			machineState.Add<ExtraState11021>();
		}
		public override void BindingWheelView(MachineContext machineContext)
		{

			var WheelBaseGameTrans = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelBaseGame");
			var WheelBaseGameWheel = machineContext.view.Add<Wheel>(WheelBaseGameTrans);
			WheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>());
			WheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotTran = machineContext.transform.Find("ZhenpingAnim/JackpotPanel");
			machineContext.view.Add<JackpotPanel11021>(jackpotTran);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			base.BindingExtraView(machineContext);
			
			var tranPrizeView = machineContext.transform.Find("ZhenpingAnim/Wheels/PrizeDiskPanel");
			machineContext.view.Add<TitlePrizeView>(tranPrizeView);

			machineContext.view.Add<TransitionView11021>(machineContext.transform);

		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11021);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11021);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11021);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11021);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundEndProxy11021);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11021);
			logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11021);
			logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11021);
			logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11021);
			logicProxy[LogicStepType.STEP_LATE_HIGH_LEVEL_WIN_EFFECT] = typeof(LateHighLevelWinEffectProxy11021);
			logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11021);

			return logicProxy;
		}
		
		
		public override Vector2 GetPadMinXScaler()
		{
			return new Vector2(0.8f,0.85f);
		}

	}
}