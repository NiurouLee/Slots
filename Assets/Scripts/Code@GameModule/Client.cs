/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-09-30 20:01:45
Ver : 1.0.0
Description : 客户端逻辑代码入口类
ChangeLog :  
**********************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dlugin;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Tool;
using UnityEngine;

namespace GameModule
{
    public class Client
    {
        //本地存储
        public static IStorage Storage { get; set; }
   
        //基础逻辑模块
        private static Dictionary<Type,LogicController> _logicControllers;
      
        private static List<IExternalEventScheduler> _externalEventScheduler;
 
        public Client()
        {
            InitializeExternalEventScheduler();
        }

        private void InitializeExternalEventScheduler()
        {
            _externalEventScheduler = new List<IExternalEventScheduler>();
            _externalEventScheduler.Add(new UpdateScheduler());
            _externalEventScheduler.Add(new ApplicationEventScheduler());
        }

        
        public void RunGame()
        {
            InitializeGlobal();
            RegisterEvent();
            
            //DragonSDK
            PerformanceTracker.AddTrackPoint("InitSDK");
            XDebug.Log("[[ShowOnExceptionHandler]] TrackerPoint: InitSDKStart");
          
            InitSDK();
            
            PerformanceTracker.FinishTrackPoint("InitSDK");
            //  LoginWithDeviceId();
            XDebug.LogOnExceptionHandler("new EventSwitchScene(SceneType.TYPE_LOADING)");
            
            EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_LOADING));
            
