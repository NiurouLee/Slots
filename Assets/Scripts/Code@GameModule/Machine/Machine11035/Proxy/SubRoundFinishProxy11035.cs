// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/19/18:14
// Ver : 1.0.0
// Description : SubRoundFinishProxy.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class SubRoundFinishProxy11035: SubRoundFinishProxy
    {
        public SubRoundFinishProxy11035(MachineContext context)
            : base(context)
        {
            
        }

        private ExtraState11035 _extraState11035;
      
        public override void SetUp()
        {
            _extraState11035 = machineContext.state.Get<ExtraState11035>();
        }
    
        public override bool IsMatchCondition()
        {
            return !machineContext.state.Get<ReSpinState>().NextIsReSpin && !_extraState11035.HasJackpotWheel();
        }
    }
}