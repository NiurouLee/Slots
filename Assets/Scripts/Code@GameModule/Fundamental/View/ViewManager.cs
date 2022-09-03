// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/20/19:47
// Ver : 1.0.0
// Description : ViewManager.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;

namespace GameModule
{
    public class ViewManager : LogicController
    {
        private static ViewManager _instance;
        public static ViewManager Instance => _instance;

        [ComponentBinder("HighPriorityUIContainerCanvas")]
        protected Transform highPriorityUIContainerCanvas;

        protected Transform blockingUserClickUI;

        [ComponentBinder("PopupContainerCanvas")]
        protected Transform popupContainerCanvas;

        [ComponentBinder("SceneContainer")] protected Transform sceneTransform;

        [ComponentBinder("SceneContainer/3DScene")]
        protected Transform scene3DContainerTransform;

        [ComponentBinder("SceneContainer/CanvasScene")]
        protected Transform sceneCanvasContainerTransform;

        protected ISceneView sceneView;

        protected Dictionary<Type, View> highPriorityViewDict;
        protected List<Type> loadingHighPriorityViewList;

        public bool IsValid { get; private set; }

        /// <summary>
        /// 当前是否是竖屏显示
        /// </summary>
        public bool IsPortrait { get; private set; }

        private Dictionary<SceneType, SceneSwitchAction> _switchActions;

        private static bool _inSwitching;

        public static SceneType ActiveSceneType { get; private set; }

        public static SceneType LastSceneType { get; private set; }

        public static SceneType SwitchingSceneType { get; private set; }

        private string _switchViewAddress;

        public bool isLoadingViewShowing;

        public static int SwitchActionId { get; private set; }

        public EventSwitchScene LastEventSwitchScene { get; private set; }
        public EventSwitchScene CurrentEventSwitchScene { get; private set; }

        public ViewManager(Client client)
            : base(client)
        {
            ComponentBinder.BindingComponent(this, GameObject.Find("Launcher").transform);

            highPriorityViewDict = new Dictionary<Type, View>();
            loadingHighPriorityViewList = new List<Type>();

            UpdateViewToLandscape(true);

#if !PRODUCTION_PACKAGE

            if (Camera.main != null && Camera.main.GetComponent<FPSDisplay>() == null)
            {
                Camera.main.gameObject.AddComponent<FPSDisplay>();
            }
#endif
            _instance = this;

            //重启之后，解除玩家点击锁定状态
            BlockingUserClick(false, "Init");
            XUtility.ShowBlockUserOperationMask(false);
            
            IsValid = true;
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventSwitchScene>(OnSwitchScene);
            SubscribeEvent<EventSceneSwitchMask>(OnSceneSwitchMask);
            SubscribeEvent<EventBackToLastScene>(OnBackToLastScene);
        }

        public void OnSceneSwitchMask(EventSceneSwitchMask maskEvent)
        {
            if (_inSwitching)
            {
                if (maskEvent.switchActionId != SwitchActionId)
                    return;
                //离开当前ActiveSceneType
                _switchActions[ActiveSceneType].OnSceneSwitchMask(maskEvent.mask);

                //进入switchingSceneType
                /*必须加这个判断，如果this.currentSceneType 和 this.switchingSceneType是同一个type，的时候，上面的调用可能导致场景切换完成了，下面this.switchingSceneType变成了undefined*/
                if (SwitchingSceneType != SceneType.TYPE_INVALID)
                {
                    _switchActions[SwitchingSceneType].OnSceneSwitchMask(maskEvent.mask);
                }
            }
        }

        public void OnSwitchScene(EventSwitchScene eventSwitchScene)
        {
            if (eventSwitchScene.sceneType != SceneType.TYPE_INVALID)
            {
                LastEventSwitchScene = CurrentEventSwitchScene;
                CurrentEventSwitchScene = eventSwitchScene;

                SwitchToScene(eventSwitchScene);
            }
        }

