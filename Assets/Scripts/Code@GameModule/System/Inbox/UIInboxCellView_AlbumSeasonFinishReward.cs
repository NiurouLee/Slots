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
    public class UIInboxCellView_AlbumSeasonFinishReward : UIInboxCellView_Mail
    {
        [ComponentBinder("CollectGroup/TimerText")]
        public TMP_Text tmpTextTimer;
 
        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;    
        
        [ComponentBinder("DetailGroup/IntegralGroup/IntegralText")]
        public TMP_Text coinText;
 
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

        protected override async void OnClaimFinishFromServer(SClaimMail sClaimMail)
        {
            if (sClaimMail != null)
            {
                var contentData = InboxController.ParseMailContent(sClaimMail.Content);

                if (contentData != null)
                {
                    var coins = XItemUtility.GetItem(contentData.Item, Item.Types.Type.Coin);

                    if (coins != null)
                    {
                        await XUtility.FlyCoins(
                            button.transform,
                            new EventBalanceUpdate(coins, "AlbumSeasonFinishReward")
                        );
                    }

                    ItemSettleHelper.SettleItems(contentData.Item, source: "AlbumSeasonFinishReward", finishCallback:
                        () => { EventBus.Dispatch(new EventInBoxItemUpdated()); });
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
                var pbData = mailData?.ItemList;
              
                if (pbData == null)
                {
                    return; 
                }
                
                var data = MailRewardsPB.Parser.ParseFrom(pbData.ToByteArray());
                
                if (data == null || data.Rewards.Count <= 0)
                {
                    return;
                }

                var coinItem  = XItemUtility.GetItem(data.Rewards[0].Items, Item.Types.Type.Coin);
                
                coinText.text = coinItem.Coin.Amount.GetCommaFormat();
                
                //TODO Update Reward
       
                UpdateTimeLeft();
            }
        }
    }
}
