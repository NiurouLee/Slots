using System;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    [AssetAddress("UIInboxMain")]
    public class UIInboxPopup : Popup<UIInboxController>
    {
        [ComponentBinder("Root/ShiftGroup/InboxButton")]
        public Button buttonInbox;

        [ComponentBinder("Root/ShiftGroup/InboxButton/EnableState")]
        public Transform transformInboxEnableState;

        [ComponentBinder("Root/ShiftGroup/InboxButton/Reminder/BG/CountText")]
        public TMP_Text tmpTextInboxNoticeCount;

        [ComponentBinder("Root/ShiftGroup/InboxButton/Reminder")]
        public Transform transformInboxReminder;
 
        [ComponentBinder("Root/ShiftGroup/SendButton")]
        public Button buttonSend;

        [ComponentBinder("Root/ShiftGroup/SendButton/EnableState")]
        public Transform transformSendEnableState;

        [ComponentBinder("Root/ShiftGroup/SendButton/Reminder/BG/CountText")]
        public TMP_Text tmpTextSendNoticeCount;

        [ComponentBinder("Root/ShiftGroup/SendButton/Reminder")]
        public Transform transformSendReminder;


        [ComponentBinder("Root/InboxGroup")]
        public Transform transformInboxGroup;

        [ComponentBinder("Root/SendGroup")]
        public Transform transformSendGroup;

        public UIInboxPopupInboxGroupView inboxGroup;
        public UIInboxPopupSendGroupView sendGroup;
        private InboxController _inboxController;


        public enum State
        {
            None, Inbox, Send
        }

        public State currentState { get; private set; } = State.None;

        public UIInboxPopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1365, 768);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _inboxController = Client.Get<InboxController>();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            Set(true);
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            inboxGroup = AddChild<UIInboxPopupInboxGroupView>(transformInboxGroup);
            sendGroup = AddChild<UIInboxPopupSendGroupView>(transformSendGroup);
        }


        public void SetState(State state, bool forceUpdate)
        {
            if (currentState == state && forceUpdate == false) { return; }
            currentState = state;
            switch (state)
            {
                case State.Inbox:
                    transformInboxGroup.gameObject.SetActive(true);
                    transformSendGroup.gameObject.SetActive(false);
                    buttonInbox.transform.SetAsLastSibling();
                    transformInboxEnableState.gameObject.SetActive(true);
                    transformSendEnableState.gameObject.SetActive(false);

                    if (inboxGroup != null)
                    {
                        inboxGroup.viewController.RefreshInboxContent();
                    }
                    break;
                case State.Send:
                    transformInboxGroup.gameObject.SetActive(false);
                    transformSendGroup.gameObject.SetActive(true);
                    buttonSend.transform.SetAsLastSibling();
                    transformInboxEnableState.gameObject.SetActive(false);
                    transformSendEnableState.gameObject.SetActive(true);
                    break;
                default:
                    SetState(State.Inbox, forceUpdate);
                    break;
            }
        }

        public void SetNotice()
        {
            if (_inboxController != null)
            {
                var itemCount = _inboxController.GetAllInboxItem().Count;
                if (transformInboxReminder != null)
                {
                    transformInboxReminder.gameObject.SetActive(itemCount > 0);
                }

                if (tmpTextInboxNoticeCount != null)
                {
                    tmpTextInboxNoticeCount.text = itemCount.ToString();
                }

                //TODO:Set correct data
                if (transformSendReminder != null)
                {
                    transformSendReminder.gameObject.SetActive(false);
                }

                if (tmpTextSendNoticeCount != null)
                {
                    tmpTextSendNoticeCount.text = null;
                }
            }
        }

        public void Set(bool forceUpdate)
        {
            SetNotice();
            SetState(State.Inbox, forceUpdate);
        }

        // public void Update()
        // {
        //     if (inboxGroup != null) { inboxGroup.Update(); }
        // }

        public override string GetOpenAudioName()
        {
            return "General_OpenInbox";
        }
    }

    public class UIInboxController : ViewController<UIInboxPopup>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.buttonInbox.onClick.AddListener(OnInboxButtonClicked);
            view.buttonSend.onClick.AddListener(OnSendButtonClicked);

         //   EnableUpdate(1);

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventInboxOpen, ("messageCount", Client.Get<InboxController>().GetAllInboxItem().Count.ToString()));
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventInBoxItemUpdated>(OnEventInBoxItemUpdated);
        }

        private void OnEventInBoxItemUpdated(EventInBoxItemUpdated obj)
        {
            view.SetNotice();
            view.inboxGroup.viewController.RefreshInboxContent();
        }

        private void OnInboxButtonClicked()
        {
            if (view.currentState != UIInboxPopup.State.Inbox)
            {
                SoundController.PlaySwitchFx();
            }
            view.SetState(UIInboxPopup.State.Inbox, false);
        }

        private void OnSendButtonClicked()
        {
            if (view.currentState != UIInboxPopup.State.Send)
            {
                SoundController.PlaySwitchFx();
            }
            view.SetState(UIInboxPopup.State.Send, false);
        }

        // public override void Update()
        // {
        //     view.Update();
        // }
        
        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            
            EventBus.Dispatch(new EventTriggerPopupPool("CloseInbox", null));
        }
    }
}