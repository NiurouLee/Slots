using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11026 : MachineContextBuilder
    {
        public MachineContextBuilder11026(string inMachineId)
            : base(inMachineId)
        {
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11026();
        }

        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11026(machineContext);
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11026>();
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState>();

            //Free
            machineState.Add<WheelState11026>();

            //Link1
            machineState.Add<WheelStateLeft11026>().SetResultIndex(0);
            ;
            //Link2
            machineState.Add<WheelStateCenter11026>().SetResultIndex(1);
            ;
            //Link3
            machineState.Add<WheelStateCenter11026>().SetResultIndex(1);
            ;
            //Link4
            machineState.Add<WheelStateCenter11026>().SetResultIndex(1);
            ;
            //Link5
            machineState.Add<WheelStateCenter11026>().SetResultIndex(1);
            ;
            //Link6
            machineState.Add<WheelStateRight11026>().SetResultIndex(2);
            ;


            machineState.Add<ExtraState11026>();
            machineState.Replace<ReSpinState, ReSpinState11026>();
        }

        public override void BindingBackgroundView(MachineContext machineContext)
        {
            var background = machineContext.transform.Find("Zhenping/Background");
            machineContext.view.Add<BackGroundView11001>(background);
        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelTransform = machineContext.transform.Find("Zhenping/Wheels");
            var wheelCount = wheelTransform.childCount;
            for (var i = 0; i < wheelCount; i++)
            {
                Transform tranWheel = wheelTransform.GetChild(i);
                Wheel wheel = null;
                if (tranWheel.name == "WheelLinkGame1")
                {
                    wheel = machineContext.view.Add<LinkWheel11206>(wheelTransform.GetChild(i));
                    wheel.BuildWheel<SoloRoll, ElementSupplier,
                        IndependentSpinningController<LinkWheelAnimationController11026>>(machineContext.state
                        .Get<WheelStateLeft11026>());
                    wheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
                else if (tranWheel.name == "WheelLinkGame2")
                {
                    wheel = machineContext.view.Add<LinkWheel11206>(wheelTransform.GetChild(i));
                    wheel.BuildWheel<SoloRoll, ElementSupplier,
                        IndependentSpinningController<LinkWheelAnimationController11026>>(
                        machineContext.state.Get<WheelStateCenter11026>(0));
                    wheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
                else if (tranWheel.name == "WheelLinkGame3")
                {
                    wheel = machineContext.view.Add<LinkWheel11206>(wheelTransform.GetChild(i));
                    wheel.BuildWheel<SoloRoll, ElementSupplier,
                        IndependentSpinningController<LinkWheelAnimationController11026>>(
                        machineContext.state.Get<WheelStateCenter11026>(1));
                    wheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
                else if (tranWheel.name == "WheelLinkGame4")
                {
                    wheel = machineContext.view.Add<LinkWheel11206>(wheelTransform.GetChild(i));
                    wheel.BuildWheel<SoloRoll, ElementSupplier,
                        IndependentSpinningController<LinkWheelAnimationController11026>>(
                        machineContext.state.Get<WheelStateCenter11026>(2));
                    wheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
                else if (tranWheel.name == "WheelLinkGame5")
                {
                    wheel = machineContext.view.Add<LinkWheel11206>(wheelTransform.GetChild(i));
                    wheel.BuildWheel<SoloRoll, ElementSupplier,
                        IndependentSpinningController<LinkWheelAnimationController11026>>(
                        machineContext.state.Get<WheelStateCenter11026>(3));
                    wheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
                else if (tranWheel.name == "WheelLinkGame6")
                {
                    wheel = machineContext.view.Add<LinkWheel11206>(wheelTransform.GetChild(i));
                    wheel.BuildWheel<SoloRoll, ElementSupplier,
                        IndependentSpinningController<LinkWheelAnimationController11026>>(machineContext.state
                        .Get<WheelStateRight11026>());
                    wheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
                else if (tranWheel.name == "WheelBaseGame")
                {
                    List<int> rowCountInEachRoll = new List<int> {3, 4, 4, 4, 3};
                    List<Vector3> roolPositions = new List<Vector3> { };
                    roolPositions = IrregularWheel.GetIrregularRollPosition(5, rowCountInEachRoll,
                        new Vector2(1.37f, 1.16f), IrregularWheel.ElementAlignType.AlignBottom);
                    var wheelBaseGameWheel = machineContext.view.Add<IrregularWheel>(wheelTransform.GetChild(i));
                    wheelBaseGameWheel.SetWheelExtraConfig(rowCountInEachRoll, roolPositions,
                        new Vector2(1.37f, 1.16f));
                    wheelBaseGameWheel
                        .BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11026>>(
                            machineContext.state.Get<WheelState>());
                    wheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
                else if (tranWheel.name == "WheelFreeGame")
                {
                    List<int> rowFreeCountInEachRoll = new List<int> {3, 4, 4, 4, 3};
                    List<Vector3> roolFreePositions = new List<Vector3> { };
                    roolFreePositions = IrregularWheel.GetIrregularRollPosition(5, rowFreeCountInEachRoll,
                        new Vector2(1.37f, 1.16f), IrregularWheel.ElementAlignType.AlignBottom);
                    var wheelFreeGameWheel = machineContext.view.Add<IrregularWheel>(wheelTransform.GetChild(i));
                    wheelFreeGameWheel.SetWheelExtraConfig(rowFreeCountInEachRoll, roolFreePositions,
                        new Vector2(1.37f, 1.16f));
                    wheelFreeGameWheel
                        .BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11026>>(
                            machineContext.state.Get<WheelState11026>());
                    wheelFreeGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11026>();
                }
            }
        }

        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("Zhenping/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var tranGroupTransitions = machineContext.transform.Find("Zhenping/Wheels/GroupTransitions");
            machineContext.view.Add<TransitionsView11026>(tranGroupTransitions);
            var tranLinkMultiplier = machineContext.transform.Find("Zhenping/Wheels/WheelLinkGame1/AllWins");
            machineContext.view.Add<LinkMultiplier11026>(tranLinkMultiplier);
            var tranLinkRowMore = machineContext.transform.Find("Zhenping/Wheels/WheelLinkGame1/MoreRow");
            machineContext.view.Add<LinkRowMore11026>(tranLinkRowMore);
            var bonusProgress = machineContext.transform.Find("Zhenping/Wheels/WheelBaseGame/BonusProgress");
            machineContext.view.Add<SuperFreeProgressView11026>(bonusProgress);
            var tranBackGroundView = machineContext.transform.Find("Zhenping/Background");
            machineContext.view.Add<BackGroundView11026>(tranBackGroundView);
            var tranFeatureGameTips = machineContext.transform.Find("Zhenping/Wheels/UILinkGame11026");
            machineContext.view.Add<FeatureGameTips11026>(tranFeatureGameTips);
            // var tranFeatureGameTips = machineContext.transform.Find("Wheels/Canvas/UILinkGame11026");
            //          machineContext.view.Add<FeatureGameTips11026>(tranFeatureGameTips); 
            var tranFreeGameTips = machineContext.transform.Find("Zhenping/Wheels/WheelFreeGame/BtnTipsBg");
            machineContext.view.Add<FreeGameTips11026>(tranFreeGameTips);
            var tranLinkMultiplierFinish =
                machineContext.transform.Find($"Zhenping/Wheels/WheelLinkGame1/AllwinsFinish");
            machineContext.view.Add<LinkMultiplierFinish11026>(tranLinkMultiplierFinish);
            var tranLinkLockItem = machineContext.transform.Find($"Zhenping/Wheels/WheelLinkGame1/Tips");
            machineContext.view.Add<LinkLockView11026>(tranLinkLockItem);
            var tranLinkAnimationNode = machineContext.transform.Find($"Zhenping/Wheels/WheelLinkGame1/animationNode");
            machineContext.view.Add<LinkAnimationNode11026>(tranLinkAnimationNode);
            // var bonusProgress = machineContext.transform.Find("Wheels/WheelBaseGame/spiningMask");
            // machineContext.view.Add<SuperFreeProgressView11026>(bonusProgress);
        }

        public override void AttachTopPanel(MachineContext machineContext)
        {
            base.AttachTopPanel(machineContext);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11026);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11026);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11026);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11026);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11026);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11026);
            logicProxy[LogicStepType.STEP_WIN_LINE_BLINK] = typeof(WinLineBlinkProxy11026);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11026);
            return logicProxy;
        }

        public override void AdaptMachineView(MachineContext machineContext)
        {
            var wheels = machineContext.transform.Find("Zhenping/Wheels");

            var uiTotalHeight = Constant11026.wheelDesignHeight + Constant11026.jackpotPanelHeight;
            var systemUIHeight = MachineConstant.controlPanelVHeight + MachineConstant.topPanelHeight;

            layoutHelper = new VerticalMachineLayoutHelper();
          //  layoutHelper.ModifyTopPanelHeight(58);
            
            layoutHelper.AddElement(wheels, Constant11026.wheelDesignHeight, 0.07f, 0.3f);
            
            var jackpotPanel = machineContext.view.Get<JackPotPanel>();
            
            layoutHelper.AddElement(jackpotPanel.transform, Constant11026.jackpotPanelHeight, 0.5f, 0.3f);
            
            layoutHelper.DoLayout();

            // var gameTotalHeight = systemUIHeight + uiTotalHeight;
            //
            // var deviceHeight = ViewResolution.referenceResolutionPortrait.y;
            //
            // int wheelsOffsetY = 35;
            // float scale = 1.0f;
            // if (gameTotalHeight <= deviceHeight)
            // {
            //     var deltaHeight = deviceHeight - gameTotalHeight;
            //
            //     if (deltaHeight > 75)
            //     {
            //         wheelsOffsetY = 35;
            //     }
            // }
            // else
            // {
            //     scale = 1 - (gameTotalHeight - deviceHeight) / uiTotalHeight;
            // }
            //
            // //Transform tranMan = machineContext.transform.Find("GroupTransitions");
            //
            // float offsetY = wheels.localPosition.y;
            //
            // wheels.localPosition = new Vector3(0,
            //     (-deviceHeight * 0.5f + MachineConstant.controlPanelVHeight + wheelsOffsetY) *
            //     MachineConstant.pixelPerUnitInv, 0);
            // wheels.localScale = new Vector3(scale, scale, scale);
            //
            // // offsetY = wheels.localPosition.y - offsetY;
            // // var posMan = tranMan.localPosition;
            // // posMan.y = posMan.y + offsetY;
            // // tranMan.localPosition = posMan;
            // // tranMan.localScale = new Vector3(scale, scale, scale);
            //
            // if (scale < 1)
            // {
            //     var jackpotPanel = machineContext.view.Get<JackPotPanel>();
            //     jackpotPanel.transform.localScale = new Vector3(scale, scale, scale);
            //
            //     var jackpotIdealPos =
            //         Constant11026.wheelDesignHeight * scale + MachineConstant.controlPanelVHeight +
            //         Constant11026.jackpotPanelHeight * scale * 0.5f;
            //
            //     jackpotPanel.transform.localPosition =
            //         new Vector3(0, (jackpotIdealPos - deviceHeight * 0.5f) * MachineConstant.pixelPerUnitInv, 0);
            // }
            // else
            // {
            //     float jackpotPanelStartPos = wheelsOffsetY +
            //                                  Constant11026.wheelDesignHeight + MachineConstant.controlPanelVHeight +
            //                                  Constant11026.jackpotPanelHeight;
            //
            //     var availableHeight = deviceHeight - jackpotPanelStartPos - MachineConstant.topPanelVHeight;
            //
            //     var jackpotOffset = Mathf.Min(50, availableHeight / 2);
            //
            //     // if(availableHeight > 120)
            //     // {
            //     //     jackpotPanelStartPos = jackpotPanelStartPos - Constant11019.jackpotPanelHeight * 0.5f + 40 - deviceHeight * 0.5f;
            //     //     
            //     //     machineContext.view.Get<JackPotPanel>().transform.localPosition =
            //     //         new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
            //     // }
            //     // else
            //     {
            //         jackpotPanelStartPos = (jackpotPanelStartPos + jackpotOffset) -
            //                                Constant11026.jackpotPanelHeight * 0.5f - deviceHeight * 0.5f;
            //         machineContext.view.Get<JackPotPanel>().transform.localPosition =
            //             new Vector3(0, jackpotPanelStartPos * MachineConstant.pixelPerUnitInv, 0);
            //     }
            // }
        }
    }
}