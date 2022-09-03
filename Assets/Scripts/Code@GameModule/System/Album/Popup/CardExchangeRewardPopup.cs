// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/22/13:50
// Ver : 1.0.0
// Description : CardExchangeRewardPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICardExchangePackPopup")]
    public class CardExchangeRewardPopup : Popup<CardExchangeRewardPopupViewController>
    {
        [ComponentBinder("Root/Bonus/Bonus01/CurrencyReward/Bonus")] public HorizontalLayoutGroup layoutGroup;
        [ComponentBinder("Root/Bonus/Bonus01/CurrencyReward/Bonus/CoinText")] public Text coinText;
        [ComponentBinder("Card")] public Transform cardRewardNode;
 
        [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        public Button confirmButton;

        public CardExchangeRewardPopup(string address)
            : base(address)
        {
        }
    }

    public class CardExchangeRewardPopupViewController : ViewController<CardExchangeRewardPopup>
    {
        private Action rewardCollectFinishAction;

        private Reward exchangeReward;

        protected override void SubscribeEvents()
        {
            view.confirmButton.onClick.AddListener(OnCollectButtonClicked);
        }

        public void SetUpRewardUI(Reward inExchangeReward, Action inRewardCollectFinishAction)
        {
            exchangeReward = inExchangeReward;
            rewardCollectFinishAction = inRewardCollectFinishAction;

            if (exchangeReward != null)
            {
               //view.Hide();
                var coinItem = XItemUtility.GetCoinItem(exchangeReward.Items);
                if (coinItem != null)
                {
                    view.coinText.text = coinItem.Coin.Amount.GetCommaFormat();
                }

                var cardPackage = XItemUtility.GetItem(exchangeReward.Items, Item.Types.Type.CardPackage);
                if (cardPackage == null)
                {
                    view.cardRewardNode.gameObject.SetActive(false);
                }

                // view.Show();
                // view.layoutGroup.enabled = true;

                // Canvas.ForceUpdateCanvases();
            }
        }

        // public string RewardDescribeFunction(List<Item> items)
        // {
        //     if (items != null && items.Count > 0)
        //     {
        //         switch (items[0].Type)
        //         {
        //             case Item.Types.Type.CardPackage:
        //                 return "+" + items.Count;
        //
        //             case Item.Types.Type.Coin:
        //             {
        //                 ulong totalAmount = 0;
        //
        //                 for (int i = 0; i < items.Count; i++)
        //                 {
        //                     totalAmount += items[i].Coin.Amount;
        //                 }
        //
        //                 return ((long) totalAmount).GetCommaOrSimplify(7);
        //             }
        //             case Item.Types.Type.Emerald:
        //             {
        //                 ulong totalAmount = 0;
        //                 for (int i = 0; i < items.Count; i++)
        //                 {
        //                     totalAmount += items[i].Emerald.Amount;
        //                 }
        //
        //                 return ((long) totalAmount).GetCommaOrSimplify(7);
        //             }
        //
        //             case Item.Types.Type.VipPoints:
        //             {
        //                 ulong totalAmount = 0;
        //                 for (int i = 0; i < items.Count; i++)
        //                 {
        //                     totalAmount += items[i].VipPoints.Amount;
        //                 }
        //
        //                 return "+" + ((long) totalAmount).GetCommaOrSimplify(7);
        //             }
        //         }
        //     }
        //
        //     return "";
        // }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            SoundController.PlaySfx("General_GiftBoxOpen");
        }

        public async void OnCollectButtonClicked()
        {
            view.confirmButton.interactable = false;

            if (exchangeReward != null)
            {
                var coins = XItemUtility.GetItems(exchangeReward.Items, Item.Types.Type.Coin);

                if (coins != null && coins.Count > 0)
                {
                    ulong totalCoinCount = 0;
                    for (int i = 0; i < coins.Count; i++)
                    {
                        totalCoinCount += coins[i].Coin.Amount;
                    }
                    
                    await XUtility.FlyCoins(view.confirmButton.transform, new EventBalanceUpdate(totalCoinCount, "LuckySpinReward"));
                }

           
                var items = XItemUtility.FilterOutItemsByType(new RepeatedField<Reward>(){exchangeReward},
                    new List<Item.Types.Type>()
                    {
                        Item.Types.Type.Coin, Item.Types.Type.Emerald
                    });

                if(items.Count > 0)
                    ItemSettleHelper.SettleItems(items);
            }
            rewardCollectFinishAction?.Invoke();
            view.Close();
        }
    }
}