using System;
using System.Collections.Generic;
namespace GameModule
{
    public class WheelSpinningController11029 <TWheelAnimationController> : WheelSpinningController<TWheelAnimationController>
        where TWheelAnimationController : IWheelAnimationController
    {
        public override bool OnSpinResultReceived(bool preWheelHasAnticipation)
        {
            var extraState = wheelToControl.GetContext().state.Get<ExtraState11029>();
            var freeSpinState = wheelToControl.GetContext().state.Get<FreeSpinState>();
            int drumReelFirstIndex = wheelState.GetAnticipationAnimationStartRollIndex();

            for (var i = 0; i < runningUpdater.Count; i++)
            {
                int rollIndex = runningUpdater[i].RollIndex;
                bool needWaitAnticipation = (startUpdaterIndex > 0 && preWheelHasAnticipation) || rollIndex >= drumReelFirstIndex;
                if (startUpdaterIndex == 0)
                {
                    CheckAndShowAnticipationAnimation();
                }
                if (extraState.GetIsDrag())
                {
                    if (i == 0)
                    {
                        runningUpdater[i].OnSpinResultReceived(needWaitAnticipation);
                        break;
                    }
                }
                else
                {
                     runningUpdater[i].OnSpinResultReceived(needWaitAnticipation);
                }
            }
            if (startUpdaterIndex == 0)
            {
                CheckAndShowAnticipationAnimation();
            }
            return (drumReelFirstIndex < wheelToControl.GetMaxSpinningUpdaterCount());
        }
        
        public async void OnSpinResultReceivedNew()
        {
            if (wheelToControl.GetContext().state.Get<FreeSpinState>().IsInFreeSpin
                || wheelToControl.GetContext().state.Get<ReSpinState>().IsInRespin
                || !wheelToControl.GetContext().state.Get<AutoSpinState>().IsAutoSpin)
            {
                wheelToControl.GetContext().view.Get<ControlPanel>().ShowStopButton(true);
            }
            
            for (var i = 1; i < runningUpdater.Count; i++)
            {
                await wheelToControl.GetContext().WaitSeconds(1.0f);
                
                if(runningUpdater.Count > i)
                    runningUpdater[i].OnSpinResultReceived(false);
            }
        }

        public override void OnQuickStopped()
        {
            for (var i = finishUpdaterIndex; i < runningUpdater.Count; i++)
            {
                runningUpdater[i].OnSpinResultReceived(false);
            }
            base.OnQuickStopped();
        }
    }
}