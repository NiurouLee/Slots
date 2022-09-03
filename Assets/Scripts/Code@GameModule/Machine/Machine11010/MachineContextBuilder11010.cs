//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-31 17:40
//  Ver : 1.0.0
//  Description : MachineContextBuilder11010.cs
//  ChangeLog :
//  **********************************************


using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11010: MachineContextBuilder
    {
        public MachineContextBuilder11010(string inMachineId)
            :base(inMachineId)
        {
            machineId = inMachineId;
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("Wheels/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
            
        }
        
        public override void BindingWheelView(MachineContext machineContext)
        {
            var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
            var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
            baseWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11010>>(machineContext.state.Get<WheelState>());
            baseWheel.SetUpWinLineAnimationController<WinLineAnimationController11010>();
            
            var wheelTransform = machineContext.transform.Find("Wheels/WheelLinkGame");
            var wheel = machineContext.view.Add<IndependentWheel>(wheelTransform);
            wheel.BuildWheel<SoloRoll, ElementSupplier,IndependentSpinningController<WheelAnimationController11010>>(machineContext.state.Get<LinkWheelState11010>());
            wheel.SetUpWinLineAnimationController<WinLineAnimationController>();
            
            var wheelFreeTransform = machineContext.transform.Find("Wheels/WheelFreeGame");
            var wheelFree = machineContext.view.Add<Wheel>(wheelFreeTransform);
            wheelFree.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController11010>>(machineContext.state.Get<WheelState>(2));
            wheelFree.SetUpWinLineAnimationController<WinLineAnimationController11010>();

            for (int i = 0; i < 3; i++)
            {
                var transLinkCounter = wheelTransform.Find("RespinsCount"+(3+i));
                machineContext.view.Add<LinkCounter11010>(transLinkCounter);
                transLinkCounter.gameObject.SetActive(false);
            }
        }
        
        public override void SetUpMachineState(MachineState machineState)
        {
            machineState.Add<WheelState>();
            machineState.Add<LinkWheelState11010>();
            machineState.Add<WheelState>();
            machineState.Add<ExtraState11010>();
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11010>();
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11010);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11010);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11010);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11010);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11010);
            return logicProxy;
        }
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11010();
        }
        
        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11010(machineContext);
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.95f,0.95f);
        }
        
        
    }
}