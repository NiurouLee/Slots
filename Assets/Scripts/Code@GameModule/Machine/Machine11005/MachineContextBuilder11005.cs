using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using GameModule.Utility;

namespace GameModule
{
	public class MachineContextBuilder11005 : MachineContextBuilder
	{
		public MachineContextBuilder11005(string inMachineId)
		:base(inMachineId)
		{

	
		}


		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11005();
		}

		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11005>();
		}
		
		
		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
			
			//Free
			machineState.Add<WheelState>();
			machineState.Add<WheelState>().SetResultIndex(1);
			machineState.Add<WheelState>().SetResultIndex(2);
			machineState.Add<WheelState>().SetResultIndex(3);
			
			machineState.Add<ExtraState11005>();

			machineState.Replace<BetState, BetState11005>();
		}
		
		

		
		public override void BindingWheelView(MachineContext machineContext)
		{

			// var WheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
			// var WheelBaseGameWheel = machineContext.view.Add<Wheel>(WheelBaseGameTrans);
			// WheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>());
			// WheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var wheelsTransform = machineContext.transform.Find("ZhenpingAnim/Wheels");
			var wheelCount = wheelsTransform.childCount;

			for (var i = 0; i < wheelCount; i++)
			{
				Transform tranWheel = wheelsTransform.GetChild(i);
				Wheel wheel = null;
				if (tranWheel.name=="WheelFreeGame1" ||
				    tranWheel.name=="WheelFreeGame2")
				{
					
					wheel = machineContext.view.Add<Wheel>(wheelsTransform.GetChild(i));
					wheel.BuildWheel<FreeRoll11005, ElementSupplier,WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(i));

				}
				else if(tranWheel.name=="WheelFreeGame3" ||
				        tranWheel.name=="WheelFreeGame4")
				{
					wheel = machineContext.view.Add<WheelUp11005>(wheelsTransform.GetChild(i));
					wheel.BuildWheel<FreeRollUp11005, ElementSupplier,WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(i));

				}
				else
				{
					wheel = machineContext.view.Add<Wheel>(wheelsTransform.GetChild(i));
					wheel.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController11005>>(machineContext.state.Get<WheelState>(i));

				}
				wheel.SetUpWinLineAnimationController<WinLineAnimationController>();
			}

		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotTran = machineContext.transform.Find("ZhenpingAnim/JackpotPanel");
			machineContext.view.Add<JackPotPanel11005>(jackpotTran);
            
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			var tranBaseLetter = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelBaseGame");
			machineContext.view.Add<BaseLetterView11005>(tranBaseLetter);
			
			var tranFreeGame1 = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelFreeGame1");
			machineContext.view.Add<FreeCollectView11005>(tranFreeGame1);
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11005);
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11005);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11005);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11005);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11005);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11005);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11005);
			logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11005);
			logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11005);

			return logicProxy;
		}

		public override Vector2 GetPadMinXScaler()
		{
			return new Vector2(0.73f,0.79f);
		}
	}
}