using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11020 : MachineContextBuilder
    {
        public MachineContextBuilder11020(string inMachineId) 
			: base(inMachineId)
        {
            
        }
        
        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<SuperBonusInfoState11020>();
            machineState.Add<WheelsActiveState11020>();
        }
        
        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState>();
            
            //Free
            machineState.Add<WheelState>();
            
			//superBonus
            machineState.Add<WheelState>();
             
            machineState.Add<ExtraState11020>();
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            
        }
         
        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelTrans = machineContext.transform.Find("Wheels/" + Constant11020.baseWheelName);

			var wheel = machineContext.view.Add<Wheel>(wheelTrans);
			wheel.BuildWheel<Roll11020, ElementSupplier, WheelSpinningController<WheelAnimationController11020>>(machineContext.state.Get<WheelState>());
			wheel.SetUpWinLineAnimationController<WinLineAnimationController11020>();
            updateWheelMask(wheelTrans);

			wheelTrans = machineContext.transform.Find("Wheels/" + Constant11020.freeWheelName);

            wheel = machineContext.view.Add<Wheel>(wheelTrans);
            wheel.BuildWheel<Roll11020, ElementSupplier, WheelSpinningController<WheelAnimationController11020>>(machineContext.state.Get<WheelState>(1));
            wheel.SetUpWinLineAnimationController<WinLineAnimationController11020>();
            updateWheelMask(wheelTrans);

			wheelTrans = machineContext.transform.Find("Wheels/" + Constant11020.superBonusWheelName);

            wheel = machineContext.view.Add<Wheel>(wheelTrans);
            wheel.BuildWheel<Roll11020, ElementSupplier, WheelSpinningController<WheelAnimationController11020>>(machineContext.state.Get<WheelState>(2));
            wheel.SetUpWinLineAnimationController<WinLineAnimationController11020>();
            updateWheelMask(wheelTrans);
        }

        private void updateWheelMask(Transform transform)
        { 
            var mask = transform.Find("WheelMask").GetComponent<SpriteMask>();
              
            mask.frontSortingOrder = 10000;

            mask.backSortingOrder  = 1200;
        }
        
        public override void BindingExtraView(MachineContext machineContext)
        {
            var transform = machineContext.transform.Find("Wheels/" + Constant11020.baseWheelName + "/SuperBonus");
            machineContext.view.Add<SuperBonusInfoView11020>(transform);

            transform = machineContext.transform.Find("Wheels");
            machineContext.view.Add<LockedFramesView11020>(transform);

            machineContext.view.Add<WheelView11020>(transform);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();

            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11020);

            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11020);

            logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11020);
            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11020);
            logicProxy[LogicStepType.STEP_WHEEL_SPINNING] = typeof(WheelsSpinningProxy11020);

            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11020);

            logicProxy[LogicStepType.STEP_WIN_LINE_BLINK]   = typeof(WinLineBlinkProxy11020);
            logicProxy[LogicStepType.STEP_ROUND_FINISH] = typeof(RoundFinishProxy11020);
			
            return logicProxy;
        }

        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11020();
        }

        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.88f, 0.88f);
        }

        public override void AdaptMachineView(MachineContext machineContext)
        {
            var aspectRatio = ViewResolution.referenceResolutionPortrait.y / ViewResolution.referenceResolutionPortrait.x;
            var designAspectRatio = MachineConstant.aspectRatio16To9;
            var machineScaleFactor = CaculateMachineScaleFactor(designAspectRatio, aspectRatio);
            machineContext.MachineScaleFactor = machineScaleFactor;
            machineContext.transform.localScale = new Vector3(machineScaleFactor, machineScaleFactor, machineScaleFactor);
            var bgWheel = machineContext.transform.Find("Wheels/Transition" + machineContext.assetProvider.MachineId);
            if (bgWheel)
            {
                machineContext.RevertMachineNodeSize(bgWheel);
            }
        }
        
        protected virtual float CaculateMachineScaleFactor(float designAspectRatio, float currentAspectRatio)
        {
            float scale = 1.0f;
            if (currentAspectRatio < designAspectRatio)
            {
                scale = currentAspectRatio / designAspectRatio;
            }
            var extraScale = GetPadMinXScaler().x;
            if (scale < 1.0f && extraScale < 1.0f)
            {
                var scaleDiff = currentAspectRatio - 1024.0f / 768.0f;
                if (scaleDiff < 0)
                {
                    scaleDiff = 0;
                }
                var maxScaleDiff = designAspectRatio - 1024.0f / 768.0f;
                extraScale =  1 - (1 - scaleDiff / maxScaleDiff) * (1 - extraScale);
                return scale * extraScale;
            }
            return scale;
        }

    }
}