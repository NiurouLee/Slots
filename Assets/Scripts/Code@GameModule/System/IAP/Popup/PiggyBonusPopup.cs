// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/28/20:39
// Ver : 1.0.0
// Description : PiggyBonusPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;

namespace GameModule 
{
    [AssetAddress("UIPiggyBonus")]
    public class PiggyBonusPopup: Popup<PiggyBonusPopupViewController>
    {
        [ComponentBinder("Root/MiddleGroup/IntegralGroup/IntegralText")]
        public Text integralText;

        [ComponentBinder("Root/BottomGroup/PriceButton/PriceText")]
        public Text priceText;

        [ComponentBinder("Root/BottomGroup/PriceButton")]
        public Button priceButton;

        [ComponentBinder("Root/BottomGroup/DetailButton")]
        public Button detailButton;    
        
        [ComponentBinder("Root/TopGroup/TimerGroup/TimerText")]
        public Text timerText;

        [ComponentBinder("Root/BottomGroup/OriginalPriceGroup/OriginalPriceText")]
        public Text originalPriceText;

        public PiggyBonusPopup(string address)
            :base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
    }
    public class PiggyBonusPopupViewController: ViewController<PiggyBonusPopup>
    {
        private SGetAdvertisementItem _sGetAdvertisementItem;
        private string source;
        protected override void SubscribeEvents()
        {
            view.priceButton.onClick.AddListener(OnPriceButtonClicked);
            view.detailButton.onClick.AddListener(OnDetailButtonClicked);
            base.SubscribeEvents();
            
            SubscribeEvent<EventCommonPaymentComplete>(OnCommonPaymentComplete);
        }

        public void OnCommonPaymentComplete(EventCommonPaymentComplete evt)
        {
            if (evt.shopType == ShopType.SlotDeal)
            {
                view.Close();
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            UpdateViewUiContent();
 
            var leftTime = Client.Get<BannerController>().GetDealCountDown(_sGetAdvertisementItem.Adv);

            if (leftTime <= 0)
            {
                view.Close();
            }
            else
            {
                WaitForSeconds(leftTime, () =>
                {
                    view.Close();
                });
            }
            
            EnableUpdate(2);
        }
        
        public override void BindingView(View inView, object inExtraData, object inAsyncExtraData = null)
        {
            base.BindingView(inView, inExtraData, inAsyncExtraData);

            _sGetAdvertisementItem = inAsyncExtraData as SGetAdvertisementItem;

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

                    source = GetTriggerSource();

                    BiManagerGameModule.Instance.SendGameEvent(
                        BiEventFortuneX.Types.GameEventType.GameEventPiggyBonusGiftboxPopup,
                        ("source", popupArgs.source), ("scene", sceneType));
                }
            }
        }

        public void UpdateViewUiContent()
        {
            view.priceText.text = "$" + _sGetAdvertisementItem.DealPayItem.Price;
            view.originalPriceText.text = "$" + _sGetAdvertisementItem.DealPayItem.OldPrice;
            var shopConfig = _sGetAdvertisementItem.DealPayItem;

            if (shopConfig != null && shopConfig.SubItemList[0].ShopCoin != null)
            {
                view.integralText.text = shopConfig.SubItemList[0].ShopCoin.AdditionAmount.GetCommaFormat();
            }
            else
            {
                view.integralText.text = "0";
            }
            
            // view.integralText.text = _sGetAdvertisementItem.DealPayItem.SubItemList[0].Coin.Amount
            //     .GetCommaFormat();
        }

        public void OnPriceButtonClicked()
        {
            var shopItemConfig = _sGetAdvertisementItem.DealPayItem;
            Client.Get<IapController>().BuyProduct(shopItemConfig);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPiggyBonusGiftboxBuy);
        }

        public async void OnDetailButtonClicked()
        {
            var shopItemConfig = _sGetAdvertisementItem.DealPayItem;
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
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
    }
}