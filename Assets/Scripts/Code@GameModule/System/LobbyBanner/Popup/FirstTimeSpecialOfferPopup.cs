// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/03/14:43
// Ver : 1.0.0
// Description : FirstTimeSpecialOfferPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

namespace GameModule
{
    [AssetAddress("UIFirstRechargeMain", "UIFirstRechargeMainV")]
    public class FirstTimeSpecialOfferPopup:Popup<FirstTimeSpecialOfferPopupViewController>
    {
        [ComponentBinder("CurrentPriceText")] public Text currentPriceText;
        [ComponentBinder("OriginalPriceText")] public Text originalPriceText;
        [ComponentBinder("TimerText")] public Text timerText;
        [ComponentBinder("IntegralText")] public Text integralText;
        [ComponentBinder("PriceText")] public Text priceText;
        [ComponentBinder("PriceButton")] public Button buyButton;
        [ComponentBinder("PreviewButton")] public Button purchaseBenefit;

        public string closeReason;
        public FirstTimeSpecialOfferPopup(string address)
            : base(address)
        {
            
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();
           
            if (viewController.source != "LevelUp" && 
                viewController.source != "InsufficientBalance" && 
                viewController.source != "EnterLobby")
            {
                EventBus.Dispatch(new EventTriggerPopupPool("CloseFirstTimeSpecialOfferPopup"));
            }
        }
    }

    public class FirstTimeSpecialOfferPopupViewController : ViewController<FirstTimeSpecialOfferPopup>
    {
        private SGetAdvertisementItem _sGetAdvertisementItem;
        public string source;
        public override void BindingView(View inView, object inExtraData, object inAsyncExtraData = null)
        {
            base.BindingView(inView, inExtraData, inAsyncExtraData);

            _sGetAdvertisementItem = inAsyncExtraData as SGetAdvertisementItem;

            //_sGetAdvertisementItem.Adv.Id;
             
            if (inExtraData != null)
            {
                var popupArgs = inExtraData as PopupArgs;

                if (popupArgs != null)
                {
                    var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

                    string sceneType = "Lobby";

                    if (machineScene != null)
                    {
                        sceneType = Client.Get<MachineLogicController>().LastGameId;
                    }

                    source = popupArgs.source;
                    
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventSpecialOfferPop,
                        ("source", popupArgs.source), ("scene", sceneType));
                }
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            UpdateViewUiContent();
            
            EnableUpdate(2);
        }

        public override void Update()
        {
            var leftTime = Client.Get<BannerController>().GetDealCountDown(_sGetAdvertisementItem.Adv);
            view.timerText.text = XUtility.GetTimeText(leftTime);

            if (leftTime <= 0)
            {
                view.Close();
            }
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventDealOfferConsumeComplete>(OnDealOfferConsumeComplete);
        }

        protected  void OnDealOfferConsumeComplete(EventDealOfferConsumeComplete evt)
        {
            view.Close();
        }

        public void UpdateViewUiContent()
        {
            view.currentPriceText.text = "$" + _sGetAdvertisementItem.DealPayItem.Price;
            view.originalPriceText.text = "$" + _sGetAdvertisementItem.DealPayItem.OldPrice;
            view.integralText.text = _sGetAdvertisementItem.DealPayItem.SubItemList[0].ShopCoin.AdditionAmount.GetCommaFormat();
            view.priceText.text = "$" + _sGetAdvertisementItem.DealPayItem.Price;
            
            view.purchaseBenefit.onClick.AddListener(OnPurchaseBenefitClicked);
            view.buyButton.onClick.AddListener(OnBuyButtonClicked);
        }
        
        protected void OnBuyButtonClicked()
        {
            var shopItemConfig = _sGetAdvertisementItem.DealPayItem;
            
            Client.Get<IapController>().BuyProduct(_sGetAdvertisementItem.DealPayItem);
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventSpecialOfferPurchase,
                ("paymentId", shopItemConfig.PaymentId.ToString()),
                ("price", shopItemConfig.Price.ToString()),
                ("productType", shopItemConfig.ProductType));
        }

        protected async void OnPurchaseBenefitClicked()
        {
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(_sGetAdvertisementItem.DealPayItem.SubItemList);
            
        }
    }
}