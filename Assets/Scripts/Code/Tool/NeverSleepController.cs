// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/17/14:51
// Ver : 1.0.0
// Description : ScreenSleepController:MonoBehaviour.cs
// ChangeLog :
// **********************************************

using UnityEngine;
public class NeverSleepController: MonoBehaviour
{
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void OnDestroy()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }
}

 