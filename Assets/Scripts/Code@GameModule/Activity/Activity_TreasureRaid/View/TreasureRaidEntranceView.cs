using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidEntrance")]
    public class TreasureRaidEntranceView : View<TreasureRaidEntranceViewController>
    {
        [ComponentBinder("Timer/TimerText")]
        public TextMeshProUGUI _timerText;
        
        [ComponentBinder("Content/ReminderGroup")]
        public Transform reminderGroup;

        // [ComponentBinder("Content/LockState")]
        // public Transform lockState;

        [ComponentBinder("Content/ReminderGroup/NoticeText")]
        public TextMeshProUGUI ticketCountText;

        [ComponentBinder("LobbyTextBubbleM")] public Transform lockTips;

        public CommonTextBubbleView bubbleView;

        public TreasureRaidEntranceView(string address)
            : base(address)
        {
         
        }

        public void RefreshUI(bool isOpen)
        {
            if (!isOpen)
            {
                Hide();
            }
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            bubbleView = AddChild<CommonTextBubbleView>(lockTips);
        }
        
        public override void Hide()
        {
            var lobbyScene = GetParentView() as LobbyScene;
            lobbyScene?.RemoveChild(this);
            var passView = lobbyScene?.GetChildView<SeasonPassLobbyEntranceView>();
            passView?.Show();
        }
    }

    public class TreasureRaidEntranceViewController : ViewController<TreasureRaidEntranceView>
    {
        private Button _playBtn;

        private Activity_TreasureRaid _activityTreasureRaid;

        public override void OnViewDidLoad()
        {
            var activityController = Client.Get<ActivityController>();
            _activityTreasureRaid = activityController.GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;

            base.OnViewDidLoad();
            _playBtn = view.transform.GetComponent<Button>();
            _playBtn.onClick.AddListener(OnEntranceBtnClick);
            var content = view.transform.Find("Content");
            var pointerEventCustomHandler = view.transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown((eventData) => { content.localScale = Vector3.one * 0.95f; });
            pointerEventCustomHandler.BindingPointerUp((eventData) => { content.localScale = Vector3.one; });
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
            SubscribeEvent<EventActivityServerDataUpdated>(UpdateTicketCount);
        }

        private void UpdateTicketCount(EventActivityServerDataUpdated evt)
        {
            if (evt.activityType != ActivityType.TreasureRaid)
                return;

            if (_activityTreasureRaid != null)
            {
                view.reminderGroup.gameObject.SetActive(_activityTreasureRaid.TicketCount > 0);
                view.ticketCountText.SetText( _activityTreasureRaid.TicketCount > 99 ? "99+" :  _activityTreasureRaid.TicketCount.ToString());
            }
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            DisableUpdate();
            // 待处理UI
            view.RefreshUI(false);
        }

        private void OnEntranceBtnClick()
        {
            if (!_activityTreasureRaid.IsUnlockState())
            {
                view.bubbleView.SetText($"Unlock At LEVEL {_activityTreasureRaid.GetUnlockLevel()}");
                view.bubbleView.ShowBubble(3);
                return;
            }

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidEnter, ("OperationId", "7"));
            // 这里要判断当前是否已经开始大富翁游戏，如果开始了直接进入关卡，如果没开始进地图。
            if (_activityTreasureRaid.GetCurrentRunningRoundID() == 0)
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMapPopup), true, "Lobby")));
            else
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMainPopup),
                    true, "Lobby")));
            // BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatEnter, ("Operation:", "AlbumIcon"),("OperationId","1"));
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(1);
            view.RefreshUI(true);

            // view.lockState.gameObject.SetActive(!_activityTreasureRaid.IsUnlockState());

            float countDown = _activityTreasureRaid.GetCountDown();
            view._timerText.text = XUtility.GetTimeText(countDown);
            view.reminderGroup.gameObject.SetActive(_activityTreasureRaid.TicketCount > 0);
            view.ticketCountText.SetText( _activityTreasureRaid.TicketCount > 99 ? "99+" :  _activityTreasureRaid.TicketCount.ToString());
        }

        public override void Update()
        {
            float countDown = _activityTreasureRaid.GetCountDown();
            if (countDown >= 0)
            {
                view._timerText.text = XUtility.GetTimeText(countDown);
            }
            else
            {
                DisableUpdate();
                view.RefreshUI(false);
            }
        }
    }
}