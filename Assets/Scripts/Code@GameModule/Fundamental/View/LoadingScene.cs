// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/21/11:33
// Ver : 1.0.0
// Description : LoadingView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class LoadingScene : SceneView<LoadingSceneViewController>
    {
        public override bool IsPortraitScene { get; } = false;
        public override SceneType SceneType { get; } = SceneType.TYPE_LOADING;

        [ComponentBinder("LoadingRoot")] 
        public Transform loadingRoot;
        
        [ComponentBinder("LoginRoot")] 
        public Transform loginRoot;
        
        [ComponentBinder("LoginButton")] 
        public Transform loginButton;
        
        [ComponentBinder("LoginTwoTypeButton")] 
        public Transform loginTwoTypeButton;
        
        [ComponentBinder("Progress")]
        public Slider progressBar;

        [ComponentBinder("LoadingText")]
        public TextMeshProUGUI loadingText;  
        
        [ComponentBinder("PhoneEmailButton")]
        public Button contactUsButton;
        
        //Login Context
        [ComponentBinder("PolicyText")] 
        public TextMeshProUGUI policyText;

        [ComponentBinder("LoginRoot/LoginButton/FBButton")]
        public Button fbButton;  
        
        [ComponentBinder("LoginRoot/LoginButton/GuestButton")]
        public Button guestButton; 
        
        [ComponentBinder("LoginRoot/LoginButton/AppleButton")]
        public Button appleButton; 
        
        [ComponentBinder("LoginRoot/LoginTwoTypeButton/FBButton")]
        public Button fbButton2;
        
        [ComponentBinder("LoginRoot/LoginTwoTypeButton/GuestButton")]
        public Button guestButton2;
         
        [ComponentBinder("CheckVersion")] public Transform checkVersionTransform;
        [ComponentBinder("CheckVersionText")] public TextMeshProUGUI checkVersionText;
        
        public LoadingScene()
            : base("", SceneType.TYPE_LOADING)
        {
            
        }
        public LoadingScene(string address)
            : base(address, SceneType.TYPE_LOADING)
        {
        }

        protected override void OnViewSetUpped()
        {
            progressBar.value = 0.0f;
              
            var inPackageLoadingView = gameObject.GetComponent<InPackageLoadingView>();
            if (inPackageLoadingView)
            {
                GameObject.Destroy(inPackageLoadingView);
            }
            
            base.OnViewSetUpped();
        }

        public void UpdateLoadingProgress(float amount, float duration)
        {
            if (amount < progressBar.value)
                return;

            DOTween.Kill(progressBar);

            progressBar.DOValue(amount, duration);
        }
    }

    public class LoadingSceneViewController : ViewController<LoadingScene>
    {
        private AssetDependenciesDownloadOperation _assetDependenciesDownloadOperation;

        private bool _loadAssetsCompleted = false;

        private long _coreNeedDownloadBytes = 0;
        private long _coreDownloadBytes = 0;
        private long _firstNeedDownloadBytes = 0;
        private long _firstDownloadBytes = 0;

        private int _sceneSwitchId = 0;
        private bool _enterLobbyServerInfoReady = false;
          
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
             
            _loadAssetsCompleted = false;
            
            SoundController.PlaySfx("Welcome_Cash_Craze");

          //  BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventEnterLoadingScene);
         //   LoadCoreAssets();
            
            view.fbButton.onClick.AddListener(OnFacebookLoginClicked);
            view.fbButton2.onClick.AddListener(OnFacebookLoginClicked);
            view.guestButton.onClick.AddListener(OnGuestLoginClicked);
            view.guestButton2.onClick.AddListener(OnGuestLoginClicked);
            view.appleButton.onClick.AddListener(OnAppleLoginClicked);

            if (view.contactUsButton)
            {
                view.contactUsButton.onClick.AddListener(OnContactUsClicked);
            }
            
            var pointerEventCustomHandler = view.policyText.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnPointerClick);
            
            StartLogin();
        }

        protected void OnContactUsClicked()
        {
            XUtility.SendHelpEmail();
        }

        private async void LoadCoreAssets()
        {
            PerformanceTracker.AddTrackPoint("CallLoadAssetPriorityLevel1");
            PerformanceTracker.AddTrackPoint("LoadAssetPriorityLevel1");
            XDebug.Log("[[ShowOnExceptionHandler]] Start LoadCoreAssets");
#if !UNITY_EDITOR    
            var labels = new List<string>() {"AssetPriorityLevel1"};
            var needDownloadLabels = Client.Get<ActivityController>().GetNeedDownloadActivityHighPriorityAssets();
            
            if (needDownloadLabels != null)
            {
                labels.AddRange(needDownloadLabels);
            }
            
            _assetDependenciesDownloadOperation = AssetHelper.DownloadDependencies(labels,
                (success) =>
                {
#endif
                    PerformanceTracker.FinishTrackPoint("LoadAssetPriorityLevel1");
                    PerformanceTracker.AddTrackPoint("PrepareResidentAssets");
                    AssetHelper.PrepareResidentAssets(() =>
                    {
                        _loadAssetsCompleted = true;
                        PerformanceTracker.FinishTrackPoint("PrepareResidentAssets");
                    });
#if !UNITY_EDITOR                      
                }, true);
#endif

            PerformanceTracker.FinishTrackPoint("CallLoadAssetPriorityLevel1");
            
            _coreNeedDownloadBytes = 0;
            
#if !UNITY_EDITOR                 
            _coreNeedDownloadBytes =
                _assetDependenciesDownloadOperation.GetOperationHandle().GetDownloadStatus().TotalBytes;
#endif
            
            EnableUpdate(0);
        }

        public float GetLoadingProgress()
        {
            if (_loadAssetsCompleted || _coreNeedDownloadBytes == 0)
                return 0.9f;

            if (_assetDependenciesDownloadOperation.GetOperationHandle().IsValid())
            {
                _coreDownloadBytes = _assetDependenciesDownloadOperation.GetOperationHandle().GetDownloadStatus()
                    .DownloadedBytes;
            }

            var amount = (float) _coreDownloadBytes / _coreNeedDownloadBytes;

            return Mathf.Min(amount,0.9f);
        }

        public string GetLoadingText()
        {
            if (_coreNeedDownloadBytes > 0)
            {
                return $"Downloading ({_coreDownloadBytes/1024}KB/{_coreNeedDownloadBytes/1024}KB)";
            }
            else
            {
                return "Check Resources";
            }
        }

        public override void Update()
        {
            if (_loadAssetsCompleted && _enterLobbyServerInfoReady)
            {
                view.loadingText.text = "Welcome to CASH CRAZE!";
                DisableUpdate();
                view.UpdateLoadingProgress(1, 0.1f);
                OnLoadAssetComplete();
            }
            else
            {
                view.loadingText.text = GetLoadingText();
                var amount = GetLoadingProgress();
                view.UpdateLoadingProgress(amount, 2.0f);
            }
        }

        private void OnLoadAssetComplete()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDownloadResource);
            EventBus.Dispatch(new EventSceneSwitchMask(SwitchMask.MASK_RESOURCE_READY, _sceneSwitchId));
            EventBus.Dispatch(new EventLoadingP1AssetComplete());
            
