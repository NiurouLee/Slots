using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;


namespace GameModule
{
    public struct EventFbLogin : IEvent
    {
    }

    public struct EventGuestLogin : IEvent
    {

    }

    public struct EventAppleLogin : IEvent
    {

    }

    public struct EventEnterLoginScene : IEvent
    {
    }

    public struct EventStartAutoLogin : IEvent
    {
    }

    public struct EventRequestGameRestart : IEvent
    {
        public bool autoLogin;

        public EventRequestGameRestart(bool inAutoLogin)
        {
            autoLogin = inAutoLogin;
        }
    }

    public struct EventLoadingP1AssetComplete : IEvent { }

    public struct EventGameOnFocus : IEvent
    {

    }

    public struct EventGameOnPause : IEvent
    {

    }

    public struct EventOnApplicationQuit : IEvent
    {

    }


    public struct EventRequestReconnect : IEvent
    {
    }

    public struct EventNetWorkDisconnect : IEvent
    {
    }

    public struct EventLoginSuccess : IEvent
    {
        // public ServerPlayer player;
        // public EventGetUserFromServer(ServerPlayer inPlayer)
        // {
        //     player = inPlayer;
        // }
    }

    public struct EventOnSyncServerUserInfo : IEvent
    {

    }

    public struct EventLevelAchieved : IEvent
    {
        public LevelUpInfo levelUpInfo;

        public EventLevelAchieved(LevelUpInfo inLevelUpInfo)
        {
            levelUpInfo = inLevelUpInfo;
        }
    }

    public struct EventLevelChanged : IEvent
    {
        public LevelUpInfo levelUpInfo;

        public EventLevelChanged(LevelUpInfo inLevelUpInfo)
        {
            levelUpInfo = inLevelUpInfo;
        }
    }

    public struct EventPreNoticeLevelChanged : IEvent
    {
        public LevelUpInfo levelUpInfo;

        public EventPreNoticeLevelChanged(LevelUpInfo inLevelUpInfo)
        {
            levelUpInfo = inLevelUpInfo;
        }
    }

    public struct EventSwitchScene : IEvent
    {
        public SceneType sceneType;
        public string machineId;
        public string assetId;
        public string enterType;
        public bool isBackToLastScene;
        public bool allowBackButton;

        public EventSwitchScene(SceneType inSceneType, bool inAllowBackButton = true)
        {
            sceneType = inSceneType;
            machineId = "";
            enterType = "";
            assetId = "";
            isBackToLastScene = false;
            allowBackButton = inAllowBackButton;
        }
        public EventSwitchScene(SceneType inSceneType, string inMachineId, bool inAllowBackButton = true, string inEnterType = "")
        {
            sceneType = inSceneType;
            machineId = inMachineId;
            enterType = inEnterType;
            assetId = inMachineId;
            isBackToLastScene = false;
            allowBackButton = inAllowBackButton;
        }

        public EventSwitchScene(SceneType inSceneType, string inMachineId, string inAssetId, bool inAllowBackButton = true, string inEnterType = "")
        {
            sceneType = inSceneType;
            machineId = inMachineId;
            enterType = inEnterType;
            assetId = inAssetId;
            isBackToLastScene = false;
            allowBackButton = inAllowBackButton;
        }
    }

    public struct EventBackToLastScene : IEvent
    {

    }



    public struct EventSceneSwitchMask : IEvent
    {
        public SwitchMask mask;
        public int switchActionId;

        public EventSceneSwitchMask(SwitchMask inMask, int inSwitchActionId)
        {
            mask = inMask;
            switchActionId = inSwitchActionId;
        }
    }


    public struct EventBeforeEnterScene : IEvent
    {
        public SceneType sceneType;

        public EventBeforeEnterScene(SceneType inSceneType)
        {
            sceneType = inSceneType;
        }
    }

    public struct EventShowPopup : IEvent
    {
        public PopupArgs popupArgs;

        public EventShowPopup(PopupArgs inDialogArgs)
        {
            popupArgs = inDialogArgs;
        }
    }

    public struct EventUpdateExp : IEvent
    {
        public bool updateToFull;
        public EventUpdateExp(bool inUpdateToFull = false)
        {
            updateToFull = inUpdateToFull;
        }
    }

    public struct EventBuffDataUpdated : IEvent
    {

    }

