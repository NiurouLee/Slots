// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/17:58
// Ver : 1.0.0
// Description : ApplicationEventScheduler.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;

public class ApplicationEventListener : MonoBehaviour
{
    public static Action OnApplicationQuitAction { get; set; }

    public static Action<bool> OnApplicationPauseAction { get; set; }

    public static Action<bool> OnApplicationFocusAction { get; set; }

    public static Action<string> OnDeepLinkActivateAction { get; set; }

    private void Awake()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;

        if (!String.IsNullOrEmpty(Application.absoluteURL))
        {
            // cold start and Application.absoluteURL not null so process Deep Link
            OnDeepLinkActivated(Application.absoluteURL);
        }
    }


    void OnApplicationQuit()
    {
        OnApplicationQuitAction?.Invoke();

        ILRuntimeHelp.Dispose();

        GC.Collect();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        OnApplicationFocusAction?.Invoke(hasFocus);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        OnApplicationPauseAction?.Invoke(pauseStatus);
    }

    private void OnDeepLinkActivated(string url)
    {
        OnDeepLinkActivateAction?.Invoke(url);
    }
}