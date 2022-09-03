using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine.UI;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class UIInboxCellView_FreeLottoBonus : UIInboxCellView_Mail
    {
        [ComponentBinder("CollectGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        [ComponentBinder("CollectGroup/PlayButton")]
        public Button button;

        [ComponentBinder("BGGroup/WinUpto/CurrencyBg1/CurrencyText")]
        public Text coinText;

        protected InboxController inboxController;

        private LevelRushGameInfo freeLottoGameInfo;

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

        protected override void OnButtonClick()
        {
            button.interactable = false;
            base.OnButtonClick();
        }

        protected override void OnClaimMail()
        {
            // inboxController.ClaimMail(mailData, OnClaimFinishFromServer);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushMailplay);
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LottoBonusFreePopup), freeLottoGameInfo, () =>
            {
                inboxController.RemoveMail(itemData);
                EventBus.Dispatch(new EventInBoxItemUpdated());
            })));
        }

        protected override void OnClaimFinishFromServer(SClaimMail sClaimMail)
        {
            if (sClaimMail != null)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushMailplay);
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LottoBonusFreePopup), freeLottoGameInfo)));
                button.interactable = true;
            }
            else
            {
                button.interactable = true;
            }
        }

        public override void UpdateView()
        {
            base.UpdateView();
            button.interactable = true;
            if (mailData != null)
            {
                var pbData = mailData?.ExtraInfo?.Data;
              
                if (pbData == null)
                {
                    return; 
                }
                
                var data = LevelRushGameInfo.Parser.ParseFrom(pbData.ToByteArray());
                freeLottoGameInfo = data;
                var coinItem = XItemUtility.GetItem(freeLottoGameInfo.GameCoinsMax.Items, Item.Types.Type.Coin);
                var coin = (long)coinItem.Coin.Amount;
                coinText.SetText(coin.GetCommaOrSimplify(7));
            }
        }
        
        public override void UpdateTimerText(string remainString)
        {
            if (tmpTextTimer != null)
            {
                tmpTextTimer.text = remainString;
            }
        }
    }
}