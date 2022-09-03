// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/21:28
// Ver : 1.0.0
// Description : LogicController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class LogicController:Controller
    {
        private Client _client;

        protected bool beforeEnterLobbyServerDataReceived = false;

        public LogicController(Client client)
        {
            _client = client;
            client.RegisterLogicController(this);
            
            Initialization();
        }
 
        public virtual void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
        
        }
        
        public virtual void OnPushNotification(FortunexNotification data)
        {
        
        }
        
        public static void OnServerPushNotification(FortunexNotification data)
        {
            if (data != null && data.NotificationType == FortunexNotificationType.EmailUpdated)
            {
                Client.Get<InboxController>().OnPushNotification(data);
            }
            else if (data != null && data.NotificationType == FortunexNotificationType.TimerHourlyBonusSpeedUpdat)
            {
                Client.Get<TimeBonusController>().OnPushNotification(data);
            }
        }
        
        public virtual async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            finishCallback?.Invoke();
            await Task.CompletedTask;
        }
        
        protected virtual void Initialization()
        {
            SubscribeEvents();
        }
        
        public virtual void Start()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSpinSystemContentUpdate>(OnSpinSystemContentUpdate);
        }

        protected virtual void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {

        }
        
        protected T GetSystemData<T>(RepeatedField<AnyStruct> systemContent, string filter) where T : Google.ilruntime.Protobuf.IMessage
        {
            if (systemContent!=null && systemContent.Count>0)
            {
                for (int i = 0; i < systemContent.Count; i++)
                {
                    var extraData = systemContent[i];
                    if (extraData.Type == filter)
                    {
                        return ProtocolUtils.GetAnyStruct<T>(extraData);
                    }
                }
            }
            return default;
        }

        public virtual bool IsDataReady()
        {
            return false;
        }
    }
}