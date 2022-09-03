// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/18:03
// Ver : 1.0.0
// Description : ApplicationEventHandler.cs
// ChangeLog :
// **********************************************

using System;

namespace GameModule
{
    public class ApplicationEventScheduler : IExternalEventScheduler
    {
        public static bool ApplicationIsPaused;

        public void AttachToListener()
        {
            ApplicationEventListener.OnApplicationFocusAction = OnApplicationFocus;
            ApplicationEventListener.OnApplicationPauseAction = OnApplicationPause;
            ApplicationEventListener.OnApplicationQuitAction = OnApplicationQuit;
            ApplicationEventListener.OnDeepLinkActivateAction = OnApplicationDeepLink;
        }

        public void DetachFromListener()
        {
            ApplicationEventListener.OnApplicationFocusAction = null;
            ApplicationEventListener.OnApplicationPauseAction = null;
            ApplicationEventListener.OnApplicationQuitAction = null;
            ApplicationEventListener.OnDeepLinkActivateAction = null;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            bool diff = ApplicationIsPaused == hasFocus;

            if (diff)
            {
                ApplicationIsPaused = !hasFocus;
                if (ApplicationIsPaused)
                    EventBus.Dispatch(new EventGameOnPause());
                else
                    EventBus.Dispatch(new EventGameOnFocus());
            }
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            bool diff = ApplicationIsPaused != pauseStatus;

            if (diff)
            {
                ApplicationIsPaused = pauseStatus;
                if (ApplicationIsPaused)
                    EventBus.Dispatch(new EventGameOnPause());
                else
                    EventBus.Dispatch(new EventGameOnFocus());
            }
        }

        public void OnApplicationQuit()
        {
            EventBus.Dispatch(new EventOnApplicationQuit());
        }
        
        public void OnApplicationDeepLink(string url)
        {
            EventBus.Dispatch(new EventOnApplicationDeepLink(), null);
        }
    }
}