
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICrazeSmashNotice", "UICrazeSmashNoticeV")]
    public class UICrazeSmashNoticePopup : Popup<UICrazeSmashNoticePopupController>
    {
        [ComponentBinder("Root/MainGroup/TimerText")]
        public Text textTimer;

        [ComponentBinder("Root/MainGroup/CheckButton")]
        public Button buttonCheck;

        public UICrazeSmashNoticePopup(string address) : base(address) { }
    }

    public class UICrazeSmashNoticePopupController : ViewController<UICrazeSmashNoticePopup>
    {
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            var controller = Client.Get<CrazeSmashController>();

            if (controller == null || controller.eggInfo == null) { return; }

            controller.SetNoticeShown();

            var eggInfo = controller.eggInfo;

            var startDateTime = new DateTime(1970, 1, 1).AddSeconds(eggInfo.StartTimestamp);

            view.textTimer.text = startDateTime.ToString("yyyy.MM.dd");
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonCheck.onClick.AddListener(OnButtonCheck);
        }

        private void OnButtonCheck()
        {
            BiManagerGameModule.Instance.SendGameEvent(
                  BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashPop, ("source", "1"));

            PopupStack.ClosePopup<UICrazeSmashNoticePopup>();
            EventBus.Dispatch(
                new EventShowPopup(
                    new PopupArgs(typeof(DailyMissionMainPopup),
                    (object)new[] { "CrazeSmash", "Notice" })
                )
            );
        }
    }
}
