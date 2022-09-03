using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    /*
    public class RollSimpleUpdater11312 : RollSimpleUpdater
    {
        /// <summary>
        ///是否开启特殊回弹
        /// </summary>
        protected bool isOpenSpecialBound=false;
        // 回弹暂缓计数
        protected float bounceTimes = 0;
        public void StartSpinning(IRollUpdaterEasingConfig inEasingConfig, RollSpinningEventsCallback eventCallback)
        {
            base.StartSpinning(inEasingConfig,eventCallback);
            isOpenSpecialBound = easingConfig.isOpenSpecialBound;
        }
        public override void Update()
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
                    
                    currentVelocity = Mathf.Pow(easingProgress, 3) * spinSpeed;
                    
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
                                currentSpinDuration + roll.GetSpinningDurationMultiplier(wheelUpdaterStartIndex, preStoppedUpdaterIndex, preStoppedRollIndex, updaterIndex) * easingConfig.stopIntervalTime;
                        }
                    }
                    break;
                case State.STATE_SPIN_CHECK_SLOW_DOWN:
                    
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
                    break;
                case State.STATE_SLOWDOWN:
                    easingProgress += deltaTime / slowDownDuration;
                    easingProgress = easingProgress > 1 ? 1 : easingProgress;

                    var t = easingProgress - 1;
                    t = t * t * ((overShoot + 1) * t + overShoot) + 1;
 
                    lastT = t;
                    
                    
                    shiftAmount = slowDownStartPos + t * slowDownDistance;
                    // XDebug.Log($"=====shiftAmount:{shiftAmount} = {t}  {easingProgress}");
                    UpdateShift(0);
                    
                    // 是否开启特殊回弹方式
                    if(isOpenSpecialBound){
                        SpecialBound();
                    }else{
                        //这个判断必须放在UpdateShift之后，不然轮盘图标还没停到的结果，导致Blink音效播放出错
                        if (t >= 1 && !bounceEventFired)
                        {
                            rollSpinningEventsCallback.startBounce.Invoke(this);
                            bounceEventFired = true;
                        }
                    }
                     
                    if (easingProgress >= 1)
                    {
                        spinState = State.STATE_IDLE;

                        roll.DoShift(0);

                        if (!bounceEventFired)
                        {
                            rollSpinningEventsCallback.startBounce.Invoke(this);
                            bounceEventFired = true;
                            bounceTimes = 0;
                        }
                        
                        rollSpinningEventsCallback.spinningEndCallback?.Invoke(this);
                        rollSpinningEventsCallback.spinningEndCallback = null;
                    }
                    break;
            }
        }
        /// <summary>
        /// 特殊回弹
        /// </summary>
        private void SpecialBound(){
            // 0.63 为减速图标到该回弹的时候
            if (easingProgress >= 0.63f && bounceTimes <= 3)
            {
                easingProgress = 0.63f;
                bounceTimes += 0.1f;

                if (!bounceEventFired)
                {
                    rollSpinningEventsCallback.startBounce.Invoke(this);
                    bounceEventFired = true;
                }
                
                if (bounceTimes >= 3){
                    slowDownDuration = 0.3f;
                }
                    
            }
        }
    }*/
}

