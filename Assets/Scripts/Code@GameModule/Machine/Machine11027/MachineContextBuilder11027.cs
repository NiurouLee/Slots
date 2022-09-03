using System.Collections.Generic;
using System;
using UnityEngine;


namespace GameModule
{
    public class MachineContextBuilder11027 : MachineContextBuilder
    {
        public MachineContextBuilder11027(string inMachineId)
        :base(inMachineId)
        {

    
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11027();
        }
        
        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11027>();
        }
        
        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState11027>();
            machineState.Add<ExtraState11027>();
        }
        
        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelBaseGameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame");
			var wheelBaseGameWheel = machineContext.view.Add<Wheel11027>(wheelBaseGameTrans);
			wheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController11027<WheelAnimationController11027>>(machineContext.state.Get<WheelState11027>());
            wheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11027>();
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("WheelFeature/JackpotPanel");
            machineContext.view.Add<JackpotPanel11027>(jackpotTran);
        }
        
        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var tranCollectionGroup = machineContext.transform.Find("WheelFeature/Wheels/CollectionGroup");
            machineContext.view.Add<CollectionGroup11027>(tranCollectionGroup);
            var tranPickGame = machineContext.transform.Find("WheelFeature/Wheels/WheelPickGame");
            machineContext.view.Add<PickGame11027>(tranPickGame);
            var tranWheelRollingGame = machineContext.transform.Find("WheelFeature/WheelBonusGame02");
            machineContext.view.Add<WheelRollingView11027>(tranWheelRollingGame);
            var tranTouchToSpin = machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame/TouchToSpinWheelNotice");
            machineContext.view.Add<TouchToSpin11027>(tranTouchToSpin);
            var tranWheelFeature = machineContext.transform.Find("WheelFeature");
            machineContext.view.Add<WheelFeature11027>(tranWheelFeature);
            var pickTrans = machineContext.transform.Find("WheelFeature/TransitionCutOutPick");
            machineContext.view.Add<TransitionPickView11027>(pickTrans).Initialize(machineContext);
            machineContext.view.Get<TransitionPickView11027>().Hide();
            var wheelTrans = machineContext.transform.Find("WheelFeature/TransitionCutOut");
            machineContext.view.Add<TransitionWheelView11027>(wheelTrans).Initialize(machineContext);
            machineContext.view.Get<TransitionWheelView11027>().Hide();
            var revealTrans = machineContext.transform.Find("WheelFeature/Reveal");
            machineContext.view.Add<TransitionRevealView11027>(revealTrans).Initialize(machineContext);
            machineContext.view.Get<TransitionRevealView11027>().Hide();
        }
        public override void AttachTopPanel(MachineContext machineContext)
        {
            base.AttachTopPanel(machineContext);
        }
        protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11027);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11027);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11027);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11027);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11027);
            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11027);
            return logicProxy;
        }
        
        public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("WheelFeature");
            float scale = 1.0f;
            var uiTotalHeight = Constant11027.wheelDesignHeight + Constant11027.jackpotPanelHeight+Constant11027.wheelPanelHeight;
            var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;
            var deviceHeight = ViewResolution.referenceResolutionPortrait.y;
            var gameTotalHeight = systemUIHeight + uiTotalHeight;
            if (gameTotalHeight <= deviceHeight)
            {
                var deltaHeight = deviceHeight - gameTotalHeight;
            }
            else
            {
                scale = 1 - (gameTotalHeight - deviceHeight) / uiTotalHeight;
            }
            
            if (scale < 1)
            {
                 wheels.localScale = new Vector3(scale, scale, scale);
                // var wheels = machineContext.transform.Find("WheelFeature/Wheels");
                //
                // layoutHelper = new VerticalMachineLayoutHelper();
                //
                // layoutHelper.AddElement(wheels, Constant11027.wheelDesignHeight, 0.47f, 0.3f);
                //
                // var jackpotPanel = machineContext.view.Get<JackPotPanel>();
                //
                // layoutHelper.AddElement(jackpotPanel.transform, Constant11027.jackpotPanelHeight, 0.85f, 0.3f);
                //
                // var wheelPanel = machineContext.transform.Find("WheelFeature/WheelBonusGame02");
                //
                // layoutHelper.AddElement(wheelPanel, Constant11027.wheelPanelHeight, 0.95f, 0.8f);
                //
                // layoutHelper.DoLayout();
            }
        }
    }
}