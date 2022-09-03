//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 22:15
//  Ver : 1.0.0
//  Description : UIDailyMissionRewardHonor.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIDailyMissionRewardCellHonerH", "UIDailyMissionRewardCellHonerV")]
    public class DailyMissionRewardHonor : DailyMissionRewardBase
    {
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdRewardedVideoPlacementMonitor.Bind(this.adsButton.gameObject, eAdReward.HonorMissionRV.ToString());

            adsButton.onClick.AddListener(OnBtnADSClick);
            confirmButton.onClick.AddListener(OnBtnConfirmClick);
            collectButton.onClick.AddListener(OnBtnCollectClick);
        }
        public DailyMissionRewardHonor(string address) : base(address)
        {
            _adPlaceID = eAdReward.HonorMissionRV;
        }

        public override string GetOpenAudioName()
        {
            return "General_GiftBoxOpen";
        }

        private  void OnBtnCollectClick()
        {
            CollectReward(collectButton.transform);
        }
        
        private  void OnBtnConfirmClick()
        {
            CollectReward(confirmButton.transform);
        }
        
        private async void CollectReward(Transform flyPoint)
        {
            SetButtonsInteractable(false);
            if (_reward.GetCoinAmount() > 0)
            {
                var itemUI = XItemUtility.GetItemUI(rewardGroup, Item.Types.Type.Coin);
                var iconLabel = itemUI.Find("StandardType/RewardIcon");
                await XUtility.FlyCoins(iconLabel, new EventBalanceUpdate(_reward.GetCoinAmount(), "DaiMissionHonorReward"));
            }
            
            ItemSettleHelper.SettleItems(_reward.Reward.Items, async () =>
            {
                Close();
                await _mainViewController.ShowCollectHonorMissionPoint(_reward.GetMissionPointAmount(), _reward.GetMissionStarAmount());
                CheckActivityItem("HonorMission");
            });
         
        }

        protected async  void OnWatchRvFinish(bool success, string reason)
        {
            if (success)
            {
                ulong coin = (ulong)GetCoinAmount();
                var rvRandomCoin = AdController.Instance.GetRandomRvReward(_adPlaceID, coin);
                
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
                long v =  (long) coin;

                long target = (long) (rvRandomCoin + coin);

                DOTween.To(() => v, (x) =>
                {
                    v = x;
                    countText.text = v.GetCommaFormat();
                }, target, 1.0f).OnComplete(() => { countText.text = target.GetCommaFormat(); });

                await viewController.WaitForSeconds(1);
                
                await XUtility.FlyCoins(iconLabel, new EventBalanceUpdate(coin + rvRandomCoin, "DaiMissionHonorReward"));
 
                ItemSettleHelper.SettleItems(_reward.Reward.Items, async () =>
                {
                    Close();
                    await _mainViewController.ShowCollectHonorMissionPoint(_reward.GetMissionPointAmount(),
                        _reward.GetMissionStarAmount());
                    CheckActivityItem("HonorMission");
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
            if (!AdController.Instance.ShouldShowRV(_adPlaceID,false))
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
                    //     _adPlaceID, rvRandomCoin, rvRandomCoin + coin, _adPlaceID.ToString(), adsButton.transform,
                    //     async (b, s) =>
                    //     {
                    //        
                    //     }
                    // );
                }
                else
                {
                    Close();
                    await _mainViewController.ShowCollectHonorMissionPoint(_reward.GetMissionPointAmount(), _reward.GetMissionStarAmount());
                    if (adsButton != null) { adsButton.interactable = true; }
                    CheckActivityItem("HonorMission");
                }
            }
        }
    }
}