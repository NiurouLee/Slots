using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using static DragonU3DSDK.Network.API.ILProtocol.SGetActivityUserData.Types;

namespace GameModule
{
    public class Activity_CashBack : ActivityBase
    {
        public CashBackActivityConfig config { get; private set; }
        public CashBackActivityData userData { get; private set; }
        
        private bool _hasShown = false;

        public bool isStoreCommodityValid
        {
            get
            {
                return isLevelReached && !isStoreCommodityExpired;
            }
        }

        public Activity_CashBack() : base(ActivityType.CashBack)
        {
            
        }
        
        protected override bool IsExpired()
        {
            if (config == null) { return true; }
            return XUtility.GetLeftTime((ulong)(config.EndTimestamp * 1000)) <= 0;
        }

        public override bool IsUnlockState()
        {
            if (config == null || userData == null) { return false; }
            var userController = Client.Get<UserController>();
            var level = userController.GetUserLevel();
            return (long)level >= config.LevelLimited;
        }

        public bool isStoreCommodityExpired
        {
            get
            {
                if (config == null) { return true; }
                return XUtility.GetLeftTime((ulong)(config.GetBuffEndTimestamp * 1000)) <= 0;
            }
        }

        public bool isLevelReached
        {
            get
            {
                if (config == null || userData == null) { return false; }
                var userController = Client.Get<UserController>();
                var level = userController.GetUserLevel();
                return (long)level >= config.LevelLimited;
            }
        }

        public async Task<bool> RequestCGetCashBackBuffRewardMail()
        {
            if (userData != null && userData.CashBackBuffs != null && userData.CashBackBuffs.count > 0)
            {
                var buff = userData.CashBackBuffs[0];
                var c = new CGetCashBackBuffRewardMail() { Key = buff.Key };
                var s = await APIManagerGameModule.Instance.SendAsync<CGetCashBackBuffRewardMail, SGetCashBackBuffRewardMail>(c);
                if (s != null && s.ErrorCode == 0 && s.Response != null && s.Response.Success) { return true; }
            }
            return false;
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventPurchasedInStore>(OnEventPurchasedInStore);
            SubscribeEvent<EventLevelChanged>(OnEventLevelChanged);
            SubscribeEvent<EventOnVipLevelUp>(OnEventOnVipLevelUp);
            SubscribeEvent<EventSpinRoundEnd>(OnEventSpinRoundEnd, HandlerPriorityWhenSpinEnd.CashBack);
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
            SubscribeEvent<EventStorePrizeButtonAttachExtraItem>(OnEventSetActivityIcon,1);
        }

        private async void OnEventSetActivityIcon(Action handleEndCallback, EventStorePrizeButtonAttachExtraItem evt,
            IEventHandlerScheduler scheduler)
        {
            if (userData != null && config != null)
            {
                var view = await View.CreateView<CashBackStoreWidget>();
                if (evt.priceButtonExtraItemView.transform != null)
                {
                    evt.priceButtonExtraItemView.AddExtraItem(view);
                }
            }
            
            handleEndCallback.Invoke();
        }

        protected async void OnEventSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
    IEventHandlerScheduler scheduler)
        {
            await RequestCGetActivityUserDataAsync();
            handleEndCallback?.Invoke();
        }

        private async void OnEventLevelChanged(EventLevelChanged obj)
        {
            await RequestCGetActivityUserDataAsync();
        }

        private async void OnEventOnVipLevelUp(EventOnVipLevelUp obj)
        {
            await RequestCGetActivityUserDataAsync();
        }

        private async void OnEventPurchasedInStore(EventPurchasedInStore obj)
        {
            await RequestCGetActivityUserDataAsync();
        }
 
        // Test for activity end

        // long timestamp = 0;
        // long commodityTimestamp = 0;

        // Test for activity end

        public override void OnRefreshUserData(ActivityData inActivityData)
        {
            config = null;
            if (inActivityData != null)
            {
                config = CashBackActivityConfig.Parser.ParseFrom(inActivityData.ActivityConfig.Data);
                userData = CashBackActivityData.Parser.ParseFrom(inActivityData.ActivityUserData.Data);
                OnUpdateCountDown((ulong) XUtility.GetLeftTime((ulong) config.EndTimestamp * 1000));
            }
            
        
            
            base.OnRefreshUserData(inActivityData);
             
            // Test for activity end

            // if (timestamp == 0)
            // {
            //     var seconds = (DateTime.UtcNow.AddSeconds(40) - new DateTime(1970, 1, 1)).TotalSeconds;
            //     timestamp = (long)seconds;
            // }

            // if (commodityTimestamp == 0)
            // {
            //     var seconds = (DateTime.UtcNow.AddSeconds(30) - new DateTime(1970, 1, 1)).TotalSeconds;
            //     commodityTimestamp = (long)seconds;
            // }

            // if (config != null)
            // {
            //     config.EndTimestamp = timestamp;
            //     config.GetBuffEndTimestamp = commodityTimestamp;
            // }

            // Test for activity end
            
            ///EventBus.Dispatch<EventCashBackUserDateChanged>();
        }

        public override void OnEnterLobby()
        {
            base.OnEnterLobby();
            if (IsValid() == false)
            { return; }
            if (isStoreCommodityValid == false) { return; }
            if (_hasShown) { return; }

            EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(UIActivity_CashBack_MainPopup)) { extraArgs = this }));
            _hasShown = true;
        }

        protected override async void OnExpire()
        {
            await RequestCGetActivityUserDataAsync();
            base.OnExpire();
            XDebug.Log($"11111111111111 activity on expire:{ServerActivityType}");
        }
        
        protected async void OnEnterMachineScene(EventEnterMachineScene evt)
        {
            // var cashBackView = await View.CreateView<UIActivity_CashBack_SlotWidget>("UICashBackTimeInSlot", evt.context.MachineUICanvasTransform, this);
            // cashBackView.transform.SetAsFirstSibling();
            // evt.context.SubscribeDestroyEvent(cashBackView);
        }
    }
}