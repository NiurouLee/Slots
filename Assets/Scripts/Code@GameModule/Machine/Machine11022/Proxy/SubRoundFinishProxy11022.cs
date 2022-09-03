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
    public class SubRoundFinishProxy11022 : SubRoundFinishProxy
    {
        private ExtraState11022 extraState;
        public SubRoundFinishProxy11022(MachineContext context)
            : base(context)
        {
            extraState = context.state.Get<ExtraState11022>();
        }

        public override bool IsMatchCondition()
        {
            return !machineContext.state.Get<ReSpinState>().NextIsReSpin && extraState.GetNextCollectShapeIndex() == -1;
        }
        
        protected override void HandleCustomLogic()
        {
            var runningWheel = machineContext.state.Get<WheelsActiveState11022>().GetRunningWheel()[0];
            if (runningWheel.wheelName == "WheelFreeGame" && !runningWheel.wheelState.HasNormalWinLine())
            {
                machineContext.WaitSeconds(1f, Proceed);
            }
            else
            {
                Proceed();
            }
        }
    }
}