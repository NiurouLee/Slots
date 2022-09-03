// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/14/12:04
// Ver : 1.0.0
// Description : TimeBonusWheelBonusPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

using UnityEngine;

namespace GameModule
{
    [AssetAddress("UITimerBonusWheelBonus")]
    public class TimeBonusWheelBonusPopup : Popup<TimeBonusWheelBonusViewController>
    {
        [ComponentBinder("LuckyBonusWheel")] public Transform luckyBonusWheel;

        [ComponentBinder("UITimerBonusFinish")]
        public Transform bonusFinish;

        [ComponentBinder("GoldenBonusWheel")] public Transform goldenBonusWheel;

        [ComponentBinder("Character")] public Transform characterNode;

        public WheelBonusCollectView collectView;
        public TimeBonusLuckyWheelView luckyWheelView;
        public TimeBonusGoldenWheelView goldenWheelView;

        public TimeBonusWheelBonusPopup(string address)
            : base(address)
        {
            
        }
        
        public override float GetPopUpMaskAlpha()
        {
            return 0;
        }
        
        public override bool NeedForceLandscapeScreen()
        {
            return true;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            collectView = AddChild<WheelBonusCollectView>(bonusFinish);
            luckyWheelView = AddChild<TimeBonusLuckyWheelView>(luckyBonusWheel);
            goldenWheelView = AddChild<TimeBonusGoldenWheelView>(goldenBonusWheel);
        }

        public override void Close()
        {
            goldenWheelView.HideMultipleArrow(true);
            viewController.LogBiExitEvent();
            SoundController.RecoverLastMusic();
            base.Close();
        }
        
        public override string GetOpenAudioName()
        {
            return "Wheel_Screen";
        }
        
        public override string GetCloseAudioName()
        {
            return "Wheel_Screen";
        }
    }

    public class TimeBonusWheelBonusViewController : ViewController<TimeBonusWheelBonusPopup>
    {
        private SGetWheelBonus sGetWheelBonus;
        private SSpinWheelBonusWithNoRecord sAdWheelBonus;
        private SGetGoldenWheelBonus sGetGoldenWheelBonus;

        private VerifyExtraInfo _verifyExtraInfo;
        private FulfillExtraInfo _fulfillExtraInfo;
        private Action _fulfillFxEndCallback;

        private bool _closeClicked = false;

        private Action<Action<FulFillCallbackArgs>> _fullFillRequestHandler;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            
            if (Client.Get<TimeBonusController>().IsBonusReady(TimerBonusStage.LuckyWheelBonus))
            {
                sGetWheelBonus = inExtraAsyncData as SGetWheelBonus;
            }
            else 
            {
                sGetGoldenWheelBonus = inExtraAsyncData as SGetGoldenWheelBonus;
            }
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTimeBonusGoldenWheelPurchaseSucceed>(OnGoldenWheelPurchaseSucceed);
            SubscribeEvent<EventOnLuckyWheelAdNoticeChoose>(OnAdChooseNotice);
        }

        protected  void OnAdChooseNotice(EventOnLuckyWheelAdNoticeChoose evt)
        {
            if (evt.playAgain)
            {
                ADSController.TryShowRewardedVideo(eAdReward.WheelBonusRV, (b, s) =>
                {
                    if (b)
                    {
                        StartLuckyWheelAdBonus();
                    }
                    else
                    {
                        SwitchToGoldenWheel();
                    }
                });
            }
            else
            {
                SwitchToGoldenWheel();
            }
        }

        protected async void StartLuckyWheelAdBonus()
        {
            var adWheelBonus = await Client.Get<TimeBonusController>().GetAdBonus();

            if (adWheelBonus == null)
            {
                SwitchToGoldenWheel();
            }
            else
            {
                sAdWheelBonus = adWheelBonus;
                view.luckyWheelView.InitializeWheelView(adWheelBonus.WheelBonus[0], 6);
                PlayAnimation(view.luckyWheelView.animator, "Idle");
                PlayAnimation(view.animator, "LuckyWheelOpen");
                view.luckyWheelView.spinButton.onClick.RemoveAllListeners();
                view.luckyWheelView.spinButton.onClick.AddListener(OnAdWheelBonusButtonClicked);
                view.luckyWheelView.spinButton.interactable = true;
            }
        }

