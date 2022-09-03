using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using GameModule.Utility;

namespace GameModule
{
	public class MachineContextBuilder11009 : MachineContextBuilder
	{
		public MachineContextBuilder11009(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11009();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return base.GetSequenceElementConstructor(machineContext);
		}
		
		
		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
			
			//Free
			machineState.Add<WheelState>();
			machineState.Add<WheelState>();
			
			
			machineState.Add<ExtraState11009>();
			machineState.Replace<FreeSpinState, FreeSpinState11009>();
			machineState.Replace<JackpotInfoState, JackpotState11009>();
		}
		
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11009>();
		}
		
		
		
		public override void BindingWheelView(MachineContext machineContext)
		{

			var WheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
			var WheelBaseGameWheel = machineContext.view.Add<Wheel>(WheelBaseGameTrans);
			WheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11009>>(machineContext.state.Get<WheelState>());
			WheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var WheelFreeGame5X4Trans = machineContext.transform.Find("Wheels/WheelFreeGame5X4");
			var WheelFreeGame5X4Wheel = machineContext.view.Add<Wheel>(WheelFreeGame5X4Trans);
			WheelFreeGame5X4Wheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11009>>(machineContext.state.Get<WheelState>(1));
			WheelFreeGame5X4Wheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var WheelFreeGame5X6Trans = machineContext.transform.Find("Wheels/WheelFreeGame5X6");
			var WheelFreeGame5X6Wheel = machineContext.view.Add<Wheel>(WheelFreeGame5X6Trans);
			WheelFreeGame5X6Wheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11009>>(machineContext.state.Get<WheelState>(2));
			WheelFreeGame5X6Wheel.SetUpWinLineAnimationController<WinLineAnimationController>();

		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotTran = machineContext.transform.Find("JackpotPanel");
			machineContext.view.Add<JackpotView11009>(jackpotTran);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			var tranBoxesGroup = machineContext.transform.Find("BoxesGroup");
			machineContext.view.Add<BoxesView11009>(tranBoxesGroup);

			var tranTransition = machineContext.transform.Find("Transition");
			machineContext.view.Add<TransitionView11009>(tranTransition);

		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11009);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11009);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11009);

			return logicProxy;
		}
		
		
		
		
		public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("Wheels");
            
            int uiTotalHeight = Constant11009.wheelDesignHeight + Constant11009.jackpotPanelHeight;
            float systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;

            float gameTotalHeight = systemUIHeight + uiTotalHeight;
            
            var deviceHeight = ViewResolution.referenceResolutionPortrait.y;

            int wheelsOffsetY = 0;
            float scale = 1.0f;
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
            }
            

            float offsetY = wheels.localPosition.y;
            
            wheels.localPosition = new Vector3(0, (-deviceHeight * 0.5f + MachineConstant.controlPanelVHeight + wheelsOffsetY) * MachineConstant.pixelPerUnitInv, 0);
            wheels.localScale = new Vector3(scale, scale, scale);

            offsetY = wheels.localPosition.y - offsetY;
            
            
            List<string> listExtraName = new List<string>();
            listExtraName.Add("BoxesGroup");
            //listExtraName.Add("Transition");
            listExtraName.Add("BoxesGroup_Jackpot");

            foreach (string extraName in listExtraName)
            {
	            Transform tranExtra = machineContext.transform.Find(extraName);
	            var posExtra = tranExtra.localPosition;
	            posExtra.y = posExtra.y + offsetY;
	            tranExtra.localPosition = posExtra;
	            tranExtra.localScale = new Vector3(scale, scale, scale);
            }

            
            
            if (scale < 1)
            {
                var jackpotPanel = machineContext.view.Get<JackPotPanel>();
                jackpotPanel.transform.localScale = new Vector3(scale, scale, scale);
                
                var jackpotIdealPos =
                    Constant11009.wheelDesignHeight * scale + MachineConstant.controlPanelVHeight +
                    Constant11009.jackpotPanelHeight * scale * 0.5f;

                    jackpotPanel.transform.localPosition =
                    new Vector3(0, (jackpotIdealPos -deviceHeight * 0.5f)* MachineConstant.pixelPerUnitInv, 0);
            }
            else
            {
                float jackpotPanelStartPos = wheelsOffsetY + 
                    Constant11009.wheelDesignHeight + MachineConstant.controlPanelVHeight +
                    Constant11009.jackpotPanelHeight;

                var availableHeight = deviceHeight - jackpotPanelStartPos - MachineConstant.topPanelVHeight;

                var jackpotOffset =  Mathf.Min(50, availableHeight / 2);
                
                // if(availableHeight > 120)
                // {
                //     jackpotPanelStartPos = jackpotPanelStartPos - Constant11009.jackpotPanelHeight * 0.5f + 40 - deviceHeight * 0.5f;
                //     
                //     machineContext.view.Get<JackPotPanel>().transform.localPosition =
                //         new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
                // }
                // else
                {
                    jackpotPanelStartPos = (jackpotPanelStartPos + jackpotOffset) - Constant11009.jackpotPanelHeight * 0.5f - deviceHeight * 0.5f;
                    machineContext.view.Get<JackPotPanel>().transform.localPosition =
                        new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
                }
            }
        }

	}
}