// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyFreeGame.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class SpecialBonusProxy : LogicStepProxy
    {
        public SpecialBonusProxy(MachineContext context)
            : base(context)
        {
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.HasSpecialBonus())
            {
                return true;
            }

            return false;
        }

        protected override void HandleCommonLogic()
        {
            StopWinCycle();
            base.HandleCommonLogic();
        }
    }
}