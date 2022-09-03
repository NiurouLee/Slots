// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/29/16:33
// Ver : 1.0.0
// Description : LocalNotificationController.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Account;
using UnityEngine;

namespace GameModule
{
    public class LocalNotificationController:LogicController
    {
        protected List<int> notificationIds;
        public LocalNotificationController(Client client)
            : base(client)
        {
            notificationIds = new List<int>();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventOnApplicationQuit>(OnApplicationQuit);
            SubscribeEvent<EventGameOnFocus>(OnApplicationFocus);
            SubscribeEvent<EventGameOnPause>(OnApplicationPause);
        }

        public void OnApplicationQuit(EventOnApplicationQuit evt)
        {
            PushLocalNotifications();
        }
        
        public void PushLocalNotifications()
        {
            //未登录不弹出推送
            if (!AccountManager.Instance.HasLogin) 
                return;
            LocalNotification.ClearNotifications();
            

            SetStoreCollectNotification();
            SetLuckyWheelCollectNotification();
            
        }

        public void OnApplicationPause(EventGameOnPause evt)
        {
            PushLocalNotifications();
            Debug.Log("[OnApplication] Pause");
        }
        
        public void SetStoreCollectNotification()
        {
            float leftTime = Client.Get<IapController>().GetPaymentHandler<StorePaymentHandler>().GetStoreBonusCountDown() + 1;

            int notificationId = -1;
            
            if (leftTime <= 0)
            {
                //50秒后推送
                notificationId = LocalNotification.SendNotification(1, 50000, "Hi there", "👉 Tap 💖 to collect your Store Bonus!", new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
            }
            else
            {
                //可领取后推送
                notificationId  = LocalNotification.SendNotification(1, (long)leftTime * 1000, "Hi there", "👉 Tap 💖 to collect your Store Bonus!", new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
            }

            if (notificationId > 0)
            {
                notificationIds.Add(notificationId);
            }
        }

        public void SetLuckyWheelCollectNotification()
        {
            var timeBonusController = Client.Get<TimeBonusController>();
         
            if (timeBonusController != null && timeBonusController.IsDataReady())
            {
                int notificationId = -1;

                float leftTime = timeBonusController.GetWheelBonusCountDown();

                if (leftTime <= 0)
                {
                    //50秒后推送
                    notificationId = LocalNotification.SendNotification(2, 50000, "Hi there",
                        "➡️ Lucky Wheel is ready! SPIN NOW and be a LUCKY STAR!", new Color32(0xff, 0x44, 0x44, 255),
                        true, true, true, "app_icon");
                }
                else
                {
                    //可领取后推送
                    notificationId = LocalNotification.SendNotification(2, (long) leftTime * 1000, "Hi there",
                        "➡️ Lucky Wheel is ready! SPIN NOW and be a LUCKY STAR!", new Color32(0xff, 0x44, 0x44, 255),
                        true, true, true, "app_icon");
                }
                
                if (notificationId > 0)
                {
                    notificationIds.Add(notificationId);
                }
            }
        }
        
        public void OnApplicationFocus(EventGameOnFocus evt)
        {
#if UNITY_ANDROID
            for (var i = 0; i < notificationIds.Count; i++)
            {
                LocalNotification.CancelNotification(notificationIds[i]);
            }
#endif
            notificationIds.Clear();
            
            //LocalNotification.ClearNotifications();
            //Debug.Log("[OnApplication] Focus");

            CheckNotificationSetting();
        }
        
        public async void CheckNotificationSetting()
        {
            // var mailPop = PopupStack.GetPopup<EmailMainPopup>();
            // if (mailPop == null) return;
            //
            // bool isUserNotificationEnabled = DragonU3DSDK.DragonNativeBridge.IsUserNotificationEnabled();
            //
            // var emailData = Get<EmailController>();
            // var emailList = emailData.emailModel.GetMails();
            //
            // for (int i = 0; i < emailList.Count; i++)
            // {
            //     if(emailList[i].MailSubType == (int)SubMailType.Subscribe && isUserNotificationEnabled)
            //     {
            //         //有邮件且开启了推送
            //         var pop = PopupStack.GetPopup<PushRewardPopup>();
            //         if (pop == null)
            //             await PopupStack.ShowPopup<PushRewardPopup>();
            //     }
            // }
        }
    }
}