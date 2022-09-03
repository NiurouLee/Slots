using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidMapCellView : View<TreasureRaidMapCellViewController>
    {
        [ComponentBinder("CurrentState/PlayButton")]
        public Button playBtn;
        
        // [ComponentBinder("")]
        // private Animator masterAni;

        // [ComponentBinder("")]
        // private Text levelText;
        [ComponentBinder("CurrentState/PlayButton/CountGroup/CountText")]
        public Text wheelLeftText;

        [ComponentBinder("RewardGroup/IntegralText")]
        private Text rewardCoinText;

        [ComponentBinder("RewardGroup/ExtraReward")]
        private Transform rewardGroup;

        private Animator animator;
        private MonopolyRoundSimpleInfo roundInfo;
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            animator = transform.GetComponent<Animator>();
        }

        public void SetRoundInfo(MonopolyRoundSimpleInfo roundSimpleInfo)
        {
            roundInfo = roundSimpleInfo;
        }

        public void RefreshState()
        {
            var rewardType = "StandardType";

            // 根据roundSimpleInfo 刷新UI
            switch (roundInfo.RoundStatus)
            {
                case MonopolyRoundSimpleInfo.Types.RoundStatus.Finished:
                    animator.Play("CompleteIdle_loop");
                    break;
                case MonopolyRoundSimpleInfo.Types.RoundStatus.Locked:
                    animator.Play("LockIdle");
                    rewardType = "DisableType";
                    break;
                case MonopolyRoundSimpleInfo.Types.RoundStatus.Opening:
                    animator.Play("PlayIdle_loop");
                    break;
                case MonopolyRoundSimpleInfo.Types.RoundStatus.Running:
                    animator.Play("PlayIdle_loop");
                    break;
            }

            RefreshWheelLeftCount();
            var coinItem = XItemUtility.GetCoinItem(roundInfo.CompleteRewards[0]);
            var coins = (long) coinItem.Coin.Amount;
            rewardCoinText.SetText(coins.GetCommaOrSimplify(7));

            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.Coin);
            XItemUtility.InitItemsUI(rewardGroup, roundInfo.CompleteRewards[0].Items,
                rewardGroup.Find("TreasureRaidGameRewardCell"), null, rewardType, skipList);
        }

        public void RefreshWheelLeftCount()
        {
            var parentView = GetParentView() as TreasureRaidMapPopup;
            if (parentView != null)
            {
                wheelLeftText.SetText($"{parentView.viewController.activityTreasureRaid.TicketCount}");
            }
        }

        public void PlayCompleteAni()
        {
            animator.Play("CompleteIdle");
        }

        public void PlayUnlockAni()
        {
            animator.Play("PlayIdle");
        }
        public void PlayLockIdleAni()
        {
            animator.Play("LockIdle");
        }
    }

    public class TreasureRaidMapCellViewController : ViewController<TreasureRaidMapCellView>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityServerDataUpdated>(RefreshUserData);
        }

        private void RefreshUserData(EventActivityServerDataUpdated evt)
        {
            if (evt.activityType != ActivityType.TreasureRaid)
                return;
            
            view.RefreshWheelLeftCount();
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.playBtn.onClick.AddListener(OnPlayBtnClicked);
        }

        private async void OnPlayBtnClicked()
        {
            view.playBtn.interactable = false;
            var parentView = view.GetParentView() as TreasureRaidMapPopup;
            if (parentView == null)
            {
                XDebug.LogError("parentView is null!!!!! check your code!!!!!");
                view.playBtn.interactable = true;
                return;
            }
            await PopupStack.ShowPopup<TreasureRaidMainPopup>();
            parentView.SetNeedForceLandscapeScreen(false);
            parentView.Close();
        }
    }

    [AssetAddress("UITreasureRaidLevelChoose")]
    public class TreasureRaidMapPopup : Popup<TreasureRaidMapPopupController>
    {
        [ComponentBinder("Root/MainGroup/AdaptGroup")]
        private Transform adaptGroup;

        [ComponentBinder("Root/MainGroup/AdaptGroup/TopGroup/TitleGroup/RewardGroup/IntegralText")]
        private Text rewardCoins;
        
        [ComponentBinder("Root/MainGroup/AdaptGroup/TopGroup/TitleGroup/RoundGroup/RoundText")]
        public Text roundText;

        [ComponentBinder("Root/MainGroup/AdaptGroup/Root_main/ScrollView/Viewport/Content")]
        private Transform levelContainer;

        [ComponentBinder("Root/MainGroup/AdaptGroup/TopGroup/TitleGroup/RewardGroup/ExtraReward")]
        private Transform _rewardGroup;

        [ComponentBinder("Root/MainGroup/AdaptGroup/TopGroup/TitleGroup/DailyTaskGroup")]
        private Transform dailyTaskGroup;
        
        [ComponentBinder("Root/MainGroup/AdaptGroup/TopGroup/TitleGroup/PuzzleGroup")]
        private Transform puzzleGroup;

        [ComponentBinder("Root/MainGroup/AdaptGroup/TopGroup/TitleGroup/RankGroup")]
        private Transform rankGroup;

        [ComponentBinder("Root/MainGroup/AdaptGroup/TopGroup")]
        private CanvasGroup topCanvasGroup;

        [ComponentBinder("Root/MainGroup/AdaptGroup/Root_main/ScrollView/Viewport/Content")]
        public RectTransform content;

        [ComponentBinder("Root/MainGroup/AdaptGroup/MFGGroup/TimerGroup/TimerText")]
        public TextMeshProUGUI leftTimeText;

        [ComponentBinder("Root/MainGroup/CloseButton")]
        public Button closeBtn;

        [ComponentBinder("Root/MainGroup/InformationButton")]
        public Button informationBtn;
        
        private bool _needForceLandscapeScreen = true;
        
        public List<TreasureRaidMapCellView> cellViews;

        public TreasureRaidDailyTaskView _dailyTaskView;
        public TreasureRaidRankView _rankView;
        public TreasureRaidPuzzleView _puzzleView;

        public TreasureRaidMapPopup(string address) : base(address)
        {
            
        }

        public override bool NeedForceLandscapeScreen()
        {
            return _needForceLandscapeScreen;
        }

        /// <summary>
        /// 点击进入关卡设置不转屏false，点击关闭按钮设置转屏true
        /// </summary>
        /// <param name="isForce"></param>
        public void SetNeedForceLandscapeScreen(bool isForce)
        {
            _needForceLandscapeScreen = isForce;
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdaptScaleTransform(adaptGroup, new Vector2(1400, 768));
            closeBtn.onClick.AddListener(OnCloseClicked);
            informationBtn.onClick.AddListener(OnInformationBtnClicked);
        }

        private void OnInformationBtnClicked()
        {
            PopupStack.ShowPopupNoWait<TreasureRaidHelpPopup>();
        }

        public override void AdaptScaleTransform(Transform transformToScale, Vector2 preferSize)
        {
            var viewSize = ViewResolution.referenceResolutionLandscape;
            if (viewSize.x < preferSize.x)
            {
                var scale = viewSize.x / preferSize.x;
                transformToScale.localScale =  new Vector3(scale, scale, scale);
            }
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            cellViews = new List<TreasureRaidMapCellView>();

            for (int i = 0; i < levelContainer.childCount; i++)
            {
                var cell = levelContainer.GetChild(i);
                var roundSimpleInfo = viewController.roundListInfo.RoundList[i];
                if (roundSimpleInfo != null)
                {
                    var cellView = AddChild<TreasureRaidMapCellView>(cell);
                    cellView.SetRoundInfo(roundSimpleInfo);
                    cellViews.Add(cellView);
                }
            }
            _dailyTaskView = AddChild<TreasureRaidDailyTaskView>(dailyTaskGroup);
            _rankView = AddChild<TreasureRaidRankView>(rankGroup);
            _puzzleView = AddChild<TreasureRaidPuzzleView>(puzzleGroup);

        }

        public override void OnOpen()
        {
            base.OnOpen();
            _puzzleView.RefreshPuzzle(false);
            _rankView.RefreshRank(viewController.roundListInfo.SelfRankInfo.MyRank);
        }

        protected override void OnCloseClicked()
        {
            if (viewController.activityTreasureRaid != null)
            {
                viewController.activityTreasureRaid.InActivity = false;
            }
            SetNeedForceLandscapeScreen(true);
            SoundController.RecoverLastMusic();
            base.OnCloseClicked();
        }

        public void SetAllBtnState(bool interactable)
        {
            foreach (var cell in cellViews)
            {
                cell.playBtn.interactable = interactable;
            }
            closeBtn.interactable = interactable;
            informationBtn.interactable = interactable;
            topCanvasGroup.interactable = interactable;
        }

        /// <summary>
        /// 刷新reward and 关卡状态
        /// </summary>
        public void RefreshReward()
        {
            var rewards = viewController.roundListInfo.CompleteRoundListRewards;
            var coinItem = XItemUtility.GetItem(rewards[0].Items, Item.Types.Type.Coin);
            var coins = (long) coinItem.Coin.Amount;
            rewardCoins.SetText(coins.GetCommaOrSimplify(7));
            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.Coin);
            XItemUtility.InitItemsUI(_rewardGroup, rewards[0].Items, _rewardGroup.Find("TreasureRaidGameRewardCell"),
                null, "StandardType", skipList);
        }
    }

    public class TreasureRaidMapPopupController : ViewController<TreasureRaidMapPopup>
    {
        public Activity_TreasureRaid activityTreasureRaid;
        public SGetMonopolyRoundListInfo roundListInfo;

        private TreasureRaidData treasureRaidData;

        private TreasureRaidGuideStepView guideStep1View;
        private TreasureRaidGuideStepView guideStep2View;

        public override void OnViewDidLoad()
        {
            if (activityTreasureRaid != null)
            {
                activityTreasureRaid.InActivity = true;
            }
            base.OnViewDidLoad();
            view.RefreshReward();
        }

        public string ToOrdinal(long number)
        {
            if (number < 0) return number.ToString();
            long rem = number % 100;
            if (rem >= 11 && rem <= 13) return number + "TH";

            switch (number % 10)
            {
                case 1:
                    return number + "ST";
                case 2:
                    return number + "ND";
                case 3:
                    return number + "RD";
                default:
                    return number + "TH";
            }
        }

        public override async void OnViewEnabled()
        {
            // if (SoundController.GetPlayingBgMusicName() != "TreasureRaidBGM")
            // {
            //     SoundController.PlayBgMusic("TreasureRaidBGM");
            // }
            base.OnViewEnabled();
            view.roundText.SetText(ToOrdinal(roundListInfo.RoundNum));

            EnableUpdate(1);
            foreach (var cellView in view.cellViews)
            {
                cellView.RefreshState();
            }
            UpdateLeftTime();

            if (treasureRaidData != null)
            {
                view.SetAllBtnState(false);
                PlayUnlockStateAni(treasureRaidData.roundInfo, treasureRaidData.roundListCompleteRewards, () =>
                {
                    view.SetAllBtnState(true);
                    // 查看任务是否完成
                    view._dailyTaskView.RefreshDailyTask(true, () =>
                    {
                        activityTreasureRaid.SetActivityState(false);
                    }, treasureRaidData.dailyTask);
                });
            }

            if (activityTreasureRaid != null)
            {
                var roundId = activityTreasureRaid.GetCurrentRoundID();
                if (roundId == 6 || roundId == 0)
                {
                    var level = roundListInfo.RoundList[0];
                    if (level.IsCurrentRound && treasureRaidData == null)
                    {
                        roundId = 1;
                    }
                }

                await WaitNFrame(1);
                if (roundId <= 2 && roundId > 0)
                    view.content.anchoredPosition = Vector2.zero;
                else if (roundId == 3)
                    view.content.anchoredPosition = new Vector2(0, view.content.sizeDelta.y / 4);
                else
                    view.content.anchoredPosition = new Vector2(0, view.content.sizeDelta.y / 2);

                if (activityTreasureRaid.BeginnersGuideStep == 0 && roundId == 1)
                {
                    ShowGuideStep1();
                }
                else if (activityTreasureRaid.BeginnersGuideStep == 1 && roundId == 1)
                {
                    ShowGuideStep2();
                }
            }
        }

        public override void Update()
        {
            base.Update();
            UpdateLeftTime();
        }

        private void UpdateLeftTime()
        {
            if (activityTreasureRaid != null)
            {
                view.leftTimeText.SetText(XUtility.GetTimeText(activityTreasureRaid.GetCountDown()));
            }
        }

        private async void PlayUnlockStateAni(MonopolyRoundInfo roundInfo, RepeatedField<Reward> roundListCompleteRewards, Action callback)
        {
            var level = roundInfo.SimpleInfo.RoundId > 6
                ? roundInfo.SimpleInfo.RoundId % 6
                : roundInfo.SimpleInfo.RoundId;
            level = level == 0 ? 6 : level;
            // TODO 播放 level 完成动画
            view.cellViews[(int) level - 1].PlayCompleteAni();
            var nextLevel = level + 1;
            bool toNextRound = false;
            if (nextLevel > view.cellViews.Count)
            {
                toNextRound = true;
                nextLevel = 1;
            }
            await XUtility.WaitSeconds(0.5f);
            if (roundListCompleteRewards != null && roundListCompleteRewards.Count > 0)
            {
                // 展示最后的大奖
                var collectView = await PopupStack.ShowPopup<TreasureRaidCollectFinalRewardPopup>();
                collectView.InitRewardContent(roundListCompleteRewards);
                collectView.ShowRewardCollect(async () =>
                {
                    // 这中间有可能活动到期，到期了transform为空，就不执行后面的操作了
                    if (view.transform == null)
                        return;
                    if (toNextRound)
                    {
                        // content回到对应位置
                        view.content.DOLocalMoveY(0, 0.3f);
                        await WaitForSeconds(0.3f);
                        view.cellViews[(int) level - 1].PlayLockIdleAni();
                        view.roundText.SetText(ToOrdinal(roundInfo.RoundNum + 1));
                    }
                    // 播放 level + 1 解锁动画
                    view.cellViews[(int) nextLevel - 1].PlayUnlockAni();
                    callback?.Invoke();
                });
            }
            else
            {
                // 播放 level + 1 解锁动画
                view.cellViews[(int) nextLevel - 1].PlayUnlockAni();
                callback?.Invoke();
                // EventBus.Dispatch(new EventRefreshUserProfile());
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
            SubscribeEvent<EventActivityExpire>(OnCloseGuideStepView);
        }

        private void OnCloseGuideStepView(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.TreasureRaid)
                return;
            if (guideStep1View != null && guideStep1View.transform != null)
            {
                guideStep1View.Destroy();
            }
            if (guideStep2View != null && guideStep2View.transform != null)
            {
                guideStep2View.Destroy();
            }
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.SetNeedForceLandscapeScreen(true);
            SoundController.RecoverLastMusic();
            view.Close();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;

            if (inExtraData != null)
            {
                if (inExtraData is TreasureRaidData data)
                {
                    treasureRaidData = data;
                }
                if (inExtraData is PopupArgs args)
                {
                    if (args.extraArgs != null)
                    {
                        var openBGM = (bool) args.extraArgs;
                        if (openBGM)
                        {
                            SoundController.PlayBgMusic("TreasureRaidBGM");
                        }
                    }
                }
            }
            roundListInfo = inExtraAsyncData as SGetMonopolyRoundListInfo;
        }
        
        public async void ShowGuideStep1()
        {
            guideStep1View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide1", view.transform);
            view.AdaptScaleTransform(guideStep1View.transform.Find("Root"), new Vector2(1400, 768));
            SoundController.PlaySfx("TreasureRaid_Bubble");
            guideStep1View.SetGuideClickHandler(ToGuideStep2);
        }

        private async void ToGuideStep2()
        {
            ViewManager.Instance.BlockingUserClick(true, "UITreasureRaidGuide1");
            activityTreasureRaid.IncreaseGuideStep(null);
            ViewManager.Instance.BlockingUserClick(false, "UITreasureRaidGuide1");
            ShowGuideStep2();
        }

        private async void ShowGuideStep2()
        {
            view.content.anchoredPosition = new Vector2(0, view.content.sizeDelta.y / 2);
            guideStep2View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide2", view.transform);
            view.AdaptScaleTransform(guideStep2View.transform.Find("Root"), new Vector2(1400, 768));
            if (view.transform == null)
                return;
            await WaitForSeconds(0.5f);
            if (view.transform == null)
                return;
            view.content.DOLocalMoveY(0f, 1f);
            await WaitForSeconds(1f);
            if (view.transform == null)
                return;
            var canvas = view.cellViews[0].transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");

            var canvasGroup = view.cellViews[0].transform.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            var handle = guideStep2View.transform.Find("Root");
            var handleCanvas = handle.gameObject.AddComponent<Canvas>();
            handleCanvas.overrideSorting = true;
            handleCanvas.sortingOrder = 11;
            handleCanvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            var components = new List<Component>();
            components.Add(canvas);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            guideStep2View.SetGuideClickHandler( async () =>
            {
                ViewManager.Instance.BlockingUserClick(true, "UITreasureRaidGuide2");
                activityTreasureRaid?.IncreaseGuideStep(null);
                await PopupStack.ShowPopup<TreasureRaidMainPopup>();
                if (view != null)
                {
                    view.SetNeedForceLandscapeScreen(false);
                    view.Close();
                }

                ViewManager.Instance.BlockingUserClick(false, "UITreasureRaidGuide2");
            }, components);
        }
    }
}