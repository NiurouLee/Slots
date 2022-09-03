using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIAmazingTheHatWidget")]
    public class AmazingHatWidget: SystemWidgetView<AmazingHatWidgetViewController>
    {
        public AmazingHatWidget(string address) : base(address)
        {
            
        }

        public override void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            viewController.OnWidgetClicked(widgetContainerViewController);
        }

    }

    public class AmazingHatWidgetViewController : ViewController<AmazingHatWidget>
    {
        private bool _lastState;

        private AmazingHatController _controller;
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        private void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.HideWidget();
        }
        
        private void OnEventAmazingHatStateUpdate(EventAmazingHatStateUpdate obj)
        {
            view.HideWidget();
        }

        public override void OnViewDidLoad()
        {
            _controller = Client.Get<AmazingHatController>();
            base.OnViewDidLoad();
            view.transform.gameObject.SetActive(true);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(1);
        }

        public void InitViewUI()
        {
            var isOpen = _controller.IsOpen();
            _lastState = isOpen;
            if (isOpen)
                view.ShowWidget();
            else
                view.HideWidget();
        }
        public override void Update()
        {
            bool isOpen = _controller.IsOpen();
            // Debug.Log("isOpen : " + isOpen);
            if (isOpen != _lastState)
            {
                _lastState = isOpen;
                if (isOpen)
                    view.ShowWidget();
                else
                    view.HideWidget();
            }
        }
        
        public void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            if (!_controller.IsOpen())
                return;
            
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(AmazingHatMainPopup), (object)new []{"AmazingHat","Machine"})));
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatEnter, ("Operation:", "Machine"),("OperationId","2"));
        }
    }
}