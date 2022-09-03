using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidTicketSecondNotice")]
    public class TreasureRaidTicketSecondNoticePopup : Popup<TreasureRaidTicketSecondNoticePopupController>
    {
        [ComponentBinder("Root/BGGroup/BG/MainGroup/TicketGroup/CountGroup/CountText")]
        public Text ticketCountText;

        [ComponentBinder("Root/BGGroup/BG/MainGroup/ConfirmButton")]
        private Button goBtn;
        
        [ComponentBinder("Root/BGGroup/BG/MainGroup/ConfirmButton/ClickMask")]
        private Transform goBtnMask;

        public TreasureRaidTicketSecondNoticePopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1150, 768);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            goBtn.onClick.AddListener(OnGoBtnClicked);
        }

        private void SetBtnState(bool interactable)
        {
            closeButton.interactable = interactable;
            goBtn.interactable = interactable;
            goBtnMask.gameObject.SetActive(!interactable);
        }

        private void OnGoBtnClicked()
        {
            SetBtnState(false);
            ResetCloseAction();
            Close();
            var _activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;

            if (_activityTreasureRaid == null)
                return;

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidEnter, ("OperationId", "6"));

            if (_activityTreasureRaid.GetCurrentRunningRoundID() == 0)
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMapPopup), true, "SpinNotice")));
            else
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMainPopup),
                    true, "SpinNotice")));
        }
    }

    public class TreasureRaidTicketSecondNoticePopupController : ViewController<TreasureRaidTicketSecondNoticePopup>
    {
        private MonopolyEnergyInfoWhenSpin data;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            var args = (PopupArgs) inExtraData;
            data = args.extraArgs as MonopolyEnergyInfoWhenSpin;
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            var activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;

            if (activityTreasureRaid != null)
            {
                view.ticketCountText.SetText(activityTreasureRaid.TicketCount.ToString());
            }
            else
            {
                view.ticketCountText.SetText(data.TicketCount.ToString());
            }
        }
    }
}