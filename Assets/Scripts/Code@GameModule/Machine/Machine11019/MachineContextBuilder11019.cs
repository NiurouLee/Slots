using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11019 : MachineContextBuilder
	{
		public MachineContextBuilder11019(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11019();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return new SequenceElementConstructor11019(machineContext);
		}

		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11019>();
		}

		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
			
			//Free
			machineState.Add<WheelState>();
			
			//Link
			machineState.Add<WheelStateLink11019>();
            
			
            
            
			machineState.Add<ExtraState11019>();
			machineState.Replace<ReSpinState, ReSpinState11019>();
		}
		public override void BindingWheelView(MachineContext machineContext)
		{

			var WheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
			var WheelBaseGameWheel = machineContext.view.Add<Wheel>(WheelBaseGameTrans);
			WheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>());
			WheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var wheelTransform = machineContext.transform.Find("Wheels/WheelLinkGame");
			var wheel = machineContext.view.Add<IndependentWheel>(wheelTransform);
			wheel.BuildWheel<SoloRoll, ElementSupplier,IndependentSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelStateLink11019>());
			wheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var WheelFreeGameTrans = machineContext.transform.Find("Wheels/WheelFreeGame");
			var WheelFreeGameWheel = machineContext.view.Add<Wheel>(WheelFreeGameTrans);
			WheelFreeGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(1));
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
			var tranGroupTransitions = machineContext.transform.Find("Wheels/GroupTransitions");
			machineContext.view.Add<TransitionsView11019>(tranGroupTransitions);


			var tranFreeBonusWin = machineContext.transform.Find($"Wheels/WheelFreeGame/{Constant11019.FreeBonusWinViewName}");
			machineContext.view.Add<BonusWinView11019>(tranFreeBonusWin);
			
			var tranLinkBonusWin = machineContext.transform.Find($"Wheels/WheelLinkGame/{Constant11019.LinkBonusWinViewName}");
			machineContext.view.Add<BonusWinView11019>(tranLinkBonusWin);
			
			var tranLinkLockView = machineContext.transform.Find($"Wheels/WheelLinkGame/LockGroup");
			machineContext.view.Add<LinkLockView11019>(tranLinkLockView);

		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11019);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11019);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11019);
			logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11019);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11019);
			logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11019);

			return logicProxy;
		}
		
		
		public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("Wheels");
            
            var uiTotalHeight = Constant11019.wheelDesignHeight + Constant11019.jackpotPanelHeight;
            var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;

            var gameTotalHeight = systemUIHeight + uiTotalHeight;
            
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
            
            //Transform tranMan = machineContext.transform.Find("GroupTransitions");

            float offsetY = wheels.localPosition.y;
            
            wheels.localPosition = new Vector3(0, (-deviceHeight * 0.5f + MachineConstant.controlPanelVHeight + wheelsOffsetY) * MachineConstant.pixelPerUnitInv, 0);
            wheels.localScale = new Vector3(scale, scale, scale);

            // offsetY = wheels.localPosition.y - offsetY;
            // var posMan = tranMan.localPosition;
            // posMan.y = posMan.y + offsetY;
            // tranMan.localPosition = posMan;
            // tranMan.localScale = new Vector3(scale, scale, scale);
            
            if (scale < 1)
            {
                var jackpotPanel = machineContext.view.Get<JackPotPanel>();
                jackpotPanel.transform.localScale = new Vector3(scale, scale, scale);
                
                var jackpotIdealPos =
                    Constant11019.wheelDesignHeight * scale + MachineConstant.controlPanelVHeight +
                    Constant11019.jackpotPanelHeight * scale * 0.5f;

                    jackpotPanel.transform.localPosition =
                    new Vector3(0, (jackpotIdealPos -deviceHeight * 0.5f)* MachineConstant.pixelPerUnitInv, 0);
            }
            else
            {
                float jackpotPanelStartPos = wheelsOffsetY + 
                    Constant11019.wheelDesignHeight + MachineConstant.controlPanelVHeight +
                    Constant11019.jackpotPanelHeight;

                var availableHeight = deviceHeight - jackpotPanelStartPos - MachineConstant.topPanelVHeight;

                var jackpotOffset =  Mathf.Min(50, availableHeight / 2);
                
                // if(availableHeight > 120)
                // {
                //     jackpotPanelStartPos = jackpotPanelStartPos - Constant11019.jackpotPanelHeight * 0.5f + 40 - deviceHeight * 0.5f;
                //     
                //     machineContext.view.Get<JackPotPanel>().transform.localPosition =
                //         new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
                // }
                // else
                {
                    jackpotPanelStartPos = (jackpotPanelStartPos + jackpotOffset) - Constant11019.jackpotPanelHeight * 0.5f - deviceHeight * 0.5f;
                    machineContext.view.Get<JackPotPanel>().transform.localPosition =
                        new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
                }
            }
        }
		
		
		

	}
}