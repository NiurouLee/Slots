// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : MachineInternalEvent.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public enum MachineInternalEvent
    {
        /// <summary>
        ///控制面板点击了MaxBET
        /// </summary>
        EVENT_CONTROL_MAX_BET,
        /// <summary>
        /// 控制面板点击了加BET
        /// </summary>
        EVENT_CONTROL_ADD_BET,
        /// <summary>
        /// 控制面板点击了减少BET
        /// </summary>
        EVENT_CONTROL_MINUS_BET,
        /// <summary>
        /// 控制面板点击了SPIN
        /// </summary>
        EVENT_CONTROL_SPIN,
        /// <summary>
        /// 控制面板点击了AUTO_SPIN
        /// </summary>
        EVENT_CONTROL_AUTO_SPIN,
        /// <summary>
        /// 控制面板点击了STOP
        /// </summary>
        EVENT_CONTROL_STOP,
        /// <summary>
        /// 控制面板点击AUTO STOP
        /// </summary>
        EVENT_CONTROL_AUTO_SPIN_STOP,
        /// <summary>
        /// 老虎机被暂停了
        /// </summary>
        EVENT_MACHINE_PAUSED,
        /// <summary>
        /// 老虎机从暂停恢复了
        /// </summary>
        EVENT_MACHINE_RESUMED,
        
        /// <summary>
        /// 等待的某个WaitEvent完成了
        /// </summary>
        EVENT_WAIT_EVENT_COMPLETE,
        
        /// <summary>
        /// 收到了服务器的Spin数据
        /// </summary>
        EVENT_SERVER_SPIN_DATA_RECEIVED,
        
        /// <summary>
        /// Bet发生了变化
        /// </summary>
        EVENT_BET_CHANGED,
        
        /// <summary>
        /// 显示PayTable
        /// </summary>
        EVENT_UI_PAY_TABLE,
        
        /// <summary>
        /// sound 音效开关发生变化
        /// </summary>
        EVENT_SOUND_PREFERENCE_STATUS_CHANGE,
        
        /// <summary>
        /// music 音效开关状态发生变化
        /// </summary>
        EVENT_MUSIC_PREFERENCE_STATUS_CHANGE,
        
        
        /// <summary>
        /// 玩家点击UI，解锁被锁住的feature
        /// </summary>
        EVENT_CLICK_UI_UNLOCK_GAME_FEATURE,
        
        /// <summary>
        /// 新的BET解锁了
        /// </summary>
        EVENT_MAX_BET_UNLOCKED,
        /// <summary>
        /// 机台点击事件
        /// </summary>
        EVENT_CUSTOM_CLICK,
    }
}