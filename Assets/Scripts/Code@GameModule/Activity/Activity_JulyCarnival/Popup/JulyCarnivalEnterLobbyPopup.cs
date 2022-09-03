using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIIndependenceDayNotice")]
    public class JulyCarnivalEnterLobbyPopup : Popup<JulyCarnivalEnterLobbyPopupController>
    {
        [ComponentBinder("Root/BottomGroup/PriceButton")]
        public Button goButton;

        public JulyCarnivalEnterLobbyPopup(string address)
            : base(address)
        {
            // contentDesignSize = new Vector2(1465, 768);
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            goButton.onClick.AddListener(OnGoBtnClicked);
        }

        private void OnGoBtnClicked()
        {
            goButton.interactable = false;
            // 展示独立日主页面
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalMainPopup), false, "1")));
            Close();
        }
    }

    public class JulyCarnivalEnterLobbyPopupController : ViewController<JulyCarnivalEnterLobbyPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityExpire>(OnEventActivityExpire);
        }

        private void OnEventActivityExpire(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.JulyCarnival)
                return;
            view.goButton.interactable = false;
            view.Close();
        }
    }
}