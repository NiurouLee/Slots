//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 09:06
//  Ver : 1.0.0
//  Description : SeasonPassRewardFree.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace GameModule
{
    [AssetAddress("UISeasonPassRewardFreePassH","UISeasonPassRewardFreePassV")]
    public class SeasonPassRewardFree: SeasonPassRewardBase
    {
        [ComponentBinder("Root/BottomGroup/UnlockButton")]
        public Button btnUnlock;
        
        [ComponentBinder("Root/RewardGroup")]
        public Image _imageFreeScrollRect;
        [ComponentBinder("Root/RewardGroup")]
        public ScrollRect _FreeScrollRect;
        [ComponentBinder("Root/RewardGroup/PreviousButton")]
        public Button _btnFreePrevious;
        [ComponentBinder("Root/RewardGroup/NextButton")]
        public Button _btnFreeNext;
        [ComponentBinder("Root/LockGroup/PreviousButton")]
        public Button _btnLockPrevious;
        [ComponentBinder("Root/LockGroup/NextButton")]
        public Button _btnLockNext;
        [ComponentBinder("Root/LockGroup")]
        public ScrollRect _LockScrollRect;
        

        [ComponentBinder("Root/LockGroup/Viewport/Content")]
        public Transform _transLockScrollViewContent;


        public SeasonPassRewardFree(string assetAddress)
            : base(assetAddress)
        {
            
        }

        [ComponentBinder("Root/RewardGroup/PreviousButton")]
        private void OnBtnPreviousClick()
        {
            _FreeScrollRect.horizontalNormalizedPosition = 0;
        }
        [ComponentBinder("Root/RewardGroup/NextButton")]
        private void OnBtnNextClick()
        {
            _FreeScrollRect.horizontalNormalizedPosition = 1;
        }

        [ComponentBinder("Root/LockGroup/PreviousButton")]
        private void OnBtnPrevious0Click()
        {
            _LockScrollRect.horizontalNormalizedPosition = 0;
        }

        [ComponentBinder("Root/LockGroup/NextButton")]
        private void OnBtnNext0Click()
        {
            _LockScrollRect.horizontalNormalizedPosition = 1;
        }
        
        
        public override void InitRewards(RepeatedField<Reward> rewards=null,bool isGolden=false, MissionPassReward missionPassReward=null)
        {
            base.InitRewards(rewards, isGolden, missionPassReward);
            if (rewards == null)
            {
                isCollectAll = true;
                rewards = Client.Get<SeasonPassController>().GetFreeRewards();
            }
           // int nTotalCount = GetItemTypes(rewards).Count;
            InitRewardContent(rewards);
            
            var coinItems = XItemUtility.GetItems(rewards, Item.Types.Type.Coin);
            for (int i = 0; i < coinItems.Count; i++)
            {
                totalWin += coinItems[i].Coin.Amount;
            }
            
            rewards = Client.Get<SeasonPassController>().GetGoldenRewards();
         
            var uiCount = InitLockRewardContent(rewards);
          
            if (uiCount <= 5)
            {
                _btnFreeNext.gameObject.SetActive(false);
                _btnFreePrevious.gameObject.SetActive(false);
                _imageFreeScrollRect.color = new Color(1, 1, 1,0);
                _FreeScrollRect.horizontal = false;
                
                _btnLockNext.gameObject.SetActive(false);
                _btnLockPrevious.gameObject.SetActive(false);
                _LockScrollRect.horizontal = false;
            }

            if (Client.Get<SeasonPassController>().Paid && !isCollectAll)
            {
                btnUnlock.gameObject.SetActive(false);
                _LockScrollRect.gameObject.SetActive(false);
            }
        }

        public int InitLockRewardContent(RepeatedField<Reward> rewards)
        {
            var template = _transLockScrollViewContent.transform.Find("DailyMissionRewardCell");
            SetTransformActive(template, false);
            var count = XItemUtility.InitItemsUI(_transLockScrollViewContent, rewards, template, GetItemDescribe,
                "BGYellowType");
            
            rewardGroup.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            return count;
        }

        [ComponentBinder("Root/BottomGroup/UnlockButton")]
        private async void OnBtnUnLockClick()
        {
            PopupStack.ShowPopup<SeasonPassPurchaseGolden>();
        }
        
        public override async void OnSeasonPassUpdate(EventSeasonPassUpdate evt)
        {
            if (evt.CheckPaid && Client.Get<SeasonPassController>().Paid)
            {
                Close();
                var popup = await PopupStack.ShowPopup<SeasonPassRewardGolden>();
                popup.InitRewards(null, true);
            }
        }
        
        public RepeatedField<Item.Types.Type> GetItemTypes(RepeatedField<Reward> listRewards)
        {
            var listTypes = new RepeatedField<Item.Types.Type>();
            for (int i = 0; i < listRewards.Count; i++)
            {
                var reward = listRewards[i];
                for (int j = 0; j < reward.Items.Count; j++)
                {
                    if (listTypes.Contains(reward.Items[j].Type))
                    {
                        continue;
                    }
                    listTypes.Add(reward.Items[j].Type);
                }
            }
            return listTypes;
        }
    }
}