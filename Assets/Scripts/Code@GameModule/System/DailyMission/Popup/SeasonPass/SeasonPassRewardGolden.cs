//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 09:09
//  Ver : 1.0.0
//  Description : SeasonPassRewardGolden.cs
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
    [AssetAddress("UISeasonPassRewardGoldenPassH","UISeasonPassRewardGoldenPassV")]
    public class SeasonPassRewardGolden: SeasonPassRewardBase
    {
        protected RepeatedField<Reward> _rewards;

        public SeasonPassRewardGolden(string assetAddress)
            : base(assetAddress)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            _rewards = new RepeatedField<Reward>();
        }
        
        
        public override void InitRewards(RepeatedField<Reward> rewards=null,bool isGolden=false, MissionPassReward missionPassReward=null)
        {
            base.InitRewards(rewards, isGolden, missionPassReward);
            if (rewards == null)
            {
                isCollectAll = true;
                _rewards.AddRange(Client.Get<SeasonPassController>().GetFreeRewards());
                _rewards.AddRange(Client.Get<SeasonPassController>().GetGoldenRewards());
            }
            else
            {
                _rewards.AddRange(rewards);
            }

            InitRewardContent(_rewards);
            var coinItems = XItemUtility.GetItems(_rewards, Item.Types.Type.Coin);
            UpdateTotalWin(coinItems);

        }
        
        protected override int InitRewardContent(RepeatedField<Reward> rewards)
        {
            var template = _transScrollViewContent.transform.Find("DailyMissionRewardCell");
            SetTransformActive(template,false);
            
            var count = XItemUtility.InitItemsUI(_transScrollViewContent, rewards, template, GetItemDescribe,"BGYellowType");
            rewardGroup.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            return count;
        }

        private void UpdateTotalWin(List<Item> coinItems)
        {
            for (int i = 0; i < coinItems.Count; i++)
            {
                totalWin += coinItems[i].Coin.Amount;
            }     
        }

    }
}