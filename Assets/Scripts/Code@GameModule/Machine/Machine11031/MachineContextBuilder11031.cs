using System.Collections.Generic;
using System;
using UnityEngine;


namespace GameModule
{
    public class MachineContextBuilder11031 : MachineContextBuilder
    {
        public MachineContextBuilder11031(string inMachineId)
            : base(inMachineId)
        {
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11031>();
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11031();
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState>();
            //Link
            machineState.Add<LinkWheelState11031>().SetStaticResultIndex(0);
            //Link1,2,3,4
            machineState.Add<LinkWheelState11031>().SetStaticResultIndex(0);
            machineState.Add<LinkWheelState11031>().SetStaticResultIndex(1);
            machineState.Add<LinkWheelState11031>().SetStaticResultIndex(2);
            machineState.Add<LinkWheelState11031>().SetStaticResultIndex(3);
            machineState.Add<ExtraState11031>();
            machineState.Replace<ReSpinState, ReSpinState11031>();
             machineState.Replace<WinState, WinState11031>();
        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
            var wheelBaseGameWheel = machineContext.view.Add<Wheel>(wheelBaseGameTrans);
            wheelBaseGameWheel
                .BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11031>>(
                    machineContext.state.Get<WheelState>());
            wheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11031>();

            for (var i = 0; i <= 4; i++)
            {
                string transformName = "Wheels/WheeRespinGame" + ((i == 0) ? "" : i.ToString());
                var wheelLinkGameTrans = machineContext.transform.Find(transformName);
                var wheelLinkGameWheel = machineContext.view.Add<LinkWheel11031>(wheelLinkGameTrans);
                wheelLinkGameWheel
                    .BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11031>>(
                        machineContext.state.Get<LinkWheelState11031>(i));
                wheelLinkGameWheel.SetUpWinLineAnimationController<WinLineAnimationController11031>();

                wheelLinkGameWheel.AttachLockLayer();
                if (i > 0)
                {
                    wheelLinkGameWheel.UpdateElementContainerSize(0.5f);
                    wheelLinkGameWheel.SetWheelIndex(i - 1);
                }
            }
        }

        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("JackpotPanel");
            machineContext.view.Add<JackpotPanel11031>(jackpotTran);
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var tranWinGroup = machineContext.transform.Find("Wheels/WinGroup");
            machineContext.view.Add<WinGroupView11031>(tranWinGroup);
            var tranLinkFeature = machineContext.transform.Find("Wheels/WheelBaseGame/RespinStart");
            machineContext.view.Add<LinkFeatureView11031>(tranLinkFeature);
            var tranWinGroupFeature = machineContext.transform.Find("Wheels/ChiliRespinTips");
            machineContext.view.Add<WinGroupFeature11031>(tranWinGroupFeature);
            var tranSuperLinkFeature = machineContext.transform.Find("Wheels/SuperRespin");
            machineContext.view.Add<SuperLinkView11031>(tranSuperLinkFeature);
            var tranCollectGroup = machineContext.transform.Find("JackpotPanel/CollectGroup");
            machineContext.view.Add<CollectGroupView11031>(tranCollectGroup);
            var tranjackpotGrand = machineContext.transform.Find("Wheels/UIJackpotGrand11031");
            machineContext.view.Add<UIJackpotGrandView11031>(tranjackpotGrand);
            var tranjackpotMajor = machineContext.transform.Find("Wheels/UIJackpotMajor11031");
            machineContext.view.Add<UIJackpotMajorView11031>(tranjackpotMajor);
            var tranjackpotMini = machineContext.transform.Find("Wheels/UIJackpotMini11031");
            machineContext.view.Add<UIJackpotMiniView11031>(tranjackpotMini);
            var tranjackpotMinor = machineContext.transform.Find("Wheels/UIJackpotMinor11031");
            machineContext.view.Add<UIJackpotMInorView11031>(tranjackpotMinor);
            var tranjackpotUltra = machineContext.transform.Find("Wheels/UIJackpotUltra11031");
            machineContext.view.Add<UIJackpotUltraView11031>(tranjackpotUltra);
            var tranLinkRemaining = machineContext.transform.Find("JackpotPanel/RespinNums");
            machineContext.view.Add<LinkRemaining11031>(tranLinkRemaining);
            var tranActiveLight = machineContext.transform.Find("Wheels/ActiveRespinEFX");
            machineContext.view.Add<LightView11031>(tranActiveLight);
            var tranLuckyPot = machineContext.transform.Find("LuckyPotRoot");
            machineContext.view.Add<LuckyPot11031>(tranLuckyPot);
            var tranGroupTransitions = machineContext.transform.Find("GroupTransitions");
            machineContext.view.Add<TransitionsView11031>(tranGroupTransitions);
            var tranBowlView = machineContext.transform.Find("Bowl");
            machineContext.view.Add<BowlView11031>(tranBowlView);
            var tranBackGroundView = machineContext.transform.Find("Background");
            machineContext.view.Add<BackGroundView11031>(tranBackGroundView);
            var LineBackLineView = machineContext.transform.Find("Wheels/BGGroup");
            machineContext.view.Add<LinkBackLineView11031>(LineBackLineView);
        }

