//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 14:37
//  Ver : 1.0.0
//  Description : SeasonPassLobbyEntranceView.cs
//  ChangeLog :
//  **********************************************

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassLobbyEntranceView: View<SeasonPassLobbyEntranceViewController>
    {
        [ComponentBinder("LockState")] 
        public Transform transLock;
        [ComponentBinder("Content/ReminderGroup")] 
        private Transform transReminder;
        [ComponentBinder("Content/ReminderGroup/NoticeText")] 
        private TextMeshProUGUI txtReminderNotice;
        
        // [ComponentBinder("Timer")] 
        // public Transform transTimer;
        [ComponentBinder("LobbyTextBubbleM")] public Transform lockTips;

        public CommonTextBubbleView seasonPassBubble;

        public SeasonPassLobbyEntranceView()
            : base(null)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            seasonPassBubble = AddChild<CommonTextBubbleView>(lockTips);
        }

        public void ToggleLockState(bool isLocked)
        {
            transLock.gameObject.SetActive(isLocked);
            //transTimer.gameObject.SetActive(!isLocked);
        }
        public void ToggleReminderState(int nFinishCount)
        {
            txtReminderNotice.text = nFinishCount.ToString();
            transReminder.gameObject.SetActive(nFinishCount > 0);
        }
    }

    public class SeasonPassLobbyEntranceViewController : ViewController<SeasonPassLobbyEntranceView>
    {
        private SeasonPassController _seasonPassController;
        public override void OnViewDidLoad()
        {
            _seasonPassController = Client.Get<SeasonPassController>();
            base.OnViewDidLoad();
            EnableUpdate(1);
            var isLocked = _seasonPassController.IsLocked;
            view.ToggleLockState(isLocked);
            view.ToggleReminderState(_seasonPassController.CollectRewardCount);
            view.transform.GetComponent<Button>().onClick.AddListener(OnSeasonPassClicked);
            
            var content = view.transform.Find("Content");
            var pointerEventCustomHandler = view.transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown((eventData) => { content.localScale = Vector3.one * 0.95f; });
            pointerEventCustomHandler.BindingPointerUp((eventData) => { content.localScale = Vector3.one; });
        }

        public override void Update()
        {
            base.Update();
            if (_seasonPassController.IsLocked) return;
            view.ToggleReminderState(_seasonPassController.CollectRewardCount);
        }
        
        private void OnSeasonPassClicked()
        {
            SoundController.PlayButtonClick();
            if (_seasonPassController.IsLocked)
            {
                if (view.seasonPassBubble != null)
                {
                    view.seasonPassBubble.transform.gameObject.SetActive(false);
                    view.seasonPassBubble.ShowBubble();
                    view.seasonPassBubble.SetText($"UNLOCK AT LEVEL {Client.Get<SeasonPassController>().UnlockLevel}");
                }
            }
            else
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup), (object)new []{"SeasonPass","Lobby"})));    
            }
        }
    }
}