    public struct EventLevelRushStateChanged : IEvent
    {

    }

    public struct EventEnqueuePopup : IEvent
    {
        public PopupArgs popupArgs;


        public EventEnqueuePopup(PopupArgs inPopupArgs)
        {
            popupArgs = inPopupArgs;
        }
    }

    public struct EventEnqueueFencePopup : IEvent
    {
        public Action fenceAction;
        public BlockLevel blockLevel;
        public EventEnqueueFencePopup(Action inFenceAction, BlockLevel inBlockLevel = BlockLevel.DefaultLevel)
        {
            fenceAction = inFenceAction;
            blockLevel = inBlockLevel;
        }
    }

    public struct EventPopupClose : IEvent
    {
        public Type popupTypeType;
        public EventPopupClose(Type t)
        {
            popupTypeType = t;
        }
    }

    public struct EventNewBetLevelUnlockedDueToPurchase : IEvent
    {

    }

    public struct EventTriggerPopupPool : IEvent
    {
        public string triggerType;
        public Action handleEndCallback;
        public EventTriggerPopupPool(string inTriggerType, Action inHandleEndCallback = null)
        {
            triggerType = inTriggerType;
            handleEndCallback = inHandleEndCallback;
        }
    }

    public struct EventBalanceUpdate : IEvent
    {
        public long delta;               //玩家金币变化量
        public string source;            //引起金币变化的原因
        public bool hasAnimation;        //是否有金币滚动动画
        public bool mute;                //是否有金币滚动音效

        public EventBalanceUpdate(long inDelta, string inSource, bool inHasAnimation = true, bool inMute = false)
        {
            delta = inDelta;
            source = inSource;
            mute = inMute;
            hasAnimation = inHasAnimation;
        }

        public EventBalanceUpdate(ulong inDelta, string inSource, bool inHasAnimation = true, bool inMute = false)
        {
            delta = (long)inDelta;
            source = inSource;
            mute = inMute;
            hasAnimation = inHasAnimation;
        }

        public EventBalanceUpdate(Item item, string inSource, bool inHasAnimation = true, bool inMute = false)
        {
            delta = (long)item.Coin.Amount;
            source = inSource;
            mute = inMute;
            hasAnimation = inHasAnimation;
        }
    }

    public class EventEmeraldBalanceUpdate : IEvent
    {
        public long delta;               //玩家Emerald变化量
        public string source;            //引起Emerald变化的原因
        public bool hasAnimation;        //是否有Emerald滚动动画
        public bool mute;                //是否有Emerald滚动音效
        public EventEmeraldBalanceUpdate(long inDelta, string inSource, bool inHasAnimation = true, bool inMute = false)
        {
            delta = inDelta;
            source = inSource;
            mute = inMute;
            hasAnimation = inHasAnimation;
        }

        public EventEmeraldBalanceUpdate(ulong inDelta, string inSource, bool inHasAnimation = true, bool inMute = false)
        {
            delta = (long)inDelta;
            source = inSource;
            mute = inMute;
            hasAnimation = inHasAnimation;
        }

        public EventEmeraldBalanceUpdate(Item item, string inSource, bool inHasAnimation = true, bool inMute = false)
        {
            delta = (long)item.Coin.Amount;
            source = inSource;
            mute = inMute;
            hasAnimation = inHasAnimation;
        }
    }

    public class EventCurrencyUpdate : IEvent
    {
        public long coinDelta;
        public long emeraldDelta;
        public bool hasAnimation;
        public float delayTime = 2f;
        public string source;
        public EventCurrencyUpdate(long inCoinDelta, long inEmeraldDelta, bool inHasAnimation = true, string inSource = "", float inDelayTime = 2f)
        {
            coinDelta = inCoinDelta;
            emeraldDelta = inEmeraldDelta;
            hasAnimation = inHasAnimation;
            source = inSource;
            delayTime = inDelayTime;
        }
    }

    public class EventCheckAndShowStoreBonusGuide : IEvent
    {

    }

    public class EventShowNewAvatarGuide : IEvent
    {

    }

    public struct EventGuideFinished : IEvent
    {
        public Guide Guide;
        public Action Callback;
        public EventGuideFinished(Guide guide, Action callback = null)
        {
            Guide = guide;
            Callback = callback;
        }
    }

