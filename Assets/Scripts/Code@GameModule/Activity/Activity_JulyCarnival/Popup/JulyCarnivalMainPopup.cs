using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class JulyCarnivalRewardContentView : View
    {
        [ComponentBinder("FreeRewardGroup")] private Transform freeRewardContent;
        [ComponentBinder("ValentineGroup")] private Transform payRewardContent;
        [ComponentBinder("LockState/Lock")] private Animator lockAni;

        [ComponentBinder("LevelPoint/StateGroup/LevelText/LevelText")]
        private TextMeshProUGUI levelText;

        private SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward _stepReward;
        public void SetViewContent(SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward stepReward)
        {
            _stepReward = stepReward;
            InitUI();
        }

        private void InitUI()
        {
            levelText.SetText(_stepReward.Step.ToString());
            XItemUtility.InitItemsUI(freeRewardContent, _stepReward.NormalReward.Items,
                freeRewardContent.Find("UIValentinesDayRewardCell_FinishState"), null, "StandarType");
            XItemUtility.InitItemsUI(payRewardContent, _stepReward.SpecialReward.Items,
                payRewardContent.Find("UIValentinesDayRewardCell_FinishState"), null, "StandarType");
        }

        public void RefreshRewardStatus(SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward stepReward, bool bIsPaid)
        {
            _stepReward = stepReward;
            // var bLock = _stepReward.SpecialRewardStatus == SGetIndependenceDayMainPageInfo.Types
            //     .IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Locked;
            lockAni.Play(bIsPaid ? "UnLock" : "Lock");
            // 刷新reward 领取状态
            for (int i = 0; i < freeRewardContent.childCount; i++)
            {
                var item = freeRewardContent.GetChild(i).GetComponent<Animator>();
                var collect = stepReward.NormalRewardStatus == SGetIndependenceDayMainPageInfo.Types
                    .IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Received;
                
                item.Play(collect ? "idle" : "Collect");
            }
            
            for (int i = 0; i < payRewardContent.childCount; i++)
            {
                var item = payRewardContent.GetChild(i).GetComponent<Animator>();
                var collect = stepReward.SpecialRewardStatus == SGetIndependenceDayMainPageInfo.Types
                    .IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Received;
                item.Play(collect ? "idle" : "Collect");
            }
        }

        public void ShowUnlockAni()
        {
            lockAni.Play( "UnLock");
        }

        public void ShowRewardCollectAni(SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward stepReward, bool bIsPaid)
        {
            _stepReward = stepReward;
            if (lockAni.GetCurrentAnimatorStateInfo(0).IsName("Lock"))
            {
                lockAni.Play(bIsPaid ? "UnLock" : "Lock");
            }

            // 刷新reward 领取状态
            for (int i = 0; i < freeRewardContent.childCount; i++)
            {
                var item = freeRewardContent.GetChild(i).GetComponent<Animator>();
                if (_stepReward.NormalRewardStatus == SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Received
                    && !item.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    item.Play("Show");
                }
            }
            
            for (int i = 0; i < payRewardContent.childCount; i++)
            {
                var item = payRewardContent.GetChild(i).GetComponent<Animator>();
                if (_stepReward.SpecialRewardStatus == SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Received
                    && !item.GetCurrentAnimatorStateInfo(0).IsName("idle"))
                {
                    item.Play("Show");
                }
            }
        }
    }
    
    [AssetAddress("UIIndependenceDayMain", "UIIndependenceDayMainV")]
    public class JulyCarnivalMainPopup : Popup<JulyCarnivalMainPopupController>
    {
        [ComponentBinder("Root/MainGroup/AdaptorGroup/start")]
        public Transform startTr;

        [ComponentBinder("Root/MainGroup/AdaptorGroup/LevelReward/RewardGroup")]
        private Transform rewardContent;

        [ComponentBinder("Root/MainGroup/AdaptorGroup/LevelReward/PriceButton")]
        private Button buyBtn;
        
        [ComponentBinder("Root/MainGroup/AdaptorGroup/LevelReward/PriceButton/ClickMask")]
        private Transform btnMask;
        
        [ComponentBinder("Root/MainGroup/AdaptorGroup/LevelReward/ExtraContentsButton")]
        private Button benefitBtn;
        
        [ComponentBinder("Root/MainGroup/TopGroup/HelpButton")]
        private Button helpBtn;
        [ComponentBinder("Root/MainGroup/TopGroup/CloseButton")]
        private Button closeBtn;

        [ComponentBinder("Root/MainGroup/AdaptorGroup/LevelPointGroup")]
        private Transform pointGroup;

        [ComponentBinder("Root/MainGroup/AdaptorGroup/TimerGroup/TimerText")]
        private Text leftTimeText;
        [ComponentBinder("Root/MainGroup/AdaptorGroup/qiqiu/L/L2/L2/LevelText")]
        private Text levelText;
        
        [ComponentBinder("Root/MainGroup/AdaptorGroup/LevelReward/PriceButton/PriceText")]
        private Text priceText;

        private List<Transform> levelPoints;

        private JulyCarnivalStarShow _starShowView;

        private List<JulyCarnivalRewardContentView> _rewardContentViews;

        public JulyCarnivalMainPopup(string address)
            : base(address)
        {
            // contentDesignSize = new Vector2(1350, 768);
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdaptScaleTransform(transform.Find("Root/MainGroup/AdaptorGroup"), new Vector2(1350, 768));
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
            buyBtn.onClick.AddListener(OnBuyBtnClicked);
            benefitBtn.onClick.AddListener(OnBenefitBtnClicked);
            helpBtn.onClick.AddListener(OnHelpBtnClicked);
            closeBtn.onClick.AddListener(OnCloseClicked);
            SoundController.PlayBgMusic("IndependenceBGM");
        }

        public override void Close()
        {
            base.Close();
            SoundController.RecoverLastMusic();
        }

        public void SetBtnStatus(bool interactable)
        {
            buyBtn.interactable = interactable;
            benefitBtn.interactable = interactable;
            helpBtn.interactable = interactable;
            closeBtn.interactable = interactable;
            btnMask.gameObject.SetActive(!interactable);
        }

        private void OnHelpBtnClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalHelpPopup))));
        }

        private async void OnBenefitBtnClicked()
        {
            SetBtnStatus(false);
            // show benefit
            ShopItemConfig shopItemConfig = viewController.currentData.PayItem;
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
            SetBtnStatus(true);
        }

        private void OnBuyBtnClicked()
        {
            SetBtnStatus(false);
            // 内购逻辑
            Client.Get<IapController>().BuyProduct(viewController.currentData.PayItem);
            SetBtnStatus(true);
        }

        public void InitUI()
        {
            _rewardContentViews = new List<JulyCarnivalRewardContentView>();
            var rewards = viewController.lastData.IndependenceRewards;
            var index = 0;
            for (int i = rewardContent.childCount - 1; i >= 0; i--)
            {
                if (i >= rewards.Count) 
                    continue;

                var rewardItem = rewardContent.GetChild(i);
                var stepRewardView = AddChild<JulyCarnivalRewardContentView>(rewardItem);
                stepRewardView.SetViewContent(rewards[index]);
                _rewardContentViews.Add(stepRewardView);
                index++;
            }

            levelPoints = new List<Transform>();
            for (int i = pointGroup.childCount - 1; i >= 0; i--)
            {
                var levelPoint = pointGroup.GetChild(i);
                levelPoints.Add(levelPoint);
            }

            var level = viewController.lastData.Step > viewController.lastData.StepMax
                ? viewController.lastData.StepMax
                : viewController.lastData.Step;
            levelText.SetText($"{level}/{viewController.lastData.StepMax}");
            priceText.SetText($"$ {viewController.lastData.PayItem.Price}");
        }

        public void HideBuyBtn()
        {
            buyBtn.gameObject.SetActive(false);
            benefitBtn.gameObject.SetActive(false);
        }

        public void RefreshUI()
        {
            buyBtn.gameObject.SetActive(!viewController.lastData.PaymentAlreadyPaid);
            benefitBtn.gameObject.SetActive(!viewController.lastData.PaymentAlreadyPaid);
            for (int i = 0; i < levelPoints.Count; i++)
            {
                var levelPoint = levelPoints[i];
                var ani = levelPoint.GetComponent<Animator>();
                var playName = i + 1 > (int) viewController.lastData.Step ? "await" : "Collect_idle";
                ani.Play(playName);
                if (viewController.CheckIsBigStepReward(i))
                {
                    // 大节点ui
                    levelPoint.Find("LevelText").GetComponent<TextMeshProUGUI>().SetText($"{i + 1}");
                }
                else
                {
                    // 小节点UI 
                }
            }

            for (int i = 0; i < _rewardContentViews.Count; i++)
            {
                _rewardContentViews[i].RefreshRewardStatus(viewController.lastData.IndependenceRewards[i], viewController.lastData.PaymentAlreadyPaid);
            }
        }

        public void ShowUnlockAni()
        {
            buyBtn.gameObject.SetActive(false);
            benefitBtn.gameObject.SetActive(false);
            foreach (var t in _rewardContentViews)
            {
                t.ShowUnlockAni();
            }
        }

        /// <summary>
        /// 收集页面点击收集后做动画
        /// </summary>
        /// <param name="pageInfo"></param>
        public async void ShowCollectRewardAni(SGetIndependenceDayMainPageInfo pageInfo)
        {
            viewController.lastData = pageInfo;
            viewController.currentData = pageInfo;
            buyBtn.gameObject.SetActive(!viewController.lastData.PaymentAlreadyPaid);
            benefitBtn.gameObject.SetActive(!viewController.lastData.PaymentAlreadyPaid);
            for (int i = 0; i < _rewardContentViews.Count; i++)
            {
                _rewardContentViews[i].ShowRewardCollectAni(pageInfo.IndependenceRewards[i], pageInfo.PaymentAlreadyPaid);
            }

            await XUtility.WaitSeconds(1);
            SetBtnStatus(true);
            viewController.ActivityJulyCarnival.SetActivityState(false);
        }

        public void UpdateLeftTime()
        {
            leftTimeText.SetText(XUtility.GetTimeText(viewController.ActivityJulyCarnival.GetCountDown()));
        }

        public async void BeginShowAni()
        {
            SetBtnStatus(false);
            if (_starShowView == null)
            {
                _starShowView = await AddChild<JulyCarnivalStarShow>();
                _starShowView.SetViewContent((int)viewController.currentData.Step - (int)viewController.lastData.Step);
            }

            await XUtility.WaitSeconds(2);
            _starShowView.Hide();

            // 展示获得过程动画
            for (int i = (int)viewController.lastData.Step; i < (int)viewController.currentData.Step; i++)
            {
                if (i >= levelPoints.Count)
                    break;
                var ani = levelPoints[i].GetComponent<Animator>();
                await XUtility.PlayAnimationAsync(ani, "Collect");
                levelText.SetText($"{i+1}/{viewController.lastData.StepMax}");
            }
            // check reward
            if (CheckHasRewardToCollect())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalRewardCollectPopup), viewController.currentData)));
            }
            else
            {
                SetBtnStatus(true);
            }
        }

        public bool CheckHasRewardToCollect()
        {
            for (int i = 0; i < viewController.currentData.IndependenceRewards.Count; i++)
            {
                var reward = viewController.currentData.IndependenceRewards[i];
                if (reward.NormalRewardStatus == 
                    SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Unlocked 
                    || reward.SpecialRewardStatus == 
                    SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Unlocked )
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class JulyCarnivalMainPopupController : ViewController<JulyCarnivalMainPopup>
    {
        public SGetIndependenceDayMainPageInfo currentData;

        public SGetIndependenceDayMainPageInfo lastData;

        public Activity_JulyCarnival ActivityJulyCarnival;

        private bool showAni = false;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            ActivityJulyCarnival =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.JulyCarnival) as
                    Activity_JulyCarnival;
            if (inExtraData != null)
            {
                if (inExtraData is PopupArgs args)
                {
                    if (args.extraArgs != null)
                    {
                        showAni = (bool) args.extraArgs;
                    }
                }
            }
            if (ActivityJulyCarnival == null)
            {
                XDebug.LogError("No Activity Independence Day!!!!");
                return;
            }

            currentData = inExtraAsyncData as SGetIndependenceDayMainPageInfo;
            lastData = ActivityJulyCarnival?.GetIndependenceDayMainPageInfo();
            if (lastData == null)
            {
                lastData = currentData;
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityExpire>(OnEventActivityExpire);
        }

        private void OnEventActivityExpire(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.JulyCarnival)
                return;
            view.Close();
        }
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.InitUI();
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            if (currentData.Step >= 1)
            {
                view.startTr.gameObject.SetActive(false);
            }
            view.UpdateLeftTime();
            EnableUpdate(1);
            view.RefreshUI();
            if (showAni && ((int) currentData.Step - (int) lastData.Step) > 0)
            {
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventFourthofjulyProgress,
                    ("step", $"{(int) currentData.Step - (int) lastData.Step}"),
                    ("source", $"{ActivityJulyCarnival.itemSource}"));

                // 把所有的按钮不可点击后，展示升级动画
                view.BeginShowAni();
            }
            else
            {
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventFourthofjulyEnter,
                    ("source", $"{view.GetTriggerSource()}"));
                //  这里处理短线重连的情况。有可能玩家在获取到奖励之前断线
                if (view.CheckHasRewardToCollect())
                {
                    view.SetBtnStatus(false);
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalRewardCollectPopup), currentData)));
                }
            }
        }

        public override void Update()
        {
            view.UpdateLeftTime();
        }

        public override void OnViewDestroy()
        {
            ActivityJulyCarnival.SetIndependenceDayMainPageInfo(currentData);
            ActivityJulyCarnival.SetActivityState(false);
            base.OnViewDestroy();
        }

        public bool CheckIsBigStepReward(int index)
        {
            for (int i = 0; i < lastData.IndependenceRewards.Count; i++)
            {
                if (((int)lastData.IndependenceRewards[i].Step - 1) == index)
                {
                    return true;
                }
            }
            return false;
        }
    }
}