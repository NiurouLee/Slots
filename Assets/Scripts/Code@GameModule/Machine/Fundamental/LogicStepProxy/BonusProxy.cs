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


using System.Threading.Tasks;

namespace GameModule
{
    public class BonusProxy : LogicStepProxy
    {
        public BonusProxy(MachineContext context)
            : base(context)
        {
            
        }

        protected override void HandleCommonLogic()
        {
            StopWinCycle();
            base.HandleCommonLogic();
          
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        protected async Task BlinkBonusLine()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if (wheels.Count > 0)
            {
                await wheels[0].winLineAnimationController.BlinkBonusLine();
            }
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.HasBonusGame())
            {
                return true;
            }
            return false;
        }
    }
}