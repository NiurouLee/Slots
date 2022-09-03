/**********************************************
Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-11-24 21:59:03
Ver : 1.0.0
Description : 
ChangeLog :  添加基础功能的MonoBehaviour方便以后做功能扩展
**********************************************/


using UnityEngine;
using System;

public class MonoBasicEventProxy : MonoBehaviour
{
    private Action destroyEventCallback;
    private Action startEventCallback;
    private Action onEnableEventCallback;
    private Action onDisableEventCallback;
        
    public void BindDestroyEvent(Action eventCallback)
    {
        destroyEventCallback = eventCallback;
    }
    
    public void BindStartEvent(Action eventCallback)
    {
        startEventCallback = eventCallback;
    }
        
    public void BindEnableEvent(Action eventCallback)
    {
        onEnableEventCallback = eventCallback;
    }
        
    public void BindDisableEvent(Action eventCallback)
    {
        onDisableEventCallback = eventCallback;
    }

    private void Start()
    {
        startEventCallback?.Invoke();
    }

    private void OnEnable()
    {
        onEnableEventCallback?.Invoke();
    }
        
    private void OnDisable()
    {
        onDisableEventCallback?.Invoke();
    }
    private void OnDestroy()
    {
        destroyEventCallback?.Invoke();
    }
}