// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/04/10:10
// Ver : 1.0.0
// Description : MachineContextBuilder11001.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11001:MachineContextBuilder
    {
        public MachineContextBuilder11001(string inMachineId)
            :base(inMachineId)
        {
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11001>();
        }
        public override void SetUpMachineState(MachineState machineState)
        {
            machineState.Add<WheelState>();
            machineState.Add<ExtraState11001>();
            machineState.Replace<BetState, BetState11001>();
        }

        public override void BindingBackgroundView(MachineContext machineContext)
        {
            var background = machineContext.transform.Find("Background");
            machineContext.view.Add<BackGroundView11001>(background);
        }
 
        public override void BindingExtraView(MachineContext machineContext)
        {
            var bingoMapView = machineContext.transform.Find("Wheels/WheelBaseGame/Bingo");
            machineContext.view.Add<BingoMapView11001>(bingoMapView);

            var bonusProgress = machineContext.transform.Find("Wheels/WheelBaseGame/BonusProgress");
            machineContext.view.Add<SuperFreeProgressView11001>(bonusProgress);


            var wheelBonusView = machineContext.transform.Find("WheelBonusView");
            machineContext.view.Add<JackpotWheelView11001>(wheelBonusView);
        }
        
        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");

            var wheel = machineContext.view.Add<IndependentWheel>(wheelTransform);
            wheel.BuildWheel<SoloRoll, ElementSupplier,IndependentSpinningController<IndependentWheelAnimationController>>(machineContext.state.Get<WheelState>(0));
            wheel.SetUpWinLineAnimationController<WinLineAnimationController>();
        }
        
        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11001);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11001);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11001);
            logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11001);
            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11001);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11001);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11001);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11001);
            logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11001);

            return logicProxy;
        }
        
        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11001(machineContext);
        }
        
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11001();
        }

        public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("Wheels");
            wheels.localPosition = new Vector3(0, (-ViewResolution.referenceResolutionPortrait.y * 0.5f + MachineConstant.controlPanelVHeight - 13) * MachineConstant.pixelPerUnitInv, 0);

            float panelHeight = MachineConstant.topPanelVHeight + MachineConstant.controlPanelVHeight;
            
            int uiTotalHeight = Constant11001.bingoWheelDesignHeight + Constant11001.jackpotPanelHeight;

            var deviceHeight = ViewResolution.referenceResolutionPortrait.y;
            var aspectRatio = ViewResolution.referenceResolutionPortrait.y / ViewResolution.referenceResolutionPortrait.x;
           
            float scale = 1.0f;

            if (aspectRatio >= MachineConstant.designAspectRatio)
            {
                var t = Mathf.Min(
                    (aspectRatio - MachineConstant.designAspectRatio) /
                    (MachineConstant.aspectRatio18To9 - MachineConstant.aspectRatio16To9), 1.0f);
                scale = Mathf.Lerp(1, 1.1f, t);
            }
            else
            {
                var t = Mathf.Max(0, aspectRatio - MachineConstant.aspectRatio4To3) /
                        (MachineConstant.aspectRatio16To9 - MachineConstant.aspectRatio4To3);
                
                scale = Mathf.Lerp( (float)(1024 - panelHeight)/(ViewResolution.designSize.x - panelHeight), 1, t);
            }

            wheels.localScale = new Vector3(scale, scale, scale);

            var jackpotWheelView11001 = machineContext.view.Get<JackpotWheelView11001>();
            jackpotWheelView11001.transform.localScale = wheels.localScale;

            if (scale < 1)
            {
                var jackpotPanel = machineContext.view.Get<JackPotPanel>();
                jackpotPanel.transform.localScale = new Vector3(scale, scale, scale);
                
                var jackpotIdealPos =
                    Constant11001.bingoWheelDesignHeight * scale + MachineConstant.controlPanelVHeight +
                    Constant11001.jackpotPanelHeight * scale * 0.5f;

                    jackpotPanel.transform.localPosition =
                    new Vector3(0, (jackpotIdealPos -deviceHeight * 0.5f)* MachineConstant.pixelPerUnitInv, 0);
                    
                 machineContext.view.Get<BackGroundView11001>().HideLogo();     
            }
            else
            {
                var jackpotPanelStartPos =
                    Constant11001.bingoWheelDesignHeight * scale + MachineConstant.controlPanelVHeight +
                    Constant11001.jackpotPanelHeight;

                var availableHeight = deviceHeight - jackpotPanelStartPos - MachineConstant.topPanelVHeight;
                
             //   machineContext.view.Get<BackGroundView11001>().HideLogo();
                    
                  
                if(availableHeight > Constant11001.logoHeight - 50)
                {
                    var logoStartPos = jackpotPanelStartPos - deviceHeight * 0.5f +  Constant11001.logoHeight  * 0.5f;
                    
                    machineContext.view.Get<BackGroundView11001>().SetLogoYPosition(logoStartPos * MachineConstant.pixelPerUnitInv);
                }
                else
                {
                    machineContext.view.Get<BackGroundView11001>().HideLogo();
                }
                
                jackpotPanelStartPos = jackpotPanelStartPos - Constant11001.jackpotPanelHeight * 0.5f - deviceHeight * 0.5f;
                machineContext.view.Get<JackPotPanel>().transform.localPosition =
                    new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);

            }
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
        }
    }
}