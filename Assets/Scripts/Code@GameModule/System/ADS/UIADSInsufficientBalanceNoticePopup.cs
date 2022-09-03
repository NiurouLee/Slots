using System;
using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIADSInsufficientBalanceNotice", "UIADSInsufficientBalanceNoticeV")]
    public class UIADSInsufficientBalanceNoticePopup : Popup
    {
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        public Button buttonCollect;

        [ComponentBinder("Root/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        private eAdReward _adPlaceID = eAdReward.Free;
        private ulong _coin;


        private Action _watchFvFinished;
        private Action _cancelWatchRv;

        public UIADSInsufficientBalanceNoticePopup(string address) : base(address) { }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (buttonCollect != null)
            {
                buttonCollect.onClick.AddListener(OnClick);
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAdsPop, ("adsType", "rewardVideo"), ("placeId", eAdReward.InsufficientBalanceRV.ToString()), ("userGroup", AdConfigManager.Instance.MetaData.GroupId.ToString()));
        }

        public void Set(ulong rewardCoinCount, eAdReward eAdReward)
        {
            _coin = rewardCoinCount;
            _adPlaceID = eAdReward;
            if (tmpTextIntegral != null)
            {
                tmpTextIntegral.text = rewardCoinCount.GetCommaFormat();
            }
            if (buttonCollect != null) { buttonCollect.interactable = true; }
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();

            var rewardCoinBase = ADSController.GetFirstBonusCount(eAdReward.InsufficientBalanceRV);
            var rewardCoin = ADSController.GetRewardCoin(rewardCoinBase);
           
            Set(rewardCoin, eAdReward.InsufficientBalanceRV);
        }

        private void OnClick()
        {
            if (!AdController.Instance.ShouldShowRV(_adPlaceID,false))
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
            else
            {
                if (buttonCollect != null) { buttonCollect.interactable = false; }
                
                buttonCollect.interactable = false;
                 
                AdController.Instance.TryShowRewardedVideo(_adPlaceID, OnWatchRvFinished);
            }
        }
        
        private async void OnWatchRvFinished(bool success, string reason)
        {
            if (success)
            {
                var baseReward = ADSController.GetFirstBonusCount(eAdReward.InsufficientBalanceRV);
                var rewardCoin = ADSController.GetRewardCoin(baseReward);
                var actualRandomReward = AdController.Instance.GetRandomRvReward(_adPlaceID, rewardCoin);
                var sClaimAdReward = await AdController.Instance.ClaimRvReward(_adPlaceID, actualRandomReward);

                if (sClaimAdReward != null)
                {
                    base.Close();
                    var rewardPopup = await PopupStack.ShowPopup<AdCoinRewardClaimPopup>();
                    rewardPopup.SetRewardCoin(actualRandomReward, _watchFvFinished, "InsufficientBalanceRV");
                    return;
                }
            }

            buttonCollect.interactable = true;
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();
            _cancelWatchRv?.Invoke();
        }

        public void BindUserAction(Action watchRvFinished, Action cancelWatchRv)
        {
            _watchFvFinished = watchRvFinished;
            _cancelWatchRv = cancelWatchRv;
        }
    }
}
