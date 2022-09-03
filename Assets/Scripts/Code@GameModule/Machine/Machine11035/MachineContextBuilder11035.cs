using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class MachineContextBuilder11035 : MachineContextBuilder
    {
        public MachineContextBuilder11035(string inMachineId)
        : base(inMachineId)
        {
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11035();
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            machineState.Add<WheelState>();
            machineState.Add<WheelState>().SetResultIndex(1);
            machineState.Add<ExtraState11035>();
            machineState.Replace<WinState, WinState11035>();
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11035>();
        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelTrans = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelBaseGame");
            var wheel = machineContext.view.Add<Wheel>(wheelTrans);
            wheel.BuildWheel<Roll11035, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>());
            // wheel.SetUpWinLineAnimationController<WinLineAnimationController11035>();
            wheel.SetUpWinLineAnimationController<WinLineAnimationController>();

            var wheelJackpotTrans = machineContext.transform.Find("ZhenpingAnim/Wheels/WheelJackpotGame");
            var wheelJackpot = machineContext.view.Add<Wheel>(wheelJackpotTrans);
            wheelJackpot.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(1));
            wheelJackpot.SetUpWinLineAnimationController<WinLineAnimationController11035>();
            // wheelJackpot.SetUpWinLineAnimationController<WinLineAnimationController>();
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);

            var combo = machineContext.transform.Find("ZhenpingAnim/Wheels/JackpotProgressBar");
            machineContext.view.Add<UIComboView>(combo);

            var comboNotice = machineContext.transform.Find("ZhenpingAnim/Wheels/BetNotice");
            machineContext.view.Add<UIComboNoticeView>(comboNotice);

            var jackpotNotice = machineContext.transform.Find("ZhenpingAnim/Wheels/FireJackpotNotice");
            var jackpotNoticeView = machineContext.view.Add<UIFireJackpotNoticeView>(jackpotNotice);
            jackpotNoticeView.Hide();

            var transformTransition = machineContext.transform.Find("ZhenpingAnim/Wheels/Transition");
            machineContext.view.Add<UITransitionView11035>(transformTransition);

            var transformLineGroup = machineContext.transform.Find("ZhenpingAnim/Wheels/MachineLineGroup");
            machineContext.view.Add<UIMachineLineGroupView11035>(transformLineGroup);
        }

        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("ZhenpingAnim/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11035);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11035);
            logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11035);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(ReSpinProxy11035);
            logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11035);
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11035);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11035);
            logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11035);
            // logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11035);
            return logicProxy;
        }

        public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("ZhenpingAnim/Wheels");

            layoutHelper = new VerticalMachineLayoutHelper();
            layoutHelper.AddElement(wheels, 738, 0.32f, 0.3f);
            var jackpotPanel = machineContext.view.Get<JackPotPanel>();
            layoutHelper.AddElement(jackpotPanel.transform, 240, 0.62f, 0.3f);
            layoutHelper.DoLayout();
            // var charater = machineContext.view.Get<CharacterView11002>();
            // charater.transform.localScale = wheels.transform.localScale;
        }
    }
}