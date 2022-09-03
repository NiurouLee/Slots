/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-10-23 18:03:03
Ver : 1.0.0
Description : 大厅类广告牌的数据，资源控制
ChangeLog :  
**********************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule.UI;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace GameModule
{
    public enum JumpType
    {
        UNDEFINED = 0,
        SHOP_FIRST = 1,
        FIST_SPECIAL_OFFER = 2,
        SPECIAL_OFFER = 3,
        PIGGY = 4,
        FACEBOOK = 5,
        TOMACHINE = 6,
        QUEST = 7,

        LINK = 8,
        DAILYMISSION = 9,
        CONTACT_US = 10,
        CASH_BACK = 11,
        IAP_STORE = 12,
        SEASON_PASS = 13,
        GOLDEN_COUPON = 14,
        SEASON_QUEST = 15,
        ACTIVITY = 16,
        AD_TASK = 17,
        CRAZE_SMASH = 18,
        COIN_DASH = 19,
    }

    public class JumpInfo
    {
        public JumpType jumpType;
        public string jumpParam1;
        public string source;

        public JumpInfo(JumpType inJumpType, string inJumpParam1 = "", string inSource = "")
        {
            jumpType = inJumpType;
            jumpParam1 = inJumpParam1;
            source = inSource;
        }
    }

    public class JumpHandler
    {
        protected JumpType type;

        public JumpHandler(JumpType type)
        {
            this.type = type;
        }

        public virtual bool CheckValid(JumpInfo jumpInfo)
        {
            return true;
        }

        public virtual bool Execute(JumpInfo jumpInfo)
        {
            return true;
        }

        public virtual void UpdateBannerContent(Advertisement adv, View parentView, Transform bannerContent)
        {

        }
    }

    public class FacebookJumpHandler : JumpHandler
    {
        public FacebookJumpHandler()
            : base(JumpType.FACEBOOK)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            // if (!Client.Player.HaveBindFackbook)
            // {
            //     return true;
            // }
            return false;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {

            EventBus.Dispatch<EventBindingFaceBook>();

            return true;
        }
    }

    public class SlotJumpHandler : JumpHandler
    {
        public SlotJumpHandler()
            : base(JumpType.TOMACHINE)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            //  SubjectInfo info = Client.Slot.GetSubjectInfo(Convert.ToInt32(jumpInfo.jumpParam1));
            // if(info != null && info.unlockLevel <= Client.Level.PlayerLevel)
            // {
            //     return true;
            // }
            return false;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            // if (DialogManager.HasDialog(typeof(QuestMainUIDialog)))
            // {
            //     MiscUtil.ShowLoadingView();
            //     Client.Assets.LoadAsset<GameObject>("MachineLoadingView_" + jumpInfo.jumpParam1, task =>
            //     {
            //         Client.Slot.EnterRoom(Convert.ToInt32(jumpInfo.jumpParam1));
            //
            //         MiscUtil.DelayFrameCall(() =>
            //         {
            //             MiscUtil.HideLoadingView();
            //             DialogManager.CloseDialog<QuestMainUIDialog>();
            //         });
            //     });
            // }
            //else
            {
                // MiscUtil.ShowLoadingView();
                // Client.Assets.LoadAsset<GameObject>("MachineLoadingView_" + jumpInfo.jumpParam1, task =>
                // {
                //
                //     Client.Slot.EnterRoom(Convert.ToInt32(jumpInfo.jumpParam1));
                //     MiscUtil.HideLoadingView();
                // });
            }
            return true;
        }
    }

    public class LinkJumpHandler : JumpHandler
    {
        public LinkJumpHandler()
            : base(JumpType.LINK)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            return true;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            Application.OpenURL(jumpInfo.jumpParam1);
            return true;
        }
    }

    public class QuestJumpHandler : JumpHandler
    {
        public QuestJumpHandler()
            : base(JumpType.QUEST)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            var newBieQuest = Client.Get<NewBieQuestController>();
            if (newBieQuest != null)
            {
                return !newBieQuest.IsLocked() && !newBieQuest.IsQuestFinished() && !newBieQuest.IsTimeOut();
            }

            return false;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(QuestPopup), "Advertisement")));
            return true;
        }
    }

    public class SeasonQuestJumpHandler : JumpHandler
    {
        public SeasonQuestJumpHandler()
            : base(JumpType.SEASON_QUEST)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            var seasonQuest = Client.Get<SeasonQuestController>();
            if (seasonQuest != null)
            {
                return !seasonQuest.IsLocked() && !seasonQuest.IsTimeOut();
            }

            return false;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SeasonQuestPopup), "Advertisement")));
            return true;
        }
    }

    public class ContactUsJumpHandler : JumpHandler
    {
        public ContactUsJumpHandler()
            : base(JumpType.CONTACT_US)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            return true;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {

            return true;
        }
    }

    public class DailyMissionJumpHandler : JumpHandler
    {
        public DailyMissionJumpHandler()
            : base(JumpType.DAILYMISSION)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            var dailyMission = Client.Get<DailyMissionController>();
            if (dailyMission != null)
            {
                return !dailyMission.IsLocked;
            }

            return false;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup), (object)new[] { "DailyMission", "Advertisement" })));
            return true;
        }
    }

    public class SeasonPassJumpHandler : JumpHandler
    {
        public SeasonPassJumpHandler()
            : base(JumpType.SEASON_PASS)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            var seasonPass = Client.Get<SeasonPassController>();
            if (seasonPass != null)
            {
                return !seasonPass.IsLocked;
            }

            return false;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup), (object)new[] { "SeasonPass", "Advertisement" })));
            return true;
        }
    }

    public class CrazeSmashJumpHandler : JumpHandler
    {
        public CrazeSmashJumpHandler() : base(JumpType.CRAZE_SMASH) { }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            var controller = Client.Get<CrazeSmashController>();
            return controller.available
                && (!controller.goldGameFinish || !controller.silverGameFinish);
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup), (object)new[] { "CrazeSmash", "Advertisement" })));
            return true;
        }
    }

    public class StoreJumpHandler : JumpHandler
    {
        public StoreJumpHandler()
            : base(JumpType.IAP_STORE)
        {

        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            return true;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            var source = string.IsNullOrEmpty(jumpInfo.source) ? "AdvJump" : jumpInfo.source;
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), source)));

            return true;
        }
    }

    public class AdTaskJumpHandler : JumpHandler
    {
        public AdTaskJumpHandler()
            : base(JumpType.AD_TASK)
        {

        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            return AdController.Instance.GetAdTaskInfo() != null;
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            // var source = string.IsNullOrEmpty(jumpInfo.source) ? "AdvJump" : jumpInfo.source;
            PopupStack.ShowPopupNoWait<AdRushPopup>();

            return true;
        }
    }

    public class ActivityJumpHandler : JumpHandler
    {
        public ActivityJumpHandler() : base(JumpType.ACTIVITY) { }


        private ActivityBase GetActivity(string activityType)
        {
            var activityController = Client.Get<ActivityController>();
            if (activityController == null) { return null; }
            ActivityBase activity = activityController.GetDefaultActivity(activityType);;
            return activity;
        }
        public override bool CheckValid(JumpInfo jumpInfo)
        {
            if (jumpInfo == null) { return false; }
            var activity = GetActivity(jumpInfo.jumpParam1);
            if (activity == null) { return false; }
            return activity.OnCheckBannerValid(jumpInfo);
        }

        public override void UpdateBannerContent(Advertisement advertisement, View parentView, Transform bannerContent)
        {
            if (advertisement == null) { return; }
            var activity = GetActivity(advertisement.Jump2);
            activity?.OnUpdateBannerContent(advertisement, parentView, bannerContent);
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            if (jumpInfo == null) { return true; }
            var activity = GetActivity(jumpInfo.jumpParam1);
            activity?.OnBannerJump(jumpInfo);
            return true;
        }
    }

    public class GoldenCouponJumpHandler : JumpHandler
    {
        public GoldenCouponJumpHandler()
            : base(JumpType.GOLDEN_COUPON)
        {

        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            return true;
        }

        public override void UpdateBannerContent(Advertisement advertisement, View parentView, Transform bannerContent)
        {
            //TODO 
            if (bannerContent == null) { return; }
            var transformText = bannerContent.Find("Root/PercentageText");
            if (transformText == null) { return; }
            var text = transformText.GetComponent<Text>();
            if (text == null) { return; }
            var goldenCouponActivity = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.GoldenCoupon) as Activity_BonusCoupon;
            var coupon = goldenCouponActivity?.GetLinkedCoupon();
            if (coupon == null) { return; }
            text.SetText(coupon.BonusPersentage + "%");
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            //TODO
            InternalExecute(jumpInfo);
            return true;
        }

        private async Task InternalExecute(JumpInfo jumpInfo)
        {
            var activityController = Client.Get<ActivityController>();
            var activityGolden = activityController.GetDefaultActivity(ActivityType.GoldenCoupon) as Activity_BonusCoupon;
            if (activityGolden == null)
                return;

            var coupon = activityGolden.GetLinkedCoupon();
          
            if (coupon != null)
            {
                await Client.Get<InboxController>().SendCBindCouponToStore(coupon.Id);
                var source = string.IsNullOrEmpty(jumpInfo.source) ? "AdvJump" : jumpInfo.source;
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), source)));
            }
        }
    }
    
    public class CoinDashJumpHandler : JumpHandler
    {
        public CoinDashJumpHandler()
            : base(JumpType.DAILYMISSION)
        {
        }

        public override bool CheckValid(JumpInfo jumpInfo)
        {
            var controller = Client.Get<CoinDashController>();
            return controller.IsOpen();
        }

        public override bool Execute(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(CoinDashPopup))));
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCoindashEnter);
            return true;
        }
    }


    public class JumpController : LogicController
    {
        public Dictionary<JumpType, JumpHandler> jumpHandlers;

        public JumpController(Client client)
            : base(client)
        {
        }

        protected override void Initialization()
        {
            RegisterJumpHandler();
        }

        public void RegisterJumpHandler()
        {
            jumpHandlers = new Dictionary<JumpType, JumpHandler>();

            jumpHandlers.Add((JumpType.TOMACHINE), new SlotJumpHandler());
            jumpHandlers.Add((JumpType.LINK), new LinkJumpHandler());
            jumpHandlers.Add((JumpType.CONTACT_US), new ContactUsJumpHandler());
            jumpHandlers.Add((JumpType.QUEST), new QuestJumpHandler());
            jumpHandlers.Add((JumpType.DAILYMISSION), new DailyMissionJumpHandler());
            jumpHandlers.Add((JumpType.SEASON_PASS), new SeasonPassJumpHandler());
            jumpHandlers.Add((JumpType.IAP_STORE), new StoreJumpHandler());
            jumpHandlers.Add((JumpType.GOLDEN_COUPON), new GoldenCouponJumpHandler());
            jumpHandlers.Add((JumpType.SEASON_QUEST), new SeasonQuestJumpHandler());
            jumpHandlers.Add((JumpType.ACTIVITY), new ActivityJumpHandler());
            jumpHandlers.Add((JumpType.AD_TASK), new AdTaskJumpHandler());
            jumpHandlers.Add((JumpType.CRAZE_SMASH), new CrazeSmashJumpHandler());
            jumpHandlers.Add((JumpType.COIN_DASH), new CoinDashJumpHandler());
        }


        /// <summary>
        /// 提供一个空的虚接口统一订阅游戏内的事件
        /// </summary>
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventExecuteJump>(OnExecuteJump);
        }

        public JumpHandler GetJumpHandler(JumpInfo jumpInfo)
        {
            //这里加一个判空，防止策划配错表
            if (jumpInfo != null && jumpInfo.jumpType != (int)(JumpType.UNDEFINED) && jumpHandlers.ContainsKey(jumpInfo.jumpType))
            {
                return jumpHandlers[jumpInfo.jumpType];
            }

            return null;
        }

        public bool CheckJumpInvalid(JumpInfo jumpInfo)
        {
            if (jumpInfo.jumpType != (int)(JumpType.UNDEFINED) && jumpHandlers.ContainsKey(jumpInfo.jumpType))
            {
                return jumpHandlers[jumpInfo.jumpType].CheckValid(jumpInfo);
            }

            return false;
        }
        public void OnExecuteJump(EventExecuteJump eventExecuteJump)
        {
            var jumpInfo = eventExecuteJump.jumpInfo;
            if (jumpInfo != null && CheckJumpInvalid(jumpInfo))
            {
                jumpHandlers[jumpInfo.jumpType].Execute(jumpInfo);
            }
        }
    }
}