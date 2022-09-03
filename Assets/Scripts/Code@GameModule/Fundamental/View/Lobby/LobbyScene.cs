// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/21/11:37
// Ver : 1.0.0
// Description : LobbyScene.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class LobbyScene : SceneView<LobbyViewController>
    {
        public override bool IsPortraitScene { get; } = false;
        public override SceneType SceneType { get; } = SceneType.TYPE_LOBBY;

        [ComponentBinder("TopPanel")] public Transform topPanel;

        [ComponentBinder("BottomPanel")] public Transform bottomPanel;

        [ComponentBinder("FixedBanner")] public Transform fixedBanner;

        [ComponentBinder("ContentListView")] public Transform contentListView;

        [ComponentBinder("InBoxButton")] public Transform inboxButton;
        [ComponentBinder("PassButton")] public Transform passButton;
        [ComponentBinder("MissionButton")] public Transform missionButton;

        [ComponentBinder("DailyBonusButton")] public Button dailyBonusButton;
        [ComponentBinder("QuestButton")] public Transform questButton;
        [ComponentBinder("SeasonQuestButton")] public Transform seasonQuestButton;

        [ComponentBinder("TimerBonusGroup")] public Transform timerBonusGroup;

        [ComponentBinder("VIPButton")] public Button btnVIP;
        
        [ComponentBinder("ChallengeButton")] public Button challengeButton;
        
        [ComponentBinder("CollectionButton")] public Button collectionButton;
        
        [ComponentBinder("UIADSButton")] public Button uiADTaskButton;

        public BannerPageView fixedBannerPageView;


        [ComponentBinder("UIADSButtonInLobby")] 
        public Transform transformADSButtonInLobby;
        public UIADSButtonInLobbyView uiADSButtonInLobby;

        public LobbyScene(string address)
            : base(address, SceneType.TYPE_LOBBY)
        {

        }
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            fixedBannerPageView = AddChild<BannerPageView>(fixedBanner);
            var advertisement = Client.Get<BannerController>().GetLobbyFixedAdvertisement();
            fixedBannerPageView.viewController.SetUpContent(advertisement);

            // uiADSButtonInLobby = AddChild<UIADSButtonInLobbyView>(transformADSButtonInLobby);
            // uiADSButtonInLobby.Hide();
            
            AdaptBottomUI();
        }
        
        protected void AdaptBottomUI()
        {
            if (!ViewManager.Instance.IsPortrait)
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    var localScale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                    bottomPanel.localScale = new Vector3(localScale, localScale, localScale);
                }
            }
        }

        public override async Task PrepareAsyncAssets()
        {
            var task = new TaskCompletionSource<bool>();
            viewController.AddWaitTask(task, null);
          
            viewController.PrepareAsyncAssets(() =>
            {
                viewController.RemoveTask(task);
                task.SetResult(true);
            });
            
            await task.Task;
        }
    }

    public class LobbyViewController : ViewController<LobbyScene>
    {
        private BannerIconListView _bannerIconListView;
        private TopPanel _topPanel;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            SetUpContentList();

#if UNITY_EDITOR || !PRODUCTION_PACKAGE
            if (GameObject.Find("ShowPlayerInfo") == null)
            {
                GameObject obj = new GameObject("ShowPlayerInfo");
                obj.AddComponent<ShowPlayerInfo>();
            }
#endif
            ContentUpdater.EnableExceptionHandler(false);
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventUpdateLobbyBannerIconContent>(OnUpdateLobbyBannerIconContent);
            SubscribeEvent<EventPopupClose>(OnPopupClosed);
            SubscribeEvent<EventSceneSwitchEnd>(OnCheckStoreBonusGuide, HandlerPriorityWhenEnterLobby.LobbyStoreBonusGuide);
            SubscribeEvent<EventUserNewAvatarStateChanged>(OnAvatarNewStateChanged);
            SubscribeEvent<EventActivityOpen>(OnActivityOpen);
        }

        protected  void OnAvatarNewStateChanged(EventUserNewAvatarStateChanged evt)
        {
            if (Client.Get<UserController>().HasNewAvatar())
            {
                var state = Client.Storage.GetItem("NewAvatarGuideShowed","FALSE");
                if (state == "FALSE")
                {
                    canShowNewAvatarGuide = true;
                }
            }
        }
        
        protected  void OnCheckStoreBonusGuide(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
            {
                if (Client.Get<IapController>().IsStoreBonusCollectable())
                {
                    canCheckStoreBonusGuideCheckTriggered = true;
                }
                
                if (Client.Get<UserController>().HasNewAvatar())
                {
                    var state = Client.Storage.GetItem("NewAvatarGuideShowed","FALSE");
                    if (state == "FALSE")
                    {
                        canShowNewAvatarGuide = true;
                    }
                }
                
                if (eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_MACHINE)
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventBackLobby);
                }
            }
 
            handleEndCallback.Invoke();
        }

        protected bool canCheckStoreBonusGuideCheckTriggered = false;
        protected bool canShowNewAvatarGuide = false;

        protected void OnPopupClosed(EventPopupClose evt)
        {
            var userId = Client.Get<UserController>().GetUserId();
            
            if (canCheckStoreBonusGuideCheckTriggered || canShowNewAvatarGuide)
            {
                if (PopupStack.GetPopupCount() == 0 && !PopupStack.PopUpInLoading)
                {
                    if (Client.Get<IapController>().IsStoreBonusCollectable() && canCheckStoreBonusGuideCheckTriggered)
                    {
                        var lastTimeStamp = Convert.ToInt64(Client.Storage.GetItem($"LastShowStoreBonusGuide|{userId}",
                            "0"));

                        var currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                        //coolingTime = 24*3 *60 * 60 = 72 * 3600;
                        if (currentTimeStamp - lastTimeStamp > 259200000)
                        {
                            canCheckStoreBonusGuideCheckTriggered = false;

                            Client.Storage.SetItem($"LastShowStoreBonusGuide|{userId}",
                                currentTimeStamp.ToString());
                        }
                        else
                        {
                            CheckAndShowNewAvatarGuide();
                        }
                    }
                    else
                    {
                        CheckAndShowNewAvatarGuide();
                    }
                }
            }
        }
        
        public void CheckAndShowNewAvatarGuide()
        {
            if(canShowNewAvatarGuide)
            {
                if (Client.Get<UserController>().HasNewAvatar())
                {
                    EventBus.Dispatch(new EventShowNewAvatarGuide());
                    Client.Storage.SetItem("NewAvatarGuideShowed","True");
                    canShowNewAvatarGuide = false;
                }
            }
        }

        protected void OnUpdateLobbyBannerIconContent(EventUpdateLobbyBannerIconContent evt)
        {
            var advertisement = Client.Get<BannerController>().GetLobbyFixedAdvertisement();
            view.fixedBannerPageView.viewController.SetUpContent(advertisement);
            _bannerIconListView.viewController.UpdateLobbyBannerIconContent();
        }

        public void SetUpContentList()
        {
            _bannerIconListView = view.AddChild<BannerIconListView>(view.contentListView);
            _bannerIconListView.viewController.SetUpListViewContent();

            _topPanel = view.AddChild<TopPanel>(view.topPanel);
            view.AddChild<DailyMissionLobbyEntranceView>(view.missionButton);
            view.AddChild<SeasonPassLobbyEntranceView>(view.passButton);
            view.AddChild<TimeBonusLobbyEntranceView>(view.timerBonusGroup);
            view.AddChild<UIInboxEntranceView>(view.inboxButton);
            view.AddChild<QuestLobbyEntranceView>(view.questButton);
            view.AddChild<SeasonQuestLobbyEntranceView>(view.seasonQuestButton);
            view.AddChild<UIVIPEntranceView>(view.btnVIP.transform);
            view.AddChild<AdTaskLobbyEntranceView>(view.uiADTaskButton.transform);
            view.AddChild<AlbumLobbyEntranceView>(view.collectionButton.transform);
 
            view.dailyBonusButton.onClick.AddListener(OnDailyBonusButtonClicked);
            // view.challengeButton.onClick.AddListener(OnChallengeButtonClicked);
            // view.collectionButton.onClick.AddListener(OnCollectionButtonClicked);

            var content = view.dailyBonusButton.transform.Find("Content");
            var pointerEventCustomHandler = view.dailyBonusButton.gameObject.AddComponent<PointerEventCustomHandler>();
           
            pointerEventCustomHandler.BindingPointerDown((eventData) =>
            {
                content.localScale = Vector3.one * 0.95f;
            });
            
            pointerEventCustomHandler.BindingPointerUp((eventData) =>
            {
                content.localScale = Vector3.one;
            });
        }
 
        private void OnActivityOpen(EventActivityOpen obj)
        {
            EventBus.Dispatch(new EventChangeLobbyEntranceInActivity(view));
        }

        private async void OnDailyBonusButtonClicked()
        {
            SoundController.PlayButtonClick();
            await PopupStack.ShowPopup<DailyBonusCalendarPopup>(null, true);
        }

        public void OnChallengeButtonClicked()
        {
            //SoundController.PlayButtonClick();
        }
        
        public void OnCollectionButtonClicked()
        {
           // SoundController.PlayButtonClick();
        }
        
        public void PrepareAsyncAssets(Action finishCallback)
        {
            EventBus.Dispatch(new EventOnLobbyCreated(view), finishCallback);
        }
    }
}