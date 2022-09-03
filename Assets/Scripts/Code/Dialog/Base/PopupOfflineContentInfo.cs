using System;

public class PopupOfflineContentInfo
{
    public string title;
    public string contentText;
    public string confirmText;
    public string cancelText;

    public Action confirmAction;
    public Action cancelAction;

    private PopupOfflineContentInfo(PopupOfflineContentInfo dialogContent)
    {
        contentText = dialogContent.contentText;
        confirmText = dialogContent.confirmText;
        cancelText = dialogContent.cancelText;
        confirmAction = dialogContent.confirmAction;
        cancelAction = dialogContent.cancelAction;
        title = dialogContent.title;
    }
        
    public PopupOfflineContentInfo(string inTitle, string inContent, string inConfirmText, string inCancelText = "")
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
        
    public PopupOfflineContentInfo CloneContent()
    {
        return new PopupOfflineContentInfo(this);
    }
}