    public struct EventGuideShowMaxBetTip : IEvent
    {
    }

    public struct EventMaxBetUnlocked : IEvent
    {
        public ulong maxBet;

        public EventMaxBetUnlocked(ulong inMaxBet)
        {
            maxBet = inMaxBet;
        }
    }

    public struct EventLevelGuideUpdate : IEvent
    {
    }

    public struct EventEnterMachineScene : IEvent
    {
        public MachineContext context;

        public EventEnterMachineScene(MachineContext inContext)
        {
            context = inContext;
        }
    }

    public struct EventExecuteJump : IEvent
    {
        public JumpInfo jumpInfo;

        public EventExecuteJump(JumpInfo inJumpInfo)
        {
            jumpInfo = inJumpInfo;
        }
    }


    public struct EventShopProductUpdated : IEvent
    {

    }

    public struct EventPiggyBankUpdated : IEvent
    {

    }

    public struct EventShowGameInfo : IEvent
    {

    }

    public struct EventStoreBonusStateChanged : IEvent
    {

    }

    public struct EventBuyMenuStatusChanged : IEvent
    {

    }



    public struct EventCoinCollectFx : IEvent
    {
        public bool isFxBegin;

        public EventCoinCollectFx(bool inIsFxBegin)
        {
            isFxBegin = inIsFxBegin;
        }
    }

    public struct EventSetSettingCanvas : IEvent
    {
        public int popup;

        public EventSetSettingCanvas(int popup)
        {
            this.popup = popup;
        }
    }

    public struct EventSetPiggyCanvas : IEvent
    {
        public int popup;

        public EventSetPiggyCanvas(int popup)
        {
            this.popup = popup;
        }
    }

    public struct EventIAPCollectClosed : IEvent
    {

    }

    public struct EventEnterLobbyScene : IEvent
    {
        public SceneType fromSceneType;

        public EventEnterLobbyScene(SceneType inFromSceneType)
        {
            fromSceneType = inFromSceneType;
        }
    }


    public struct EventSpinRoundStart : IEvent
    {

    }

    public struct EventSpinSuccess : IEvent
    {

    }

    public struct EventPerformRemoved : IEvent
    {
        public PerformCategory performCategory;

        public EventPerformRemoved(PerformCategory inPerformCategory)
        {
            performCategory = inPerformCategory;
        }
    }

    public struct EventSpinRoundEnd : IEvent
    {
        public float winRate;
        public ulong winChips;
        public uint winLevel;

        public AdStrategy adStrategy;

        public EventSpinRoundEnd(float inWinRate, ulong inWinChips, uint inWinLevel, AdStrategy inAdStrategy = null)
        {
            winRate = inWinRate;
            winChips = inWinChips;
            winLevel = inWinLevel;
            adStrategy = inAdStrategy;
        }
    }

    public struct EventSubRoundEnd : IEvent
    {

    }

    public struct EventPaymentSuccess : IEvent
    {

    }

    public struct EventPaymentFailed : IEvent
    {

    }

    public struct EventShowPayTable : IEvent
    {

    }

    public struct EventPiggyUnLock : IEvent
    {

    }

    public struct EventPiggyConsumeComplete : IEvent
    {

    }


    public struct EventDealOfferConsumeComplete : IEvent
    {

    }

    public struct EventCommonPaymentComplete : IEvent
    {
        public string shopType;
        public EventCommonPaymentComplete(string inShopType)
        {
            shopType = inShopType;
        }
    }

    public struct EventUserNewAvatarStateChanged : IEvent { }

    // public struct EventUserExpUpdate : IEvent
    // {
    //     public LevelUpData levelUpData;
    //
    //     public EventUserExpUpdate(LevelUpData inLevelUpData)
    //     {
    //         levelUpData = inLevelUpData;
    //     }
    // }

    public struct EventInboxUpdate : IEvent
    {

    }

    public struct EventInboxCatalogComplete : IEvent
    {

    }

    public struct EventInboxMask : IEvent
    {
        public int mask;

        public EventInboxMask(int showMask)
        {
            this.mask = showMask;
        }
    }

    public struct EventPrizeMailUpdate : IEvent
    {

    }

    public struct EventMailUpdate : IEvent
    {

    }

    public struct EventIapConsumeEndSchedule : IEvent
    {

    }

