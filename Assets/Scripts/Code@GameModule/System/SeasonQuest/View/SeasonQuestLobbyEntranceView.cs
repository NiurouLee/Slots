// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/08/17:14
// Ver : 1.0.0
// Description : QuestLobbyEnteranceView.cs
// ChangeLog :
// **********************************************
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonQuestLobbyEntranceView:View<SeasonQuestLobbyEntranceViewController>
    {
        [ComponentBinder("LockState")] 
        public Transform lockState;
        
        [ComponentBinder("ReminderGroup")] 
        private Transform transReminder;
        [ComponentBinder("ReminderGroup/NoticeText")] 
        private TextMeshProUGUI txtReminderNotice;
       
        [ComponentBinder("Timer")] 
        public Transform timerTransform;
        
        [ComponentBinder("Timer/TimerText")] 
        public TextMeshProUGUI txtTimer;
        
        [ComponentBinder("LobbyTextBubbleM")] public Transform lockTips;

        public CommonTextBubbleView bubbleView;
        
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            bubbleView = AddChild<CommonTextBubbleView>(lockTips);
        }
    }
     
    public class SeasonQuestLobbyEntranceViewController:ViewController<SeasonQuestLobbyEntranceView>
    {
        private SeasonQuestController _seasonQuestController;
         
        public override void OnViewEnabled()
        {
            _seasonQuestController = Client.Get<SeasonQuestController>();
            
            base.OnViewEnabled();
            
            RefreshEntranceState();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSeasonQuestSeasonFinish>(OnSeasonQuestFinished);
            SubscribeEvent<EventSeasonQuestUnlock>(OnSeasonQuestUnlocked);
        }
        
      
        protected void OnSeasonQuestUnlocked(EventSeasonQuestUnlock evt)
        {
            RefreshEntranceState();
        }

        
        protected void OnSeasonQuestFinished(EventSeasonQuestSeasonFinish evt)
        {
            RefreshEntranceState();
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            var button = view.transform.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnQuestButtonClicked);
            
        //    view.lockStateButton.onClick.AddListener(OnLockStateButtonClicked);
            
            var content = view.transform.Find("Content");
            var pointerEventCustomHandler = view.transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown((eventData) => { content.localScale = Vector3.one * 0.95f; });
            pointerEventCustomHandler.BindingPointerUp((eventData) => { content.localScale = Vector3.one; });
        }

        // public void OnLockStateButtonClicked()
        // {
        //     view.bubbleView.ShowBubble();
        //     SoundController.PlayButtonClick();
        // }

        public void OnQuestButtonClicked()
        {
            SoundController.PlayButtonClick();
            if (_seasonQuestController.IsLocked())
            {
                view.bubbleView.SetText($"Unlock At LEVEL {_seasonQuestController.GetUnlockLevel()}");
                view.bubbleView.ShowBubble(3);
            }
            else if (_seasonQuestController.IsTimeOut())
            {
                view.bubbleView.SetText($"NEW Season QUEST IS ON THE WAY!");
                view.bubbleView.ShowBubble(3);
            }
            else
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SeasonQuestPopup), "Lobby")));
            }
        }
        
        public void RefreshEntranceState()
        {
            var isLocked = _seasonQuestController.IsLocked()
                           || _seasonQuestController.GetQuestCountDown() <= 0;

            if (isLocked)
            {
                view.transform.gameObject.SetActive(false);
                return;
            }
            
            view.transform.gameObject.SetActive(true);

            view.timerTransform.gameObject.SetActive(true);
            
            var icon = view.transform.Find("Content/Icon");
            
            if (icon) {
                icon.gameObject.SetActive(!isLocked);
            }
            
            view.lockState.gameObject.SetActive(isLocked);

            if (!_seasonQuestController.IsLocked())
            {
                view.txtTimer.text = XUtility.GetTimeText(_seasonQuestController.GetQuestCountDown());
                
                EnableUpdate(1);
            }
        }
        
        public override void Update()
        {
            var countDown = _seasonQuestController.GetQuestCountDown();
            if (countDown > 0)
            {
                view.txtTimer.text = XUtility.GetTimeText(countDown);
            }
            else
            {
                EventBus.Dispatch(new EventSeasonQuestSeasonFinish());
                view.timerTransform.gameObject.SetActive(false);
                RefreshEntranceState();
                DisableUpdate();
            }
        }
    }
}