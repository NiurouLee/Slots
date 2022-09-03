// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:59 AM
// Ver : 1.0.0
// Description : SubState.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class SubState
    {
        protected MachineState machineState;

        public virtual bool MatchFilter(string filter)
        {
            return false;
        }
        
        public SubState(MachineState state)
        {
            machineState = state;
        }

        public virtual void UpdateStateOnRoundStart()
        {
        }

        public virtual void UpdateStateOnSubRoundStart()
        {
        }
        
        public virtual void UpdateStateOnSubRoundFinish()
        {
        }

        public virtual void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            
        }
        
        public virtual void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
        }

        public virtual void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
        }
 
        public virtual void UpdateStateBeforeCallRoundFinish()
        {
        }

        public virtual void UpdateStateOnRoundFinish()
        {
        }

        public virtual void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            
        }

        public virtual void UpdateStateOnSpecialProcess(SSpecialProcess sSpecialProcess)
        {
            
        }
        public virtual void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            
        }
    }
}