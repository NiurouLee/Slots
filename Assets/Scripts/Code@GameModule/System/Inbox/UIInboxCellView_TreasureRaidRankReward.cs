using System;
using DragonPlus;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DragonU3DSDK.Network.API.ILProtocol.SGetUserCoupons.Types;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class UIInboxCellView_TreasureRaidRankReward : UIInboxCellView_Mail
    {
        [ComponentBinder("CollectGroup/TimerText")]
        public Text tmpTextTimer;

        [ComponentBinder("BGGroup/mail/CurrencyBg2/CurrencyText")]
        public Text coinText;

        [ComponentBinder("BGGroup/mail/CurrencyBg2/CurrencyText/GiftIcon")]
        public Transform giftIcon;

        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;    

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
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidCollectrankreward);
                    var rewardPopup = await PopupStack.ShowPopup<UICommonGetRewardPopup>();
                    rewardPopup.viewController.SetUpReward(contentData.Item,"TreasureRaidRankReward");
                    EventBus.Dispatch(new EventInBoxItemUpdated());
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
            
            if (mailData == null || mailData.ItemList == null)
            {
                return;
            }

            var data = MailRewardsPB.Parser.ParseFrom(mailData.ItemList.ToByteArray());

            var coinReward = XItemUtility.GetCoinItem(data.Rewards[0].Items);

            if (coinReward != null)
            {
                bool hasMore = data.Rewards[0].Items.Count > 1;
                var count = (long)coinReward.Coin.Amount;
                var str = hasMore
                    ? $"{count.GetCommaOrSimplify(7)} +"
                    : $"{count.GetCommaOrSimplify(8)}";
                coinText.SetText(str);
                giftIcon.gameObject.SetActive(hasMore);
            }
            UpdateTimeLeft();
        }
    }
}
