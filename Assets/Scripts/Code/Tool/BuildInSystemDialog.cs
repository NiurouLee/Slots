// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-01-30 3:36 PM
// Ver : 1.0.0
// Description : BuildInSystemDialog.cs
// ChangeLog :
// **********************************************


using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Tool;
public class BuildInSystemDialog : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI confirmText;
    public TextMeshProUGUI yesText;
    public TextMeshProUGUI noText;
    public TextMeshProUGUI contextText;

    public Button yesButton;
    public Button noButton;
    public Button confirmButton;

    public Action yesAction;
    public Action noAction;
    public Action confirmAction;

    public static BuildInSystemDialog instance;

    private void Awake()
    {
        instance = this;
        BindVariable();
    }
    
    private void BindVariable()
    {
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }
 
    public void ShowDialog(string inContentText, string inConfirmText, Action inConfirmAction = null, string inTitleText = "Notice",
        string inYesText = "", Action inYesAction = null, string inNoText = "", Action inNoAction = null)
    {
        gameObject.transform.SetAsLastSibling();
        
        titleText.text = inTitleText;
        contextText.text = inContentText;
        yesText.text = inYesText;
        noText.text = inNoText;
        confirmText.text = inConfirmText;

        if (String.IsNullOrEmpty(inConfirmText))
        {
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(false);
        }
        else
        {
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(true);
        }

        noAction = inNoAction;
        confirmAction = inConfirmAction;
        yesAction = inYesAction;
        
        transform.parent.Find("AlphaBlack").gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void OnYesButtonClicked()
    {
        yesAction?.Invoke();
        CloseDialog();
    }

    public void OnNoButtonClicked()
    {
        noAction?.Invoke();
        CloseDialog();
    }

    public void OnConfirmButtonClicked()
    {
        confirmAction?.Invoke();
        CloseDialog();
    }

    private void CloseDialog()
    {
        transform.parent.Find("AlphaBlack").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}