using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{

    public class UIInboxCellView_SeasonPass : UIInboxCellView_Mail
    {
        [ComponentBinder("DetailGroup/SeasonText")]
        public TMP_Text tmpTextSeason;

        [ComponentBinder("DetailGroup/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        [ComponentBinder("CollectGroup/CollectButton")]
        public Button button;

        private ulong _coin;
        private string _season;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
            }
        }

        protected override void OnButtonClick()
        {
            base.OnButtonClick();
            button.interactable = false;
        }

        protected override async void OnClaimFinishFromServer(SClaimMail sClaimMail)
        {
            if (sClaimMail != null)
            {
                await XUtility.FlyCoins(
                    button.transform,
                    new EventBalanceUpdate(_coin, "MailSeasonReward")
                );

                EventBus.Dispatch(new EventInBoxItemUpdated());
            }
            else
            {
                button.interactable = false;
            }
        }

   

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

            var data = PBMailSeasonEndReward.Parser.ParseFrom(pbData.ToByteArray());
            if (data == null || data.Rewards == null || data.Rewards.Count == 0)
            {
                return;
            }

            _season = data.SeasonNum;

            var reward = data.Rewards[0];
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
            base.UpdateView();
            
            button.interactable = true;
          
            if (tmpTextSeason != null)
            {
                tmpTextSeason.text = $"Season {_season}";
            }

            if (tmpTextIntegral != null)
            {
                tmpTextIntegral.text = _coin.GetCommaFormat();
            }
        }
    }
}
