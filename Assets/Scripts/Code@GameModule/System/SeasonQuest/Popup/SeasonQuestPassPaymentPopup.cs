// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/18/11:09
// Ver : 1.0.0
// Description : SeasonQuestPassPaymentPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestSeasonOnePass")]
    public class SeasonQuestPassPaymentPopup:Popup<SeasonQuestPassPaymentPopupViewController>
    {
        [ComponentBinder("PriceButton")]
        public Button priceButton;

        [ComponentBinder("PriceText")] 
        public Text priceText;  
       
        [ComponentBinder("IntegralText")] 
        public Text integralText;  
        
        [ComponentBinder("ExtraContentsButton")] 
        public Button purchaseBenefits;  
        
        
        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText/Icon")] 
        public Transform coinIcon;
         
        public SeasonQuestPassPaymentPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1150,768);
        }
    }
    
    public class SeasonQuestPassPaymentPopupViewController : ViewController<SeasonQuestPassPaymentPopup>
    {
        private ShopItemConfig _itemConfig;

        private string source;
        private string machineId;
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            view.priceButton.onClick.AddListener(OnBuyButtonClicked);
            view.purchaseBenefits.onClick.AddListener(OnPurchaseBenefitsButtonClicked);
        }


        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            if (_itemConfig != null)
            {
                view.priceText.text = "$" + _itemConfig.Price;
                var coin = XItemUtility.GetItem(_itemConfig.SubItemList, Item.Types.Type.Coin);
                view.integralText.text = coin.Coin.Amount.GetCommaFormat();
            }
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonQuestpassCheck,("source",source),("machineId",machineId));
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            _itemConfig = inExtraAsyncData as ShopItemConfig;

            var tuple = ((string, string))(inExtraData);

            source = tuple.Item1;
            machineId = tuple.Item2;
        }

        protected  void OnBuyButtonClicked()
        {
            Client.Get<IapController>().BuyProduct(_itemConfig);
        }

        public void OnPaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            view.priceButton.interactable = false;
            view.purchaseBenefits.interactable = false;

            Client.Get<IapController>()
                .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,async (success, fulfillExtraInfo, collectCallback) =>
                {
                    if (success)
                    {
                        var coinItem = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.Coin);

                        await XUtility.FlyCoins(view.coinIcon, new EventBalanceUpdate(coinItem.Coin.Amount, "SeasonPass"));
                        view.Close();
                        
                        EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
                        collectCallback.Invoke();
                      
                        ItemSettleHelper.SettleItems(fulfillExtraInfo.RewardItems,
                            async () =>
                            {
                                await Client.Get<SeasonQuestController>().RefreshSeasonQuestData();

                                BiManagerGameModule.Instance.SendGameEvent(
                                    BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonQuestpassBuy,
                                    ("source", source), ("machineId", machineId));

                                EventBus.Dispatch(new EventSeasonQuestCheckQuestFinish());
                                
                            }, 1, "Purchase");
                    }
                    else
                    {
                        view.priceButton.interactable = false;
                        view.purchaseBenefits.interactable = false;
                    }
                },false);
        }
        
        protected  async void OnPurchaseBenefitsButtonClicked()
        {
            SoundController.PlayButtonClick();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(_itemConfig.SubItemList);
        }
    }
}