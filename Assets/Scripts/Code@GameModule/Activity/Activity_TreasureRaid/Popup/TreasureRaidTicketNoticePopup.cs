using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidTicketNotice")]
    public class TreasureRaidTicketNoticePopup : Popup<TreasureRaidTicketNoticePopupController>
    {
        [ComponentBinder("Root/MainGroup/TicketGroup/CountGroup/CountText")]
        public Text ticketCountText;

        [ComponentBinder("Root/MainGroup/TicketGroup/NoticeGroup/Toggle")]
        private Toggle toggle;

        [ComponentBinder("Root/MainGroup/ConfirmButton")]
        private Button goBtn;
        
        [ComponentBinder("Root/MainGroup/ConfirmButton/ClickMask")]
        private Transform goBtnMask;
        
        [ComponentBinder("Root/MainGroup/SpinsGroup")]
        public Transform spinGroup;

        [ComponentBinder("Root/MainGroup/TicketGroup/NoticeGroup")]
        public Transform noticeGroup;
        
        public TreasureRaidTicketNoticePopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1250, 768);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            goBtn.onClick.AddListener(OnGoBtnClicked);
            toggle.isOn = true;
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            noticeGroup.gameObject.SetActive(false);
        }

        private void SetBtnState(bool interactable)
        {
            closeButton.interactable = interactable;
            goBtn.interactable = interactable;
            goBtnMask.gameObject.SetActive(!interactable);
        }
        
        private void OnToggleValueChanged(bool arg0)
        {
            //TODO
            var activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activityTreasureRaid != null)
            {
                activityTreasureRaid.ShowTip = arg0;
            }
        }

        protected override void OnCloseClicked()
        {
            if (viewController.data.TicketCount >= viewController.data.AddedCountMax)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(
                    typeof(TreasureRaidTicketSecondNoticePopup), viewController.data,
                    GetCloseAction())));
                ResetCloseAction();
            }

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidTicketfull);
            base.OnCloseClicked();
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
            if (_activityTreasureRaid.TicketCount >= 30)
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidEnter, ("OperationId", "5"));
            else
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidEnter, ("OperationId", "4"));

            if (_activityTreasureRaid.GetCurrentRunningRoundID() == 0)
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMapPopup), true, "SpinNotice")));
            else
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMainPopup),
                    true, "SpinNotice")));
        }
    }

    public class TreasureRaidTicketNoticePopupController : ViewController<TreasureRaidTicketNoticePopup>
    {
        public MonopolyEnergyInfoWhenSpin data;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSceneSwitchBegin>(OnSceneSwitchBegin);
        }

        private void OnSceneSwitchBegin(EventSceneSwitchBegin obj)
        {
            view.Close();
        }

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
                view.spinGroup.gameObject.SetActive(activityTreasureRaid.TicketCount >= data.AddedCountMax);
            }
            else
            {
                view.ticketCountText.SetText(data.TicketCount.ToString());
                view.spinGroup.gameObject.SetActive(data.TicketCount >= data.AddedCountMax);
            }
        }
    }
}