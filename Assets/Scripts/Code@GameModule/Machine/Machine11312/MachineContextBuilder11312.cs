using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule
{
    public class MachineContextBuilder11312 : MachineContextBuilder
    {
        public MachineContextBuilder11312(string inMachineId)
            : base(inMachineId)
        {
            machineId = inMachineId;
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            //Base -- 有几个轮盘配置几个wheelState
            machineState.Add<WheelState>();

            //Free -- 有几个轮盘配置几个wheelState
            machineState.Add<WheelState>();

            //link -- 
            machineState.Add<LinkWheelState11312>();
            //smallWheel
            machineState.Add<LinkWheelState11312>();

            machineState.Add<ExtraState11312>();
            machineState.Replace<ReSpinState, ReSpinState11312>();
        }
        
        public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor11312(machineContext);
        }

        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11312>();
        }
        public override void AttachControlPanel(MachineContext machineContext)
        {
            var address = "ControlPanel";

            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
                address += "V";

            var controlPanelGameObject = machineContext.assetProvider.InstantiateGameObject(address);
            controlPanelGameObject.transform.SetParent(machineContext.MachineUICanvasTransform, false);
            machineContext.view.Add<ControlPanel11312>(controlPanelGameObject.transform);
        }
        // 几个轮盘配几个控制器转轴
        public override void BindingWheelView(MachineContext machineContext)
        {
            var ZhenpingAnim = machineContext.transform.Find("ZhenpingAnim");
            var wheelBase = ZhenpingAnim.transform.Find("checkerboard/Wheels/WheelBaseGame");
            var wheel = machineContext.view.Add<Wheel>(wheelBase);
            wheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11312>>(machineContext.state.Get<WheelState>());
            wheel.SetUpWinLineAnimationController<WinLineAnimationController11312>();

            var wheelFree = ZhenpingAnim.transform.Find("checkerboard/Wheels/WheelFreeGame");
            var wheelF = machineContext.view.Add<Wheel>(wheelFree);
            wheelF.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11312>>(machineContext.state.Get<WheelState>(1));
            wheelF.SetUpWinLineAnimationController<WinLineAnimationController11312>();

            var wheelLink = ZhenpingAnim.transform.Find("checkerboard/Wheels/WheelLinkGame");
            var wheelL = machineContext.view.Add<IndependentWheel>(wheelLink);
            wheelL.BuildWheel<LinkRoll11312, ElementSupplier, IndependentSpinningController<WheelAnimationController11312>>(machineContext.state.Get<LinkWheelState11312>());
            wheelL.SetUpWinLineAnimationController<WinLineAnimationController>();
            wheelL.wheelMask.gameObject.SetActive(true);
            wheelL.wheelMask.GetComponent<SpriteMask>().enabled = false;

            var WheelSmallGame = ZhenpingAnim.transform.Find("checkerboard/Wheels/WheelLinkGame/SpinRemainingGroup/FeatureGroupR/WheelSmallGame");
            var wheelS = machineContext.view.Add<LinkWheel11312>(WheelSmallGame);
            wheelS.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11312>>(machineContext.state.Get<LinkWheelState11312>(1));
            wheelS.SetUpWinLineAnimationController<WinLineAnimationController>();
            wheelS.SetActive(true);
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11312();
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var ZhenpingAnim = machineContext.transform.Find("ZhenpingAnim");
            var FeatureView = ZhenpingAnim.transform.Find("checkerboard/Wheels/FeatureView");
            machineContext.view.Add<FeatureView11312>(FeatureView);
            var FreeFeatureView = ZhenpingAnim.transform.Find("checkerboard/Wheels/FreeFeatureView");
            machineContext.view.Add<FreeFeatureView11312>(FreeFeatureView);

            var FreeWheelRowFx = ZhenpingAnim.transform.Find("checkerboard/Wheels/WheelFreeGame/FreeWheelRowFx");
            machineContext.view.Add<FreeWheelRowFx11312>(FreeWheelRowFx);

            var WheelLinkGameView = ZhenpingAnim.transform.Find("checkerboard/Wheels/WheelLinkGame");
            machineContext.view.Add<WheelLinkGameView11312>(WheelLinkGameView);

            var Background = ZhenpingAnim.transform.Find("bg");
            machineContext.view.Add<Background11312>(Background);
        }
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var ZhenpingAnim = machineContext.transform.Find("ZhenpingAnim");
            var jackpotTran = ZhenpingAnim.transform.Find("checkerboard/Wheels/JackpotPanel");
            machineContext.view.Add<JackpotView11312>(jackpotTran);
        }
        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11312);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11312);
            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11312);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11312);
            logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11312);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11312);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11312);
            return logicProxy;
        }

        /// <summary>
        /// 绑定wheelmask点击回调
        /// </summary>
        /// <param name="machineContext"></param>
        // public override void BindingWheelMaskClick(MachineContext machineContext)
        // {
        //     var wheels = machineContext.transform.Find("ZhenpingAnim/checkerboard/Wheels");
        //     if (wheels == null)
        //     {
        //         for (int i = 0; i < machineContext.transform.childCount; i++)
        //         {
        //             if (machineContext.transform.GetChild(i).name.Contains("Wheel"))
        //             {
        //                 wheels = machineContext.transform.GetChild(i).Find("Wheels");
        //                 if (wheels)
        //                 {
        //                     break;
        //                 }
        //             }
        //         }
        //     }
        //     for (int i = 0; i < wheels.childCount; i++)
        //     {
        //         var curChild = wheels.GetChild(i);
        //         var wheelConfigScrtpt = curChild.transform.GetComponent<WheelConfigScriptable>();
        //         if(wheelConfigScrtpt!=null){
        //             XDebug.Log("wheelsChildName:"+curChild.transform.name);
        //             var wheelMask = curChild.transform.Find("WheelMask");
        //             if(wheelMask!=null){
        //                 wheelMask.Bind<BoxCollider2D>(true);
        //                 var pointerEventCustomHandler = wheelMask.Bind<PointerEventCustomHandler>(true);
        //                 pointerEventCustomHandler.BindingPointerClick((data)=>{
        //                     if(machineContext.state.Get<AutoSpinState>().IsAutoSpin)
        //                         return;
        //                     var controlPanel = machineContext.view.Get<ControlPanel>();
        //                     if(controlPanel.SpinBtnIsShow())
        //                     {
        //                         machineContext.view.Get<ControlPanel>().OnWheelClick();
        //                         machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_SPIN);
        //                         AudioUtil.Instance.PlayAudioFxOneShot("spin_generic");
        //                     }else if(controlPanel.StopBtnIsShow()){
        //                         machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_STOP);
        //                         AudioUtil.Instance.PlayAudioFxOneShot("spin_generic");
        //                     }
        //                 });
        //             }
        //         }
        //     }
        // }


        public override void AdaptMachineView(MachineContext machineContext)
        {
            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
            {
                var aspectRatio = ViewResolution.referenceResolutionPortrait.y / ViewResolution.referenceResolutionPortrait.x;
                //   var designAspectRatio = GetDesignedAspectRatio();
                //   var machineScaleFactor = CaculateMachineScaleFactor(designAspectRatio, aspectRatio);
                //   machineContext.MachineScaleFactor = machineScaleFactor;
                //   machineContext.transform.localScale = new Vector3(machineScaleFactor, machineScaleFactor, machineScaleFactor);
            }
            else
            {
                float x = Mathf.Clamp(ViewResolution.referenceResolutionLandscape.x, 1024, ViewResolution.designSize.x);
                float lerp = (x - 1024f) / (ViewResolution.designSize.x - 1024f);
                Vector2 padScaler = GetPadMinXScaler();
                float lerpScalerX = Mathf.Lerp(padScaler.x, 1, lerp);
                float lerpScalerY = Mathf.Lerp(padScaler.y, 1, lerp);

                var wheels = machineContext.transform.GetComponentInChildren<SimpleRollUpdaterEasingScriptable>()?.transform;
                Vector3 lerpScalerVec = new Vector3(lerpScalerX, lerpScalerY, 1);

                if (wheels == null)
                {
                    wheels = machineContext.transform.Find("Wheels");
                }
                if (wheels == null)
                {
                    wheels = machineContext.transform.Find("ZhenpingAnim/checkerboard/Wheels");
                }

                var tranJackpotPanel = wheels.transform.parent.Find("JackpotPanel");
                if (tranJackpotPanel == null)
                {
                    tranJackpotPanel = wheels.transform.parent.Find("ZhenpingAnim/checkerboard/JackpotPanel");
                }

                Transform parentJackpot = null;
                if (tranJackpotPanel != null)
                {
                    parentJackpot = tranJackpotPanel.parent;
                    tranJackpotPanel.parent = wheels;
                }


                wheels.localScale = lerpScalerVec;

                if (tranJackpotPanel != null)
                {
                    tranJackpotPanel.parent = parentJackpot;
                }
            }
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.85f, 0.85f);
        }
    }
}

