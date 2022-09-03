//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-12 13:49
//  Ver : 1.0.0
//  Description : MachineContextBuilder11007.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11007: MachineContextBuilder
    {
        public MachineContextBuilder11007(string inMachineId)
        :base(inMachineId)
        {
            machineId = inMachineId;
        }
        public override void SetUpMachineState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState>();
            machineState.Add<WheelState>();
            machineState.Add<LinkWheelState11007>();
            machineState.Add<BonusWheelState11007>();
            machineState.Add<ExtraState11007>();
            machineState.Replace<ReSpinState, ReSpinState11007>();
        }
         
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTrans = machineContext.transform.Find("Wheels/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTrans);
            machineContext.view.Get<JackPotPanel>().Show();
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11007>();
        }
        
        public override void BindingExtraView(MachineContext machineContext)
        {
            var wheelTrans = machineContext.transform.Find("Wheels");
            var jackpotTrans =wheelTrans.Find("WheelRespinBonus");
            machineContext.view.Add<LinkBonusTitle11007>(jackpotTrans);

            var linkCounterTrans = machineContext.transform.Find("Wheels/WheelRespinLink/RespinsCount");
            machineContext.view.Add<LinkCounter11007>(linkCounterTrans);
        }
        
        
        public override void BindingWheelView(MachineContext machineContext)
        {
            var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
            var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
            baseWheel.BuildWheel<StepperRoll11007, ElementSupplier, WheelSpinningController<WheelAnimationController11007>>(machineContext.state.Get<WheelState>());
            baseWheel.SetUpWinLineAnimationController<WinLineAnimationController>();
            
            var wheelTransform = machineContext.transform.Find("Wheels/WheelRespinLink");
            var wheel = machineContext.view.Add<IndependentWheel>(wheelTransform);
            wheel.BuildWheel<SoloRoll11007, ElementSupplier,IndependentSpinningController<WheelAnimationController>>(machineContext.state.Get<LinkWheelState11007>());
            wheel.SetUpWinLineAnimationController<WinLineAnimationController>();

            var bonusWheelTransform = machineContext.transform.Find("Wheels/WheelRespinBonus");
            var bonusWheel = machineContext.view.Add<Wheel>(bonusWheelTransform);
            bonusWheel.BuildWheel<StepperRoll11007, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<BonusWheelState11007>());
            bonusWheel.SetUpWinLineAnimationController<WinLineAnimationController>();
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy =  base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11007);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11007);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11007);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11007);
            logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11007);
            logicProxy[LogicStepType.STEP_RE_SPIN]       = typeof(LinkLogicProxy11007);
            logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11007);
            return logicProxy;
        }
        
        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11007(machineContext);
        }
                
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11007();
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.88f,0.92f);
        }
    }
}