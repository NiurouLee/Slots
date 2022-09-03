// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyRoundStart.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class RoundStartProxy : LogicStepProxy
    {
        public RoundStartProxy(MachineContext context)
            : base(context)
        {
        }

        protected override void HandleCommonLogic()
        {
            machineContext.state.UpdateStateOnRoundStart();

            machineContext.view.Get<ControlPanel>().StopWinAnimation(false,0);

            CostSpinBet();
            
            EventBus.Dispatch(new EventSpinRoundStart());
        }

        protected void CostSpinBet()
        {
            var bet = machineContext.state.Get<BetState>().totalBet;
            EventBus.Dispatch(new EventBalanceUpdate(-(long) bet, "Spin_Cost", false, true));
        }
    }
}