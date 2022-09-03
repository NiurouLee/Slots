// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/30/19:42
// Ver : 1.0.0
// Description : SubRoundFinishProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class SubRoundFinishProxy11001 : SubRoundFinishProxy
    {
        public SubRoundFinishProxy11001(MachineContext machineContext)
            : base(machineContext)
        {
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_SPECIAL_EFFECT_2);
        }
    }
}