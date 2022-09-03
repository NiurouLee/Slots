namespace GameModule
{
    public class
        WheelSpinningController11027<TWheelAnimationController> : WheelSpinningController<TWheelAnimationController>
        where TWheelAnimationController : IWheelAnimationController
    {
        //处理第一列触发Anticipation情况，或者多轮盘的 Anticipation 依赖的情况
        public override void CheckAndShowAnticipationAnimation()
        {
            if (wheelState.playerQuickStopped)
                return;

            if (runningUpdater.Count <= 0)
                return;
            
            if (wheelState.HasAnticipationAnimationInRollIndex(runningUpdater[0].RollIndex))
            {
                runningUpdater[0].EnterAnticipation();
            }
            else
            {
                if (runningUpdater[0].IsWaitAnticipation())
                    runningUpdater[0].OnAnticipationStopped(startUpdaterIndex - 1, 0, 0);
            }
        }
        
        protected override void UpdateAnticipationAnimationAndState(IRollSpinningUpdater updater)
        {
            if (wheelState.playerQuickStopped)
            {
                AudioUtil.Instance.StopAudioFx("AnticipationSound");
                return;
            }
                

            if (updater.UpdaterIndex - startUpdaterIndex < runningUpdater.Count)
            {
                //如果不是最后一个
                var nextUpdaterIndex = updater.UpdaterIndex + 1 - startUpdaterIndex;
                if (nextUpdaterIndex < runningUpdater.Count)
                {
                    int nextRollIndex = runningUpdater[nextUpdaterIndex].RollIndex;
                    if (wheelState.HasAnticipationAnimationInRollIndex(nextRollIndex))
                    {
                        //    XDebug.Log("ShowNextDrum:" + nextRollIndex);
                        //有锁列的情况，所以不能直接用nextRollIndex
                        for (int i = 0; i < runningUpdater.Count; i++)
                        {
                            if (runningUpdater[i].RollIndex == nextRollIndex)
                            {
                                runningUpdater[i].EnterAnticipation();
                                AudioUtil.Instance.PlayAudioFx("AnticipationSound");
                                animationController.ShowAnticipationAnimation(nextRollIndex);
                                anticipationIsPlaying = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!anticipationIsPlaying)
                            return;
                        //下一列没有DrumAnimation
                        animationController.StopAnticipationAnimation(true);
                        anticipationIsPlaying = false;

                        //更新Wait Drum，下一列到下一个有Drum动画的列 之间的列都不要等Drum动画了

                        bool allAnticipationFinished = true;
                        for (var i = nextUpdaterIndex; i < runningUpdater.Count; i++)
                        {
                            if (wheelState.HasAnticipationAnimationInRollIndex(runningUpdater[i].RollIndex))
                            {
                                allAnticipationFinished = false;
                                break;
                            }

                            if(runningUpdater[i].IsActive())
                                runningUpdater[i].OnAnticipationStopped(updater.UpdaterIndex,updater.RollIndex,updater.UpdaterStopIndex);
                        }
                        
                        // 如果所有列的Anticipation都停止了，调用回调，通知后续轮盘不用再等了
                        if (allAnticipationFinished)
                        {
                            onWheelAnticipationEnd?.Invoke(wheelToControl);
                            onWheelAnticipationEnd = null;
                        }
                    }
                }
                else
                {
                    //最后一列停下的时候隐藏 DrumAnimation
                    if (onWheelAnticipationEnd != null)
                    {
                        anticipationIsPlaying = false;
                        animationController.StopAnticipationAnimation();
                        onWheelAnticipationEnd?.Invoke(wheelToControl);
                        onWheelAnticipationEnd = null;
                    }
                }
            }
        }
    }
}