// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-27 6:25 PM
// Ver : 1.0.0
// Description : IWheelAnimationController.cs
// ChangeLog :
// **********************************************
using System;
namespace GameModule
{
    public interface IWheelAnimationController
    {
        void BindingWheel(Wheel wheel);
        
        /// <summary>
        /// 停止播放期待动画
        /// </summary>
        /// <param name="playStopSound"></param>
        void StopAnticipationAnimation(bool playStopSound = true);



        /// <summary>
        /// 当轮盘开始转动了
        /// </summary>
        void OnWheelStartSpinning();
       
        
        /// <summary>
        /// 滚轴停止了
        /// </summary>
        /// <param name="reelIndex"></param>
        /// <param name="reelLogicEnd"></param>
        void OnRollSpinningStopped(int reelIndex, Action reelLogicEnd);

        /// <summary>
        /// 滚轴开始回弹
        /// </summary>
        /// <param name="reelIndex"></param>
        void OnRollStartBounceBack(int reelIndex);

        /// <summary>
        /// 滚轴进入减速模式
        /// </summary>
        /// <param name="reelIndex"></param>
        void OnRollEnterSlowDown(int reelIndex);

        /// <summary>
        /// 所有滚轮都停止了
        /// </summary>
        void OnAllRollSpinningStopped(Action callback);
        
        /// <summary>
        /// 播放期待动画
        /// </summary>
        /// <param name="reelIndex"></param>
        void ShowAnticipationAnimation(int reelIndex);
    }
}