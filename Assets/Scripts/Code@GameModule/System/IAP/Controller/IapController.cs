// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/31/11:05
// Ver : 1.0.0
// Description : IapController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dlugin;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine.Purchasing;

namespace GameModule
{

    public static class ShopType
    {
        public static string Store = "store";
        public static string PiggyBank = "piggybank";
        public static string SeasonPass = "missionpass";
        public static string SeasonQuest = "questpass";
        public static string GoldenWheel = "wheel";
        public static string Deal = "deal";
        public static string Activity_Valentine2022 = "valentinegift";
        public static string CrazeSmash = "Crazesmash";
        public static string NewBieQuestBoost = "newbiequestboost";
        public static string SlotDeal = "slotsdeal";
        public static string TreasureRaidDeal = "treasureraid";
        public static string CoinDash = "coindash";
        public static string Lotto = "levelrush";
        public static string JulyCarnival = "julycarnival";
        public static string SuperSpinX = "superspinx";
        public static string RushPass = "rushpass";
    }

    public class FulFillCallbackArgs
    {
        public bool isSuccess;
        public Action fullFillFxEndCallback;
        public FulfillExtraInfo fulfillExtraInfo;
    }

    public class IapController : LogicController
    {
        private PaymentInfoModal _paymentInfo;

        private Dictionary<string, PaymentHandler> _handlerNameDict;

        private bool _purchaseCallbackIsCalled = true;

        public IapController(Client client)
            : base(client)
        {
            _paymentInfo = new PaymentInfoModal();
        }

        protected override void Initialization()
        {
            base.Initialization();

            _handlerNameDict = new Dictionary<string, PaymentHandler>();
            _handlerNameDict.Add(ShopType.Store, new StorePaymentHandler());
            _handlerNameDict.Add(ShopType.PiggyBank, new PiggyPaymentHandler());
            _handlerNameDict.Add(ShopType.SeasonPass, new SeasonPassPaymentHandler());
            _handlerNameDict.Add(ShopType.SeasonQuest, new SeasonQuestPassPaymentHandler());
            _handlerNameDict.Add(ShopType.GoldenWheel, new GoldenWheelPaymentHandler());
            _handlerNameDict.Add(ShopType.Deal, new DealOfferPaymentHandler());
            _handlerNameDict.Add(ShopType.Activity_Valentine2022, new Activity_ValentinesDay_PaymentHandler());
            _handlerNameDict.Add(ShopType.NewBieQuestBoost, new CommonPaymentHandler(ShopType.NewBieQuestBoost));
            _handlerNameDict.Add(ShopType.SlotDeal, new CommonPaymentHandler(ShopType.SlotDeal));
            _handlerNameDict.Add(ShopType.CrazeSmash, new CrazeSmash_PaymentHandler());
            _handlerNameDict.Add(ShopType.TreasureRaidDeal, new TreasureRaidPaymentHandler());
            _handlerNameDict.Add(ShopType.CoinDash, new CommonPaymentHandler(ShopType.CoinDash));
            _handlerNameDict.Add(ShopType.Lotto, new LottoPaymentHandler());
            _handlerNameDict.Add(ShopType.JulyCarnival, new JulyCarnivalPaymentHandler());
            _handlerNameDict.Add(ShopType.SuperSpinX, new SuperSpinXPaymentHandler());
            _handlerNameDict.Add(ShopType.RushPass,new RushPassRaidHandler());
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            base.OnGetInfoBeforeEnterLobby(sGetInfoBeforeEnterLobby);
            _paymentInfo.UpdateModelData(sGetInfoBeforeEnterLobby.SGetPaymentBaseInfo.BaseInfo);
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (_paymentInfo.LastTimeUpdateData == 0)
            {
                await _paymentInfo.FetchModalDataFromServerAsync();
            }

            //TODO 这里有潜在风险，如果热更配置了新的商品，游戏类重启是不能拉取到最新的商品信息的
            if (!SDK.GetInstance().iapManager.IsInitialized())
            {
                SDK.GetInstance().iapManager.Init(_paymentInfo.GetConsumableProductIds());
            }

        }

        public bool IsPaymentInitialized()
        {
            return SDK.GetInstance().iapManager.IsInitialized();
        }

        public void GetPlatformProductInfo()
        {
            var products = SDK.GetInstance().iapManager.GetAllProductInfo();

            if (products == null)
            {
                XDebug.Log("NO PRODUCT INFO AVAIlABLE");
            }


            if (products != null && products.Length > 0)
            {
                for (var i = 0; i < products.Length; i++)
                {
                    XDebug.Log("Pid:" + products[i].definition.id);
                }
            }
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.CheckProcessingTransaction);
        }