    public struct EventCoinDashDataUpdate : IEvent { }

    public struct EventDailyMissionUpdate : IEvent
    {

    }

    public struct EventDailyMissionUnLock : IEvent
    {

    }

    public struct EventPauseMachine : IEvent
    {

    }

    public struct EventQuestUnlock : IEvent
    {
    }



    public struct EventQuestUpdate : IEvent
    {

    }

    public struct EventSeasonQuestUnlock : IEvent
    {

    }

    public struct EventSeasonQuestDifficultyChose : IEvent
    {

    }

    public struct EventSeasonQuestCheckQuestFinish : IEvent
    {

    }

    public struct EventSeasonQuestClaimSuccess : IEvent
    {
    }

    public struct EventQuestClaimSuccess : IEvent
    {
    }

    public struct EventMusicEnabled : IEvent
    {
        public bool enabled;
        public EventMusicEnabled(bool inEnabled)
        {
            enabled = inEnabled;
        }
    }
    public struct EventSoundEnabled : IEvent
    {
        public bool enabled;
        public EventSoundEnabled(bool inEnabled)
        {
            enabled = inEnabled;
        }
    }

    public struct EventPayTableShowed : IEvent
    {

    }

    public struct EventPayTableClosed : IEvent
    {

    }

    public struct EventJackpotNotificationEnabled : IEvent
    {
        public bool enabled;
        public EventJackpotNotificationEnabled(bool inEnabled)
        {
            enabled = inEnabled;
        }
    }

    public struct EventBindingFaceBook : IEvent
    {

    }

    public struct EventBindStatusChanged : IEvent
    {

    }

    public struct EventCheckNeedDisableGameWelcome : IEvent
    {

    }

    public struct EventFirstPopupShow : IEvent
    {

    }

    public struct EventStopAutoSpin : IEvent
    {

    }
    public struct EventOnBalanceIsInsufficient : IEvent
    {

    }


    public struct EventLastPopupClose : IEvent
    {

    }



    public struct EventLevelUpTipsChecking : IEvent
    {
        // public LevelUpData levelUpData;
        // public EventLevelUpTipsChecking(LevelUpData inLevelUpData)
        // {
        //     levelUpData = inLevelUpData;
        // }
    }

    public struct EventLevelUpUnlockNewMachine : IEvent
    {
        public LevelUpInfo levelUpInfo;
        public EventLevelUpUnlockNewMachine(LevelUpInfo inLevelUpInfo)
        {
            levelUpInfo = inLevelUpInfo;
        }
    }

    public struct EventContactUsHasNewMessage : IEvent
    {
        public bool hasNewMessage;
        public EventContactUsHasNewMessage(bool inHasNewMessage = true)
        {
            hasNewMessage = inHasNewMessage;
        }
    }
    public struct EventSceneSwitchBegin : IEvent
    {
        public SceneType targetScene;
        public SceneType currentSceneType;

        public EventSceneSwitchBegin(SceneType inCurrentSceneType, SceneType inTargetScene)
        {
            currentSceneType = inCurrentSceneType;
            targetScene = inTargetScene;
        }
    }

    public struct EventSceneSwitchEnd : IEvent
    {
        public SceneType lastSceneType;
        public SceneType currentSceneType;
        public bool isBackToLastScene;

        public EventSceneSwitchEnd(SceneType inSceneType, SceneType
            inLastSceneType, bool inIsBackToLastScene = false)
        {
            currentSceneType = inSceneType;
            lastSceneType = inLastSceneType;
            isBackToLastScene = inIsBackToLastScene;
        }
    }
    
    public struct EventSilentSceneSwitchEnd : IEvent
    {
        public SceneType lastSceneType;
        public SceneType currentSceneType;
        public bool isBackToLastScene;

        public EventSilentSceneSwitchEnd(SceneType inSceneType, SceneType
            inLastSceneType, bool inIsBackToLastScene = false)
        {
            currentSceneType = inSceneType;
            lastSceneType = inLastSceneType;
            isBackToLastScene = inIsBackToLastScene;
        }
    }

    public struct EventSceneSwitchBackToQuest : IEvent
    {
    }

    public struct EventEnterLobbyServerInfoReady : IEvent
    {

    }

    public struct EventBindEmailEnd : IEvent
    {
    }

    public struct EventOnVipLevelUp : IEvent
    {

    }

