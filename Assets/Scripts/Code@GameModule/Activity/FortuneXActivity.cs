using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using static DragonU3DSDK.Network.API.ILProtocol.SGetActivityUserData.Types;

namespace GameModule
{
    public class FortuneXActivity
    {
        public string activityID { get; private set; }
        public string activityType { get; private set; }

        public virtual bool isExpired { get; protected set; }

        public virtual bool isValid { get; protected set; }

        protected List<EventBus.Listener> listeners;
        protected ActivityController activityController;
        protected InboxController inboxController;

        public virtual void OnCreate(Activity activity)
        {
            this.activityID = activity.ActivityId;
            this.activityType = activity.ActivityType;

            activityController = Client.Get<ActivityController>();
            inboxController = Client.Get<InboxController>();
        }

        // override this to parse custom data
        public virtual void OnRefreshUserData(ActivityData activityData) { }

        public virtual void OnEnterLobby() { }

        // override this to attach system widgets in machine
        public virtual Task OnAttachSystemWidgets(MachineContext machineContext, GameObject widgetGameObject)
        {
            return Task.CompletedTask;
        }

        public virtual void OnUpdate() { }

        public virtual void OnExpire() { }

        public virtual void OnBannerJump(JumpInfo jumpInfo) { }

        public virtual bool OnCheckBannerValid(JumpInfo jumpInfo) { return true; }

        public virtual void OnUpdateBannerContent(Advertisement advertisement, View parentView, Transform bannerContent) { }

        public virtual void OnDestroy() { CleanAllSubscribedEvents(); }

        public void CleanAllSubscribedEvents()
        {
            if (listeners == null)
                return;
            for (var i = 0; i < listeners.Count; i++)
            {
                EventBus.UnSubscribe(listeners[i]);
            }

            listeners.Clear();
        }

        public bool SubscribeEvent<T>(Action<T> handleAction) where T : IEvent
        {
            if (listeners == null)
            {
                listeners = new List<EventBus.Listener>();
            }

            var listener = EventBus.Subscribe<T>(handleAction);

            if (listener != null)
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }

        public bool SubscribeEvent<T>(Action<Action, T, IEventHandlerScheduler> handleAction, int priority) where T : IEvent
        {
            if (listeners == null)
            {
                listeners = new List<EventBus.Listener>();
            }

            var listener = EventBus.Subscribe<T>(handleAction, priority);

            if (listener != null)
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }

        public bool UnsubscribeEvent<T>(Action<Action, T, IEventHandlerScheduler> handleAction) where T : IEvent
        {
            if (listeners == null)
                return false;

            for (var i = 0; i < listeners.Count; i++)
            {
                if (listeners[i].eventHandler == (Delegate)handleAction)
                {
                    EventBus.UnSubscribe(listeners[i]);
                    listeners.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        // call this to refresh user data
        public async Task RequestActivityUserDataAsync()
        {
            var ids = new List<string>();
            ids.Add(activityID);
            var response = await activityController.RequestCGetActivityUserDataAsync(ids);

            ActivityData activityData = null;

            if (response != null && response.ActivityDatas != null && response.ActivityDatas.Count > 0)
            {
                var datas = response.ActivityDatas;

                datas.TryGetValue(activityID, out activityData);

            }

            OnRefreshUserData(activityData);
        }
    }
}
