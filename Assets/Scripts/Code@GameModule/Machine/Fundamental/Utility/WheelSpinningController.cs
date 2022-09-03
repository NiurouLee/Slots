// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-27 10:20 AM
// Ver : 1.0.0
// Description : 控制一个轮盘的转动，负责IRollSpinningUpdater的回调处理
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using DragonU3DSDK;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;


namespace GameModule
{
    public class WheelSpinningController<TWheelAnimationController> : IWheelSpinningController
        where TWheelAnimationController : IWheelAnimationController
    {
        protected Wheel wheelToControl;
        protected WheelState wheelState;

        protected readonly List<IRollSpinningUpdater> spinningUpdaterList;
        protected List<IRollSpinningUpdater> runningUpdater;

        protected Action<Wheel> onWheelSpinningEnd;
        protected Action<Wheel> onWheelAnticipationEnd;
        protected Action onCanEnableQuickStop;

        protected int updaterIndex = 0;
        protected int startUpdaterIndex = 0;
        protected int finishUpdaterIndex = 0;
        protected bool isSpinningFinished;

        protected RollSpinningEventsCallback rollSpinningEventsCallback;
        public IWheelAnimationController animationController;
        
        protected bool anticipationIsPlaying;

        protected bool spinResultReceived = false;

        public WheelSpinningController()
        {
            spinningUpdaterList = new List<IRollSpinningUpdater>();
            runningUpdater = new List<IRollSpinningUpdater>();

            isSpinningFinished = true;

            rollSpinningEventsCallback = new RollSpinningEventsCallback(OnUpdaterLeaveSpeedUpCallback,
                OnRollEnterSlowDownCallback, OnUpdaterStartBounceCallback, OnUpdaterSpinningEndCallback);
        }

        public void BindingWheel(Wheel inWheel)
        {
            if (wheelToControl == inWheel)
                return;

            wheelToControl = inWheel;

            if (animationController == null
                || animationController.GetType() != typeof(TWheelAnimationController))
            {
                animationController = Activator.CreateInstance<TWheelAnimationController>();
            }

            animationController.BindingWheel(inWheel);
            wheelState = wheelToControl.wheelState;
            spinningUpdaterList.Clear();
            isSpinningFinished = false;
        }

        public virtual IRollSpinningUpdater GetUpdater(Type updaterType, Roll reel, int inUpdaterIndex)
        {
            if (spinningUpdaterList.Count > 0)
            {
                for (var i = spinningUpdaterList.Count - 1; i >= 0; i--)
                {
                    if (spinningUpdaterList[i].GetType() == updaterType)
                    {
                        var updater = spinningUpdaterList[i];
                        spinningUpdaterList.RemoveAt(i);
                        updater.UpdateRollToControl(startUpdaterIndex,inUpdaterIndex, inUpdaterIndex, reel);
                        return updater;
                    }
                }
            }

            var newUpdater = (IRollSpinningUpdater) Activator.CreateInstance(updaterType);

            newUpdater.UpdateRollToControl(startUpdaterIndex,inUpdaterIndex, inUpdaterIndex, reel);

            return newUpdater;
        }

        public void RecycleUpdater(IRollSpinningUpdater updater)
        {
            spinningUpdaterList.Add(updater);
        }

        public virtual int StartSpinning(Action<Wheel> inOnWheelSpinningEnd, Action<Wheel> inOnWheelAnticipationEnd, Action inOnCanEnableQuickStop,
            int inUpdaterIndex = 0)
        {
            onWheelSpinningEnd = inOnWheelSpinningEnd;
            onCanEnableQuickStop = inOnCanEnableQuickStop;
            onWheelAnticipationEnd = inOnWheelAnticipationEnd;

            var maxUpdaterCount = wheelToControl.GetMaxSpinningUpdaterCount();

            IRollUpdaterEasingConfig easingConfig = wheelState.GetEasingConfig();

            startUpdaterIndex = updaterIndex = inUpdaterIndex;
            
            finishUpdaterIndex = inUpdaterIndex;
          
            anticipationIsPlaying = false;

            spinResultReceived = false;
            
            wheelToControl.UpdateElementMaskInteraction(false);
           
            animationController.OnWheelStartSpinning();

            for (var rollIndex = 0; rollIndex < maxUpdaterCount; rollIndex++)
            {
                if (!wheelState.IsRollLocked(rollIndex))
                {
                    var spinningUpdater = GetUpdater(easingConfig.GetUpdaterType(),
                        wheelToControl.GetStopRoll(rollIndex),
                        updaterIndex);

                    updaterIndex++;
                     
                    runningUpdater.Add(spinningUpdater);
                    //Debug.LogError($"================= Start===wheel:{wheelState.wheelName}");
                   
                    
                    spinningUpdater.StartSpinning(easingConfig, rollSpinningEventsCallback);
                }
            }

            if (updaterIndex > inUpdaterIndex)
            {
                isSpinningFinished = false;
            }
            else
            {
                onWheelSpinningEnd.Invoke(wheelToControl);
            }

            return updaterIndex;
        }

        public virtual void OnRollStartSpinning(int rollIndex)
        {
            
        }
        
        public virtual void ForceUpdateReels(string reelName)
        {
            if (!spinResultReceived)
            {
                wheelToControl.wheelState.UpdateCurrentActiveSequence(reelName);
                
                for (var i = 0; i < runningUpdater.Count; i++)
                {
                    runningUpdater[i].OnForceUpdateReels();
                }
            }
        }

        public virtual bool OnSpinResultReceived(bool preWheelHasAnticipation)
        {
            spinResultReceived = true;
            
            int drumReelFirstIndex = wheelState.GetAnticipationAnimationStartRollIndex();

            for (var i = 0; i < runningUpdater.Count; i++)
            {
                int rollIndex = runningUpdater[i].RollIndex;
                bool needWaitAnticipation = (startUpdaterIndex > 0 && preWheelHasAnticipation) || rollIndex >= drumReelFirstIndex;
                runningUpdater[i].OnSpinResultReceived(needWaitAnticipation);
            }
            
            if (startUpdaterIndex == 0)
            {
                CheckAndShowAnticipationAnimation();
            }

            return (drumReelFirstIndex < wheelToControl.GetMaxSpinningUpdaterCount());

        }

        public virtual void OnQuickStopped()
        {
            wheelState.playerQuickStopped = true;

            for (var i = 0; i < runningUpdater.Count; i++)
            {
                runningUpdater[i].OnQuickStopped();
            }

            animationController.StopAnticipationAnimation();
        }

        public void OnUpdaterLeaveSpeedUpCallback(IRollSpinningUpdater updater)
        {
            if (updater.UpdaterIndex == 0)
                onCanEnableQuickStop?.Invoke();
        }

        public virtual void OnUpdaterSpinningEndCallback(IRollSpinningUpdater updater)
        {
            XDebug.Log("OnUpdaterSpinningEndCallback:" + updater.RollIndex);

            animationController.OnRollSpinningStopped(updater.RollIndex, () =>
            {
                UpdateAnticipationAnimationAndState(updater);
                OnRollLogicEnd();
            });
        }

        //处理第一列触发Anticipation情况，或者多轮盘的 Anticipation 依赖的情况
        public virtual void CheckAndShowAnticipationAnimation()
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
                    runningUpdater[0].OnAnticipationStopped(startUpdaterIndex - 1, 0, 0);
            }
        }

        protected virtual void UpdateAnticipationAnimationAndState(IRollSpinningUpdater updater)
        {
            if (wheelState.playerQuickStopped)
                return;

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

        public virtual void OnRollLogicEnd()
        {
            finishUpdaterIndex++;

            if (finishUpdaterIndex == updaterIndex)
            {
                OnAllReelSpinningEnd();
            }
        }

        public virtual void OnAllReelSpinningEnd()
        {
            animationController.OnAllRollSpinningStopped(CheckAllReelBlinkEnd);
        }

        /// <summary>
        /// 所有的Blink结束后，才走下一步
        /// </summary>
        private void CheckAllReelBlinkEnd()
        {
            isSpinningFinished = true;

            //Debug.LogError($"++++++++++++ Clear===wheel:{wheelState.wheelName}");
            for (var i = 0; i < runningUpdater.Count; i++)
            {
                RecycleUpdater(runningUpdater[i]);
            }

            runningUpdater.Clear();

            ValidateWheelSpinResult();

            updaterIndex = 0;
            finishUpdaterIndex = 0;
 
            onWheelSpinningEnd?.Invoke(wheelToControl);
        }

        private void ValidateWheelSpinResult()
        {
            for (var i = 0; i < wheelToControl.rollCount; i++)
            {
                var roll = wheelToControl.GetRoll(i);
                
                if (wheelState.IsRollLocked(roll.rollIndex)) continue;
                roll.ValidateSpinResult();   
            }
        }

        public void OnLogicUpdate()
        {
            if (isSpinningFinished)
                return;
 
            for (var i = 0; i < runningUpdater.Count; i++)
            {
                runningUpdater[i].Update();
            }
            
        }

        public virtual void OnRollEnterSlowDownCallback(IRollSpinningUpdater updater)
        {
            animationController.OnRollEnterSlowDown(updater.RollIndex);
        }

        public void OnUpdaterStartBounceCallback(IRollSpinningUpdater updater)
        {
            animationController.OnRollStartBounceBack(updater.RollIndex);
            //Appear动画逻辑
            // reelAnimationLogicProxy?.OnReelStartBounce(panelView, panelView.ReelViews[updater.ReelIndex]);
        }
    }
}