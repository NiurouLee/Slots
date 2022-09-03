// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/27/13:44
// Ver : 1.0.0
// Description : NextSpinPrepareProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class NextSpinPrepareProxy11026:NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11026(MachineContext machineContext) : base(machineContext)
        {
            
        }
        
        protected override  void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<SuperFreeProgressView11026>().EnableButtonResponse(true);
                // machineContext.view.Get<JackpotPanel11026>().EnableButtonResponse(true);
            }
        }
        
        public override void OnAutoSpinStopClicked()
        {
            base.OnAutoSpinStopClicked();
            
            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<SuperFreeProgressView11026>().EnableButtonResponse(true);
                // machineContext.view.Get<JackpotPanel11026>().EnableButtonResponse(true);
            }
        }
        
        public override void StartNextSpin()
        {
            machineContext.view.Get<SuperFreeProgressView11026>().EnableButtonResponse(false);
            // machineContext.view.Get<JackpotPanel11026>().EnableButtonResponse(false);
            base.StartNextSpin();
        }
        
        protected override void OnBetChange()
        {
            base.OnBetChange();
            var superFreeProgressView11026 = machineContext.view.Get<SuperFreeProgressView11026>();
            superFreeProgressView11026.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
            // machineContext.view.Get<JackpotPanel11026>().UpdateJackpotLockState(true,false);
        }
        
        protected override void OnUnlockBetFeatureConfigChanged()
        {
            var superFreeProgressView11026 = machineContext.view.Get<SuperFreeProgressView11026>();
            superFreeProgressView11026.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0),false);
            // machineContext.view.Get<JackpotPanel11026>().UpdateJackpotLockState(false,false);
        }

    }
}