        public void OnBackToLastScene(EventBackToLastScene eventBackToLastScene)
        {
            XDebug.Log("EventBackToLastScene" + LastEventSwitchScene.sceneType);
            if (LastEventSwitchScene.sceneType == SceneType.TYPE_INVALID)
            {
                CurrentEventSwitchScene = LastEventSwitchScene;
                var switchEvent = new EventSwitchScene(SceneType.TYPE_LOBBY);
                switchEvent.isBackToLastScene = true;
                OnSwitchScene(switchEvent);
            }
            else
            {
                if (CurrentEventSwitchScene.enterType != "Quest")
                {
                    CurrentEventSwitchScene = LastEventSwitchScene;
                    var switchEvent = LastEventSwitchScene;
                    LastEventSwitchScene = new EventSwitchScene(SceneType.TYPE_INVALID);
                    switchEvent.isBackToLastScene = true;
                    SwitchToScene(switchEvent);
                }
                else
                {
                    RemoveHighPriorityView<SceneSwitchView>();
                    RemoveHighPriorityView<SceneSwitchView2>();
                    EventBus.Dispatch(new EventSceneSwitchBackToQuest());
                }
            }
        }

        protected override void Initialization()
        {
            base.Initialization();

            ActiveSceneType = SceneType.TYPE_LAUNCH;
            _switchActions = new Dictionary<SceneType, SceneSwitchAction>();

            _switchActions[SceneType.TYPE_LAUNCH] = new LaunchSceneSwitchAction();
            _switchActions[SceneType.TYPE_LOADING] = new LoadingSceneSwitchAction();
     //       _switchActions[SceneType.TYPE_LOGIN] = new LoginSceneSwitchAction();
            _switchActions[SceneType.TYPE_LOBBY] = new LobbySceneSwitchAction();
            _switchActions[SceneType.TYPE_MACHINE] = new MachineSceneSwitchAction();

            isLoadingViewShowing = false;

            SwitchActionId = 0;
        }

        public async void SwitchToScene(EventSwitchScene eventSwitchScene)
        {
            var targetSceneType = eventSwitchScene.sceneType;

            XDebug.Log("SwitchToScene: " + targetSceneType);

            if (targetSceneType != SceneType.TYPE_INVALID && _switchActions.ContainsKey(targetSceneType) &&
                !_inSwitching)
            {
                //  Log.LogBenchmark("SceneSwitchStart:" + targetSceneType);

                _inSwitching = true;

                var fromSwitchAction = _switchActions[ActiveSceneType];
                var toSwitchAction = _switchActions[targetSceneType];

                fromSwitchAction.SetUpSwitchEventData(eventSwitchScene);
                toSwitchAction.SetUpSwitchEventData(eventSwitchScene);

                SwitchActionId++;

                _switchViewAddress = _switchActions[targetSceneType].GetTargetSceneSwitchViewAddress(ActiveSceneType);
                fromSwitchAction.SwitchActionId = SwitchActionId;
                toSwitchAction.SwitchActionId = SwitchActionId;

                SwitchingSceneType = targetSceneType;

                //横竖屏切换, 移至实际切换时
                bool isPortrait = _switchActions[targetSceneType].IsPortraitScene();

                if (!string.IsNullOrEmpty(_switchViewAddress))
                {
                    SceneSwitchView view = null;
                    SceneSwitchView lastSceneSwitchView = null;

                    if (!HasHighPriorityView<SceneSwitchView2>())
                    {
                        lastSceneSwitchView = GetHighPriorityView<SceneSwitchView>();
                        view = await ShowSceneSwitchView<SceneSwitchView2>(_switchViewAddress,
                            isPortrait,lastSceneSwitchView == null);
                    }
                    else
                    {
                        lastSceneSwitchView = GetHighPriorityView<SceneSwitchView2>();
                        
                        view = await ShowSceneSwitchView<SceneSwitchView>(_switchViewAddress,
                            isPortrait, lastSceneSwitchView == null);
                    }
                    
                    view.Hide();
                    
                    await UpdateScreenOrientation(isPortrait);

                    if (lastSceneSwitchView != null)
                    {
                        RemoveHighPriorityView(lastSceneSwitchView.GetType());
                    }

                    LayoutRebuilder.ForceRebuildLayoutImmediate(view.rectTransform);
                    view.Show();

                    EventBus.Dispatch(new EventSceneSwitchBegin(ActiveSceneType, targetSceneType));

                    view.viewController.BindingSwitchAction(fromSwitchAction, toSwitchAction);
                }
                else
                {
                    if (toSwitchAction.ShowLoadingScreenView(ActiveSceneType))
                    {
                        await ShowScreenLoadingView();
                        isLoadingViewShowing = true;
                    }

                    await UpdateScreenOrientation(isPortrait);

                    EventBus.Dispatch(new EventSceneSwitchBegin(ActiveSceneType, targetSceneType));

                    fromSwitchAction.LeaveScene(() =>
                    {
                        WaitNFrame(1, ()=>
                        {
                            toSwitchAction.EnterScene();
                        });
                    });
                }
            }
        }

