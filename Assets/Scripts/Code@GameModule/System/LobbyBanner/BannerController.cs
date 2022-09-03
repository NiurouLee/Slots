// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/02/19:14
// Ver : 1.0.0
// Description : BannerController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class BannerController : LogicController
    {
        private BannerModel _bannerModel;

        private List<uint> _lastValidAdIds;

        private bool _levelChanged = false;
        private bool _levelUpToLevel3 = false;

        private Dictionary<string, DealOfferHandler> _dealOfferHandlers;
        
        public BannerController(Client client)
            : base(client)
        {

        }

        protected override void Initialization()
        {
            base.Initialization();
            _bannerModel = new BannerModel();
        }

        public override void Update()
        {
            if (!ViewManager.Instance.InLobbyScene())
            {
                return;
            }

            if (_lastValidAdIds == null)
            {
                _lastValidAdIds = _bannerModel.GetValidAdIds("LobbyBanner");
                return;
            }

            var currentAdIds = _bannerModel.GetValidAdIds("LobbyBanner");

            if (currentAdIds.Count != _lastValidAdIds.Count || !currentAdIds.SequenceEqual(_lastValidAdIds))
            {
                EventBus.Dispatch(new EventUpdateLobbyBannerIconContent());
            }

            _lastValidAdIds = currentAdIds;
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            _bannerModel.UpdateModelData(sGetInfoBeforeEnterLobby.SGetAdvertisement.AdvsInfo);
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (_bannerModel.LastTimeUpdateData == 0)
            {
                await UpdateBannerData();
            }
            
            UpdateDealHandleInfo();
            
            finishCallback?.Invoke();

            EnableUpdate(1);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventDealOfferConsumeComplete>(OnDealOfferConsumeEnd);
            SubscribeEvent<EventCommonPaymentComplete>(OnPaymentConsumeEnd);
            
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.LobbyBanner);
            SubscribeEvent<EventStoreClose>(OnStoreClosed);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnEventPreNoticeLevelChanged);
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd,HandlerPriorityWhenSpinEnd.Banner);
        }

        protected void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSpinRoundEnd,
            IEventHandlerScheduler scheduler)
        {
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
           
            //PiggyBonus触发逻辑
            //1.玩家升级到Level3的时候弹出
            //2.玩家SPIN WinLevel大于HugeWin
            if (machineScene != null &&
                machineScene.viewController.GetMachineContext().assetProvider.MachineId == "11003")
            {
                // if (_levelUpToLevel3)
                // {
                //     _levelUpToLevel3 = false;
                //     if (_dealOfferHandlers.ContainsKey("PiggyBonus"))
                //     {
                //         TriggerDeal(_dealOfferHandlers["PiggyBonus"].adv, handleEndCallback, "Level3");
                //         return;
                //     }
                // }

                if (eventSpinRoundEnd.winLevel >= (int) WinLevel.ColossalWin)
                {
                    if (_dealOfferHandlers.ContainsKey("PiggyBonus"))
                    {
                        TriggerDeal(_dealOfferHandlers["PiggyBonus"].adv, handleEndCallback, "HugeWin");
                        return;
                    }
                }
            }
            
            handleEndCallback.Invoke();
        }

        private List<string> _validTriggerSource = new List<string>()
        {
            "Buy",
            "CoinIcon",
            "StoreBonus",
            "ProfileUIAddCoin"
        };

        public void OnEventPreNoticeLevelChanged(EventPreNoticeLevelChanged evt)
        {
            if (evt.levelUpInfo != null)
            {
                _levelChanged = true;
                _levelUpToLevel3 = evt.levelUpInfo.Level == 3;
            }
        }

        protected async void OnStoreClosed(EventStoreClose eventStoreClose)
        {
            
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            //如果在11003中，优先弹出 PiggyBonus
            if (machineScene != null &&
                machineScene.viewController.GetMachineContext().assetProvider.MachineId == "11003")
            {
               
                if (_dealOfferHandlers.ContainsKey("PiggyBonus"))
                {
                    TriggerDeal(_dealOfferHandlers["PiggyBonus"].adv);
                    return;
                }
            }
            
            if (_dealOfferHandlers.ContainsKey("FirstTimeSpecialOffer"))
            {
                await ActiveFirstDealOffer();

                if (_validTriggerSource.Contains(eventStoreClose.storeOpenSource) && !eventStoreClose.purchasedInStore)
                {
                    EventBus.Dispatch(new EventTriggerPopupPool("CloseStorePopup", null));
                }
            }
        }

        protected async Task ActiveFirstDealOffer()
        {
            if (_dealOfferHandlers.ContainsKey("FirstTimeSpecialOffer"))
            {
                await _dealOfferHandlers["FirstTimeSpecialOffer"].ActiveOffer(_bannerModel.TimeElapseSinceLastUpdate());
            }
        }
        
        protected async Task ActivePiggyBonusOffer()
        {
            if (_dealOfferHandlers.ContainsKey("PiggyBonus"))
            {
                await _dealOfferHandlers["PiggyBonus"].ActiveOffer(_bannerModel.TimeElapseSinceLastUpdate());
            }
        }

        public async void OnEnterMachineScene(EventEnterMachineScene enterMachineScene)
        {
            if (enterMachineScene.context.assetProvider.MachineId == "11003")
            {
                await ActivePiggyBonusOffer();
            }
        }

        protected async void OnSceneSwitchEnd(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (_levelChanged)
            {
                _levelChanged = false;
                await UpdateBannerData();
            }
            
            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY
                && eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING)
            {
                UpdateDealHandleInfo();
                
                await ActiveFirstDealOffer();
            }
            
            handleEndCallback?.Invoke();
        }


        protected async void OnDealOfferConsumeEnd(EventDealOfferConsumeComplete evt)
        {
            await UpdateBannerData();
        }
        
        protected async void OnPaymentConsumeEnd(EventCommonPaymentComplete evt)
        {
            if (evt.shopType == ShopType.SlotDeal)
            {
                await UpdateBannerData();
            }
        }

        protected async Task UpdateBannerData()
        {
            await _bannerModel.FetchModalDataFromServerAsync();

            UpdateDealHandleInfo();
        }

        protected void UpdateDealHandleInfo()
        {
            _dealOfferHandlers = new Dictionary<string, DealOfferHandler>();

            var advertisements = _bannerModel.GetValidDealAdvertisement();

            for (var i = 0; i < advertisements.Count; i++)
            {
                _dealOfferHandlers[advertisements[i].Jump] = new DealOfferHandler(advertisements[i]);
            }
        }

        public ulong GetActiveDealAdvertisementCountDown()
        {
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            //如果在11003中，优先弹出 PiggyBonus
            if (machineScene != null &&
                machineScene.viewController.GetMachineContext().assetProvider.MachineId == "11003")
            {
                if (_dealOfferHandlers.ContainsKey("PiggyBonus"))
                {
                    return _dealOfferHandlers["PiggyBonus"].GetCountDown(_bannerModel.TimeElapseSinceLastUpdate());
                }
            }
            
            if (_dealOfferHandlers.ContainsKey("FirstTimeSpecialOffer"))
            {
                return _dealOfferHandlers["FirstTimeSpecialOffer"].GetCountDown(_bannerModel.TimeElapseSinceLastUpdate());
            }
            return 0;
        }

        public void TriggerActiveDealOffer(string source, Action closeAction = null)
        {
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            //如果在11003中，优先弹出 PiggyBonus
            if ((source == "DealButton" || source == "LevelUp") && machineScene != null &&
                machineScene.viewController.GetMachineContext().assetProvider.MachineId == "11003")
            {
                if (_dealOfferHandlers.ContainsKey("PiggyBonus"))
                {
                    TriggerDeal(_dealOfferHandlers["PiggyBonus"].adv, closeAction, source);
                    return;
                }
            }

            if (_dealOfferHandlers.ContainsKey("FirstTimeSpecialOffer"))
            {
                TriggerDeal(_dealOfferHandlers["FirstTimeSpecialOffer"].adv, closeAction, source);
                return;
            }
            
            closeAction.Invoke();
        }
 
        public void TriggerDeal(Advertisement advertisement, Action closeAction = null, string source = "")
        {
            if (advertisement.Jump.Contains("FirstTimeSpecialOffer"))
            {
                EventBus.Dispatch(
                    new EventShowPopup(new PopupArgs(typeof(FirstTimeSpecialOfferPopup), closeAction, source)));
            }
            else if (advertisement.Jump.Contains("PiggyBonus"))
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(PiggyBonusPopup), closeAction, source)));
            }
        }
        
        public bool DealButtonEnabled()
        {
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
            //如果在11003中，优先弹出 PiggyBonus
            if (machineScene != null &&
                machineScene.viewController.GetMachineContext().assetProvider.MachineId == "11003")
            {
                if (_dealOfferHandlers.ContainsKey("PiggyBonus"))
                {
                    return _dealOfferHandlers["PiggyBonus"].IsActive(_bannerModel.TimeElapseSinceLastUpdate());
                }
            }

            if (_dealOfferHandlers.ContainsKey("FirstTimeSpecialOffer"))
            {
                return _dealOfferHandlers["FirstTimeSpecialOffer"].IsActive(_bannerModel.TimeElapseSinceLastUpdate());
            }
            return false;
        }

        public Advertisement GetFirstTimeSpecialOfferAdv()
        {
            if (_dealOfferHandlers.ContainsKey("FirstTimeSpecialOffer"))
            {
                return _dealOfferHandlers["FirstTimeSpecialOffer"].adv;
            }
            
            return null;
        }
        
        public Advertisement GetDealOfferAdv(string name)
        {
            if (_dealOfferHandlers.ContainsKey(name))
            {
                return _dealOfferHandlers[name].adv;
            }
            
            return null;
        }
 
        public float GetDealCountDown(Advertisement adv)
        {
            if (adv != null && adv.DealInfo != null)
            {
                return (float)adv.DealInfo.HiddenCountDown - _bannerModel.TimeElapseSinceLastUpdate();
            }
            return 0;
        }
 
        public async Task<SGetAdvertisementItem> GetDealOfferInfo(string dealName)
        {
            if (_dealOfferHandlers.ContainsKey(dealName))
            {
                var dealInfo = await _dealOfferHandlers[dealName].GetDealInfo();
                _bannerModel.UpdateAdvertisement(dealInfo.Adv, dealInfo.Now);
                return dealInfo;
            }
            
            return null;
        }

        public List<Advertisement> GetLobbyFixedAdvertisement()
        {
            return _bannerModel.GetLobbyFixedAdvertisement();
        }

        public Dictionary<uint, List<Advertisement>> GetLobbyScrollableAdvertisement()
        {
            return _bannerModel.GetValidScrollableAdvertisement();
        }

        public void ResponseAdvertisement(Advertisement advertisement)
        {
            if (advertisement.Type == AdvertisementType.Deal && advertisement.Jump == "FirstTimeSpecialOffer")
            {
                TriggerDeal(advertisement, null, "Advertisement");
            }
            else if (advertisement.Type == AdvertisementType.Quicksystem)
            {
                Client.Get<JumpController>().OnExecuteJump(new EventExecuteJump(GetJumpInfo(advertisement)));
            }
        }

        public void UpdateBannerContent(Advertisement advertisement, View parentView, Transform bannerContent)
        {
            if (advertisement.Type == AdvertisementType.Quicksystem)
            {
                var jumpHandler = Client.Get<JumpController>().GetJumpHandler(GetJumpInfo(advertisement));

                if (jumpHandler != null)
                {
                    jumpHandler.UpdateBannerContent(advertisement, parentView, bannerContent);
                }
            }
        }

        public bool IsValidAdvertisement(Advertisement advertisement)
        {
            return _bannerModel.IsValidAdvertisement(advertisement);
        }

        public JumpInfo GetJumpInfo(Advertisement advertisement)
        {
            if (advertisement.Jump == "Quest")
            {
                return new JumpInfo(JumpType.QUEST);
            }
            if (advertisement.Jump == "DailyMission")
            {
                return new JumpInfo(JumpType.DAILYMISSION);
            }
            if (advertisement.Jump == "SeasonPass")
            {
                return new JumpInfo(JumpType.SEASON_PASS);
            }

            if (advertisement.Jump == "Store")
            {
                return new JumpInfo(JumpType.IAP_STORE);
            }

            if (advertisement.Jump == "GoldCoupon")
            {
                return new JumpInfo(JumpType.GOLDEN_COUPON);
            }

            if (advertisement.Jump == "SeasonQuest")
            {
                return new JumpInfo(JumpType.SEASON_QUEST);
            }

            if (advertisement.Jump == "CrazeSmash")
            {
                return new JumpInfo(JumpType.CRAZE_SMASH);
            }

            if (advertisement.Jump == "ADTask")
            {
                return new JumpInfo(JumpType.AD_TASK);
            }
            
            if (advertisement.Jump == "CoinDash")
            {
                return new JumpInfo(JumpType.COIN_DASH);
            }

            if (advertisement.Jump == "Activity")
            {
                return new JumpInfo(JumpType.ACTIVITY, advertisement.Jump2);
            }

            return null;
        }
    }
}