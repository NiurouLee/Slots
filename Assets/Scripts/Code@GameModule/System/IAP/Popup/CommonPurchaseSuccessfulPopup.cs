// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/28/19:07
// Ver : 1.0.0
// Description : PurchaseSuccessfulPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
namespace GameModule 
{
    [AssetAddress("UICommonPurchaseSuccessful","UICommonPurchaseSuccessfulV")]
    public class CommonPurchaseSuccessfulPopup: Popup<CommonPurchaseSuccessfulPopupViewController>
    {
        [ComponentBinder("Root/RewardGroup")]
        public RectTransform rewardGroup;

        [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        public Button collectButton;

        public CommonPurchaseSuccessfulPopup(string address)
            :base(address)
        {
            
        }
    }
    public class CommonPurchaseSuccessfulPopupViewController: ViewController<CommonPurchaseSuccessfulPopup>
    {
        private Action<Action<bool, FulfillExtraInfo>> _collectActionHandler;
        
        private string _productType;
      
        private VerifyExtraInfo _verifyExtraInfo;
        
        protected override void SubscribeEvents()
        {
            view.collectButton.onClick.AddListener(OnCollectButtonClicked);
        }
        public void OnCollectButtonClicked()
        {
            view.collectButton.interactable = false;
            
            _collectActionHandler.Invoke((succeeded,fulfillExtraInfo) =>
            {
                if (succeeded)
                {
                    OnFulfilledSucceeded(fulfillExtraInfo);
                }
                else
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError("FulfillPaymentFailed"));
                    view.Close();
                }
            });
        }
        
        public void SetUpViewContent(VerifyExtraInfo verifyExtraInfo, Action<Action<bool, FulfillExtraInfo>> collectActionHandler)
        {
            if (verifyExtraInfo == null || verifyExtraInfo.Item.SubItemList.Count < 0)
                return;

            _verifyExtraInfo = verifyExtraInfo;
            _collectActionHandler = collectActionHandler;
            string productType = verifyExtraInfo.Item.ProductType;
            _productType = productType;
            
            XItemUtility.InitItemsUI(view.rewardGroup, verifyExtraInfo.Item.SubItemList, view.rewardGroup.Find("CommonCell"));
        }
        
        public async void OnFulfilledSucceeded(FulfillExtraInfo fulfillExtraInfo)
        {
            EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
 
            var coinItem =  XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.Coin);
            if (coinItem != null)
            {
                await XUtility.FlyCoins(view.collectButton.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "IAPCommon"));
            } 
            
            var shopCoin =  XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.ShopCoin);
         
            if (shopCoin != null)
            {
                await XUtility.FlyCoins(view.collectButton.transform,
                    new EventBalanceUpdate((long) shopCoin.ShopCoin.AdditionAmount, "IAPCommon"));
            } 
            
            var emeraldItem =  XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.Emerald);
            if(emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate((long) emeraldItem.Emerald.Amount, "IAPCommon"));
            }
            
            var shopEmeraldItem =  XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.ShopEmerald);
            if(emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate((long) shopEmeraldItem.ShopEmerald.Amount, "IAPCommon"));
            }
           
            view.Close();
        }
    }
}