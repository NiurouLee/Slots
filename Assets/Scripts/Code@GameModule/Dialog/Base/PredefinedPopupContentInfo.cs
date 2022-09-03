// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-11-13 3:28 PM
// Ver : 1.0.0
// Description : DialogArgs.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class PopupContentInfo
    {
        public string title;
        public string contentText;
        public string confirmText;
        public string cancelText;

        public Action confirmAction;
        public Action cancelAction;

        private PopupContentInfo(PopupContentInfo dialogContent)
        {
            contentText = dialogContent.contentText;
            confirmText = dialogContent.confirmText;
            cancelText = dialogContent.cancelText;
            confirmAction = dialogContent.confirmAction;
            cancelAction = dialogContent.cancelAction;
            title = dialogContent.title;
        }
        
        public PopupContentInfo(string inTitle, string inContent, string inConfirmText, string inCancelText = "")
        {
            title = inTitle;
            contentText = inContent;
            confirmText = inConfirmText;
            cancelText = inCancelText;
        }
        
        public void BindAction(Action inConfirmAction, Action inCancelAction = null)
        {
            confirmAction = inConfirmAction;
            cancelAction = inCancelAction;
        }
        
        public PopupContentInfo CloneContent()
        {
            return new PopupContentInfo(this);
        }
    }

    public static class PredefinedPopupContentInfo
    {
        private static Dictionary<string, PopupContentInfo> contentInfoDict;

        static PredefinedPopupContentInfo()
        {
            contentInfoDict = new Dictionary<string, PopupContentInfo>();

            contentInfoDict.Add("UI_UPDATE_NEW_VER",
                new PopupContentInfo("New Update", "There is a new version of this game available to you now!", "Update",
                    "Cancel"));
            contentInfoDict.Add("UI_WARNING_CONNECT_TO_FB_FAILED",
                new PopupContentInfo("Warning", "Failed to connect to Facebook.", "Ok"));
            contentInfoDict.Add("UI_IAP_PENDING",
                new PopupContentInfo("Notice", "Almost there! \n Go complete your purchase now.", "Okay"));
            contentInfoDict.Add("UI_IAP_IS_PENDING",
                new PopupContentInfo("Notice", "Oops! You've already selected the item, go complete your purchase now.",
                    "Okay"));
            contentInfoDict.Add("UI_SERVER_ERROR_10001",
                new PopupContentInfo("Notice", "Login failed, please try again later.", "Ok"));
            contentInfoDict.Add("UI_SERVER_ERROR_10002",
                new PopupContentInfo("Notice", "Failed to connect to Facebook.", "Ok"));
            contentInfoDict.Add("UI_SERVER_ERROR_10004",
                new PopupContentInfo("Notice", "Sorry! We're under maintenance at the moment. Please try again soon!\n",
                    "Okay"));
            
            contentInfoDict.Add("UI_CONNECTION_LOST",
                new PopupContentInfo("Notice", "Your connection has been lost! Check your Internet connection and try again!",
                    "RECONNECT"));
            contentInfoDict.Add("UI_NOTICE_IAP_NOT_READY",
                new PopupContentInfo("SORRY!", "The purchase did not go through. You were not charged. Please try again later.", 
                    "OK"));
            
            contentInfoDict.Add("UI_FB_LOGOUT_WARNING",
                new PopupContentInfo("Notice", "If you log out, you may lose your account. For the security of your account, we recommend that you don't do this.Are you sure to continue?", 
                    "YES", "NO"));
            
            contentInfoDict.Add("UI_QUIT_GAME",
                new PopupContentInfo("Warning", "Are you sure you want to exit the game?", "Quit", "Cancel"));
            
            contentInfoDict.Add("UI_PURCHASE_FAILED",
                new PopupContentInfo("PURCHASE FAILED", "Sorry, your account has not been charged, please try again later.", "Ok"));

            
            contentInfoDict.Add("UI_COUPON_SPINNING",
                new PopupContentInfo("Notice", "You are currently spinning. Tap again when your spin has finished.", "Confirm"));
            
            contentInfoDict.Add("UI_NOTICE_ERROR_OCCURRED",
                new PopupContentInfo("SORRY!", "Oops! An unexpected error occurred. Please try again later.", 
                    "OK"));
            
            contentInfoDict.Add("COUPON_ERROR_98",
                new PopupContentInfo("SORRY!", "Oops! The coupon you claimed is expired.", 
                    "OK"));
            
            contentInfoDict.Add("COUPON_ERROR_97",
                new PopupContentInfo("SORRY!", "Oops! The coupon is already claimed.", 
                    "OK"));
            
            contentInfoDict.Add("COUPON_ERROR_96",
                new PopupContentInfo("SORRY!", "Oops! The coupon you claimed is not exist.", 
                    "OK"));
            
            contentInfoDict.Add("COUPON_ERROR_99",
                new PopupContentInfo("SORRY!", "Oops! The coupon you claimed is give out.", 
                    "OK"));
        }

        public static PopupContentInfo GetContentInfo(string contentKey)
        {
            if (contentInfoDict.ContainsKey(contentKey))
            {
                return contentInfoDict[contentKey].CloneContent();
            }
            
            return null;
        }
    }
}