//             if (!AccountManager.Instance.HasLogin)
//             {
//                view.loadingRoot.gameObject.SetActive(false);
//                view.loginRoot.gameObject.SetActive(true);
// #if UNITY_ANDROID
//                 view.loginTwoTypeButton.gameObject.SetActive(true);
//                 view.loginButton.gameObject.SetActive(false);
// #elif UNITY_IOS
//                var supportAppleLogin = AppleLoginManager.Instance.IsCurrentPlatformSupported();
//             
//                if(supportAppleLogin) {
//                     view.loginButton.gameObject.SetActive(true);
//                     view.loginTwoTypeButton.gameObject.SetActive(false);
//                }
//                else
//                {
//                     view.loginTwoTypeButton.gameObject.SetActive(true);
//                     view.loginButton.gameObject.SetActive(false);
//                }
// #endif
//                 
//                     
//                 BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventEnterLoginScreen);
//             }
//             else
//             {
//                 EventBus.Dispatch(new EventStartAutoLogin());
//                 
//                 // Log.LogStateEvent(State.EnterLoginScene, new Dictionary<string, object>() {{"AutoLogin:", true}},
//                 //     StepId.ID_ENTER_LOGIN);
//             }
//             
//             //EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOGIN));
        }
        
        private void StartLogin()
        {
            if (!Client.Get<LoginController>().CanAutoLogin())
            {
                view.checkVersionTransform.gameObject.SetActive(false);
                view.loadingRoot.gameObject.SetActive(false);
                view.loginRoot.gameObject.SetActive(true);
#if UNITY_ANDROID
                view.loginTwoTypeButton.gameObject.SetActive(true);
                view.loginButton.gameObject.SetActive(false);
#elif UNITY_IOS
               var supportAppleLogin = AppleLoginManager.Instance.IsCurrentPlatformSupported();
            
               if(supportAppleLogin) {
                    view.loginButton.gameObject.SetActive(true);
                    view.loginTwoTypeButton.gameObject.SetActive(false);
               }
               else
               {
                    view.loginTwoTypeButton.gameObject.SetActive(true);
                    view.loginButton.gameObject.SetActive(false);
               }
#endif
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventEnterLoginScreen);
            }
            else
            {
                view.checkVersionText.text = "Login To Server";
                
                var checkVersionAnimator = view.checkVersionText.GetComponent<Animator>();
                if(checkVersionAnimator)
                    checkVersionAnimator.enabled = false;
 
                for (var i = 0; i < view.checkVersionText.transform.childCount; i++)
                {
                    view.checkVersionText.transform.GetChild(i).gameObject.SetActive(false);
                }
                
                EventBus.Dispatch(new EventStartAutoLogin());
            }
        }


        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventLoginSuccess>(OnLoginSuccess);
            SubscribeEvent<EventSceneSwitchBegin>(OnSceneSwitchBegin);
            SubscribeEvent<EventEnterLobbyServerInfoReady>(OnEnterLobbyServerInfoReady);
        }

        protected void OnEnterLobbyServerInfoReady(EventEnterLobbyServerInfoReady evt)
        {
            _enterLobbyServerInfoReady = true;
        }
        protected void OnSceneSwitchBegin(EventSceneSwitchBegin evt)
        {
            if (evt.targetScene == SceneType.TYPE_LOBBY && evt.currentSceneType == SceneType.TYPE_LOADING)
            {
                _sceneSwitchId = ViewManager.SwitchActionId;
                
                ShowLoadingUI();
                
                LoadCoreAssets();
            }
        }
         
        protected async void OnLoginSuccess(EventLoginSuccess evt)
        {
            ViewManager.Instance.HideScreenLoadingView();
            _enterLobbyServerInfoReady = false;
            
            PerformanceTracker.AddTrackPoint("PrepareActivitiesServerData");
            await Client.Get<ActivityController>().PrepareActivitiesServerData();
            PerformanceTracker.FinishTrackPoint("PrepareActivitiesServerData");
             
            EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOBBY));
        }
        
        protected void ShowLoadingUI()
        {
            view.loginRoot.gameObject.SetActive(false);
            view.loadingRoot.gameObject.SetActive(true);
            view.checkVersionTransform.gameObject.SetActive(false);
            view.loadingText.text = GetLoadingText();
            view.progressBar.value = 0.0f;
        }
        
        //Login Logic
        protected void OnFacebookLoginClicked()
        {
            EventBus.Dispatch(new EventFbLogin());
        }

        protected void OnGuestLoginClicked()
        {
            EventBus.Dispatch(new EventGuestLogin());
        }
        
        protected void OnAppleLoginClicked()
        {
            EventBus.Dispatch(new EventAppleLogin());
        }
        
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            int linkIndex =
                TMP_TextUtilities.FindIntersectingLink(view.policyText, pointerEventData.position,
                    Camera.main); // If you are not in a Canvas using Screen Overlay, put your camera instead of null
            if (linkIndex != -1)
            {
                // was a link clicked?
                TMP_LinkInfo linkInfo = view.policyText.textInfo.linkInfo[linkIndex];
                if (linkInfo.GetLinkID() == "POLICY")
                {
                    Application.OpenURL(ConfigurationController.Instance.PrivacyPolicyURL);
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPrivacyClick);
                }
                else if (linkInfo.GetLinkID() == "SERVICE")
                {
                    Application.OpenURL(GlobalSetting.termsOfServiceURL);
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTermServiceClick);
                }
            }
        }
    }
}