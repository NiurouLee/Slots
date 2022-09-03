// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/14/20:08
// Ver : 1.0.0
// Description : AdMysteryGiftPopup.cs
// ChangeLog :
// **********************************************

using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIMysteryGift")]
    
    public class AdMysteryGiftPopup:Popup
    {
        [ComponentBinder("WatchButton")] 
        protected Button _watchButton;

        private PopupArgs popupArgs;
        public AdMysteryGiftPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(900, 768);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _watchButton.onClick.AddListener(OnWatchRvClicked);
        }

        protected void OnWatchRvClicked()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.MysteryGift,false))
            {
                _watchButton.interactable = false;
                AdController.Instance.TryShowRewardedVideo(eAdReward.MysteryGift, OnWatchRvFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
        }

        protected async void OnWatchRvFinished(bool success, string failed)
        {
            if (success)
            {
                var claimResult = await AdController.Instance.ClaimRvReward(eAdReward.MysteryGift);
               
                if (claimResult != null)
                {
                    Close();
                    
                    var claimPopup = await PopupStack.ShowPopup<AdRewardClaimPopup>();
                  
                    claimPopup.viewController.SetUpClaimUI(claimResult, eAdReward.AdTask, null);
                }
                else
                {
                    XDebug.Log("ClaimRewardFromServerFailed");
                    _watchButton.interactable = true;
                }
            }
            else
            {
                _watchButton.interactable = true;
            }
        }
        

        protected override void SetUpController(object inExtraData, object inExtraAsyncData = null)
        {
            popupArgs = (PopupArgs) inExtraData;
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();

            if (popupArgs != null && popupArgs.source == "BackToLobby")
            {
                AdLogicManager.Instance.specialOrder = false;
                if (AdController.Instance.ShouldShowInterstitial(eAdInterstitial.BackToLobby, false))
                {
                    AdController.Instance.TryShowInterstitial(eAdInterstitial.BackToLobby);
                }
            }
        }
    }
}