        protected void OnSceneSwitchEnd(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY && _paymentInfo != null && _paymentInfo.IsInitialized())
            {
                if (SDK.GetInstance().iapManager.IsInitialized())
                {
                    //先尝试验证本地未验证的订完，再从服务器拉取未完成的订单，然后尝试补单
                    SDK.GetInstance().iapManager.VerifyUnfulfilledPayment((callback) =>
                    {
                        SDK.GetInstance().iapManager.RequestSpecifiedUnfulfilledPayment(OnReplenishmentOrderPurchaseCallback);
                    });
                }
                
                SyncStoreInfo();
            }
            handleEndCallback?.Invoke();
        }

        protected async void SyncStoreInfo()
        {
            await GetPaymentHandler<StorePaymentHandler>().GetShopInfo();
        }

        public async void BuyProduct(ShopItemConfig shopItemConfig)
        {
            var products = Dlugin.SDK.GetInstance().iapManager.GetAllProductInfo();
            if (products == null || products.Length <= 0)
            {
                CommonNoticePopup.ShowCommonNoticePopUp("UI_NOTICE_IAP_NOT_READY",
                    () => { XDebug.Log("Payment Not Ready!"); });
                return;
            }

            Product product = null;
            for (var i = 0; i < products.Length; i++)
            {
                if (products[i].definition.id == shopItemConfig.ProductId)
                {
                    product = products[i];
                    break;
                }
            }

            if (product == null)
            {
                GetPlatformProductInfo();

                CommonNoticePopup.ShowCommonNoticePopUp("UI_NOTICE_IAP_NOT_READY",
                    () => { XDebug.Log("Payment Not Ready!"); });
                return;
            }

            OnStartPayment(shopItemConfig);

            await ViewManager.Instance.ShowScreenLoadingView();
#if PRODUCTION_PACKAGE
            SDK.GetInstance().iapManager.PurchaseProduct(shopItemConfig.ProductId, OnPurchaseCallback);
#else

            _purchaseCallbackIsCalled = false;

            if (SDK.GetInstance().iapManager.IsProductAlreadyOwned(shopItemConfig.ProductId))
            {
                SDK.GetInstance().iapManager.VerifyUnfulfilledPayment((callback) =>
                {
                    SDK.GetInstance().iapManager.RequestSpecifiedUnfulfilledPayment(OnReplenishmentOrderPurchaseCallback, shopItemConfig.ProductId);
                });
                return;
            }

            if (SandBoxEnabled())
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR             
                SDK.GetInstance().iapManager.SandBoxPurchaseProduct(shopItemConfig.ProductId, OnPurchaseCallback);
#endif
            }
            else
            {
                SDK.GetInstance().iapManager.PurchaseProduct(shopItemConfig.ProductId, OnPurchaseCallback);
            }
#endif

            WaitForSeconds(120, () =>
            {
                if (!_purchaseCallbackIsCalled)
                {
                    ViewManager.Instance.HideScreenLoadingView();
                    SDK.GetInstance().iapManager.ResetIapState();
                }
            });
        }
#if !PRODUCTION_PACKAGE
        bool SandBoxEnabled()
        {
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
            return false;
#endif
            return UIDebugger.sandboxIapEnabled;
        }
