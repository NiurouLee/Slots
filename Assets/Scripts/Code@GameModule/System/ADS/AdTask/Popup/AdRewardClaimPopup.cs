// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/14/14:36
// Ver : 1.0.0
// Description : AdRewardClaimPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;
using UnityEngine;
namespace GameModule
{
    [AssetAddress("UIADTaskSettlement")]
    public class AdRewardClaimPopup:Popup<AdRewardClaimPopupViewController>
    {
        [ComponentBinder("RewardGroup")]
        public Transform rewardGroup;
        
        [ComponentBinder("CollectButton")]
        public Button claimButton;

        public AdRewardClaimPopup(string address)
            : base(address)
        {
            
        }
    }

    public class AdRewardClaimPopupViewController : ViewController<AdRewardClaimPopup>
    {
        protected SClaimAdReward _sClaimAdReward;
        protected eAdReward adPlaceId;
        protected Action claimFinishCallback;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.claimButton.onClick.AddListener(OnClaimButtonClicked);
            view.claimButton.interactable = false;
        }
    
        public void SetUpClaimUI(SClaimAdReward sClaimAdReward, eAdReward adPlace, Action finishCallback)
        {
            _sClaimAdReward = sClaimAdReward;
            adPlaceId = adPlace;
            view.claimButton.interactable = true;
            claimFinishCallback = finishCallback;
            
            XItemUtility.InitItemsUI(view.rewardGroup,sClaimAdReward.Rewards[0].Items,view.rewardGroup.Find("RewardCell"));
        }

        protected async void OnClaimButtonClicked()
        {
            var items = _sClaimAdReward.Rewards[0].Items;

            view.claimButton.interactable = false;
            
            var coinItem = XItemUtility.GetItem(items, Item.Types.Type.Coin);

            if (coinItem != null)
            {
               await XUtility.FlyCoins(view.claimButton.transform, new EventBalanceUpdate(coinItem.Coin.Amount, adPlaceId.ToString()));
            }
            
            var emeraldItem = XItemUtility.GetItem(items, Item.Types.Type.Emerald);

            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, adPlaceId.ToString()));
            }
             
            ItemSettleHelper.SettleItems(items);
            
            view.Close();
            
            claimFinishCallback?.Invoke();
        }
    }
}