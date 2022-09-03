using System.Collections.Generic;
using System;
using UnityEngine;


namespace GameModule
{
    public class MachineContextBuilder11029 : MachineContextBuilder
    {
        public MachineContextBuilder11029(string inMachineId)
            : base(inMachineId)
        {
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11029();
        }


        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11029>();
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState>();
            //Free
            machineState.Add<WheelState>();
            //classic free
            machineState.Add<WheelState>();
            //Belle free
            machineState.Add<MagicBonusWheelState11029>();
            //Belle free
            machineState.Add<WheelState>();
            //Belle free
            machineState.Add<WheelState>();
            //Belle free
            machineState.Add<WheelState>();
            machineState.Add<ExtraState11029>();
        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelBaseGameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame");
            var wheelBaseGameWheel = machineContext.view.Add<WheelBase11029>(wheelBaseGameTrans);
            wheelBaseGameWheel.BuildWheel<Roll, ElementSupplier11029, WheelSpinningController11029<WheelAnimationController11029>>(
                machineContext.state.Get<WheelState>());
            wheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

            var wheelFreeGameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelFreeGame");
            var wheelFreeGameWheel = machineContext.view.Add<WheelBase11029>(wheelFreeGameTrans);
            wheelFreeGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(
                machineContext.state.Get<WheelState>(1));
            wheelFreeGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11029>();
            
            var wheelMiniGameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelMiniGame");
            var wheelMiniGameWheel = machineContext.view.Add<WheelBase11029>(wheelMiniGameTrans);
            wheelMiniGameWheel.BuildWheel<StepperRoll11029, ElementSupplier, WheelSpinningController<WheelAnimationController>>(
                machineContext.state.Get<WheelState>(2));
            wheelMiniGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11029>();

            var wheelBonusGameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelMagicBonusGame");
            var wheelBonusGameWheel = machineContext.view.Add<Wheel11029>(wheelBonusGameTrans);
            wheelBonusGameWheel.BuildWheel<Roll, ElementSupplier11029, WheelSpinningController11029<WheelAnimationController11029>>(
                machineContext.state.Get<MagicBonusWheelState11029>());
            wheelBonusGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11029>();

            var wheelPoint1GameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelBigPointGame1");
            var wheelPoint1GameWheel = machineContext.view.Add<WheelBase11029>(wheelPoint1GameTrans);
            wheelPoint1GameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(
                machineContext.state.Get<WheelState>(4));
            wheelPoint1GameWheel.SetUpWinLineAnimationController<WinLineAnimationController11029>();

            var wheelPoint2GameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelBigPointGame2");
            var wheelPoint2GameWheel = machineContext.view.Add<WheelBase11029>(wheelPoint2GameTrans);
            wheelPoint2GameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(
                machineContext.state.Get<WheelState>(5));
            wheelPoint2GameWheel.SetUpWinLineAnimationController<WinLineAnimationController11029>();

            var wheelPoint3GameTrans = machineContext.transform.Find("WheelFeature/Wheels/WheelBigPointGame3");
            var wheelPoint3GameWheel = machineContext.view.Add<WheelBase11029>(wheelPoint3GameTrans);
            wheelPoint3GameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController>>(
                machineContext.state.Get<WheelState>(6));
            wheelPoint3GameWheel.SetUpWinLineAnimationController<WinLineAnimationController11029>();
        }

        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("JackpotPanel");
            machineContext.view.Add<JackpotPanel11029>(jackpotTran);
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var tranProgressBar = machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame/ProgressBar");
            machineContext.view.Add<ProgressBar11029>(tranProgressBar);
            var tranWheelFeature = machineContext.transform.Find("WheelFeature/Wheels/WheelBonusStartNotice");
            machineContext.view.Add<WheelFeature11029>(tranWheelFeature);
            var tranWheelBonus = machineContext.transform.Find("WheelFeature/Wheels/WheelBonusView");
            machineContext.view.Add<WheelBonus11029>(tranWheelBonus);
            var tranMoneyBag = machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame/CollectionGroup");
            machineContext.view.Add<MoneyBag11029>(tranMoneyBag);
            var tranBonusTop = machineContext.transform.Find("WheelFeature/Wheels/CollectionW11FinishNotice");
            machineContext.view.Add<BonusClollectionPop11029>(tranBonusTop);
            var tranBackGround = machineContext.transform.Find("Background");
            machineContext.view.Add<BackGroundView11029>(tranBackGround);
            var tranMeiDuSha = machineContext.transform.Find("WheelFeature/Wheels/Meidusha");
            machineContext.view.Add<MeiDuSha11029>(tranMeiDuSha);
            var tranGroupTransitions = machineContext.transform.Find("WheelFeature/GroupTransitions");
			machineContext.view.Add<TransitionsView11029>(tranGroupTransitions);
            var tranLight = machineContext.transform.Find("WheelFeature/Wheels/WheelMagicBonusGame/BGGroup/Light");
            machineContext.view.Add<LightView11029>(tranLight);
            var tranHighLight = machineContext.transform.Find("WheelFeature/Wheels/WheelMagicBonusGame/Fx_MagicBonusGame");
            machineContext.view.Add<HighLightView11029>(tranHighLight);
            var tranBgLight = machineContext.transform.Find("WheelFeature/Wheels/WheelMagicBonusGame/Fx_Trail");
            machineContext.view.Add<BgLightView11029>(tranBgLight);
            var tranLightBase = machineContext.transform.Find("WheelFeature/Wheels/WheelBaseGame/Fx_BaseGame");
            machineContext.view.Add<LightBaseView11029>(tranLightBase);
            var tranMiniGame = machineContext.transform.Find("WheelFeature/Wheels/WheelMiniGame/BGGroup/PaytableGroup");
            machineContext.view.Add<MiniGameView11029>(tranMiniGame);
        }

        public override void AttachTopPanel(MachineContext machineContext)
        {
            base.AttachTopPanel(machineContext);
        }
        
        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11029(machineContext);
        }
        
         public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("WheelFeature/Wheels");

            var uiTotalHeight = Constant11029.wheelDesignHeight + Constant11029.jackpotPanelHeight;
            var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;

            layoutHelper = new VerticalMachineLayoutHelper();

            layoutHelper.AddElement(wheels, Constant11029.wheelDesignHeight, 0.26f, 0.3f);
            
            var jackpotPanel = machineContext.view.Get<JackPotPanel>();
            
            layoutHelper.AddElement(jackpotPanel.transform, Constant11029.jackpotPanelHeight, 0.45f, 0.3f);
            
            layoutHelper.DoLayout();
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11029);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11029);
             logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11029);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11029);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11029);
            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11029);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11029);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11029);
            return logicProxy;
        }
    }
}