// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/16:37
// Ver : 1.0.0
// Description : AutoSpinState.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.PlayerProperties;
using UnityEngine;

namespace GameModule
{
    public class AutoSpinState : SubState
    {
        /// <summary>
        /// 当前是否是AutoSpin
        /// </summary>
        public bool IsAutoSpin { get; private set; } = false;
        
        /// <summary>
        /// AutoSpin 剩余次数
        /// </summary>
        public int AutoLeftCount { get; private set; } = 0;
        
        //下次Spin停止AutoSpin
        public bool StopAutoOnNextSpin { get; set; }

        private NeverSleepController _neverSleepController;
        
        public AutoSpinState(MachineState machineState) : base(machineState)
        {
            
        }
        
        public void OnSelectAutoSpin(int selectOption)
        {
            AutoLeftCount = selectOption;
            IsAutoSpin = true;
           // Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void EnableNeverSleep()
        {
            if (_neverSleepController == null)
            {
                _neverSleepController =
                    machineState.machineContext.transform.gameObject.AddComponent<NeverSleepController>();
            }
        }
        public void DisableNeverSleep()
        {
            if (_neverSleepController != null)
            {
                Object.Destroy(_neverSleepController);
                _neverSleepController = null;
            }
        }
         
        public override void UpdateStateOnRoundFinish()
        {
            if (IsAutoSpin)
            {
                AutoLeftCount--;
                if (AutoLeftCount == 0)
                {
                    IsAutoSpin = false;
                }
            }
        }
        public void OnDisableAutoSpin()
        {
            IsAutoSpin = false;
            AutoLeftCount = 0;
           // Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}