    public struct EventOnJackpotNotification : IEvent
    {

    }

    public struct EventHotDayEntryStatusRefresh : IEvent
    {
    }


    public struct EventOnWelcomeBonusClosed : IEvent
    {

    }

    public struct EventHighLightMachineEntrance : IEvent
    {
        public string machineId;
        public EventHighLightMachineEntrance(string inMachineId)
        {
            machineId = inMachineId;
        }
    }


    public struct EventOnMachineEntranceHighlight : IEvent
    {
        public Transform containerTransform;
        public EventOnMachineEntranceHighlight(Transform inTransform)
        {
            containerTransform = inTransform;
        }
    }

    public struct EventBetChanged : IEvent
    {
        public int delta;
        public int betLevel;
        public ulong totalBet;
        public int maxBetLevel;
        public EventBetChanged(int inDelta, int inBetLevel, ulong inTotalBet, int inMaxBetLevel)
        {
            delta = inDelta;
            betLevel = inBetLevel;
            totalBet = inTotalBet;
            maxBetLevel = inMaxBetLevel;
        }
    }

    public struct EventGuideTaskUpdate : IEvent
    {
        public bool hasTaskFinished;
        public EventGuideTaskUpdate(bool inHasTaskFinished)
        {
            hasTaskFinished = inHasTaskFinished;
        }
    }

    public struct EventGuideUserStep : IEvent
    {
        public string stepName;
        public EventGuideUserStep(string inStepName)
        {
            stepName = inStepName;
        }
    }


    public struct EventToggleWidget : IEvent
    {
        public bool showWidget;
        public EventToggleWidget(bool inShowWidget)
        {
            showWidget = inShowWidget;
        }
    }


    public struct EventOnApplicationDeepLink : IEvent
    {

    }

    public struct EventCheckBalanceChips : IEvent
    {
        public long chipsToCheck;
        public EventCheckBalanceChips(long inChipsToCheck)
        {
            chipsToCheck = inChipsToCheck;
        }
    }

    public struct EventRequestEnterGame : IEvent
    {
        public string machineId;
        public string enterType;
        public EventRequestEnterGame(string inMachineId, string inEnterType)
        {
            machineId = inMachineId;
            enterType = inEnterType;
        }
    }

    public struct EventCashBackBuffEnd : IEvent
    {
        public long chips;
        public EventCashBackBuffEnd(long chip)
        {
            this.chips = chip;
        }
    }

    public struct EventCashBackUserDateChanged : IEvent { }

    public struct EventSetActivityIcon : IEvent
    {
        public Transform parentTransform;
        public View parentView;
        public EventSetActivityIcon(View parentView, Transform parentTransform)
        {
            this.parentView = parentView;
            this.parentTransform = parentTransform;
        }
    }

    public struct EventGoldenCouponActivityUserDateChanged : IEvent { }

    public struct EventSeasonPassUpdate : IEvent
    {
        public bool CheckPaid;

        public EventSeasonPassUpdate(bool check = false)
        {
            CheckPaid = check;
        }
    }

    public struct EventSeasonPassUnlocked : IEvent
    {
    }

    public struct EventSeasonPassChestUpdate : IEvent
    {
        public string ActionName;
        public Action Action;

        public EventSeasonPassChestUpdate(string actionName, Action action = null)
        {
            Action = action;
            ActionName = actionName;
        }
    }

    public struct EventUserProfileUpdate : IEvent
    {
        public UserProfile userProfile;

        public EventUserProfileUpdate(UserProfile inProfile)
        {
            userProfile = inProfile;
        }
    }

    public struct EventUpdateRoleInfo : IEvent
    {
    }

    public struct EventStorePrizeButtonAttachExtraItem : IEvent
    {
        public PriceButtonExtraItemView priceButtonExtraItemView;

        public EventStorePrizeButtonAttachExtraItem(PriceButtonExtraItemView inExtraView)
        {
            priceButtonExtraItemView = inExtraView;
        }
    }

    public struct EventOnSuperSpinXPurchaseSucceed : IEvent
    {
        public VerifyExtraInfo verifyExtraInfo;
        public Action<Action<FulFillCallbackArgs>> fulFillRequestHandler;

