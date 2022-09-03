using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11006 : MachineContextBuilder
    {
        public MachineContextBuilder11006(string inMachineId) : base(inMachineId)
        {
        }


        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11006(machineContext);
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11006();
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            machineState.Add<WheelState11006>();
            machineState.Add<ExtraState11006>();
            machineState.Add<WheelsActiveState11006>();
        }
        
        
        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelsTransform = machineContext.transform.Find("ZhenpingAnim/Wheels");
            var wheelCount = wheelsTransform.childCount;

            var wheel = machineContext.view.Add<Wheel>(wheelsTransform.Find("Wheel"));
            wheel.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>());
            wheel.SetUpWinLineAnimationController<WinLineAnimationController>();
            
        }
        
        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var tranBase = machineContext.transform.Find("ZhenpingAnim/Wheels/BaseGameInfomation");
            var tranFree = machineContext.transform.Find("ZhenpingAnim/Wheels/FreeGameInfomation");
            var tranMultiplier = machineContext.transform.Find("ZhenpingAnim/Wheels/MultiplierNotice");
            
            machineContext.view.Add<BaseGameInfomationView11006>(tranBase);
            machineContext.view.Add<FreeGameInfomationView11006>(tranFree);
            machineContext.view.Add<MultiplierNoticeView11006>(tranMultiplier);
        }


        // public override IMachineAudioConfig SetUpCommonAudioConfig()
        // {
        //     var config = base.SetUpCommonAudioConfig();
        //     config.freeMusicName = "Bg_FreeGame_WildBuffalo_Sample01";
        //
        //     return config;
        // }

        public override void AttachControlPanel(MachineContext machineContext)
        {
            base.AttachControlPanel(machineContext);
            machineContext.view.Get<ControlPanel>().SetNiceWinAnimationToOnlyFx();
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.75f,0.8f);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();

            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11006);
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11006);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11006);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11006);
            logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11006);
            logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11006);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11006);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11006);
            return logicProxy;
        }

    }
}