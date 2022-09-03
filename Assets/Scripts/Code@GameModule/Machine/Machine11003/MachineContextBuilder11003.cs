using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11003 : MachineContextBuilder
    {
        public MachineContextBuilder11003(string inMachineId) : base(inMachineId)
        {
            
        }

        
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11003();
        }
        
        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11003>();
        }
        
        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.85f, 0.85f);
        }
        
        public override void SetUpMachineState(MachineState machineState)
        {
            //Base
            machineState.Add<WheelState>();
           
            //Base Jackpot
            machineState.Add<WheelState>().SetResultIndex(1);
            
            //Free
            machineState.Add<WheelState>();
            
            //FreeJackpot
            machineState.Add<WheelState>().SetResultIndex(1);
            
            //SuperFree 5x3
            machineState.Add<WheelState>();
            
            //SuperFree 5x4
            machineState.Add<WheelState>();
            
            //SuperFree 6x3
            machineState.Add<WheelState>();
          
            //SuperFree 4x6
            machineState.Add<WheelState>();
            
             
            machineState.Add<ExtraState11003>();

        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("JackpotPanel");
            machineContext.view.Add<JackpotPanel11003>(jackpotTran);
        }
         
        public override void BindingBackgroundView(MachineContext machineContext)
        {
            var background = machineContext.transform.Find("Background");
            machineContext.view.Add<BackgroundView11003>(background);
        }

        
        public override void BindingWheelView(MachineContext machineContext)
        {
            var wheelsTransform = machineContext.transform.Find("Wheels");
            var wheelCount = wheelsTransform.childCount;

            for (var i = 0; i < wheelCount; i++)
            {
                if (i == 0)
                {
                    var wheel = machineContext.view.Add<BaseWheel11003>(wheelsTransform.GetChild(i));
                    wheel.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(i));

                    wheel.SetUpWinLineAnimationController<WinLineAnimationController11003>();
                }
                else
                {
                    var wheel = machineContext.view.Add<Wheel>(wheelsTransform.GetChild(i));
                    wheel.BuildWheel<Roll, ElementSupplier,WheelSpinningController<WheelAnimationController>>(machineContext.state.Get<WheelState>(i));

                    wheel.SetUpWinLineAnimationController<WinLineAnimationController>();
                }
            }
        }
        
        public override void BindingExtraView(MachineContext machineContext)
        {
            var baseTitleMap = machineContext.transform.Find("Wheels/WheelBaseGame/ProgressBarButton");
            machineContext.view.Add<BaseSpinMapTitleView>(baseTitleMap);
            
            var freeTitleView = machineContext.transform.Find("Wheels/WheelFreeGame/TipsGroup");
            machineContext.view.Add<FreeSpinTitleView>(freeTitleView);
            
            var mapViewTransform = machineContext.transform.Find("Wheels/WheelBaseGame/ProgressPreview");
            
            machineContext.view.Add<MapView11003>(mapViewTransform);
        }

        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();

            logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11003);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11003);
            logicProxy[LogicStepType.STEP_SPECIAL_BONUS] = typeof(SpecialBonusProxy11003);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11003);
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11003);
          
            return logicProxy;
        }
    }
}