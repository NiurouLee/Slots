// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 9:25 PM
// Ver : 1.0.0
// Description : ControlPanel.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameModule
{
    public class ControlPanel : TransformHolder
    {
        private HoldClickButton spinButton;
        private Button stopButton, autoSpinStopButton, maxBetButton, addBetButton, minusBetButton, spinProgressButton;
        private Button niceWinButton;
        private Button guideSpinButton;

        private TextMeshProUGUI totalBetText;
        private TextMeshProUGUI freeBetText;
        private TextMeshProUGUI winText;
        private TextMeshProUGUI largeWinText;
        private TextMeshProUGUI freeSpinCountTxt;

        private GameObject freeAverageBet;
        private GameObject normalAverageBet;
        private GameObject totalBetImg;
        private GameObject freeTotalBetImg;

        private Transform normalExtraBet;

        private Transform freeExtraBet;

        private GameObject freeBetObj;
        private GameObject normalBetObj;

        private bool isBetLocked = false;

        private bool isMaxBet = false;
        private bool isExtraBet = false;
        private bool isMinBet = false;
        private ulong totalBetNum = 0;
        private bool betInitialized = false;


        private long winCoinNum = 0;

        //
        private AutoSpinChoosePanel autoSpinChoosePanel;
        private TextMeshProUGUI autoSpinNumText;
        private GameObject infinityImage;

        // private TipView levelUpUnlockTipView;
        // private MaxBetTipView maxBetTipView;
        //
        // private VegasPassWidget vegasPassWidget;

        private float numAnimationStartTime;
        private float numAnimationDuration;
        private string numAnimationAudioName;
        private string numAnimationStopAudioName;

        private Animator winAnimationAnimator;

        private AudioSource numAnimationAudioSource;

        private View guideTapSpin;
        private View increaseBetTip;
        private bool canDestroyIncreaseBetTip;
        private EventBus.Listener listenerLevelChanged;

        private Animator spinButtonFxAnimator;

        private Animator maxBetFxAnimator;

        private Transform transLackOfBetTipContainer;
        private LackOfBetTipView lackOfBetTipView;

        private Vector3 winTextRefWorldPosition;

        public Vector3 WinTextRefWorldPosition => new Vector3(winTextRefWorldPosition.x, winTextRefWorldPosition.y,
            winTextRefWorldPosition.z);

        private DailyMissionMachineEntranceView _dailyMissionMachineEntranceView;
        private uint TotalFreeCount;

        protected string niceWinOpenAnimation = "NiceWinOpen";
        protected string niceWinLoopAnimation = "NiceWinLoop";
        protected string niceWinCloseAnimation = "NiceWinClose";
        protected string niceWinAudioName = "NiceWin_Common";

        protected Transform _betTipAttachPoint;
        protected AlbumBetTipView albumBetTipView;

        protected Transform maxBetActivateTip;

        public ControlPanel(Transform transform)
            : base(transform)
        {
        }

        public override void Initialize(MachineContext context)
        {
            base.Initialize(context);
            InitializePanel();
            RegisterButtonEvent();
            InitializeExtraView();
        }

        public void SetNiceWinAnimationToOnlyFx()
        {
            niceWinOpenAnimation += "Fx";
            niceWinLoopAnimation += "Fx";
            niceWinCloseAnimation += "Fx";
            niceWinAudioName = "NiceWin_Classic";
        }

        private void InitializePanel()
        {
            //Init hold click button
            var spinButtonGameObject = transform.Find("Canvas/SpinButton").gameObject;
            var button = spinButtonGameObject.GetComponent<Button>();
            var spriteState = button.spriteState;
            UnityEngine.Object.DestroyImmediate(button);

            spinButton = spinButtonGameObject.AddComponent<HoldClickButton>();
            spinButton.transition = Selectable.Transition.SpriteSwap;
            spinButton.spriteState = spriteState;

            guideSpinButton = transform.Find("Canvas/GuideSpinButton").GetComponent<Button>();
            guideSpinButton.gameObject.SetActive(false);

            // spinButtonFxAnimator = transform.Find("Canvas/SpinButtonFx").GetComponent<Animator>();
            //
            // var handler = spinButton.gameObject.AddComponent<PointerEventCustomHandler>();
            // handler.BindingPointerDown(OnPointerDown);
            // handler.BindingPointerUp(OnPointerUp);

            _betTipAttachPoint = transform.Find("BetTipAttachPoint");

            autoSpinStopButton = transform.Find("AutoStopButton").GetComponent<Button>();
            autoSpinNumText = transform.Find("AutoStopButton/AutoSpinNumText").GetComponent<TextMeshProUGUI>();
            autoSpinStopButton.gameObject.SetActive(false);

            stopButton = transform.Find("StopButton").GetComponent<Button>();
            maxBetButton = transform.Find("MaxBetButton").GetComponent<Button>();
            addBetButton = transform.Find("NormalBet/AddButton").GetComponent<Button>();

            var maxBetBubbleH = transform.Find("MaxBetTipAttachNode/MaxBetBubbleH");
            var maxBetBubbleV = transform.Find("MaxBetTipAttachNode/MaxBetBubbleV");

            if (maxBetBubbleH != null && maxBetBubbleV != null)
            {
                maxBetBubbleH.gameObject.SetActive(false);
                maxBetBubbleV.gameObject.SetActive(false);

                if (transform.gameObject.name.Contains("ControlPanelV"))
                {
                    maxBetActivateTip = maxBetBubbleV;
                }
                else
                {
                    maxBetActivateTip = maxBetBubbleH;
                }
            }

            niceWinButton = transform.Find("WinEffectNiceWin/HotArea").GetComponent<Button>();

            minusBetButton = transform.Find("NormalBet/SubButton").GetComponent<Button>();
            spinProgressButton = transform.Find("SpinProgressButton").GetComponent<Button>();

            freeBetObj = transform.Find("FreeBet").gameObject;
            normalBetObj = transform.Find("NormalBet").gameObject;

            freeExtraBet = freeBetObj.transform.Find("UltraBetImage");
            normalExtraBet = normalBetObj.transform.Find("UltraBetImage");

            normalBetObj.SetActive(true);
            freeBetObj.SetActive(false);

            //Labels
            totalBetText = normalBetObj.transform.Find("BetText").GetComponent<TextMeshProUGUI>();
            freeSpinCountTxt = freeBetObj.transform.Find("FreeCountText").GetComponent<TextMeshProUGUI>();
            freeBetText = freeBetObj.transform.Find("FreeBetText").GetComponent<TextMeshProUGUI>();

            freeAverageBet = freeBetObj.transform.Find("FreeAverageBet").gameObject;
            normalAverageBet = normalBetObj.transform.Find("AverageBet").gameObject;

            totalBetImg = normalBetObj.transform.Find("TotalBetImage").gameObject;
            freeTotalBetImg = freeBetObj.transform.Find("FreeTotalBetImg").gameObject;

            freeAverageBet.SetActive(false);
            normalAverageBet.SetActive(false);

            winText = transform.Find("WinText").GetComponent<TextMeshProUGUI>();
            largeWinText = transform.Find("WinEffectNiceWin/UI_R/WinUINode/WinFrame/WinChipText")
                .GetComponent<TextMeshProUGUI>();

            maxBetFxAnimator = transform.Find("NormalBet/BetBg_fx").GetComponent<Animator>();

            //AutoSpin 
            var autoSpinSelectPanel = transform.Find("AutoSpinSelectPanel");
            autoSpinChoosePanel = new AutoSpinChoosePanel(autoSpinSelectPanel, OnAutoSpinClicked);

            winAnimationAnimator = transform.GetComponent<Animator>();

            GetWinTextRefWorldPosition();

            CheckAndInitializeTapToSpinGuide(false);
            CreateLackOfBetTipView();
            CreateAlbumBetTipView();
            AdaptPanelUI();
        }

        protected void AdaptPanelUI()
        {
            if (!ViewManager.Instance.IsPortrait)
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    var localScale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                    transform.localScale = new Vector3(localScale, localScale, localScale);
                }
            }
            else
            {
#if UNITY_IOS
                ((RectTransform) (transform)).anchoredPosition = new Vector2(0, MachineConstant.controlPanelOffsetY);
#endif
            }
        }

        public async void CheckAndInitializeTapToSpinGuide(bool force)
        {
            if (guideTapSpin != null)
            {
                return;
            }

            if ((!spinButton.interactable|| !spinButton.gameObject.activeSelf) &&
                    (!guideSpinButton.interactable|| !guideSpinButton.gameObject.activeSelf))
            {
                return;
            }
            
            if (Client.Get<GuideController>().CheckNeedShowGuideSpin() || force)
            {
                var canvasNode = transform.Find("Canvas");
                var spinButtonGameObj = canvasNode.Find("SpinButton");
                var canvas = canvasNode.GetComponent<Canvas>();

                guideTapSpin = await View.CreateView<View>("UIGuideSpinArrow", canvasNode);
                guideTapSpin.transform.localPosition = spinButtonGameObj.localPosition;


                canvas.overrideSorting = true;
                canvas.sortingOrder = 10;
                canvas.sortingLayerID = SortingLayer.NameToID("UI");

                if (force)
                {
                    guideTapSpin.transform.SetAsFirstSibling();
                    guideTapSpin.transform.Find("Image").gameObject.SetActive(false);
                }

                if (!force)
                {
                    guideTapSpin.transform.SetAsLastSibling();

                    context.WaitSeconds(2, () => { guideTapSpin.transform.SetAsFirstSibling(); });

                    guideSpinButton.gameObject.SetActive(true);
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                        .GameEventGuidePopSpin1);
                }
            }
        }

        public bool maxBetFxIsShowing = false;

        public async void ShowMaxBetFx()
        {
            if (isMaxBet)
                return;

            var animator = maxBetActivateTip.gameObject.GetComponent<Animator>();

            if (animator)
            {
                maxBetActivateTip.gameObject.SetActive(true);
                    animator.Play("Open");
            }
 
            if (Client.Get<UserController>().GetUserLevel() == 2)
            {
                var tipAddress = ViewManager.Instance.IsPortrait ? "UIGuideBetDialogV" : "UIGuideBetDialog";
                increaseBetTip = await View.CreateView<View>(tipAddress, transform);
                increaseBetTip.transform.SetAsLastSibling();
                increaseBetTip.transform.gameObject.SetActive(true);
            }

            maxBetFxIsShowing = true;

            context.WaitSeconds(6, HideMaxBetFx);
        }

        public void HideMaxBetFx()
        {
            if (maxBetFxIsShowing)
            {
                var animator = maxBetActivateTip.gameObject.GetComponent<Animator>();

                if (animator)
                {
                    XUtility.PlayAnimation(animator, "Close", () =>
                    {
                        maxBetActivateTip.gameObject.SetActive(false);
                    });
                }

                if (increaseBetTip != null && increaseBetTip.transform)
                {
                    increaseBetTip.Destroy();
                    increaseBetTip = null;
                }

                maxBetFxIsShowing = false;
            }
        }

        // private async void OnLevelChanged(EventGuideShowMaxBetTip evt)
        // {
        //     if (Client.Get<UserController>().GetUserLevel() == 2 && increaseBetTip.transform)
        //     {
        //         canDestroyIncreaseBetTip = true;
        //         increaseBetTip.transform.gameObject.SetActive(true);
        //         
        //         ShowMaxBetFx();
        //         
        //         await context.WaitSeconds(3);
        //         DestroyIncreaseBetTip();
        //     }
        // }

        // private void DestroyIncreaseBetTip()
        // {
        //     if (canDestroyIncreaseBetTip && increaseBetTip != null && increaseBetTip.transform)
        //     {
        //         winAnimationAnimator.Play("Idle",0,0);
        //         EventBus.UnSubscribe(listenerLevelChanged);
        //         listenerLevelChanged = null;
        //         increaseBetTip.Destroy();
        //         increaseBetTip = null;
        //     }
        // }

        private void RegisterButtonEvent()
        {
            spinButton.onLongClick.AddListener(OnSpinButtonLongClick);
            spinButton.onClick.AddListener(OnSpinButtonClick);
            stopButton.onClick.AddListener(OnStopButtonClick);
            maxBetButton.onClick.AddListener(OnMaxBetButtonClick);
            addBetButton.onClick.AddListener(OnAddBetButtonClick);
            minusBetButton.onClick.AddListener(OnMinusBetButtonClick);
            autoSpinStopButton.onClick.AddListener(OnAutoSpinStopButtonClick);
            niceWinButton.onClick.AddListener(OnStopButtonClick);
            guideSpinButton.onClick.AddListener(OnSpinButtonClick);
            //  addBetMaxButton.onClick.AddListener(OnAddBetMaxButtonClicked);
        }

        public void OnWheelClicked()
        {
            if (spinButton.gameObject.activeInHierarchy && spinButton.interactable)
            {
                OnSpinButtonClick();
            }
            else if (stopButton.gameObject.activeInHierarchy && stopButton.interactable)
            {
                OnStopButtonClick();
            }
            else if (autoSpinStopButton.gameObject.activeInHierarchy && autoSpinStopButton.interactable)
            {
                OnAutoSpinStopButtonClick();
            }
        }

        private void InitializeExtraView()
        {
            _dailyMissionMachineEntranceView = new DailyMissionMachineEntranceView(spinProgressButton.transform);
        }

        void OnSpinButtonLongClick()
        {
            //context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_SPIN);
            CheckAndCleanTapToSpinGuide();
            HideMaxBetFx();
            autoSpinChoosePanel.CheckAndShowSelectView();
            SoundController.PlayButtonClick();
        }

        void OnAutoSpinClicked(int spinCount)
        {
            HideMaxBetFx();
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN, spinCount);
            SoundController.PlayButtonClick();
        }

        void OnSpinButtonClick()
        {
            HideMaxBetFx();
            autoSpinChoosePanel.HideSelectView();

            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_SPIN);
            // Log.LogUiClickEvent(UiClick.SpinUiSpin);
            CheckAndCleanTapToSpinGuide();
            AudioUtil.Instance.PlayAudioFx("Spin");
        }

        void OnAutoSpinStopButtonClick()
        {
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN_STOP);
            //  Log.LogUiClickEvent(UiClick.SpinUiAutoSpinStop);
        }

        void OnStopButtonClick()
        {
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_STOP);
            // Log.LogUiClickEvent(UiClick.SpinUiStop);
            SoundController.PlayButtonClick();
        }

        void OnMaxBetButtonClick()
        {
            HideMaxBetFx();
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_MAX_BET);
            SoundController.PlayButtonClick();
            // Log.LogUiClickEvent(UiClick.SpinUiMaxBet);
            // EventBus.Dispatch(new EventUserIncreasedBet(true));
            //  CheckAndHideIncreaseBetTip();
            //  SoundManager.PlaySlotClickSound();
        }

        void OnAddBetButtonClick()
        {
            HideMaxBetFx();
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_ADD_BET);
            SoundController.PlayButtonClick();
            //   Log.LogUiClickEvent(UiClick.SpinUiAddBet);
            //   EventBus.Dispatch(new EventUserIncreasedBet(true));
            //  CheckAndHideIncreaseBetTip();
        }

        void OnMinusBetButtonClick()
        {
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CONTROL_MINUS_BET);
            //  Log.LogUiClickEvent(UiClick.SpinUiMinusBet);
            SoundController.PlayButtonClick();
        }

        public void ShowSpinButton(bool interactable)
        {
            if (spinButton)
            {
                spinButton.gameObject.SetActive(true);
                spinButton.interactable = interactable;
                if (!interactable)
                {
                    CheckAndCleanTapToSpinGuide();
                }
            }

            if (stopButton)
            {
                stopButton.gameObject.SetActive(false);
                stopButton.interactable = false;
            }

            var wheels = context.view.GetAll<Wheel>();
            if (wheels != null && wheels.Count > 0)
            {
                for (var i = 0; i < wheels.Count; i++)
                {
                    wheels[i].ToggleInteractableStatues(interactable);
                }
            }

            if (autoSpinStopButton)
            {
                autoSpinStopButton.gameObject.SetActive(false);
            }

            EnableBetChange(interactable);
        }

        public void ShowStopButton(bool interactable)
        {
            if (stopButton)
            {
                stopButton.gameObject.SetActive(true);
                stopButton.interactable = interactable;
            }

            if (autoSpinStopButton)
            {
                autoSpinStopButton.gameObject.SetActive(false);
            }
            
            if (spinButton)
            {
                spinButton.gameObject.SetActive(false);
            }
            
            var wheels = context.view.GetAll<Wheel>();
            if (wheels != null && wheels.Count > 0)
            {
                for (var i = 0; i < wheels.Count; i++)
                {
                    wheels[i].ToggleInteractableStatues(interactable);
                }
            }

            EnableBetChange(false);
        }

        public void ShowAutoStopButton(bool interactable)
        {
            if (autoSpinStopButton)
            {
                autoSpinStopButton.gameObject.SetActive(true);
                autoSpinStopButton.interactable = interactable;
            }

            if (stopButton)
            {
                stopButton.gameObject.SetActive(true);
            }

            if (spinButton)
            {
                spinButton.gameObject.SetActive(false);
            }

            EnableBetChange(false);
        }

        public void EnableBetChange(bool bEnable)
        {
            bEnable = bEnable && !isBetLocked;

            if (maxBetButton)
            {
                maxBetButton.interactable = bEnable && !isMaxBet;
            }

            addBetButton.interactable = bEnable && !isMaxBet;
            minusBetButton.interactable = bEnable && !isMinBet;

            //addBetMaxButton.gameObject.SetActive(isMaxBet);
            // addBetButton.interactable = !isMaxBet;
        }

        public void SetTotalBet(ulong totalBet, bool inIsMaxBet, bool inIsMinBet, bool inIsExtraBet, bool lockBet)
        {
            isMaxBet = inIsMaxBet;
            isMinBet = inIsMinBet;
            isExtraBet = inIsExtraBet;
            isBetLocked = lockBet;

            if (totalBetNum != totalBet)
            {
                totalBetNum = totalBet;

                UpdateTotalBet(lockBet);

                //  Log.LogStateEvent(State.BetChange,new Dictionary<string, object>{{"TotalBet:", totalBet}});

                // eventDispatcher.BroadcastViewEvent(SlotEventType.TYPE_SLOT_BET_CHANGED);
                //eventDispatcher.DispatcherEvent(SlotEventType.TYPE_SLOT_BET_CHANGED);

                if (betInitialized)
                {
                    context.DispatchInternalEvent(MachineInternalEvent.EVENT_BET_CHANGED);
                }

                betInitialized = true;

                if (isMaxBet)
                {
                    HideMaxBetFx();
                    //HandlerMaxBetFxAnimator(true);
                }
            }

            if (!inIsMaxBet)
            {
                //HandlerMaxBetFxAnimator(false);
            }

            EnableBetChange(!lockBet);
        }

        public void UpdateTotalBet(bool isFreeSpin = false)
        {
            if (totalBetText)
                totalBetText.text = totalBetNum.GetCommaFormat();

            if (freeBetText)
                freeBetText.text = totalBetNum.GetCommaFormat();

            if (maxBetButton)
            {
                maxBetButton.interactable = !isMaxBet && !isBetLocked;

                if (isFreeSpin)
                {
                    maxBetButton.interactable = false;
                }
            }
        }

        public void UpdateWinLabelChips(long winChips)
        {
            winCoinNum = winChips;
            DOTween.Kill(winText);
            winText.color = Color.white;

            if (winCoinNum == 0)
            {
                winText.text = "";
                largeWinText.text = "";
            }
            else
            {
                winText.text = winCoinNum.GetCommaFormat();
                largeWinText.text = winCoinNum.GetCommaFormat();
            }
        }

        private List<Tween> pausedTween;

        public void OnContextPause()
        {
            winAnimationAnimator.speed = 0;
            var activeTween = DOTween.TweensByTarget(largeWinText);
            var activeTween2 = DOTween.TweensByTarget(winText);

            if (pausedTween == null)
                pausedTween = new List<Tween>();

            pausedTween.Clear();

            if (activeTween != null && activeTween.Count > 0)
            {
                for (var i = 0; i < activeTween.Count; i++)
                {
                    activeTween[i].Pause();
                    pausedTween.Add(activeTween[i]);
                }
            }

            if (activeTween2 != null && activeTween2.Count > 0)
            {
                for (var i = 0; i < activeTween2.Count; i++)
                {
                    activeTween2[i].Pause();
                    pausedTween.Add(activeTween2[i]);
                }
            }

            var particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Pause();
            }
        }

        public void OnContextUnPause()
        {
            winAnimationAnimator.speed = 1;

            var particleSystems = transform.GetComponentsInChildren<ParticleSystem>();

            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }

            if (pausedTween != null && pausedTween.Count > 0)
            {
                for (var i = 0; i < pausedTween.Count; i++)
                {
                    pausedTween[i].TogglePause();
                }
            }
        }

        public void UpdateWinLabelChipsWithAnimation(long winChips, float duration, bool winOutAni,
            string audioName = null, string stopAudioName = null, bool audioLoop = false)
        {
            // duration = 5;
            if (winChips > 0 && winCoinNum != winChips)
            {
                DOTween.Kill(winText);
                fadeSequence?.Kill();

                winText.color = Color.white;

                numAnimationAudioSource = null;

                numAnimationAudioName = audioName;
                numAnimationStopAudioName = stopAudioName;

                if (winOutAni)
                {
                    numAnimationAudioName = niceWinAudioName;
                    numAnimationStopAudioName = niceWinAudioName + "Ending";
                   

                    winAnimationAnimator.speed = 1.0f;
                    winAnimationAnimator.Play(niceWinOpenAnimation, 0, 0);
                    context.AddWaitEvent(WaitEvent.WAIT_WIN_NUM_ANIMTION);

                    largeWinText.DOCounter(winCoinNum, winChips, duration, true).OnComplete(() =>
                    {
                        largeWinText.text = winCoinNum.GetCommaFormat();

                        winText.DOKill();

                        winText.text = winCoinNum.GetCommaFormat();
                        XUtility.PlayAnimation(winAnimationAnimator, niceWinCloseAnimation,
                            () => { context.RemoveWaitEvent(WaitEvent.WAIT_WIN_NUM_ANIMTION); },
                            context);

                        if (audioLoop && numAnimationAudioSource != null)
                        {
                            numAnimationAudioSource.Stop();
                            numAnimationAudioSource = null;
                        }

                        if (!string.IsNullOrEmpty(numAnimationStopAudioName))
                            AudioUtil.Instance.PlayAudioFx(numAnimationStopAudioName);

                        AudioUtil.Instance.FadeMusicTo(1, 0.5f, 0.5f);
                    });

                    winText.DOCounter(winCoinNum, winChips, duration, true);

                    //End animation add one more second
                    numAnimationDuration = duration + 1;
                }
                else
                {
                    context.AddWaitEvent(WaitEvent.WAIT_WIN_NUM_ANIMTION);
                    winText.DOCounter(winCoinNum, winChips, duration, true).OnComplete(() =>
                    {
                        context.RemoveWaitEvent(WaitEvent.WAIT_WIN_NUM_ANIMTION);
                        winText.text = winCoinNum.GetCommaFormat();

                        if (audioLoop && numAnimationAudioSource != null)
                        {
                            numAnimationAudioSource.Stop();
                            numAnimationAudioSource = null;
                        }

                        if (!string.IsNullOrEmpty(numAnimationStopAudioName))
                            AudioUtil.Instance.PlayAudioFx(numAnimationStopAudioName);

                        AudioUtil.Instance.FadeMusicTo(1, 0.5f, 0.5f);
                    });

                    numAnimationDuration = duration;
                }

                numAnimationStartTime = Time.realtimeSinceStartup;

                if (numAnimationAudioName != null)
                {
                    AudioUtil.Instance.FadeMusicTo(0.5f, 0.5f, 1);
                    numAnimationAudioSource = AudioUtil.Instance.PlayAudioFx(numAnimationAudioName, audioLoop);
                }

                winCoinNum = winChips;
            }
        }

        public void UpdateControlPanelState(bool isFreeSpin, bool isAverage)
        {
            freeBetObj.SetActive(isFreeSpin);
            normalBetObj.SetActive(!isFreeSpin);

            if (isFreeSpin)
            {
                TotalFreeCount = 0;
                freeTotalBetImg.SetActive(!isAverage && !isExtraBet);
                freeExtraBet.gameObject.SetActive(isExtraBet && !isAverage);
                freeBetText.gameObject.SetActive(!isAverage);
                freeAverageBet.SetActive(isAverage);
            }
            else
            {
                totalBetImg.SetActive(!isAverage && !isExtraBet);
                normalExtraBet.gameObject.SetActive(isExtraBet && !isAverage);
                totalBetText.gameObject.SetActive(!isAverage);
                normalAverageBet.SetActive(isAverage);
            }
        }

        public void UpdateFreeSpinCountText(uint currentCount, uint totalCount, bool enterRoomFromLink = false,
            bool playAddFree = true)
        {
            var countIndex = currentCount + 1;
            if (enterRoomFromLink)
            {
                countIndex = currentCount;
            }

            if (countIndex > totalCount)
            {
                countIndex = totalCount;
            }

            if (totalCount > TotalFreeCount && TotalFreeCount > 0 && playAddFree)
            {
                winAnimationAnimator.Play("Add", 0, 0);
            }

            TotalFreeCount = totalCount;
            freeSpinCountTxt.text = countIndex + "/" + totalCount;
        }

        public void UpdateAutoSpinLeftCount(int leftCount)
        {
            autoSpinNumText.gameObject.SetActive(leftCount >= 0);

            if (leftCount >= 0)
            {
                autoSpinNumText.text = leftCount.ToString();
            }
        }


        public float GetNumAnimationEndTime()
        {
            return Math.Max(0, numAnimationStartTime + numAnimationDuration - Time.realtimeSinceStartup);
        }

        private Sequence fadeSequence = null;

        public void StopWinAnimation(bool stopAtWinCoinNum = true, ulong endWinCoinNum = 0)
        {
            DOTween.Kill(winText);
            DOTween.Kill(largeWinText);

            fadeSequence?.Kill();

            //Reset winText Alpha to one
            winText.color = new Color(winText.color.r, winText.color.g, winText.color.b, 1);

            var stateInfo = winAnimationAnimator.GetCurrentAnimatorStateInfo(0);
            //有打断
            if (stateInfo.IsName(niceWinLoopAnimation) || stateInfo.IsName(niceWinOpenAnimation))
            {
                winAnimationAnimator.speed = 2.0f;
                winAnimationAnimator.Play(niceWinCloseAnimation, 0, 0);

                UpdateWinLabelChips(winCoinNum);

                if (!stopAtWinCoinNum && endWinCoinNum == 0)
                {
                    fadeSequence = DOTween.Sequence();

                    fadeSequence.AppendInterval(1.0f);
                    fadeSequence.OnComplete(() =>
                    {
                        winText.text = "";
                        fadeSequence = null;
                    });
                    fadeSequence.Play();
                }
            }
            //没有打断
            else
            {
                UpdateWinLabelChips(winCoinNum);
                //没有动画直接 fade 
                if (!stopAtWinCoinNum && endWinCoinNum == 0)
                {
                    context.WaitSeconds(0.5f, () =>
                    {
                        winText.text = "";
                        //winText.DOFade(0, 0.2f);
                    });
                }
            }


            //音效没有播完，掐调，播尾音
            if (numAnimationAudioSource != null)
            {
                if (numAnimationAudioSource.isPlaying && numAnimationAudioSource.clip != null && numAnimationAudioName != null && numAnimationAudioSource.clip.name == numAnimationAudioName)
                {
                    numAnimationAudioSource.Stop();
                    if (!string.IsNullOrEmpty(numAnimationStopAudioName))
                        AudioUtil.Instance.PlayAudioFx(numAnimationStopAudioName);
                    AudioUtil.Instance.FadeMusicTo(1, 0.5f, 0.5f);
                }

                numAnimationAudioSource = null;
            }

            if (!stopAtWinCoinNum)
                winCoinNum = (long) endWinCoinNum;

            numAnimationDuration = 0;
            numAnimationStartTime = 0;
            numAnimationAudioName = null;
            numAnimationStopAudioName = null;

            context.RemoveWaitEvent(WaitEvent.WAIT_WIN_NUM_ANIMTION);
        }

        public override void OnDestroy()
        {
            _dailyMissionMachineEntranceView.Destroy();
            _dailyMissionMachineEntranceView = null;

            lackOfBetTipView?.Destroy();
            lackOfBetTipView = null;

            albumBetTipView?.Destroy();
            albumBetTipView = null;

            base.OnDestroy();
        }

        public Vector3 GetWinTextRefWorldPosition(Vector3 startPos)
        {
            var position = winText.transform.position;
            var cameraPos = Camera.main.transform.position;
            var winTextRelative = position - cameraPos;
            var startRelativePos = startPos - cameraPos;

            return new Vector3((winTextRelative.x * startRelativePos.z) / (winTextRelative.z) + cameraPos.x,
                (winTextRelative.y * startRelativePos.z) / (winTextRelative.z) + cameraPos.y, startPos.z);
        }

        private void GetWinTextRefWorldPosition()
        {
            var position = winText.transform.position;
            var cameraZ = Camera.main.transform.position.z;
            var canvasZ = context.MachineUICanvasTransform.position.z;
            var factor = Mathf.Abs(cameraZ / canvasZ);
            winTextRefWorldPosition = new Vector3(factor * position.x, factor * position.y, 0);
        }

        public Vector3 GetFreeCountRefWorldPosition()
        {
            var position = freeSpinCountTxt.transform.position;
            var cameraZ = Camera.main.transform.position.z;
            var canvasZ = context.MachineUICanvasTransform.position.z;
            var factor = Mathf.Abs(cameraZ / canvasZ);
            return new Vector3(factor * position.x, factor * position.y, 0);
        }

        private void CheckAndCleanTapToSpinGuide()
        {
            if (guideTapSpin != null)
            {
                var canvas = transform.Find("Canvas");
                canvas.GetComponent<Canvas>().overrideSorting = false;

                guideSpinButton.gameObject.SetActive(false);

                guideTapSpin.Destroy();
                guideTapSpin = null;

                if (Client.Get<GuideController>().CheckNeedShowGuideSpin())
                {
                    EventBus.Dispatch(new EventGuideFinished(Client.Get<GuideController>().GetEnterMachineGuide()));

                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                        .GameEventGuideTapSpin1);
                }
                // Log.LogUiClickEvent(UiClick.GuideSpinClicked, null, StepId.ID_GUIDE_SPIN_CLICK);
            }
        }

        private async void CreateLackOfBetTipView()
        {
            transLackOfBetTipContainer = transform.Find("LackOfBetTipContainer");
            var lackTipAddress = ViewManager.Instance.IsPortrait
                ? "ControlPanelLackOfBetBubbleV"
                : "ControlPanelLackOfBetBubbleH";
            lackOfBetTipView = await View.CreateView<LackOfBetTipView>(lackTipAddress, transLackOfBetTipContainer);
            lackOfBetTipView.viewController.Initialize(context);
        }

        private async void CreateAlbumBetTipView()
        {
            albumBetTipView = await View.CreateView<AlbumBetTipView>("AlbumBetTipView", _betTipAttachPoint);
            albumBetTipView.SetUpView(ViewManager.Instance.IsPortrait);
        }

        public Vector3 GetFreeCountTxtPosition()
        {
            return freeSpinCountTxt.transform.position;
        }
    }
}