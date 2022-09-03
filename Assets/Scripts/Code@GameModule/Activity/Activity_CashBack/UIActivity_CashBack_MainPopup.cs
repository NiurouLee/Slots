using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICashBackTimeMain")]
    public class UIActivity_CashBack_MainPopup : Popup<UIActivity_CashBack_MainPopupController>
    {
        [ComponentBinder("Root/MainGroup/GetButton")]
        public Button buttonGet;

        [ComponentBinder("Root/TopGroup/InformationButton")]
        public Button buttonInfomation;

        [ComponentBinder("Root/MainGroup/IntegralText")]
        public Text textIntegral;


        public UIActivity_CashBack_MainPopup(string address) : base(address)
        {
        }
    }

    public class UIActivity_CashBack_MainPopupController : ViewController<UIActivity_CashBack_MainPopup>
    {
        private ActivityController _controller;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            _controller = Client.Get<ActivityController>();
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            var popupArgs = extraData as PopupArgs;
            if (popupArgs != null)
            {
                var activity = popupArgs.extraArgs as Activity_CashBack;

                if (activity != null && activity.config != null)
                {
                    SetCount(activity.config.ReturnLimited);
                }
            }
        }

        private void SetCount(long count)
        {
            if (view.textIntegral != null) { view.textIntegral.SetText(count.GetCommaFormat()); }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonGet.onClick.AddListener(OnButtonGetClick);
            view.buttonInfomation.onClick.AddListener(OnButtonInfomationClick);
        }

        private async void OnButtonInfomationClick()
        {
            await PopupStack.ShowPopup<UIActivity_CashBack_InformationPopup>();
        }

        private void OnButtonGetClick()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), $"CashBackMainPage")));

            // EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(PopupId.IAP_STORE)));
            // EventBus.Dispatch(new EventEnqueueFencePopup(null));

            view.Close();
        }
    }
}
