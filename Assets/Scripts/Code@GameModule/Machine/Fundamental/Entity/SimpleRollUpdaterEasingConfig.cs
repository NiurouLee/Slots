// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : SimpleReelUpdaterEasingConfig.cs
// ChangeLog :
// **********************************************
using System;
namespace GameModule
{
    public class SimpleRollUpdaterEasingConfig : IRollUpdaterEasingConfig
    {
        public float speedUpDuration; //加速时长
        public float spinSpeed;  //滚轴滚动速度
        public float anticipationSpeed; //anticipation模式下的滚动速度
        public float anticipationExtraTime; //anticipation模式额外滚动时长
        public float slowDownDuration; //减速需要的时间
        public int slowDownStepCount; //减速阶段滚动的步数
        public float leastSpinDuration; //最长SPIN时间
        public float startOverShootAmount; //启动回弹量
        public float overShootAmount; //回弹量
        public float stopIntervalTime; //列间停下间隔时长
        public string reelStopSoundName;
        public string updaterTypeName;
        public float minStopMoveDistance;//停轮偏移距离最小值
        public float maxStopMoveDistance;//停轮偏移距离最大值
        public float stopMoveDuration;//停轮偏移调回时间
        public Type GetUpdaterType()
        {
            if (!string.IsNullOrEmpty(updaterTypeName))
            {
                Type updateType = Type.GetType($"GameModule.{updaterTypeName}");
                if (updateType != null && updateType.IsSubclassOf(typeof(RollSimpleUpdater)))
                {
                    return updateType;
                }
            }
            return typeof(RollSimpleUpdater);
        }

        public SimpleRollUpdaterEasingConfig()
        {
            spinSpeed = 25f;
            speedUpDuration = 0.5f;
            anticipationSpeed = 25;
            anticipationExtraTime = 1.0f;
            slowDownDuration = 0.5f;
            leastSpinDuration = 2;
            slowDownStepCount = 3;
            overShootAmount = 1.5f;
            stopIntervalTime = 0.2f;
            startOverShootAmount = 0;
            reelStopSoundName = "ReelStop";
            minStopMoveDistance = 0.3f;
            maxStopMoveDistance = 0.7f;
            stopMoveDuration = 0.2f;
        }

        public string GetReelStopSoundName()
        {
            return reelStopSoundName;
        }
    }
}