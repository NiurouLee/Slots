using System;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class RollRandomMoveUpdater : RollSimpleUpdater
    {
        private float minStopMoveDistance;
        private float maxStopMoveDistance;
        private float stopMoveDuration;
        private float stopMoveDistance;
        protected float stopMoveProgress;
        private float slowDownTotalTime;

        public RollRandomMoveUpdater() : base()
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

        public override void StartSpinning(IRollUpdaterEasingConfig inEasingConfig,
            RollSpinningEventsCallback eventCallback)
        {
            base.StartSpinning(inEasingConfig, eventCallback);
            minStopMoveDistance = easingConfig.minStopMoveDistance;
            maxStopMoveDistance = easingConfig.maxStopMoveDistance;
            stopMoveDuration = easingConfig.stopMoveDuration;
            if (maxStopMoveDistance < minStopMoveDistance)
                maxStopMoveDistance = minStopMoveDistance;
            stopMoveProgress = 0;
            var tempStopMoveDistance = Random.Range(minStopMoveDistance, maxStopMoveDistance);
            stopMoveDistance = Random.Range(0, 2) == 1 ? tempStopMoveDistance : -tempStopMoveDistance;
        }

        public override void DealAboutCheckSlowDown(float deltaTime)
        {
            if (currentSpinDuration >= leastSpinTime && reelStopIndex < 0)
            {
                reelStopIndex = roll.ComputeReelStopIndex(currentIndex, slowDownStepCount);
            }

            if (reelStopIndex >= 0 && reelStopIndex != currentIndex)
            {
                int stepToStopIndex = currentIndex < reelStopIndex
                    ? (maxIndex - reelStopIndex + currentIndex)
                    : currentIndex - reelStopIndex;

                if (stepToStopIndex == slowDownStepCount)
                {
                    PrepareToSlowDown(stepToStopIndex);
                    return;
                }
            }

            UpdateShiftWithCurrentSpeed(deltaTime);
        }

        public override void DealAboutSlowDown(float deltaTime)
        {
            slowDownTotalTime += deltaTime;
            easingProgress = slowDownTotalTime / slowDownDuration;
            easingProgress = easingProgress > 1 ? 1 : easingProgress;
            stopMoveProgress = (slowDownTotalTime - slowDownDuration) / stopMoveDuration;
            stopMoveProgress = stopMoveProgress > 1 ? 1 : stopMoveProgress;

            if (easingProgress < 1)
            {
                var t = easingProgress - 1;
                t = GetSlowStateByProcess(t);
                shiftAmount = slowDownStartPos + t * (slowDownDistance + stopMoveDistance);
                lastT = t * (slowDownDistance + stopMoveDistance) / slowDownDistance;
            }
            else
            {
                var t = stopMoveProgress;
                t = GetStopMoveStateByProcess(t);
                shiftAmount = slowDownStartPos + slowDownDistance + stopMoveDistance * (1 - t);
                lastT = (slowDownDistance + stopMoveDistance * (1 - t)) / slowDownDistance;
            }

            UpdateShift(0);
            if (stopMoveProgress >= 1 && !bounceEventFired)
            {
                rollSpinningEventsCallback.startBounce.Invoke(this);
                bounceEventFired = true;
            }

            if (stopMoveProgress >= 1)
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

        protected virtual float GetStopMoveStateByProcess(float process)
        {
            return process;
        }

        protected override float GetSlowStateByProcess(float process)
        {
            return 1 - process * process;
        }

        protected override void PrepareToSlowDown(int stepToStopIndex)
        {
            stopMoveProgress = 0;
            slowDownTotalTime = 0;
            base.PrepareToSlowDown(stepToStopIndex);
        }
    }
}