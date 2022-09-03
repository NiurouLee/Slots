using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIInboxEntranceView : View<UIInboxEntranceViewController>
    {
        [ComponentBinder("")]
        public Button button;

        [ComponentBinder("Content/ReminderGroup")]
        public Transform transformReminderGroup;

        [ComponentBinder("Content/ReminderGroup/NoticeText")]
        public TMP_Text tmpTextNotice;

        public void SetNoticeCount(int count)
        {
            if (transformReminderGroup != null)
            {
                if (count > 0)
                {
                    transformReminderGroup.gameObject.SetActive(true);
                    if (tmpTextNotice != null) { tmpTextNotice.text = count.ToString(); }
                }
                else
                {
                    transformReminderGroup.gameObject.SetActive(false);
                }
            }
        }
    }

    public class UIInboxEntranceViewController : ViewController<UIInboxEntranceView>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.button.onClick.AddListener(OnButtonClick);
            EnableUpdate(1);
            RefreshNotice();

            var content = view.transform.Find("Content");
            var pointerEventCustomHandler = view.transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown((eventData) =>
            {
                content.localScale = Vector3.one * 0.95f;
            });
            pointerEventCustomHandler.BindingPointerUp((eventData) =>
            {
                content.localScale = Vector3.one;
            });
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventInBoxItemUpdated>(OnInxBoxItemUpdatedList);
        }
 
        private void OnInxBoxItemUpdatedList(EventInBoxItemUpdated obj)
        {
            RefreshNotice();
        }

        private void RefreshNotice()
        {
            var inboxController = Client.Get<InboxController>();
            if (inboxController != null)
            {
                view.SetNoticeCount(inboxController.GetAllInboxItem().Count);
            }
        }
  
        private async void OnButtonClick()
        {
            SoundController.PlayButtonClick();
            await PopupStack.ShowPopup<UIInboxPopup>();
        }
    }
}