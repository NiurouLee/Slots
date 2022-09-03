using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11013 : MachineContextBuilder
	{
		public MachineContextBuilder11013(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11013();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return base.GetSequenceElementConstructor(machineContext);
		}
		
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11013>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
            
			
            
            
			machineState.Add<ExtraState11013>();
		}
		public override void BindingWheelView(MachineContext machineContext)
		{

			var wheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
			var wheelBaseGameWheel = machineContext.view.Add<Wheel>(wheelBaseGameTrans);
			wheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11013>>(machineContext.state.Get<WheelState>());
			wheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11013>();

		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			base.BindingJackpotView(machineContext);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			base.BindingExtraView(machineContext);
			
			var tranBaseCollect = machineContext.transform.Find($"Wheels/WheelBaseGame/CollectGroup");
			machineContext.view.Add<BaseCollect11013>(tranBaseCollect);
			
			machineContext.view.Add<LockView11013>(null);

			var tranFeatureGame = machineContext.transform.Find($"Wheels/FeaureGame");
			machineContext.view.Add<FeatureGame11013>(tranFeatureGame);
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11013);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11013);
			logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11013);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11013);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundEndProxy11013);
			logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11013);

			return logicProxy;
		}
		
		
		public override Vector2 GetPadMinXScaler()
		{
			return new Vector2(0.8f,0.85f);
		}

	}
}