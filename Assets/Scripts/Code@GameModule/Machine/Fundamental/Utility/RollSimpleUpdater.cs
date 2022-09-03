// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:28 AM
// Ver : 1.0.0
// Description : RollSimpleUpdater.cs
// ChangeLog :
// **********************************************


using System;
using UnityEngine;

namespace GameModule
{
    public class RollSimpleUpdater: IRollSpinningUpdater
    {
        public RollerType RollerType => RollerType.TYPE_SIMPLE;
        public int RollIndex => roll.rollIndex;
        
        public int UpdaterStopIndex =>updaterStopIndex;
        public int UpdaterIndex => updaterIndex;

        protected int wheelUpdaterStartIndex;
        
        /// <summary>
        /// 当前Updater控制的滚轴
        /// </summary>
        protected Roll roll;

        /// <summary>
        /// 当前updater的Index
        /// </summary>
        protected int updaterIndex;

        /// <summary>
        /// 加速时长
        /// </summary>
        protected float speedUpDuration;
        
        /// <summary>
        /// 卷轴停下的位置
        /// </summary>
        protected int reelStopIndex;
        /// <summary>
        /// 在卷轴上的当前位置
        /// </summary>
        protected int currentIndex;
        /// <summary>
        /// 卷轴上的图标个数
        /// </summary>
        protected int maxIndex;
        /// <summary>
        /// 减速阶段需要走的步数
        /// </summary>
        protected int slowDownStepCount;
 
        /// <summary>
        /// 减速阶段需要的的时间
        /// </summary>
        protected float slowDownDuration;
        /// <summary>
        /// 减速阶段的需要移动的距离
        /// </summary>
        protected float slowDownDistance;
        
        //减速阶段，相当于你卷轴的开始位置
        protected float slowDownStartPos;

        /// <summary>
        /// 滚轮停下的时候回弹的力度参数
        /// </summary>
        protected float overShoot; 
            
        /// <summary>
        ///启动时候回弹的力度参数
        /// </summary>
        protected float startOverShoot;

        /// <summary>
        /// 图标单元格的高度
        /// </summary>
        protected float stepSize;
         
        /// <summary>
        /// 当前相对于卷轴的偏移量
        /// </summary>
        protected float shiftAmount;

        protected float currentSpinDuration;
        /// <summary>
        /// 滚动的匀速阶段的速度
        /// </summary>
        protected float spinSpeed;
        /// <summary>
        /// Anticipation开始的初始速度
        /// </summary>
        protected float startSpeed;
        
        /// <summary>
        /// 匀速模式和Anticipation模式的速度差
        /// </summary>
        protected float deltaSpeed;

        /// <summary>
        /// 最少需要滚动的时长
        /// </summary>
        protected float leastSpinTime;

        /// <summary>
        /// SPIN结果是否收到了
        /// </summary>
        protected bool spinResultReceived;
        
        /// <summary>
        /// 是否在等待Anticipation动画
        /// </summary>
        protected bool waitAnticipation;
        
        /// <summary>
        /// 是否进入快停模式
        /// </summary>
        protected bool inQuickStopMode;

        public int preStoppedUpdaterIndex = -1;
        public int preStoppedRollIndex = -1;
        public int preStoppedUpdaterStopIndex = -1;

        public enum State
        {
            /// <summary>
            /// 停止状态
            /// </summary>
            STATE_IDLE,
            /// <summary>
            /// 加速度状态
            /// </summary>
            STATE_SPEED_UP,
            /// <summary>
            /// 匀速循环状态
            /// </summary>
            STATE_SPIN_LOOP,
            /// <summary>
            /// 接收到服务器结果，并且不在等待别的列的Anticipation动画，检查是否需要减速状态
            /// </summary>
            STATE_SPIN_CHECK_SLOW_DOWN,
            /// <summary>
            /// 减速状态
            /// </summary>
            STATE_SLOWDOWN,
        }

        /// <summary>
        /// RollerUpdater的当前状态
        /// </summary>
        protected State spinState;
        /// <summary>
        /// 当前的滚动速度
        /// </summary>
        protected float currentVelocity;
        
        /// <summary>
        /// 缓动进度
        /// </summary>
        protected float easingProgress;

        protected int updaterStopIndex;
        
        /// <summary>
        /// 为了计算回弹时机，记录减速阶段的进度T的上次值
        /// </summary>
        protected float lastT;
        protected bool bounceEventFired;

        protected int waitElementCountBeforeUpdateReels = 0;
        
        protected SimpleRollUpdaterEasingConfig easingConfig;

        protected RollSpinningEventsCallback rollSpinningEventsCallback;

