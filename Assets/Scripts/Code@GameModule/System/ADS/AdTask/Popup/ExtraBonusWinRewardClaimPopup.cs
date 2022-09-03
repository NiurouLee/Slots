// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/23/16:53
// Ver : 1.0.0
// Description : ExtraBonusWinRewardClaimPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
     
     [AssetAddress("UIADTaskSettlement")]
    public class ExtraBonusWinRewardClaimPopup:Popup<ExtraBonusWinRewardClaimPopupViewController>
    {
        [ComponentBinder("RewardGroup")]
        public Transform rewardGroup;
        
        [ComponentBinder("CollectButton")]
        public Button claimButton;

        [ComponentBinder("ExtraWinBonusText")] public Transform extraWinBonusText;
        
        [ComponentBinder("CommonText")] public Transform commonText;

        public ExtraBonusWinRewardClaimPopup(string address)
            : base(address)
        {
            
        }
    }

    public class ExtraBonusWinRewardClaimPopupViewController : ViewController<ExtraBonusWinRewardClaimPopup>
    {
        protected eAdReward adPlaceId;
        protected Action claimFinishCallback;

        protected ulong coinAmount;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.claimButton.onClick.AddListener(OnClaimButtonClicked);
            
            view.claimButton.interactable = false;
            
            view.extraWinBonusText.gameObject.SetActive(true);
            view.commonText.gameObject.SetActive(false);
        }
        
        public void SetUpClaimUI(ulong inCoinAmount, eAdReward adPlace, Action finishCallback)
        {
            adPlaceId = adPlace;
            view.claimButton.interactable = true;
            claimFinishCallback = finishCallback;

            Item coinItem = new Item();

            coinAmount = inCoinAmount;
            
            coinItem.Coin = new Item.Types.Coin();
            coinItem.Coin.Amount = coinAmount;
            coinItem.Type = Item.Types.Type.Coin;

            RepeatedField<Item> items = new RepeatedField<Item>();
            
            items.Add(coinItem);
            
            XItemUtility.InitItemsUI(view.rewardGroup, items,view.rewardGroup.Find("RewardCell"),XItemUtility.GetItemRewardFullDescText);
        }

        protected async void OnClaimButtonClicked()
        {
            view.claimButton.interactable = false;

            await XUtility.FlyCoins(view.claimButton.transform,
                new EventBalanceUpdate(coinAmount, adPlaceId.ToString()));

            view.Close();

            claimFinishCallback?.Invoke();
        }
    }
}