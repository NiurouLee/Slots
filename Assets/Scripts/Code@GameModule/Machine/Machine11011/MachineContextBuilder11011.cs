//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-18 13:16
//  Ver : 1.0.0
//  Description : MachineContextBuilder11011.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11011:MachineContextBuilder
    {
        public MachineContextBuilder11011(string inMachineId)
            :base(inMachineId)
        {
            machineId = inMachineId;
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            machineState.Add<WheelState>();
            machineState.Add<WheelState>();
            machineState.Add<WheelState>();
            machineState.Add<ExtraState11011>();

            machineState.Replace<FreeSpinState, FreeSpinState11011>();
            machineState.Replace<ReSpinState, ReSpinState11011>();
            machineState.Replace<WinState, WinState11011>();
        }
        
        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11011>();
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("Wheels/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var transCollect = machineContext.transform.Find("Wheels/WheelBaseGame/CollectNoticeGroup");
            machineContext.view.Add<CollectBonusView11011>(transCollect);
            machineContext.view.Get<CollectBonusView11011>().ToggleVisible(false);

            var transLink = machineContext.transform.Find("Wheels/WheelLinkGame/WinIntegralGroup");
            machineContext.view.Add<FeatureView11011>(transLink).Initialize(machineContext);
            
            var transFree = machineContext.transform.Find("Wheels/WheelFreeGame/WinIntegralGroup");
            machineContext.view.Add<FeatureView11011>(transFree).Initialize(machineContext);
            
            var transCoinCollect = machineContext.transform.Find("Wheels/WheelBaseGame");
            machineContext.view.Add<CollectCoinView11011>(transCoinCollect);
            
            var transCut = machineContext.transform.Find("Wheels/FeatureStartCutPopup");
            machineContext.view.Add<FeatureCutView11011>(transCut);
            machineContext.view.Get<FeatureCutView11011>().ToggleVisible(false);
        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
            var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
            baseWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11011>>(machineContext.state.Get<WheelState>(0));
            baseWheel.SetUpWinLineAnimationController<WinLineAnimationController>();
            
            var wheelTransform = machineContext.transform.Find("Wheels/WheelLinkGame");
            var wheel = machineContext.view.Add<IndependentWheel>(wheelTransform);
            wheel.BuildWheel<SoloRoll11011, ElementSupplier,IndependentSpinningController<WheelAnimationController11011>>(machineContext.state.Get<WheelState>(1));
            wheel.SetUpWinLineAnimationController<WinLineAnimationController>();
            
            var wheelFreeTransform = machineContext.transform.Find("Wheels/WheelFreeGame");
            var wheelFree = machineContext.view.Add<Wheel>(wheelFreeTransform);
            wheelFree.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(2));
            wheelFree.SetUpWinLineAnimationController<WinLineAnimationController>();

            machineContext.view.Add<MainPickView11011>(machineContext.transform.Find("Wheels/WheelJackpotGame")).Initialize(machineContext);
            machineContext.view.Get<MainPickView11011>().ToggleVisible(false);
        }
        
        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11011);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11011);
            logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11011);
            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11011);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11011);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11011);
            return logicProxy;
        }
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11011();
        }
        
        
        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.91f, 0.91f);
        }
        
    }
}