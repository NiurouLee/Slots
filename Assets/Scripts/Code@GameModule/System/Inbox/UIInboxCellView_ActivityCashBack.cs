using System;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    public class UIInboxCellView_ActivityCashBack : UIInboxCellView_Mail
    {
        [ComponentBinder("CollectGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        [ComponentBinder("DetailGroup/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;

        private ulong _coin;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
            }
        }


        public override void ParseData()
        {
            base.ParseData();

            _coin = 0;
            if (mailData == null || mailData.ItemList == null)
            {
                return;
            }

            var data = MailRewardsPB.Parser.ParseFrom(mailData.ItemList.ToByteArray());

            var coinReward = XItemUtility.GetCoinItem(data.Rewards[0].Items);

            if (coinReward != null)
                _coin = coinReward.Coin.Amount;
        }

        public override void UpdateView()
        {
            base.ParseData();
            if (button != null)
            {
                button.interactable = true;
            }

            tmpTextIntegral.text = _coin.GetCommaFormat();
        }

        public override void UpdateTimerText(string remainString)
        {
            if (tmpTextTimer != null)
            {
                tmpTextTimer.text = remainString;
            }
        }

        protected override async void OnClaimFinishFromServer(SClaimMail sClaimMail)
        {
            if (sClaimMail != null)
            {
                await XUtility.FlyCoins(
                    button.transform,
                    new EventBalanceUpdate(_coin, "Mail cash back reward")
                );

                EventBus.Dispatch(new EventInBoxItemUpdated());
            }
            else
            {
                button.interactable = true;
            }
        }

        protected override void OnButtonClick()
        {
            if (button != null)
            {
                button.interactable = false;
            }

            base.OnButtonClick();
            button.interactable = false;
        }
    }
}