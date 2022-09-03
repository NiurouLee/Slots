using System;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DragonU3DSDK.Network.API.ILProtocol.SGetUserCoupons.Types;

namespace GameModule
{
    public class UIInboxCellView_SeasonQuestRankReward : UIInboxCellView_Mail
    {
        [ComponentBinder("CollectGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        [ComponentBinder("DetailGroup/RankGroup/RankNumberText")]
        public TMP_Text rankText;

        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;    
        
        [ComponentBinder("DetailGroup/RewardGroup")]
        public Transform rewardGroup;
        
        [ComponentBinder("DetailGroup/RewardGroup/InboxRewardCell")]
        public Transform rewardCell;
 
        protected InboxController inboxController;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            inboxController = Client.Get<InboxController>();
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
            }
        }


        protected override void OnClaimMail()
        {
            button.interactable = false;
            inboxController.ClaimMail(mailData, OnClaimFinishFromServer);
        }

        protected override async void OnClaimFinishFromServer(SClaimMail sClaimMail)
        {
            if (sClaimMail != null)
            {
                var content = sClaimMail.Content;

                var contentData = InboxController.ParseMailContent(content);

                if (contentData != null)
                {
                    
                    var coins = XItemUtility.GetItem(contentData.Item, Item.Types.Type.Coin);

                    if (coins != null)
                    {
                        await XUtility.FlyCoins(
                            button.transform,
                            new EventBalanceUpdate(coins, "SeasonQuestRankReward")
                        );
                    }

                    var itemEmerald = XItemUtility.GetItem(contentData.Item, Item.Types.Type.Emerald);

                    if (itemEmerald != null)
                    {
                        EventBus.Dispatch(new EventEmeraldBalanceUpdate((long) itemEmerald.Emerald.Amount,
                            "SeasonQuestRankReward"));
                    }
                    
                    ItemSettleHelper.SettleItems(contentData.Item, source: "SeasonQuestRankReward",
                        finishCallback: () => { EventBus.Dispatch(new EventInBoxItemUpdated()); });
                }
            }
            else
            {
                button.interactable = false;
            }
        }

        protected  override void OnButtonClick()
        {
            base.OnButtonClick();

            button.interactable = false;
        }
        
        
        public override void UpdateTimerText(string remainString)
        {
            if (tmpTextTimer != null)
            {
                tmpTextTimer.text = remainString;
            }
        }

        public override void UpdateView()
        {
            base.UpdateView();
            
            if (mailData != null)
            {
                var pbData = mailData?.ExtraInfo?.Data;
              
                if (pbData == null)
                {
                    return; 
                }
                
                var data = PBMailSeasonLeaderboardReward.Parser.ParseFrom(pbData.ToByteArray());

                if (data == null || data.Rewards.Count <= 0)
                {
                    return;
                }

                if (rankText != null)
                {
                    rankText.SetText(data.Rank.ToString());
                }

                for (var i = rewardGroup.childCount - 1; i >= 0; i--)
                {
                    var child = rewardGroup.GetChild(i);
                    if (child.name != "InboxRewardCell")
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
                
                XItemUtility.InitItemsUI(rewardGroup, data.Rewards[0].Items,rewardGroup.Find("InboxRewardCell"));
                
                UpdateTimeLeft();
            }
        }
    }
}
