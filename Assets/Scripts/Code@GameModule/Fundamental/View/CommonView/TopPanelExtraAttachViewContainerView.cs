// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/20/12:08
// Ver : 1.0.0
// Description : TopPanelExtraAttachViewContainerView.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    //挂在TopView上面的其他View的容器View
    public class TopPanelExtraAttachViewContainerView : View<TopPanelExtraAttachViewContainerViewController>
    {
        public LevelUpNoticeView levelUpNoticeView;

        public MachineUnlockView machineUnlockView;

        public JackpotNotificationView jackpotNotificationView;


        public async void ShowLevelUpView(LevelUpInfo levelUpInfo)
        {
            if (levelUpNoticeView == null)
                levelUpNoticeView = await AddChild<LevelUpNoticeView>();
            levelUpNoticeView.ShowNoticeView(levelUpInfo, parentView.transform.gameObject.name.Contains("V"));
        }

        public async void ShowMachineUnlockView(LevelUpInfo levelUpInfo)
        {
            if (machineUnlockView == null)
            {
                machineUnlockView = await AddChild<MachineUnlockView>();
            }

            machineUnlockView.ShowUnlockMachineTip(levelUpInfo.UnlockedMachines[0], parentView.transform.gameObject.name.Contains("V"));
        }

        public async void AttachAndShowJackpotNotificationView()
        {
            if (jackpotNotificationView == null)
            {
                jackpotNotificationView = await AddChild<JackpotNotificationView>();
            }
        }

        public async void ShowJackpotNotificationView(SJackpotNotification data)
        {
            if (jackpotNotificationView == null)
            {
                jackpotNotificationView = await AddChild<JackpotNotificationView>();

            }
            jackpotNotificationView.ShowView(data);
        }

        public async void ShowCardDropView(CardUpdateInfo cardUpdateInfo)
        {
            var cardSpinDropView = await AddChild<AlbumCardSpinDropView>();
            cardSpinDropView.ShowView(cardUpdateInfo);
        }
    }

    public class TopPanelExtraAttachViewContainerViewController : ViewController<TopPanelExtraAttachViewContainerView>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            SubscribeEvent<EventLevelAchieved>(OnLevelAchieved);
            SubscribeEvent<EventLevelUpUnlockNewMachine>(OnUnlockNewMachine);
            SubscribeEvent<EventPNotification>(OnPNotification);
            SubscribeEvent<EventShowSpinDropCardInfo>(OnEventShowCardDrop);
        }

        private void OnPNotification(EventPNotification eventData)
        {
            if (eventData.notificationData == null) { return; }

            var notificationData = eventData.notificationData;
            var type = notificationData.NotificationType;
            switch (type)
            {
                case NotificationType.Jackpot:
                    if (notificationData.Pb == null || notificationData.Pb.Data == null) { return; }
                    var result = SJackpotNotification.Parser.ParseFrom(notificationData.Pb.Data.ToByteArray());
                    if (result == null) { return; }
                    var userLevel = Client.Get<UserController>().GetUserLevel();
                    var pay = result.Pay;
                    XDebug.Log($"1111111111111 Receive PNotification:type={type}, pay={pay}");

                    if (
                        userLevel < 50
                        || (userLevel >= 50 && userLevel < 100 && pay >= Math.Pow(10, 10))
                        || (userLevel >= 100 && pay >= Math.Pow(10, 11))
                    )
                    {
                        if (Client.Get<MachineLogicController>().IsValidGameId(result.GameId))
                        {
                            view.ShowJackpotNotificationView(result);
                        }
                    }
                    //view.ShowJackpotNotificationView(result);
                    break;
            }
        }

        public void OnEventShowCardDrop(EventShowSpinDropCardInfo eventShowSpinDropCardInfo)
        {
            view.ShowCardDropView(eventShowSpinDropCardInfo.cardUpdateInfo);
        }

        protected void OnLevelAchieved(EventLevelAchieved levelAchieved)
        {
            view.ShowLevelUpView(levelAchieved.levelUpInfo);
        }

        protected void OnUnlockNewMachine(EventLevelUpUnlockNewMachine evt)
        {
            if (evt.levelUpInfo != null && evt.levelUpInfo.UnlockedMachines.Count > 0)
            {
                if (Client.Get<MachineLogicController>().IsMachineExist(evt.levelUpInfo.UnlockedMachines[0])
                && !Client.Get<MachineLogicController>().HasHotFlag(evt.levelUpInfo.UnlockedMachines[0]))
                    view.ShowMachineUnlockView(evt.levelUpInfo);
            }
        }
    }
}