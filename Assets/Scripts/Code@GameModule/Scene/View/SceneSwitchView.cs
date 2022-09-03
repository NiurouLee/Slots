/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-09-30 21:07:03
Ver : 1.0.0
Description : 
ChangeLog :  
**********************************************/

using System;
using DG.Tweening;
using GameModule.UI;
using UnityEngine;
using UnityEngine.UI;
namespace GameModule
{
    // public class SceneSwitchView: UIView
    // {
    //     private SceneSwitchAction fromSwitchAction;
    //     private SceneSwitchAction toSwitchAction;
    //
    //     private Button backButton;
    //
    //     private Image progressBar;
    //
    //     private bool abortSwitch = false;
    //     
    //     public int SwitchActionId { get; set;}
    //
    //     public float lastAmount = 0;
    //     private bool waitCompleteAction = false;
    //     public float startTime = 0;
    //     
    //     public SceneSwitchView(string url) :
    //         base(url)
    //     {
    //        
    //     }
    //     // public override void Show()
    //     // {
    //     //     gameObject.SetActive(false);
    //     //   //  base.Show();
    //     // //    EnableUpdate();
    //     // //    fromSwitchAction.LeaveScene(OnReadyLeaveLastScene);
    //     // }
    //     
    //     public override void BindingViewVariable()
    //     {
    //         base.BindingViewVariable();
    //
    //         if (transform.Find("UI/LoadingBar/ProgressBar"))
    //         {
    //             progressBar = transform.Find("UI/LoadingBar/ProgressBar").GetComponent<Image>();
    //             progressBar.fillAmount = 0;
    //         }
    //
    //         if (transform.Find("UI/Back/Button"))
    //         {
    //             backButton = transform.Find("UI/Back/Button").GetComponent<Button>();
    //             backButton.onClick.AddListener(OnBackClicked);
    //             backButton.gameObject.SetActive(false);
    //         }
    //         
    //         if (transform.Find("body"))
    //         {
    //            var body = transform.Find("body");
    //            var sizeDelta = ((RectTransform) body).sizeDelta;
    //
    //            var referenceResolution = ViewResolution.referenceResolutionLandscape;
    //            
    //            if (referenceResolution.x < 1280)
    //            {
    //                var scale = (referenceResolution.x / 1280);
    //                body.localScale = new Vector3(scale, scale, 1);
    //            }
    //         }
    //
    //         if (transform.Find("ContactUsBtn"))
    //         {
    //             transform.Find("ContactUsBtn").gameObject.SetActive(false);
    //         }
    //
    //         if (transform.Find("UI/LoadingBar/TipText"))
    //         {
    //             transform.Find("UI/LoadingBar/TipText").gameObject.SetActive(false);
    //         }
    //         
    //         abortSwitch = false;
    //         waitCompleteAction = false;
    //     }
    //     public void OnBackClicked()
    //     {
    //         abortSwitch = true;
    //
    //         backButton.interactable = false;
    //         
    //         Log.LogUiClickEvent(UiClick.LoadingBack);
    //         
    //         toSwitchAction.OnAbortSwitchScene();
    //         EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOBBY));
    //         SoundManager.PlaySlotClickSound();
    //     }
    //
    //     public void HideBackButton()
    //     {
    //         if (backButton != null)
    //         {
    //             backButton.gameObject.SetActive(false);
    //         }
    //     }
    //     
    //     public void BindingSwitchAction(SceneSwitchAction inFromSwitchAction, SceneSwitchAction inToSwitchAction)
    //     {
    //         fromSwitchAction = inFromSwitchAction;
    //         toSwitchAction = inToSwitchAction;
    //        // SwitchActionId = SceneDirector.SwitchActionId;
    //         
    //         if (toSwitchAction.IsPortraitScene())
    //         {
    //             var backTransform = (RectTransform) transform.Find("UI/Back");
    //             if (backTransform)
    //             {
    //                 backTransform.anchorMax = Vector2.one * 0.5f;
    //                 backTransform.anchorMin = Vector2.one * 0.5f;
    //                 backTransform.anchoredPosition = new Vector2(
    //                     ViewResolution.referenceResolutionLandscape.y * 0.5f - 100,
    //                     ViewResolution.referenceResolutionLandscape.x * 0.42f);
    //             }
    //
    //             var tipTransform = (RectTransform) transform.Find("UI/tips");
    //             if (tipTransform)
    //             {
    //                 tipTransform.anchorMax = Vector2.one * 0.5f;
    //                 tipTransform.anchorMin = Vector2.one * 0.5f;
    //                 tipTransform.anchoredPosition = new Vector2(
    //                     -ViewResolution.referenceResolutionLandscape.y * 0.5f + 100,
    //                     ViewResolution.referenceResolutionLandscape.x * 0.42f);
    //             }
    //         }
    //         
    //         StartSwitchScene();
    //     }
    //     
    //     public void StartSwitchScene()
    //     {
    //         EnableUpdate();
    //         fromSwitchAction.LeaveScene(OnReadyLeaveLastScene);
    //     }
    //     
    //     public void OnReadyLeaveLastScene()
    //     {
    //         toSwitchAction.EnterScene();
    //
    //         startTime = Time.realtimeSinceStartup;
    //         
    //         // if (backButton != null && !Client.Get<FeatureGuide>().NeedShowEnterRoomGuide())
    //         // {
    //         //     backButton.gameObject.SetActive(true);
    //         // }   
    //     }
    //
    //     public override void Update()
    //     {
    //         if (progressBar != null && !waitCompleteAction)
    //         {
    //             if (toSwitchAction.GetSwitchProgress() < 1)
    //             {
    //                 var amount = toSwitchAction.GetSwitchProgress();
    //                 var deltaTime = Time.realtimeSinceStartup - startTime;
    //               
    //                 if (deltaTime < 2 || amount <= 0 || amount > 0.8)
    //                 {
    //                     lastAmount += 0.0014f * (1 - lastAmount);
    //              
    //                     //避免走满了，还没进游戏
    //                     if (lastAmount >= 0.98f)
    //                     {
    //                         lastAmount = 0.98f;
    //                     }
    //                     amount  = lastAmount;
    //                 }
    //               //  XDebug.Log("LastAmount:" + amount);
    //               
    //               if(amount > lastAmount)
    //                   lastAmount = amount;
    //                 
    //                 DOTween.Kill(progressBar);
    //                 progressBar.DOFillAmount(lastAmount, 0.5f);
    //             }
    //         }
    //     }
    //
    //     public void DoCompleteAction(Action action)
    //     {
    //         waitCompleteAction = true;
    //         
    //         if (!abortSwitch)
    //         {
    //             if (progressBar != null)
    //             {
    //                 DOTween.Kill(progressBar);
    //                 progressBar.DOFillAmount(1, 1 - progressBar.fillAmount).OnComplete(() =>
    //                 {
    //                     if(!abortSwitch)
    //                         action?.Invoke();
    //                 });
    //             }
    //             else
    //             {
    //                 action?.Invoke();
    //             }
    //         }
    //     }
    // }
}