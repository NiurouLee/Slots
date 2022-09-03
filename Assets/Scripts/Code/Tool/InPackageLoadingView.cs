// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/13/21:38
// Ver : 1.0.0
// Description : InPackageLoadingView.cs
// ChangeLog :
// **********************************************

using System;
using DG.Tweening;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InPackageLoadingView : MonoBehaviour
{
    [OffineComponentBinder("LoadingRoot")] public Transform loadingRoot;

    [OffineComponentBinder("LoginRoot")] public Transform loginRoot;

    [OffineComponentBinder("Progress")] public Slider progressBar;

    [OffineComponentBinder("LoadingText")] public TextMeshProUGUI loadingText;
    [OffineComponentBinder("CheckVersion")] public Transform checkVersionGameObject;
   
    [OffineComponentBinder("PhoneEmailButton")]
    public Button contactUsButton;
 
    private void Awake()
    {
        OffineComponentBinder.BindingComponent(this, transform);    
    }

    private void Start()
    {
        if (loadingRoot)
        {
            loadingRoot.gameObject.SetActive(false);
        }

        if (loginRoot)
        {
            loginRoot.gameObject.SetActive(false);
        }

        if (checkVersionGameObject)
        {
            checkVersionGameObject.gameObject.SetActive(true);
        }

        if (contactUsButton)
        {
            contactUsButton.onClick.AddListener(OnContactUsClicked);
        }
    }
    
    private void OnContactUsClicked()
    {
        string email = "support@casualjoygames.com";

        var userId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId;
        var deviceId = DeviceHelper.GetDeviceId();
            
        string subject = $"HelpMail_From_{userId}";

        string body = $"userId:{userId},deviceId:{deviceId}";

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    
}