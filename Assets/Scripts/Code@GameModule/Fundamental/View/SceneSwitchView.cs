// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/21/10:09
// Ver : 1.0.0
// Description : SceneSwitchViewController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    //添加一个类，解决Back中断，横竖屏幕切换的效果的问题
    public class SceneSwitchView2 : SceneSwitchView
    {
        public SceneSwitchView2(string address)
            : base(address)
        {
            
        }
    }
    public class SceneSwitchView : View<SceneSwitchViewController>
    {
        [ComponentBinder("ProgressBar")]
        public Image progressBar;  
        
        [ComponentBinder("Progress")]
        public Slider ProgressBar;  
        
        [ComponentBinder("LoadingText")]
        public TextMeshProUGUI loadingText;
        
        [ComponentBinder("PhoneEmailButton")]
        public Button contactUsButton;
        
        [ComponentBinder("CheckVersion")]
        public Transform checkVersionTransform;
        
        [ComponentBinder("BackButton")]
        public Button backButton;
        
        [ComponentBinder("LoadingRoot")] 
        public Transform loadingRoot;

        public SceneSwitchView(string address)
            : base(address)
        {
            
        }
    }  
    public class SceneSwitchViewController : ViewController<SceneSwitchView>
    {
        private SceneSwitchAction _fromSwitchAction;
        private SceneSwitchAction _toSwitchAction;
 
        private bool _abortSwitch = false;
 
        private float _lastAmount = 0;
        private bool _waitCompleteAction = false;
        private float _startTime = 0;

        public int SwitchActionId { get; set; }
 
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            if (view.progressBar)
            {
                view.progressBar.fillAmount = 0;   
            }
            if (view.ProgressBar)
            {
                view.ProgressBar.value = 0;
            }

            if (view.checkVersionTransform)
            {
                view.checkVersionTransform.gameObject.SetActive(false);
            }

            if (view.loadingRoot)
            {
                view.loadingRoot.gameObject.SetActive(true);
            }
            
            if(view.loadingText)
                view.loadingText.text = "Welcome to CASH CRAZE!";

            if (view.progressBar)
            {
                view.progressBar.gameObject.SetActive(true);
            }

            if (view.contactUsButton)
            {
                view.contactUsButton.gameObject.SetActive(false);
            }

            _abortSwitch = false;
            
            HideBackButton();

            if (view.backButton != null)
            {
                view.backButton.onClick.RemoveAllListeners();
                view.backButton.onClick.AddListener(OnBackClicked);
            }
        }
        public void BindingSwitchAction(SceneSwitchAction inFromSwitchAction, SceneSwitchAction inToSwitchAction)
        {
            SwitchActionId = inFromSwitchAction.SwitchActionId;
            
            _fromSwitchAction = inFromSwitchAction;
            _toSwitchAction = inToSwitchAction;
 
            StartSwitchScene();
        }

        public void StartSwitchScene()
        {
            EnableUpdate();
            _fromSwitchAction.LeaveScene(OnReadyLeaveLastScene);
        }

        public void OnReadyLeaveLastScene()
        {
            _toSwitchAction.EnterScene();

            _startTime = Time.realtimeSinceStartup;

            if (view.backButton != null && Client.Get<GuideController>().GetEnterMachineGuide() == null)
            {
                if (_toSwitchAction.AllowBackButton())
                    view.backButton.gameObject.SetActive(true);
            }   
        }

        public void OnBackClicked()
        {
            _abortSwitch = true;

            view.backButton.interactable = false;
             
            _toSwitchAction.OnAbortSwitchScene();
            
            EventBus.Dispatch(new EventBackToLastScene());
        }
        
        public override void Update()
        {
            if (!_waitCompleteAction)
            {
                if (_toSwitchAction.GetSwitchProgress() < 1)
                {
                    var amount = _toSwitchAction.GetSwitchProgress();
                    var deltaTime = Time.realtimeSinceStartup - _startTime;
            
                    if (deltaTime < 2 || amount <= 0 || amount > 0.8)
                    {
                        _lastAmount += 0.0014f * (1 - _lastAmount);
            
                        //避免走满了，还没进游戏
                        if (_lastAmount >= 0.98f)
                        {
                            _lastAmount = 0.98f;
                        }
            
                        amount = _lastAmount;
                    }
                    //  XDebug.Log("LastAmount:" + amount);
            
                    if (amount > _lastAmount)
                        _lastAmount = amount;

                    if (view.ProgressBar)
                    {
                        DOTween.Kill(view.ProgressBar);
                        view.ProgressBar.DOValue(_lastAmount, 0.5f);   
                    }

                    if (view.progressBar)
                    {
                        DOTween.Kill(view.progressBar);
                        view.progressBar.DOFillAmount(_lastAmount, 0.5f);   
                    }
                }
            }
        }

        public void HideBackButton()
        {
            if (view.backButton != null)
            {
                view.backButton.gameObject.SetActive(false);
            }
        }
        
        public void DoCompleteAction(Action action)
        {
            _waitCompleteAction = true;

            if (!_abortSwitch)
            {
                if (view.progressBar != null)
                {
                    DOTween.Kill(view.progressBar);
                    view.progressBar.DOFillAmount(1, 1 - view.progressBar.fillAmount).OnComplete(() =>
                    {
                        if (!_abortSwitch)
                            action?.Invoke();
                    });
                }
                else if (view.ProgressBar)
                {
                    DOTween.Kill(view.ProgressBar);
                    view.ProgressBar.DOValue(1, 1 - view.ProgressBar.value).OnComplete(() =>
                    {
                        if (!_abortSwitch)
                            action?.Invoke();
                    });
                }
                else
                {
                    action?.Invoke();
                }
            }
        }
    }
}