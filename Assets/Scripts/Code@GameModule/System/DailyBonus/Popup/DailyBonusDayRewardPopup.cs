// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/23/18:42
// Ver : 1.0.0
// Description : DailyBonusDayRewardPopUp.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyBonusDayRewardPopup : Popup<ViewController>
    {
        [ComponentBinder("RewardGroup")] public Transform rewardGroup;

        [ComponentBinder("ADSConfirmButton")] public Button adsConfirmButton;

        [ComponentBinder("AdCollectButton")] public Button adCollectButton;

        [ComponentBinder("CollectButton")] public Button collectButton;

        [ComponentBinder("Root/BottomGroup/ADSConfirmButton/Label")]
        public TMP_Text additionLabel;

        private Reward _reward;

        private bool disableShowAdCollectButton = false;

        public DailyBonusDayRewardPopup(string address)
            : base(address)
        {
        }

        public void InitRewardContent(Reward reward)
        {
            XItemUtility.InitItemsUI(rewardGroup, reward.Items, rewardGroup.Find("DailyBonusCell"));

            _reward = reward;

            adCollectButton.gameObject.SetActive(false);
            adsConfirmButton.gameObject.SetActive(false);
            collectButton.gameObject.SetActive(false);

            viewController.WaitForSeconds(1, UpdateButtonStatus);
        }

        private void UpdateButtonStatus()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.DailyBonusRV, false))
            {
                adsConfirmButton.gameObject.SetActive(true);
                collectButton.gameObject.SetActive(false);
                adCollectButton.gameObject.SetActive(true);

                SetAdButtonInteractable(true);

                additionLabel.text = Client.Get<DailyBonusController>().GetWatchRvExtraCoinDesc();
            }
            else
            {
                adCollectButton.gameObject.SetActive(false);
                adsConfirmButton.gameObject.SetActive(false);
                collectButton.gameObject.SetActive(true);
                collectButton.interactable = true;
                SetAdButtonInteractable(false);
            }
        }

        protected void SetAdButtonInteractable(bool interactable)
        {
            adCollectButton.interactable = interactable;
            adsConfirmButton.interactable = interactable;
        }

        protected void OnWatchRewardClicked()
        {
            if (!ADSController.ShouldShowRV(eAdReward.DailyBonusRV))
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                SetAdButtonInteractable(true);
            }
            else
            {
                SetAdButtonInteractable(false);

                ADSController.TryShowRewardedVideo(
                    eAdReward.DailyBonusRV, (watchSuccess, b) =>
                    {
                        if (watchSuccess)
                            CollectReward(adsConfirmButton, true);
                        else
                        {
                            UpdateButtonStatus();
                        }
                    });
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            adCollectButton.onClick.AddListener(OnAdCollectClicked);
            collectButton.onClick.AddListener(OnCollectClicked);
            adsConfirmButton.onClick.AddListener(OnWatchRewardClicked);
        }

        protected void OnCollectClicked()
        {
            CollectReward(collectButton, false);
        }

        protected void OnAdCollectClicked()
        {
            CollectReward(adCollectButton, false);
        }


        protected async void CollectReward(Button inCollectButton, bool watchRv = false)
        {
            collectButton.interactable = false;
            adsConfirmButton.interactable = false;
            adCollectButton.interactable = false;

            var result = await Client.Get<DailyBonusController>().CollectWeekReward(watchRv);

            if (result != null)
            {
                var items = XItemUtility.GetItems(result.Rewards);
                Item coinItem = XItemUtility.GetCoinItem(items);

                if (coinItem != null)
                {
                    var coinRewardAmount = coinItem.Coin.Amount;

                    if (watchRv)
                    {
                        float extraAddition = (float) Client.Get<DailyBonusController>().GetWatchRvExtraCoinAddition() /
                                              100;
                        coinRewardAmount = (ulong) Mathf.Floor(coinRewardAmount * (1 + extraAddition));
                    }

                    coinItem.Coin.Amount = coinRewardAmount;

                    XItemUtility.RefreshItemsUI(rewardGroup, _reward.Items, rewardGroup.Find("DailyBonusCell"));

                    await XUtility.FlyCoins(inCollectButton.transform,
                        new EventBalanceUpdate(coinRewardAmount, "DailyBonusWeekReward"));
                }

                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                    .GameEventDailyBonusDaily);
                EventBus.Dispatch(new EventRefreshUserProfile());
               
                ItemSettleHelper.SettleItems(items, ()=>
                {
                    DispatchCloseClickAction();
                    Close();
                });
            }
            else
            {
                Close();
            }
        }
    }
}