        /// <summary>
        /// 场景切换完成
        /// </summary>
        public void OnSwitchSceneEnd(bool isAbort = false, bool isBackToLastScene = false)
        {
            if (string.IsNullOrEmpty(_switchViewAddress))
            {
                isLoadingViewShowing = false;
                HideScreenLoadingView();
            }
            else if (!isAbort)
            {
                RemoveHighPriorityView<SceneSwitchView>();
                RemoveHighPriorityView<SceneSwitchView2>();
            }

            LastSceneType = ActiveSceneType;

            ActiveSceneType = SwitchingSceneType;
            _inSwitching = false;
            SwitchingSceneType = SceneType.TYPE_INVALID;

            if (!isAbort)
            {
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                if (UIDebugger.isSilent && ActiveSceneType == SceneType.TYPE_LOBBY)
                {
                    EventBus.Dispatch(new EventSilentSceneSwitchEnd(ActiveSceneType, LastSceneType, isBackToLastScene),
                        () => { XDebug.Log("SceneSwitchEndLoicHandleEnd"); });
                    return;
                }
#endif
                EventBus.Dispatch(new EventSceneSwitchEnd(ActiveSceneType, LastSceneType, isBackToLastScene),
                    () => { XDebug.Log("SceneSwitchEndLoicHandleEnd"); });

                if (LastSceneType == SceneType.TYPE_LOADING && ActiveSceneType == SceneType.TYPE_LOBBY)
                {
                    PerformanceTracker.FinishTrackPoint("SwitchToLobbyScene");

                    var splashPoint = PerformanceTracker.GetTrackPoint("ShowSplashView");

                    if (splashPoint != null)
                    {
                        var trackPoint = PerformanceTracker.AddTrackPoint("EnterGame");

                        trackPoint.startTime = splashPoint.startTime;

                        PerformanceTracker.FinishTrackPoint("EnterGame");
                    }
                }
            }
        }

        public async Task UpdateViewToLandscape(bool force = false)
        {
            if (IsPortrait || force)
            {
                Screen.orientation = ScreenOrientation.Landscape;
                Screen.autorotateToLandscapeLeft = true;
                Screen.autorotateToLandscapeRight = true;
                Screen.autorotateToPortrait = false;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.orientation = ScreenOrientation.AutoRotation;
                
#if UNITY_ANDROID
                await XUtility.WaitNFrame(1, this);
#endif
                
#if UNITY_IOS
                UnityEngine.iOS.Device.hideHomeButton = true;
#endif
                var camera = Camera.main;
                if (camera != null)
                    camera.transform.localPosition = ViewResolution.GetCameraPositionByViewResolution(camera, false);

                var launcher = GameObject.Find("Launcher");
                var canvasScalerArray = launcher.GetComponentsInChildren<CanvasScaler>(true);
                for (var i = 0; i < canvasScalerArray.Length; i++)
                {
                    canvasScalerArray[i].referenceResolution = ViewResolution.referenceResolutionLandscape;
                }

                IsPortrait = false;

#if UNITY_EDITOR
                GameViewUtils.SwitchOrientation(false);
#endif
            }
        }

