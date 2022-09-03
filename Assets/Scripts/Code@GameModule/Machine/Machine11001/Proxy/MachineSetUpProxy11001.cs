// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/16:50
// Ver : 1.0.0
// Description : MachineSetUpProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class MachineSetUpProxy11001 : MachineSetUpProxy
    {
        public MachineSetUpProxy11001(MachineContext context)
            : base(context)
        {
        }
         
        protected override void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            
            UpdateBingoViewUI();
            UpdateSuperBonusProgressView();
            LockWheelMiddleCenterRoll();
            UpdateBackGroupView();
        }

        protected void UpdateBackGroupView()
        {
            machineContext.view.Get<BackGroundView11001>()
                .ShowFreeBackground(machineContext.state.Get<FreeSpinState>().IsInFreeSpin);
        }
  
        protected void UpdateBingoViewUI()
        {
            machineContext.view.Get<BingoMapView11001>().RefreshBingoItem();
        }
        
        protected void UpdateSuperBonusProgressView()
        {
            machineContext.view.Get<SuperFreeProgressView11001>().UpdateProgress();
            var superFreeProgressView11001 = machineContext.view.Get<SuperFreeProgressView11001>();
            superFreeProgressView11001.LockSuperFree(! machineContext.state.Get<BetState>().IsFeatureUnlocked(0),false);
        }

        protected void LockWheelMiddleCenterRoll()
        {
            var wheel = machineContext.view.Get<Wheel>(0);

            wheel.wheelState.SetRollLockState(12, true);
        }
        
    }
}