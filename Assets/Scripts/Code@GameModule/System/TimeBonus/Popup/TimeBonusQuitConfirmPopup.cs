// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/10/11:19
// Ver : 1.0.0
// Description : TimeBonusQuitConfirmPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    
    [AssetAddress("UITimerBonusBack")]
    public class TimeBonusQuitConfirmPopup:Popup<TimeBonusQuitConfirmPopupViewController>
    {
        [ComponentBinder("CoinIntegralText")] 
        public TextMeshProUGUI coinIntegralText;

        [ComponentBinder("PriceText")]
        public Text priceText;

        [ComponentBinder("ConfirmButton")] 
        public Button confirmButton;
 
        public TimeBonusQuitConfirmPopup(string address)
            : base(address)
        {
            
        }
    }

    public class TimeBonusQuitConfirmPopupViewController : ViewController<TimeBonusQuitConfirmPopup>
    {
        private TimeBonusWheelBonusPopup _wheelPopup;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
          
            view.confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            view.closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventCloseTimeBonusQuitPopup>(OnQuitTimeBonusPopup);
        }

        public void OnQuitTimeBonusPopup(EventCloseTimeBonusQuitPopup evt)
        {
            view.Close();
        }
        
        public void SetUpQuitConfirmUI(ShopItemConfig shopItemConfig, ulong winUpCoins, TimeBonusWheelBonusPopup wheelPopup)
        {
            _wheelPopup = wheelPopup;
            view.priceText.text = "$" + shopItemConfig.Price;
            view.coinIntegralText.text = winUpCoins.GetCommaFormat();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusGoldenWheelPop2, 
                ("paymentId",shopItemConfig.PaymentId.ToString()),("price", shopItemConfig.Price.ToString()));
        }

        public void OnCloseButtonClicked()
        {
            view.Close();
            _wheelPopup.Close();
          
        }
        
        public void OnConfirmButtonClicked()
        {
            SoundController.PlayButtonClick();
            _wheelPopup.viewController.OnGoldenWheelSpinButtonClicked();
        }
     }
}