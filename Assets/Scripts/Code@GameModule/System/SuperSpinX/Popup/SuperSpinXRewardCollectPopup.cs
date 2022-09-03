// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/24/14:48
// Ver : 1.0.0
// Description : SuperSpinXRewardCollectPopup.cs
// ChangeLog :
// **********************************************
using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UISuperSpinXRewardCollectH", "UISuperSpinXRewardCollectV")]
    public class SuperSpinXRewardCollectPopup : Popup<SuperSpinXRewardCollectPopupViewController>
    {
        [ComponentBinder("Root/RewardGroup")] public RectTransform rewardGroup;

        [ComponentBinder("Root/RewardGroup/PlusText")]
        public Transform plusText;

        [ComponentBinder("Root/RewardGroup/Primary/CountText")]
        public TextMeshProUGUI coinCountText;

        [ComponentBinder("Root/RewardGroup/ExtraItem")]
        public Transform extraItemGroup;

        [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        public Button collectButton;

        public SuperSpinXRewardCollectPopup(string address)
            : base(address)
        {
        }
    }

    public class SuperSpinXRewardCollectPopupViewController : ViewController<SuperSpinXRewardCollectPopup>
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

            _collectActionHandler.Invoke((succeeded, fulfillExtraInfo) =>
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

        public void SetUpViewContent(VerifyExtraInfo verifyExtraInfo,
            Action<Action<bool, FulfillExtraInfo>> collectActionHandler)
        {
            if (verifyExtraInfo == null || verifyExtraInfo.Item.SubItemList.Count < 0)
                return;

            _verifyExtraInfo = verifyExtraInfo;
            _collectActionHandler = collectActionHandler;
            string productType = verifyExtraInfo.Item.ProductType;
            _productType = productType;

            //SpinXGameItem奖励给的金币，和卡包奖励数据在SuperSpinxGame里面
            var spinXItem = XItemUtility.GetItem(verifyExtraInfo.Item.SubItemList, Item.Types.Type.SuperSpinxGame);

            if (spinXItem != null && spinXItem.SuperSpinxGame.GameInfo != null &&
                spinXItem.SuperSpinxGame.GameInfo.Result != null)
            {
                var result = spinXItem.SuperSpinxGame.GameInfo.Result;
                var coinReward = XItemUtility.GetItem(result.Reward.Items, Item.Types.Type.Coin);

                if (coinReward != null)
                {
                    view.coinCountText.text = coinReward.Coin.Amount.GetCommaFormat();
                    view.coinCountText.DOCounter(0, (long)coinReward.Coin.Amount, 2.0f).OnComplete(() =>
                    {
                        view.coinCountText.text = coinReward.Coin.Amount.GetCommaFormat();
                    });
                }

                var skipList = new List<Item.Types.Type>() {Item.Types.Type.SuperSpinxGame};
                var benefitsReward = XItemUtility.GetItems(verifyExtraInfo.Item.SubItemList, skipList);

                for (var i = 0; i < result.Reward.Items.count; i++)
                {
                    if (result.Reward.Items[i].Type != Item.Types.Type.Coin)
                    {
                        benefitsReward.Insert(0, result.Reward.Items[i]);
                    }
                }

                if (benefitsReward.count > 0)
                {
                    XItemUtility.InitItemsUI(view.extraItemGroup, benefitsReward,
                        view.extraItemGroup.Find("CommonCell"));
                }
                else
                {
                    view.plusText.gameObject.SetActive(false);
                    view.extraItemGroup.gameObject.SetActive(false);
                }
            }
        }

        public async void OnFulfilledSucceeded(FulfillExtraInfo fulfillExtraInfo)
        {
            EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));

            var coinItem = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.Coin);
            if (coinItem != null)
            {
                view.coinCountText.DOKill();
                view.coinCountText.text = coinItem.Coin.Amount.GetCommaFormat();
                await XUtility.FlyCoins(view.collectButton.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "IAPCommon"));
            }
            
            view.Close();
        }
    }
}