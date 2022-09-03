// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/17:11
// Ver : 1.0.0
// Description : UpdateScheduler.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;

public class UpdateEventListener : MonoBehaviour
{
    public static Action OnUpdate { get; set; }
    public static Action OnLateUpdate { get; set; }
    public static Action OnFixedUpdate { get; set; }
     
    void Update()
    {
        OnUpdate?.Invoke();
    }

    void LateUpdate()
    {
        OnLateUpdate?.Invoke();
    }

    void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }
}
 