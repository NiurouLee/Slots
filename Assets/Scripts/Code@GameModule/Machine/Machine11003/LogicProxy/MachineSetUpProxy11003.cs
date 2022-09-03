// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/27/18:10
// Ver : 1.0.0
// Description : MachineSetUpProxy11003.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class MachineSetUpProxy11003 : MachineSetUpProxy
    {
        public MachineSetUpProxy11003(MachineContext context)
            : base(context)
        {
        }

        
        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            
            var extraState11003 = machineContext.state.Get<ExtraState11003>();
            var freeSpinState = machineContext.state.Get<FreeSpinState>();

            if (extraState11003.IsSuperFreeGame())
            {
                machineContext.view.Get<BackgroundView11003>().OpenSuperFree();
            } else if (freeSpinState.IsInFreeSpin)
            {
                machineContext.view.Get<BackgroundView11003>().OpenFree();
            }
            else
            {
                machineContext.view.Get<BackgroundView11003>().OpenBase();
            }
            
            UpdateUIViewLockFeatureStatus();

            ExtendWheelMask();
        }

        protected  void ExtendWheelMask()
        {
            var listWheelNames = new List<string>
            {
                "WheelFreeGame", "WheelFreeGameSuper4x5", "WheelFreeGameSuper3x5", "WheelFreeGameSuper4x6",
                "WheelFreeGameSuper3x6"
            };

            for (var i = 0; i < listWheelNames.Count; i++)
            {
                var wheel = machineContext.view.Get<Wheel>(listWheelNames[i]);
                wheel.ExtendWheelMaskSize(40,0);
            }
        }
        
        protected void UpdateUIViewLockFeatureStatus()
        {
            machineContext.view.Get<BaseSpinMapTitleView>().LockFeatureUI(!machineContext.state.Get<BetState>().IsFeatureUnlocked(2),false,true);
            machineContext.view.Get<JackpotPanel11003>().UpdateJackpotLockState(false,true);
        }

        protected override void SeekLogicTypeToJump()
        {
            base.SeekLogicTypeToJump();

            var extraState11003 = machineContext.state.Get<ExtraState11003>();
            if (logicStepToJump == LogicStepType.STEP_MACHINE_SETUP)
            {
                if (extraState11003.NextIsSuperFreeGame() || extraState11003.IsSuperFreeGameNeedSettle())
                {
                    logicStepToJump = LogicStepType.STEP_FREE_GAME;
                }
            }
        }

        protected override void HandleCustomLogic()
        {
            machineContext.view.Get<BaseSpinMapTitleView>().UpdateCollectionProgress(false);

            if (logicStepToJump == LogicStepType.STEP_MACHINE_SETUP)
            {
                if (machineContext.state.Get<BetState>().IsFeatureUnlocked(2))
                {
                    machineContext.view.Get<BaseSpinMapTitleView>().ShowCollectTip();
                }
            }
            
            base.HandleCustomLogic();
        }
    }
}