        public EventOnSuperSpinXPurchaseSucceed(VerifyExtraInfo inVerifyExtraInfo,
            Action<Action<FulFillCallbackArgs>> inFulFillRequestHandler)
        {
            verifyExtraInfo = inVerifyExtraInfo;
            fulFillRequestHandler = inFulFillRequestHandler;
        }
    }

    public struct EventPaymentFinish : IEvent
    {
        public VerifyExtraInfo verifyExtraInfo;

        public EventPaymentFinish(VerifyExtraInfo extraInfo)
        {
            verifyExtraInfo = extraInfo;
        }
    }

    public struct EventRefreshUserProfile : IEvent
    {

    }

    public struct EventNetworkClosed : IEvent
    {

    }

    public struct EventOnUnExceptedServerError : IEvent
    {
        public string errorInfo;
        public EventOnUnExceptedServerError(string inErrorInfo)
        {
            errorInfo = inErrorInfo;
        }
    }

    public struct EventGiftBoxSetToEmpty : IEvent
    {

    }

    public struct EventSpinSystemContentUpdate : IEvent
    {
        public RepeatedField<AnyStruct> systemContent;
        public string updateSource;
        public EventSpinSystemContentUpdate(RepeatedField<AnyStruct> inSystemContent, string inUpdateSource)
        {
            systemContent = inSystemContent;
            updateSource = inUpdateSource;
        }
    }

    public struct EventSeasonPassCloseBuyLevel : IEvent
    {

    }

    public struct EventSeasonPassCloseBuyGolden : IEvent
    {

    }

    public struct EventTimeBonusStateChanged : IEvent
    {

    }

    public struct EventOnWheelSpinEnd : IEvent
    {

    }

    public struct EventTimeBonusGoldenWheelPurchaseSucceed : IEvent
    {
        public VerifyExtraInfo verifyExtraInfo;
        public Action<Action<FulFillCallbackArgs>> fulFillRequestHandler;

        public EventTimeBonusGoldenWheelPurchaseSucceed(VerifyExtraInfo inVerifyExtraInfo,
            Action<Action<FulFillCallbackArgs>> inFulFillRequestHandler)
        {
            verifyExtraInfo = inVerifyExtraInfo;
            fulFillRequestHandler = inFulFillRequestHandler;
        }
    }

    public struct EventOnCollectSystemWidget : IEvent
    {
        public SystemWidgetContainerViewController viewController;

        public EventOnCollectSystemWidget(SystemWidgetContainerViewController inViewController)
        {
            viewController = inViewController;
        }
    }

    public struct EventOnSystemWidgetCollectedEnd : IEvent
    {
        public SystemWidgetContainerViewController viewController;

        public EventOnSystemWidgetCollectedEnd(SystemWidgetContainerViewController inViewController)
        {
            viewController = inViewController;
        }
    }

    public struct EventSystemWidgetActive : IEvent
    {
        public bool Active;
        public EventSystemWidgetActive(bool active)
        {
            Active = active;
        }
    }

    public struct EventSystemWidgetNeedAttach : IEvent
    {
        public ISystemWidget systemWidget;
        public int priority;
        public EventSystemWidgetNeedAttach(ISystemWidget inSystemWidget, int inPriority)
        {
            priority = inPriority;
            systemWidget = inSystemWidget;
        }
    }

    public struct EventSystemWidgetDetached : IEvent
    {
        public ISystemWidget systemWidgetView;
        public EventSystemWidgetDetached(ISystemWidget inSystemWidget)
        {
            systemWidgetView = inSystemWidget;
        }
    }

    public struct EventQuestDataUpdated : IEvent
    {

    }

    public struct EventSeasonQuestDataUpdated : IEvent
    {

    }

    public struct EventUpdateQuestWidgetProgress : IEvent
    {

    }

    public struct EventUpdateSeasonQuestWidgetProgress : IEvent
    {

    }

    //开启SeasonQuest，speedUp buff  和season quest pass buff的切换动画
    public struct EventEnableSeasonQuestBuffSwitchProgress : IEvent
    {

    }

    public struct EventQuestFinished : IEvent
    {

    }

    public struct EventQuestTimeOut : IEvent
    {

    }

    public struct EventSeasonQuestSeasonFinish : IEvent
    {

    }

    public struct EventUpdateLobbyBannerIconContent : IEvent
    {

    }

