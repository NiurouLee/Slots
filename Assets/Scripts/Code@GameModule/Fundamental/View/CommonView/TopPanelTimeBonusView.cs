// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/27/14:25
// Ver : 1.0.0
// Description : TopPanelTimeBonusView.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TopPanelTimeBonusView : View<TopPanelTimeBonusViewController>
    {
        [ComponentBinder("MultWheelBonusButton")]
        public Transform multWheelBonus;

        [ComponentBinder("LuckyWheelBonusButton")]
        public Transform luckyWheelBonus;

        [ComponentBinder("CoinButton")] public Transform coinButton;
        
        [ComponentBinder("SpinBuffNotice")] public Transform spinBuffNotice;

        protected override void EnableView()
        {
            gameObject.SetActive(true);
            if(gameObject.activeInHierarchy)
                viewController.OnViewEnabled();
        }
    }

    public class BonusEntranceView : View<ViewController>
    {
        protected bool canSwitchToOther = true;
        
        public virtual bool CanSwitchToOther()
        {
            return canSwitchToOther;
        }
    }

    public class TopPanelCoinView : BonusEntranceView
    {
        [ComponentBinder("IntegralText")] 
        protected Text collectCoinLabel;   
        
        [ComponentBinder("CollectGroup/Content/IntegralGroup/Icon")] 
        protected Transform flyRoot;  
        
        [ComponentBinder("CollectGroup")] 
        protected RectTransform collectGroup;

        protected TimeBonusController _timeBonusController;

        protected Button collectButton;

        protected Animator animator;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            collectButton = transform.GetComponent<Button>();
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            
            collectButton.onClick.AddListener(OnCollectClicked);
            _timeBonusController = Client.Get<TimeBonusController>();

            if (collectGroup != null && ViewManager.Instance.IsPortrait)
            {
                collectGroup.anchoredPosition = new Vector2(100, collectGroup.anchoredPosition.y);
            }
        }

       protected void OnCollectClicked()
       {
           collectButton.interactable = false;
           canSwitchToOther = false;
           
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
                   BiEventFortuneX.Types.GameEventType.GameEventTimerBonusHourlyCollect, ("source", "Machine"),
                   ("accLevel",accLevel.ToString()),("SpinBuffLevel", spinBuffLevel.ToString()));

               if (item != null)
               {
                   animator.Play("Collect",0,0);
                   
                   collectCoinLabel.text = item.Coin.Amount.GetCommaFormat();
                 
                   await viewController.WaitForSeconds(0.3f);
                 
                   await XUtility.FlyCoins(flyRoot,
                       new EventBalanceUpdate(item.Coin.Amount, "HourlyBonus"));

                   //   OnTimeBonusStateChanged(new EventTimeBonusStateChanged());
               }
               else
               {
                   CommonNoticePopup.ShowCommonNoticePopUp("Unknown error Occured! Please try again later");
               }

               collectButton.interactable = true;
               canSwitchToOther = true;
               
           }, false);
       }
    }
    
    public class TopPanelWheelView : BonusEntranceView
    {
        protected TimeBonusController _timeBonusController;

        protected bool isMultWheel = false;

        [ComponentBinder("IconGroup/CoinIconBG/TimerGroup/TimerText")]
        protected TMP_Text timerText;
        protected Animator animator;
        
        public void SetIsMultWheel(bool isMult)
        {
            isMultWheel = isMult;
        }

        public bool IsWheelStageReady()
        {
            if (isMultWheel)
            {
                return _timeBonusController.IsBonusReady(TimerBonusStage.SuperWheelBonus);
            }
            
            return _timeBonusController.IsBonusReady(TimerBonusStage.LuckyWheelBonus);
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _timeBonusController = Client.Get<TimeBonusController>();
            transform.GetComponent<Button>().onClick.AddListener(OnCollectClicked);
            
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        
        protected void OnCollectClicked()
        {
            if (isMultWheel)
            {
                if (_timeBonusController.IsBonusReady(TimerBonusStage.SuperWheelBonus))
                {
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusSuperWheelPopup), "TopPanel")));
                }
            }
            else
            {
                if (_timeBonusController.IsBonusReady(TimerBonusStage.LuckyWheelBonus))
                {
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusWheelBonusPopup),"TopPanel")));
                }
            }
        }

        public override void Show()
        {
            base.Show();
            if (_timeBonusController.IsBonusReady())
            {
                animator.Play("Spin");
            }
            else
            {
                animator.Play("Time");
            }
        }

        public void Update()
        {
            var countDown = _timeBonusController.GetWheelBonusCountDown();
            if (countDown > 0)
            {
                timerText.text = XUtility.GetTimeText(_timeBonusController.GetWheelBonusCountDown());
            }
        }
    }

    public class TopPanelBuffView : BonusEntranceView
    {
        [ComponentBinder("PercentText")] protected Text bufferPercent;
        [ComponentBinder("ProgressBar")] protected Slider slider;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            bufferPercent.text = Client.Get<TimeBonusController>().GetSpinBuffMultiplier() + "%";

            var spinBuff = Client.Get<BuffController>().GetBuff<TimerbonusSpinBuff>();

            if (spinBuff != null)
                slider.value = spinBuff.exp / 100.0f;
        }

        public override void Show()
        {
            base.Show();
            UpdateSpinBuff(false);
        }

        public void UpdateSpinBuff(bool hasAnimation = true)
        {
            canSwitchToOther = false;
            if(hasAnimation)
                transform.GetComponent<Animator>().Play("Hit");

            bufferPercent.text = Client.Get<TimeBonusController>().GetSpinBuffMultiplier() + "%";

            var spinBuff = Client.Get<BuffController>().GetBuff<TimerbonusSpinBuff>();

            if (spinBuff != null)
                slider.value = spinBuff.exp / 100.0f;

            viewController.WaitForSeconds(1, () => { canSwitchToOther = true; });
        }
        
      
    }

    public class TopPanelTimeBonusViewController : ViewController<TopPanelTimeBonusView>
    {
        protected enum EntranceType
        {
            Coin,
            Wheel,
            Buff,
        };

        protected float lastSwitchTime = 0;
        protected float switchInterval = 5;

        protected TimeBonusController _timeBonusController;

        protected TopPanelCoinView topPanelCoinView;
        protected TopPanelWheelView topPanelLuckyWheelView;
        protected TopPanelWheelView topPanelMultWheelView;
        protected TopPanelBuffView topPanelBufView;

        protected BonusEntranceView currentActiveView;
        protected EntranceType activeEntranceType = EntranceType.Coin;
        
        public override void OnViewDidLoad()
        {
            _timeBonusController = Client.Get<TimeBonusController>();
            
            // view.coinButton.onClick.AddListener(OnCoinCollectClicked);
            // view.multWheelBonusButton.onClick.AddListener(OnMultWheelBonusCollectClicked);
            // view.luckyWheelBonusButton.onClick.AddListener(OnLuckyWheelBonusCollectClicked);
             
            base.OnViewDidLoad();
            
            EnableUpdate(2);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
           SubscribeEvent<EventTimeBonusStateChanged>(OnEventTimeBonusStateChanged);
           SubscribeEvent<EventSpinSuccess>(OnSpinSuccess);
        }
        
        private void OnSpinSuccess(EventSpinSuccess evt)
        {
            if (currentActiveView == topPanelBufView)
            {
                topPanelBufView.UpdateSpinBuff();
            }
        }

        protected void OnEventTimeBonusStateChanged(EventTimeBonusStateChanged evt)
        {
            if (activeEntranceType == EntranceType.Wheel)
            {
                bool isLuckyStage = _timeBonusController.IsLuckyWheelStage();

                if (isLuckyStage && currentActiveView == topPanelMultWheelView)
                {
                    topPanelMultWheelView.Hide();
                    currentActiveView = topPanelLuckyWheelView;
                    topPanelLuckyWheelView.Show();
                }
                else if (!isLuckyStage && currentActiveView == topPanelLuckyWheelView)
                {
                    topPanelLuckyWheelView.Hide();
                    currentActiveView = topPanelMultWheelView;
                    topPanelMultWheelView.Show();
                }
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            if (topPanelCoinView == null)
            {
                topPanelCoinView = view.AddChild<TopPanelCoinView>(view.coinButton);
                topPanelMultWheelView = view.AddChild<TopPanelWheelView>(view.multWheelBonus);
                topPanelLuckyWheelView = view.AddChild<TopPanelWheelView>(view.luckyWheelBonus);
                topPanelBufView = view.AddChild<TopPanelBuffView>(view.spinBuffNotice);

                topPanelMultWheelView.SetIsMultWheel(true);
                topPanelLuckyWheelView.SetIsMultWheel(false);

                topPanelCoinView.Show();
                topPanelMultWheelView.Hide();
                topPanelLuckyWheelView.Hide();
                topPanelBufView.Hide();
                activeEntranceType = EntranceType.Coin;
                currentActiveView = topPanelCoinView;

                CheckStartNextViewSwitch();
            }
        }

        public void CheckStartNextViewSwitch()
        {
            if (Time.realtimeSinceStartup - lastSwitchTime >= switchInterval)
            {
                bool isLuckyStage = _timeBonusController.IsLuckyWheelStage();

                var activeWheelStage =
                    isLuckyStage ? topPanelLuckyWheelView : topPanelMultWheelView;

                if (isLuckyStage)
                {
                    if (topPanelMultWheelView.IsActive())
                        topPanelMultWheelView.Hide();
                }
                else if (topPanelLuckyWheelView.IsActive())
                {
                    topPanelLuckyWheelView.Hide();
                }

                if (!currentActiveView.CanSwitchToOther())
                {
                    WaitForSeconds(2, CheckStartNextViewSwitch);
                    return;
                }

                switch (activeEntranceType)
                {
                    case EntranceType.Coin:
                        topPanelCoinView.Hide();
                     
                        if (activeWheelStage.IsWheelStageReady())
                        {
                            activeWheelStage.Show();
                            currentActiveView = activeWheelStage;
                            activeEntranceType = EntranceType.Wheel;
                        }
                        else
                        {
                            topPanelBufView.Show();
                            currentActiveView = topPanelBufView;
                            activeEntranceType = EntranceType.Buff;
                        }
                        break;
                    case EntranceType.Wheel:
                        activeWheelStage.Hide();
                        topPanelBufView.Show();
                        currentActiveView = topPanelBufView;
                        activeEntranceType = EntranceType.Buff;
                        break;
                    case EntranceType.Buff:
                        topPanelBufView.Hide();
                        topPanelCoinView.Show();
                        currentActiveView = topPanelCoinView;
                        activeEntranceType = EntranceType.Coin;
                        break;
                }

                lastSwitchTime = Time.realtimeSinceStartup;

                WaitForSeconds(switchInterval, CheckStartNextViewSwitch);
            }
            else
            {
                var waitTime = Time.realtimeSinceStartup - lastSwitchTime - switchInterval;

                if (waitTime < 0)
                    WaitForSeconds(-waitTime, CheckStartNextViewSwitch);
                else
                {
                    CheckStartNextViewSwitch();
                }
            }
        }

        public override void Update()
        {
            if (currentActiveView is TopPanelWheelView wheelView)
            {
                wheelView.Update();
            }
        }
    }
}