        public override void AttachTopPanel(MachineContext machineContext)
        {
            base.AttachTopPanel(machineContext);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11031);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11031);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11031);
            logicProxy[LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE] = typeof(ControlPanelWinUpdateProxy11031);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11031);
            logicProxy[LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT] = typeof(EarlyHighLevelWinEffectProxy11031);
            logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11031);
            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11031);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11031);
            return logicProxy;
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.84f, 0.84f);
        }

        public override void AdaptMachineView(MachineContext machineContext)
        {
            //base.AdaptMachineView(machineContext);
            if (!Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
            {
                float x = Mathf.Clamp(ViewResolution.referenceResolutionLandscape.x, 1024, ViewResolution.designSize.x);
                float lerp = (x - 1024f) / (ViewResolution.designSize.x - 1024f);
                Vector2 padScaler = GetPadMinXScaler();
                float lerpScalerX = Mathf.Lerp(padScaler.x, 1, lerp);
                float lerpScalerY = Mathf.Lerp(padScaler.y, 1, lerp);

                var wheels = machineContext.transform.GetComponentInChildren<SimpleRollUpdaterEasingScriptable>()
                    ?.transform;
                Vector3 lerpScalerVec = new Vector3(lerpScalerX, lerpScalerY, 1);

                if (wheels == null)
                {
                    wheels = machineContext.transform.Find("Wheels");
                }

                if (wheels == null)
                {
                    wheels = machineContext.transform.Find("ZhenpingAnim/Wheels");
                }

                
                var tranJackpotPanel = wheels.transform.parent.Find("JackpotPanel");
                if (tranJackpotPanel == null)
                {
                    tranJackpotPanel = wheels.transform.parent.Find("ZhenpingAnim/JackpotPanel");
                }

                Transform parentJackpot = null;
                if (tranJackpotPanel != null)
                {
                    parentJackpot = tranJackpotPanel.parent;
                    tranJackpotPanel.parent = wheels;
                }

                var doorView = wheels.transform.parent.Find("LockElementLayer");
                if (doorView != null)
                {
                    doorView.parent = wheels;

                }


                var luckyPotTrf = machineContext.transform.Find("LuckyPotRoot");
                if (luckyPotTrf!=null)
                {
                    luckyPotTrf.parent = wheels;
                }


                var bowIposTrf = machineContext.transform.Find("BowlPositionNode");
                if (bowIposTrf!=null)
                {
                    bowIposTrf.parent = wheels;
                }


                var bowlTrf = machineContext.transform.Find("Bowl");
                if (bowlTrf!=null)
                {
                    bowlTrf.parent = wheels;
                }
                
                if (lerpScalerX <= 0.84f)
                {
                    machineContext.transform.Find("Wheels").transform.localPosition = new Vector3(0, -0.2f, 0);
                    // machineContext.transform.Find("Background/Title").gameObject.SetActive(true);
                }
                
                wheels.localScale = lerpScalerVec;

                if (tranJackpotPanel != null)
                {
                    tranJackpotPanel.parent = parentJackpot;
                }

                if (doorView != null)
                {
                    doorView.parent = parentJackpot;
                }

                if (luckyPotTrf!=null)
                {
                    var scaleMul = luckyPotTrf.localScale.x;
                    luckyPotTrf.parent = parentJackpot;

                    if (luckyPotTrf.localScale != Vector3.one)
                    {
                        luckyPotTrf.localScale = new Vector3(0.84f, 0.84f, 0.84f);
                    }
                }

                if (bowIposTrf != null)
                {
                    bowIposTrf.parent = parentJackpot;
                }
                if (bowlTrf!=null)
                {
                    bowlTrf.parent = parentJackpot;
                }
            }

        }
    }
}