    public struct EventStartDownloadMachineAsset : IEvent
    {
        public string machineAssetId;
        public EventStartDownloadMachineAsset(string inMachineAssetId)
        {
            machineAssetId = inMachineAssetId;
        }
    }

    public struct EventMachineDownloadSizeUpdated : IEvent
    {

    }


    public struct EventMachineAssetDownloadFinished : IEvent
    {
        public string machineAssetId;
        public bool isSuccess;

        public EventMachineAssetDownloadFinished(string inMachineAssetId, bool inIsSuccess)
        {
            machineAssetId = inMachineAssetId;
            isSuccess = inIsSuccess;
        }
    }

    public struct EventCloseTimeBonusQuitPopup : IEvent
    {

    }

    public struct EventPushNotification : IEvent
    {
        public FortunexNotification notification;
        public EventPushNotification(FortunexNotification notification)
        {
            this.notification = notification;
        }
    }


    public struct EventRefreshWeekendVipMail : IEvent
    {
    }

    public struct EventInBoxItemUpdated : IEvent
    {
    }

    public struct EventOnLuckyWheelAdNoticeChoose : IEvent
    {
        public bool playAgain;
        public EventOnLuckyWheelAdNoticeChoose(bool inPlayAgain)
        {
            playAgain = inPlayAgain;
        }
    }

    public struct EventPNotification : IEvent
    {
        public PNotification notificationData;
        public EventPNotification(PNotification data)
        {
            notificationData = data;
        }
    }


    public struct EventAdConfigRefresh : IEvent
    {

    }

    public struct EventPurchasedInStore : IEvent
    {

    }


    public struct EventStoreClose : IEvent
    {
        public string storeOpenSource;

        public bool purchasedInStore;

        public EventStoreClose(string source, bool inPurchasedInStore = false)
        {
            storeOpenSource = source;
            purchasedInStore = inPurchasedInStore;
        }
    }

    public struct EventAdTaskStepUpdated : IEvent { }
    public struct EventAdTaskConfigRefreshed : IEvent { }



    public struct EventReceiveSListUserComplainMessage : IEvent { }
    public struct EventReceiveSSendUserComplainMessage : IEvent { }
    public struct EventAccountUpdate : IEvent { }

    public struct EventReceiveUserProfileInRole : IEvent { }

    public struct EventAdStart : IEvent
    {
        public eAdReward adPos;

        public EventAdStart(eAdReward inAdPos)
        {
            adPos = inAdPos;
        }
    }

    public struct EventAdEnd : IEvent { }

    public struct EventReceiveUserCoupons : IEvent { }

    public struct EventCollectStoreBonusFinish : IEvent
    {
        public SGetStoreBonus sGetStoreBonus;
        public EventCollectStoreBonusFinish(SGetStoreBonus sGetStoreBonus)
        {
            this.sGetStoreBonus = sGetStoreBonus;
        }
    }
    public struct EventShowSpinDropCardInfo : IEvent
    {
        public CardUpdateInfo cardUpdateInfo;

        public EventShowSpinDropCardInfo(CardUpdateInfo inCardUpdateInfo)
        {
            cardUpdateInfo = inCardUpdateInfo;
        }
    }

    public struct EventShowSpinDropCardFinished : IEvent
    {
        public CardUpdateInfo cardUpdateInfo;
        public EventShowSpinDropCardFinished(CardUpdateInfo inCardUpdateInfo)
        {
            cardUpdateInfo = inCardUpdateInfo;
        }
    }

    public struct EventOnShowAlbumGuide4Finished : IEvent
    {

    }

    public struct EventUpdateAlbumRedDotReminder : IEvent { }

    public struct EventAlbumSeasonEnd : IEvent { }
    public struct EventAlbumInfoDataUpdate : IEvent { }
    public struct EventAmazingHatStateUpdate : IEvent { }

    #region Activity
    public struct EventActivityExpire : IEvent
    {
        public string activityType;
        public string activityId;

        public EventActivityExpire(string inActivityType, string inActivityId)
        {
            activityType = inActivityType;
            activityId = inActivityId;
        }
    }
    public struct EventActivityCreate : IEvent
    {
        public string activityType;
        public string activityId;

        public EventActivityCreate(string inActivityType, string inActivityId)
        {
            activityType = inActivityType;
            activityId = inActivityId;
        }
    }
    public struct EventActivityOpen : IEvent
    {
        public string activityType;
        public string activityId;

