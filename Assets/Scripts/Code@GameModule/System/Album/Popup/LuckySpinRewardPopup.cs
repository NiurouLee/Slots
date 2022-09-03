// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/14:24
// Ver : 1.0.0
// Description : LuckySpinRewardPopup.cs
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
    [AssetAddress("UILuckySpinRewardPopup")]
    public class LuckySpinRewardPopup : Popup<LuckySpinRewardPopupViewController>
    {
        [ComponentBinder("Root/Bonus")] public RectTransform bonus;

        [ComponentBinder("Root/BottomGroup/CollectButton")]
        public Button collectButton;

        [ComponentBinder("Root/BottomGroup/LaterButton")]
        public Button laterButton;

        public LuckySpinRewardPopup(string address)
            : base(address)
        {
        }
    }

    public class LuckySpinRewardPopupViewController : ViewController<LuckySpinRewardPopup>
    {
        private Action rewardCollectFinishAction;

        protected override void SubscribeEvents()
        {
            view.collectButton.onClick.AddListener(OnCollectButtonClicked);
            view.laterButton.onClick.AddListener(OnLaterButtonClicked);
        
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        protected void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();   
        }
 
        public void SetUpRewardUI(RepeatedField<Reward> luckySpinReward, Action inRewardCollectFinishAction,
            bool showLater = true)
        {
            rewardCollectFinishAction = inRewardCollectFinishAction;
            XItemUtility.InitItemsUI(view.bonus, luckySpinReward, view.bonus.Find("CommonCell"),
                RewardDescribeFunction);

            view.laterButton.gameObject.SetActive(showLater);
        }

        public string RewardDescribeFunction(List<Item> items)
        {
            if (items != null && items.Count > 0)
            {
                switch (items[0].Type)
                {
                    case Item.Types.Type.CardPackage:
                        return "+" + items.Count;

                    case Item.Types.Type.Coin:
                    {
                        ulong totalAmount = 0;

                        for (int i = 0; i < items.Count; i++)
                        {
                            totalAmount += items[i].Coin.Amount;
                        }

                        return ((long) totalAmount).GetCommaOrSimplify(7);
                    }
                    case Item.Types.Type.Emerald:
                    {
                        ulong totalAmount = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            totalAmount += items[i].Emerald.Amount;
                        }

                        return ((long) totalAmount).GetCommaOrSimplify(7);
                    }

                    case Item.Types.Type.VipPoints:
                    {
                        ulong totalAmount = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            totalAmount += items[i].VipPoints.Amount;
                        }

                        return "+" + ((long) totalAmount).GetCommaOrSimplify(7);
                    }
                }
            }

            return "";
        }

        public async void OnCollectButtonClicked()
        {
            view.collectButton.interactable = false;
            view.laterButton.interactable = false;
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinResult,("Operation:", "LuckySpinCollectNow"),("OperationId","2"));

            var sCollectLuckySpinRewards = await Client.Get<AlbumController>().ClaimLuckySpinReward();

            if (sCollectLuckySpinRewards != null && view.transform != null)
            {
                var coins = XItemUtility.GetItems(sCollectLuckySpinRewards.Rewards, Item.Types.Type.Coin);

                if (coins != null && coins.Count > 0)
                {
                    ulong totalCoinCount = 0;
                    for (int i = 0; i < coins.Count; i++)
                    {
                        totalCoinCount += coins[i].Coin.Amount;
                    }

                    var coinItemUi = XItemUtility.GetItemUI(view.bonus, Item.Types.Type.Coin);


                    await XUtility.FlyCoins(coinItemUi, new EventBalanceUpdate(totalCoinCount, "LuckySpinReward"));
                }

                var emerald = XItemUtility.GetItems(sCollectLuckySpinRewards.Rewards, Item.Types.Type.Emerald);

                if (emerald != null && emerald.Count > 0)
                {
                    ulong totalEmeraldCount = 0;
                    for (int i = 0; i < emerald.Count; i++)
                    {
                        totalEmeraldCount += emerald[i].Emerald.Amount;
                    }

                    EventBus.Dispatch(new EventEmeraldBalanceUpdate(totalEmeraldCount, "LuckySpinReward"));
                }

                var items = XItemUtility.FilterOutItemsByType(sCollectLuckySpinRewards.Rewards,
                    new List<Item.Types.Type>()
                    {
                        Item.Types.Type.Coin, Item.Types.Type.Emerald
                    });

                ItemSettleHelper.SettleItems(items);

                rewardCollectFinishAction?.Invoke();

                view.Close();
            }
            else
            {
                XDebug.LogError("LuckySpinClaimRewardError");
                view.Close();
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            SoundController.PlaySfx("General_GiftBoxOpen");
        }

        public void OnLaterButtonClicked()
        {
        
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinResult,("Operation:", "LuckySpinLater"),("OperationId","1"));

            view.Close();
        }
    }
}