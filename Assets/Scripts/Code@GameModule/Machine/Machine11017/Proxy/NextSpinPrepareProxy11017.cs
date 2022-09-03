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
    public class NextSpinPrepareProxy11017:NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11017(MachineContext machineContext) : base(machineContext)
        {
            
        }
        
        protected override  void HandleCommonLogic()
        {
            base.HandleCommonLogic();
            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                 machineContext.view.Get<FreeGameMark11017>().NoticeTipBright();
            }
        }

        public override void StartNextSpin()
        {
            machineContext.view.Get<FreeGameMark11017>().NoticeTipDark();
            base.StartNextSpin();
        }
        
        protected override void OnBetChange()
        {
            base.OnBetChange();
            machineContext.view.Get<FeatureGame11017>().SetFeature();
            var SuperFreeGameLock11017 = machineContext.view.Get<SuperFreeGameLock11017>();
            SuperFreeGameLock11017.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
        }
        
        protected override void OnUnlockBetFeatureConfigChanged()
        {
            var SuperFreeGameLock11017 = machineContext.view.Get<SuperFreeGameLock11017>();
            SuperFreeGameLock11017.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0),false);
        }
    }
}