            Input.multiTouchEnabled = false;
        }
        
        public static void InitSDK()
        {
            AccountManager.Instance.Init();
            
            SDK.GetInstance().Initialize();
            
            APIConfigLoad.Load();
            //AdController.Instance.Init();
        }
     
        private void RegisterEvent()
        {
            EventBus.Subscribe<EventRequestGameRestart>(OnRequestGameRestart);
        
            EventBus.Subscribe<EventRequestReconnect>(OnRequestReconnect);
            
            EventBus.Subscribe<EventSceneSwitchBegin>(OnSceneSwitchBegin);
            
            EventBus.Subscribe<EventOnApplicationQuit>(OnApplicationQuit);
            
            EventBus.Subscribe<EventGameOnPause>(OnApplicationPause);

            EventBus.Subscribe<EventAdStart>(OnAdRvStart);
            EventBus.Subscribe<EventAdEnd>(OnAdRvEnd);
        }


        private async void OnAdRvStart(EventAdStart eventData)
        {
            //Debug.Log($"===========OnAdRvStart ==AudioListener.pause:{AudioListener.pause}");

            if (ViewManager.Instance.IsPortrait)
            {
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = true;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }

            AudioListener.pause = true;
            
            ViewManager.Instance.BlockingUserClick(true,"OnAdRvStart");
            var startTime = Time.realtimeSinceStartup;
//#if UNITY_IOS
            await XUtility.WaitSeconds(2);

            //resumeGame可能已经被调用了，所以这里再做一个判断，如果已经Resume了就不在做额外处理了
            if (AudioListener.pause)
            {
                UnityEngine.Time.timeScale = 0;
//#endif      
                ViewManager.Instance.SetBlockingUserCallback(() =>
                {
                    ResumeGame();
                    var duration = Time.realtimeSinceStartup - startTime;
                    AdController.Instance.RewardedVideoCallBack(eventData.adPos, true, duration ,"BlockingUserCallback");
                });
            }
            //Debug.Log($"===========OnAdRvStart11 ==AudioListener.pause:{AudioListener.pause}");

        }


        private void OnAdRvEnd(EventAdEnd obj)
        {
            //Debug.Log($"===========OnAdRvEnd ==AudioListener.pause:{AudioListener.pause}");
            ResumeGame();
           
            //Debug.Log($"===========OnAdRvEnd11 ==AudioListener.pause:{AudioListener.pause}");

        }


        protected void ResumeGame()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.orientation = ScreenOrientation.Portrait;
            }
            
            AudioListener.pause = false;
            UnityEngine.Time.timeScale = 1;
            ViewManager.Instance.SetBlockingUserCallback(null);
            ViewManager.Instance.BlockingUserClick(false,"ResumeGame");
        }


        /// <summary>
        /// 进大厅之前，各个logicController 需要从服务器拉取到必须的数据
        /// </summary>
        /// <param name="evt"></param>
        private async void OnSceneSwitchBegin(EventSceneSwitchBegin evt)
        {
            if (evt.targetScene == SceneType.TYPE_LOBBY && evt.currentSceneType == SceneType.TYPE_LOADING)
            {
                PerformanceTracker.AddTrackPoint("EnterLobbyServerRequest");
                
                var keys = _logicControllers.Keys.ToList();
                 
                Client.Get<AdController>().InitAdSDKConfig();
               // await XUtility.WaitSeconds(1);

                APIManagerGameModule.Instance.OnPush<FortunexNotification>(LogicController.OnServerPushNotification);
 
                //---------------------封装大协议，减少进入大厅的时间消耗-------------------------------------------
                CGetInfoBeforeEnterLobby c = new CGetInfoBeforeEnterLobby();
                c.UserGroup = AdController.GetUserGroupId();

                var r = await APIManagerGameModule.Instance.SendAsync<CGetInfoBeforeEnterLobby, SGetInfoBeforeEnterLobby>(c);

                if (r.ErrorCode == ErrorCode.Success)
                {
                    for (var i = 0; i < keys.Count; i++)
                    {
                        _logicControllers[keys[i]].OnGetInfoBeforeEnterLobby(r.Response);
                    }
                }
                //-----------------------------------------------------------------
                
                for (var i = 0; i < keys.Count; i++)
                {
                    await _logicControllers[keys[i]].PrepareModelDataBeforeEnterLobby();
                }
                
                for (var i = 0; i < keys.Count; i++)
                {
                    _logicControllers[keys[i]].Start();
                } 
                
                EventBus.Dispatch(new EventEnterLobbyServerInfoReady());
                EventBus.Dispatch(new EventSceneSwitchMask(SwitchMask.MASK_SERVER_READY, ViewManager.SwitchActionId));
                
                PerformanceTracker.FinishTrackPoint("EnterLobbyServerRequest");
            }
        }

        private void OnRequestGameRestart(EventRequestGameRestart eventRequestGameRestart)
        {
            Get<LoginController>().SetAutoLogin(eventRequestGameRestart.autoLogin);
            //游戏内重启，如果屏幕不是横屏，先将屏幕旋转为横屏
          
            ViewManager.Instance.UpdateViewToLandscape(true);
            
            CleanUp();
            
            PerformanceTracker.ClearAllTrackPoint();
          
            //底包漏加了Track点，在这里补一下           
            PerformanceTracker.AddTrackPoint("ShowSplashView");
            
            Launcher.Restart();
            XDebug.Log("Restart");
        }
        private void OnRequestReconnect(EventRequestReconnect eventRequestReconnect)
        {
            //Do Context Clean Up
            //update 回调
            //资源释放
            //
        }
 
        public void CleanUp()
        {
            APIManagerGameModule.Instance.Clear();
            APIManagerBridge.ClearBridgeRequest();
            
            // await XUtility.WaitNFrame(5);
            
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
            var playerInfo = GameObject.Find("ShowPlayerInfo");
            if (playerInfo != null)
            {
                GameObject.Destroy(playerInfo);
            }
#endif            
            PopupStack.CloseAllPopup();
            ViewManager.Instance.RemoveSceneView();
            ViewManager.Instance.RemoveAllHighPriorityView();
            AssetHelper.CleanUp();
            // PTS.OnDestroy();
 
            DefaultPauseAndCancelContext.Instance.CleanUp();

            IEnumeratorTool.instance.StopAllCoroutines();
            DetachExternalEventSchedulerFromListener();
            CleanUpLogicController();
            EventBus.Reset();
            SDK.GetInstance().iapManager.ResetIapState();
            
            AdConfigManager.Instance.Release();
        }


        public void OnApplicationQuit(EventOnApplicationQuit evt)
        {
            SDK.GetInstance().Clear();
            CleanUp();
        }
        
        public void OnApplicationPause(EventGameOnPause evt)
        {
            AdController.Instance.RecordBackgroundTime();
            
            Debug.Log("[OnApplication] Pause");
        }

        public void AttachExternalEventSchedulerToListener()
        {
            for (int i = 0; i < _externalEventScheduler.Count; i++)
            {
                _externalEventScheduler[i].AttachToListener();
            }
        }

        public void DetachExternalEventSchedulerFromListener()
        {
            for (int i = 0; i < _externalEventScheduler.Count; i++)
            {
                _externalEventScheduler[i].DetachFromListener();
            }
        }

        public void InitializeFundamental()
        {
            Storage = new Storage();
            // PTS.InitThirdSupport();
            // PTS.OnAfterLaunch();
        }

        public void InitializeGlobal()
        {
            InitializeFundamental();
            RegisterEssentialLogicController();
            AttachExternalEventSchedulerToListener();
        }
        private void RegisterEssentialLogicController()
        {
            _logicControllers = new Dictionary<Type, LogicController>();

            var allTypes = GameModuleLaunch.GetAllType();
            
            XDebug.Log("TypeCount:" + allTypes.Count);
            
            for (var i = 0; i < allTypes.Count; i++)
            {
                if (allTypes[i].IsSubclassOf(typeof(LogicController)))
                {
                    Activator.CreateInstance(allTypes[i],this);
                }
            }
        }

        public void RegisterLogicController(LogicController logicController)
        {
            _logicControllers.Add(logicController.GetType(), logicController);
        }

        public void CleanUpLogicController()
        {
            foreach (var item in _logicControllers)
            {
                item.Value.CleanUp();
            }
            _logicControllers.Clear();
        }

        public void RemoveLogicController(LogicController logicController)
        {
            if (!_logicControllers.ContainsKey(logicController.GetType()))
            {
                return;
            }

            _logicControllers.Remove(logicController.GetType());
        }

        public static T Get<T>() where T : LogicController
        {
            if (_logicControllers.ContainsKey(typeof(T)))
            {
                return _logicControllers[typeof(T)] as T;
            }
            
            return null;
        }
    }
}