        public RollSimpleUpdater()
        {
            currentSpinDuration = 0;
            easingProgress = 0;
            spinState = State.STATE_IDLE;
            spinSpeed = 10;
            currentIndex = 0;
            maxIndex = 50;
            reelStopIndex = -1;
            slowDownStepCount = 5;
            overShoot = 0.2f;
            shiftAmount = 0;
        }
        
        public void UpdateRollToControl(int inWheelUpdaterStartIndex, int inUpdaterIndex, int inUpdateStopIndex, Roll inRoll)
        {
            roll = inRoll;
            wheelUpdaterStartIndex = inWheelUpdaterStartIndex;
            updaterIndex = inUpdaterIndex;
            stepSize = roll.stepSize;
            updaterStopIndex = inUpdateStopIndex;
            spinState = State.STATE_IDLE;
        }

        public virtual void StartSpinning(IRollUpdaterEasingConfig inEasingConfig, RollSpinningEventsCallback eventCallback)
        {
            easingConfig = (SimpleRollUpdaterEasingConfig) inEasingConfig;
            rollSpinningEventsCallback = eventCallback;
            
            currentSpinDuration = 0;
            easingProgress = 0;
            reelStopIndex = -1;
            spinState = State.STATE_SPEED_UP;
            spinSpeed = easingConfig.spinSpeed;
            bounceEventFired = false;
           
            lastT = 0;
            
            currentVelocity = 0;
            
            spinResultReceived = false;

            preStoppedUpdaterIndex = -1;
            preStoppedRollIndex = -1;
            preStoppedUpdaterStopIndex = -1;

            speedUpDuration = easingConfig.speedUpDuration;
            slowDownStepCount = easingConfig.slowDownStepCount;
            overShoot = easingConfig.overShootAmount;
            startOverShoot = easingConfig.startOverShootAmount;
            slowDownDuration = easingConfig.slowDownDuration;
           
            slowDownDistance = 0.0f;
            slowDownStartPos = 0.0f;
            
            waitElementCountBeforeUpdateReels = 0;
         
            spinResultReceived = false;
            waitAnticipation = false;
            inQuickStopMode = false;
          
            leastSpinTime = easingConfig.leastSpinDuration + roll.GetSpinningDurationMultiplier(wheelUpdaterStartIndex, updaterIndex, updaterStopIndex) * easingConfig.stopIntervalTime;
            
            currentIndex = roll.GetReelStartIndex();
            maxIndex = roll.elementSupplier.GetElementSequenceLength();
            currentIndex = Math.Min(maxIndex - 1, currentIndex);
            
            shiftAmount = maxIndex - currentIndex - 1;
        }

        #region RollerUpdate

        public float GetSpeedUpVelocity(float accEasingProgress) 
        { 
            return -accEasingProgress * accEasingProgress * ((startOverShoot + 1) * (-accEasingProgress) + startOverShoot);
        }

        public void Update()
        {
            if (spinState == State.STATE_IDLE)
                return;

            var deltaTime = Time.smoothDeltaTime;
            currentSpinDuration += deltaTime;

            switch (spinState)
            {
                case State.STATE_SPEED_UP:
                    easingProgress += deltaTime /speedUpDuration;
                    
                    if (easingProgress > 1)
                    {
                        easingProgress = 1;
                        spinState = State.STATE_SPIN_LOOP;
                        currentVelocity = spinSpeed;
                        rollSpinningEventsCallback.leaveSpeedUp?.Invoke(this);
                        return;
                    } 
                    
                    currentVelocity = GetSpeedUpVelocity(easingProgress) * spinSpeed;
                    
                    UpdateShiftWithCurrentSpeed(deltaTime);
                    break;
 
                case State.STATE_SPIN_LOOP:
              
                    UpdateShiftWithCurrentSpeed(deltaTime);
                 
                    if (spinResultReceived && !waitAnticipation && waitElementCountBeforeUpdateReels == 0)
                    {
                        spinState = State.STATE_SPIN_CHECK_SLOW_DOWN;
 
                        if (currentSpinDuration > easingConfig.leastSpinDuration)
                        {
                            leastSpinTime =
                                currentSpinDuration + roll.GetSpinningDurationMultiplier(wheelUpdaterStartIndex, preStoppedUpdaterIndex, preStoppedRollIndex, preStoppedUpdaterStopIndex, updaterIndex, updaterStopIndex) * easingConfig.stopIntervalTime;
                        }
                    }
                    break;
                case State.STATE_SPIN_CHECK_SLOW_DOWN:
                    DealAboutCheckSlowDown(deltaTime);
                    break;
                case State.STATE_SLOWDOWN:
                    DealAboutSlowDown(deltaTime);
                    break;
            }
        }

