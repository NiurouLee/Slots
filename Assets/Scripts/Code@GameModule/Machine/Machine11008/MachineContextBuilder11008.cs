using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class MachineContextBuilder11008 : MachineContextBuilder
    {
        public MachineContextBuilder11008(string inMachineId):base(inMachineId)
        {

        }
        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11008>();
        }
        public override void SetUpMachineState(MachineState machineState)
        {
            machineState.Add<WheelState>();
            machineState.Add<WheelState>();
            machineState.Add<ExtraState11008>();
        }

        public override void SetUpCommonMachineState(MachineState machineState)
        {
            machineState.Add<BetState>();
            machineState.Add<AdStrategyState>();
            machineState.Add<FreeSpinState>();
            machineState.Add<AutoSpinState>();
            machineState.Add<ReSpinState>();
            machineState.Add<WinState>();
            machineState.Add<JackpotInfoState>();

            SetUpWheelActiveState(machineState);
        }
        public override void BindingWheelView(MachineContext machineContext)
        {
            var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
            var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
            baseWheel.BuildWheel<Roll11008, ElementSupplier, WheelSpinningController<WheelAnimationController11008>>(machineContext.state.Get<WheelState>());
            baseWheel.SetUpWinLineAnimationController<WinLineAnimationController11008>();
            
            var wheelTransform = machineContext.transform.Find("Wheels/WheelFreeGame");
            var freeWheel = machineContext.view.Add<Wheel>(wheelTransform);
            freeWheel.BuildWheel<Roll11008, ElementSupplier,WheelSpinningController<WheelAnimationController11008>>(machineContext.state.Get<WheelState>(1));
            freeWheel.SetUpWinLineAnimationController<WinLineAnimationController11008>();

        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            var Background = machineContext.transform.Find("Background");
            machineContext.view.Add<BgView11008>(Background);
            var WheelBaseGame = machineContext.transform.Find("Wheels/WheelBaseGame");
            machineContext.view.Add<WheelBaseGame11008>(WheelBaseGame);
            var WheelFreeGame = machineContext.transform.Find("Wheels/WheelFreeGame");
            machineContext.view.Add<WheelFreeGame11008>(WheelFreeGame);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11008);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11008);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11008);

            return logicProxy;
        }
        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.78f, 0.78f);
        }
        public override void AdaptMachineView(MachineContext machineContext)
        {
            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
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
                wheels.localScale = lerpScalerVec;

            }
        }
    }

}

