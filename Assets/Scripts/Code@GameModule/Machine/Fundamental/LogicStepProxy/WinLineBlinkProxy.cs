// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyHighlightWinElement.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class WinLineBlinkProxy : LogicStepProxy
    {
        protected WheelsActiveState wheelsRunningStatusState;

        public WinLineBlinkProxy(MachineContext context)
            : base(context)
        {
        }

        public override void SetUp()
        {
            wheelsRunningStatusState = machineContext.state.Get<WheelsActiveState>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var wheels = wheelsRunningStatusState.GetRunningWheel();

            if (wheels != null && wheels.Count > 0)
            {
                for (var i = 0; i < wheels.Count; i++)
                {
                    if (wheels[i].wheelState.HasNormalWinLine())
                        return true;
                }
            }

            return false;
        }

        protected override void HandleCommonLogic()
        {
            var wheels = wheelsRunningStatusState.GetRunningWheel();

            if (wheels != null && wheels.Count > 0)
            {
                machineContext.AddWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);

                var finishCount = 0;
                for (var i = 0; i < wheels.Count; i++)
                {
                    wheels[i].winLineAnimationController.BlinkAllWinLine(() =>
                    {
                        finishCount++;
                        if(finishCount == wheels.Count)
                            machineContext.RemoveWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                    });
                }

                CheckAndShowFiveOfKindAnimation(wheels);
            }
        }

        public void CheckAndShowFiveOfKindAnimation(List<Wheel> runningWheel)
        {
            var haveFiveOfKindWinLine = false;

            for (var i = 0; i < runningWheel.Count; i++)
            {
                if (runningWheel[i].wheelState.HaveFiveKindWinLine())
                {
                    haveFiveOfKindWinLine = true;
                    break;
                }
            }

            if (haveFiveOfKindWinLine)
            {
                var fiveOfKindView = machineContext.view.Get<FiveOfKindView>();
                if (fiveOfKindView != null)
                {
                    fiveOfKindView.ShowFiveOfKind();
                }
            }
        }
    }
}