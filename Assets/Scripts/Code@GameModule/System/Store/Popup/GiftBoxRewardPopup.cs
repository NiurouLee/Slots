// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/28/10:38
// Ver : 1.0.0
// Description : GiftBoxRewardPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIStoreGiftBoxRewardH", "UIStoreGiftBoxRewardV")]
    public class GiftBoxRewardPopup : Popup
    {
        [ComponentBinder("ConfirmButton")] public Button confirmButton;

        [ComponentBinder("Root/RewardGroup/StoreCell/StandardType/CountText")] 
        public TextMeshProUGUI countText;
        
        private Item giftBoxItem;

        private long rewardCoinCount;
        public GiftBoxRewardPopup(string assetAddress)
            : base(assetAddress)
        {
        }
        
        public void SetUpRewardUI(Item inGiftBoxItem)
        {
            giftBoxItem = inGiftBoxItem;
            EventBus.Dispatch(new EventGiftBoxSetToEmpty());
            rewardCoinCount = (long)giftBoxItem.ShopGiftBox.GiftBox.AccumulateCoins;
            countText.text = rewardCoinCount.GetCommaFormat();
            
        }

        protected override void SetUpController(object inExtraData, object inExtraAsyncData = null)
        {
            if (inExtraData != null)
            {
                var popupArgs = inExtraData as PopupArgs;

                if (popupArgs != null)
                {
                    giftBoxItem = popupArgs.extraArgs as Item;
                }
            }
        }
        
        protected override void OnViewSetUpped()
        {
            if(giftBoxItem != null)
                SetUpRewardUI(giftBoxItem);
        }

        public void SetUpRewardUI(long accumulateCoins)
        {
            rewardCoinCount = accumulateCoins;
            EventBus.Dispatch(new EventGiftBoxSetToEmpty());
            
            countText.text = rewardCoinCount.GetCommaFormat();
        }

        [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        private async void OnBtnConfirmClick()
        {
            confirmButton.interactable = false;
            SoundController.PlayButtonClick();
            var storePaymentHandler = Client.Get<IapController>().GetPaymentHandler<StorePaymentHandler>();

            bool success = await storePaymentHandler.ClaimGiftBox();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStoreGiftBoxCollect);
         
            if (success)
            {
                await XUtility.FlyCoins(confirmButton.transform,
                    new EventBalanceUpdate(rewardCoinCount, "GiftBox"));
                Close();
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("Server Error, Collect Failed");
            }
        }
        
       
    }
}