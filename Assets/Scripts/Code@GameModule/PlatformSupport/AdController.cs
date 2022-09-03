using System.Collections.Generic;
using UnityEngine;
using DragonU3DSDK.Storage;
using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Tool;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class AdController : LogicController
    {
        private bool isInit = false;

        private const long INTERSTITIAL_GLOBAL_CD = 360 * 1000;

        private long backgroundStartTime = 0;

        private Action<bool, string> rewardedVideoCB = null;

        private static AdController _instance;
        public static AdController Instance => _instance;
        
        public AdModel adModel;
        

        public bool enableAdConfigRefresh = true;
        protected bool isRefreshAdConfigRefreshing = false;
         
        private AdLogicConfigModel _adLogicConfigModel;
        
        public AdController(Client client)
            : base(client)
        {
            _instance = this;
        }

        protected override void Initialization()
        {
            base.Initialization();
            _adLogicConfigModel = new AdLogicConfigModel();
        }

        public static string GetUserGroupId()
        {
            var groupId = AdConfigManager.Instance.MetaData.GroupId;
            if (groupId == 0)
            {
                XDebug.LogWarning("UserGroupFetchFailed, UserDefaultGroup:99");
                return "99";
            }

            return groupId.ToString();
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            _adLogicConfigModel.UpdateModelData(sGetInfoBeforeEnterLobby.SGetRvAdvertisingConfig);
        }

        public override async  Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (_adLogicConfigModel.LastTimeUpdateData == 0)
            {
                await _adLogicConfigModel.FetchModalDataFromServerAsync();
            }
            
            finishCallback?.Invoke();
        }

        public void InitAdSDKConfig()
        {
            AdConfigManager.Instance.Init();
            DragonPlus.ConfigHub.ConfigHubManager.Instance.Init();
          
            Init();
        }
        
        public SGetRVAdvertisingConfig.Types.AdTaskInfo GetAdTaskInfo()
        {
            return _adLogicConfigModel.GetAdTaskInfo();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.Ad);
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.Ad);
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnEventPreNoticeLevelChanged);
            SubscribeEvent<EventOnCollectSystemWidget>(OnEventCollectSystemWidget,
                HandlerPrioritySystemCollectWidget.AdTaskWidget);

        }

        protected async void OnEventCollectSystemWidget(Action handleEndAction, EventOnCollectSystemWidget evt,
            IEventHandlerScheduler eventHandlerScheduler)
        {

            if (Client.Get<UserController>().GetUserLevel() >= 5)
            {
                var adTaskInfo = GetAdTaskInfo();
                if (adTaskInfo != null && adTaskInfo.TaskStep < adTaskInfo.Rewards.Count)
                {
                    var widget = await View.CreateView<AdTaskSystemWidget>();
                    evt.viewController.AddSystemWidget(widget);
                }
            }

            handleEndAction.Invoke();
        }

        protected async void OnEventPreNoticeLevelChanged(EventPreNoticeLevelChanged evt)
        {
            if (evt.levelUpInfo != null)
            {
                await RefreshAdLogicConfig();

                if (evt.levelUpInfo.Level == 5)
                {
                    var adTaskInfo = GetAdTaskInfo();
                    if (adTaskInfo != null && adTaskInfo.TaskStep < adTaskInfo.Rewards.Count)
                    {
                        var widget = await View.CreateView<AdTaskSystemWidget>();
                        EventBus.Dispatch(new EventSystemWidgetNeedAttach(widget, 0));
                    }
                }
            }
        }

        protected async void OnEnterMachineScene(EventEnterMachineScene evt)
        {
            var mysteryBoxView = await View.CreateView<MysteryBoxView>();
            mysteryBoxView.transform.SetParent(evt.context.MachineUICanvasTransform,false);
            mysteryBoxView.transform.SetAsFirstSibling();
            mysteryBoxView.viewController.SetUpViewState();
            evt.context.SubscribeDestroyEvent(mysteryBoxView);
        }

        public override void CleanUp()
        {
            base.CleanUp();
            
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNewuserlevel, 
                null);

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdSeperateCooldown, 
                null);
            
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdOverdisplay, 
                null);

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNofill, 
                null);
            
            ////////插屏///////
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdMysteryPower, 
                null);

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNewuserlevel, 
                null);

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial,
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdCommonCooldown,
                null);

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdSeperateCooldown, 
                null);

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNofill, 
                null);
            
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdPaid, 
                null);

        }

        public bool Init()
        {
            if (isInit)
            {
                DebugUtil.LogError("AdLogicManager already initialed");
                return false;
            }

            isInit = true;
            
            adModel = new AdModel();
            adModel.InitAdReward();

            ///////////////激励视频////////////////////
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNewuserlevel, 
                pos => !IsRvUnlocked(pos));

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdSeperateCooldown, 
                pos => IsRewardVideoInCD((eAdReward)Enum.Parse(typeof(eAdReward), pos)));
            
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdOverdisplay, 
                pos => adModel.IsRewardedVideoMeetCap((eAdReward)Enum.Parse(typeof(eAdReward), pos)));

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.RewardVideo, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNofill, 
                pos => !IsRewardVideoReady((eAdReward)Enum.Parse(typeof(eAdReward), pos)));
            
            ////////插屏///////
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdMysteryPower, 
                pos => AdLogicManager.Instance.specialOrder);

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNewuserlevel, 
                pos => !IsInterstitialUnlocked((eAdInterstitial)Enum.Parse(typeof(eAdInterstitial), pos)));

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial,
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdCommonCooldown,
                pos => IsAdCommonCooldown((eAdInterstitial)Enum.Parse(typeof(eAdInterstitial), pos)));

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdSeperateCooldown, 
                pos => IsInterstitialInCD((eAdInterstitial)Enum.Parse(typeof(eAdInterstitial), pos)));

            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNofill, 
                pos => !IsInterstitialReady((eAdInterstitial)Enum.Parse(typeof(eAdInterstitial), pos)));   
            
            AdLogicManager.Instance.RegisterFailDelegate(Dlugin.AD_Type.Interstitial, 
                BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdPaid, 
                IsUserPaidForInterstitial);
            

            //AdRewardedVideoPlacementMonitor.BindUICamera(UIRoot.Instance.mUICamera);
            
            return true;
        }

        // public bool IsRewardVideoOverDisplay(eAdReward placeId)
        // {
        //     if (placeId == eAdReward.AdTask)
        //     {
        //         return true;
        //     }
        //
        //     return adModel.IsRewardedVideoMeetCap(placeId);
        // }

        public bool IsUserPaidForInterstitial(string pos)
        {
            return false;
        }

        public void ResetAdCD()
        {
            foreach (var pos in Enum.GetValues(typeof(eAdInterstitial)))
            {
                PlayerPrefs.SetString($"ad_Interstitial_last_show_time_pos_{(int)pos}", "0");
                PlayerPrefs.SetInt($"ad_Interstitial_pos_{(int)pos}", 0);
            }
        }


        private int GetInterstitialWatchCount(int pos)
        {
            adModel.TryToSetAdResetTime();
            return PlayerPrefs.GetInt($"ad_Interstitial_pos_{pos}", 0);
        }

        private int GetRewardVideoWatchCount(eAdReward pos)
        {
            Dictionary<string, int> watchCount = StorageManager.Instance.GetStorage<StorageAd>().AdVideoWatchCount;

            int posInt = (int)pos;

            string posKey = posInt.ToString();

            if (!watchCount.ContainsKey(posKey))
            {
                watchCount.Add(posKey, 0);
            }

            return watchCount[posKey];
        }

        private bool IsRvUnlocked(string pos)
        {
            if (adModel.AdRewardConfigs == null) 
                return false;
            
            foreach (var kv in adModel.AdRewardConfigs)
            {
                var cfg = kv.Value;
                if (cfg.placeId == (int)Enum.Parse(typeof(eAdReward),pos))
                {
                    var userLevel = Client.Get<UserController>().GetUserLevel();
                    
                    if (cfg.effectiveLevel <= (long)userLevel)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsInterstitialUnlocked(eAdInterstitial pos)
        {
            if (adModel.AdInterstitialConfigs == null) return false;
            foreach (var kv in adModel.AdInterstitialConfigs)
            {
                var cfg = kv.Value;
                if (cfg.placeId == (int)pos)
                {
                    var userLevel = Client.Get<UserController>().GetUserLevel();
                    
                    if (cfg.limitLevel <= (long)userLevel)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        public bool ShouldShowRV(eAdReward pos, bool withBI = true)
        {
  #if UNITY_EDITOR
            return false;
  #endif
            return AdLogicManager.Instance.ShouldShowRV(pos.ToString(), withBI);
        }

        private bool IsAdCommonCooldown(eAdInterstitial pos)
        {
            if (pos != eAdInterstitial.E_Background) 
                return false;
            return AdModel.GetTimeStamp() - backgroundStartTime < 30 * 1000;
        }

        public ulong GetRandomRvReward(eAdReward posId, ulong baseReward)
        {
             var weightRatio = adModel.GetArg2(posId);
             
             if(weightRatio == null || weightRatio.Count < 2)
                 return baseReward;
             
             var length = weightRatio.Count / 2;
             
             var ratios = new List<int>(length);
             var weights = new List<int>(length);

             int totalWeight = 0;
            
             for (int i = 0; i < length; i++)
             {
                 ratios.Add(weightRatio[i * 2]);
                 weights.Add(weightRatio[i * 2 + 1]);
                 totalWeight += weights[i];
             }

             var ratio = (ulong)ratios[XUtility.RandomSelect(weights, totalWeight)];

             return baseReward * ratio / 100;
        }

        private bool IsInterstitialInCD(eAdInterstitial pos)
        {
            if (pos == eAdInterstitial.E_Background)
                return false;
            var cd = INTERSTITIAL_GLOBAL_CD;
            var cfg = adModel.GetInterstitialConfig((int)pos);
            if (cfg!=null)
            {
                cd = cfg.showInterval * 1000;
            }

            var str = PlayerPrefs.GetString($"ad_Interstitial_last_show_time_pos_{(int)pos}", "0");
            var lastInterstitialTime = long.Parse(str);
            var interval = AdModel.GetTimeStamp() - lastInterstitialTime;
            var isInCD = interval < cd;
            DebugUtil.Log($"插屏广告在CD中?->{AdModel.GetTimeStamp()} - {lastInterstitialTime} = {interval}, cd = {cd}, isInCD = {isInCD}");

            if (isInCD)
            {
                DebugUtil.Log("插屏广告在CD中");
            }

            return isInCD;
        }

        private bool IsInterstitialTimesNotEnough(eAdInterstitial pos)
        {
            if (adModel != null)
            {
                var cfg = adModel.GetInterstitialConfig((int) pos);
                if (cfg != null)
                {
                    DebugUtil.Log($"插屏广告次数：{GetInterstitialWatchCount((int) pos)}/{cfg.limitPerDay}");
                    return GetInterstitialWatchCount((int) pos) >= cfg.limitPerDay;
                }
            }
            return true;
        }


        private bool IsRewardVideoInCD(eAdReward pos)
        {
            return adModel.IsRewardedVideoInCD(pos);
        }

        public bool ShouldShowInterstitial(eAdInterstitial pos, bool withBI = true)
        {
            return AdLogicManager.Instance.ShouldShowInterstitial(pos.ToString(), withBI);
        }

        public bool TryShowInterstitial(eAdInterstitial pos)
        {
            XDebug.Log("尝试展示插屏广告，位置为：" + pos);
            
            if (IsInterstitialTimesNotEnough(pos))
            {
                return false;
            }
#if UNITY_EDITOR
            return true;
#else
            XDebug.Log("TryShowInterstitialInternal:" + pos);

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAdsPop, ("adsType", "InterstitialAd"), ("placeId", pos.ToString()), ("userGroup", AdConfigManager.Instance.MetaData.GroupId.ToString()));

            if (AdLogicManager.Instance.TryShowInterstitialInternal(pos.ToString(), (b, s) =>
            {
                if(b)
                {
                    adModel.AddInterstitialWatchCount(pos);
                }
                EventBus.Dispatch(new EventAdEnd());
                EmptyInCallBack(pos);
            }))
            {
                EventBus.Dispatch(new EventAdStart());
                return true;
            }
            DebugUtil.LogError("TryShowInterstitial 4");
            return false;
#endif
        }

        public void DEBUG_Reset_Interstitial()
        {
            for (var pos = 0; pos < 2; pos++)
            {
                PlayerPrefs.SetInt($"ad_Interstitial_pos_{pos}", 0);
                PlayerPrefs.SetString($"ad_Interstitial_last_show_time_pos_{pos}", "0");
            }
        }

        public bool TryShowRewardedVideo(eAdReward pos, Action<bool, string> cb)
        {
            DebugUtil.Log("尝试展示奖励视频，位置为：" + pos);
            
#if UNITY_EDITOR
           // EventBus.Dispatch(new EventAdStart(cb));
            adModel.AddRewardedVideoWatchCount(pos);
            cb(true, "");
            //EventBus.Dispatch(new EventAdEnd());
            return true;
#else
 
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventAdsPop,
                ("adsType", "rewardVideo"),
                ("placeId", $"{pos}"),
                ("groupId", $"{AdConfigManager.Instance.MetaData.GroupId}")
            );

            float startTime = Time.realtimeSinceStartup;
            if(AdLogicManager.Instance.TryShowRewardedVideoInternal(pos.ToString(), (b, s) =>
            {
                EventBus.Dispatch(new EventAdEnd());
                if(b)
                {
                    adModel.AddRewardedVideoWatchCount(pos);
                }

                var duration = Time.realtimeSinceStartup - startTime;
                RewardedVideoCallBack(pos, b, duration, "");
            }))
            
            {
                if (cb != null)
                {
                    rewardedVideoCB = cb;
                }

                EventBus.Dispatch(new EventAdStart(pos));
                
                return true;
            }
            return false;
#endif
        }
        
        public bool IsInterstitialReady(eAdInterstitial pos)
        {
            return Dlugin.SDK.GetInstance().m_AdsManager.IsInterstitialReady();
        }

        public bool IsRewardVideoReady(eAdReward pos)
        {
            return Dlugin.SDK.GetInstance().m_AdsManager.IsRewardReady();
        }

        public string GetAdsStatus()
        {
            return "";
        }

        protected void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSpinRoundEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSpinRoundEnd.adStrategy == null ||
                eventSpinRoundEnd.adStrategy.Type != AdStrategy.Types.Type.MultipleBigWin)
            {
                handleEndCallback.Invoke();
                return;
            }

            if (ShouldShowRV(eAdReward.MultipleCoin, false))
            {
                PopupStack.ShowPopupNoWait<AdBigWinPopup>(argument: eventSpinRoundEnd.adStrategy,
                    closeAction: handleEndCallback);
            }
            else
            {
                handleEndCallback.Invoke();
            }
        }

        public void RefreshAdConfigFromPlatform()
        {
            // MethodInfo dynMethod = typeof(DragonPlus.ConfigHub.ConfigHubManager).GetMethod("getRemoteConfig",
            //     BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof(bool)}, null);
            // dynMethod?.Invoke(DragonPlus.ConfigHub.ConfigHubManager.Instance, new object[] {true});
            
            DragonPlus.ConfigHub.ConfigHubManager.Instance.CheckRemoteConfig(true);
        }
        
        protected void OnSceneSwitchEnd(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (ViewManager.Instance.IsUserClickBlocked())
            {
                handleEndCallback.Invoke();
                return;
            }
            
            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY
                && eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_MACHINE)
            {
               
                RefreshAdConfigFromPlatform();
                
            //    DragonPlus.ConfigHub.ConfigHubManager.Instance.CheckRemoteConfig();

                  //   XDebug.Log("CheckShouldShowInterstitial");
                 
                // if (ShouldShowRV(eAdReward.MysteryGift))
                // {
                //     XDebug.Log("ShouldShowRV(eAdReward.MysteryGift)_True");
                //     PopupStack.ShowPopupNoWait<AdMysteryGiftPopup>();
                // }
                // else
                // {
                //     XDebug.Log("ShouldShowRV(eAdReward.MysteryGift)_False");
                // }
            }

            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
            {
                if (!updateEnabled)
                {
                    EnableUpdate(1);
                }
            }

            handleEndCallback.Invoke();
        }

        public void RewardedVideoCallBack(eAdReward pos, bool success, float duration, string extra)
        {
            //Todo ADD callback logic
            DebugUtil.Log(pos + "视频播放时长:" + duration + " extra info is " + extra + " success is " + success);
            if (rewardedVideoCB != null)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventRvWatchedFinished, ("Suspected", duration < 10 ? "True": "False"),("RvDuration",duration.ToString()),("PlaceId",pos.ToString()));
                rewardedVideoCB(success, extra);
                rewardedVideoCB = null;
            }
        }

        public void EmptyInCallBack(eAdInterstitial pos)
        {
            //Todo ADD callback logic
            DebugUtil.Log("插屏播放完毕");

            //RemoveAdsModel.Instance.AddPopUpShowAdCount();
            //RemoveAdsModel.Instance.TryPopBuyRemoveAdsUi();
        }

        public void RecordBackgroundTime()
        {
            backgroundStartTime = AdModel.GetTimeStamp();
        }


        /// <summary>
        /// 从服务器领取RV广告奖励
        /// </summary>
        /// <param name="adPlaceID"></param>
        /// <returns></returns>
        public async Task<SClaimAdReward> ClaimRvReward(eAdReward adPlaceID)
        {
            return await ClaimRvReward(adPlaceID, -1, 0);
        }
        
        public async Task<SClaimAdReward> ClaimRvReward(eAdReward adPlaceID, int id, ulong count)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAdsCollect, ("placeId", adPlaceID.ToString()));
            var cClaimAdReward = new CClaimAdReward();
            cClaimAdReward.PlaceId = (ulong) adPlaceID;
            cClaimAdReward.UserGroup = GetUserGroupId();
         
            if (id > 0)
            {
                cClaimAdReward.ItemIdList.Add((uint)id);
                cClaimAdReward.ItemCountList.Add(count);
            }

            var response  = await APIManagerGameModule.Instance.SendAsync<CClaimAdReward, SClaimAdReward>(cClaimAdReward);

            if (response.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(response.Response.UserProfile));
                
                return response.Response;
            }
            else
            {
                XDebug.Log("ClaimRvReward:With Error Code:" + response.ErrorCode.ToString());
            }
            return null;
        }
        
        /// <summary>
        /// 从服务器领取RV广告奖励
        /// </summary>
        /// <param name="adPlaceID"></param>
        /// <param name="coin"></param>
        /// <returns></returns>
        public async Task<SClaimAdReward> ClaimRvReward(eAdReward adPlaceID, ulong coin)
        {
            return await ClaimRvReward(adPlaceID, 300, coin);
        }
        
        public async Task RefreshAdLogicConfig()
        {
            isRefreshAdConfigRefreshing = true;
            await _adLogicConfigModel.FetchModalDataFromServerAsync();
            isRefreshAdConfigRefreshing = false;
            EventBus.Dispatch(new EventAdTaskConfigRefreshed());
        }
        
        public float GetAdTaskRefreshLeftTime()
        {
            return _adLogicConfigModel.GetAdTaskRefreshLeftTime();
        }

        public override void Update()
        {
            var leftTime = _adLogicConfigModel.GetAdTaskRefreshLeftTime();
            if (leftTime <= 0 && enableAdConfigRefresh && !isRefreshAdConfigRefreshing)
            {
                RefreshAdLogicConfig();
            }
        }
    }
}