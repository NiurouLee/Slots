using System;

namespace GameModule
{
    public class EventCallback:IEventCallback
    {
        private Action callBack;
        public EventCallback(Action inCallBack)
        {
            callBack = inCallBack;
        }

        public void Callback(Object EventData = null)
        {
            callBack.Invoke();
        }
    }
}