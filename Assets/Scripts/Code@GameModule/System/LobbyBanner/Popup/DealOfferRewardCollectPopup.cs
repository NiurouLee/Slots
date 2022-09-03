// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/09/20:49
// Ver : 1.0.0
// Description : StoreRewardCollectPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    
    [AssetAddress("UIFirstRechargeReward", "UIFirstRechargeRewardV")]
    public class DealOfferCollectPopup : Popup<DealOfferCollectPopupController>
    {
        [ComponentBinder("CoinCell")] public Transform coin;

        [ComponentBinder("ExtraItem")] public Transform extraItemTransform;

        [ComponentBinder("ConfirmButton")] public Button confirmButton;

        public CoinCommodityClaimView coinCommodityClaimView;
        public ExtraItemClaimView extraItemClaimView;


        public DealOfferCollectPopup(string address)
            : base(address)
        {
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            coinCommodityClaimView = AddChild<CoinCommodityClaimView>(coin);
            extraItemClaimView = AddChild<ExtraItemClaimView>(extraItemTransform);
        }

        // public override Vector3 CalculateScaleInfo()
        // {
        //     if (ViewManager.Instance.IsPortrait)
        //     {
        //         return new Vector3(0.8f, 0.8f, 0.8f);
        //     }
        //     return Vector3.zero;
        // }
    }

    public class DealOfferCollectPopupController : ViewController<DealOfferCollectPopup>
    {
        private Action<Action<bool, FulfillExtraInfo>> _collectActionHandler;

        private string _productType;

        private VerifyExtraInfo _verifyExtraInfo;
        // public override void OnViewDidLoad()
        // {
        //     base.OnViewDidLoad();
        // }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            view.confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        public void SetUpViewContent(VerifyExtraInfo verifyExtraInfo,
            Action<Action<bool, FulfillExtraInfo>> collectActionHandler)
        {
            if (verifyExtraInfo == null || verifyExtraInfo.Item.SubItemList.Count < 0)
                return;

            _verifyExtraInfo = verifyExtraInfo;

            _collectActionHandler = collectActionHandler;

            string productType = verifyExtraInfo.Item.ProductType;

            _productType = productType;

            view.extraItemClaimView.SetUpView(verifyExtraInfo.Item);

            view.coinCommodityClaimView.SetUpView(verifyExtraInfo.Item);
            view.coinCommodityClaimView.Show();
        }

        public void OnConfirmButtonClicked()
        {
            view.confirmButton.interactable = false;

            _collectActionHandler.Invoke((succeeded, fulfillExtraInfo) =>
            {
                if (succeeded)
                {
                    OnFulfilledSucceeded(fulfillExtraInfo);
                }
                else
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError("DealOffer[FulfillPaymentFailed]"));
                    view.Close();
                }
            });
        }

        public async void OnFulfilledSucceeded(FulfillExtraInfo fulfillExtraInfo)
        {
            var shopCoinItem = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.ShopCoin);
            await XUtility.FlyCoins(view.confirmButton.transform,
                new EventBalanceUpdate((long) shopCoinItem.ShopCoin.AdditionAmount, "IapDeal"));
            view.Close();

            EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
        }
    }
}