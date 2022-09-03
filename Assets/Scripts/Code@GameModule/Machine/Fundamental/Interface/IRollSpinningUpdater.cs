// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:40 AM
// Ver : 1.0.0
// Description : IReelRoller.cs
// ChangeLog :
// **********************************************
using System;

namespace GameModule
{
    public struct RollSpinningEventsCallback
    {
        public Action<IRollSpinningUpdater> leaveSpeedUp;
        public Action<IRollSpinningUpdater> enterSlowDown;
        public Action<IRollSpinningUpdater> startBounce;
        public Action<IRollSpinningUpdater> spinningEndCallback;

        public RollSpinningEventsCallback(Action<IRollSpinningUpdater> inLeaveSpeedUp, Action<IRollSpinningUpdater> inEnterSlowDown, Action<IRollSpinningUpdater> inStartBounce, Action<IRollSpinningUpdater> inSpinningEndCallback)
        {
            leaveSpeedUp = inLeaveSpeedUp;
            spinningEndCallback = inSpinningEndCallback;
            enterSlowDown = inEnterSlowDown;
            startBounce = inStartBounce;
        }
    }
    
    public interface IRollSpinningUpdater
    {
        
        //Updater的Index，用于计算控制轮盘卷轴先后停下的顺序
        int UpdaterIndex { get;}
        
        //该Updater缓动的类型
        RollerType RollerType { get;}
         
        //控制滚轴的Reel Index
        int RollIndex { get;}
        
        int UpdaterStopIndex { get;}
        /// <summary>
        /// 开始一轮Spin
        /// </summary>
        /// <param name="config"></param>
        /// <param name="spinningEndCallback"></param>
        /// <param name="enterEaseOutMode"></param>
        /// <param name="startBounce"></param>
        /// <param name="eventCallback"></param>
        void StartSpinning(IRollUpdaterEasingConfig config, RollSpinningEventsCallback eventCallback);

        /// <summary>
        /// 更新回调
        /// </summary>
        void Update();

        /// <summary>
        /// 进入Anticipation模式
        /// </summary>
        void EnterAnticipation();
        
        /// <summary>
        /// 收到Spin 结果通知
        /// </summary>
        /// <param name="needWaitAnticipation"></param>
        void OnSpinResultReceived(bool needWaitAnticipation);
        
        /// <summary>
        /// 触发回调的时候通知
        /// </summary>
        void OnQuickStopped();
        
        /// <summary>
        /// 等待drum结束的回调
        /// </summary>
        /// <param name="stoppedRollIndex">停止动画的Updater的index</param>
        /// <param name="rollIndex">停止动画的Roll的index</param>
        /// <param name="updaterStopIndex">停止动画的Roll的updaterStopIndex</param>
        void OnAnticipationStopped(int stoppedRollIndex, int rollIndex, int updaterStopIndex);

        /// <summary>
        /// 该函数用于复用Updater的时候，更新控制的对象
        /// </summary>
        /// <param name="wheelUpdaterStartIndex"></param>
        /// <param name="updaterIndex"></param>
        /// <param name="updateStopIndex"></param>
        /// <param name="reel"></param>
        void UpdateRollToControl(int wheelUpdaterStartIndex, int updaterIndex, int updateStopIndex, Roll reel);
        
        /// <summary>
        /// 当前Updater是否是激活的
        /// </summary>
        /// <returns></returns>
        bool IsActive();

        bool IsWaitAnticipation();
         
        /// <summary>
        /// 滚动过程中强制换了滚动的卷轴，Updater中关于卷轴相关的属性需要同步修改
        /// </summary>
        /// <returns></returns>
        void OnForceUpdateReels();
    }
}