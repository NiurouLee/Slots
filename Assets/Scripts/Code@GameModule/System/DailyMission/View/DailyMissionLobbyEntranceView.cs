//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-26 14:40
//  Ver : 1.0.0
//  Description : DailyMissionLobbyView.cs
//  ChangeLog :
//  **********************************************

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyMissionLobbyEntranceView : View<DailyMissionLobbyEntranceViewController>
    {
        [ComponentBinder("LockState")] public Transform transLock;
        [ComponentBinder("ReminderGroup")] private Transform transReminder;

        [ComponentBinder("Content/ReminderGroup/NoticeText")]
        private TextMeshProUGUI txtReminderNotice;

        [ComponentBinder("Timer")] public Transform transTimer;
        [ComponentBinder("Timer/TimerText")] public TextMeshProUGUI txtTimer;
 
        [ComponentBinder("LobbyTextBubbleM")] public Transform lockTips;
        
        public CommonTextBubbleView dailyMissionBubble;

        public DailyMissionLobbyEntranceView()
            : base(null)
        {
        }


        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
           
            dailyMissionBubble = AddChild<CommonTextBubbleView>(lockTips);
        }

        public void ToggleLockState(bool isLocked)
        {
            transLock.gameObject.SetActive(isLocked);
            transTimer.gameObject.SetActive(!isLocked);
        }

        public void ToggleReminderState(int nFinishCount)
        {
            txtReminderNotice.text = nFinishCount.ToString();
            transReminder.gameObject.SetActive(nFinishCount > 0);
        }
    }

    public class DailyMissionLobbyEntranceViewController : ViewController<DailyMissionLobbyEntranceView>
    {
        private DailyMissionController _dailyMissionController;

        public override void OnViewDidLoad()
        {
            _dailyMissionController = Client.Get<DailyMissionController>();
            base.OnViewDidLoad();
            EnableUpdate(1);
            var isLocked = _dailyMissionController.IsLocked;
            view.ToggleLockState(isLocked);
            view.ToggleReminderState(_dailyMissionController.GetFinishedMissionCount());
            view.transform.GetComponent<Button>().onClick.AddListener(OnDailyMissionClicked);

            var content = view.transform.Find("Content");
            var pointerEventCustomHandler = view.transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown((eventData) => { content.localScale = Vector3.one * 0.95f; });
            pointerEventCustomHandler.BindingPointerUp((eventData) => { content.localScale = Vector3.one; });
        }

        public override void Update()
        {
            base.Update();
            if (_dailyMissionController.IsLocked) return;
            view.ToggleReminderState(_dailyMissionController.GetFinishedMissionCount());
            if (view.txtTimer)
            {
                view.txtTimer.text = _dailyMissionController.GetNormalMissionTimeLeft();
            }
        }

        private void OnDailyMissionClicked()
        {
            SoundController.PlayButtonClick();
            if (_dailyMissionController.IsLocked)
            {
                if (view.dailyMissionBubble != null)
                {
                    view.dailyMissionBubble.transform.gameObject.SetActive(false);
                    view.dailyMissionBubble.ShowBubble();
                    view.dailyMissionBubble.SetText($"UNLOCK AT LEVEL {Client.Get<DailyMissionController>().UnlockLevel}");
                }
            }
            else
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup),
                    (object) new[] {"DailyMission", "Lobby"})));
            }
        }
    }
}