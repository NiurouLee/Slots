using System;
using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIADTaskSettlement")]
    public class AdCoinRewardClaimPopup : Popup
    {
        [ComponentBinder("RewardGroup")] public Transform rewardGroup;

        [ComponentBinder("CollectButton")] public Button claimButton;
 
        private Action _claimFinishCallback;

        private string source = "AdReward";

        private ulong coinReward = 0;
        
        public AdCoinRewardClaimPopup(string address)
            : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            claimButton.onClick.AddListener(OnClicked);
        }

        protected async void OnClicked()
        {
            claimButton.interactable = false;

            var rewardCoin = rewardGroup.GetChild(0).Find("StandardType/RewardIcon");
            await XUtility.FlyCoins(rewardCoin, new EventBalanceUpdate(coinReward, source));
            _claimFinishCallback?.Invoke();
            Close();
        }

        public void SetRewardCoin(ulong rewardCoin, Action claimFinishCallback, string inSource)
        {
            var countTextTrans = rewardGroup.GetChild(0).Find("StandardType/CountText");
            var countText = countTextTrans.GetComponent<TMP_Text>();
            countText.text = rewardCoin.GetCommaFormat();
            source = inSource;
            coinReward = rewardCoin;
            _claimFinishCallback = claimFinishCallback;
        }
    }
}
