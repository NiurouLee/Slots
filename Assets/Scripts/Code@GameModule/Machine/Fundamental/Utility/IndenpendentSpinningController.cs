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

using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModule
{
    public class IndependentSpinningController<TWheelAnimationController> : WheelSpinningController<TWheelAnimationController>
        where TWheelAnimationController : IWheelAnimationController
    {
        private int _nextCheckAnticipationColumnIndex;
        
        public override bool OnSpinResultReceived(bool preWheelHasAnticipation)
        {
            int drumReelFirstIndex = wheelState.GetAnticipationAnimationStartRollIndex();
          
            _nextCheckAnticipationColumnIndex = 0;
            
            var wheelConfig = wheelState.GetWheelConfig();
            var startRoll = (drumReelFirstIndex) / wheelConfig.rollRowCount;

            if (drumReelFirstIndex % wheelConfig.rollRowCount != 0)
            {
                startRoll = startRoll + 1;
            }
           
            for (var i = 0; i < runningUpdater.Count; i++)
            {
                int rollIndex = runningUpdater[i].RollIndex;
                
                var visualRollIndex = rollIndex / wheelConfig.rollRowCount;
                bool needWaitAnticipation = (startUpdaterIndex > 0 && preWheelHasAnticipation) || visualRollIndex >= startRoll;
              
                runningUpdater[i].OnSpinResultReceived(needWaitAnticipation);
            }
            
            if (startUpdaterIndex == 0)
            {
                CheckAndShowAnticipationAnimation();
            }

            return (drumReelFirstIndex < wheelToControl.GetMaxSpinningUpdaterCount());
        }

        public override IRollSpinningUpdater GetUpdater(Type updaterType, Roll roll, int inUpdaterIndex)
        {
            OnGetRollUpdaterForRoll(roll);

            var stopColumnIndex = activeColumn == null ? 0 : activeColumn.Count - 1;

            if (spinningUpdaterList.Count > 0)
            {
                for (var i = spinningUpdaterList.Count - 1; i >= 0; i--)
                {
                    if (spinningUpdaterList[i].GetType() == updaterType)
                    {
                        var updater = spinningUpdaterList[i];
                        spinningUpdaterList.RemoveAt(i);
                        updater.UpdateRollToControl(startUpdaterIndex, inUpdaterIndex,
                            startUpdaterIndex + stopColumnIndex, roll);
                    }
                }
            }

            var newUpdater = (IRollSpinningUpdater) Activator.CreateInstance(updaterType);

            newUpdater.UpdateRollToControl(startUpdaterIndex, inUpdaterIndex, startUpdaterIndex + stopColumnIndex,
                roll);

            return newUpdater;
        }

        protected List<int> activeColumn = null;
        public void OnGetRollUpdaterForRoll(Roll roll)
        {
            var wheelConfig = wheelToControl.wheelState.GetWheelConfig();
            var columnIndex = roll.rollIndex / wheelConfig.rollRowCount;

            if (activeColumn == null)
            {
                activeColumn = new List<int>();
            }

            if (!activeColumn.Contains(columnIndex))
            {
                activeColumn.Add(columnIndex);
            }
        }
        
        public override int StartSpinning(Action<Wheel> inOnWheelSpinningEnd, Action<Wheel> inOnWheelAnticipationEnd,
            Action inOnCanEnableQuickStop,
            int inUpdaterIndex = 0)
        {
            if (activeColumn != null)
                activeColumn.Clear();
        
            base.StartSpinning(inOnWheelSpinningEnd, inOnWheelAnticipationEnd, inOnCanEnableQuickStop, inUpdaterIndex);
        
            return activeColumn != null ? inUpdaterIndex + activeColumn.Count : inUpdaterIndex;
        }
        
        
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
                animationController.ShowAnticipationAnimation(runningUpdater[0].RollIndex);
                anticipationIsPlaying = true;
            }
            else
            {
                if (runningUpdater[0].IsWaitAnticipation())
                    runningUpdater[0].OnAnticipationStopped(startUpdaterIndex - 1,0,0);
            }
        }


        protected virtual bool IsColumnHasAnticipation(int columnIndex)
        {
            var wheelConfig = wheelState.GetWheelConfig();

            if (columnIndex >= wheelConfig.rollCount)
            {
                return false;
            }

            var startRollIndex = columnIndex * wheelConfig.rollRowCount;
            var endRollIndex = startRollIndex + wheelConfig.rollRowCount;

            for (int i = startRollIndex; i < endRollIndex; i++)
            {
                if (!wheelState.HasAnticipationAnimationInRollIndex(i))
                {
                    return false;
                }
            }

            return true;
        }

        protected override void UpdateAnticipationAnimationAndState(IRollSpinningUpdater updater)
        {
            if (wheelState.playerQuickStopped)
                return;
            var wheelConfig = wheelState.GetWheelConfig();

            if (updater.UpdaterIndex - startUpdaterIndex < runningUpdater.Count)
            {
              
                
                var nextColumnIndex = (updater.RollIndex) / wheelConfig.rollRowCount + 1;

                if (nextColumnIndex == _nextCheckAnticipationColumnIndex)
                    return;
                
                _nextCheckAnticipationColumnIndex = nextColumnIndex;

                if (nextColumnIndex >= wheelConfig.rollCount)
                {
                    if (anticipationIsPlaying)
                        {
                            animationController.StopAnticipationAnimation();
                            onWheelAnticipationEnd?.Invoke(wheelToControl);
                            return;
                        }
                }

                if (IsColumnHasAnticipation(nextColumnIndex))
                {
                    var nextRollIndex = nextColumnIndex * wheelConfig.rollRowCount;

                    bool anticipationUpdated = false;
                    
                    for (int i = 0; i < runningUpdater.Count; i++)
                    {
                        if (runningUpdater[i].RollIndex >= nextRollIndex
                            && runningUpdater[i].RollIndex < nextRollIndex + wheelConfig.rollRowCount)
                        {
                            runningUpdater[i].EnterAnticipation();

                            if (!anticipationUpdated)
                            {
                                anticipationUpdated = true;
                                
                                animationController.ShowAnticipationAnimation(nextRollIndex);
                                anticipationIsPlaying = true;
                                
                            }
                        }
                    }
                }
                else
                {
                    if (anticipationIsPlaying)
                    {
                        animationController.StopAnticipationAnimation(true);
                        anticipationIsPlaying = false;
                    }

                    bool allAnticipationFinished = true;

                    for (var i = nextColumnIndex; i < wheelConfig.rollCount; i++)
                    {
                        if (IsColumnHasAnticipation(i))
                        {
                            allAnticipationFinished = false;
                            break;
                        }

                        var startIndex = i * wheelConfig.rollRowCount;

                        for (var index = updater.UpdaterIndex + 1 - startUpdaterIndex;
                            index < runningUpdater.Count;
                            index++)
                        {
                            if (runningUpdater[index].IsActive()
                                && runningUpdater[index].RollIndex >= startIndex
                                && runningUpdater[index].RollIndex < startIndex + wheelConfig.rollRowCount)
                            {
                                runningUpdater[index].OnAnticipationStopped(updater.UpdaterIndex, updater.RollIndex, updater.UpdaterStopIndex);
                            }
                        }
                    }

                    // 如果所有列的Anticipation都停止了，调用回调，通知后续轮盘不用再等了
                    if (allAnticipationFinished)
                        onWheelAnticipationEnd?.Invoke(wheelToControl);
                }
            }
            else
            {
                animationController.StopAnticipationAnimation(true);
                anticipationIsPlaying = false;
            }
        }
    }
}