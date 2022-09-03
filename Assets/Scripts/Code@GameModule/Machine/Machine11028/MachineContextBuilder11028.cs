//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-07 14:50
//  Ver : 1.0.0
//  Description : MachineContextBuilder11028.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11028: MachineContextBuilder
    {
        public MachineContextBuilder11028(string inMachineId) 
            : base(inMachineId)
        {
            
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState11028>();

            machineState.Add<ExtraState11028>();
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11028>();
        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelTrans = machineContext.transform.Find("Wheels/WheelBaseGame");

            var wheel = machineContext.view.Add<Wheel>(wheelTrans);
            wheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11028>>(machineContext.state.Get<WheelState>());
            wheel.SetUpWinLineAnimationController<WinLineAnimationController11028>();

        }

        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("Wheels/JackpotPanel");
            machineContext.view.Add<JackpotPotPanel11028>(jackpotTran);
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            machineContext.view.Add<BackgroundView11028>(machineContext.transform).Initialize(machineContext);

            var jackpotTran = machineContext.transform.Find("Wheels/WheelBonusChose");
            machineContext.view.Add<WheelBonusChooseView11028>(jackpotTran).Initialize(machineContext);
            jackpotTran.gameObject.SetActive(false);
            
            var characterTran = machineContext.transform.Find("Wheels/Anticipation");
            machineContext.view.Add<TransitionView11028>(characterTran).Initialize(machineContext);
            machineContext.view.Get<TransitionView11028>().Hide();
            
            var freeTrans = machineContext.transform.Find("Wheels/TransitionAnimation");
            machineContext.view.Add<TransitionView11028>(freeTrans).Initialize(machineContext);
            machineContext.view.Get<TransitionView11028>().Hide();
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.8f,0.8f);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11028);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11028);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11028);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11028);
            logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11028);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11028);
            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11028);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11028);
            return logicProxy;
        }
        
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11028();
        }
    }
}