// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 8:45 PM
// Ver : 1.0.0
// Description : MachineContext.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace GameModule
{
    public class MachineContext : DefaultPauseAndCancelContext, IFlowControlContext
    {
        private LogicStep runningLogicStep;

        public MachineConfig machineConfig;

        public MachineState state;
        public MachineView view;
        public MachineFeature feature;

        #region WaitEvent

        private List<WaitEvent> waitEvents;

        public MachineContextBuilder contextBuilder;
        public MachineAssetProvider assetProvider;
        public IMachineServiceProvider serviceProvider;
        public ISequenceElementConstructor sequenceElementConstructor;
        public MachineClassProvider classProvider;
        public IElementExtraInfoProvider elementExtraInfoProvider;
         
        public Transform transform;

        private FlowController flowControl;

        public Transform MachineUICanvasTransform;
        public Transform MachinePopUpCanvasTransform;

        private MonoUpdateProxy _proxy;

        private List<IOnContextDestroy> _destroyEventObservers;

        public MachineContext(Transform inTransform)
        {
            transform = inTransform.Find("MachineContext");
            MachineUICanvasTransform = inTransform.Find("UICanvas");
            MachinePopUpCanvasTransform = inTransform.Find("PopUpCanvas");
            _proxy = transform.gameObject.AddComponent<MonoUpdateProxy>();
        }

        public async Task InitializeContext(MachineContextBuilder inContextBuilder, MachineAssetProvider inAssetProvider,
            IMachineServiceProvider inServiceProvider)
        {
            contextBuilder = inContextBuilder;
            assetProvider = inAssetProvider;
            serviceProvider = inServiceProvider;
 
            waitEvents = new List<WaitEvent>();
            
            _destroyEventObservers = new List<IOnContextDestroy>();

            classProvider = new MachineClassProvider();
            SetUpUtility();
            
            //state
            SetUpMachineState();

            //view
            SetUpMachineView();
            
            AddMachineFeature();

            await AttachAsyncView();
            
            //MachineUIAdapt
            AdaptMachineView();

            //logic block
            InstantiateLogicProxy();

            //binding
            BuildLogicControlFlow();

            BindingContext();

            StarLogicFlow();
        }

        public async Task AttachAsyncView()
        {
           await contextBuilder.AttachSystemWidget(this);
        }

        private void BindingContext()
        {
            PopUpManager.Instance.BindingContext(this);
            AudioUtil.BindingContext(this);
        }

        private void SetUpUtility()
        {
            //关卡特殊图标的一些额外配置，通过代码来实现
            elementExtraInfoProvider = contextBuilder.GetElementExtraInfoProvider();
            //
            sequenceElementConstructor = contextBuilder.GetSequenceElementConstructor(this);
        }

        private void SetUpMachineState()
        {
            machineConfig = contextBuilder.SetUpMachineConfig(this);

            state = new MachineState(this, machineConfig);

            contextBuilder.SetUpCommonMachineState(state);
            contextBuilder.SetUpMachineState(state);
        }

        private void SetUpMachineView()
        {
            view = new MachineView(this);

            contextBuilder.BindingMachineView(this);
            contextBuilder.AttachMachineView(this);
        }
        
        private void AdaptMachineView()
        {
            contextBuilder.AdaptMachineView(this);
        }
        
        private void AddMachineFeature()
        {
            feature = new MachineFeature(this);
            contextBuilder.AddMachineFeatures(feature);
        }
 
        /// <summary>
        /// 添加一个WaitEvent到当前Context，让关注该WaitEvent的LogicStep，需要等待WaitEvent完成之后才能开始自己的逻辑
        /// </summary>
        /// <param name="waitEvent"></param>
        public void AddWaitEvent(WaitEvent waitEvent)
        {
            if (!waitEvents.Contains(waitEvent))
                waitEvents.Add(waitEvent);
        }

        /// <summary>
        /// 清掉所有的WaitEvent
        /// </summary>
        public void ClearAllWaitEvent()
        {
            waitEvents.Clear();
        }

        /// <summary>
        ///查询List中的WaitEvents 是否有没有完成的
        /// </summary>
        /// <param name="inWaitEvent"></param>
        /// <returns></returns>
        public bool HasWaitEvent(WaitEvent inWaitEvent)
        {
            return waitEvents.Contains(inWaitEvent);
        }
        
        /// <summary>
        ///查询List中的WaitEvents 是否有没有完成的
        /// </summary>
        /// <param name="inWaitEvents"></param>
        /// <returns></returns>
        public bool HasWaitEvent(List<WaitEvent> inWaitEvents)
        {
            if (inWaitEvents == null || inWaitEvents.Count == 0)
                return false;

            for (var i = 0; i < inWaitEvents.Count; i++)
            {
                if (waitEvents.Contains(inWaitEvents[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 当WaitEvent完成时候，要通知当前RunningLogicStep
        /// </summary>
        /// <param name="waitEvent"></param>
        public void RemoveWaitEvent(WaitEvent waitEvent)
        {
            if (waitEvents.Contains(waitEvent))
            {
                waitEvents.Remove(waitEvent);
                runningLogicStep?.OnMachineInternalEvent(MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE, waitEvent);
            }
        }

        public void DispatchInternalEvent(MachineInternalEvent machineInternalEvent, params object[] args)
        {
            runningLogicStep?.OnMachineInternalEvent(machineInternalEvent, args);
        }

        #endregion

        #region FlowControl
 
        private Dictionary<LogicStepType, LogicStepProxy> logicStepProxiesDict;
        
        public float MachineScaleFactor = 1.0f;

        /// <summary>
        /// 实例化LogicStep
        /// </summary>
        private void InstantiateLogicProxy()
        {
            logicStepProxiesDict = contextBuilder.InstantiateLogicProxy(this);
        }

        /// <summary>
        /// 创建LogicStep控制器
        /// </summary>
        private void BuildLogicControlFlow()
        {
            flowControl = contextBuilder.BuildLogicStepFlow(this);
        }
        
        private void StarLogicFlow()
        {
            flowControl.SetUp();
            flowControl.EnterFlow(LogicStepType.STEP_START);
        }
        
        //------------------------------------------
        
        public LogicStep GetRunningStep()
        {
            return runningLogicStep;
        }

        public void SetStepToRunning(LogicStep step)
        {
            runningLogicStep = step;
        }

        /// <summary>
        /// 处理完了当前LogicStep的所有逻辑，进入预先定义好的下一个流程
        /// </summary>
        public void Proceed()
        {
            flowControl.Proceed();
        }

        public ILogicStepProxy GetLogicStepProxy(LogicStepType stepType)
        {
            if (logicStepProxiesDict.ContainsKey(stepType))
                return logicStepProxiesDict[stepType];
            return null;
        }

        /// <summary>
        /// 跳转到特定的LogicStep
        /// </summary>
        /// <param name="logicStepType">目的LogicStep</param>
        /// <param name="preLogicStepType">从什么Step跳转</param>
        public void JumpToLogicStep(LogicStepType logicStepType, LogicStepType preLogicStepType)
        {
            flowControl?.JumpToLogicStep(logicStepType, preLogicStepType);
        }

        #endregion

        /// <summary>
        /// Content Logic Update
        /// </summary>
        public void Update()
        {
            if (IsPaused)
                return;
            
            runningLogicStep?.LogicUpdate();
            
            this.view?.Update();
        }
 
        public async Task WaitSeconds(float seconds)
        {
            await XUtility.WaitSeconds(seconds, this);
        }

        public async Task WaitNFrame(int frame)
        {
            await XUtility.WaitNFrame(frame, this);
        }
        
        public void WaitNFrame(int frame, Action callback)
        {
            XUtility.WaitNFrame(frame, callback, this);
        }

        public Coroutine WaitSeconds(float seconds, Action callback)
        {
            return StartCoroutine(DelayCall(seconds, callback));
        }

        protected IEnumerator DelayCall(float seconds, Action callback)
        {
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                while (IsPaused)
                {
                    yield return null;
                }
                
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            callback.Invoke();
        }

        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
           return _proxy.StartCoroutine(coroutine);
        }
        
        
        public void StopCoroutine(Coroutine coroutine)
        {
            _proxy.StopCoroutine(coroutine);
        }
        
        public void StopCoroutine(IEnumerator coroutine)
        {
            _proxy.StopCoroutine(coroutine);
        }
        
        /// <summary>
        /// 老虎机暂停需要恢复的Tween List
        /// </summary>
        private List<Tween> _pausedTweenList = null;
        public void PauseMachine()
        {
            if (IsPaused)
            {
                return;
            }
            
            IsPaused = true;
            
            var animators = transform.GetComponentsInChildren<Animator>();
            var audioSources = transform.GetComponentsInChildren<AudioSource>();
            var particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            var skeletonAnimations = transform.GetComponentsInChildren<SkeletonAnimation>(true);

            for (var i = 0; i < animators.Length; i++)
            {
                animators[i].speed = 0;
            }
   
            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Pause();
            }
            
            for (var i = 0; i < skeletonAnimations.Length; i++)
            {
                skeletonAnimations[i].timeScale = 0;
            }
            
            AudioUtil.Instance.PauseMusic();
         
            for (var i = 0; i < audioSources.Length; i++)
            {
               // audioSources[i].volume = 0;
                audioSources[i].Pause();
            }
            
            runningLogicStep.OnMachineInternalEvent(MachineInternalEvent.EVENT_MACHINE_PAUSED);
           
            if (_pausedTweenList == null)
                _pausedTweenList = new List<Tween>();
            
            _pausedTweenList.Clear();
            
            var tweenList = DOTween.PlayingTweens();
           
            if (tweenList != null && tweenList.Count > 0)
            {
                for (var i = 0; i < tweenList.Count; i++)
                {
                    if (tweenList[i].target is Component component)
                    {
                        if (component.transform.IsChildOf(transform))
                        {
                            if (!component.transform.IsChildOf(view.Get<ControlPanel>().transform))
                            {
                                if (!TweenCanNotPause.HasTween(tweenList[i]))
                                {
                                    tweenList[i].TogglePause();
                                    _pausedTweenList.Add(tweenList[i]);
                                }
                            }
                        }
                    }
                }
            }
            
            //view.Get<ControlPanel>().OnContextPause();
        }
        
        public void UnPauseMachine()
        {
            var animators = transform.GetComponentsInChildren<Animator>();

            for (var i = 0; i < animators.Length; i++)
            {
                animators[i].speed = 1;
            }
            
            var particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
           
            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }
            
            var audioSources = transform.GetComponentsInChildren<AudioSource>();
            for (var i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].UnPause();
            }
            
            var skeletonAnimations = transform.GetComponentsInChildren<SkeletonAnimation>(true);
            
            for (var i = 0; i < skeletonAnimations.Length; i++)
            {
                skeletonAnimations[i].timeScale = 1;
            }
             
            if (PopUpManager.Instance.GetDialogCount() == 0)
            {
                AudioUtil.Instance.UnPauseMusic();
            }
            
            IsPaused = false;

            runningLogicStep.OnMachineInternalEvent(MachineInternalEvent.EVENT_MACHINE_RESUMED);;
             
            if (_pausedTweenList != null && _pausedTweenList.Count > 0)
            {
                for (var i = 0; i < _pausedTweenList.Count; i++)
                {
                    _pausedTweenList[i].TogglePause();
                }
            }
            
           // view.Get<ControlPanel>().OnContextUnPause();
        }
 
        //订阅MachineContext Destroy 事件
        public void SubscribeDestroyEvent(IOnContextDestroy observer)
        {
            if(observer != null && !_destroyEventObservers.Contains(observer))
                _destroyEventObservers.Add(observer);
        }
      
        //取消订阅MachineContext Destroy 事件
        public void UnSubscribeDestroyEvent(IOnContextDestroy observer)
        {
            if(_destroyEventObservers.Contains(observer))
                _destroyEventObservers.Remove(observer);
        }
        
        public void OnMachineDestroy()
        {
            if(_proxy)
                _proxy.StopAllCoroutines();
        
            PopUpManager.Instance.OnDestroy();
            AudioUtil.Instance.OnDestroy();

            for (int i = 0; i < _destroyEventObservers.Count; i++)
            {
                _destroyEventObservers[i].OnContextDestroy();
            }
            
            _destroyEventObservers.Clear();
            
            foreach (var handler in logicStepProxiesDict)
            {
                handler.Value.OnDestroy();
            }
            
            view.OnDestroy();
           
            assetProvider.ReleaseAssets();
 
            CleanUp();
        }
        
        public void RevertMachineNodeSize(Transform transform)
        {
            if (!transform) return;
            var oldLocalScale = transform.localScale;
            oldLocalScale.x /= MachineScaleFactor;
            oldLocalScale.y /= MachineScaleFactor;
            transform.localScale = oldLocalScale;
        }
    }
}