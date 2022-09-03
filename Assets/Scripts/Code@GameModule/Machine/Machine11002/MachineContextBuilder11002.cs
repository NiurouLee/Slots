// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:45 AM
// Ver : 1.0.0
// Description : MachineBuilder.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11002 : MachineContextBuilder
    {
        public MachineContextBuilder11002(string inMachineId)
        : base(inMachineId)
        {
        }

        public override Vector2 GetPadMinXScaler()
        {
            return Vector2.one * 0.65f;
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //   assetProvider.GetAsset<TextAsset>("ElementConfig");
            machineState.Add<WheelState11002>();
            machineState.Add<WheelState11002>().SetResultIndex(0);
            machineState.Add<ExtraState11002>();
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11002>();
        }

        public override void AttachExtraView(MachineContext machineContext)
        {
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11002();
        }

        public override void BindingBackgroundView(MachineContext machineContext)
        {

        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelTransform = machineContext.transform.Find("ZhenpingAnim/Wheels/Wheel0");
            var wheel = machineContext.view.Add<Wheel>(wheelTransform);
            wheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(0));
            wheel.SetUpWinLineAnimationController<WinLineAnimationController11002>();

            var wheelFreeTransform = machineContext.transform.Find("ZhenpingAnim/Wheels/Wheel1");
            var wheelFree = machineContext.view.Add<Wheel>(wheelFreeTransform);
            wheelFree.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(1));
            wheelFree.SetUpWinLineAnimationController<WinLineAnimationController11002>();

            var tranCharacter = machineContext.transform.Find("ZhenpingAnim/Background/Character");
            machineContext.view.Add<CharacterView11002>(tranCharacter);
        }

        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("ZhenpingAnim/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            var chillViewTran = machineContext.transform.Find("ZhenpingAnim/Wheels/Wheel0");
            machineContext.view.Add<ChillView>(chillViewTran);

            var chillViewFreeTran = machineContext.transform.Find("ZhenpingAnim/Wheels/Wheel1");
            machineContext.view.Add<ChillView>(chillViewFreeTran);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();

            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11002);
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11002);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11002);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11002);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11002);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11002);
            logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11002);
            logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11002);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11002);
            return logicProxy;
        }

        public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("ZhenpingAnim/Wheels");

            layoutHelper = new VerticalMachineLayoutHelper();
            layoutHelper.AddElement(wheels, 738, 0.32f, 0.3f);
            var jackpotPanel = machineContext.view.Get<JackPotPanel>();
            layoutHelper.AddElement(jackpotPanel.transform, 240, -0.13f, 0.3f);
            layoutHelper.DoLayout();
            var charater = machineContext.view.Get<CharacterView11002>();
            charater.transform.localScale = wheels.transform.localScale;
        }
    }
}