#endif

        protected void OnStartPayment(ShopItemConfig shopItemConfig)
        {

        }

        protected void OnPaymentEndWithSuccess(PurchaseCallbackArgs purchaseCallbackArgs)
        {
            ViewManager.Instance.HideScreenLoadingView();

            if (purchaseCallbackArgs.extraInfo != null)
            {
                try
                {
                    var verifyExtraInfo = ProtocolUtils.Get<VerifyExtraInfo>(purchaseCallbackArgs.extraInfo);

                    if (verifyExtraInfo != null)
                    {
                        var paymentHandler = GetPaymentHandler(verifyExtraInfo.Item.ShopType);
                        if (paymentHandler != null)
                        {
                            paymentHandler.HandlePaymentSuccess(purchaseCallbackArgs, verifyExtraInfo);
                        }
                    }
                }
                catch (Exception exception)
                {
                    XDebug.LogError(exception.Message);
                    XDebug.LogError(exception.StackTrace);
                }
            }
        }

        public string GetLocalPrice(ShopItemConfig shopItemConfig)
        {
            if (shopItemConfig != null)
            {
                var productInfos = SDK.GetInstance().iapManager.GetAllProductInfo();
                if (productInfos.Length > 0)
                {
                    for (var i = 0; i < productInfos.Length; i++)
                    {
                        if (productInfos[i].definition.id == shopItemConfig.ProductId)
                            return productInfos[i].metadata.localizedPriceString;
                    }
                }
            }

            return string.Empty;
        }



        public void FulfillPayment(PurchaseCallbackArgs callbackArgs, VerifyExtraInfo verifyExtraInfo, Action<bool, FulfillExtraInfo, Action> callback, bool needAutoSettleItems = true, int startIndex = 1)
        {
            SDK.GetInstance().iapManager.FulfillPaymentWithServer(callbackArgs.productId, (fulfillResult, bytes) =>
            {
                var fulfillExtraInfo = fulfillResult ? ProtocolUtils.Get<FulfillExtraInfo>(bytes) : null;

                if (fulfillExtraInfo != null && fulfillExtraInfo.UserProfile != null)
                {
                    EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
                }

                callback?.Invoke(fulfillResult, fulfillExtraInfo,
                    () =>
                    {
                        if (fulfillExtraInfo != null && needAutoSettleItems)
                        {
                            ItemSettleHelper.SettleItems(fulfillExtraInfo.RewardItems,
                                () =>
                                {
                                    EventBus.Dispatch(new EventPaymentFinish(verifyExtraInfo));
                                    AdController.Instance.RefreshAdConfigFromPlatform();
                                }, startIndex, "Purchase");
                        }
                        else if (fulfillExtraInfo != null)
                        {
                            EventBus.Dispatch(new EventPaymentFinish(verifyExtraInfo));

                            AdController.Instance.RefreshAdConfigFromPlatform();
                        }
                    });
            });
        }

        public PaymentHandler GetPaymentHandler(string shopType)
        {
            if (_handlerNameDict.ContainsKey(shopType))
            {
                return _handlerNameDict[shopType];
            }

            return null;
        }
        public T GetPaymentHandler<T>() where T : PaymentHandler
        {
            foreach (var item in _handlerNameDict)
            {
                if (item.Value is T)
                {
                    return (T)item.Value;
                }
            }

            return null;
        }

        protected void OnPaymentEndWithFailed(string productId, PurchaseFailureReason failureReason)
        {
            ViewManager.Instance.HideScreenLoadingView();
            XDebug.Log("OnPaymentEndWithFailed:" + productId);

            CommonNoticePopup.ShowCommonNoticePopUp("UI_PURCHASE_FAILED", () => { });

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPurchaseFail,
                ("productId", productId),
                ("failureReason", failureReason.ToString()));
        }

        protected void OnReplenishmentOrderPurchaseCallback(PurchaseCallbackArgs purchaseCallbackArgs)
        {
            OnPurchaseCallback(purchaseCallbackArgs);
        }

        protected void OnPurchaseCallback(PurchaseCallbackArgs purchaseCallbackArgs)
        {
            _purchaseCallbackIsCalled = true;

            if (purchaseCallbackArgs.isSuccess)
            {
                if (!ViewManager.Instance.IsInSwitching())
                    OnPaymentEndWithSuccess(purchaseCallbackArgs);


                var verifyExtraInfo = ProtocolUtils.Get<VerifyExtraInfo>(purchaseCallbackArgs.extraInfo);
                
                if (verifyExtraInfo != null && verifyExtraInfo.Item != null)
                    LogPurchaseSuccessBiEvent(verifyExtraInfo.Item);
            }
            else
            {
                if (purchaseCallbackArgs.productId != null)
                    OnPaymentEndWithFailed(purchaseCallbackArgs.productId, purchaseCallbackArgs.failureReason);
                else
                {
                    ViewManager.Instance.HideScreenLoadingView();
                }
            }
        }

        protected void LogPurchaseSuccessBiEvent(ShopItemConfig itemConfig)
        {
            float[] prices = {49.99f, 19.99f, 9.99f, 4.99f, 1.99f};

            string[] priceNames = {"4999", "1999", "999", "499", "199"};

            string priceName = null;


            for (var i = 0; i < prices.Length; i++)
            {
                if (Math.Abs(itemConfig.Price - prices[i]) < 0.1f)
                {
                    priceName = priceNames[i];
                    break;
                }
            }

            if (priceName != null)
            {
                var eventType = (BiEventFortuneX.Types.GameEventType) Enum.Parse(
                    typeof(BiEventFortuneX.Types.GameEventType), "GameEventPurchaseOnetime" + priceName);

                BiManagerGameModule.Instance.SendGameEvent(eventType);
            }

            if (itemConfig.ProductType == "shopcoin")
            {
                float[] shopCoinPrices = {19.99f, 9.99f, 4.99f, 1.99f};
                string[] shopCoinPrizeNames = {"1999", "999", "499", "199"};
                string shopPriceName = null;
                
                for (var i = 0; i < shopCoinPrices.Length; i++)
                {
                    if (Math.Abs(itemConfig.Price - shopCoinPrices[i]) < 0.1f)
                    {
                        shopPriceName = shopCoinPrizeNames[i];
                        break;
                    }
                }

                if (shopPriceName != null)
                {
                    var eventType = (BiEventFortuneX.Types.GameEventType) Enum.Parse(
                        typeof(BiEventFortuneX.Types.GameEventType), "GameEventPurchaseShopCoin" + priceName);

                    BiManagerGameModule.Instance.SendGameEvent(eventType);
                }
            }
        }
     
        public bool IsStoreBonusCollectable()
        {
            if (_paymentInfo != null)
            {
                return _paymentInfo.IsStoreBonusCollectable();
            }

            return false;
        }

        public async void UpdateStoreBonusCountDown()
        {
            await _paymentInfo.FetchModalDataFromServerAsync();
        }
    }
}
