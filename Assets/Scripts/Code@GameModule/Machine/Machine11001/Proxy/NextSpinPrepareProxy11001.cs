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
    public class NextSpinPrepareProxy11001:NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11001(MachineContext machineContext) : base(machineContext)
        {
            
        }
        
        protected override void OnBetChange()
        {
            base.OnBetChange();
            machineContext.view.Get<BingoMapView11001>().RefreshBingoItem();

            var superFreeProgressView11001 = machineContext.view.Get<SuperFreeProgressView11001>();
            superFreeProgressView11001.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
        }
        
        protected override void OnUnlockBetFeatureConfigChanged()
        {
            var superFreeProgressView11001 = machineContext.view.Get<SuperFreeProgressView11001>();
            superFreeProgressView11001.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0),false);
        }

    }
}