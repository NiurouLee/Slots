using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class MachineContextBuilder11017 : MachineContextBuilder
    {
        public MachineContextBuilder11017(string inMachineId)
        :base(inMachineId)
        {

    
        }
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11017();
        }
      
        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11017(machineContext);
        }
        

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11017>();
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState11017>();
            
            //Free
            machineState.Add<WheelState11017>();
            
            //Link
            machineState.Add<WheelState11017>();
            machineState.Add<ExtraState11017>();
            machineState.Add<WinState11017>();
            machineState.Replace<ReSpinState, ReSpinState11017>();
        }
        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelBaseGameTrans = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelBaseGame");
			var wheelBaseGameWheel = machineContext.view.Add<Wheel>(wheelBaseGameTrans);
			wheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11017>>(machineContext.state.Get<WheelState>());
			wheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11017>();
           
			var wheelLinkGameTrans = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelLinkGame");
			var wheelLinkGameWheel = machineContext.view.Add<Wheel>(wheelLinkGameTrans);
			wheelLinkGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11017>>(machineContext.state.Get<WheelState>(1));
			wheelLinkGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11017>();

			var wheelFreeGameTrans = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelFreeGame");
			var wheelFreeGameWheel = machineContext.view.Add<Wheel>(wheelFreeGameTrans);
            wheelFreeGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11017>>(machineContext.state.Get<WheelState>(2));
			wheelFreeGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11017>();
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("ZhenpingAnim/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
        }
        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            
            var tranFreeGameMark = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelBaseGame/BGGroup/machine_11017_mark");
            machineContext.view.Add<FreeGameMark11017>(tranFreeGameMark);

            var tranSuperFreeGameCoins = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelBaseGame/BGGroup/SFG_coins");
            machineContext.view.Add<SuperFreeGameCoins11017>(tranSuperFreeGameCoins);
            
            var tranFeatureGame = machineContext.transform.Find("ZhenpingAnim/Wheels/Feature");
            machineContext.view.Add<FeatureGame11017>(tranFeatureGame);
            
            var tranGroupTransitions = machineContext.transform.Find("ZhenpingAnim/Wheels/GroupTransitions");
			machineContext.view.Add<TransitionsView11017>(tranGroupTransitions);
            
            var tranFreeRemaining = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelFreeGame/BGGroup/SpinsRemaining");
            machineContext.view.Add<FreeRemaining11017>(tranFreeRemaining);
            
            var tranLinkRemaining = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelLinkGame/BGGroup/SpinsRemaining");
			machineContext.view.Add<LinkRemaining11017>(tranLinkRemaining);
            
            var tranFeatureGameTips = machineContext.transform.Find("ZhenpingAnim/Wheels/FeatureNotice");
            machineContext.view.Add<FeatureGameTips11017>(tranFeatureGameTips);
            
            var tranSuperFreeGameLock = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelBaseGame/LockSFG");
            machineContext.view.Add<SuperFreeGameLock11017>(tranSuperFreeGameLock);
            
            var tranSuperFreeGameIcon = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelFreeGame/Active_W02");
            machineContext.view.Add<SuperFreeGameIcon11017>(tranSuperFreeGameIcon);
            
            var tranSuperFreeGameGlowIcon = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelFreeGame/SFGGlow");
            machineContext.view.Add<SuperFreeGameGlowIcon11017>(tranSuperFreeGameGlowIcon);
        }
        public override void AttachTopPanel(MachineContext machineContext)
        {
            base.AttachTopPanel(machineContext);
        }
        protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11017);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11017);
            logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11017);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11017);
            logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11017);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11017);
            logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11017);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(ReSpinLogicProxy11017);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11017);
            return logicProxy;
        }
        
         public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("ZhenpingAnim/Wheels");

            int uiTotalHeight = Constant11017.wheelDesignHeight + Constant11017.jackpotPanelHeight;
            var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelVHeight;

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
            
            wheels.localPosition = new Vector3(0, (-deviceHeight * 0.5f + MachineConstant.controlPanelVHeight + wheelsOffsetY) * MachineConstant.pixelPerUnitInv, 0);
            wheels.localScale = new Vector3(scale, scale, scale);

            if (scale < 1)
            {
                var jackpotPanel = machineContext.view.Get<JackPotPanel>();
                jackpotPanel.transform.localScale = new Vector3(scale, scale, scale);
                
                var jackpotIdealPos =
                    Constant11017.wheelDesignHeight * scale + MachineConstant.controlPanelVHeight +
                    Constant11017.jackpotPanelHeight * scale * 0.5f;

                    jackpotPanel.transform.localPosition =
                    new Vector3(0, (jackpotIdealPos -deviceHeight * 0.5f)* MachineConstant.pixelPerUnitInv, 0);
            }
            else
            {

                var emptyHeight = deviceHeight - gameTotalHeight - wheelsOffsetY;

                float jackpotPanelStartPos = wheelsOffsetY +
                                             Constant11017.wheelDesignHeight + 
                                             MachineConstant.controlPanelVHeight + 
                                             Constant11017.jackpotPanelHeight * 0.5f +
                                             emptyHeight;


                var localPosition = machineContext.view.Get<FeatureGame11017>().transform.localPosition;
                localPosition.y += emptyHeight * 0.005f;
                
                machineContext.view.Get<FeatureGame11017>().transform.localPosition = localPosition;
                {
                    jackpotPanelStartPos = jackpotPanelStartPos- deviceHeight * 0.5f;
                    machineContext.view.Get<JackPotPanel>().transform.localPosition =
                        new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
                }
            }
        }
        
    }
}