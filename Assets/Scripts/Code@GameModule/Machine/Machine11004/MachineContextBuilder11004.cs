using System;
using System.Collections.Generic;
using GameModule.Utility;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11004 : MachineContextBuilder
    {
        public MachineContextBuilder11004(string inMachineId) : base(inMachineId)
        {
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState11004>();
			
            //Free
            // machineState.Add<WheelState>();
            
            //Link
            machineState.Add<LinkWheelState11004>();
            
            
            machineState.Add<ExtraState11004>();
            machineState.Replace<ReSpinState, ReSpinState11004>();
        }
        
        
        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11004>();
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11004();
        }


        public override void BindingWheelView(MachineContext machineContext)
        {
            var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
            var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
            baseWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11004>>(machineContext.state.Get<WheelState>());
            baseWheel.SetUpWinLineAnimationController<WinLineAnimationController11004>();
            
            var wheelTransform = machineContext.transform.Find("Wheels/WheelLinkGame");
            var wheel = machineContext.view.Add<IndependentWheel>(wheelTransform);
            wheel.BuildWheel<SoloRoll, ElementSupplier,IndependentSpinningController<WheelAnimationController11004>>(machineContext.state.Get<LinkWheelState11004>());
            wheel.SetUpWinLineAnimationController<WinLineAnimationController>();
            
            // var wheelFreeTransform = machineContext.transform.Find("Wheels/WheelFreeGame");
            // var wheelFree = machineContext.view.Add<Wheel>(wheelFreeTransform);
            // wheelFree.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(1));
            // wheelFree.SetUpWinLineAnimationController<WinLineAnimationController>();
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
            
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            var baseWheelTransform = machineContext.transform;
            machineContext.view.Add<LockElementView11004>(baseWheelTransform);

            var tranBaseTitle = machineContext.transform.Find("Wheels/CollectGroup/BaseGame");
            machineContext.view.Add<BaseTitleView11004>(tranBaseTitle);

            var tranLinkTitle = machineContext.transform.Find("Wheels/CollectGroup/LinkGame");
            machineContext.view.Add<LinkTitleView11004>(tranLinkTitle);
            
            var tranBackground = machineContext.transform.Find("Background");
            machineContext.view.Add<BackgroundView11004>(tranBackground);
        }


        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11004);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11004);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11004);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11004);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11004);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11004);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11004);
            logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11004);

            return logicProxy;
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.8f,0.88f);
        }
    }
}