using System;

namespace GameModule
{
    public interface IEventCallback
    {
        void Callback(Object EventData = null);
    }
}