        protected async void OnAdWheelBonusButtonClicked()
        {
            view.luckyWheelView.spinButton.interactable = false;

            view.luckyWheelView.StartSpinWheel();
            SoundController.PlaySfx("Wheel_Spin-04");
            var wheelResult = sAdWheelBonus.Result;
            var hitWedgeId = wheelResult.HitWedgeId;

            await WaitForSeconds(2.0f);

            WaitForSeconds(2, () =>
            {
                SoundController.PlaySfx("Wheel_CoinsWin");
            });
            
            await view.luckyWheelView.StopWheel((int) hitWedgeId[0] - 1);

            await view.luckyWheelView.ShowWinEffect();
            
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAdsCollect,("placeId", eAdReward.WheelBonusRV.ToString()));
            
            view.collectView.SetCollectViewData(this.view, sAdWheelBonus.Item, SwitchToGoldenWheel, "LUCKY WHEEL",
                (claimProcessHandler) =>
                {
                    claimProcessHandler.Invoke(sAdWheelBonus.Item);
                }, sAdWheelBonus.UserBuffGodenOdds);
            
            PlayAnimation(view.animator, "BonusCollectViewAppear");
        }

        protected void OnGoldenWheelPurchaseSucceed(EventTimeBonusGoldenWheelPurchaseSucceed evt)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusGoldenWheelPurchaseSuccess, 
                ("paymentId",evt.verifyExtraInfo.Item.PaymentId.ToString()),("price", evt.verifyExtraInfo.Item.Price.ToString()));
            
            SetGoldenWheelPurchaseContent(evt.verifyExtraInfo, evt.fulFillRequestHandler);
        }
        
        protected void FulfillGoldenWheel(Action<RepeatedField<Item>> claimHandler)
        {
            _fullFillRequestHandler.Invoke((fulFillCallbackArgs) =>
            {
                if (fulFillCallbackArgs.isSuccess)
                {
                    claimHandler.Invoke(fulFillCallbackArgs.fulfillExtraInfo.RewardItems);
                    _fulfillFxEndCallback = fulFillCallbackArgs.fullFillFxEndCallback;
                }
                else
                {
                    XDebug.LogError("FulfillGoldenWheelPaymentFailed");
                }
            });
        }
        
        public override void OnViewDidLoad()
        {
            
            
            view.goldenWheelView.closeButton.onClick.AddListener(OnGoldenWheelCloseClicked);


            if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
            {
                view.characterNode.gameObject.SetActive(false);
            }
            
            SoundController.PlayBgMusic("Bg_OrdinaryWheel");
            
            base.OnViewDidLoad();
        }

        protected async void OnGoldenWheelCloseClicked()
        {
            if (_closeClicked)
            {
                view.Close();
            }
            else
            {
                var wheelBonus = sGetGoldenWheelBonus.WheelBonus[0];
                ulong maxAmount = 0;
                for (var i = 0; i < wheelBonus.Wedge.Count; i++)
                {
                    var item = wheelBonus.Wedge[i].Item;
                    if (item.Type == Item.Types.Type.Coin)
                    {
                        if (item.Coin.Amount > maxAmount)
                        {
                            maxAmount = item.Coin.Amount;
                        }
                    }
                }

                var popup = await PopupStack.ShowPopup<TimeBonusQuitConfirmPopup>();

                popup.viewController.SetUpQuitConfirmUI(sGetGoldenWheelBonus.PayItem, maxAmount, this.view);
                _closeClicked = true;
            }
        }

        public void OnGoldenWheelSpinButtonClicked()
        {
            Client.Get<IapController>().BuyProduct(sGetGoldenWheelBonus.PayItem);
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusGoldenWheelPurchase, 
                ("paymentId",sGetGoldenWheelBonus.PayItem.PaymentId.ToString()),("price", sGetGoldenWheelBonus.PayItem.Price.ToString()));
        }

        public async void OnLuckyWheelSpinButtonClicked()
        {
            view.luckyWheelView.spinButton.interactable = false;

            view.luckyWheelView.StartSpinWheel();
            
            SoundController.PlaySfx("Wheel_Spin-04");

          //  XUtility.WaitSeconds(1, cancelableCallback);
            var startTime = Time.realtimeSinceStartup;
            var spinResult = await Client.Get<TimeBonusController>().GetWheelSpinResult(TimerBonusWheelId.LuckyWheel);

            var wheelResult = spinResult.Result[0];
            var hitWedgeId = wheelResult.HitWedgeId;

            var duration = Time.realtimeSinceStartup - startTime;
          //  SoundController.PreloadSoundAssets(new List<string>() {"Wheel_Spin2_3_3"}, null);
            
            if(duration < 2f)
                await WaitForSeconds(2f - duration);
            
          //  cancelableCallback.CancelCallback();

            // var leftTime = SoundController.GetAudioLeftTimeToFinish("Wheel_Spin2_2_3");
            //
            // await WaitForSeconds(leftTime);
            // SoundController.PlaySfx("Wheel_Spin2_3_3");
            // SoundController.StopSfx("Wheel_Spin2_2_3");

            WaitForSeconds(2, () =>
            {
                SoundController.PlaySfx("Wheel_CoinsWin");
            });
            
            await view.luckyWheelView.StopWheel((int) hitWedgeId[0] - 1);
            
            await view.luckyWheelView.ShowWinEffect();
            var source = GetTriggerSource();
            
            XDebug.Log("source:" + source);
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusWheelCollect, 
                ("wedgeId",hitWedgeId[0].ToString()), 
                ("superWheelBoost", spinResult.UserBuffGodenOdds.ToString()),
                ("source",source));

            view.collectView.SetCollectViewData(this.view, spinResult.Item, CheckAndShowRVWheel, "LUCKY WHEEL",
                (claimProcessHandler) =>
                {
                    Client.Get<TimeBonusController>().ClaimWheelBonus(claimProcessHandler);
                }, spinResult.UserBuffGodenOdds);
            
            PlayAnimation(view.animator, "BonusCollectViewAppear");
        }


        protected void CheckAndShowRVWheel()
        {
            if (ADSController.ShouldShowRV(eAdReward.WheelBonusRV))
            {
                PopupStack.ShowPopupNoWait<TimeBonusAdsNoticePopup>();
            }
            else
            {
                SwitchToGoldenWheel();
            }
        }

        protected async void SwitchToGoldenWheel()
        {
            sGetGoldenWheelBonus = await Client.Get<TimeBonusController>().GetGoldenWheelInfo();
            view.goldenWheelView.InitializeWheelView(sGetGoldenWheelBonus.WheelBonus[0], 6,1.0 / sGetGoldenWheelBonus.SpinBuffOdds);
            view.goldenWheelView.InitializeExtraUI(sGetGoldenWheelBonus);
            
            SoundController.PlaySfx("Wheel_Screen");
            await PlayAnimationAsync(view.animator, "LuckyWheelToGoldenWheel");

            if (sGetGoldenWheelBonus.SpinBuffOdds > 1)
            {
                await view.goldenWheelView.ShowSpinBuffAdditionAnimation(sGetGoldenWheelBonus.SpinBuffOdds,
                    sGetGoldenWheelBonus.WheelBonus[0], 6, 1);
            }
            
            if (view.goldenWheelView.spinButton)
                view.goldenWheelView.spinButton.onClick.AddListener(OnGoldenWheelSpinButtonClicked);
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusGoldenWheelPop, 
                ("paymentId",sGetGoldenWheelBonus.PayItem.PaymentId.ToString()),("price", sGetGoldenWheelBonus.PayItem.Price.ToString()));
        }

        public override async void OnViewEnabled()
        {
            base.OnViewEnabled();

            if (sGetGoldenWheelBonus != null)
            {
                view.goldenWheelView.InitializeWheelView(sGetGoldenWheelBonus.WheelBonus[0], 6,1.0 / sGetGoldenWheelBonus.SpinBuffOdds);
                view.goldenWheelView.InitializeExtraUI(sGetGoldenWheelBonus);
                
                await PlayAnimationAsync(view.animator, "GoldenWheelOpen");
                if (sGetGoldenWheelBonus.SpinBuffOdds > 1)
                {
                    await view.goldenWheelView.ShowSpinBuffAdditionAnimation(sGetGoldenWheelBonus.SpinBuffOdds,
                        sGetGoldenWheelBonus.WheelBonus[0], 6, 1);
                }
                
                if (view.goldenWheelView.spinButton)
                    view.goldenWheelView.spinButton.onClick.AddListener(OnGoldenWheelSpinButtonClicked);
            }
            else if (sGetWheelBonus != null)
            {
                view.luckyWheelView.InitializeWheelView(sGetWheelBonus.WheelBonus[0], 6,
                    1.0 / sGetWheelBonus.SpinBuffOdds);
                PlayAnimation(view.animator, "LuckyWheelOpen");
                await WaitForSeconds(4.0f);
                if (sGetWheelBonus.SpinBuffOdds > 1)
                {
                    await view.luckyWheelView.ShowSpinBuffAdditionAnimation(sGetWheelBonus.SpinBuffOdds,
                        sGetWheelBonus.WheelBonus[0], 6, 1);
                }
                
                if (view.luckyWheelView.spinButton)
                    view.luckyWheelView.spinButton.onClick.AddListener(OnLuckyWheelSpinButtonClicked);
            }
        }

        public void LogBiExitEvent()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusGoldenWheelExit, 
                ("paymentId",sGetGoldenWheelBonus.PayItem.PaymentId.ToString()),("price", sGetGoldenWheelBonus.PayItem.Price.ToString()));

        }

        public async void SetGoldenWheelPurchaseContent(VerifyExtraInfo verifyExtraInfo,
            Action<Action<FulFillCallbackArgs>> fullFillRequestHandler, bool isReplenishmentOrder = false)
        {
            _verifyExtraInfo = verifyExtraInfo;
            _fullFillRequestHandler = fullFillRequestHandler;

            view.goldenWheelView.spinButton.interactable = false;
            view.goldenWheelView.priceText.gameObject.SetActive(false);
            view.goldenWheelView.HidePurchaseBenefitsUI();
            view.goldenWheelView.closeButton.gameObject.SetActive(false);
            view.goldenWheelView.closeButton.interactable = false;
            view.goldenWheelView.HideMultipleArrow(true);
            
            EventBus.Dispatch(new EventCloseTimeBonusQuitPopup());
 
            if (isReplenishmentOrder)
            {
                var hasAddition = sGetGoldenWheelBonus.SpinBuffOdds > 1;
                await WaitForSeconds(5 + (hasAddition ? 3.5f : 0));
            }
            
            view.goldenWheelView.StartSpinWheel();
            
            SoundController.PlaySfx("Wheel_Spin-04");
            
            var hitWedgeId = (int) verifyExtraInfo.Item.SubItemList[0].PayWheelBonus.HitWedgeIds[0] - 1;

            var userBuffGoldenOdds = verifyExtraInfo.Item.SubItemList[0].PayWheelBonus.UserBuffGodenOdds;
             
            await WaitForSeconds(2f);
          
            WaitForSeconds(2.3f, () =>
            {
                SoundController.PlaySfx("Wheel_CoinsWin");
            });
           
            await view.goldenWheelView.StopWheel((int) hitWedgeId);

            await WaitForSeconds(0.5f);
            
            await view.luckyWheelView.ShowWinEffect();
            
            //TODO Verify
            RepeatedField<Item> rewards = new RepeatedField<Item>();
       
            rewards.Add(verifyExtraInfo.Item.SubItemList[1]);

            view.collectView.SetCollectViewData(this.view, rewards, () =>
                {
                    if (_fulfillFxEndCallback != null)
                        view.SubscribeCloseAction(_fulfillFxEndCallback);
                    view.Close();
                }, "GOLDEN WHEEL",
                FulfillGoldenWheel, userBuffGoldenOdds);
            
            PlayAnimation(view.animator, "GoldenBonusCollectViewAppear");
        }
    }
}