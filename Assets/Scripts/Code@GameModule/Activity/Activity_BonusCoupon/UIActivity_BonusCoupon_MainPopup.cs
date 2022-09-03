using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIGoldCouponMain")]
    public class UIActivity_BonusCoupon_MainPopup : Popup<UIActivity_GoldenCoupon_MainPopupController>
    {
        [ComponentBinder("Root/BottomGroup/GetButton")]
        public Button buttonGet;

        [ComponentBinder("Root/MainGroup/PercentageText")]
        public Text text;

        public UIActivity_BonusCoupon_MainPopup(string address) : base(address) { }
    }

    public class UIActivity_GoldenCoupon_MainPopupController : ViewController<UIActivity_BonusCoupon_MainPopup>
    {
        private ActivityController _controller;

        private Activity_BonusCoupon _activity;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _controller = Client.Get<ActivityController>();
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            view.buttonGet.interactable = true;
            var popupArgs = extraData as PopupArgs;
            if (popupArgs != null)
            {
                _activity = popupArgs.extraArgs as Activity_BonusCoupon;

                if (_activity != null && _activity.config != null)
                {
                    SetCount(_activity.config.BonusPersentage);
                }
            }
        }

        private void SetCount(long count)
        {
            if (view.text != null) { view.text.SetText(count.GetCommaFormat() + "%"); }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonGet.onClick.AddListener(OnButtonGet);
            view.SubscribeCloseClickAction(OnViewCloseClick);
        }

        private void OnViewCloseClick()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCouponActivityCoinsAdclose);
        }

        private async void OnButtonGet()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCouponActivityCoinsAdenter);

            view.buttonGet.interactable = false;
            if (_activity == null || _activity.config == null)
            {
                view.buttonGet.interactable = true;
                return;
            }
 
            string couponID = null;
           
            var coupon = _activity.GetLinkedCoupon();
            if (coupon != null)
            {
                couponID = coupon.Id;
            }

            if (string.IsNullOrWhiteSpace(couponID))
            {
                view.buttonGet.interactable = true;
                return;
            }
          
            await Client.Get<InboxController>().SendCBindCouponToStore(couponID);
          
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), $"GoldenCouponMainPage")));
            view.Close();

            view.buttonGet.interactable = true;
        }
    }
}
