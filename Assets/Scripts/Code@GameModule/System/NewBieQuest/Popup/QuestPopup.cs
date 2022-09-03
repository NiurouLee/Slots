// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/16:58
// Ver : 1.0.0
// Description : QuestPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class QuestMachineNodeView : View
    {
        [ComponentBinder("SlotGroup/SpinStateButton")]
        public Button spinStateButton;

        [ComponentBinder("SlotGroup/SlotIconPoint")]
        public Transform iconAttachPoint;

        public Animator animator;
        public Animator iconAnimator;

        protected Quest quest;
        protected int index;
        protected int activeQuestIndex;

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            animator = transform.GetComponent<Animator>();
        }

        public void SetUp(Quest inQuest, int inIndex, int inActiveQuestIndex, AssetReference iconAssetReference)
        {
            quest = inQuest;
            index = inIndex;
            activeQuestIndex = inActiveQuestIndex;

            var machineIcon = iconAssetReference.InstantiateAsset<GameObject>();
            iconAnimator = machineIcon.GetComponentInChildren<Animator>();

            var childCount = iconAttachPoint.childCount;
            if (childCount > 0)
            {
                for (var i = childCount - 1; i >= 0; i--)
                {
                    var child = iconAttachPoint.GetChild(i);
                    GameObject.Destroy(child.gameObject);
                }
            }

            machineIcon.transform.SetParent(iconAttachPoint, false);


            UpdateAnimationState();

            DisableSpinButton(true);
        }

        public void DisableSpinButton(bool disable)
        {
            spinStateButton.interactable = !disable;
        }

        public virtual void UpdateAnimationState()
        {
            if (index < activeQuestIndex)
            {
                animator.Play("MachineFinishState");
                iconAnimator.Play("MachineFinishState");
            }

            else if (index == activeQuestIndex)
            {
                // if (quest.Collectable)
                // {
                //     iconAnimator.Play("MachineFinishing");
                //     animator.Play("MachineFinishing");
                // }
                // else
                // {
                //  spinStateButton.in
                animator.Play("MachineUnlockState");
                //}
            }
            else
            {
                iconAnimator.Play("MachineLockState");
                animator.Play("MachineLockState");
            }
        }
    }

    public class QuestRewardNodeView : View<QuestRewardNodeViewController>
    {
        [ComponentBinder("IntegralText")] public TextMeshProUGUI integralText;

        [ComponentBinder("CollectStateButton")]
        public Button collectStateButton;

        [ComponentBinder("GiftGroup/CollectStateButton/Icon2")]
        public Button collectButton;

        [ComponentBinder("CloseStateButton")] public Button closeStateButton;

        [ComponentBinder("BubbleGroup")] public Transform bubbleGroup;

        [ComponentBinder("SkeletonGraphic (gold)")]
        public Transform spineNode;


        public Animator animator;

        public Quest quest;
        public Quest activeQuest;
        public int index;
        public int activeQuestIndex;

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            animator = transform.GetComponent<Animator>();
        }

        public void SetUp(Quest inQuest, Quest inActiveQuest, int inIndex, int inActiveQuestIndex)
        {
            quest = inQuest;
            index = inIndex;
            activeQuest = inActiveQuest;
            activeQuestIndex = inActiveQuestIndex;

            UpdateAnimationState();
        }

        public void UpdateAnimationState()
        {
            if (index < activeQuestIndex)
                animator.Play("QuestRewardCollected");

            else if (index == activeQuestIndex)
            {
                SetUpItemReward();
                animator.Play("QuestRewardCollectable");
            }
            else
            {
                SetUpItemReward();
                if (NeedShowEnterTipReward())
                {
                    viewController.ShowTip();
                    animator.Play("QuestRewardShowTip");
                }
                else
                {
                    animator.Play("QuestRewardLock");
                }
            }
        }

        public void HideRewardNode(bool hide)
        {
            var giftGroup = transform.Find("GiftGroup");
            if (giftGroup)
                giftGroup.gameObject.SetActive(!hide);
        }

        public void SetUpItemReward()
        {
            var rewardRoot = bubbleGroup.Find("Root");

            for (var i = rewardRoot.childCount - 1; i >= 0; i--)
            {
                var child = rewardRoot.GetChild(i);
                if (child.name.Contains("CommonCell") && child.name != "CommonCell")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            XItemUtility.InitItemsUI(rewardRoot, quest.Reward.Items);
        }

        public virtual bool NeedShowEnterTipReward()
        {
            return index == activeQuestIndex + 1 && !activeQuest.Collectable;
        }


        public virtual bool CanShouldShowTip()
        {
            if ((activeQuest.Collectable
                 || activeQuest.Collected) && activeQuestIndex == index - 1)
            {
                return false;
            }

            return true;
        }
    }

    public class QuestRewardNodeViewController : ViewController<QuestRewardNodeView>
    {
        private Action<QuestRewardNodeView> _rewardCollectHandler;

        private bool _isTipShowing = false;
        private bool inSwitching = false;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.closeStateButton.onClick.AddListener(OnCloseStateButtonClicked);
        }
        
        protected void OnCloseStateButtonClicked()
        {
            if (!_isTipShowing)
            {
                SoundController.PlaySfx("General_DropdownWindow");
                ShowTip();
            }
            else
            {
                HideTip();
            }
        }

        public void HideTip()
        {
            inSwitching = true;
            var animator = view.bubbleGroup.GetComponent<Animator>();

            XUtility.PlayAnimation(animator, "Close", () =>
            {
                _isTipShowing = false;
                inSwitching = false;
            });
        }

        private CancelableCallback _hideTipCallback = null;

        public void ShowTip()
        {
            if (!view.CanShouldShowTip())
            {
                return;
            }

            if (!_isTipShowing)
            {
                inSwitching = true;
                var animator = view.bubbleGroup.GetComponent<Animator>();
                animator.Play("Open", -1, 0);

                WaitForSeconds(0.5f, () =>
                {
                    _isTipShowing = true;
                    //  inSwitching = false;
                });
            }
        }
    }

    [AssetAddress("UIQuestMain")]
    public class QuestPopup : Popup<QuestPopupViewController>
    {
        [ComponentBinder("HelpButton")] public Button helpButton;
        [ComponentBinder("ToSlotButton")] public Button toSlotButton;

        [ComponentBinder("Root/ScrollView/Viewport/Content")]
        public Transform questNodeContent;

        [ComponentBinder("BoxGroup_FLy")] public Transform rewardFlyFx;

        [ComponentBinder("TopGroup")] public Transform topGroup;

        [ComponentBinder("Root/ScrollView")] public ScrollRect scrollRect;

        public List<QuestRewardNodeView> questRewardNode;
        public List<QuestMachineNodeView> questMachineNode;
        public List<View> questViewList;

        [ComponentBinder("Root/TopGroup/TimerGroup/TimerText")]
        public TextMeshProUGUI timerText;

        [ComponentBinder("Root/TopGroup/TitleGroup/IntegralGroup/IntegralText")]
        public TextMeshProUGUI finalPrize;

        [ComponentBinder("SpeedUpButton")] public Button speedButton;

        [ComponentBinder("Root/TopGroup/SpeedUpButton/TimerGroup/TimerText")]
        public TMP_Text speedUpTimeText;

        [ComponentBinder("Root/TopGroup/SpeedUpButton/TimerGroup")]
        public Transform speedUpTimerGroup;


        public static SceneType sourceSceneType;
        public static string machineSceneGameId;
        public static string machineSceneAssetId;
        public static bool questFromSceneAbort = false;

        public bool needForceLandscapeScreen = true;

        public QuestPopup(string address)
            : base(address)
        {
            
            questFromSceneAbort = false;
            
            if (sourceSceneType == SceneType.TYPE_INVALID || ViewManager.Instance.InLobbyScene())
            {
                UpdateBackSource();
            }
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
                machineSceneGameId = machineScene.viewController.GetMachineContext().assetProvider.AssetsId;
            }
        }

        public override bool NeedForceLandscapeScreen()
        {
            return needForceLandscapeScreen;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            var nodeCount = Client.Get<NewBieQuestController>().GetQuestTotalCount() / 2;

            questMachineNode = new List<QuestMachineNodeView>();
            questRewardNode = new List<QuestRewardNodeView>();
            questViewList = new List<View>();

            for (var i = 1; i <= nodeCount; i++)
            {
                var machineNode = questNodeContent.Find($"MachineNode{i}");
                if (machineNode)
                {
                    var machineNodeView = AddChild<QuestMachineNodeView>(machineNode);
                    questMachineNode.Add(machineNodeView);
                    questViewList.Add(machineNodeView);
                }

                var rewardNode = questNodeContent.Find($"RewardNode{i}");

                if (rewardNode)
                {
                    var rewardNodeView = AddChild<QuestRewardNodeView>(rewardNode);
                    questRewardNode.Add(rewardNodeView);
                    questViewList.Add(rewardNodeView);
                }
            }

            if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
            {
                var scale = (float) ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                topGroup.localScale = new Vector3(scale, scale, scale);
            }
        }

        public override string GetCloseAnimationName()
        {
            return null;
        }
        
        public static bool OutQuest()
        {
            bool needCloseView = true;

            if (sourceSceneType == SceneType.TYPE_LOBBY)
            {
                if (questFromSceneAbort || !ViewManager.Instance.InLobbyScene())
                {
                    EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOBBY,false));
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
                else
                {
                    if (ViewManager.Instance.InMachineScene())
                    {
                        var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();
                        var currentGameId = machineScene.viewController.GetMachineContext().assetProvider.MachineId;

                        if (machineSceneGameId != currentGameId)
                        {
                            Client.Get<MachineLogicController>()
                                .EnterGame(machineSceneGameId, machineSceneAssetId, "QuestBack", false);
                            needCloseView = false;
                        }
                    }
                }
            }

            sourceSceneType = SceneType.TYPE_INVALID;
            return needCloseView;
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

        public void ScrollToPosition(float scrollPosition, bool animation = false)
        {
            var viewportWidth = scrollRect.viewport.rect.size.x;
            var contentWidth = scrollRect.content.sizeDelta.x;

            var normalizedScrollPosition = Mathf.Min(1,
                Math.Max(0, (scrollPosition - viewportWidth * 0.5f) / (contentWidth - viewportWidth)));

            if (animation)
            {
                var startScrollPos = scrollRect.horizontalNormalizedPosition;

                var _turnPageTurn = DOTween.To(() => startScrollPos,
                    (p) => { scrollRect.horizontalNormalizedPosition = p; }, normalizedScrollPosition, 1f);
            }

            scrollRect.horizontalNormalizedPosition = normalizedScrollPosition;
        }
    }

    public class QuestPopupViewController : ViewController<QuestPopup>
    {
        private NewBieQuestController _newBieQuestController;

        public override void OnViewDidLoad()
        {
            _newBieQuestController = Client.Get<NewBieQuestController>();

            var popupArgs = extraData as PopupArgs;

            if (popupArgs != null && !string.IsNullOrEmpty(popupArgs.source))
            {
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventQuestEnter, ("source", popupArgs.source));

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

            if (QuestPopup.sourceSceneType == SceneType.TYPE_LOBBY)
            {
                view.closeButton.onClick.RemoveAllListeners();
                view.closeButton.onClick.AddListener(OnCloseClicked);
                view.toSlotButton.gameObject.SetActive(false);
            }

            if (QuestPopup.sourceSceneType == SceneType.TYPE_MACHINE)
            {
                view.closeButton.gameObject.SetActive(false);
                view.toSlotButton.onClick.RemoveAllListeners();
                view.toSlotButton.onClick.AddListener(OnCloseClicked);
            }

            for (var i = 0; i < view.questMachineNode.Count; i++)
            {
                view.questMachineNode[i].spinStateButton.onClick.AddListener(OnSpinButtonClicked);
            }
  
            view.speedButton.onClick.AddListener(OnSpeedUpButtonClicked);

            view.helpButton.onClick.AddListener(OnHelpButtonClicked);

            SubscribeEvent<EventSceneSwitchBackToQuest>(OnSwitchBackToQuest);
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd,
                HandlerPriorityWhenSceneSwitchEnd.QuestPopup);

            SubscribeEvent<EventBuffDataUpdated>(OnEventBuffDataUpdated);
        }

        protected void OnSwitchBackToQuest(EventSceneSwitchBackToQuest evt)
        {
            SoundController.PlayBgMusic("QuestBgMusic");
            
            ViewManager.Instance.UpdateViewToLandscape(true);
                
            view.closeButton.interactable = true;
            view.needForceLandscapeScreen = true;
            view.toSlotButton.interactable = true;
            QuestPopup.questFromSceneAbort = true;
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
                view.toSlotButton.interactable = true;
                QuestPopup.questFromSceneAbort = true;
            }
            else
            {
                view.Close();
            }
        }
        public void EnableBoostBuff()
        {
            var boosBuff = Client.Get<BuffController>().GetBuff<NewbieQuestBoostBuff>();
            if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
            {
                view.speedUpTimerGroup.gameObject.SetActive(true);
                view.speedUpTimeText.text = XUtility.GetTimeText(boosBuff.GetBuffLeftTimeInSecond());
            }
        }

        public void OnEventBuffDataUpdated(EventBuffDataUpdated evt)
        {
            EnableBoostBuff();
        }

        public void OnSpeedUpButtonClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(QuestPayPopup),null, "UserClick")));
        }

        public void OnHelpButtonClicked()
        {
            SoundController.PlayButtonClick();
            PopupStack.ShowPopupNoWait<QuestFaqPopUp>("UIQuestHelp");
        }

        protected void OnSpinButtonClicked()
        {
            view.needForceLandscapeScreen = false;
            view.toSlotButton.interactable = false;

            SoundController.PlaySfx("General_OpenWindow");

            var currentQuest = Client.Get<NewBieQuestController>().GetCurrentQuest();
            var currentIndex = Client.Get<NewBieQuestController>().GetCurrentQuestIndex();

            //  await XUtility.PlayAnimationAsync(view.questMachineNode[(int)currentIndex/2].animator,"MachineSpin");

            SentBiTapEvent((int) currentIndex / 2);

            Client.Get<MachineLogicController>().EnterGame(currentQuest.GameId, currentQuest.AssetId, "Quest");
        }

        public void OnCloseClicked()
        {
            SoundController.PlayButtonClose();
            if (QuestPopup.OutQuest())
            {
                view.Close();
            }
        }

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>();

            var totalCount = _newBieQuestController.GetQuestTotalCount();
            for (var i = 0; i < totalCount; i++)
            {
                var quest = _newBieQuestController.GetQuest(i);

                if (!string.IsNullOrEmpty(quest.AssetId))
                {
                    extraAssetNeedToLoad.Add($"SlotIcon{quest.AssetId}Group");
                }
            }

            string iconAddress = null;
            if (QuestPopup.sourceSceneType == SceneType.TYPE_MACHINE)
            {
                iconAddress = $"Icon{QuestPopup.machineSceneGameId}";
                extraAssetNeedToLoad.Add(iconAddress);
            }

            await base.LoadExtraAsyncAssets();

            if (iconAddress != null)
            {
                view.toSlotButton.transform.Find("SlotIcon");
                var assetReference = GetAssetReference(iconAddress);
                var icon = assetReference.InstantiateAsset<GameObject>();
                icon.transform.SetParent(view.toSlotButton.transform.Find("SlotIcon"), false);
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            InitQuestNodeState();
            InitTitleUIInfo();
            EnableUpdate(2);
            Update();

            ScrollViewToCurrentQuestNode();

            CheckMachineNodeCollectable();

            SoundController.PlayBgMusic("QuestBgMusic");
        }

        // protected void ScrollViewToCurrentQuestNode()
        // {
        //     WaitForSeconds(0.1f, () =>
        //     {
        //         var sizeDelta = ((RectTransform) view.questNodeContent).sizeDelta;
        //         var currentQuestIndex = _newBieQuestController.GetCurrentQuestIndex();
        //
        //         var currentViewNode = view.questViewList[(int) currentQuestIndex];
        //         var nodePosition = view.questViewList[(int) currentQuestIndex].transform.localPosition;
        //
        //         if (currentViewNode is QuestMachineNodeView viewNode)
        //         {
        //             var worldPosition = viewNode.spinStateButton.transform.position;
        //             nodePosition = view.questNodeContent.transform.InverseTransformPoint(worldPosition);
        //         }
        //         else if (currentViewNode is QuestRewardNodeView rewardNode)
        //         {
        //             var worldPosition = rewardNode.collectButton.transform.position;
        //             nodePosition = view.questNodeContent.transform.InverseTransformPoint(worldPosition);
        //         }
        //
        //         var currentX = nodePosition.x;
        //
        //         var viewSize = view.scrollRect.viewport.rect.size.x;
        //         var percentage = (currentX - viewSize * 0.5f) / (sizeDelta.x - viewSize);
        //
        //         view.scrollRect.horizontalNormalizedPosition = percentage;
        //     });
        // }

        protected void ScrollViewToCurrentQuestNode(bool animation = false)
        {
            WaitNFrame(1, () =>
            {
                //  var sizeDelta = ((RectTransform) view.questNodeContent).sizeDelta;

                var currentQuestIndex = _newBieQuestController.GetCurrentQuestIndex();

                var rectTransform = (RectTransform) view.questViewList[(int) currentQuestIndex].transform;

                var anchoredPositionX = rectTransform.anchoredPosition.x;

                view.ScrollToPosition(anchoredPositionX, animation);
            });
        }


        public async void CheckMachineNodeCollectable()
        {
            await WaitForSeconds(1);

            var currentQuest = _newBieQuestController.GetCurrentQuest();

            var index = (int) _newBieQuestController.GetCurrentQuestIndex();

            if (currentQuest.Collectable && !string.IsNullOrEmpty(currentQuest.AssetId))
            {
                _newBieQuestController.ClaimCurrentQuest(async (success) =>
                {
                    if (success)
                    {
                        SentBiCompleteEvent(index / 2);
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

                // if (!IsFirstOpen() && !PopupStack.HasPopup(typeof(QuestPayPopup)))
                //     EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(QuestPayPopup),null, "AutoPopup")));
                // else
                // {
                //     Client.Storage.SetItem("FirstOpenNewBieQuest", "False");
                // }
            }
        }

        public async void CheckAutoAndCollectRewardNode()
        {
            view.closeButton.interactable = false;
            view.toSlotButton.interactable = false;

            var index = (int) _newBieQuestController.GetCurrentQuestIndex();
            var totalIndex = (int) _newBieQuestController.GetQuestTotalCount();

            var currentQuest = _newBieQuestController.GetCurrentQuest();

            if (currentQuest.Collectable && string.IsNullOrEmpty(currentQuest.AssetId))
            {
                var questRewardNode = view.questRewardNode[index / 2];

                SentBiCollectEvent(index / 2);

                if (index == totalIndex - 1)
                {
                    await ClaimFinalNodeReward(questRewardNode, currentQuest);
                }
                else
                {
                    await ClaimNormalNodeReward(questRewardNode, currentQuest);

                    ScrollViewToCurrentQuestNode(true);
                }

                if (index < totalIndex - 1)
                {
                    await XUtility.PlayAnimationAsync(view.questMachineNode[(index + 1) / 2].animator,
                        "MachineUnlocking");

                    view.questMachineNode[(index + 1) / 2].iconAnimator.Play("MachineUnlockState");
                    view.questMachineNode[(index + 1) / 2].DisableSpinButton(false);

                    view.helpButton.interactable = true;
                    view.toSlotButton.interactable = true;
                    view.closeButton.interactable = true;

                    if (index < totalIndex - 1)
                        EventBus.Dispatch(new EventQuestClaimSuccess());
                }
            }
        }

        protected bool IsFirstOpen()
        {
            return Client.Storage.GetItem("FirstOpenNewBieQuest", "True") == "True";
        }

        protected void SentBiTapEvent(int taskIndex)
        {
            var eventType =
                (BiEventFortuneX.Types.GameEventType) Enum.Parse(typeof(BiEventFortuneX.Types.GameEventType),
                    "GameEventQuestTap" + (taskIndex + 1));
            BiManagerGameModule.Instance.SendGameEvent(eventType);
        }

        protected void SentBiCompleteEvent(int taskIndex)
        {
            var eventType = (BiEventFortuneX.Types.GameEventType) Enum.Parse(
                typeof(BiEventFortuneX.Types.GameEventType), "GameEventQuestComplete" + (taskIndex + 1));
            BiManagerGameModule.Instance.SendGameEvent(eventType,
                ("countDown", _newBieQuestController.GetQuestCountDown().ToString()));
        }

        protected void SentBiCollectEvent(int taskIndex)
        {
            var eventType = (BiEventFortuneX.Types.GameEventType) Enum.Parse(
                typeof(BiEventFortuneX.Types.GameEventType), "GameEventQuestCollect" + (taskIndex + 1));
            BiManagerGameModule.Instance.SendGameEvent(eventType);
        }

        // public void OnRewardCollectButtonClicked(QuestRewardNodeView questRewardNode)
        // {
        //     view.helpButton.interactable = false;
        //     view.toSlotButton.interactable = false;
        //
        //     questRewardNode.collectButton.interactable = false;
        //     questRewardNode.collectStateButton.interactable = false;
        //
        //     SoundController.PlayButtonClick();
        //
        //     var currentQuest = _newBieQuestController.GetCurrentQuest();
        //
        //     var index = (int) _newBieQuestController.GetCurrentQuestIndex();
        //     var totalIndex = (int) _newBieQuestController.GetQuestTotalCount();
        //
        //     if (currentQuest.Collectable && string.IsNullOrEmpty(currentQuest.AssetId))
        //     {
        //         _newBieQuestController.ClaimCurrentQuest(async (success) =>
        //         {
        //             if (success)
        //             {
        //                 SentBiCollectEvent(index / 2);
        //                 if (index == totalIndex - 1)
        //                 {
        //                     await ClaimFinalNodeReward(questRewardNode, currentQuest);
        //                 }
        //                 else
        //                 {
        //                     await ClaimNormalNodeReward(questRewardNode, currentQuest);
        //                 }
        //
        //                 if (index < totalIndex - 1)
        //                 {
        //                     await XUtility.PlayAnimationAsync(view.questMachineNode[(index + 1) / 2].animator,
        //                         "MachineUnlocking");
        //
        //                     view.questMachineNode[(index + 1) / 2].iconAnimator.Play("MachineUnlockState");
        //                     view.questMachineNode[(index + 1) / 2].DisableSpinButton(false);
        //
        //                     view.helpButton.interactable = true;
        //                     view.toSlotButton.interactable = true;
        //
        //                     if (index < totalIndex - 1)
        //                         EventBus.Dispatch(new EventQuestClaimSuccess());
        //                 }
        //             }
        //         });
        //     }
        // }

        public async Task ClaimNormalNodeReward(QuestRewardNodeView questRewardNode, Quest currentQuest)
        {
            var rewardNode = (questRewardNode);

            rewardNode?.HideRewardNode(true);

            var normalNode = await View.CreateView<QuestNormalNodeRewardView>();

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

            // var giftGroup = questRewardNode.transform.Find("GiftGroup");
            // if (giftGroup != null)
            // {
            //     giftGroup.gameObject.SetActive(false);
            // }
            //
            // var coinItem = XItemUtility.GetItem(currentQuest.Reward.Items, Item.Types.Type.Coin);
            //
            // //     var rewardStartFly = questRewardNode.transform.Find("GiftGroup/CollectStateButton/BoxGroup").position;
            // //     view.rewardFlyFx.position = rewardStartFly;
            //
            // var rewardText = view.rewardFlyFx.Find("IntegralGroup/IntegralText").GetComponent<TextMeshProUGUI>();
            // rewardText.text = coinItem.Coin.Amount.GetCommaFormat();
            //
            // var animator = view.rewardFlyFx.GetComponent<Animator>();
            // view.rewardFlyFx.gameObject.SetActive(true);
            // animator.Play("Open", 0, 0);
            //
            // var currencyCoinView = await view.AddCurrencyCoinView();
            // await XUtility.FlyCoins(view.rewardFlyFx, new EventBalanceUpdate(coinItem, "QuestReward"),
            //     currencyCoinView);
            // view.RemoveChild(currencyCoinView);
            //
            // await XUtility.PlayAnimationAsync(animator, "Close");
            //
            // view.rewardFlyFx.gameObject.SetActive(false);
            //
            // giftGroup.gameObject.SetActive(true);
            //
            // await XUtility.PlayAnimationAsync(questRewardNode.animator, "QuestRewardCollecting");
        }

        public async Task ClaimFinalNodeReward(QuestRewardNodeView questRewardNode, Quest currentQuest)
        {
            await XUtility.PlayAnimationAsync(questRewardNode.animator, "QuestRewardCollecting");

            var popup = await PopupStack.ShowPopup<QuestFinishPopup>();
            popup.SubscribeCloseAction(async () =>
            {
                var commonSoonQuest = await PopupStack.ShowPopup<QuestComingSoonPopup>();

                commonSoonQuest.SubscribeCloseAction(OnCloseClicked);
            });
        }

        public void InitTitleUIInfo()
        {
            view.timerText.text = XUtility.GetTimeText(_newBieQuestController.GetQuestCountDown());
            view.finalPrize.text = _newBieQuestController.GetFinalPrize().GetCommaFormat();
        }

        public override void Update()
        {
            //   XDebug.Log(_newBieQuestController.GetQuestCountDown());
            var countDown = _newBieQuestController.GetQuestCountDown();
            if (countDown <= 0)
            {
                EventBus.Dispatch(new EventQuestTimeOut());

                PopupStack.ShowPopupNoWait<QuestOverPopup>();
                DisableUpdate();
                return;
            }

            if (view.speedUpTimerGroup.gameObject.activeSelf)
            {
                var boosBuff = Client.Get<BuffController>().GetBuff<NewbieQuestBoostBuff>();
                if (boosBuff != null && boosBuff.GetBuffLeftTimeInSecond() > 0)
                {
                    view.speedUpTimeText.text = XUtility.GetTimeText(boosBuff.GetBuffLeftTimeInSecond());
                }
                else
                {
                    view.speedUpTimerGroup.gameObject.SetActive(false);
                }
            }

            view.timerText.text = XUtility.GetTimeText(_newBieQuestController.GetQuestCountDown(), true);
        }

        public void InitQuestNodeState()
        {
            var currentIndex = _newBieQuestController.GetCurrentQuestIndex();
            var currentQuest = _newBieQuestController.GetCurrentQuest();

            for (var i = 0; i < _newBieQuestController.GetQuestTotalCount(); i++)
            {
                var questI = _newBieQuestController.GetQuest(i);
                if (view.questViewList[i] is QuestMachineNodeView machineNode)
                {
                    machineNode.SetUp(questI, i, (int) currentIndex,
                        GetAssetReference($"SlotIcon{questI.AssetId}Group"));
                }
                else
                {
                    if (view.questViewList[i] is QuestRewardNodeView rewardNode)
                    {
                        if (i < currentIndex)
                        {
                            rewardNode.HideRewardNode(true);
                        }
                        else
                        {
                            rewardNode.SetUp(questI, currentQuest, i, (int) currentIndex);
                        }
                    }
                }
            }
        }
    }
}