        /// <summary>
        /// 调整朝向到竖屏
        /// </summary>
        public async Task UpdateViewToPortrait()
        {
            if (!IsPortrait)
            {
                IsPortrait = true;
                Screen.orientation = ScreenOrientation.Portrait;
#if UNITY_ANDROID
                await XUtility.WaitNFrame(1, this);
#endif
                
#if UNITY_IOS
                UnityEngine.iOS.Device.hideHomeButton = false;
#endif
                
                var camera = Camera.main;
                if (camera != null)
                    camera.transform.localPosition = ViewResolution.GetCameraPositionByViewResolution(camera, true);

                var global = GameObject.Find("Launcher");

                var canvasScalerArray = global.GetComponentsInChildren<CanvasScaler>(true);

                for (var i = 0; i < canvasScalerArray.Length; i++)
                {
                    canvasScalerArray[i].referenceResolution = ViewResolution.referenceResolutionPortrait;
                }

#if UNITY_EDITOR
                GameViewUtils.SwitchOrientation(true);
#endif
            }
        }

        public async Task UpdateScreenOrientation(bool isPortrait = false, Action updateComplete = null)
        {
            if (isPortrait)
            { 
                await UpdateViewToPortrait();
            }
            else
            {
               await UpdateViewToLandscape();
            }
        }

        public void UpdateHighPriorityCanvasReferenceResolution(bool isPortrait)
        {
            var canvasScaler = highPriorityUIContainerCanvas.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = isPortrait
                ? ViewResolution.referenceResolutionPortrait
                : ViewResolution.referenceResolutionLandscape;
        }

        public async Task ShowScreenLoadingView()
        {
            var view = GetHighPriorityView<ScreenLoadingView>();
            if (view != null)
            {
                view.Show();
            }
            else
            {
                if(!HasHighPriorityView<ScreenLoadingView>())
                    await ShowHighPriorityView<ScreenLoadingView>("ScreenLoading");
            }
        }

        public async Task DelayShowScreenLoadingView()
        {
            var view = GetHighPriorityView<ScreenLoadingView>();
            if (view != null)
            {
                view.DelayShow();
            }
            else
            {
                if (!HasHighPriorityView<ScreenLoadingView>())
                {
                    view = await ShowHighPriorityView<ScreenLoadingView>("ScreenLoading");
                    view.DelayShow();
                }
            }
        }

        public void HideScreenLoadingView()
        {
            var view = GetHighPriorityView<ScreenLoadingView>();
            if (view != null)
            {
                view.Hide();
            }
        }

        public async Task<TView> ShowHighPriorityView<TView>(string address)
            where TView : View
        {
            var view = await View.CreateView<TView>(address, highPriorityUIContainerCanvas);

            if (view != null)
            {
                highPriorityViewDict.Add(typeof(TView), view);
            }

            return view;
        }
        
        public async Task<TView> ShowSceneSwitchView<TView>(string address, bool isPortrait,bool updateReferenceResolution = true)
            where TView : View
        {
            if (updateReferenceResolution)
            {
                UpdateHighPriorityCanvasReferenceResolution(isPortrait);
            }

            var start = Time.realtimeSinceStartup;
            XDebug.LogOnExceptionHandler("ShowSceneSwitchView: " + start);
            var view = await ShowHighPriorityView<TView>(address, isPortrait);
            XDebug.LogOnExceptionHandler("ShowSceneSwitchView: Cost:" + (Time.realtimeSinceStartup - start));
            return view;
        }

        public async Task<TView> ShowHighPriorityView<TView>(string address, object extraData)
            where TView : View
        {
            loadingHighPriorityViewList.Add(typeof(TView));
            
            var view = await View.CreateView<TView>(address, highPriorityUIContainerCanvas, extraData);

            if (HasHighPriorityView<TView>())
            {
                RemoveHighPriorityView<TView>();
            }
            
            loadingHighPriorityViewList.Remove(typeof(TView));
            
            if(view != null)
                highPriorityViewDict.Add(typeof(TView), view);

            return view;
        }

        public bool HasHighPriorityView<TView>()
        {
            if (highPriorityViewDict != null
                && highPriorityViewDict.ContainsKey(typeof(TView)))
            {
                return true;
            }
            
            if (loadingHighPriorityViewList != null
                && loadingHighPriorityViewList.Contains(typeof(TView)))
            {
                return true;
            }

            return false;
        }

        public TView GetHighPriorityView<TView>() where TView : View
        {
            if (highPriorityViewDict != null
                && highPriorityViewDict.ContainsKey(typeof(TView)))
            {
                return highPriorityViewDict[typeof(TView)] as TView;
            }

            return null;
        }

