using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class CrazeSmashController : LogicController
    {
        public bool available
        {
            get { return eggInfo.StartTimestamp * 1000 < APIManager.Instance.GetServerTime() && eggInfo.EndTimestamp * 1000 > APIManager.Instance.GetServerTime(); }
        }

        public bool silverGameFinish
        {
            get
            {
                if (eggInfo == null) { return true; }
                return eggInfo.SilverOver;
            }
        }

        public bool goldGameFinish
        {
            get
            {
                if (eggInfo == null) { return true; }
                return eggInfo.GoldOver;
            }
        }

        public bool locked
        {
            get
            {
                if (eggInfo == null) { return true; }
                return eggInfo.IsLocked;
            }
        }

        public bool levelReached
        {
            get
            {
                if (eggInfo == null) { return false; }
                var controller = Client.Get<UserController>();
                if (controller == null) { return false; }
                return controller.GetUserLevel() >= eggInfo.UnlockLevel;
            }
        }

        public ulong unlockLevel
        {
            get
            {
                if (eggInfo == null) { return ulong.MaxValue; }
                return eggInfo.UnlockLevel;
            }
        }

        public bool playGoldGame { get; set; }
 
        public EggInfo eggInfo { get; private set; }

        public UserProfile userProfile { get; private set; }

        public EggShopItemConfig eggShopItemConfig { get; private set; }

        public EggShopItemConfig[] silverEggShopItems { get; private set; }
        public EggShopItemConfig[] goldEggShopItems { get; private set; }
 
        private bool _noticeShown = false;

        public bool canRefreshEggInfo = true;

        public CrazeSmashController(Client client) : base(client)
        {
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await SendCGetEggInfo();
            }
            
            EnableUpdate(1);
            finishCallback?.Invoke();
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetEggInfo != null)
            {
                eggInfo = sGetInfoBeforeEnterLobby.SGetEggInfo.Egg;
                beforeEnterLobbyServerDataReceived = true;
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventCollectStoreBonusFinish>(OnEventCollectStoreBonusFinish);
            SubscribeEvent<Event_CrazeSmash_PurchaseFinish>(OnEvent_CrazeSmash_PurchaseFinish);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnEventPreNoticeLevelChanged);
        }

        public async void OnEventPreNoticeLevelChanged(EventPreNoticeLevelChanged evt)
        {
            if (evt.levelUpInfo.Level == eggInfo.UnlockLevel)
            {
               await SendCGetEggInfo();
            }
        }

        private async void OnEvent_CrazeSmash_PurchaseFinish(Event_CrazeSmash_PurchaseFinish obj)
        {
            await SendCGetEggInfo();
        }

        private async void OnEventCollectStoreBonusFinish(EventCollectStoreBonusFinish obj)
        {
            if (obj.sGetStoreBonus == null)
            {
                return;
            }

            var extra = obj.sGetStoreBonus.ExtraRewards;
            if (extra == null || extra.count == 0)
            {
                return;
            }

            Item silverHammerItem = null;
            foreach (var reward in extra)
            {
                foreach (var item in reward.Items)
                {
                    if (item.Type == Item.Types.Type.SilverHammer)
                    {
                        silverHammerItem = item;
                        break;
                    }
                }
            }

            if (silverHammerItem == null)
            {
                return;
            }

            var popup = await PopupStack.ShowPopup<UICrazeSmashGetHammerPopup_Silver>();
            popup.Set(silverHammerItem.SilverHammer.Amount);
            await SendCGetEggInfo();
            await WaitForSeconds(3);
            PopupStack.ClosePopup<UICrazeSmashGetHammerPopup_Silver>();

            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashCollect, ("type", $"{1}"));
        }

        public override void Update()
        {
            // base.Update();
            UpdateTime();
        }

        public static RepeatedField<Item> FilterItems(RepeatedField<Item> items, out ulong coin)
        {
            coin = 0;
            if (items == null || items.Count == 0)
            {
                return null;
            }

            var clone = new RepeatedField<Item>();
            for (int i = items.count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item.Type == Item.Types.Type.Coin)
                {
                    coin += item.Coin.Amount;
                }
                else
                {
                    clone.Add(item);
                }
            }

            return clone;
        }

        private async void UpdateTime()
        {
            if (eggInfo == null || eggInfo.IsLocked) { return; }
            var endTimeStamp = eggInfo.EndTimestamp;
  
            if (XUtility.GetLeftTime(endTimeStamp * 1000) < 0 && canRefreshEggInfo)
            {
                await SendCGetEggInfo();
                EventBus.Dispatch(new Event_CrazeSmash_Expire());
            }
        }

        public bool CanShowAD()
        {
            if (_noticeShown || eggInfo == null)
            {
                return false;
            }

            if (eggInfo.SilverOver && eggInfo.GoldOver)
            {
                return false;
            }

            var showADDays = eggInfo.ShowAdDays;
           
            if (showADDays == null || showADDays.count == 0)
            {
                return false;
            }

            var inboxController = Client.Get<InboxController>();
            if (inboxController != null)
            {
                var dateTimeOffset =
                    DateTimeOffset.FromUnixTimeMilliseconds((long) APIManager.Instance.GetServerTime());
                var weekDay = (int) dateTimeOffset.Date.DayOfWeek;
               
                foreach (var day in showADDays)
                {
                    if (day == weekDay)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // protected async void OnSceneSwitchEnd(
        //     Action handleEndCallback,
        //     EventSceneSwitchEnd eventSceneSwitchEnd,
        //     IEventHandlerScheduler scheduler)
        // {
        //     if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
        //     {
        //         if (CanShowAD())
        //         {
        //             await PopupStack.ShowPopup<UICrazeSmashNoticePopup>();
        //             _noticeShown = true;
        //         }
        //     }
        //
        //     handleEndCallback?.Invoke();
        // }

        public void SetNoticeShown()
        {
            _noticeShown = true;
        }

        public async Task OpenBuyPopup()
        {
            await SendCGetEggPaymentItems();
          
            if ((playGoldGame == false && silverEggShopItems.Length == 0)
                || (playGoldGame && goldEggShopItems.Length == 0))
            {
                return;
            }

            var popup = PopupStack.GetPopup<UICrazeSmashBuy>();
            if (popup == null)
            {
                await PopupStack.ShowPopup<UICrazeSmashBuy>();
            }
        }

        public async Task SendCGetEggInfo()
        {
            var c = new CGetEggInfo();
            var s = await APIManagerGameModule.Instance.SendAsync<CGetEggInfo, SGetEggInfo>(c);

            if (s.ErrorCode == 0 && s.Response != null)
            {
                eggInfo = s.Response.Egg;
                EventBus.Dispatch(new Event_CrazeSmash_EggInfoChanged());
                XDebug.Log("111111111 SGetEggInfo success");
            }
            else
            {
                XDebug.LogError("111111111 SGetEggInfo error:" + s.ErrorInfo);
            }

            UpdateTime();
        }

        public async Task<SSmashEgg> SendCSmashEgg(bool isGold, uint index)
        {
            if (index >= 7)
            {
                return null;
            }
            
            if ((isGold && eggInfo.GoldHammer <= 0) || (isGold == false && eggInfo.SilverHammer <= 0))
            {
                await OpenBuyPopup();
                return null;
            }

            var c = new CSmashEgg() { IsGold = isGold, Index = index };
            var s = await APIManagerGameModule.Instance.SendAsync<CSmashEgg, SSmashEgg>(c);
            if (s.ErrorCode == 0 && s.Response != null)
            {
                eggInfo = s.Response.Egg;
                userProfile = s.Response.UserProfile;
                EventBus.Dispatch(new EventUserProfileUpdate(userProfile));

                EventBus.Dispatch(new Event_CrazeSmash_EggInfoChanged());
                XDebug.Log("111111111 SSmashEgg success");
                
                return s.Response;
            }
            else
            {
                XDebug.LogError("111111111 SSmashEgg error:" + s.ErrorInfo);
                EventBus.Dispatch(new EventOnUnExceptedServerError(s.ErrorInfo));
            }

            return null;
        }

        public async Task SendCGetEggPaymentItems()
        {
            silverEggShopItems = null;
            goldEggShopItems = null;

            var c = new CGetEggPaymentItems();
            
            var s = await APIManagerGameModule.Instance.SendAsync<CGetEggPaymentItems, SGetEggPaymentItems>(c);
           
            if (s.ErrorCode == 0 && s.Response != null)
            {
                var items = s.Response.Items;
                var silverList = new List<EggShopItemConfig>();
                var goldList = new List<EggShopItemConfig>();
                for (int i = 0; i < items.count; i++)
                {
                    var item = items[i];
                    if (item.IsGold)
                    {
                        goldList.Add(item);
                    }
                    else
                    {
                        silverList.Add(item);
                    }
                }

                goldEggShopItems = goldList.ToArray();
                silverEggShopItems = silverList.ToArray();

                XDebug.Log("111111111 SGetEggPaymentItems success");
            }
            else
            {
                XDebug.LogError("111111111 SGetEggPaymentItems error:" + s.ErrorInfo);
            }
        }
    }
}