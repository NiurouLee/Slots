using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11015 : MachineContextBuilder
	{
		public MachineContextBuilder11015(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11015();
		}


		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return new SequenceElementConstructor11015(machineContext);
		}
		
		
		public override IMachineAudioConfig SetUpCommonAudioConfig(MachineContext machineContext)
		{
			var audioConfig = new MachineAudioConfig11015(machineId,machineContext);
			return audioConfig;
		}


		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11015>();
		}

		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState11015>();
			
			//Free
			machineState.Add<WheelState11015>();
			
            
			
            
            
			machineState.Add<ExtraState11015>();
			machineState.Replace<FreeSpinState, FreeSpinState11015>();
		}
		public override void BindingWheelView(MachineContext machineContext)
		{

			var WheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
			var WheelBaseGameWheel = machineContext.view.Add<Wheel11015>(WheelBaseGameTrans);
			WheelBaseGameWheel.BuildWheel<Roll11015, ElementSupplier, WheelSpinningController11015<WheelAnimationController>>(machineContext.state.Get<WheelState11015>());
			WheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11015>();


			var WheelFreeGameTrans = machineContext.transform.Find("Wheels/WheelFreeGame12X4");
			var WheelFreeGameWheel = machineContext.view.Add<Wheel>(WheelFreeGameTrans);
			WheelFreeGameWheel.BuildWheel<FreeRoll11015, ElementSupplier, WheelSpinningController11015<WheelAnimationController>>(machineContext.state.Get<WheelState11015>(1));
			WheelFreeGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11015>();

		}
		
		public override void BindingExtraView(MachineContext machineContext)
		{
			base.BindingExtraView(machineContext);

			var tranRollsMask = machineContext.transform.Find("Wheels/WheelBaseGame/RollMaskGroup");
			machineContext.view.Add<RollMasks11015>(tranRollsMask);

			machineContext.view.Add<LockView11015>(machineContext.transform);

			machineContext.view.Add<TransitionView11015>(machineContext.transform);
		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11015);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11015);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11015);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11015);
			logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(ReSpinProxy11015);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11015);

			
			return logicProxy;
		}
		
		
		public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("Wheels");
            
            var uiTotalHeight = Constant11015.wheelDesignHeight + Constant11015.jackpotPanelHeight;
            var uiBaseTotalHeight = Constant11015.wheelBaseDesignHeight + Constant11015.jackpotPanelHeight;

            
            var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelVHeight;

            var gameTotalHeight = systemUIHeight + uiTotalHeight;
            var gameBaseTotalHeight = systemUIHeight + uiBaseTotalHeight;
            
            var deviceHeight = ViewResolution.referenceResolutionPortrait.y;

            int wheelsOffsetY = 0;
            float scale = 1.0f;
            float scaleBase = 1;
            if (gameTotalHeight <= deviceHeight)
            {
                var deltaHeight = deviceHeight - gameTotalHeight;

                if (deltaHeight > 75)
                {
                    wheelsOffsetY = 30;
                }
            }
            else
            {
                scale = 1 - (gameTotalHeight - deviceHeight) / uiTotalHeight;
                scaleBase = 1 - (gameBaseTotalHeight - deviceHeight) / uiBaseTotalHeight;
            }
         
            float offsetY = wheels.localPosition.y;


            
            
            
            wheels.localScale = new Vector3(scale, scale, scale);
			
          
            var tranWheelFree = wheels.Find("WheelFreeGame12X4");
            float newPosX = (MachineConstant.controlPanelVHeight - MachineConstant.topPanelVHeight) * 0.5f * MachineConstant.pixelPerUnitInv;
            //tranWheelFree.localScale = new Vector3(scale, scale, scale);
            tranWheelFree.localPosition = new Vector3(0,newPosX, 0);
            
            var tranWheelBase = wheels.Find("WheelBaseGame");
            //tranWheelBase.localScale = new Vector3(scaleBase, scaleBase, scaleBase);
            
            

        }
		
		
		

	}
}