// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/17/13:58
// Ver : 1.0.0
// Description : SeasonQuestPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace GameModule
{

    public class SeasonQuestMachineNodeView : QuestMachineNodeView
    {
        public override void UpdateAnimationState()
        {
            if (index < activeQuestIndex)
            {
                animator.Play("MachineFinishState");
                iconAnimator.Play("MachineFinishState");
            }
            
            else if (index == activeQuestIndex && !Client.Get<SeasonQuestController>().NeedChooseDifficultyLevel())
            {
                animator.Play("MachineUnlockState");
            }
            else
            {
                iconAnimator.Play("MachineLockState");
                animator.Play("MachineLockState");
            }
        }
    }
      
    [AssetAddress("UIQuestSeasonOneMain")]
    public class SeasonQuestPopup : Popup<SeasonQuestPopupController>
    {
        [ComponentBinder("Root/ScrollView/Viewport/Content")]
        public Transform questContent;

        [ComponentBinder("Root/ScrollView")] public ScrollRect contentScrollView;

        [ComponentBinder("ScrollViewCloud")] public ScrollRect contentBgCloud;

        [ComponentBinder("ScrollViewBG")] public ScrollRect contentBg;

        [ComponentBinder("ScrollViewFG")] public ScrollRect contentFg;

        [ComponentBinder("LeaderboardButton")] public Button leaderBoardButton;

        [ComponentBinder("Root/TopGroup")]
        public Transform topGroup;
        
        [ComponentBinder("Root/TopGroup/TitleGroup/IntegralGroup/IntegralText")]
        public Text integralText;

        [ComponentBinder("Root/TopGroup/TimerGroup/TimerText")]
        public TMP_Text timerText;
        
        [ComponentBinder("Root/TopGroup/LeaderboardButton/RankingText")]
        public TMP_Text rankText;
        
        [ComponentBinder("Root/TopGroup/LeaderboardButton/StateGroup/RiseIcon")]
        public Transform riseIcon;
        
        [ComponentBinder("Root/TopGroup/LeaderboardButton/StateGroup/DeclineIcon")]
        public Transform declineIcon;
        
        [ComponentBinder("Root/TopGroup/TitleGroup/PhaseText")]
        public Text phaseText;
 
        [ComponentBinder("SeasonQuestBoxGroup_FLy")]
        public Transform rewardFlyFx;

        [ComponentBinder("SpeedUpButton")] public Button speedButton;

        [ComponentBinder("Root/TopGroup/SpeedUpButton/TimerGroup/TimerText")]
        public TMP_Text speedUpTimeText; 
        
        [ComponentBinder("Root/TopGroup/SpeedUpButton/TimerGroup")]
        public Transform speedUpTimerGroup;

        public List<SeasonQuestRewardNodeView> questRewardNode;
        public List<SeasonQuestMachineNodeView> questMachineNode;
        public List<View> questViewList;
        public List<Transform> phaseNodeList;
        public List<RectTransform> phaseBlockNodeList;

        public float maxScrollableValue = 1;

        public bool needForceLandscapeScreen = true;
 
        public static SceneType sourceSceneType;
        public static string machineSceneGameId;
        
        public static string machineSceneAssetId;
        public static bool questFromSceneAbort = false;
        public override bool NeedForceLandscapeScreen()
        {
            return needForceLandscapeScreen;
        }

        public SeasonQuestPopup(string address)
            : base(address)
        {
            if (sourceSceneType == SceneType.TYPE_INVALID || ViewManager.Instance.InLobbyScene())
            {
                UpdateBackSource();
            }

            questFromSceneAbort = false;
        }

        public void UpdateBackSource()
        {
            if (ViewManager.Instance.InLobbyScene())
            {
                sourceSceneType = SceneType.TYPE_LOBBY;
            }
            else
            {
                sourceSceneType = SceneType.TYPE_MACHINE;
                var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

                machineSceneGameId = machineScene.viewController.GetMachineContext().assetProvider.MachineId;
                machineSceneAssetId = machineScene.viewController.GetMachineContext().assetProvider.AssetsId;
            }
        }

        public void ScrollToPosition(float scrollPosition, bool animation = false)
        {
            var viewportWidth = contentScrollView.viewport.rect.size.x;
            var contentWidth = contentScrollView.content.sizeDelta.x;

            var normalizedScrollPosition = Mathf.Min(maxScrollableValue, Math.Max(0, (scrollPosition - viewportWidth * 0.5f) / (contentWidth - viewportWidth)));

            if (animation)
            {
                var startScrollPos = contentScrollView.horizontalNormalizedPosition;

                var _turnPageTurn = DOTween.To(() => startScrollPos,
                    (p) => { contentScrollView.horizontalNormalizedPosition = p; }, normalizedScrollPosition, 1f);
            }
            
            contentScrollView.horizontalNormalizedPosition = normalizedScrollPosition;
        }

        protected override void SetUpExtraView()
        {
            SetUpQuestUIContent();
           
            base.SetUpExtraView();
            
            if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
            {
                var scale = (float)ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                topGroup.localScale = new Vector3(scale, scale, scale);
            }
            
            speedUpTimerGroup.gameObject.SetActive(false);

            contentScrollView.onValueChanged.AddListener(OnContentScrollValueChange);
        }

        void OnContentScrollValueChange(Vector2 position)
        {
            if (position.x > maxScrollableValue)
            {
                contentScrollView.horizontalNormalizedPosition = maxScrollableValue;
            }

            contentFg.normalizedPosition = contentScrollView.normalizedPosition;
            contentBg.normalizedPosition = contentScrollView.normalizedPosition;
            contentBgCloud.normalizedPosition = contentScrollView.normalizedPosition;
        }


        public void UpdateMaxScrollableValue(int phaseId)
        {
            if (phaseId >= phaseBlockNodeList.Count)
            {
                maxScrollableValue = 1;
            }
            else
            {
                var currentBlockNode = phaseBlockNodeList[phaseId];
                var anchoredPositionX = currentBlockNode.anchoredPosition.x;

                var viewportWidth = contentScrollView.viewport.rect.size.x;
                var contentWidth = contentScrollView.content.sizeDelta.x;

                maxScrollableValue = Mathf.Min(1, (anchoredPositionX - viewportWidth) / (contentWidth - viewportWidth));

                if (contentScrollView.normalizedPosition.x > maxScrollableValue)
                {
                    contentScrollView.horizontalNormalizedPosition = maxScrollableValue;
                }
            }
        }

        protected void SetUpQuestUIContent()
        {
            var childCount = questContent.childCount;
            questRewardNode = new List<SeasonQuestRewardNodeView>();
            questMachineNode = new List<SeasonQuestMachineNodeView>();
            questViewList = new List<View>();
            phaseNodeList = new List<Transform>();
            phaseBlockNodeList = new List<RectTransform>();

            for (var i = 0; i < childCount; i++)
            {
                var child = (RectTransform) questContent.GetChild(i);

                if (child.name.Contains("Machine"))
                {
                    var machineNodeView = AddChild<SeasonQuestMachineNodeView>(child);
                    questMachineNode.Add(machineNodeView);
                    questViewList.Add(machineNodeView);
                }
                else if (child.name.Contains("Reward"))
                {
                    var rewardNodeView = AddChild<SeasonQuestRewardNodeView>(child);
                    questRewardNode.Add(rewardNodeView);
                    questViewList.Add(rewardNodeView);
                }
                else if (child.name.Contains("PhaseCell"))
                {
                    phaseNodeList.Add(child);
                }
                else if (child.name.Contains("PhaseBlock"))
                {
                    phaseBlockNodeList.Add(child);
                }
            }
        }

        public void UpdatePhaseBlockUIState(int phaseId)
        {
            for (var i = 0; i < phaseBlockNodeList.Count; i++)
            {
                phaseBlockNodeList[i].Find("BG").gameObject.SetActive(phaseId <= i);
            }
        }

        public override void Close()
        {
            for (var i = 0; i < questRewardNode.Count; i++)
            {
                if (questRewardNode[i].spineNode != null)
                    questRewardNode[i].spineNode.gameObject.SetActive(false);
            }

            base.Close();
            SoundController.RecoverLastMusic();
        }
        
        public static bool OutQuest()
        {
            bool needCloseView = true;

            if (sourceSceneType == SceneType.TYPE_LOBBY)
            {
                if (questFromSceneAbort || !ViewManager.Instance.InLobbyScene())
                {
                    EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOBBY));
                    needCloseView = false;
                }
            }
            else if (sourceSceneType == SceneType.TYPE_MACHINE)
            {
                if (questFromSceneAbort)
                {
                    Client.Get<MachineLogicController>()
                        .EnterGame(machineSceneGameId, machineSceneAssetId, "QuestBack",false);
                    needCloseView = false;
                }
                else if (ViewManager.Instance.InMachineScene())
                {
                    var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
                    var currentGameId = machineScene.viewController.GetMachineContext().assetProvider.MachineId;

                    if (machineSceneGameId != currentGameId)
                    {
                        Client.Get<MachineLogicController>()
                            .EnterGame(machineSceneGameId, machineSceneAssetId, "QuestBack",false);
                        needCloseView = false;
                    }
                }
            }

            sourceSceneType = SceneType.TYPE_INVALID;
            return needCloseView;
        }
    }


    public class SeasonQuestPopupController : ViewController<SeasonQuestPopup>
    {
        private SeasonQuestController _seasonQuestController;

        private SSeasonQuestLeaderboard _seasonQuestLeaderboard;

        private bool _isFetchRankInfo = false;

        public override void OnViewDidLoad()
        {
            _seasonQuestController = Client.Get<SeasonQuestController>();

            var popupArgs = extraData as PopupArgs;

            if (popupArgs != null && !string.IsNullOrEmpty(popupArgs.source))
            {
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonEnter, ("source", popupArgs.source));

                if (popupArgs.source == "QuestWidget")
                {
                    view.UpdateBackSource();
                }

                if (popupArgs.source == "HomeBack")
                {
                    AdLogicManager.Instance.specialOrder = false;
                    if (AdController.Instance.ShouldShowInterstitial(eAdInterstitial.HomeBackToSeasonQuest))
                    {
                        AdController.Instance.TryShowInterstitial(eAdInterstitial.HomeBackToSeasonQuest);
                    }
                }
            }

            base.OnViewDidLoad();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            view.closeButton.onClick.RemoveAllListeners();
            view.closeButton.onClick.AddListener(OnCloseClicked);

            for (var i = 0; i < view.questMachineNode.Count; i++)
            {
                view.questMachineNode[i].spinStateButton.onClick.AddListener(OnSpinButtonClicked);
            }

            for (var i = 0; i < view.questRewardNode.Count; i++)
            {
                view.questRewardNode[i].viewController.BindCollectAction(OnRewardCollectButtonClicked);
            }

            view.leaderBoardButton.onClick.AddListener(OnLeaderBoardClicked);
            view.speedButton.onClick.AddListener(OnSpeedUpButtonClicked);

            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenSceneSwitchEnd.SeasonQuestPopup);
            SubscribeEvent<EventSceneSwitchBackToQuest>(OnSwitchBackToQuest);

            SubscribeEvent<EventSeasonQuestDifficultyChose>(OnDifficultyChose);
            SubscribeEvent<EventBuffDataUpdated>(OnEventBuffDataUpdated);
        }
        
        protected void OnSwitchBackToQuest(EventSceneSwitchBackToQuest evt)
        {
            SoundController.PlayBgMusic("QuestBgMusic");
            
            ViewManager.Instance.UpdateViewToLandscape(true);
                
            view.closeButton.interactable = true;
            view.needForceLandscapeScreen = true;
            SeasonQuestPopup.questFromSceneAbort = true;
        }


        protected void OnSceneSwitchEnd(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.isBackToLastScene)
            {
                SoundController.PlayBgMusic("QuestBgMusic");
                ViewManager.Instance.UpdateViewToLandscape(true);
                
                view.closeButton.interactable = true;
                view.needForceLandscapeScreen = true;
                SeasonQuestPopup.questFromSceneAbort = true;
            }
            else
            {
                view.Close();
            }
        }
        
        public void OnEventBuffDataUpdated(EventBuffDataUpdated evt)
        {
            EnableBoostBuff();
        }

        public void EnableBoostBuff()
        {
            var boosBuff = Client.Get<BuffController>().GetBuff<SeasonQuestStarBoostBuff>();
            if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
            {
                view.speedUpTimerGroup.gameObject.SetActive(true);
                view.speedUpTimeText.text = XUtility.GetTimeText(boosBuff.GetBuffLeftTimeInSecond());
            }
        }

        public void OnSpeedUpButtonClicked()
        {
            PopupStack.ShowPopupNoWait<SeasonQuestSpeedUpPopup>(argument:"SeasonQuestLobby");
        }

        public void OnLeaderBoardClicked()
        {
            if(!_isFetchRankInfo)
                PopupStack.ShowPopupNoWait<SeasonQuestRankPopup>(argument: _seasonQuestLeaderboard);
        }

        public async void OnDifficultyChose(EventSeasonQuestDifficultyChose evt)
        {
            view.topGroup.gameObject.SetActive(true);
            SetUpTitleReward();

            InitQuestNodeState();
         
            var index = (int) _seasonQuestController.GetCurrentQuestIndex();

            await XUtility.PlayAnimationAsync(view.questMachineNode[(index) / 2].animator,
                "MachineUnlocking", this);

            view.questMachineNode[(index) / 2].iconAnimator.Play("MachineUnlockState");
            view.questMachineNode[(index) / 2].DisableSpinButton(false);
            
            view.closeButton.interactable = true;
        }

        public async void CheckAutoAndCollectRewardNode()
        {
            view.closeButton.interactable = false;

            var index = (int) _seasonQuestController.GetCurrentQuestIndex();
            var totalIndex = (int) _seasonQuestController.GetQuestTotalCount();

            var currentQuest = _seasonQuestController.GetCurrentQuest();

            if (currentQuest.Collectable && string.IsNullOrEmpty(currentQuest.AssetId))
            {
                var questRewardNode = view.questRewardNode[index / 2];

                bool isPhaseIndex = _seasonQuestController.IsPhaseIndex(index);

                if (isPhaseIndex)
                {
                    await ClaimPhaseFinalNodeReward(questRewardNode, currentQuest);
                }
                else
                {
                    await ClaimNormalNodeReward(questRewardNode, currentQuest);
                }

                FetchRankInfo();

                if (index < totalIndex - 1)
                    EventBus.Dispatch(new EventSeasonQuestClaimSuccess());

                if (!isPhaseIndex)
                {
                    ScrollViewToCurrentQuestNode(true);

                    await XUtility.PlayAnimationAsync(view.questMachineNode[(index + 1) / 2].animator,
                        "MachineUnlocking");

                    view.questMachineNode[(index + 1) / 2].iconAnimator.Play("MachineUnlockState");
                    view.questMachineNode[(index + 1) / 2].DisableSpinButton(false);

                    view.closeButton.interactable = true;
                }
                else
                {
                    UnlockNewPhase(index);
                }
            }
        }

        public void OnRewardCollectButtonClicked(SeasonQuestRewardNodeView questRewardNode)
        {
            view.closeButton.interactable = false;

            questRewardNode.collectButton.interactable = false;
            questRewardNode.collectStateButton.interactable = false;

            SoundController.PlayButtonClick();

            var currentQuest = _seasonQuestController.GetCurrentQuest();

            var index = (int) _seasonQuestController.GetCurrentQuestIndex();
            var totalIndex = (int) _seasonQuestController.GetQuestTotalCount();

            if (currentQuest.Collectable && string.IsNullOrEmpty(currentQuest.AssetId))
            {
                bool isPhaseIndex = _seasonQuestController.IsPhaseIndex(index);

                _seasonQuestController.ClaimCurrentQuest(async (success) =>
                {
                    if (success)
                    {
                        //SentBiCollectEvent(index / 2);
                        if (isPhaseIndex)
                        {
                            await ClaimPhaseFinalNodeReward(questRewardNode, currentQuest);
                        }
                        else
                        {
                            await ClaimNormalNodeReward(questRewardNode, currentQuest);
                        }

                        _seasonQuestLeaderboard = await _seasonQuestController.RefreshLeaderBoard();
                        if (index < totalIndex - 1)
                            EventBus.Dispatch(new EventSeasonQuestClaimSuccess());

                        if (!isPhaseIndex)
                        {
                            ScrollViewToCurrentQuestNode(true);

                            await XUtility.PlayAnimationAsync(view.questMachineNode[(index + 1) / 2].animator,
                                "MachineUnlocking");

                            view.questMachineNode[(index + 1) / 2].iconAnimator.Play("MachineUnlockState");
                            view.questMachineNode[(index + 1) / 2].DisableSpinButton(false);

                            view.closeButton.interactable = true;
                        }
                        else
                        {
                            UnlockNewPhase(index);
                        }
                    }
                });
            }
        }
        
        public void UnlockNewPhase(int index)
        {
            if (_seasonQuestController.GetCurrentPhrase() == 0)
            {
                InitQuestNodeState();
            }

            ShowDifficultyChoose();

            view.UpdateMaxScrollableValue(_seasonQuestController.GetCurrentPhrase());
            view.UpdatePhaseBlockUIState(_seasonQuestController.GetCurrentPhrase());

            ScrollViewToCurrentQuestNode(true);
        }

        public async Task ClaimNormalNodeReward(SeasonQuestRewardNodeView questRewardNode, PhrasedQuest currentQuest)
        {
            var rewardNode = (questRewardNode);

            rewardNode?.HideRewardNode(true);

            var normalNode = await View.CreateView<SeasonQuestNormalNodeRewardView>();

            normalNode.InitRewardContent(currentQuest.Reward);

            normalNode.transform.SetParent(view.transform, false);

            var currencyCoinView = await view.AddCurrencyCoinView();

            var task = new TaskCompletionSource<bool>();

            normalNode.ShowRewardCollect(currencyCoinView, async () =>
            {
                view.RemoveChild(currencyCoinView);

                normalNode.Destroy();

                await XUtility.PlayAnimationAsync(questRewardNode.animator, "QuestRewardCollecting");
                task.TrySetResult(true);
            });

            await task.Task;
        }

        public async Task ClaimPhaseFinalNodeReward(SeasonQuestRewardNodeView questRewardNode, PhrasedQuest currentQuest)
        {
            await XUtility.PlayAnimationAsync(questRewardNode.animator, "QuestRewardCollecting");

            var giftGroup = questRewardNode.transform.Find("GiftGroup");

            if (giftGroup != null)
            {
                giftGroup.gameObject.SetActive(false);
            }

            var normalNode = await View.CreateView<SeasonQuestPhaseNodeRewardView>();

            normalNode.InitRewardContent(currentQuest.Reward);

            normalNode.transform.SetParent(view.transform, false);

            var currencyCoinView = await view.AddCurrencyCoinView();

            var task = new TaskCompletionSource<bool>();

            normalNode.ShowRewardCollect(currencyCoinView, async () =>
            {
                view.RemoveChild(currencyCoinView);

                normalNode.Destroy();

                giftGroup.gameObject.SetActive(true);

                await XUtility.PlayAnimationAsync(questRewardNode.animator, "QuestRewardCollecting");
                task.TrySetResult(true);
            });

            await task.Task;
        }

        protected void OnSpinButtonClicked()
        {
            view.needForceLandscapeScreen = false;
            view.closeButton.interactable = false;

            SoundController.PlaySfx("General_OpenWindow");

            var currentQuest = _seasonQuestController.GetCurrentQuest();

            var countDown = _seasonQuestController.GetQuestCountDown();
            var machineId = currentQuest.GameId;
            var seasonId = _seasonQuestController.GetSeasonId();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonTap,
                ("machine", machineId),("seasonId",seasonId.ToString()),("countDown",countDown.ToString()));

            Client.Get<MachineLogicController>().EnterGame(currentQuest.GameId, currentQuest.AssetId, "Quest");
        }

        public void OnCloseClicked()
        {
            SoundController.PlayButtonClose();

            if (SeasonQuestPopup.OutQuest())
            {
                view.Close();
            }
        }

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>();

            var totalCount = _seasonQuestController.GetQuestTotalCount();

            var currentQuestIndex = (int) _seasonQuestController.GetCurrentQuestIndex();

            var startIndex = Math.Max(0, currentQuestIndex - 4);
            var endIndex = Math.Min(currentQuestIndex + 4, totalCount - 1);

            for (var i = startIndex; i <= endIndex; i++)
            {
                var quest = _seasonQuestController.GetQuest(i);

                if (!string.IsNullOrEmpty(quest.AssetId))
                {
                    extraAssetNeedToLoad.Add($"SlotIcon{quest.AssetId}Group");
                }
            }
            FetchRankInfo();
            
            await base.LoadExtraAsyncAssets();
        }

        public void SetUpTitleReward()
        {
            if (!_seasonQuestController.NeedChooseDifficultyLevel())
            {
                var phrasedQuest = _seasonQuestController.GetPhasedRewardQuest();

                if (phrasedQuest != null && phrasedQuest.Reward.Items.Count > 0)
                {
                    var coinItem = XItemUtility.GetItem(phrasedQuest.Reward.Items, Item.Types.Type.Coin);

                    view.integralText.SetText(coinItem.Coin.Amount.GetCommaFormat());
                }

                view.phaseText.text = (_seasonQuestController.GetCurrentPhrase() + 1).ToString();
                view.timerText.text = XUtility.GetTimeText(_seasonQuestController.GetQuestCountDown());

                view.UpdatePhaseBlockUIState(_seasonQuestController.GetCurrentPhrase());
 
                UpdateRankInfo();
            }
        }

        public void UpdateRankInfo()
        {
            view.leaderBoardButton.gameObject.SetActive(_seasonQuestLeaderboard != null && _seasonQuestController.GetQuestStarCount() > 0);

            if (_seasonQuestLeaderboard == null)
            {
                return;
            }    
            
            var myRank = _seasonQuestController.GetMyRank();

            var changeAmount = _seasonQuestController.GetRankChangeAmount();

            if (myRank <= 0)
            {
                view.rankText.text = "Not\nRanked";

                view.declineIcon.gameObject.SetActive(false);
                view.riseIcon.gameObject.SetActive(false);
            }
            else
            {
                view.rankText.text = myRank.ToString();

                if (changeAmount > 0)
                {
                    view.declineIcon.gameObject.SetActive(true);
                    view.riseIcon.gameObject.SetActive(false);
                }
                else if (changeAmount < 0)
                {
                    view.declineIcon.gameObject.SetActive(false);
                    view.riseIcon.gameObject.SetActive(true);
                }
                else
                {
                    view.declineIcon.gameObject.SetActive(false);
                    view.riseIcon.gameObject.SetActive(false);
                }
            }
        }

        public override void Update()
        {
            //   XDebug.Log(_newBieQuestController.GetQuestCountDown());
            var countDown = _seasonQuestController.GetQuestCountDown();

            if (countDown <= 0)
            {
                EventBus.Dispatch(new EventSeasonQuestSeasonFinish());

                PopupStack.ShowPopupNoWait<SeasonQuestOverPopup>();
                DisableUpdate();
                return;
            }

            if (view.speedUpTimerGroup.gameObject.activeSelf)
            {
                var boosBuff = Client.Get<BuffController>().GetBuff<SeasonQuestStarBoostBuff>();
                if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
                {
                    view.speedUpTimeText.text = XUtility.GetTimeText(boosBuff.GetBuffLeftTimeInSecond());
                }
                else
                {
                    view.speedUpTimerGroup.gameObject.SetActive(false);
                }
            }

            view.timerText.text = XUtility.GetTimeText(_seasonQuestController.GetQuestCountDown());
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            InitQuestNodeState();

            SetUpTitleReward();

            EnableUpdate(2);

            view.UpdatePhaseBlockUIState(_seasonQuestController.GetCurrentPhrase());

            ScrollViewToCurrentQuestNode();

            WaitNFrame(2,() =>
            {
                if (_seasonQuestController.NeedChooseDifficultyLevel())
                {
                    ShowDifficultyChoose();
                }
                else
                {
                    CheckMachineNodeCollectable();
                }
            });

          

            //  
            EnableBoostBuff();

            SoundController.PlayBgMusic("QuestBgMusic");
        }

        public async void FetchRankInfo()
        {
            _isFetchRankInfo = true;
            _seasonQuestLeaderboard = await _seasonQuestController.RefreshLeaderBoard();
            _isFetchRankInfo = false;
            if(view.transform != null)
                UpdateRankInfo();
        }

        public async void CheckMachineNodeCollectable()
        {
            await WaitForSeconds(1);

            if (view.transform == null)
                return;
            
            var currentQuest = _seasonQuestController.GetCurrentQuest();

            var index = (int) _seasonQuestController.GetCurrentQuestIndex();
            
            if (currentQuest.Collectable && !string.IsNullOrEmpty(currentQuest.AssetId))
            {
                var countDown = _seasonQuestController.GetQuestCountDown();
                var machineId = currentQuest.GameId;
                var seasonId = _seasonQuestController.GetSeasonId();
 
                _seasonQuestController.ClaimCurrentQuest(async (success) =>
                {
                    if (success)
                    {
                        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonComplete,
                            ("questNodeIndex", index.ToString()),
                            ("machine", machineId),
                            ("seasonId",seasonId.ToString()),
                            ("countDown",countDown.ToString()));
                        
                        SoundController.PlaySfx("Wheel_GemsWin");
                        await XUtility.PlayAnimationAsync(view.questMachineNode[index / 2].animator,
                            "MachineFinishing");
                        await XUtility.PlayAnimationAsync(view.questRewardNode[index / 2].animator,
                            "QuestRewardUnlock");

                        if (view.transform != null)
                        {
                            CheckAutoAndCollectRewardNode();
                        }
                    }
                });
            }
            else if (currentQuest.Collectable && string.IsNullOrEmpty(currentQuest.AssetId))
            {
                CheckAutoAndCollectRewardNode();
            }
            else if (!string.IsNullOrEmpty(currentQuest.AssetId))
            {
                view.questMachineNode[index / 2].DisableSpinButton(false);
            }
        }

        public void ShowDifficultyChoose()
        {
            view.topGroup.gameObject.SetActive(false);
            PopupStack.ShowPopupNoWait<SeasonQuestDifficultChoosePopup>();
        }

        protected void ScrollViewToCurrentQuestNode(bool animation = false)
        {
            WaitNFrame(1, () =>
            {
                //  var sizeDelta = ((RectTransform) view.questNodeContent).sizeDelta;
                view.UpdateMaxScrollableValue(_seasonQuestController.GetCurrentPhrase());

                var currentQuestIndex = _seasonQuestController.GetCurrentQuestIndex();

                var rectTransform = (RectTransform) view.questViewList[(int) currentQuestIndex].transform;

                var anchoredPositionX = rectTransform.anchoredPosition.x;

                view.ScrollToPosition(anchoredPositionX, animation);
            });
        }

        public async void InitQuestNodeState()
        {
            var currentIndex = (int) _seasonQuestController.GetCurrentQuestIndex();
            var currentQuest = _seasonQuestController.GetCurrentQuest();
            var totalCount = _seasonQuestController.GetQuestTotalCount();
             
            var startIndex = Math.Max(0, currentIndex - 4);
            var endIndex = Math.Min(currentIndex + 4, totalCount - 1);

            var listInitOrder = new List<int>();

            for (var i = startIndex; i <= endIndex; i++)
            {
                InitQuestNodeState(i, currentIndex, currentQuest);
            }
            
            var extraAssetsList = new List<string>();

            if (startIndex > 0)
            {
                for (var i = 0; i < startIndex; i++)
                {
                    listInitOrder.Add(i);
                }
            }
            
            if (endIndex + 1 < totalCount)
            {
                for (var i = endIndex + 1; i < totalCount; i++)
                {
                    listInitOrder.Add(i);
                }
            }
            
            for (var i = listInitOrder.Count - 1; i >= 0; i--)
            {
                var index = listInitOrder[i];
                var questI = _seasonQuestController.GetQuest(index);
                if (string.IsNullOrEmpty(questI.AssetId))
                {
                    InitQuestNodeState(index, currentIndex, currentQuest);
                    listInitOrder.RemoveAt(i);
                }
                else
                {
                    var assetsName = $"SlotIcon{questI.AssetId}Group";
                    
                    if (GetAssetReference(assetsName) != null)
                    {
                        InitQuestNodeState(index, currentIndex, currentQuest);
                        listInitOrder.RemoveAt(i);
                    }
                    else
                    {
                        extraAssetsList.Add(assetsName);
                    }
                }
            }

            if (listInitOrder.Count > 0)
            {
                await LoadExtraAsyncAssets(extraAssetsList);

                for (var i = listInitOrder.Count - 1; i >= 0; i--)
                {
                    var index = listInitOrder[i];
                    var questI = _seasonQuestController.GetQuest(index);

                    var assetsName = $"SlotIcon{questI.AssetId}Group";

                    if (GetAssetReference(assetsName) != null)
                    {
                        InitQuestNodeState(index, currentIndex, currentQuest);
                        listInitOrder.RemoveAt(i);
                    }
                }
            }
        }

        protected void InitQuestNodeState(int index, int currentIndex,  PhrasedQuest currentQuest)
        {
            var questI = _seasonQuestController.GetQuest(index);
           
            if (view.questViewList[index] is QuestMachineNodeView machineNode)
            {
                machineNode.SetUp(null, index, (int) currentIndex,
                    GetAssetReference($"SlotIcon{questI.AssetId}Group"));
            }
            else
            {
                if (view.questViewList[index] is SeasonQuestRewardNodeView rewardNode)
                {
                    if (index < currentIndex && !_seasonQuestController.IsPhaseIndex(index))
                    {
                        rewardNode.HideRewardNode(true);
                    }
                    else
                    {
                        rewardNode.SetUp(questI, currentQuest, index, (int) currentIndex);
                    }
                }
            }
        }
    }
}