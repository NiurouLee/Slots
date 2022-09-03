// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/18/16:33
// Ver : 1.0.0
// Description : CommodityBonusView.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class CommodityBonusView : View<CommodityBonusViewController>
    {
        [ComponentBinder("BonusQuantityText")] public Text bonusQuantityText;

        [ComponentBinder("BonusDescriptionText")]
        public TextMeshProUGUI bonusDescriptionText;

        [ComponentBinder("QuantityGroup")]
        public Transform quantityGroup;

        [ComponentBinder("BonusTimerText")] public TextMeshProUGUI bonusTimerText;

        [ComponentBinder("TimerButton")] public Button timerButton;

        [ComponentBinder("FreeButton")] public Button freeButton;

        [ComponentBinder("IconDisable")] public Transform iconDisable;

        [ComponentBinder("IconEnable")] public Transform iconEnable;

        public Animator animator;

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public void SetViewToClaimState(ulong coinsCount)
        {
            bonusQuantityText.text = coinsCount.GetCommaFormat();
            animator.Play("Open");

            quantityGroup.gameObject.SetActive(true);

            bonusDescriptionText.text = "A store bonus has been ready for you, open it and get more free coins";
            timerButton.gameObject.SetActive(false);
            freeButton.gameObject.SetActive(true);

            var crazeSmashIcon = freeButton.transform.Find("CrazeSmashIcon");
            if (crazeSmashIcon != null)
            {
                bool locked = Client.Get<CrazeSmashController>().locked;
                crazeSmashIcon.gameObject.SetActive(!locked);
            }  
        }

        public void SetViewToWaitState(float inCountDownTime, bool animation)
        {
            animator.Play(animation ? "Finish" : "Close");

            quantityGroup.gameObject.SetActive(false);
            bonusTimerText.text = TimeSpan.FromSeconds(inCountDownTime).ToString(@"hh\:mm\:ss");

            bonusDescriptionText.text = "Come back tomorrow and enjoy the free store bonus";
            timerButton.gameObject.SetActive(true);
            freeButton.gameObject.SetActive(false);
        }
    }

    public class CommodityBonusViewController : ViewController<CommodityBonusView>
    {
        protected StoreBonus storeBonus;
        protected float syncDataTime;
        protected float countDownTime;
        private bool isUpdating = false;
        private ulong storeBonusCoinCount;

        public void SetUp(StoreBonus inStoreBonus)
        {
            storeBonus = inStoreBonus;

            storeBonusCoinCount = inStoreBonus.Coins;

            syncDataTime = Time.realtimeSinceStartup;

            if (storeBonus.CountdownTime == 0)
            {
                SetViewToClaimState(storeBonus.Coins);
            }
            else
            {
                SetViewToWaitState(storeBonus.CountdownTime);
            }

            view.freeButton.onClick.AddListener(OnStoreBonusClicked);
        }

        private bool processingClick = false;
        protected async void OnStoreBonusClicked()
        {
            if (processingClick)
            {
                return;
            }
            
            processingClick = true;
            
            SoundController.PlayButtonClick();
           
            var storePaymentHandler = Client.Get<IapController>().GetPaymentHandler<StorePaymentHandler>();
            var claimResult = await storePaymentHandler.ClaimStoreBonus();

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStoreBonusClick);

            if (claimResult != null)
            {
                await XUtility.FlyCoins(view.freeButton.transform,
                    new EventBalanceUpdate((long)claimResult.AddCoins, "StoreBonusReward"));

                if (view.transform)
                    SetViewToWaitState(claimResult.CountdownTime, true);
            }
            
            EventBus.Dispatch(new EventCollectStoreBonusFinish(claimResult));
            
            processingClick = false;
        }
        public override void Update()
        {
            var elapseTime = Time.realtimeSinceStartup - syncDataTime;

            if (countDownTime <= elapseTime)
            {
                SetViewToClaimState(storeBonusCoinCount);

                if (isUpdating)
                {
                    UpdateScheduler.UnhookUpdate(this);
                    isUpdating = false;
                }
            }
            else
            {
                var leftTime = countDownTime - elapseTime;
                view.bonusTimerText.text = TimeSpan.FromSeconds(leftTime).ToString(@"hh\:mm\:ss");
            }
        }

        public void SetViewToClaimState(ulong coinsCount)
        {
            syncDataTime = Time.realtimeSinceStartup;

            DisableUpdate();

            view.SetViewToClaimState(coinsCount);
        }

        public void SetViewToWaitState(float inCountDownTime, bool animation = false)
        {
            EnableUpdate(2);

            isUpdating = true;

            syncDataTime = Time.realtimeSinceStartup;
            countDownTime = inCountDownTime;

            view.SetViewToWaitState(inCountDownTime, animation);
        }
    }
}