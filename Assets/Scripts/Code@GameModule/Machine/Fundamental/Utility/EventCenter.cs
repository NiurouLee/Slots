

using System;
using System.Collections.Generic;
using Dlugin;
using Google.Protobuf.Collections;

namespace GameModule
{
    public class EventCenter
    {
        private static EventCenter instance;
        public static EventCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventCenter();
                }

                return instance;
            }
        }
        private Dictionary<string, List<IEventCallback>> _EventDictionary;

        public EventCenter()
        {
            _EventDictionary = new Dictionary<string, List<IEventCallback>>();
        }
        public bool RegisterEvent(string EventName,IEventCallback Func)
        {
            if (!_EventDictionary.ContainsKey(EventName))
            {
                _EventDictionary.Add(EventName,new List<IEventCallback>());
            }
            int index = _EventDictionary[EventName].FindIndex(a => a.Equals(Func));
            if (index == -1)
            {
                _EventDictionary[EventName].Add(Func);
                XDebug.Log("RegisterEvent EventName:" + EventName + " Func:" + Func);
                return true;
            }
            XDebug.LogError("RegisterEvent Repeat EventName:" + EventName + " Func:" + Func);
            return false;
        }

        public bool RemoveEvent(string EventName,IEventCallback Func)
        {
            if (!_EventDictionary.ContainsKey(EventName))
            {
                XDebug.LogError("RemoveEvent NoEventName EventName:" + EventName + " Func:" + Func);
                return false;
            }
            
            int index = _EventDictionary[EventName].FindIndex(a => a.Equals(Func));
            if (index == -1)
            {
                XDebug.LogError("RemoveEvent NoFunc EventName:" + EventName + " Func:" + Func);
                return false;
            }
            _EventDictionary[EventName].RemoveAt(index);
            XDebug.Log("RemoveEvent EventName:" + EventName + " Func:" + Func);
            return true;
        }

        public bool PushEvent(string EventName,Object EventData = null)
        {
            if (!_EventDictionary.ContainsKey(EventName))
            {
                XDebug.Log("PushEvent NoEventName EventName:" + EventName + " EventData:"+EventData);
                return false;
            }
            List < IEventCallback > temp = new List<IEventCallback>(_EventDictionary[EventName]);
            int length = temp.Count;
            if (length != 0)
            {
                for (int i = 0; i < length; i++)
                {
                    if (temp[i]!=null)
                    {
                        temp[i].Callback(EventData);   
                    }
                    XDebug.Log("PushEvent EventName:" + EventName + " Func:" + temp[i]);
                }
                return true;
            }
            else
            {
                XDebug.Log("PushEvent NoFunc EventName:" + EventName);
                return false;
            }
        }
    }
}