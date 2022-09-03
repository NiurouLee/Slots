//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-18 14:39
//  Ver : 1.0.0
//  Description : SubRoundFinishProxy11007.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class SubRoundFinishProxy11007: SubRoundFinishProxy
    {
        public SubRoundFinishProxy11007(MachineContext context)
            : base(context)
        {
        }
        
        public override bool IsMatchCondition()
        {
            var wheelActive = machineContext.state.Get<WheelsActiveState11007>();
            return base.IsMatchCondition() && !wheelActive.IsBonusWheel;
        }
    }
}