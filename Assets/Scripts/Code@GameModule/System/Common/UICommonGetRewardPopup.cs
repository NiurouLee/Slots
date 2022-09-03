// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/06/15:45
// Ver : 1.0.0
// Description : UICommonGetRewardPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using Google.ilruntime.Protobuf.Collections;
namespace GameModule
{
    [AssetAddress("UICommonGetReward", "UICommonGetRewardV")]
    public class UICommonGetRewardPopup : Popup<UICommonGetRewardPopupViewController>
    {
        [ComponentBinder("RewardGroup")] public Transform rewardGroup;

        [ComponentBinder("ConfirmButton")] public Button claimButton;

        public UICommonGetRewardPopup(string address)
            : base(address)
        {
            
        }
    }

    public class UICommonGetRewardPopupViewController : ViewController<UICommonGetRewardPopup>
    {
        protected string rewardSource;

        protected RepeatedField<Item> rewardsToGet;

        protected Action<Action<RepeatedField<Item>>> claimAction;
        protected Action finishCallback;
        
        public void SetUpReward(RepeatedField<Reward> inRewards, string inRewardSource,  Action inFinishCallback = null, Action<Action<RepeatedField<Item>>> inClaimAction = null)
        {
            rewardSource = inRewardSource;
            rewardsToGet = XItemUtility.GetItems(inRewards);
            claimAction = inClaimAction;
            finishCallback = inFinishCallback;
            
            SetUpClaimUI();
        }
        
        public void SetUpReward(RepeatedField<Item> items, string inRewardSource,  Action inFinishCallback = null, Action<Action<RepeatedField<Item>>> inClaimAction = null)
        {
            rewardSource = inRewardSource;
            rewardsToGet = items;
            claimAction = inClaimAction;
            finishCallback = inFinishCallback;
            
            SetUpClaimUI();
        }

        protected void SetUpClaimUI()
        {
            view.claimButton.interactable = true;
            XItemUtility.InitItemsUI(view.rewardGroup, rewardsToGet,view.rewardGroup.Find("CommonCell"));
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.claimButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        protected void OnConfirmButtonClicked()
        {
            view.claimButton.interactable = false;
            
            if (claimAction == null)
            {
                DoClaimEffect();
            }
            else
            {
                claimAction.Invoke((inRewardsToGet) =>
                {
                    if (inRewardsToGet != null)
                    {
                        rewardsToGet = inRewardsToGet;
                        DoClaimEffect();
                    }
                    else
                    {
                        CommonNoticePopup.ShowCommonNoticePopUp("UI_NOTICE_ERROR_OCCURRED",
                            () => { view.Close(); });
                    }
                });
            }
        }

        protected async void DoClaimEffect()
        {
            
            var coinItem = XItemUtility.GetItem(rewardsToGet, Item.Types.Type.Coin);

            if (coinItem != null)
            {
                await XUtility.FlyCoins(view.claimButton.transform, new EventBalanceUpdate(coinItem.Coin.Amount, rewardSource.ToString()));
            }
            
            var emeraldItem = XItemUtility.GetItem(rewardsToGet, Item.Types.Type.Emerald);

            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, rewardSource.ToString()));
            }
             
            view.Hide();
            ItemSettleHelper.SettleItems(rewardsToGet, OnClaimFinished);
        }

        protected void OnClaimFinished()
        {
            view.Close();
            finishCallback?.Invoke();
        }
    }
}