        public bool RemoveHighPriorityView<TView>()
        {
            return RemoveHighPriorityView(typeof(TView));
        }
        
        public bool RemoveHighPriorityView(Type type)
        {
            if (highPriorityViewDict != null
                && highPriorityViewDict.ContainsKey(type))
            {
                var view = highPriorityViewDict[type];
                highPriorityViewDict.Remove(type);
                view.Destroy();
                return true;
            }

            return false;
        }

        public void RemoveAllHighPriorityView()
        {
            if (highPriorityViewDict.Count > 0)
            {
                foreach (var item in highPriorityViewDict)
                {
                    item.Value.Destroy();
                }

                highPriorityViewDict.Clear();
            }
        }

        public void ShowSceneView(ISceneView nextSceneView, bool is3DScene)
        {
            nextSceneView.AttachToSceneContainer(is3DScene ? scene3DContainerTransform : sceneCanvasContainerTransform);

            if (sceneView != null)
            {
                sceneView.DestroySceneView();
            }

            sceneView = nextSceneView;
        }

        public void RemoveSceneView()
        {
            if (sceneView != null)
            {
                sceneView.DestroySceneView();
                sceneView = null;
            }
        }

        public async Task<TView> ShowSceneView<TView>(string address, bool is3DScene)
            where TView : View, ISceneView
        {
            var view = await View.CreateView<TView>(address,
                is3DScene ? scene3DContainerTransform : sceneCanvasContainerTransform);

            if (view != null)
            {
                if (sceneView != null)
                {
                    sceneView.DestroySceneView();
                }

                sceneView = view;
            }

            return view;
        }
        
        public async Task<TView> BindingSceneView<TView>(Transform transform, bool is3DScene)
            where TView : View, ISceneView
        {
            var view = View.CreateView<TView>(transform);

            if (view != null)
            {
                if (sceneView != null)
                {
                    sceneView.DestroySceneView();
                }

                sceneView = view;
            }

            return view;
        }

        public T GetSceneView<T>() where T : View
        {
            return sceneView as T;
        }

        public bool InLobbyScene()
        {
            return sceneView != null && sceneView.SceneType == SceneType.TYPE_LOBBY;
        }

        public bool InMachineScene()
        {
            return sceneView != null && sceneView.SceneType == SceneType.TYPE_MACHINE;
        }

        public bool IsInSwitching()
        {
            return _inSwitching;
        }
        public bool IsPortraitScene()
        {
            return sceneView.IsPortraitScene;
        }

        public override void CleanUp()
        {
            IsValid = false;
        }


        public void SetBlockingUserCallback(Action callback)
        {
            if (blockingUserClickUI != null)
            {
                var pointerEventCustomHandler = blockingUserClickUI.GetComponent<PointerEventCustomHandler>();
                pointerEventCustomHandler.BindingPointerClick((data) => { callback?.Invoke();});
            }
        }


        public void BlockingUserClick(bool disable, string source)
        {
            if (blockingUserClickUI == null)
            {
                blockingUserClickUI = highPriorityUIContainerCanvas.Find("BlockClickUI");
                if (blockingUserClickUI == null)
                {
                    var gameObject = new GameObject("BlockClickUI");
                    gameObject.transform.SetParent(highPriorityUIContainerCanvas, false);
                    var rectTransform = gameObject.AddComponent<RectTransform>();
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    var image = gameObject.AddComponent<Image>();
                    image.color = new Color(0, 0, 0, 0);
                    blockingUserClickUI = gameObject.transform;
                    blockingUserClickUI.gameObject.AddComponent<PointerEventCustomHandler>();
                }

                blockingUserClickUI.SetAsLastSibling();
            }
             
            XDebug.Log($"[[ShowOnExceptionHandler]] BlockingUserClick:state:{disable}/source:{source}");
            blockingUserClickUI.gameObject.SetActive(disable);
        }

        public bool IsUserClickBlocked()
        {
            if(blockingUserClickUI != null)
                return blockingUserClickUI.gameObject.activeInHierarchy;
            return false;
        }
    }
}