        public virtual void DealAboutCheckSlowDown(float deltaTime)
        {
            if (currentSpinDuration >= leastSpinTime && reelStopIndex < 0)
            {
                reelStopIndex = roll.ComputeReelStopIndex(currentIndex, slowDownStepCount);
            }

            if (reelStopIndex >= 0 && reelStopIndex != currentIndex)
            {
                int stepToStopIndex = currentIndex < reelStopIndex
                    ? (maxIndex - reelStopIndex +  currentIndex)
                    : currentIndex - reelStopIndex;

                if (stepToStopIndex == slowDownStepCount)
                {
                    PrepareToSlowDown(stepToStopIndex);
                    return;
                }
            }
            UpdateShiftWithCurrentSpeed(deltaTime);
        }
        public virtual void DealAboutSlowDown(float deltaTime)
        {
            easingProgress += deltaTime / slowDownDuration;
            easingProgress = easingProgress > 1 ? 1 : easingProgress;

            var t = easingProgress - 1;
            t = GetSlowStateByProcess(t);

            lastT = t;
                    
                    
            shiftAmount = slowDownStartPos + t * slowDownDistance;
                    
            UpdateShift(0);
                    
            //easingProgress<1/(1+overShoot)时t<1
            //这个判断必须放在UpdateShift之后，不然轮盘图标还没停到的结果，导致Blink音效播放出错
            if (t >= 1 && !bounceEventFired)
            {
                rollSpinningEventsCallback.startBounce.Invoke(this);
                bounceEventFired = true;
            }
                     
            if (easingProgress >= 1)
            {
                spinState = State.STATE_IDLE;
                        
                roll.DoShift(0);

                if (!bounceEventFired)
                {
                    rollSpinningEventsCallback.startBounce.Invoke(this);
                    bounceEventFired = true;
                }
                        
                rollSpinningEventsCallback.spinningEndCallback?.Invoke(this);
                rollSpinningEventsCallback.spinningEndCallback = null;
            }
        }
        #endregion

        protected virtual float GetSlowStateByProcess(float process)
        {
            return process * process * ((overShoot + 1) * process + overShoot) + 1;
        }

        protected virtual void PrepareToSlowDown(int stepToStopIndex)
        {
            //重新根据shiftAmount计算一次currentIndex;
            currentIndex = maxIndex - (int) Math.Floor((shiftAmount)) - 1;
            
            var deltaShift = shiftAmount - (maxIndex - currentIndex - 1);

            slowDownDistance = stepToStopIndex - deltaShift;

            easingProgress = 0;
            
            slowDownStartPos = shiftAmount;

            lastT = 0.0f;
            spinState = State.STATE_SLOWDOWN;
            
            rollSpinningEventsCallback.enterSlowDown.Invoke(this);
        }

        public void UpdateShiftWithCurrentSpeed(float deltaTime)
        {
            var deltaShiftAmount = currentVelocity * deltaTime;

            if (deltaShiftAmount > 0.5f)
                deltaShiftAmount = 0.5f;

            UpdateShift(deltaShiftAmount);
        }
 
        public void UpdateShift(float shiftDeltaAmount)
        {
            shiftAmount += shiftDeltaAmount;

            if (shiftAmount >= maxIndex)
            {
                shiftAmount = shiftAmount - maxIndex;
                
            } else if (shiftAmount < 0)
            {
                shiftAmount = maxIndex + shiftAmount;
            }

            var preIndex = currentIndex;

            currentIndex = maxIndex - (int) Math.Floor(shiftAmount) - 1;

            var shiftUp = false;
            if (spinState == State.STATE_SPEED_UP)
            { 
                if (currentIndex == (preIndex + 1) % maxIndex)
                { 
                    shiftUp = true; 
                    currentIndex = preIndex;
                }
            }
            
            if (preIndex != currentIndex)
            {
                if (preIndex != currentIndex + 1 && currentIndex != maxIndex - 1)
                {
                    XDebug.Log("EX:preIndex:" + preIndex);
                    XDebug.Log("EX:currentIndex:" + currentIndex);
                }
                if (spinState == State.STATE_SLOWDOWN)
                {
                    while ((preIndex - 1 + maxIndex) % maxIndex != currentIndex)
                    {
                        --preIndex;
                        preIndex = preIndex < 0 ? (preIndex + maxIndex) % maxIndex : preIndex;
                        roll.ShiftOneRow(preIndex);
                    }
                }
                
                roll.ShiftOneRow(currentIndex);
                if (spinState == State.STATE_SPIN_LOOP)
                    CheckAndForceUpdateReels();
            }

            roll.DoShift((float)((shiftAmount - Math.Floor((shiftAmount)) - (shiftUp ? 1: 0)) * stepSize));
        }

        #region OutEvent

