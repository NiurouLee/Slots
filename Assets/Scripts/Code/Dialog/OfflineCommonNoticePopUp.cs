using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


     public class OfflineCommonNoticePopup : PopupOffline
    {
        [OffineComponentBinder("Root/TopGroup/TitleText")]
        public Text titleText;

        [OffineComponentBinder("Root/BottomGroup/ConfirmButton/Label")]
        public TextMeshProUGUI confirmText;

        [OffineComponentBinder("Root/BottomGroup/CancelButton/Label")]
        public TextMeshProUGUI cancelText;

        [OffineComponentBinder("Root/MainGroup/DescriptionText")]
        public TextMeshProUGUI contentText;

        [OffineComponentBinder("Root/BottomGroup/ConfirmButton")]
        public Button confirmButton;

        [OffineComponentBinder("Root/BottomGroup/CancelButton")]
        public Button cancelButton;

        public Action confirmHandler;

        public Action cancelHandler;


       


        public override void SetInfo(object inExtraData)
        {
            contentDesignSize = new Vector2(1134,653);
            
            base.SetInfo(inExtraData);
            if (inExtraData is PopupOfflineContentInfo contentInfo)
            {
                titleText.text = contentInfo.title.ToUpper();
                confirmText.text = contentInfo.confirmText;
                contentText.text = contentInfo.contentText;

                confirmHandler = contentInfo.confirmAction;
                cancelHandler = contentInfo.cancelAction;
                
                if (!string.IsNullOrEmpty(contentInfo.cancelText))
                {
                    cancelText.text = contentInfo.cancelText;
                }
                else
                {
                    cancelButton.gameObject.SetActive(false);
                }
            }
            
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

       // [OffineComponentBinder("Root/BottomGroup/ConfirmButton")]
        public void OnConfirmButtonClicked()
        {
            confirmHandler?.Invoke();
            OnCloseClicked();
        }

     //   [OffineComponentBinder("Root/BottomGroup/CancelButton")]
        public void OnCancelButtonClicked()
        {
            cancelHandler?.Invoke();
            OnCloseClicked();
        }

        public static async void ShowCommonOfflineNoticePopup(string contentText)
        {
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", new PopupOfflineContentInfo("Notice", contentText, "Okay"));
        }
        
        public static async void ShowCommonOfflineNoticePopup(string contentText, string confirmText)
        {
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", new PopupOfflineContentInfo("Notice", contentText, confirmText));
        }
        
        public static async void ShowCommonOfflineNoticePopup(Action confirmAction, string contentText)
        {
            var PopupOfflineContentInfo = new PopupOfflineContentInfo("Notice", contentText, "Okay");
            PopupOfflineContentInfo.BindAction(confirmAction);
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", PopupOfflineContentInfo);
        }
        
        public static async void ShowCommonOfflineNoticePopup(Action confirmAction, string contentText, string confirmText)
        {
            var PopupOfflineContentInfo = new PopupOfflineContentInfo("Notice", contentText, confirmText);
            PopupOfflineContentInfo.BindAction(confirmAction);
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", PopupOfflineContentInfo);
        }
        
        public static async void ShowCommonOfflineNoticePopup(Action confirmAction, string contentText, string confirmText, string titleText)
        {
            var PopupOfflineContentInfo = new PopupOfflineContentInfo(titleText, contentText, confirmText);
            PopupOfflineContentInfo.BindAction(confirmAction);
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", PopupOfflineContentInfo);
        }
        
        public static async void ShowCommonOfflineNoticePopup(Action confirmAction, Action cancelAction, string contentText, string titleText)
        {
            var PopupOfflineContentInfo = new PopupOfflineContentInfo(titleText, contentText, "Yes", "No");
            PopupOfflineContentInfo.BindAction(confirmAction, cancelAction);
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", PopupOfflineContentInfo);
        }
        
        public static async void ShowCommonOfflineNoticePopup(Action confirmAction, Action cancelAction, string contentText)
        {
            var PopupOfflineContentInfo = new PopupOfflineContentInfo("Notice", contentText, "Yes", "No");
            PopupOfflineContentInfo.BindAction(confirmAction, cancelAction);
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", PopupOfflineContentInfo);
        }
         
        public static async void ShowCommonOfflineNoticePopup(Action confirmAction, Action cancelAction, string contentText, string confirmText, string cancelText, string titleText)
        {
            var PopupOfflineContentInfo = new PopupOfflineContentInfo(titleText, contentText, confirmText, cancelText);
            PopupOfflineContentInfo.BindAction(confirmAction, cancelAction);
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIErorrNotice", PopupOfflineContentInfo);
        }
        
       
        public static async void ShowCommonOfflineUpdatePopup(Action confirmAction)
        {
            var PopupOfflineContentInfo = new PopupOfflineContentInfo("UPDATE REQUIRED","UPDATE TO LATEST VERSION TO ENJOY THE EXCITING NEW FEATURES!","UPDATE");
            PopupOfflineContentInfo.BindAction(confirmAction);
            await PopupOfflineStack.ShowPopupOffline<OfflineCommonNoticePopup>("UIRequired", PopupOfflineContentInfo);
        }
    }
