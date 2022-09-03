using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    public class UIInboxCellView_VIP : UIInboxCellView_Mail
    {
        [ComponentBinder("DetailGroup/DescriptionText")]
        public TMP_Text tmpTextDescription;

        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;

        [ComponentBinder("DetailGroup/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        [ComponentBinder("CollectGroup/TimerText")]
        public TMP_Text tmpTextTime;

        private ulong _coin;
 
        public override void ParseData()
        {
            
            base.ParseData();
            
            _coin = 0;

            if (mailData == null)
            {
                return;
            }

            var pbData = mailData?.ExtraInfo?.Data;
          
            if (pbData == null)
            {
                return;
            }

            var pbMailVipData = PBMailVip.Parser.ParseFrom(pbData.ToByteArray());
            if (pbMailVipData == null || pbMailVipData.Rewards == null || pbMailVipData.Rewards.Count == 0)
            {
                return;
            }

            var reward = pbMailVipData.Rewards[0];

            if (reward.Items != null && reward.Items.Count > 0)
            {
                var item = reward.Items[0];
                if (item.Type == Item.Types.Type.Coin && item.Coin != null)
                {
                    _coin = item.Coin.Amount;
                }
            }
        }

        public override void UpdateView()
        {
            if (tmpTextDescription != null)
            {
                tmpTextDescription.text = mailData?.MailInfo?.Message;
            }

            if (tmpTextIntegral != null)
            {
                tmpTextIntegral.text = _coin.GetCommaFormat();
            }

            if (button != null)
            {
                button.interactable = true;
            }

            UpdateTimeLeft();
        }

        public override void UpdateTimerText(string remainString)
        {
            if (tmpTextTime != null) { tmpTextTime.text = remainString; }
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
                await XUtility.FlyCoins(
                    button.transform,
                    new EventBalanceUpdate(_coin, "MailVIPReward")
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
            if(button)
                button.interactable = false;
            base.OnButtonClick();
        }
    }
}