        public void OnSpinResultReceived(bool inWaitAnticipation)
        {
            spinResultReceived = true;
            waitAnticipation = inWaitAnticipation;
            
            //             spinDuration + (updaterIndex - preStoppedRollerIndex) * easingConfig.stopIntervalTime;
            
            // if (!waitAnticipation)
            // {
            //     spinState = State.STATE_SPIN_CHECK_SLOW_DOWN;
            //
            //     if (spinDuration > easingConfig.maxSpinDuration)
            //     {
            //         leastSpinTime =
            //             spinDuration + (updaterIndex - preStoppedRollerIndex) * easingConfig.stopIntervalTime;
            //     }
            // }
        }

        public void OnQuickStopped()
        {
            if (spinResultReceived)
            {
                if (spinState != State.STATE_SLOWDOWN && spinState != State.STATE_IDLE)
                {
                    waitAnticipation = false;
                    currentIndex = maxIndex - (int) Math.Floor((shiftAmount)) - 1;
                    reelStopIndex = (currentIndex - slowDownStepCount + maxIndex) % maxIndex;
                    reelStopIndex = roll.OnReelStopAtIndex(reelStopIndex);
                    leastSpinTime = 0;
                    roll.ForceUpdateAllElement(currentIndex);
                    currentVelocity = easingConfig.spinSpeed;
                    spinState = State.STATE_SPIN_CHECK_SLOW_DOWN;
                }
            }
        }

        public void OnAnticipationStopped(int stoppedUpdaterIndex, int rollIndex, int inUpdaterStopIndex)
        {
            preStoppedUpdaterIndex = stoppedUpdaterIndex;
            preStoppedRollIndex = rollIndex;
            preStoppedUpdaterStopIndex = inUpdaterStopIndex;
            waitAnticipation = false;
        }

        private void CheckAndForceUpdateReels()
        {
            if (waitElementCountBeforeUpdateReels > 0)
            {
                waitElementCountBeforeUpdateReels--;
                
                if (waitElementCountBeforeUpdateReels == 0)
                {
                    ForceUpdateReels();
                }
            }
        }

        private void ForceUpdateReels()
        {
            //更新ElementSupplier的卷轴
            roll.elementSupplier.OnForceUpdateReel();
                    
            maxIndex = roll.elementSupplier.GetElementSequenceLength();

            //新的maxIndex可能比旧的差很多，这里要重新算shiftAmount
            if (shiftAmount >= maxIndex)
            {
                shiftAmount = shiftAmount - Mathf.Floor(shiftAmount / maxIndex) * maxIndex;
            }
            else if (shiftAmount < 0)
            {
                shiftAmount = maxIndex + shiftAmount;
            }
                    
            currentIndex = maxIndex - (int) Math.Floor(shiftAmount) - 1;

            var sequenceElement = roll.elementSupplier.GetElement(currentIndex);

            //要保证的卷轴初始化位置不为半个大图标
            while (sequenceElement.config.height > 1 && sequenceElement.config.position > 0)
            {
                shiftAmount += 1;
                if (shiftAmount >= maxIndex)
                {
                    shiftAmount = shiftAmount - maxIndex;
                }
                
                currentIndex = maxIndex - (int) Math.Floor(shiftAmount) - 1;
                sequenceElement = roll.elementSupplier.GetElement(currentIndex);
            }
        }

        /// <summary>
        /// 滚动过程中强制换了滚动的卷轴，Updater中关于卷轴相关的属性需要同步修改
        /// </summary>
        /// <returns></returns>
        public void OnForceUpdateReels()
        {
            if (!spinResultReceived)
            {
                //如果当前最上边位置为大图标，需要等到大图标转走才能开始 ForceUpdateReels
                var sequenceElement = roll.elementSupplier.GetElement(currentIndex);
                
                if (sequenceElement.config.position == sequenceElement.config.height - 1)
                {
                    ForceUpdateReels();
                    waitElementCountBeforeUpdateReels = 0;
                }
                else
                {
                    waitElementCountBeforeUpdateReels = (int)sequenceElement.config.height - (int) sequenceElement.config.position - 1;
                }
            }
        }

        public bool IsWaitAnticipation()
        {
            return waitAnticipation;
        }
        
        public void EnterAnticipation()
        {
            if (inQuickStopMode)
                return;

            if (spinState == State.STATE_SPIN_LOOP)
            {
                easingProgress = 0;
                spinSpeed = easingConfig.anticipationSpeed;
                spinState = State.STATE_SPIN_CHECK_SLOW_DOWN;
                startSpeed = easingConfig.anticipationSpeed;
                deltaSpeed = easingConfig.anticipationSpeed - startSpeed;
                leastSpinTime = currentSpinDuration + easingConfig.anticipationExtraTime;
                waitAnticipation = false;
            }
        }
        /// <summary>
        /// 当前Updater是否是激活的
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return spinState != State.STATE_IDLE;
        }

        #endregion
    }
}