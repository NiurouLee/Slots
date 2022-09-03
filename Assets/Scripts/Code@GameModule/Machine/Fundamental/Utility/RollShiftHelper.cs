// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/26/17:33
// Ver : 1.0.0
// Description : RollShiftHelper.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DG.Tweening;
using Spine;
using UnityEngine;

namespace GameModule
{
    public class RollShiftHelper : IUpdateable, IOnContextDestroy
    {
        protected float accumulateTime = 0;
        protected float duration;
        protected float distanceNeedToShift;
        protected float stepSize;
        protected bool isShiftUp;

        protected Roll rollToControl;

        protected int currentIndex;
        protected int shiftStep;
        protected int needShiftStepCount;
        protected int maxIndex;

        protected Ease easeType;

        protected TaskCompletionSource<bool> waitTask;

        protected MachineContext machineContext;

        public RollShiftHelper(MachineContext inMachineContext)
        {
            machineContext = inMachineContext;
        }

        public void OnContextDestroy()
        {
            if (waitTask != null && !waitTask.Task.IsCompleted)
            {
                UpdateScheduler.UnhookUpdate(this);
            }
        }

        public async Task ShiftRoll(Roll roll, int inShiftStepCount, bool inIsShiftUp, float inDuration,
            Ease inEaseType = Ease.OutBack)
        {
            accumulateTime = 0;
            rollToControl = roll;
            needShiftStepCount = inShiftStepCount;
            distanceNeedToShift = roll.stepSize * needShiftStepCount;
            isShiftUp = inIsShiftUp;
            duration = inDuration;
            stepSize = roll.stepSize;
            shiftStep = 0;
            easeType = inEaseType;

            waitTask = new TaskCompletionSource<bool>();

            currentIndex = roll.elementSupplier.GetStopIndex();
            maxIndex = roll.elementSupplier.GetElementSequenceLength();
            currentIndex = (currentIndex - roll.GetExtraTopElementCount() + maxIndex) % maxIndex;
            if (inIsShiftUp)
            {
                roll.ShiftOneRowUp(currentIndex,true);
                currentIndex++;

                if (currentIndex >= maxIndex)
                    currentIndex = 0;
            }

            machineContext.SubscribeDestroyEvent(this);
            UpdateScheduler.HookUpdate(this);

            machineContext.AddWaitTask(waitTask, null);
            await waitTask.Task;
        }

        public float GetEasingTime(float time)
        {
            if (time >= duration)
                return 1;

            return DOVirtual.EasedValue(0f, 1, time / duration, easeType);
        }

        public void Update()
        {
            accumulateTime += Time.smoothDeltaTime;

            var progress = GetEasingTime(accumulateTime);
            var shiftDistance = progress * distanceNeedToShift;
            
            var currentShiftStep = (int)(progress * needShiftStepCount);
            
            var deltaDistance = shiftDistance - currentShiftStep * stepSize;

            if (shiftStep != currentShiftStep)
            {
                var deltaStep = currentShiftStep - shiftStep;

                shiftStep = currentShiftStep;

                if (isShiftUp)
                {
                    while (deltaStep > 0)
                    {
                        rollToControl.ShiftOneRowUp(currentIndex,true);
                        currentIndex++;
                        if (currentIndex >= maxIndex)
                            currentIndex = 0;

                        deltaStep--;
                    }
                }
                else
                {
                    while (deltaStep > 0)
                    {
                        rollToControl.ShiftOneRow(currentIndex,true);
                        currentIndex--;

                        if (currentIndex < 0)
                        {
                            currentIndex = maxIndex - 1;
                        }

                        deltaStep--;
                    }
                }
            }

            if (isShiftUp)
                rollToControl.DoShift(-deltaDistance);
            else
            {
                rollToControl.DoShift(deltaDistance);
            }


            if (accumulateTime >= duration)
            {
                XDebug.Log($"Error:{needShiftStepCount}/{shiftStep}" );
               
                if (needShiftStepCount!= shiftStep)
                {
                    XDebug.Log($"RError:{needShiftStepCount}/{shiftStep}" );
                }
                
                rollToControl.DoShift(0);

                if (isShiftUp)
                {
                    rollToControl.ForceUpdateAllElement((currentIndex - 2 + maxIndex) % maxIndex,true);
                    rollToControl.ForceUpdateAllElementPosition();
                }

                UpdateScheduler.UnhookUpdate(this);
                machineContext.UnSubscribeDestroyEvent(this);
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            }
        }
    }
}