//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 22:14
//  Ver : 1.0.0
//  Description : UIDailyMissionReward.cs
//  ChangeLog :
//  **********************************************

using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIDailyMissionRewardCellCommonH", "UIDailyMissionRewardCellCommonV")]
    public class DailyMissionRewardNormal : DailyMissionRewardBase
    {
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
         
            if (adsButton)
            {
                AdRewardedVideoPlacementMonitor.Bind(this.adsButton.gameObject, eAdReward.DailyMissionRV.ToString());
            }

            adsButton?.onClick.AddListener(OnBtnADSClick);
            confirmButton?.onClick.AddListener(OnBtnConfirmClick);
            collectButton?.onClick.AddListener(OnBtnCollectClick);
        }
        public DailyMissionRewardNormal()
        {
            _adPlaceID = eAdReward.DailyMissionRV;
        }

        public DailyMissionRewardNormal(string address) : base(address)
        {
            _adPlaceID = eAdReward.DailyMissionRV;
        }

        protected override void EnableView()
        {
            base.EnableView();
            gameObject.SetActive(false);
        }

        protected override void BindingComponent()
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        protected override long GetCoinAmount()
        {
            if (_missionController != null)
            {
                return _reward.GetCoinsAmount() + _missionController.GetExtraCoinAmount();
            }
            return _reward.GetCoinsAmount();
        }

        private void OnBtnCollectClick()
        {
            CollectReward(collectButton.transform);
        }

        protected virtual void OnBtnConfirmClick()
        {
            CollectReward(confirmButton.transform);
        }

        private async void CollectReward(Transform flyPoint)
        {
            SetButtonsInteractable(false);
            if (GetCoinAmount() > 0)
            {
                var itemUI = XItemUtility.GetItemUI(rewardGroup, Item.Types.Type.Coin);
                var iconLabel = itemUI.Find("StandardType/RewardIcon");
                
                await XUtility.FlyCoins(iconLabel, new EventBalanceUpdate(GetCoinAmount(), "DaiMissionNormalReward"));
            }
            
            ItemSettleHelper.SettleItems(_reward.Reward.Items, async () =>
            {
                Hide();
                await _mainViewController.ShowCollectNormalMissionPoint(
                    _reward.GetMissionPointAmount(),
                    _reward.GetMissionStarAmount(),
                    _reward.GetGoldHammerAmount()
                );

                DispatchCloseAction();
                SetButtonsInteractable(true);
                CheckActivityItem("DailyMission");
            });
        }
        
        
        protected async  void OnWatchRvFinish(bool success, string reason)
        {
            if (success)
            {
                ulong coin = (ulong)GetCoinAmount();
                var rvRandomCoin =   AdController.Instance.GetRandomRvReward(_adPlaceID, (ulong)coin);
                
                var result = await AdController.Instance.ClaimRvReward(_adPlaceID, rvRandomCoin);

                if (result == null)
                {
                    XDebug.Log("DaiMissionHonorRVReward: ClaimRVRewardFromServerFailed");
                    SetButtonState();
                    SetButtonsInteractable(true);
                    return;
                }
                
                var itemUI = XItemUtility.GetItemUI(rewardGroup, Item.Types.Type.Coin);

                var countLabel = itemUI.Find("StandardType/CountText");
                var iconLabel = itemUI.Find("StandardType/RewardIcon");

                var countText = countLabel.GetComponent<TMP_Text>();
                long v = (long)coin;
                long target = (long)(coin + rvRandomCoin);
                 
                DOTween.To(() => v, (x) =>
                {
                    v = x;
                    countText.text = v.GetCommaFormat();
                }, target, 1.0f).OnComplete(() =>
                {
                    countText.text = (target).GetCommaFormat();
                });

                await viewController.WaitForSeconds(1);
                
                await XUtility.FlyCoins(iconLabel, new EventBalanceUpdate(coin + rvRandomCoin, "DaiMissionNormalReward"));
                 
                ItemSettleHelper.SettleItems(_reward.Reward.Items, async () =>
                {
                    Hide();
                    await _mainViewController.ShowCollectNormalMissionPoint(
                        _reward.GetMissionPointAmount(),
                        _reward.GetMissionStarAmount(),
                        _reward.GetGoldHammerAmount()
                    );
                    DispatchCloseAction();
                    CheckActivityItem("DailyMission");
                });
            }
            else
            {
                SetButtonState();
            }
            
            SetButtonsInteractable(true);
        }

        private async void OnBtnADSClick()
        {
            if (!AdController.Instance.ShouldShowRV(_adPlaceID))
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                SetButtonsInteractable(true);
            }
            else
            {
                SetButtonsInteractable(false);
                ulong coin = (ulong)GetCoinAmount();
                if (coin > 0)
                {
                    AdController.Instance.TryShowRewardedVideo(_adPlaceID, OnWatchRvFinish);
                    
                    // ADSController.TryShowRewardedVideoWithTransform(
                    //     _adPlaceID, coin, coin * 2, _adPlaceID.ToString(), adsButton.transform,
                    //     async (b, s) =>
                    //     {
                    //         if (b)
                    //         {
                    //             Hide();
                    //             await _mainViewController.ShowCollectNormalMissionPoint(
                    //                         _reward.GetMissionPointAmount(),
                    //                         _reward.GetMissionStarAmount(),
                    //                         _reward.GetGoldHammerAmount()
                    //                     );
                    //             DispatchCloseAction();
                    //             CheckActivityItem("DailyMission");
                    //         }
                    //         else
                    //         {
                    //             SetButtonState();
                    //         }
                    //         SetButtonsInteractable(true);
                    //     }
                    // );
                }
                else
                {
                    Hide();
                    await _mainViewController.ShowCollectNormalMissionPoint(
                                _reward.GetMissionPointAmount(),
                                _reward.GetMissionStarAmount(),
                                _reward.GetGoldHammerAmount()
                            );
                    DispatchCloseAction();
                    CheckActivityItem("DailyMission");
                }
            }
        }

        public void ShowNormalReward()
        {
            Show();
            XUtility.PlayAnimation(animator, "Open");
        }

        public Vector3 GetTargetGiftBoxWorldPos()
        {
            var transFlyEnd = transform.Find("FlyEnd");
            var worldPos = transFlyEnd.transform.parent.TransformPoint(transFlyEnd.localPosition);
            return new Vector3(worldPos.x, worldPos.y, worldPos.z);
        }
    }
}