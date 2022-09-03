using System;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{

    [AssetAddress("UIRateUs")]
    public class UIRateUsPopup : Popup
    {

        //Rate Us
        //统一连接  :  https://app.adjust.com/8n1s6xl
        //分平台：
        //Google Play  :  market://details?id=com.casualjoygames.cashcraze
        //App Store ：itms-apps://itunes.apple.com/app/idcom.casualjoygames.cashcraze

        private const string URL = "https://app.adjust.com/8n1s6xl";
 
        [ComponentBinder("Root/MainGroup/RateUsButton")]
        public Button buttonRateUs;
        
        [ComponentBinder("Root/MainGroup/RateUs1StarButton")]
        public Button rateUs1StarButton;

        public UIRateUsPopup(string address) : base(address) { }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            buttonRateUs.onClick.AddListener(OnButtonRateUsClick);
            rateUs1StarButton.onClick.AddListener(OnButtonLowStarClicked);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventRateUsPop);
        }

        private void OnButtonRateUsClick()
        {
            Application.OpenURL(URL);
            
            Client.Storage.SetItem("RateUsChooseOpenUrl", "True");
        }

        private void OnButtonLowStarClicked()
        {
            PopupStack.ShowPopupNoWait<UIContactUsPopup>();
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();
            Client.Storage.SetItem("RateUsChooseClosedClicked", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventRateUsClose);
        }
    }
}
