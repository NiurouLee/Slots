// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/19:44
// Ver : 1.0.0
// Description : Controller.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

namespace GameModule
{
    /// <summary>
    /// 提供一些基础功能，方便统一管理，后续实现基础逻辑
    /// 包括Update功能，订阅事件功能，播放动画功能，延迟N秒，延迟N帧
    /// </summary>
    public class Controller: DefaultPauseAndCancelContext, IUpdateable
    {
        protected bool updateEnabled;

        protected List<EventBus.Listener> listeners;
 
        public Controller()
        {
        }

        /// <summary>
        /// 激活Update调用 
        /// </summary>
        /// <param name="frequency">0 为每帧调用，1为每秒调用，2为半秒调用</param>
        protected void EnableUpdate(int frequency = 0)
        {
            if (!updateEnabled)
            {
                switch(frequency)
                {
                    case 0:
                        UpdateScheduler.HookUpdate(this);
                        break;
                    case 1:
                        UpdateScheduler.HookSecondUpdate(this);
                        break;
                    case 2:
                        UpdateScheduler.HookHalfSecondUpdate(this);
                        break;
                }
                
                updateEnabled = true;
            }
        }
        /// <summary>
        /// 停止Update调用
        /// </summary>
        protected void DisableUpdate()
        {
            if (updateEnabled)
            {
                UpdateScheduler.UnhookUpdate(this);
                updateEnabled = false;
            }
        }

        /// <summary>
        /// 逻辑更新
        /// </summary>
        public virtual void Update()
        {
            
        }
        
        /// <summary>
        /// 提供一个空的虚接口统一订阅游戏内的事件
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            
        }
        
        /// <summary>
        /// 订阅游戏内事件
        /// </summary>
        /// <param name="handleAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool SubscribeEvent<T>(Action<T> handleAction) where T:IEvent
        {
            if (listeners == null)
            {
                listeners = new List<EventBus.Listener>();
            }
            
            var listener = EventBus.Subscribe<T>(handleAction);
            
            if (listener != null)
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 订阅游戏内有先后优先级的事件
        /// </summary>
        /// <param name="handleAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool SubscribeEvent<T>(Action<Action, T, IEventHandlerScheduler> handleAction, int priority) where T : IEvent
        {
            if (listeners == null)
            {
                listeners = new List<EventBus.Listener>();
            }
            
            var listener = EventBus.Subscribe<T>(handleAction, priority);
            
            if (listener != null)
            {
                listeners.Add(listener);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="handleAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool UnsubscribeEvent<T>(Action<Action, T, IEventHandlerScheduler> handleAction) where T : IEvent
        {
            if (listeners == null)
                return false;

            for (var i = 0; i < listeners.Count; i++)
            {
                if (listeners[i].eventHandler == (Delegate) handleAction)
                {
                    EventBus.UnSubscribe(listeners[i]);
                    listeners.RemoveAt(i);
                    return true;
                }
            }
            
            return false;
        }

        public async Task WaitForSeconds(float second)
        {
            await XUtility.WaitSeconds(second, this);
        }

        public CancelableCallback WaitForSeconds(float second, Action callback)
        {
            var cancelableCallback = new CancelableCallback(callback);
            XUtility.WaitSeconds(second, cancelableCallback, this);
            return cancelableCallback;
        }
        
        public CancelableCallback WaitForSecondsRealTime(float second, Action callback)
        {
            var cancelableCallback = new CancelableCallback(callback);
            XUtility.WaitSeconds(second, cancelableCallback, this, true);
            return cancelableCallback;
        }
        
        /// <summary>
        /// 等待N帧
        /// </summary>
        /// <param name="frameCount"></param>
        protected async Task WaitNFrame(int frameCount)
        {
            await XUtility.WaitNFrame(frameCount, this);
        }
        
        protected void WaitNFrame(int frameCount, Action callback)
        {
            XUtility.WaitNFrame(frameCount, callback, this);
        }

        
       /// <summary>
       /// 异步播放动画
       /// </summary>
       /// <param name="animator"></param>
       /// <param name="stateName"></param>
        protected virtual async Task PlayAnimationAsync(Animator animator, string stateName)
        {
            await XUtility.PlayAnimationAsync(animator, stateName, this);
        }
       
       /// <summary>
       /// 异步播放动画
       /// </summary>
       /// <param name="animator"></param>
       /// <param name="stateName"></param>
       /// <param name="finishCallback"></param>
       protected void PlayAnimation(Animator animator, string stateName, Action finishCallback = null)
       {
            XUtility.PlayAnimation(animator, stateName, finishCallback,this);
       }
       
       /// <summary>
       /// 清理Update回掉，WaitTask,Coroutine
       /// </summary>
       public override void CleanUp()
       {
           DisableUpdate();
            
           CleanAllSubscribedEvents();
           
           base.CleanUp();
       }
        
        /// <summary>
        /// 取消事件订阅
        /// </summary>
        /// <param name="handleAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool UnsubscribeEvent<T>(Action<T> handleAction) where T:IEvent
        {
            if (listeners == null)
                return false;

            for (var i = 0; i < listeners.Count; i++)
            {
                if (listeners[i].eventHandler == (Delegate) handleAction)
                {
                    listeners.RemoveAt(i);
                    return true;
                }
            }
            
            return false;
        }
        
        public void CleanAllSubscribedEvents()
        {
            if (listeners == null)
                return;
            for (var i = 0; i < listeners.Count; i++)
            {
                EventBus.UnSubscribe(listeners[i]);
            }
            
            listeners.Clear();
        }

        public Coroutine StartCoroutine(IEnumerator iEnumerator)
        {
            Coroutine coroutine = null;
            
            coroutine = IEnumeratorTool.instance.StartCoroutine(CoroutineWrapper(iEnumerator,
                () =>
                {
                    if(coroutine != null)
                        RemoveCoroutine(coroutine);
                }));
            
            if(coroutine != null)
                AddCoroutine(coroutine);
                
            return coroutine;
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                IEnumeratorTool.instance.StopCoroutine(coroutine);
                RemoveCoroutine(coroutine);
            }
        }
         
        public IEnumerator CoroutineWrapper(IEnumerator iEnumerator, Action finishCallback = null)
        {
            var coroutine = IEnumeratorTool.instance.StartCoroutine(iEnumerator);
            AddCoroutine(coroutine);
            yield return coroutine;
            RemoveCoroutine(coroutine);
            finishCallback?.Invoke();
        }
        
    }
}