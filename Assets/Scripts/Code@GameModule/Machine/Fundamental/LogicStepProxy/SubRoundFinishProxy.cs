// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyRoundFinish.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class SubRoundFinishProxy : LogicStepProxy
    {
        public SubRoundFinishProxy(MachineContext context)
            : base(context)
        {
        }

        protected override void HandleCommonLogic()
        {
            machineContext.state.UpdateStateOnSubRoundFinish();
            
            EventBus.Dispatch(new EventSubRoundEnd());
        }


        public override bool IsMatchCondition()
        {
            return !machineContext.state.Get<ReSpinState>().NextIsReSpin;
        }

        public override bool IsConditionStep()
        {
            return true;
        }
    }
}