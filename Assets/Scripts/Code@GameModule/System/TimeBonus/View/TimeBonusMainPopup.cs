// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/25/18:06
// Ver : 1.0.0
// Description : TimeBonusMainPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class MainPopupCoinBonusView : View<MainPopupCoinBonusViewController>
    {
        [ComponentBinder("ProgressBar")] public Slider progressBar;

        [ComponentBinder("CoinCollectButton")] public Button coinCollectButton;

        [ComponentBinder("AdsCollectButton")] public Button adsCollectButton;

        [ComponentBinder("IntegralText")] public TMP_Text integralText;

        [ComponentBinder("MaxIntegralText")] public Text maxIntegralText; 
        
        [ComponentBinder("MaskProgress")] public Slider maskProgress;
          
        [ComponentBinder("MachineGroup/IconGroup/IntegralGroup/Icon")]
        public Transform coinIcon;
        
        [ComponentBinder("MachineGroup/IconGroup/PorgerssGroup/ProgressBar/Fill Area/quanju/UIParticle")]
        public Transform fx1;
        
        [ComponentBinder("MachineGroup/IconGroup/PorgerssGroup/ProgressBar/Fill Area/quanju/UITImer_Jinbi")]
        public Transform fx2;
    }

    public class MainPopupCoinBonusViewController : ViewController<MainPopupCoinBonusView>
    {
        private TimeBonusController _timeBonusController;
        public float intervalTime;
        protected int countDownTime;
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.coinCollectButton.onClick.AddListener(OnCollectButtonClicked);
            view.adsCollectButton.onClick.AddListener(OnAdCollectButtonClicked);

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
        
        public void ToggleButton(bool interactable)
        {
            view.coinCollectButton.interactable = interactable;
            view.adsCollectButton.interactable = interactable;
            if (view.GetParentView() is Popup popup)
            {
                popup.closeButton.interactable = interactable;
            }

            if (view.coinCollectButton.gameObject.activeSelf)
            {
                var buttonAnimation = view.coinCollectButton.GetComponent<Animator>();
                buttonAnimation.Play(interactable ? "Loop":"Normal");
            }
        }

        public void OnCollectButtonClicked()
        {
            
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
                    BiEventFortuneX.Types.GameEventType.GameEventTimerBonusHourlyCollect, ("source", "MainPage"),
                    ("accLevel",accLevel.ToString()),("SpinBuffLevel", spinBuffLevel.ToString()));
                    
                    
                if (item != null)
                {
                    await XUtility.FlyCoins(view.coinIcon.transform,
                        new EventBalanceUpdate(item.Coin.Amount, "HourlyBonus"));

                    ToggleButton(true);
                    EnableUpdate();
                }
                else
                {
                    CommonNoticePopup.ShowCommonNoticePopUp("Unknown error Occured! Please try again later");
                }
            }, watchedRv);
        }

        public void OnAdCollectButtonClicked()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.InfinityVault, false))
            {
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

        protected bool isFull = false;
        public void CheckCloseFx(bool force = false)
        {
            var currentCoin = _timeBonusController.GetCurrentCoin();
            var progress = _timeBonusController.GetCoinBonusProgress(currentCoin);
            var nowIsFull = progress >= 1;
        
            if (!isFull && nowIsFull || (force && nowIsFull))
            {
                view.fx1.gameObject.SetActive(false);
                view.fx2.gameObject.SetActive(false);
                isFull = true;
                
            } else if (!nowIsFull && isFull)
            {
                view.fx1.gameObject.SetActive(true);
                view.fx2.gameObject.SetActive(true);
            }
            
            isFull = nowIsFull;
        }

        public override void Update()
        {
            var currentCoin = _timeBonusController.GetCurrentCoin();
            
            view.integralText.text = currentCoin.GetCommaFormat();
            view.maxIntegralText.text = _timeBonusController.GetMaxCollectableCoins().GetCommaFormat();
            
            CheckCloseFx();
             
            
            view.progressBar.value = Mathf.Max(1.1f/32,(int)(_timeBonusController.GetCoinBonusProgress(currentCoin) * 32));
            var countDown = GetCountDown();
            if (!view.adsCollectButton.gameObject.activeSelf && (CanShowRvButton() || countDown > 0))
            {
                view.adsCollectButton.gameObject.SetActive(true);
                view.coinCollectButton.gameObject.SetActive(false);
               
                if (countDown <= 0)
                {
                    _timeBonusController.lastTimeShowValultRV = Time.realtimeSinceStartup;
                }
            }
            else if (view.adsCollectButton.gameObject.activeSelf)
            {
                if (countDown > 0)
                {
                    view.maskProgress.value = countDown / countDownTime;
                }
                else
                {
                    view.adsCollectButton.gameObject.SetActive(false);
                    view.coinCollectButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public class MainPopupWheelBonusView : View<MainPopupWheelBonusViewController>
    {
        [ComponentBinder("SpinButton")] public Button spinButton;

        [ComponentBinder("AdsButton")] public Button adsButton;
        [ComponentBinder("TimerButton")] public Button timerButton;   
         
        [ComponentBinder("InformationGroup/Text")] public TMP_Text winUpToText;
        
        [ComponentBinder("ProgressGroup/SuperWheelFillGroup")]
        public Transform superWheelFillGroup;

        [ComponentBinder("TimerState/TimerButton/Label")]
        public TMP_Text countDownText;
    }

    public class MainPopupWheelBonusViewController : ViewController<MainPopupWheelBonusView>
    {
        private TimeBonusController _timeBonusController;

        protected Animator animator;
 
        private bool isMultiWheel = false;

        private bool isCountDown = false;

        public void SetIsMultiWheel(bool inIsMultiWheel)
        {
            isMultiWheel = inIsMultiWheel;
         
            UpdateBonusState(false);

            if (isMultiWheel)
            {
                view.winUpToText.text = "WIN UP TO " + _timeBonusController.GetMultWheelWinUpTo().GetCommaFormat();
            }
            else
            {
                view.winUpToText.text = "WIN UP TO " + _timeBonusController.GetLuckyWheelWinUpTo().GetCommaFormat();
            }
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventTimeBonusStateChanged>(OnBonusStateChanged);
        }

        public override void OnViewEnabled()
        {
            UpdateBonusState(false);
        }

        protected void OnBonusStateChanged(EventTimeBonusStateChanged evt)
        {
            UpdateBonusState(false);
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            view.adsButton.onClick.AddListener(OnAdsButtonClick);
            view.spinButton.onClick.AddListener(OnSpinButtonClicked);

            _timeBonusController = Client.Get<TimeBonusController>();

            animator = view.transform.GetComponent<Animator>();
 
            animator.keepAnimatorControllerStateOnDisable = true;
            
            WaitForSeconds(10,DoAdButtonSwitch);
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
            if (!isMultiWheel)
            {
                if (_timeBonusController.GetTimeBonusState() != TimerBonusStage.LuckyWheelBonus)
                {
                    DisableUpdate();
                    view.Hide();
                    return;
                }
            }
            else
            {
                if (_timeBonusController.GetTimeBonusState() != TimerBonusStage.SuperWheelBonus)
                {
                    DisableUpdate();
                    view.Hide();
                    return;
                }
            }
            
            view.Show();

            if (_timeBonusController.IsBonusReady())
            {
                isCountDown = false;
                animator.Play("SpinState");
            }
            else
            {

                isCountDown = true;
                
                if (AdController.Instance.ShouldShowRV(eAdReward.LuckyWheel, false))
                {
                    animator.Play("AdsState");
                }
                else
                {
                    animator.Play("TimerState");
                }
                
                if (!updateEnabled)
                    EnableUpdate(2);
            }
            
            UpdateToggleState();
        }

        public void ToggleButton(bool interactable)
        {
            view.spinButton.interactable = interactable;
            view.adsButton.interactable = interactable;
            
            if (view.GetParentView() is Popup popup)
            {
                popup.closeButton.interactable = interactable;
            }
        }

        public void OnAdsButtonClick()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.LuckyWheel, false))
            {
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
               _timeBonusController.RequestSlowDownCd(()=>
               {
                   UpdateBonusState(true);
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
          //  ToggleButton(false);

            if (!isMultiWheel)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusWheelBonusPopup), "Lobby")));
            }
            else
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusSuperWheelPopup), "Lobby")));
            }
        }

        public void DoAdButtonSwitch()
        {
            if (isCountDown)
            {
                if (view.adsButton.gameObject.activeInHierarchy)
                {
                    animator.Play("TimerState");
                }
                else
                {
                    if (AdController.Instance.ShouldShowRV(eAdReward.LuckyWheel, false))
                    {
                        animator.Play("AdsState");
                    }
                    else
                    {
                        WaitForSeconds(2, DoAdButtonSwitch);
                        return;
                    }
                }
            }

            WaitForSeconds(10, DoAdButtonSwitch);
        }

        public override void Update()
        {
            if (_timeBonusController.GetWheelBonusCountDown() <= 0)
            {
                isCountDown = false;
                
                DisableUpdate();
                animator.Play("SpinState");
            }
            else
            {
                view.countDownText.text = XUtility.GetTimeText(_timeBonusController.GetWheelBonusCountDown());
                
                
            }
        }
    }

    [AssetAddress("UITimerBonusMain")]
    public class TimeBonusMainPopup : Popup<TimeBonusMainPopupViewController>
    {
        [ComponentBinder("SpinBuffProgressBar")]
        public Slider spinBuffSlider;

        [ComponentBinder("BubbleGroup")] public Transform bubbleGroup;

        [ComponentBinder("TimerText")] public TMP_Text timerText;

        [ComponentBinder("TimerBonusGroup")] public Transform timerBonusGroup;

        [ComponentBinder("MultWheelBonusGroup")]
        public Transform multWheelBonusGroup;

        [ComponentBinder("LuckyWheelBonusGroup")]
        public Transform luckyWheelBonusGroup;
        
        [ComponentBinder("InformationButton")] 
        public Button informationButton;

        public override float GetPopUpMaskAlpha()
        {
            return 0;
        }

        public TimeBonusMainPopup(string address)
            : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
    }

    public class TimeBonusMainPopupViewController : ViewController<TimeBonusMainPopup>
    {
        public MainPopupWheelBonusView luckyWheelView;
        public MainPopupWheelBonusView multWheelView;
        public MainPopupCoinBonusView coinBonusView;

        private TimeBonusController _timeBonusController;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            _timeBonusController = Client.Get<TimeBonusController>();
            
            luckyWheelView = view.AddChild<MainPopupWheelBonusView>(view.luckyWheelBonusGroup);
            multWheelView = view.AddChild<MainPopupWheelBonusView>(view.multWheelBonusGroup);
            coinBonusView = view.AddChild<MainPopupCoinBonusView>(view.timerBonusGroup);
            
            luckyWheelView.viewController.SetIsMultiWheel(false);
            multWheelView.viewController.SetIsMultiWheel(true);
            view.bubbleGroup.gameObject.SetActive(false);
 
            UpdateBuffSliderValue();
            
            Update();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.informationButton.onClick.AddListener(OnInfoButtonClicked);
        }

        protected void OnInfoButtonClicked()
        {
            if (!view.bubbleGroup.gameObject.activeSelf)
            {
                var bubbleAnimator = view.bubbleGroup.GetComponent<Animator>();
                view.bubbleGroup.gameObject.SetActive(true);

                if (bubbleAnimator)
                {
                    bubbleAnimator.Play("BubbleGroup", 0, 0);
                }
            }
            else
            {
                var bubbleAnimator = view.bubbleGroup.GetComponent<Animator>();
                var stateInfo = bubbleAnimator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime >= 0.9f)
                {
                    bubbleAnimator.Play("BubbleGroup", 0, 0);
                }
                else
                {
                    bubbleAnimator.Play("BubbleGroup", 0, 0.9f);
                }
            }
        }
        
        public override void OnViewEnabled()
        {
            if (!updateEnabled)
            {
                EnableUpdate(2);
            }
            
            luckyWheelView.viewController.UpdateBonusState(false);
            multWheelView.viewController.UpdateBonusState(true);
            view.bubbleGroup.gameObject.SetActive(false);
        }

        protected void UpdateBuffSliderValue()
        {
            var spinBuff = Client.Get<BuffController>().GetBuff<TimerbonusSpinBuff>();
            var buffLevelCount = _timeBonusController.GetMaxSpinBuffLevel();
            var percentageEachLevel = 1.0f/buffLevelCount;
            
            if (spinBuff != null)
            {
                view.spinBuffSlider.value = percentageEachLevel * ((spinBuff.spinBuffLevel - 1) + spinBuff.exp * 0.01f);
            }

            var index = 1;
            var trans = view.spinBuffSlider.transform;

            var percentText = trans.Find($"Level{index}");
            var percentTextInactive = trans.Find($"LevelInactive{index}");
            
            while (percentText != null)
            {
                if (index <= buffLevelCount)
                {
                    percentText.GetComponent<Text>().text = _timeBonusController.GetSpinBuffMultiplier(index - 1) + "%";
                    percentTextInactive.GetComponent<Text>().text = _timeBonusController.GetSpinBuffMultiplier(index - 1) + "%";

                    if (spinBuff != null)
                    {
                        percentText.gameObject.SetActive((ulong) index <= spinBuff.spinBuffLevel);
                        percentTextInactive.gameObject.SetActive((ulong) index > spinBuff.spinBuffLevel);
                    }
                }
                else
                {
                    percentText.gameObject.SetActive(false);
                    percentTextInactive.gameObject.SetActive(false);
                }
                index++;
                percentText = trans.Find($"Level{index}");
                percentTextInactive = trans.Find($"LevelInactive{index}");
            }
        }

        private float lastRestLeftTime = 0;

        public override void Update()
        {
            var leftTime = _timeBonusController.GetMultiplierResetCountDown();
            if (leftTime > lastRestLeftTime)
            {
                UpdateBuffSliderValue();
            }

            view.timerText.text = XUtility.GetTimeText(_timeBonusController.GetMultiplierResetCountDown());
            lastRestLeftTime = leftTime;
        }
    }
}