        public EventActivityOpen(string inActivityType, string inActivityId)
        {
            activityType = inActivityType;
            activityId = inActivityId;
        }
    }
    public struct EventActivityServerDataUpdated : IEvent
    {
        public string activityType;
        public string activityId;

        public bool showAni;
        public MonopolyEnergyInfoWhenSpin monopolyEnergyInfoWhenSpin;

        public EventActivityServerDataUpdated(string inActivityType)
        {
            activityType = inActivityType;
            activityId = null;
            showAni = false;
            monopolyEnergyInfoWhenSpin = null;
        }

        public EventActivityServerDataUpdated(string inActivityType, string inActivityId)
        {
            activityType = inActivityType;
            activityId = inActivityId;
            showAni = false;
            monopolyEnergyInfoWhenSpin = null;
        }

        public EventActivityServerDataUpdated(string inActivityType, string inActivityId, bool inShowAni = false, MonopolyEnergyInfoWhenSpin inMonopolyEnergyInfoWhenSpin = null)
        {
            activityType = inActivityType;
            activityId = inActivityId;
            showAni = inShowAni;
            monopolyEnergyInfoWhenSpin = inMonopolyEnergyInfoWhenSpin;
        }
    }

    public struct EventJulyCarnivalActivityFinish : IEvent { }
    public struct EventChangeLobbyEntranceInActivity : IEvent
    {
        public LobbyScene lobbyScene;
        public EventChangeLobbyEntranceInActivity(LobbyScene inLobbyScene)
        {
            lobbyScene = inLobbyScene;
        }
    }

    public struct EventSetActivityIconInDailyMission : IEvent
    {
        public Transform parentTransform;
        public View parentView;
        public Mission mission;
        public int missionIndex;

        public EventSetActivityIconInDailyMission(View parentView, Transform parentTransform, Mission mission, int missionIndex)
        {
            this.parentView = parentView;
            this.parentTransform = parentTransform;
            this.mission = mission;
            this.missionIndex = missionIndex;
        }
    }

    #endregion
    public struct EventJulyCarnivalCollectItem : IEvent
    {
    }

    public struct EventJulyCarnivalRefreshItem : IEvent
    {
        public int nMissionIndex;

        public EventJulyCarnivalRefreshItem(int missionIndex)
        {
            nMissionIndex = missionIndex;
        }
    }
    public struct Event_Activity_Valentine2022_ReceiveUserDate : IEvent { }
    public struct Event_Activity_Valentine2022_ReceiveMainPageInfo : IEvent { }
    public struct Event_Activity_Valentine2022_PurchaseFinish : IEvent { }
    public struct Event_Activity_Valentine2022_CollectItem : IEvent { }

    public struct Event_CrazeSmash_EggInfoChanged : IEvent { }
    public struct Event_CrazeSmash_PurchaseFinish : IEvent { }
    public struct Event_CrazeSmash_GameFinish : IEvent { }
    public struct Event_CrazeSmash_BIG_WIN : IEvent { }
    public struct Event_CrazeSmash_Expire : IEvent { }
    public struct EventTreasureRaidOnExpire : IEvent { }
    public struct EventTreasureRaidDailyTaskRefresh : IEvent { }
    public struct EventTreasureRaidPurchaseFinish : IEvent { }
    public struct EventTreasureRaidRefreshPuzzleView : IEvent { }
    public struct EventTreasureRaidRefreshRankView : IEvent
    {
        public uint myRank;
        public EventTreasureRaidRefreshRankView(uint inMyRank)
        {
            myRank = inMyRank;
        }
    }

    public struct EventRushPassPaidFinish : IEvent { }

    public struct EventRushPassPaidFail : IEvent
    {
    }

    public struct EventOnLobbyCreated : IEvent
    {
        public LobbyScene lobbyScene;

        public EventOnLobbyCreated(LobbyScene lobbyScene)
        {
            this.lobbyScene = lobbyScene;
        }
    }

    public struct EventTreasureRaidRefreshChestBox : IEvent
    {
        public MonopolyRoundInfo roundInfo;
        public EventTreasureRaidRefreshChestBox(MonopolyRoundInfo roundInfo)
        {
            this.roundInfo = roundInfo;
        }
    }
}
