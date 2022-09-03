// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-08-27 08:32 PM
// Ver : 1.0.0
// Description : CommonNoticePopup.cs
// ChangeLog :
// **********************************************


using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace GameModule
{
    public class CommonNoticePopup : Popup
    {
        [ComponentBinder("Root/TopGroup/TitleText")]
        public Text titleText;

        [ComponentBinder("Root/BottomGroup/ConfirmButton/Label")]
        public TextMeshProUGUI confirmText;

        [ComponentBinder("Root/BottomGroup/CancelButton/Label")]
        public TextMeshProUGUI cancelText;

        [ComponentBinder("Root/MainGroup/NoticeText")]
        public TextMeshProUGUI contentText;

        [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        public Button confirmButton;

        [ComponentBinder("Root/BottomGroup/CancelButton")]
        public Button cancelButton;

        public Action confirmHandler;

        public Action cancelHandler;

        public CommonNoticePopup(string url) : base(url)
        {
            contentDesignSize = new Vector2(1360,768);
        }
 
        protected override void SetUpController(object inExtraData, object inAsyncExtraData = null)
        {
            BindingComponent();
            
            if (inExtraData is PopupContentInfo contentInfo)
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

       // [ComponentBinder("Root/BottomGroup/ConfirmButton")]
        public void OnConfirmButtonClicked()
        {
            confirmHandler?.Invoke();
            OnCloseClicked();
        }

     //   [ComponentBinder("Root/BottomGroup/CancelButton")]
        public void OnCancelButtonClicked()
        {
            cancelHandler?.Invoke();
            OnCloseClicked();
        }

        public static async void ShowCommonNoticePopUp(string contentText)
        {
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", new PopupContentInfo("Notice", contentText, "Okay"));
        }
        
        public static async void ShowCommonNoticePopUp(string contentText, string confirmText)
        {
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", new PopupContentInfo("Notice", contentText, confirmText));
        }
        
        public static async void ShowCommonNoticePopUp(Action confirmAction, string contentText)
        {
            var popupContentInfo = new PopupContentInfo("Notice", contentText, "Okay");
            popupContentInfo.BindAction(confirmAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
        
        public static async void ShowCommonNoticePopUp(Action confirmAction, string contentText, string confirmText)
        {
            var popupContentInfo = new PopupContentInfo("Notice", contentText, confirmText);
            popupContentInfo.BindAction(confirmAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
        
        public static async void ShowCommonNoticePopUp(Action confirmAction, string contentText, string confirmText, string titleText)
        {
            var popupContentInfo = new PopupContentInfo(titleText, contentText, confirmText);
            popupContentInfo.BindAction(confirmAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
        
        public static async void ShowCommonNoticePopUp(Action confirmAction, Action cancelAction, string contentText, string titleText)
        {
            var popupContentInfo = new PopupContentInfo(titleText, contentText, "Yes", "No");
            popupContentInfo.BindAction(confirmAction, cancelAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
        
        public static async void ShowCommonNoticePopUp(Action confirmAction, Action cancelAction, string contentText)
        {
            var popupContentInfo = new PopupContentInfo("Notice", contentText, "Yes", "No");
            popupContentInfo.BindAction(confirmAction, cancelAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
         
        public static async void ShowCommonNoticePopUp(Action confirmAction, Action cancelAction, string contentText, string confirmText, string cancelText, string titleText)
        {
            var popupContentInfo = new PopupContentInfo(titleText, contentText, confirmText, cancelText);
            popupContentInfo.BindAction(confirmAction, cancelAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
        
        public static async void ShowCommonNoticePopUp(string contentKey, Action confirmAction)
        {
            var popupContentInfo = PredefinedPopupContentInfo.GetContentInfo(contentKey);
            popupContentInfo.BindAction(confirmAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
        
        public static async void ShowCommonNoticePopUp(string contentKey, Action confirmAction, Action cancelAction)
        {
            var popupContentInfo = PredefinedPopupContentInfo.GetContentInfo(contentKey);
            popupContentInfo.BindAction(confirmAction, cancelAction);
            await PopupStack.ShowPopup<CommonNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
    }

    public class HighPriorityNoticePopup : CommonNoticePopup
    {
        public HighPriorityNoticePopup(string url) : base(url)
        {
            
        }

        protected override void SetUpExtraView()
        {
            var mask = new GameObject("Mask");
            var maskRectTransform = mask.AddComponent<RectTransform>();
            maskRectTransform.anchorMin = new Vector2(0, 0);
            maskRectTransform.anchorMax = new Vector2(1, 1);
            maskRectTransform.sizeDelta = new Vector2(ViewResolution.referenceResolutionLandscape.x,
                ViewResolution.referenceResolutionPortrait.y);
            var image = mask.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.7f);

            mask.transform.SetParent(transform, false);
            mask.transform.SetAsFirstSibling();

            transform.localScale = CalculateScaleInfo();
             
            base.SetUpExtraView();
        }

        protected override void OnCloseClicked()
        {
            ViewManager.Instance.RemoveHighPriorityView<HighPriorityNoticePopup>();
        }
         
        public static async void ShowHighPriorityPopUp(string contentKey, Action confirmAction, Action cancelAction)
        {
            var popupContentInfo = PredefinedPopupContentInfo.GetContentInfo(contentKey);
            popupContentInfo.BindAction(confirmAction, cancelAction);
            await ViewManager.Instance.ShowHighPriorityView<HighPriorityNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
    }

    public class NetErrorNoticePopup : HighPriorityNoticePopup
    {
        public NetErrorNoticePopup(string url) : base(url)
        {
            
        }
        
        public static async void ShowPopUp(string contentKey, Action confirmAction)
        {
            var popupContentInfo = PredefinedPopupContentInfo.GetContentInfo(contentKey);
            popupContentInfo.BindAction(confirmAction);
            await ViewManager.Instance.ShowHighPriorityView<NetErrorNoticePopup>("UICommonNoticePanel", popupContentInfo);
        }
    }
}