// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-27 10:20 AM
// Ver : 1.0.0
// Description : IndependentSpinningController 控制IndependentWheel轮盘的转动，主要重写Anticipation动画播放的逻辑
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class IndependentSpinningController11026<TWheelAnimationController> : WheelSpinningController<TWheelAnimationController>
        where TWheelAnimationController : IWheelAnimationController
    {
        private int _nextCheckAnticipationColumnIndex; 
        //处理第一列触发Anticipation情况，或者多轮盘的 Anticipation 依赖的情况
        // public override void CheckAndShowAnticipationAnimation()
        // {
        //     if (wheelState.playerQuickStopped)
        //         return;
        //     if (runningUpdater.Count <= 0)
        //         return;
        //     if (wheelState.HasAnticipationAnimationInRollIndex(runningUpdater[0].RollIndex))
        //     {
        //         runningUpdater[0].EnterAnticipation();
        //         animationController.ShowAnticipationAnimation(runningUpdater[0].RollIndex);
        //         anticipationIsPlaying = true;
        //     }
        //     else
        //     {
        //         if (runningUpdater[0].IsWaitAnticipation())
        //             runningUpdater[0].OnAnticipationStopped(startUpdaterIndex - 1,0);
        //     }
        // }
    }
}