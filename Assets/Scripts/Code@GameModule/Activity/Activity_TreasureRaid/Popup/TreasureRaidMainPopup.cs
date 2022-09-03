using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidData
    {
        public RepeatedField<Reward> roundListCompleteRewards;
        public MonopolyRoundInfo roundInfo;
        public MonopolyDailyTask dailyTask;
        public TreasureRaidData(MonopolyRoundInfo inRoundInfo, RepeatedField<Reward> inRoundListCompleteRewards, MonopolyDailyTask inDailyTask)
        {
            roundInfo = inRoundInfo;
            roundListCompleteRewards = inRoundListCompleteRewards;
            dailyTask = inDailyTask;
        }
    }

    public class TreasureRaidBoosterBarView : View<TreasureRaidBoosterBarViewController>
    {
        [ComponentBinder("BoostList/EnergyCell/PropertyGroup/PropertyText")]
        private Text energyText;
        
        [ComponentBinder("BoostList/EnergyCell/PropertyGroup/PlusButton")]
        private Button energyBtn;
        
        
        [ComponentBinder("BoostList/WeaponCell/PropertyGroup/PropertyText")]
        private Text weaponText;
        
        [ComponentBinder("BoostList/WeaponCell/PropertyGroup/PlusButton")]
        private Button weaponBtn;
        
        
        [ComponentBinder("BoostList/ToAnyWhereCell/PropertyGroup/PropertyText")]
        private Text portalText;
        
        [ComponentBinder("BoostList/ToAnyWhereCell/PropertyGroup/PlusButton")]
        private Button portalBtn;

        [ComponentBinder("BoostList/UseButton")]
        private Button useBtn;

        private CanvasGroup _canvasGroup;
        
        protected override void OnViewSetUpped()
        {
            _canvasGroup = transform.GetComponent<CanvasGroup>();
            useBtn.onClick.AddListener(OnUseBtnClicked);
            energyBtn.onClick.AddListener(OnAddBtnClicked);
            weaponBtn.onClick.AddListener(OnAddBtnClicked);
            portalBtn.onClick.AddListener(OnAddBtnClicked);
            
            base.OnViewSetUpped();
        }

        public void SetCanvasGroup(bool interactable)
        {
            _canvasGroup.blocksRaycasts = interactable;
        }

        private void OnAddBtnClicked()
        {
            // 打开内购Booster页面
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidBoosterPopup))));
        }

        private void OnUseBtnClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidChooseMapPopup), parentView)));
        }

        public void RefreshBoosterUI()
        {
            var leftTime = viewController.GetEnergyCountDown();
            energyText.SetText(XUtility.GetTimeText(leftTime));

            var leftTimeW = viewController.GetWeaponCountDown();
            weaponText.SetText(XUtility.GetTimeText(leftTimeW));

            var portalCount = viewController.GetPortalCount();
            portalText.SetText(portalCount.ToString());
            if (portalCount > 0 && !useBtn.gameObject.activeSelf)
            {
                useBtn.gameObject.SetActive(true);
            }
            else if (portalCount <= 0 && useBtn.gameObject.activeSelf)
            {
                useBtn.gameObject.SetActive(false);
            }
        }
    }

    public class TreasureRaidBoosterBarViewController : ViewController<TreasureRaidBoosterBarView>
    {
        private Activity_TreasureRaid activity_TreasureRaid;

        public override void OnViewDidLoad()
        {
            activity_TreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.OnViewDidLoad();
            view.RefreshBoosterUI();
            EnableUpdate(1);
        }

        public override void Update()
        {
            view.RefreshBoosterUI();
        }

        public float GetEnergyCountDown()
        {
            var roundInfo = activity_TreasureRaid.GetRoundInfo();
            var moreTicket = roundInfo.MonopolyBuffMoreTicket;

            if (moreTicket == null)
                return 0;

            var leftTime = XUtility.GetLeftTime((ulong) moreTicket.Expire*1000);
            leftTime = leftTime < 0 ? 0 : leftTime;
            return leftTime;
        }
        
        public float GetWeaponCountDown()
        {
            var roundInfo = activity_TreasureRaid.GetRoundInfo();
            var moreDamage = roundInfo.MonopolyBuffMoreDamage;

            if (moreDamage == null)
                return 0;

            var leftTime = XUtility.GetLeftTime((ulong) moreDamage.Expire*1000);
            leftTime = leftTime < 0 ? 0 : leftTime;
            return leftTime;
        }

        public uint GetPortalCount()
        {
            var roundInfo = activity_TreasureRaid.GetRoundInfo();

            if (roundInfo.MonopolyPortal == null)
                return 0;

            return roundInfo.MonopolyPortal.Amount;
        }
    }

    public class TreasureRaidChestBoxView : View<TreasureRaidChestBoxViewController>
    {
        [ComponentBinder("StateGroup/BoxGroup")]
        public Transform boxGroup;

        [ComponentBinder("StateGroup/OpenState")]
        public Transform openState;
        
        [ComponentBinder("StateGroup/OpenState/OpenButton")]
        public Button openBtn;

        [ComponentBinder("StateGroup/TimerState")]
        private Transform leftTimeGroup;

        [ComponentBinder("StateGroup/TimerState/TimerGroup/TimerText")]
        public TextMeshProUGUI leftTimeText;

        [ComponentBinder("StateGroup/TimerState/SpendButton/SpendGroup/CountText")]
        private TextMeshProUGUI costDiamondCount;

        [ComponentBinder("StateGroup/TimerState/SpendButton")]
        public Button openUseDiamondBtn;

        [ComponentBinder("BoxEmpty")]
        private Transform boxSlot;

        [ComponentBinder("StateGroup")]
        private Transform stateGroup;

        public bool SetUIState()
        {
            var giftBoxPosition = viewController._giftBoxPosition;
            bool openUpdate = false;
            boxSlot.gameObject.SetActive(!giftBoxPosition.HasBox);
            stateGroup.gameObject.SetActive(giftBoxPosition.HasBox);
            
            if (giftBoxPosition.HasBox)
            {
                for (int i = 0; i < boxGroup.childCount; i++)
                {
                    boxGroup.GetChild(i).gameObject.SetActive((i+1) == giftBoxPosition.GiftBox.Level);
                }
                openUpdate = viewController.GetCountDown() > 0;
                costDiamondCount.SetText(giftBoxPosition.GiftBox.OpenDiamondCurrent.ToString());
                leftTimeGroup.gameObject.SetActive(openUpdate);
                openState.gameObject.SetActive(!openUpdate);
            }
            else
            {
                leftTimeGroup.gameObject.SetActive(false);
            }

            return openUpdate;
        }

        public void SetBtnState(bool interactable)
        {
            openBtn.interactable = interactable;
            openUseDiamondBtn.interactable = interactable;
        }

        public void ShowGetBoxAni(Transform particle, int level, Transform latticeTr, Action callback)
        {
            boxSlot.gameObject.SetActive(false);
            stateGroup.gameObject.SetActive(true);

            leftTimeGroup.gameObject.SetActive(false);
            openState.gameObject.SetActive(false);
            var ani = transform.GetComponent<Animator>();
            SoundController.PlaySfx("TreasureRaid_BoxFly");
            XUtility.PlayAnimation(ani, "appear");
            var box = boxGroup.GetChild(level - 1);
            particle.position = latticeTr.GetChild(0).Find("BoxRewardType").position;
            particle.gameObject.SetActive(true);
            particle.DOMove(transform.position, 0.33f).OnComplete(() =>
            {
                if (transform == null)
                    return;

                callback?.Invoke();
                box.gameObject.SetActive(true);
            });
        }
    }

    public class TreasureRaidChestBoxViewController : ViewController<TreasureRaidChestBoxView>
    {
        public MonopolyRoundInfo.Types.GiftBoxPosition _giftBoxPosition;

        private Activity_TreasureRaid activity_TreasureRaid;
        private int _boxIndex;


        public override void OnViewDidLoad()
        {
            activity_TreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.OnViewDidLoad();
            view.openBtn.onClick.AddListener(OnOpenChestBoxBtnClicked);
            view.openUseDiamondBtn.onClick.AddListener(OnUseDiamondOpenChestBoxBtnClicked);
        }

        private async void OnUseDiamondOpenChestBoxBtnClicked()
        {
            view.SetBtnState(false);
            var useDiamondOpenChestNowView =
                await PopupStack.ShowPopup<TreasureRaidUseDiamondOpenChestNowPopup>();
            useDiamondOpenChestNowView.SetMonopolyGiftBoxAndCallback(_giftBoxPosition.GiftBox, (bUseDiamond, inDailyTask) =>
            {
                view.SetBtnState(true);
                if (bUseDiamond)
                {
                    var parentView = view.GetParentView() as TreasureRaidMainPopup;
                    parentView?._dailyTaskView.RefreshDailyTask(true, () =>
                    {
                        activity_TreasureRaid?.SetActivityState(false);
                    }, inDailyTask, false);
                }
            });
        }

        private async void OnOpenChestBoxBtnClicked()
        {
            view.SetBtnState(false);

            if (activity_TreasureRaid != null && _giftBoxPosition != null)
            {
                var parentView = view.GetParentView() as TreasureRaidMainPopup;
                parentView?.SetBtnState(false);
                
                SMonopolyOpenGiftBox sMonopolyOpenGiftBox = await activity_TreasureRaid.MonopolyOpenGiftBox(_giftBoxPosition.GiftBox);
                if (sMonopolyOpenGiftBox == null)
                    return;
                if (view.transform == null)
                    return;
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidCountdownOver);
                var ani = view.transform.GetComponent<Animator>();
                var initialPos = view.boxGroup.localPosition;
                view.boxGroup.DOMove(new Vector3(0, 0, view.boxGroup.position.z), 0.33f);
                view.openState.gameObject.SetActive(false);
                SoundController.PlaySfx("TreasureRaid_BoxOpen");
                await XUtility.PlayAnimationAsync(ani, "Open");

                parentView?.CheckHasOpenBox();
                var openChestView =  await PopupStack.ShowPopup<TreasureRaidOpenChestPopup>();
                openChestView.InitRewardContent(sMonopolyOpenGiftBox, _giftBoxPosition.GiftBox);
                openChestView.ShowRewardCollect(() =>
                {
                    if (view.transform == null)
                        return;
                    view.boxGroup.localPosition = initialPos;
                    SetChestPosition(sMonopolyOpenGiftBox.MonopolyRoundInfo.GiftBoxPositions[_boxIndex], _boxIndex);
                    XUtility.PlayAnimation(ani, "idle");
                    view.SetBtnState(true);
                    parentView?.SetBtnState(true);
                    parentView?._dailyTaskView.RefreshDailyTask(true, () =>
                    {
                        activity_TreasureRaid?.SetActivityState(false);
                    }, sMonopolyOpenGiftBox.DailyTask, false);
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="giftBoxPosition"></param>
        /// <returns>返回如果是true 开启Update， 如果为false 关闭Update</returns>
        public void SetChestPosition(MonopolyRoundInfo.Types.GiftBoxPosition giftBoxPosition, int boxIndex)
        {
            _giftBoxPosition = giftBoxPosition;
            _boxIndex = boxIndex;

            bool openUpdate = view.SetUIState();
            if (openUpdate)
            {
                var leftTime = GetCountDown();
                view.leftTimeText.SetText(XUtility.GetTimeText(leftTime));
                EnableUpdate(1);
            }
            else
            {
                DisableUpdate();
            }
        }

        public override void Update()
        {
            base.Update();
            var leftTime = GetCountDown();
            if (leftTime <= 0)
            {
                DisableUpdate();
                view.SetUIState();
                var parentView = view.GetParentView() as TreasureRaidMainPopup;
                parentView?.CheckHasOpenBox();
            }
            else
            {
                view.leftTimeText.SetText(XUtility.GetTimeText(leftTime));
            }
        }

        public float GetCountDown()
        {
            if (!_giftBoxPosition.HasBox)
                return 0;
            return _giftBoxPosition.GiftBox.OpenSecondsCountDown - (Time.realtimeSinceStartup - activity_TreasureRaid.GetBoxUpdateTime());
        }
    }

    [AssetAddress("UITreasureRaidGameMap")]
    public class TreasureRaidMainPopup : Popup<TreasureRaidMainPopupController>
    {
        [ComponentBinder("Root")] private Transform root;

        [ComponentBinder("Root/AdaptGroup/GameMapPathPointGridCell")]
        public Transform pathPointPrefab;

        [ComponentBinder("Root/GameMapPathPointPlayerCell")]
        public Transform _playerTr;

        [ComponentBinder("Root/GameMapPathPointPlayerCell")]
        public Animator _playerAni;

        [ComponentBinder("Root/TopGroup")]
        private Transform topGroup;
        
        [ComponentBinder("Root/TopGroup")]
        private CanvasGroup topCanvasGroup;
        
        [ComponentBinder("Root/TopGroup/BackButton")]
        public Button backBtn;

        [ComponentBinder("Root/AdaptGroup/PathGroup")]
        public Transform latticeContainer;//路径父节点

        [ComponentBinder("Root/AdaptGroup/Puzzle")]
        public Transform puzzlePrefab;

        [ComponentBinder("Root/BoxListGroup/BoxLayoutGroup")] 
        private Transform chestBoxContainer;

        [ComponentBinder("Root/AdaptGroup/PathGroup/GameMapEnemyGroup")] 
        public Transform bossGroup;

        [ComponentBinder("Root/TopGroup/TitleGroup")] 
        public Transform titleGroup;

        [ComponentBinder("Root/AdaptGroup/PathGroup/GameMapEnemyGroup/ProgressGroup/ProgressText")] 
        private TextMeshProUGUI bossBloodText;

        [ComponentBinder("Root/AdaptGroup/PathGroup/GameMapEnemyGroup/ProgressGroup/ProgressBar")] 
        private Slider bossBloodProgress;

        [ComponentBinder("Root/SpinGroup")]
        public Transform wheelGroup;

        [ComponentBinder("Root/UITreasureRaidGameMapBoostList")]
        public Transform boosterGroup;

        [ComponentBinder("Root/TopGroup/TitleGroup/DailyTaskGroup")]
        public Transform dailyTaskGroup;
        
        [ComponentBinder("Root/SpinGroup")]
        public CanvasGroup wheelCanvasGroup;
        
        [ComponentBinder("Root/SpinGroup/WheelRoot")]
        public Transform wheelRoot;
        [ComponentBinder("Root/SpinGroup/ExtraGroup")]
        public Transform wheelExtraGroup;
        
        [ComponentBinder("Root/TopGroup/TitleGroup/RewardGroup/IntegralText")]
        public Text rewardCoinText;

        [ComponentBinder("Root/TopGroup/TitleGroup/RoundGroup/RoundText")]
        public Text roundText;

        [ComponentBinder("Root/TopGroup/TitleGroup/RewardGroup/ExtraReward")]
        public Transform extraRewardGroup;

        [ComponentBinder("Root/CoinGroup")]
        private Transform coinGroup;

        [ComponentBinder("Root/Coins")]
        private Transform coinPrefab;

        [ComponentBinder("Root/GameMapPathPointPlayerCell/Root/AvatarMask/Icon")]
        private RawImage rawImageAvatar;

        [ComponentBinder("Root/BoxListGroup/ControlButton")]
        private Button controllerBtn;

        [ComponentBinder("Root/TopGroup/InformationButton")]
        private Button helpBtn;

        [ComponentBinder("Root/BoxListGroup")]
        public RectTransform boxListGroup;

        [ComponentBinder("Root/BoxListGroup")]
        public Animator boxListGroupAnimator;
        
        [ComponentBinder("Root/BoxListGroup")]
        public CanvasGroup boxListGroupCanvasGroup;
        
        [ComponentBinder("Root/BoxListGroup/ControlButton/ReminderGroup")]
        private Transform boxReminderGroup;

        [ComponentBinder("Root/BoxListGroup/ControlButton/Icon")] 
        private Transform boxListIcon;

        [ComponentBinder("Root/AdaptGroup")]
        private Transform adaptGroup;

        [ComponentBinder("Root/SpinGroup/ExtraGroup/PressNoticeTag")]
        public Transform pressNoticeTr;
        
        [ComponentBinder("Root/BoxCell_FlyEff")]
        public Transform boxParticle;

        [ComponentBinder("Root/TeleportParticle")]
        public Transform teleportParticle;

        [ComponentBinder("Root/TopGroup/TitleGroup/RankGroup")]
        public Transform rankGroup;

        [ComponentBinder("Root/TopGroup/TitleGroup/PuzzleGroup")]
        public Transform puzzleGroup;

        private bool boxListGroupOut = true;

        private List<TreasureRaidChestBoxView> _chestBoxViews;


        private bool _needForceLandscapeScreen = true;

        private CurrencyCoinView _currencyCoinView;

        private CancelableCallback flyCancelableCallback;

        private TreasureRaidWheelView _wheelView;
        private TreasureRaidBoosterBarView _boosterBarView;
        public TreasureRaidDailyTaskView _dailyTaskView;
        public TreasureRaidRankView _rankView;
        public TreasureRaidPuzzleView _puzzleView;

        public uint lastPlayerIndex;

        private float lastBlood;

        private int lastBossLevel;

        public TreasureRaidMainPopup(string address) : base(address)
        {
            
        }

        protected override async void SetUpExtraView()
        {
            base.SetUpExtraView();

            _chestBoxViews = new List<TreasureRaidChestBoxView>();
            for (int i = 0; i < chestBoxContainer.childCount; i++)
            {
                var chestBoxView = AddChild<TreasureRaidChestBoxView>(chestBoxContainer.GetChild(i));
                _chestBoxViews.Add(chestBoxView);
            }
            boxParticle.gameObject.SetActive(false);

            boxListIcon.localScale = new Vector3(-1, 1, 0);
            _wheelView = AddChild<TreasureRaidWheelView>(wheelGroup);
            _boosterBarView = AddChild<TreasureRaidBoosterBarView>(boosterGroup);
            _dailyTaskView = AddChild<TreasureRaidDailyTaskView>(dailyTaskGroup);
            _rankView = AddChild<TreasureRaidRankView>(rankGroup);
            _puzzleView = AddChild<TreasureRaidPuzzleView>(puzzleGroup);

            _currencyCoinView = await AddCurrencyCoinView();

            if (transform == null)
                return;

            _currencyCoinView.Hide();
            _currencyCoinView.transform.SetParent(root, false);
            _currencyCoinView.transform.SetSiblingIndex(coinGroup.GetSiblingIndex());
        }

        public override void OnOpen()
        {
            base.OnOpen();
            var bAdaptor = Client.Get<ActivityController>().IsAdaptor();
            boxListGroup.anchoredPosition = bAdaptor ? new Vector2(71f, -52.6f) : new Vector2(0, -52.6f);
            boosterGroup.GetComponent<RectTransform>().anchoredPosition = bAdaptor ? new Vector2(-71f, 84) : new Vector2(0, 84);
            _puzzleView.RefreshPuzzle(false);
            _rankView.RefreshRank(viewController.sGetMonopolyRoundInfo.SelfRankInfo.MyRank);
            _puzzleView.viewController.CheckHasRewardToCollect(() =>
            {
                SetBtnState(false);
            }, () =>
            {
                SetBtnState(true);
            });
        }

        public void CheckHasOpenBox()
        {
            var activityTreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                Activity_TreasureRaid;
            if (activityTreasureRaid == null)
                return;
            var giftBoxPosition = activityTreasureRaid.GetGiftBoxInfo();
            bool hasBox = false;
            for (int i = 0; i < giftBoxPosition.Count; i++)
            {
                if (giftBoxPosition[i].HasBox && _chestBoxViews[i].viewController.GetCountDown() <= 0)
                {
                    hasBox = true;
                    break;
                }
            }
            boxReminderGroup.gameObject.SetActive(hasBox);
        }

        public override bool NeedForceLandscapeScreen()
        {
            return _needForceLandscapeScreen;
        }

        public void SetBtnState(bool interactable)
        {
            closeButton.interactable = interactable;
            backBtn.interactable = interactable;
            controllerBtn.interactable = interactable;
            helpBtn.interactable = interactable;
            if (pressNoticeTr.gameObject.activeSelf)
            {
                pressNoticeTr.gameObject.SetActive(false);
            }
            boxListGroupCanvasGroup.blocksRaycasts = interactable;
            wheelCanvasGroup.blocksRaycasts = interactable;
            if (_boosterBarView != null)
            {
                _boosterBarView.SetCanvasGroup(interactable);
            }
            topCanvasGroup.blocksRaycasts = interactable;
        }

        /// <summary>
        /// 点击返回关卡列表设置不转屏false，点击关闭按钮是设置为true
        /// </summary>
        /// <param name="isForce"></param>
        public void SetNeedForceLandscapeScreen(bool isForce)
        {
            _needForceLandscapeScreen = isForce;
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            backBtn.onClick.AddListener(OnBackBtnClicked);
            controllerBtn.onClick.AddListener(OnControllerBtnClicked);
            helpBtn.onClick.AddListener(OnHelpBtnClicked);
            AdaptScaleTransform(adaptGroup, new Vector2(1365, 606));
            AdaptScaleTransform(boxListGroup, new Vector2(1365, 606));
            AdaptScaleTransform(topGroup, new Vector2(1570, 606));
            AdaptScaleTransform(wheelRoot, new Vector2(1165, 300));
            AdaptScaleTransform(wheelExtraGroup, new Vector2(1165, 300));
            coinPrefab.gameObject.SetActive(false);
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

        private void OnHelpBtnClicked()
        {
            PopupStack.ShowPopupNoWait<TreasureRaidHelpPopup>();
        }

        private void OnControllerBtnClicked()
        {
            ShowInOutBoxListGroup(!boxListGroupOut);
        }

        private void ShowInOutBoxListGroup(bool showIn)
        {
            if (boxListGroupOut == showIn)
                return;

            boxListGroupOut = showIn;
            boxListIcon.localScale = new Vector3(showIn ? -1 : 1, 1, 0);
            XUtility.PlayAnimation(boxListGroupAnimator, showIn ? "Open" : "Close");
        }

        private async void OnBackBtnClicked()
        {
            SetBtnState(false);
            foreach (var boxView in _chestBoxViews)
            {
                boxView.SetBtnState(false);
            }
            await PopupStack.ShowPopup<TreasureRaidMapPopup>();
            SetNeedForceLandscapeScreen(false);
            Close();
        }

        protected override void OnCloseClicked()
        {
            SetBtnState(false);
            if (viewController.activityTreasureRaid != null)
            {
                viewController.activityTreasureRaid.InActivity = false;
            }
            SetNeedForceLandscapeScreen(true);
            SoundController.RecoverLastMusic();
            base.OnCloseClicked();
        }

        public override void Close()
        {
            var sks = transform.GetComponentsInChildren<SkeletonGraphic>();
            var noColor = new Color(1, 1, 1, 0);
            foreach (var sk in sks)
            {
                sk.DOColor(noColor, 0.28f);
            }
            base.Close();
        }

        public void SetChestBoxPositionExceptCurrent(SMonopolySpin sMonopolySpin)
        {
            if (sMonopolySpin.GiftBoxReward != null && !sMonopolySpin.GiftBoxFailedToPosition)
            {
                for (int i = 0; i < sMonopolySpin.MonopolyRoundInfo.GiftBoxPositions.Count; i++)
                {
                    if (i == sMonopolySpin.GiftBoxRewardPositionIndex) continue;
                    var giftBoxPosition = sMonopolySpin.MonopolyRoundInfo.GiftBoxPositions[i];
                    _chestBoxViews[i].viewController.SetChestPosition(giftBoxPosition, i);
                }
            }
            else
            {
                SetAllChestBoxPosition(sMonopolySpin.MonopolyRoundInfo);
            }
        }
        
        public void SetAllChestBoxPosition(MonopolyRoundInfo roundInfo)
        {
            for (int i = 0; i < roundInfo.GiftBoxPositions.Count; i++)
            {
                var giftBoxPosition = roundInfo.GiftBoxPositions[i];
                _chestBoxViews[i].viewController.SetChestPosition(giftBoxPosition, i);
            }
        }

        public async void BeginShowSpinEndAni(SMonopolySpin sMonopolySpin, int forwardStep, Action callback)
        {
            if (flyCancelableCallback != null)
            {
                flyCancelableCallback.CancelCallback();
            }
            _currencyCoinView.Show();
            InitCoinsItem((int)lastPlayerIndex + 1, forwardStep);
            await XUtility.WaitSeconds(0.5f);
            if (transform == null)
                return;

            PlayerMoveToNext(sMonopolySpin,lastPlayerIndex + 1, callback, 0, forwardStep);
            lastPlayerIndex = sMonopolySpin.MonopolyRoundInfo.CurrentLatticeIndex;
        }

        private void InitCoinsItem(int start, int forwardStep)
        {
            SoundController.PlaySfx("TreasureRaid_GoldAppear");
            for (int i = 0; i < forwardStep; i++)
            {
                var coin = GameObject.Instantiate(coinPrefab, coinGroup, false);
                coin.gameObject.SetActive(true);
                coin.position = viewController.GetSpecialLatticeTransform( start + i > 27 ? start + i - 28 : start + i).position;
            }
        }

        private async void PlayerMoveToNext(SMonopolySpin sMonopolySpin, uint currentIndex, Action callback, int currentCoinIndex, int forwardStep)
        {
            if (currentIndex > 27)
                currentIndex = 0;

            if ((currentCoinIndex + 1) > forwardStep)
            {
                await XUtility.WaitSeconds(0.8f);
                for (int i = coinGroup.childCount - 1; i >= 0; i--)
                {
                    GameObject.Destroy(coinGroup.GetChild(i).gameObject);
                }
                CheckMoveEndAni(sMonopolySpin, callback);
                return;
            }

            var targetTr = viewController.GetSpecialLatticeTransform((int) currentIndex);
            var targetPos = viewController.GetPlayPosInLattice(currentIndex);
            XUtility.PlayAnimation(_playerAni, "move");
            _playerTr.DOJump(targetPos, 5f, 1, 0.266f).OnComplete(async () =>
            {
                if (coinGroup == null)
                    return;
                var coinItem = XItemUtility.GetCoinItem(sMonopolySpin.CoinReward);
                XUtility.PlayAnimation(_playerAni, "idle");
                SoundController.PlaySfx("TreasureRaid_Jump");
                await FlyCoins(targetTr, coinGroup.GetChild(currentCoinIndex),
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "Activity_Treasure_Move_Coin_Reward"),
                    _currencyCoinView);
            });
            await XUtility.WaitSeconds(0.33f);
            PlayerMoveToNext(sMonopolySpin, currentIndex + 1, callback, currentCoinIndex + 1, forwardStep);
        }

        public async Task FlyCoins(Transform source, Transform flyTr,  EventBalanceUpdate eventBalanceUpdate, CurrencyCoinView currencyCoinView)
        {
            currencyCoinView.viewController.ShowCollectFx();
            
            SoundController.PlaySfx("CashCrazy_Coins_Fly");   

            Vector3 sourceWorldPos = source.parent.TransformPoint(source.localPosition);
            Vector3 fromLocalPos = coinGroup.InverseTransformPoint(sourceWorldPos);
            Transform target = TopPanel.GetCoinIcon();

            target = currencyCoinView.GetCoinIcon();

            Vector3 targetWorldPos = target.parent.TransformPoint(target.localPosition);
            Vector3 targetLocalPos = coinGroup.InverseTransformPoint(targetWorldPos);

            Vector3 midLocalPos = (fromLocalPos + targetLocalPos) * 0.5f;
            midLocalPos.x -= 50;
            midLocalPos.y -= 50;
            midLocalPos.z = -100;

            Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};
            flyTr.localPosition = fromLocalPos;
            wayPoints[0] = flyTr.localPosition;
            flyTr.DOScale(0.5f, 0.65f);
            flyTr.DOLocalPath(wayPoints, 0.65f, PathType.CatmullRom, PathMode.Full3D, 10)
                .SetDelay(0.1f).OnComplete(() =>
                {
                    flyTr.gameObject.SetActive(false);
                    EventBus.Dispatch(eventBalanceUpdate);
                }).SetEase(Ease.InQuad);
            await XUtility.WaitSeconds(0.65f);
            EventBus.Dispatch(new EventCoinCollectFx(false));
            if (flyCancelableCallback != null)
            {
                flyCancelableCallback.CancelCallback();
            }
            flyCancelableCallback = viewController.WaitForSeconds(3f, () =>
            {
                _currencyCoinView.Hide();
            });
        }

        private void CheckMoveEndAni(SMonopolySpin sMonopolySpin, Action callback)
        {
            MonopolyRoundInfo.Types.SpecialLattice currentLattices = null;
            // 先判断当前格子是武器还是宝箱或者什么都没有
            foreach (var specialLattice in sMonopolySpin.MonopolyRoundInfo.SpecialLattices)
            {
                if (sMonopolySpin.MonopolyRoundInfo.CurrentLatticeIndex == specialLattice.Index)
                {
                    currentLattices = specialLattice;
                    break;
                }
            }

            // currentLattices 为空 表示在普通格子上
            if (currentLattices == null)
            {
                viewController.CheckCurrentLevelComplete(sMonopolySpin.MonopolyRoundInfo,
                    sMonopolySpin.RoundCompleteRewards, sMonopolySpin.RoundListCompleteRewards, callback,
                    sMonopolySpin.DailyTask, true, sMonopolySpin.RewardLatticeReward);
            }
            else
            {
                _playerTr.gameObject.SetActive(false);

                void ShowAniCallback(MonopolyDailyTask task)
                {
                    _playerTr.gameObject.SetActive(true);
                    viewController.CheckCurrentLevelComplete(sMonopolySpin.MonopolyRoundInfo,
                        sMonopolySpin.RoundCompleteRewards, sMonopolySpin.RoundListCompleteRewards, callback, task);
                }

                switch (currentLattices.Type)
                {
                    case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Cannon:
                        // 展示攻击动画
                        ShowAttackAni(sMonopolySpin.MonopolyRoundInfo, currentLattices, ShowAniCallback, callback,
                            sMonopolySpin.DailyTask);
                        break;
                    case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Giftbox:
                        // 展示获取宝箱奖励
                        ShowCollectBoxAni(sMonopolySpin.GiftBoxReward, sMonopolySpin.GiftBoxFailedToPosition,
                            sMonopolySpin.GiftBoxRewardPositionIndex, sMonopolySpin.MonopolyRoundInfo, currentLattices,
                            ShowAniCallback, sMonopolySpin.DailyTask);
                        break;
                }
            }
        }

        private async void ShowAttackAni(MonopolyRoundInfo roundInfo, MonopolyRoundInfo.Types.SpecialLattice currentLattices,
            Action<MonopolyDailyTask> endCallback, Action fireAgainCallback, MonopolyDailyTask dailyTask, bool showBooster = true)
        {
            var lattice = viewController.GetSpecialLatticeTransform((int) currentLattices.Index);
            // 找到Animator 播放攻击动画
            await viewController.PlayLatticeAttackAni(lattice, currentLattices);
            RefreshMasterUI(roundInfo, true);
            await XUtility.WaitSeconds(0.5f);

            if (roundInfo.BloodCurrent <= 0)
            {
                endCallback.Invoke(dailyTask);
            }
            else
            {
                // 刷新boss血量并弹出FireAgain弹窗
                var fireAgainView = await PopupStack.ShowPopup<TreasureRaidFireAgainPopup>();
                fireAgainView.SetEndCallback( async (bFireAgain, sMonopolyShootAgain) =>
                {
                    if (transform == null)
                        return;

                    if (bFireAgain)
                    {
                        // 找到Animator 播放攻击动画
                        await viewController.PlayLatticeAttackAni(lattice, currentLattices);
                        RefreshMasterUI(sMonopolyShootAgain.MonopolyRoundInfo, true);
                        await XUtility.WaitSeconds(0.5f);

                        var task = sMonopolyShootAgain.DailyTask;
                        if (dailyTask.TaskRewardsGot != null && dailyTask.TaskRewardsGot.Items.Count > 0)
                        {
                            task = dailyTask;
                        }
                        
                        viewController.CheckCurrentLevelComplete(sMonopolyShootAgain.MonopolyRoundInfo,
                            sMonopolyShootAgain.RoundCompleteRewards, sMonopolyShootAgain.RoundListCompleteRewards,
                            () =>
                            {
                                _playerTr.gameObject.SetActive(true);
                                fireAgainCallback?.Invoke();
                            }, task, showBooster);
                    }
                    else
                    {
                        endCallback.Invoke(dailyTask);
                    }
                }, roundInfo.ShootAgainDiamondCost);
            }
        }

        private async void ShowCollectBoxAni(MonopolyGiftBox giftBoxReward, bool giftBoxFailedToPosition, uint giftBoxRewardPositionIndex, MonopolyRoundInfo roundInfo,
            MonopolyRoundInfo.Types.SpecialLattice currentLattices, Action<MonopolyDailyTask> endCallback, MonopolyDailyTask dailyTask)
        {
            var lattice = viewController.GetSpecialLatticeTransform((int) currentLattices.Index);

            if (giftBoxReward == null)
            {
                XDebug.LogError("-------------服务器数据错乱，配置显示这里有宝箱，但没有给宝箱数据！！！！！！！");
                return;
            }
            if (giftBoxFailedToPosition)
            {
                // 打开花费钻石开宝箱界面
                var useDiamondOpenChestView = await PopupStack.ShowPopup<TreasureRaidBoxFullNoticePopup>();
                useDiamondOpenChestView.SetMonopolyGiftBoxAndCallback(giftBoxReward, endCallback, dailyTask);
            }
            else
            {
                ShowInOutBoxListGroup(true);
                await XUtility.WaitSeconds(0.3f);
                // 播放收集动画
                var index = (int)giftBoxRewardPositionIndex;
                _chestBoxViews[index].ShowGetBoxAni(boxParticle, (int) giftBoxReward.Level, lattice, async () =>
                {
                    SetAllChestBoxPosition(roundInfo);

                    if (viewController.activityTreasureRaid != null && viewController.activityTreasureRaid.BeginnersGuideStep == 6)
                    {
                        viewController.ShowGuideStep7(endCallback, dailyTask);
                    }
                    else
                    {
                        endCallback.Invoke(dailyTask);
                    }

                    await XUtility.WaitSeconds(1f);
                    if (transform == null)
                        return;

                    boxParticle.gameObject.SetActive(false);
                });
            }
        }

        public override void Destroy()
        {
            if (flyCancelableCallback != null)
            {
                flyCancelableCallback.CancelCallback();
            }
            base.Destroy();
        }

        public void RefreshMasterUI(MonopolyRoundInfo monopolyRoundInfo, bool showAni = false)
        {
            var level = monopolyRoundInfo.SimpleInfo.RoundId > 6
                ? monopolyRoundInfo.SimpleInfo.RoundId % 6
                : monopolyRoundInfo.SimpleInfo.RoundId;
            level = level == 0 ? 6 : level;
            for (int i = 0; i < viewController.bossList.Count; i++)
            {
                viewController.bossList[i].gameObject.SetActive((i + 1) == (int) level);
            }
            var levelContainer = viewController.bossList[(int) level - 1].GetChild(0).Find("ContentGroup");
            var ani = levelContainer.GetComponent<Animator>();
            var percent = monopolyRoundInfo.BloodCurrent / (float) monopolyRoundInfo.BloodFull;
            var bossLevel = GetBossLevel(percent);

            if (showAni)
            {
                if (lastBossLevel != bossLevel)
                {
                    XUtility.PlayAnimation(ani, bossLevel.ToString());
                    lastBossLevel = bossLevel;
                }
                DOTween.To( value =>
                {
                    bossBloodText.SetText($"{(int)value}%");
                }, lastBlood, Mathf.Ceil(percent * 100), 0.3f);

                bossBloodProgress.DOValue(percent, 0.3f).OnComplete(
                    () =>
                    {
                        if (transform == null)
                            return;

                        bossBloodText.SetText($"{Mathf.Ceil(percent * 100)}%");
                    });
            }
            else
            {
                var aniName = bossLevel == 100 ? "100" : $"{bossLevel}_idle";
                XUtility.PlayAnimation(ani, aniName);
                bossBloodProgress.value = percent;
                bossBloodText.SetText($"{Mathf.Ceil(percent * 100)}%");
                lastBossLevel = bossLevel;
            }

            lastBlood = Mathf.Ceil(percent * 100);
        }

        private int GetBossLevel(float percent)
        {
            if (percent >= 0.7f)
            {
                return 100;
            }
            else if (percent >= 0.4f)
            {
                return 70;
            }
            else if (percent >= 0.01f)
            {
                return 60;
            }
            else
            {
                return 30;
            }
        }
        
        public void UpdateUserAvatar(uint avatarID)
        {
            if (rawImageAvatar != null)
            {
                rawImageAvatar.texture = AvatarController.defaultAvatar;

                AvatarController.GetSelfAvatar(avatarID, (t) =>
                {
                    var controller = Client.Get<UserController>();
                    if (rawImageAvatar != null && controller != null && avatarID == controller.GetUserAvatarID())
                    {
                        rawImageAvatar.texture = t;
                    }
                });
            }
        }
        
        public async void BeginTeleportAni(SMonopolyTeleport sMonopolyTeleport, int targetIndex)
        {
            SetBtnState(false);
            lastPlayerIndex = (uint) targetIndex;
            await XUtility.WaitSeconds(1f);
            // 开始播放传送门动画
            var targetPos = viewController.GetPlayPosInLattice(lastPlayerIndex);
            teleportParticle.position = targetPos;
            teleportParticle.gameObject.SetActive(true);

            XUtility.PlayAnimation(_playerAni, "fly");
            SoundController.PlaySfx("TreasureRaid_Send");
            await XUtility.WaitSeconds(1f);
            _playerTr.position = targetPos;
            await XUtility.WaitSeconds(0.5f);
            teleportParticle.gameObject.SetActive(false);
            CheckFlyEndAni(sMonopolyTeleport, () =>
            {
                SetBtnState(true);
            });
        }
        
        private void CheckFlyEndAni(SMonopolyTeleport sMonopolyTeleport, Action callback)
        {
            MonopolyRoundInfo.Types.SpecialLattice currentLattices = null;
            // 先判断当前格子是武器还是宝箱或者什么都没有
            foreach (var specialLattice in sMonopolyTeleport.MonopolyRoundInfo.SpecialLattices)
            {
                if (sMonopolyTeleport.MonopolyRoundInfo.CurrentLatticeIndex == specialLattice.Index)
                {
                    currentLattices = specialLattice;
                    break;
                }
            }

            // currentLattices 为空 表示在普通格子上
            if (currentLattices == null)
            {
                viewController.CheckCurrentLevelComplete(sMonopolyTeleport.MonopolyRoundInfo,
                    sMonopolyTeleport.RoundCompleteRewards, sMonopolyTeleport.RoundListCompleteRewards, callback,
                    sMonopolyTeleport.DailyTask, false, sMonopolyTeleport.RewardLatticeReward);
            }
            else
            {
                _playerTr.gameObject.SetActive(false);

                void ShowAniCallback(MonopolyDailyTask task)
                {
                    _playerTr.gameObject.SetActive(true);
                    viewController.CheckCurrentLevelComplete(sMonopolyTeleport.MonopolyRoundInfo,
                        sMonopolyTeleport.RoundCompleteRewards, sMonopolyTeleport.RoundListCompleteRewards, callback, task, false);
                }

                switch (currentLattices.Type)
                {
                    case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Cannon:
                        // 展示攻击动画
                        ShowAttackAni(sMonopolyTeleport.MonopolyRoundInfo, currentLattices, ShowAniCallback, callback,
                            sMonopolyTeleport.DailyTask, false);
                        break;
                    case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Giftbox:
                        // 展示获取宝箱奖励
                        ShowCollectBoxAni(sMonopolyTeleport.GiftBoxReward, sMonopolyTeleport.GiftBoxFailedToPosition,
                            sMonopolyTeleport.GiftBoxRewardPositionIndex, sMonopolyTeleport.MonopolyRoundInfo, currentLattices,
                            ShowAniCallback, sMonopolyTeleport.DailyTask);
                        break;
                }
            }
        }
    }

    public class TreasureRaidMainPopupController : ViewController<TreasureRaidMainPopup>
    {
        public SGetMonopolyRoundInfo sGetMonopolyRoundInfo;

        private List<Transform> lattices;

        public List<Transform> bossList;

        public Activity_TreasureRaid activityTreasureRaid;

        private TreasureRaidGuideStepView guideStep3View;
        private TreasureRaidGuideStepView guideStep4View;
        private TreasureRaidGuideStepView guideStep5View;
        private TreasureRaidGuideStepView guideStep6View;
        private TreasureRaidGuideStepView guideStep7View;
        private TreasureRaidGuideStepView guideStep10View;

        public override void OnViewDidLoad()
        {
            if (activityTreasureRaid != null)
            {
                activityTreasureRaid.InActivity = true;
            }
            base.OnViewDidLoad();
            view.UpdateUserAvatar(Client.Get<UserController>().GetUserAvatarID());
            view.teleportParticle.gameObject.SetActive(false);
        }

        /// <summary>
        /// 关卡完成后，并将所有奖励领取后，调用
        /// </summary>
        public async void CheckCurrentLevelComplete(MonopolyRoundInfo roundInfo, RepeatedField<Reward> roundCompleteRewards,
            RepeatedField<Reward> roundListCompleteRewards, Action callback, MonopolyDailyTask dailyTask, bool showBooster = true, Reward latticesReward = null)
        {
            var puzzleIndexOfMap = -1;
            if (latticesReward != null && latticesReward.Items.Count > 0)
            {
                var item = XItemUtility.GetItem(latticesReward.Items, Item.Types.Type.MonopolyPuzzleFragment);
                if (item != null)
                {
                    var lattice = GetPuzzleSpecialLattice(roundInfo.CurrentLatticeIndex);
                    if (lattice != null)
                    {
                        puzzleIndexOfMap = (int) lattice.IndexOfMap;
                        sGetMonopolyRoundInfo.MonopolyRoundInfo.RewardSpecialLattices.Remove(lattice);
                        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidGetpuzzle);
                    }
                }
            }
            if (puzzleIndexOfMap != -1)
            {
                if (lattices[puzzleIndexOfMap].childCount < 1)
                {
                    XDebug.LogError($"puzzleIndexOfMap:{puzzleIndexOfMap},没有生成对应的碎片");
                }
                else
                {
                    var puzzle = lattices[puzzleIndexOfMap].GetChild(0);
                    view.boxParticle.position = puzzle.position;
                    view.boxParticle.gameObject.SetActive(true);
                    GameObject.Destroy(puzzle.gameObject);
                    view.boxParticle.DOMove(view.puzzleGroup.position, 0.45f).OnComplete(() =>
                        {
                            view._puzzleView.RefreshPuzzle(true);
                            view.boxParticle.gameObject.SetActive(false);
                        }).SetEase(Ease.InQuad);
                    await WaitForSeconds(0.5f);
                }
            }
            if (roundCompleteRewards != null && roundCompleteRewards.Count > 0 )
            {
                // TODO 如果完成了关卡是不可能是获得碎片，这里就不考虑获得碎片的情况
                await WaitForSeconds(1f);
                var collectView = await PopupStack.ShowPopup<TreasureRaidCollectRewardPopup>();
                collectView.InitRewardContent(roundCompleteRewards);
                collectView.ShowRewardCollect( async () =>
                {
                    // 这中间有可能活动到期，到期了transform为空，就不执行后面的操作了
                    if (view.transform == null)
                        return;

                    await PopupStack.ShowPopup<TreasureRaidMapPopup>(null, new TreasureRaidData(roundInfo, roundListCompleteRewards, dailyTask));
                    view.SetNeedForceLandscapeScreen(false);
                    view.Close();
                });
                return;
            }

            await WaitForSeconds(0.2f);
            void EndCallback()
            {
                callback.Invoke();
                activityTreasureRaid.SetActivityState(false);
            }
            view._dailyTaskView.RefreshDailyTask(true, EndCallback, dailyTask, showBooster, puzzleIndexOfMap != -1);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
            SubscribeEvent<EventTreasureRaidRefreshChestBox>(RefreshAllChestBox);
            SubscribeEvent<EventActivityExpire>(OnCloseGuideStepView);
        }

        private void RefreshAllChestBox(EventTreasureRaidRefreshChestBox obj)
        {
            view.SetAllChestBoxPosition(obj.roundInfo);
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.SetNeedForceLandscapeScreen(true);
            SoundController.RecoverLastMusic();
            view.Close();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;

            base.BindingView(inView, inExtraData, inExtraAsyncData);
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
            sGetMonopolyRoundInfo = inExtraAsyncData as SGetMonopolyRoundInfo;
            if (sGetMonopolyRoundInfo == null)
            {
                XDebug.LogError("-----------sGetMonopolyRoundInfo is null, check your code");
                return;
            }
            view.lastPlayerIndex = sGetMonopolyRoundInfo.MonopolyRoundInfo.CurrentLatticeIndex;
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            bossList = new List<Transform>();
            for (int i = 1; i <= 6; i++)
            {
                bossList.Add(view.bossGroup.Find($"EnemyLevel{i}Group"));
            }
            view.SetAllChestBoxPosition(sGetMonopolyRoundInfo.MonopolyRoundInfo);
            view.RefreshMasterUI(sGetMonopolyRoundInfo.MonopolyRoundInfo);
            var levelId = sGetMonopolyRoundInfo.MonopolyRoundInfo.SimpleInfo.RoundId;
            levelId = levelId > 6 ? levelId % 6 : levelId;
            levelId = levelId == 0 ? 6 : levelId;
            view.roundText.SetText(levelId.ToString());
            view.pressNoticeTr.gameObject.SetActive(false);

            view.pathPointPrefab.gameObject.SetActive(false);
            lattices = new List<Transform>();
            for (int i = 0; i < 28; i++)
            {
                var path = view.latticeContainer.Find($"Path{i}");

                var specialLattice = GetSpecialLattice((uint)i);
                if (specialLattice != null)
                {
                    var cell = GameObject.Instantiate(view.pathPointPrefab, path, false);
                    UpdateSpecialLatticeUI(cell, specialLattice);
                    lattices.Add(cell);
                }
                else
                {
                    var puzzleLattice = GetPuzzleSpecialLattice((uint) i);
                    if (puzzleLattice != null)
                    {
                        var cell = GameObject.Instantiate(view.puzzlePrefab, path, false);
                        cell.localPosition = Vector3.zero;
                        cell.gameObject.SetActive(true);
                    }
                    lattices.Add(path);
                }
            }

            view._playerTr.position = GetPlayPosInLattice(view.lastPlayerIndex);
            var rewards = sGetMonopolyRoundInfo.MonopolyRoundInfo.SimpleInfo.CompleteRewards;
            var coinItem = XItemUtility.GetCoinItem(rewards[0]);
            var coins = (long) coinItem.Coin.Amount;
            view.rewardCoinText.SetText(coins.GetCommaOrSimplify(7));

            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.Coin);
            XItemUtility.InitItemsUI(view.extraRewardGroup, rewards[0].Items,
                view.extraRewardGroup.Find("TreasureRaidGameRewardCell"), null, "StandardType", skipList);

            view.CheckHasOpenBox();
            
            if (activityTreasureRaid != null)
            {
                if (activityTreasureRaid.BeginnersGuideStep == 2)
                    ShowGuideStep3();
                else if (activityTreasureRaid.BeginnersGuideStep == 3)
                    ShowGuideStep4();
                else if (activityTreasureRaid.BeginnersGuideStep == 4)
                    ShowGuideStep5();
                else if (activityTreasureRaid.BeginnersGuideStep == 5)
                    ShowGuideStep6();
            }
            
            var sks = view.transform.GetComponentsInChildren<SkeletonGraphic>();
            var noColor = new Color(1, 1, 1, 0);
            foreach (var sk in sks)
            {
                sk.color = noColor;
                sk.DOColor(Color.white, 0.28f);
            }
        }

        private static class LatticeName
        {
            public const string GiftBox = "BoxRewardType";
            public const string Weapon = "WeaponRewardType";
        }
        
        private void UpdateSpecialLatticeUI(Transform cell, MonopolyRoundInfo.Types.SpecialLattice lattice)
        {
            var childName = LatticeName.GiftBox;
            
            switch (lattice.Type)
            {
                case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Giftbox:
                    childName = LatticeName.GiftBox;
                    break;
                case MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Cannon:
                    childName = LatticeName.Weapon;
                    break;
            }

            var latticeTrParent = cell.GetChild(0);
            for (int i = 0; i < latticeTrParent.childCount; i++)
            {
                var child = latticeTrParent.GetChild(i);
                child.gameObject.SetActive(child.name == childName);
                if (child.name == childName)
                {
                    for (int j = 0; j < child.childCount; j++)
                    {
                        var itemContainer = child.GetChild(j);
                        itemContainer.gameObject.SetActive((j + 1) == lattice.Level);
                        if (childName == LatticeName.Weapon)
                        {
                            var weaponIndex = GetWeaponIndex((int)lattice.Index);
                            for (int k = 0; k < itemContainer.childCount; k++)
                            {
                                itemContainer.GetChild(k).gameObject.SetActive(k == weaponIndex);
                            }
                        }
                    }
                }
            }
            cell.gameObject.SetActive(true);
            cell.localPosition = Vector3.zero;
        }

        public async Task PlayLatticeAttackAni(Transform cell, MonopolyRoundInfo.Types.SpecialLattice lattice)
        {
            var childName = LatticeName.Weapon;
            if (lattice.Type == MonopolyRoundInfo.Types.SpecialLattice.Types.LatticeType.Giftbox)
                return;

            var boss = bossList[0];
            for (int i = 0; i < bossList.Count; i++)
            {
                if (bossList[i].gameObject.activeSelf)
                {
                    boss = bossList[i].GetChild(0);
                    break;
                }
            }
            var latticeTrParent = cell.GetChild(0);
            var weaponIndex = GetWeaponIndex((int)lattice.Index);
            var animator =
                latticeTrParent.Find(childName).GetChild((int) lattice.Level - 1).GetChild(weaponIndex)
                    .GetComponent<Animator>();
            var flyTr = animator.transform.Find("Fly");
            var flyImg = flyTr.GetChild(0);
            flyImg.localPosition = Vector3.zero;
            
            var targetPos = boss.transform.position;
            var jumpPower = 10f;
            if (weaponIndex == 3)
            {
                targetPos = flyImg.transform.position + new Vector3(25, -25, 0);
                jumpPower = 25f;
            }
            else if (weaponIndex == 1)
            {
                targetPos = flyImg.transform.position + new Vector3(-25, -25, 0);
                jumpPower = 25f;
            }
            else if (weaponIndex == 0)
            {
                targetPos = flyImg.transform.position + new Vector3(-25, 0, 0);
            }
            else
            {
                targetPos = flyImg.transform.position + new Vector3(25, 0, 0);
            }

            WaitForSeconds(0.8f, () =>
            {
                flyTr.gameObject.SetActive(true);
                flyImg.DOJump(targetPos, jumpPower, 1, 0.2f).OnComplete(() =>
                {
                    flyTr.gameObject.SetActive(false);
                });
            });
            SoundController.PlaySfx("TreasureRaid_Fire");
            XUtility.PlayAnimation(animator, "Fire");
            var ani = boss.GetComponent<Animator>();
            XUtility.PlayAnimation(ani, "hit");
            await WaitForSeconds(2f);
        }

        private int GetWeaponIndex(int latticeIndex)
        {
            if (latticeIndex <= 7)
                return 3;
            else if (latticeIndex <= 14)
                return 1;
            else if (latticeIndex <= 21)
                return 0;

            return 2;
        }
        private MonopolyRoundInfo.Types.SpecialLattice GetSpecialLattice(uint index)
        {
            foreach (var lattice in sGetMonopolyRoundInfo.MonopolyRoundInfo.SpecialLattices)
            {
                if (lattice.Index == index)
                {
                    return lattice;
                }
            }

            return null;
        }

        public MonopolyRoundInfo.Types.RewardSpecialLattice GetPuzzleSpecialLattice(uint index)
        {
            foreach (var lattice in sGetMonopolyRoundInfo.MonopolyRoundInfo.RewardSpecialLattices)
            {
                if (lattice.IndexOfMap == index)
                {
                    return lattice;
                }
            }

            return null;
        }
        
        public Transform GetSpecialLatticeTransform(int index)
        {
            return lattices[index];
        }

        public Vector3 GetPlayPosInLattice(uint index)
        {
            var pos = lattices[(int) index].position;
            if (GetSpecialLattice(index) != null)
            {
                pos += new Vector3(0, 5, 0);
            }
            return pos;
        }

        private void OnCloseGuideStepView(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.TreasureRaid)
                return;
            if (guideStep3View != null && guideStep3View.transform != null)
            {
                guideStep3View.Destroy();
            }
            if (guideStep4View != null && guideStep4View.transform != null)
            {
                guideStep4View.Destroy();
            }
            if (guideStep5View != null && guideStep5View.transform != null)
            {
                guideStep5View.Destroy();
            }
            if (guideStep6View != null && guideStep6View.transform != null)
            {
                guideStep6View.Destroy();
            }
            if (guideStep7View != null && guideStep7View.transform != null)
            {
                guideStep7View.Destroy();
            }
            if (guideStep10View != null && guideStep10View.transform != null)
            {
                guideStep10View.Destroy();
            }
        }
        
        private async void ShowGuideStep3()
        {
            if (view._playerTr.gameObject.activeSelf)
            {
                view._playerTr.gameObject.SetActive(false);
            }
            view.SetBtnState(false);
            guideStep3View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide3", view.transform);
            view.SetBtnState(true);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            view.AdaptScaleTransform(guideStep3View.transform.Find("Root"), new Vector2(1265, 606));
            var canvas = view.bossGroup.transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            var components = new List<Component>();
            components.Add(canvas);
            guideStep3View.SetGuideClickHandler( async () =>
            {
                ToGuideStep4();
            },components);
        }

        private async void ToGuideStep4()
        {
            ViewManager.Instance.BlockingUserClick(true, "UITreasureRaidGuide3");
            activityTreasureRaid.IncreaseGuideStep(null);
            ViewManager.Instance.BlockingUserClick(false, "UITreasureRaidGuide3");
            ShowGuideStep4();
        }

        private async void ShowGuideStep4()
        {
            if (view._playerTr.gameObject.activeSelf)
            {
                view._playerTr.gameObject.SetActive(false);
            }
            view.SetBtnState(false);
            guideStep4View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide4", view.transform);
            view.SetBtnState(true);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            view.AdaptScaleTransform(guideStep4View.transform.Find("Root"), new Vector2(1265, 606));
            guideStep4View.transform.Find("Root/BubbleGroup").position = view.titleGroup.transform.position - new Vector3(0, 10f, 0);
            var canvas = view.titleGroup.transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            
            var root = guideStep4View.transform.Find("Root");
            var rootCanvas = root.gameObject.AddComponent<Canvas>();
            rootCanvas.overrideSorting = true;
            rootCanvas.sortingOrder = 11;
            rootCanvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            
            var components = new List<Component>();
            components.Add(canvas);
            components.Add(rootCanvas);
            guideStep4View.SetGuideClickHandler( async () =>
            {
                ToGuideStep5();
            }, components);
        }

        private void ToGuideStep5()
        {
            ViewManager.Instance.BlockingUserClick(true, "UITreasureRaidGuide4");
            activityTreasureRaid.IncreaseGuideStep(null);
            ViewManager.Instance.BlockingUserClick(false, "UITreasureRaidGuide4");
            ShowGuideStep5();
        }

        private async void ShowGuideStep5()
        {
            if (view._playerTr.gameObject.activeSelf)
            {
                view._playerTr.gameObject.SetActive(false);
            }
            view.SetBtnState(false);
            guideStep5View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide5", view.transform);
            view.SetBtnState(true);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            view.AdaptScaleTransform(guideStep5View.transform.Find("Root"), new Vector2(1265, 606));
            guideStep5View.SetGuideClickHandler(ToGuideStep6);
        }

        private void ToGuideStep6()
        {
            ViewManager.Instance.BlockingUserClick(true, "UITreasureRaidGuide5");
            activityTreasureRaid.IncreaseGuideStep(null);
            EventBus.Dispatch(new EventActivityServerDataUpdated(ActivityType.TreasureRaid));
            ViewManager.Instance.BlockingUserClick(false, "UITreasureRaidGuide5");
            ShowGuideStep6();
        }

        private async void ShowGuideStep6()
        {
            if (view._playerTr.gameObject.activeSelf)
            {
                view._playerTr.gameObject.SetActive(false);
            }
            view.SetBtnState(false);
            guideStep6View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide6", view.transform);
            view.SetBtnState(true);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            var guideRoot = guideStep6View.transform.Find("Root");
            guideStep6View.transform.Find("Root/BubbleGroup").position = view.wheelGroup.transform.position - new Vector3(10f, 0, 0);

            view.AdaptScaleTransform(guideRoot, new Vector2(1465, 606));
            var canvas = view.wheelGroup.transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");

            var guideRootCanvas = guideRoot.gameObject.AddComponent<Canvas>();
            guideRootCanvas.overrideSorting = true;
            guideRootCanvas.sortingOrder = 11;
            guideRootCanvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            var components = new List<Component>();
            components.Add(canvas);
            guideStep6View.SetGuideClickHandler( async () =>
            {
                ViewManager.Instance.BlockingUserClick(true, "UITreasureRaidGuide6");
                activityTreasureRaid.IncreaseGuideStep(null);
                view.pressNoticeTr.gameObject.SetActive(true);
                view._playerTr.gameObject.SetActive(true);
                ViewManager.Instance.BlockingUserClick(false, "UITreasureRaidGuide6");
            }, components);
        }
        
        public async void ShowGuideStep7(Action<MonopolyDailyTask> endCallback, MonopolyDailyTask dailyTask)
        {
            guideStep7View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide7", view.transform);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            guideStep7View.transform.SetAsLastSibling();
            view.AdaptScaleTransform(guideStep7View.transform.Find("Root"), new Vector2(1400, 606));
            var canvas = view.boxListGroup.transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            var components = new List<Component>();
            components.Add(canvas);
            guideStep7View.SetGuideClickHandler( async () =>
            {
                ViewManager.Instance.BlockingUserClick(true, "UITreasureRaidGuide7");
                activityTreasureRaid.IncreaseGuideStep(null);
                ViewManager.Instance.BlockingUserClick(false, "UITreasureRaidGuide7");
                endCallback.Invoke(dailyTask);
            }, components);
        }
        
        public async void ShowGuideStep10()
        {
            view.SetBtnState(false);
            guideStep10View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide10", view.transform);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            var guideRoot = guideStep10View.transform.Find("Root");
            // guideStep10View.transform.Find("Root/BubbleGroup").position = view.wheelGroup.transform.position - new Vector3(10f, 0, 0);

            view.AdaptScaleTransform(guideRoot, new Vector2(1465, 606));
            var canvas = view.puzzleGroup.transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;
            guideRoot.position = view.puzzleGroup.position - new Vector3(0, 10, 0);
            var guideRootCanvas = guideRoot.gameObject.AddComponent<Canvas>();
            guideRootCanvas.overrideSorting = true;
            guideRootCanvas.sortingOrder = 11;
            guideRootCanvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            var components = new List<Component>();
            components.Add(canvas);
            guideStep10View.SetGuideClickHandler(() =>
            {
                view.SetBtnState(true);
            }, components);
        }
    }
}