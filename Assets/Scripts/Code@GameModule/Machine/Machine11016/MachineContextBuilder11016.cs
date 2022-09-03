//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 11:04
//  Ver : 1.0.0
//  Description : MachineContextBuilder11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineContextBuilder11016:MachineContextBuilder
    {
        public MachineContextBuilder11016(string inMachineId)
            :base(inMachineId)
        {
            machineId = inMachineId;
        }

        public override void SetUpMachineState(MachineState machineState)
        {
            for (int i = 0; i < Constant11016.TotalFreePanels+1; i++)
            {
                machineState.Add<WheelState>();
                machineState.Get<WheelState>(i).IsShowPayLine = false;
            }
            machineState.Add<ExtraState11016>();
            machineState.Add<ReelStopSoundState>();
        }
         
        public override void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState11016>();
        }
        
        protected override Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            var logicProxy = base.SetUpLogicStepProxy();
            logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11016);
            logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11016);
            logicProxy[LogicStepType.STEP_BONUS] = typeof(BonusProxy11016);
            logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11016);
            return logicProxy;
        }
        
        public override IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider11016();
        }
        
        public override void BindingJackpotView(MachineContext machineContext)
        {
            var jackpotTran = machineContext.transform.Find("Wheels/JackpotPanel");
            machineContext.view.Add<JackPotPanel>(jackpotTran);
        }

        public override void BindingExtraView(MachineContext machineContext)
        {
            base.BindingExtraView(machineContext);
            var transSmallSlot = machineContext.transform.Find("Wheels/SmallSlotPaytables");
            machineContext.view.Add<SmallSlotPaytableView11016>(transSmallSlot);
            machineContext.view.Get<SmallSlotPaytableView11016>().Hide();
            
            var transNotice = machineContext.transform.Find("Wheels/WheelFreeGameNotice");
            machineContext.view.Add<FreeTopView11016>(transNotice);
            machineContext.view.Get<FreeTopView11016>().Hide();
            
            var transCollect = machineContext.transform.Find("Wheels/CollectFreePanel");
            machineContext.view.Add<CollectView11016>(transCollect);
            machineContext.view.Get<CollectView11016>().Show();

            // var transExtraFree = machineContext.transform.Find("Wheels/SpinNotice");
            // machineContext.view.Add<FreeExtraView11016>(transExtraFree);
            // machineContext.view.Get<FreeExtraView11016>().Hide();
            
            var transFreeContainer = machineContext.transform.Find("Wheels/FreeContainer");
            machineContext.view.Add<MultiFreePanelView11016>(transFreeContainer);

            var transCut = machineContext.transform.Find("Wheels/Free");
            machineContext.view.Add<FreeCutAnimationView11016>(transCut);
            transCut.gameObject.SetActive(false);
        }

        public override void BindingWheelView(MachineContext machineContext)
        {
            var baseWheelTransform = machineContext.transform.Find("Wheels/WheelBaseGame");
            var baseWheel = machineContext.view.Add<Wheel>(baseWheelTransform);
            baseWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11016>>(machineContext.state.Get<WheelState>(0));
            baseWheel.SetUpWinLineAnimationController<WinLineAnimationController11016>();


            for (int i = 0; i < Constant11016.TotalFreePanels; i++)
            {
                var wheelFreeTransform = machineContext.transform.Find($"Wheels/WheelFreeGame{i}");
                var wheelFree = machineContext.view.Add<FreeWheel11016>(wheelFreeTransform);
                wheelFree.BuildWheel<FreeRoll11016, ElementSupplier,WheelSpinningController<WheelAnimationController11016>>(machineContext.state.Get<WheelState>(i+1));
                wheelFree.SetUpWinLineAnimationController<WinLineAnimationController11016>();
            }
        }
        
        
        public override Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.86f, 0.9f);
        }
       
    }
}