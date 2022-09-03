// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/06/17:43
// Ver : 1.0.0
// Description : SimpleBonusWheelRoll.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public interface IElementProvider
    {
        int GetReelMaxLength();
        GameObject GetElement(int index);
 
        int GetElementMaxHeight();

        int ComputeReelStopIndex(int currentIndex,int slowDownStepCount);

        int OnReelStopAtIndex(int currentIndex);
    }
    
    public class SingleColumnWheel
    {
        private Transform _transform;
        private Transform _rollTransform;
        
        private List<Transform> _elementContainer;
        private List<float> _initPosition;

        private Action _bounceBack;
        private Action _startSlowDown;
        private Action<GameObject> _recycleElementFunc;
        protected  IElementProvider elementProvider;
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

  
        protected SimpleRollUpdaterEasingConfig easingConfig;

        protected int rowCount;

        protected Action spinEndCallback;

        protected int shiftRowArrowIndex = 0;

        protected bool isHorizontal = false;
        
        public SingleColumnWheel(Transform inTransform, float wheelHeight, int inRowCount, IElementProvider inElementProvider, int inStartIndex, bool inIsHorizontal = false,Action<GameObject> inRecycleFunc = null)
        {
            _transform = inTransform;
            currentSpinDuration = 0;
            easingProgress = 0;
            spinState = State.STATE_IDLE;
            spinSpeed = 10;

            isHorizontal = inIsHorizontal;
           
            reelStopIndex = -1;
            slowDownStepCount = 5;
            overShoot = 0.2f;
            shiftAmount = 0;
 
            rowCount = inRowCount;

            stepSize = wheelHeight / rowCount;
            elementProvider = inElementProvider;
            
            rowCount += elementProvider.GetElementMaxHeight() - 1;
            
            currentIndex = inStartIndex;
           
            maxIndex = elementProvider.GetReelMaxLength();
            shiftAmount = maxIndex - currentIndex - 1;

            _recycleElementFunc = inRecycleFunc;
            SetUpRoll(wheelHeight);
        }

        public void SetUpRoll(float wheelHeight)
        {
            _rollTransform = _transform.Find("Roll");
            
            if (_rollTransform == null)
            {
                _rollTransform = _transform;
            }
            
            _initPosition = new List<float>(rowCount + 1);
            _elementContainer = new List<Transform>(rowCount + 1);
                
            for (var i = 0; i <= rowCount; i++)
            {
                var container = new GameObject("Container");
                container.transform.SetParent(_rollTransform,false);
                
                if (isHorizontal)
                {
                    _initPosition.Add(-wheelHeight * 0.5f + (i - 0.5f) * stepSize);
                    container.transform.localPosition = new Vector3(_initPosition[i], 0, 0);
                }
                else
                {
                    _initPosition.Add(wheelHeight * 0.5f - (i - 0.5f) * stepSize);
                    container.transform.localPosition = new Vector3(0, _initPosition[i], 0);
                }

                _elementContainer.Add(container.transform);
            }

            ForeUpdateElementContainer(currentIndex);
        }
        
        
        public void StartSpinning(IRollUpdaterEasingConfig inEasingConfig, Action inSpinEndCallback, int startIndex, Action bounceBack = null, Action startSlowDown = null,bool keepSpinning = false)
        {
            easingConfig = (SimpleRollUpdaterEasingConfig) inEasingConfig;

            _bounceBack = bounceBack;
            _startSlowDown = startSlowDown;
            
            spinEndCallback = inSpinEndCallback;
            
            currentSpinDuration = 0;
            easingProgress = 0;
            reelStopIndex = -1;
            spinState = State.STATE_SPEED_UP;
        
            spinSpeed = easingConfig.spinSpeed;

            speedUpDuration = easingConfig.speedUpDuration;
            slowDownStepCount = easingConfig.slowDownStepCount;
            overShoot = easingConfig.overShootAmount;
            slowDownDuration = easingConfig.slowDownDuration;
            leastSpinTime = easingConfig.leastSpinDuration;
            
            maxIndex = elementProvider.GetReelMaxLength();
            currentIndex = startIndex;
            if (keepSpinning)
            {
                var extraPartAmount = shiftAmount - (float)Math.Floor(shiftAmount);
                shiftAmount = maxIndex - currentIndex - 1;
                shiftAmount += extraPartAmount;
            }
            else
            {
                shiftAmount = maxIndex - currentIndex - 1;   
            }
            spinResultReceived = false;
        }

        public void ForceStateToLoop()
        {
            currentVelocity = spinSpeed;
            spinState = State.STATE_SPIN_LOOP;
        }
        public void ForceStateToIdle()
        {
            currentVelocity = spinSpeed;
            spinState = State.STATE_IDLE;
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }

        #region RollerUpdate

        public void Update()
        {
            if (spinState == State.STATE_IDLE)
                return;

            var deltaTime = Time.smoothDeltaTime;
           
            currentSpinDuration += deltaTime;

            switch (spinState)
            {
                case State.STATE_SPEED_UP:
                    easingProgress += deltaTime / speedUpDuration;

                    if (easingProgress > 1)
                    {
                        easingProgress = 1;
                        spinState = State.STATE_SPIN_LOOP;
                        currentVelocity = spinSpeed;
                        return;
                    }

                    currentVelocity = Mathf.Pow(easingProgress, 3) * spinSpeed;

                    UpdateShiftWithCurrentSpeed(deltaTime);
                    break;


                case State.STATE_SPIN_LOOP:
                    UpdateShiftWithCurrentSpeed(deltaTime);

                    if (spinResultReceived)
                    {
                        spinState = State.STATE_SPIN_CHECK_SLOW_DOWN;

                        if (currentSpinDuration > leastSpinTime)
                        {
                            leastSpinTime = currentSpinDuration;
                        }
                    }

                    break;

                case State.STATE_SPIN_CHECK_SLOW_DOWN:

                    if (currentSpinDuration >= leastSpinTime && reelStopIndex < 0)
                    {
                        reelStopIndex = elementProvider.ComputeReelStopIndex(currentIndex, slowDownStepCount);
                    }

                    if (reelStopIndex >= 0 && reelStopIndex != currentIndex)
                    {
                        int stepToStopIndex = currentIndex < reelStopIndex
                            ? (maxIndex - reelStopIndex + currentIndex)
                            : currentIndex - reelStopIndex;

                        if (stepToStopIndex == slowDownStepCount)
                        {
                            PrepareToSlowDown(stepToStopIndex);
                            _startSlowDown?.Invoke();
                            _startSlowDown = null;
                            return;
                        }
                    }

                    UpdateShiftWithCurrentSpeed(deltaTime);

                    break;
                
                case State.STATE_SLOWDOWN:

                    easingProgress += deltaTime / slowDownDuration;
                    easingProgress = easingProgress > 1 ? 1 : easingProgress;

                    var t = easingProgress - 1;
                    t = GetSlowStateByProcess(t);
 
                    shiftAmount = slowDownStartPos + t * slowDownDistance;

                    if (t > 1)
                    {
                        _bounceBack?.Invoke();
                        _bounceBack = null;
                    }

                    UpdateShift(0);

                    if (easingProgress >= 1)
                    {
                        spinState = State.STATE_IDLE;

                        DoShift(0);

                        spinEndCallback.Invoke();
                    }
                    break;
            }
        }

        #endregion

        protected virtual float GetSlowStateByProcess(float process)
        {
            return process * process * ((overShoot + 1) * process + overShoot) + 1;
        }

        
        private void PrepareToSlowDown(int stepToStopIndex)
        {
            var deltaShift = shiftAmount - (maxIndex - currentIndex - 1);

            slowDownDistance = stepToStopIndex - deltaShift;

            easingProgress = 0;

            slowDownStartPos = shiftAmount;
            
            spinState = State.STATE_SLOWDOWN;
        }

        public void UpdateShiftWithCurrentSpeed(float deltaTime)
        {
            var deltaShiftAmount = currentVelocity * deltaTime;

            if (deltaShiftAmount > 0.5f)
                deltaShiftAmount = 0.5f;

            UpdateShift(deltaShiftAmount);
        }
        
        public void OnQuickStopped()
        {
            if (spinResultReceived)
            {
                if (spinState != State.STATE_SLOWDOWN && spinState != State.STATE_IDLE)
                {
                    reelStopIndex = (currentIndex - slowDownStepCount + maxIndex) % maxIndex;
                    reelStopIndex = elementProvider.OnReelStopAtIndex(reelStopIndex);
                    leastSpinTime = 0;
                    ForeUpdateElementContainer(currentIndex);
                    currentVelocity = easingConfig.spinSpeed;
                    spinState = State.STATE_SPIN_CHECK_SLOW_DOWN;
                }
            }
        }

        public void UpdateShift(float shiftDeltaAmount)
        {
            shiftAmount += shiftDeltaAmount;

            if (shiftAmount >= maxIndex)
            {
                shiftAmount = shiftAmount - maxIndex;
            }

            var preIndex = currentIndex;

            currentIndex = maxIndex - (int) Math.Floor((shiftAmount)) - 1;

            while (preIndex != currentIndex)
            {
                preIndex = (preIndex - 1 + maxIndex) % maxIndex;
                ShiftOneRow(preIndex);
            }

            DoShift((float) ((shiftAmount - Math.Floor((shiftAmount))) * stepSize));
        }
 
        public void OnSpinResultReceived()
        {
            spinResultReceived = true;
        }

        public Transform GetElement(int containerIndex)
        {
            int offsetIndex = shiftRowArrowIndex + 1 + containerIndex;
            return _elementContainer[offsetIndex % (rowCount + 1)].GetChild(0);
        }
        
        public void DoShift(float amount)
        {
            XDebug.Log("ShiftAmount:" + amount);
            
            if (isHorizontal)
            {
                _rollTransform.localPosition =
                    new Vector3(amount, 0, 0);
            }
            else
            {
                _rollTransform.localPosition =
                    new Vector3(0, -amount, 0);
            }
        }

        public void ForeUpdateElementContainer(int index)
        {
            currentIndex = index;
            shiftAmount = maxIndex - currentIndex - 1;
            _rollTransform.localPosition = Vector3.zero;
            ForceUpdateElementPosition();
            for (var i = 0; i <= rowCount; i++)
            {
                if (_elementContainer[i].childCount > 0)
                {
                    var oldElement = _elementContainer[i].GetChild(0);
                    if (_recycleElementFunc != null)
                    {
                        _recycleElementFunc(oldElement.gameObject);
                    }
                    else
                    {
                        GameObject.Destroy(oldElement.gameObject);
                    }
                }
                
                var element = elementProvider.GetElement((currentIndex + i) % maxIndex);
                element.transform.SetParent(_elementContainer[i],false);
            }
        }
        
        public virtual void ForceUpdateElementPosition()
        {
            shiftRowArrowIndex = 0;
            var containerCount = rowCount + 1;

            for (var i = 0; i <= rowCount; i++)
            {
                if (isHorizontal)
                {
                    _elementContainer[(shiftRowArrowIndex + i) % containerCount].transform.localPosition =
                        new Vector3(_initPosition[i], 0 , 0);
                }
                else
                {
                    _elementContainer[(shiftRowArrowIndex + i) % containerCount].transform.localPosition =
                        new Vector3(0, _initPosition[i], 0);
                }
            }
        }

        public void ShiftOneRow(int index)
        {
            shiftRowArrowIndex--;

            if (shiftRowArrowIndex < 0)
                shiftRowArrowIndex = rowCount;

            var nextContainerView = _elementContainer[shiftRowArrowIndex];

            var element = elementProvider.GetElement(index);

            for (var i = 0; i < nextContainerView.childCount; i++)
            {
                if (_recycleElementFunc != null)
                {
                    _recycleElementFunc(nextContainerView.GetChild(i).gameObject);
                }
                else
                {
                    GameObject.Destroy(nextContainerView.GetChild(i).gameObject);
                }
            }

            element.transform.SetParent(nextContainerView,false);

            var containerCount = rowCount + 1;

            for (var i = 0; i <= rowCount; i++)
            {
                if (isHorizontal)
                {
                    _elementContainer[(shiftRowArrowIndex + i) % containerCount].transform.localPosition =
                        new Vector3(_initPosition[i], 0 , 0);
                }
                else
                {
                    _elementContainer[(shiftRowArrowIndex + i) % containerCount].transform.localPosition =
                        new Vector3(0, _initPosition[i], 0);
                }
            }
        }
    }
}