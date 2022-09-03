// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/25/15:27
// Ver : 1.0.0
// Description : SuperSpinXGetCard.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class Activity_SuperSpinGiveCard:ActivityBase
    {
        private SuperSpinGiveCardActivityConfigPB _configPb;
        private SuperSpinGiveCardActivityDataPB _dataPb;
        public Activity_SuperSpinGiveCard() 
            : base(ActivityType.SuperSpinGiveCard)
        {
            
        }

        public override void OnEnterLobby()
        {
            if (!activityPosterPopped && IsValid())
            {
                EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(SuperSpinXCardPosterPopup))));
            }

            base.OnEnterLobby();
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventPaymentFinish>(OnEventPaymentFinish);
        }

        protected async void OnEventPaymentFinish(EventPaymentFinish evt)
        {
            if (evt.verifyExtraInfo != null && evt.verifyExtraInfo.Item.ShopType == ShopType.SuperSpinX)
            {
                await RequestCGetActivityUserDataAsync();
            }
        }

        /// <summary>
        /// 获取是否过期
        /// </summary>
        protected override bool IsExpired()
        {
            if (_configPb != null)
            {
                if (XUtility.GetLeftTime((ulong) _configPb.StartTimestamp*1000) > 0)
                {
                    return true;
                }

                if (XUtility.GetLeftTime((ulong) _configPb.EndTimestamp*1000) <= 0)
                {
                    return true;
                }

                if (_dataPb.CollectedAllCard)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取是否解锁状态
        /// </summary>
        public override bool IsUnlockState()
        {
            return Client.Get<AlbumController>().IsOpen() && Client.Get<AlbumController>().IsUnlocked();
        }

        public override string GetAssetLabelName()
        {
            return "SuperSpinXActivity";
        }

        public override void OnBannerJump(JumpInfo jumpInfo)
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "SuperSpinXCardActivity")));
        }
        
        public override void OnRefreshUserData(SGetActivityUserData.Types.ActivityData inActivityData)
        {
            if (inActivityData != null)
            {
                _configPb = SuperSpinGiveCardActivityConfigPB.Parser.ParseFrom(inActivityData.ActivityConfig.Data);
                _dataPb = SuperSpinGiveCardActivityDataPB.Parser.ParseFrom(inActivityData.ActivityUserData.Data);
            }
            
            OnUpdateCountDown((ulong) XUtility.GetLeftTime((ulong) _configPb.EndTimestamp * 1000));
            
            base.OnRefreshUserData(inActivityData);
        }

    }
}