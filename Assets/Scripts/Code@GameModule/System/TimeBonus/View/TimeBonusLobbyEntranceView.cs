// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/08/11:25
// Ver : 1.0.0
// Description : TimeBonusLobbyEntranceView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class CoinBonusLobbyEntranceView : View<CoinBonusLobbyEntranceViewController>
    {
        [ComponentBinder("ProgressBar")] public Slider progressBar;

        [ComponentBinder("CoinCollectButton")] public Button coinCollectButton;

        [ComponentBinder("AdsCollectButton")] public Button adsCollectButton;
       
        [ComponentBinder("MaskProgress")] public Slider maskProgress;

        [ComponentBinder("IntegralText")] public TMP_Text integralText;

        [ComponentBinder("IconGroup/IntegralGroup/Icon")]
        public Transform coinIcon;

        public Button mainButton;
        
    }

    public class CoinBonusLobbyEntranceViewController : ViewController<CoinBonusLobbyEntranceView>
    {
        private TimeBonusController _timeBonusController;

        public float intervalTime;
        protected int countDownTime;

        protected bool isClaimingBonus = false;

        protected Animator animator;

        private bool isFull = false;
        private bool isAdModel = false;
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            animator = view.transform.GetComponent<Animator>();
            
            animator.keepAnimatorControllerStateOnDisable = true;
            
            view.mainButton = view.transform.GetComponent<Button>();

            view.mainButton.onClick.AddListener(OnMainButtonClicked);
            view.coinCollectButton.onClick.AddListener(OnCollectButtonClicked);
            view.adsCollectButton.onClick.AddListener(OnAdCollectButtonClicked);
            view.adsCollectButton.gameObject.SetActive(false);
            
            _timeBonusController = Client.Get<TimeBonusController>();
            
            countDownTime = AdController.Instance.adModel.GetArg1(eAdReward.InfinityVault);;
            intervalTime = AdController.Instance.adModel.GetRewardedVideoCDinSeconds(eAdReward.InfinityVault);

            countDownTime = 30;
            intervalTime = 60;

            if (_timeBonusController.lastTimeShowValultRV == 0)
            {
                _timeBonusController.lastTimeShowValultRV = Time.realtimeSinceStartup - intervalTime;
            }
            
            EnableUpdate();
        }
         
        public bool CanShowRvButton()
        {
            if (Time.realtimeSinceStartup - _timeBonusController.timeCheckShowRv < 10)
                return false;

            _timeBonusController.timeCheckShowRv = Time.realtimeSinceStartup;
            
            if (intervalTime - (Time.realtimeSinceStartup - _timeBonusController.lastTimeShowValultRV) <= 0)
            {
                return AdController.Instance.ShouldShowRV(eAdReward.InfinityVault, false);
            }
            
            return false;
        }
        public float GetCountDown()
        {
            if (_timeBonusController.lastTimeShowValultRV <= 0)
                return -1;
            
            if(_timeBonusController.lastWatchedTime > _timeBonusController.lastTimeShowValultRV)
                return 0;
            
            return countDownTime - (Time.realtimeSinceStartup - _timeBonusController.lastTimeShowValultRV);
        }

        public void OnMainButtonClicked()
        {
            PopupStack.ShowPopupNoWait<TimeBonusMainPopup>();
        }

        public void ToggleButton(bool interactable)
        {
            view.coinCollectButton.interactable = interactable;
            view.adsCollectButton.interactable = interactable;
            view.mainButton.interactable = interactable;
            
            if (view.coinCollectButton.gameObject.activeInHierarchy)
            {
                var buttonAnimation = view.coinCollectButton.GetComponent<Animator>();
                buttonAnimation.Play(interactable ? "Loop":"Normal");
            }
        }

        public void OnCollectButtonClicked()
        {
            isClaimingBonus = true;
            ClaimCoinBonus(false);
        }

        public void ClaimCoinBonus(bool watchedRv)
        {
            ToggleButton(false);
            DisableUpdate();
            
            var currentCoin = _timeBonusController.GetCurrentCoin();
            var progress = _timeBonusController.GetCoinBonusProgress(currentCoin);

            var accLevel = (int)(progress / 0.25f); 
            
            _timeBonusController.ClaimTimeBonus(async item =>
            {
                ulong spinBuffLevel = 0;
                var spinBuff = Client.Get<BuffController>().GetBuff<TimerbonusSpinBuff>();
               
                if (spinBuff != null)
                {
                    spinBuffLevel = spinBuff.spinBuffLevel;
                }
               
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventTimerBonusHourlyCollect, ("source", "Lobby"),
                    ("accLevel",accLevel.ToString()),("SpinBuffLevel", spinBuffLevel.ToString()));
                    
                    
                if (item != null)
                {
                    await XUtility.FlyCoins(view.coinIcon.transform,
                        new EventBalanceUpdate(item.Coin.Amount, "HourlyBonus"));

                    ToggleButton(true);
                    EnableUpdate();
                   
                    animator.Play("Wait");
                    // OnTimeBonusStateChanged(new EventTimeBonusStateChanged());
                }
                else
                {
                    CommonNoticePopup.ShowCommonNoticePopUp("Unknown error Occured! Please try again later");
                }
                
                isClaimingBonus = false; 
            }, watchedRv);
        }

        public void OnAdCollectButtonClicked()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.InfinityVault, false))
            {
                isClaimingBonus = true;
                AdController.Instance.TryShowRewardedVideo(eAdReward.InfinityVault, OnWatchRvFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
        }

        public void OnWatchRvFinished(bool success, string reason)
        {
            if (success)
            {
                ClaimCoinBonus(true);
                _timeBonusController.lastWatchedTime = Time.realtimeSinceStartup;
            }
            else
            {
                ToggleButton(true);
            }
        }
        
        public override void Update()
        {
            if (!view.IsActive())
                return;
            
            var currentCoin = _timeBonusController.GetCurrentCoin();
            view.integralText.text = currentCoin.GetCommaFormat();
            view.progressBar.value = _timeBonusController.GetCoinBonusProgress(currentCoin);
            
            CheckNeedSwitchToFull();

            var countDown = GetCountDown();
          
            if (!isAdModel && (CanShowRvButton() || countDown > 0))
            {
                view.adsCollectButton.gameObject.SetActive(true);
                view.coinCollectButton.gameObject.SetActive(false);
                if(countDown <= 0)
                    _timeBonusController.lastTimeShowValultRV = Time.realtimeSinceStartup;
                isAdModel = true;
            }
            else if (isAdModel)
            {
                if (countDown > 0)
                {
                    view.maskProgress.value = countDown / countDownTime;
                }
                else
                {
                    view.adsCollectButton.gameObject.SetActive(false);
                    view.coinCollectButton.gameObject.SetActive(true);
                    isAdModel = false;
                }
            }
        }
        
        public void CheckNeedSwitchToFull(bool force = false)
        {
            var currentCoin = _timeBonusController.GetCurrentCoin();
            var progress = _timeBonusController.GetCoinBonusProgress(currentCoin);
            var nowIsFull = progress >= 1;
        
            if (!isFull && nowIsFull || (force && nowIsFull))
            {
                animator.Play("Full");
            }
        }
        public void GradualAppear()
        {
            view.Show();
            
            view.transform.GetComponent<CanvasGroup>().alpha = 0;
            view.transform.GetComponent<CanvasGroup>().DOFade(1, 1f).OnComplete(() => {  });
        }

        public void GradualHide()
        {
            view.transform.GetComponent<CanvasGroup>().alpha = 1;
            view.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => { view.Hide(); });
        }

        public bool CanGradualHide()
        {
            if (view.adsCollectButton.gameObject.activeInHierarchy)
            {
                return false;
            }
            
            return !isClaimingBonus;
        }
    }

    public class WheelBonusLobbyEntranceView : View<WheelBonusLobbyEntranceViewController>
    {
        [ComponentBinder("SpinButton")] public Button spinButton;

        [ComponentBinder("AdsButton")] public Button adsButton;

        [ComponentBinder("ProgressGroup/SuperWheelFillGroup")]
        public Transform superWheelFillGroup;

        [ComponentBinder("InformationGroup/Text")]
        public TMP_Text countDownText;

        public Button mainButton;
    }

    public class WheelBonusLobbyEntranceViewController : ViewController<WheelBonusLobbyEntranceView>
    {
        private TimeBonusController _timeBonusController;

        protected Animator animator;
        
        private bool isMultiWheel = false;

        private string collectAnimationName = "LuckyWheelCollectable";
        private string countDownAnimationName = "LucyWheelCountDown";
        private string adSpeedUpAnimationName = "LuckyWheelRvSpeedUp";
        private string countDownPrefx = "LUCKY WHEEL ";

        private bool canGradualHide = true; 
 
        public void SetIsMultiWheel(bool inIsMultiWheel)
        {
            isMultiWheel = inIsMultiWheel;
            if (isMultiWheel)
            {
                collectAnimationName = "MultWheelCollectable";
                countDownAnimationName = "MultWheelCountDown";
                adSpeedUpAnimationName = "MultWheelRvSpeedUp";
                countDownPrefx = "MULTI WHEEL ";
            }

            UpdateBonusState(false);
            
        }
 
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventTimeBonusStateChanged>(OnBonusStateChanged); 
        }

        protected void OnBonusStateChanged(EventTimeBonusStateChanged evt)
        {
            UpdateBonusState(false);
            canGradualHide = true;
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.mainButton = view.transform.GetComponent<Button>();

            view.mainButton.onClick.AddListener(OnMainButtonClicked);
            view.adsButton.onClick.AddListener(OnAdsButtonClick);
            view.spinButton.onClick.AddListener(OnSpinButtonClicked);

            _timeBonusController = Client.Get<TimeBonusController>();

            animator = view.transform.GetComponent<Animator>();

            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public void OnMainButtonClicked()
        {
            PopupStack.ShowPopupNoWait<TimeBonusMainPopup>();
        }

        public void UpdateToggleState()
        {
            var currentIndex = _timeBonusController.GetWheelBonusProgress();

            for (var i = 0; i < 4; i++)
            {
                var child = view.superWheelFillGroup.GetChild(i);
                child.Find("FinishState").gameObject.SetActive(i < currentIndex);
            }
        }

        public void UpdateBonusState(bool animation)
        {
            if (_timeBonusController.IsBonusReady())
            {
                if(animator.gameObject.activeInHierarchy)
                    animator.Play(collectAnimationName);
                view.spinButton.transform.Find("ClickMask").gameObject.SetActive(false);
            }
            else
            {
                if(animator.gameObject.activeInHierarchy)
                    animator.Play(countDownAnimationName);

                if(!updateEnabled)
                    EnableUpdate(2);
            }

            UpdateToggleState();
        }

        public void ToggleButton(bool interactable)
        {
            view.spinButton.interactable = interactable;
            view.adsButton.interactable = interactable;
            view.mainButton.interactable = interactable;
            
        }

        public void OnAdsButtonClick()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.LuckyWheel, false))
            {
                canGradualHide = false;
                AdController.Instance.TryShowRewardedVideo(eAdReward.LuckyWheel, OnWatchRvFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
        }

        public void OnWatchRvFinished(bool success, string reason)
        {
            if (success)
            {
                //TODO request reduce cd
                _timeBonusController.RequestSlowDownCd(() =>
                {
                    UpdateBonusState(false);
                    canGradualHide = true;
                });
            }
            else
            {
                ToggleButton(true);
            }
        }

        public void OnSpinButtonClicked()
        {
            SoundController.PlayButtonClick();
            ToggleButton(false);
            canGradualHide = false;
           
            if (!isMultiWheel)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusWheelBonusPopup), "Lobby")));
            }
            else
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusSuperWheelPopup), "Lobby")));
            }
        }

        public override void Update()
        {
            if (_timeBonusController.GetWheelBonusCountDown() <= 0)
            {
                DisableUpdate();
                animator.Play(collectAnimationName);
            }
            else
            {
                view.countDownText.text =
                    countDownPrefx + XUtility.GetTimeText(_timeBonusController.GetWheelBonusCountDown());
            }
        }
        
        public void GradualAppear()
        {
            view.Show();
            
            UpdateBonusState(false);
            
            view.transform.GetComponent<CanvasGroup>().alpha = 0;
            view.transform.GetComponent<CanvasGroup>().DOFade(1, 1).OnComplete(() => { });
        }

        public void GradualHide()
        {
            view.transform.GetComponent<CanvasGroup>().alpha = 1;
            view.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => { view.Hide(); });
        }

        public bool CanGradualHide()
        {
            return canGradualHide;
        }
    }
    
    public class TimeBonusLobbyEntranceView : View<TimeBonusLobbyEntranceViewController>
    {
        [ComponentBinder("CoinBonusGroup")]
        public Transform coinBonusGroup;
        [ComponentBinder("LuckyWheelBonusGroup")] 
        public Transform luckyWheelBonusGroup;
        [ComponentBinder("MultWheelBonusGroup")] 
        public Transform multWheelBonusGroup;

        protected override void EnableView()
        {
            gameObject.SetActive(true);
            if(gameObject.activeInHierarchy)
                viewController.OnViewEnabled();
        }
    }

    public class TimeBonusLobbyEntranceViewController : ViewController<TimeBonusLobbyEntranceView>
    {
        protected CoinBonusLobbyEntranceView coinBonusLobbyEntranceView;
        protected WheelBonusLobbyEntranceView luckyWheelBonusLobbyEntranceView;
        protected WheelBonusLobbyEntranceView multWheelBonusLobbyEntranceView;

        protected float lastSwitchTime = 0;
        protected float switchInterval = 5;

        protected TimeBonusController _timeBonusController;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
       
            coinBonusLobbyEntranceView = view.AddChild<CoinBonusLobbyEntranceView>(view.coinBonusGroup);
            luckyWheelBonusLobbyEntranceView = view.AddChild<WheelBonusLobbyEntranceView>(view.luckyWheelBonusGroup);
            multWheelBonusLobbyEntranceView = view.AddChild<WheelBonusLobbyEntranceView>(view.multWheelBonusGroup);
            luckyWheelBonusLobbyEntranceView.viewController.SetIsMultiWheel(false);
            multWheelBonusLobbyEntranceView.viewController.SetIsMultiWheel(true);

            _timeBonusController = Client.Get<TimeBonusController>();
             
            lastSwitchTime = Time.realtimeSinceStartup;
            
            luckyWheelBonusLobbyEntranceView.Hide();
            multWheelBonusLobbyEntranceView.Hide();

            CheckAndStartSwitch();
        }

        public override void OnViewEnabled()
        {
            coinBonusLobbyEntranceView.viewController.CheckNeedSwitchToFull(true);
        }
        
        public void CheckAndStartSwitch()
        {
            if (Time.realtimeSinceStartup - lastSwitchTime >= switchInterval)
            {
                bool isLuckyStage = _timeBonusController.IsLuckyWheelStage();

                WheelBonusLobbyEntranceView activeWheelStage =
                    isLuckyStage ? luckyWheelBonusLobbyEntranceView : multWheelBonusLobbyEntranceView;

                if (isLuckyStage)
                {
                    if (multWheelBonusLobbyEntranceView.IsActive())
                        multWheelBonusLobbyEntranceView.Hide();
                }
                else if (luckyWheelBonusLobbyEntranceView.IsActive())
                {
                    luckyWheelBonusLobbyEntranceView.Hide();
                }
                
                if (coinBonusLobbyEntranceView.IsActive())
                {
                    if (!coinBonusLobbyEntranceView.viewController.CanGradualHide())
                    {
                        WaitForSeconds(2,CheckAndStartSwitch);
                        return;
                    }
                   
                    coinBonusLobbyEntranceView.viewController.GradualHide();
                    activeWheelStage.viewController.GradualAppear();
                }
                else
                {
                    if (!activeWheelStage.viewController.CanGradualHide())
                    {
                        WaitForSeconds(2, CheckAndStartSwitch);
                        return;
                    }
                    coinBonusLobbyEntranceView.viewController.GradualAppear();
                    activeWheelStage.viewController.GradualHide();
                }
                
                lastSwitchTime = Time.realtimeSinceStartup;

                WaitForSeconds(switchInterval, CheckAndStartSwitch);
            }
            else
            {
                var waitTime = Time.realtimeSinceStartup - lastSwitchTime - switchInterval;
              
                if (waitTime < 0)
                    WaitForSeconds(- waitTime, CheckAndStartSwitch);
                else
                {
                    CheckAndStartSwitch();
                }
            }
        }
    }
}