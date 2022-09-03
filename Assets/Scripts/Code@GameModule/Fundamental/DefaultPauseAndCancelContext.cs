// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/20:24
// Ver : 1.0.0
// Description : DefaultPauseAndCancelContext.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Tool;
using UnityEngine;

namespace GameModule
{
    public class DefaultPauseAndCancelContext: IPauseAndCancelableContext
    {
        private static DefaultPauseAndCancelContext _instance;

        public static DefaultPauseAndCancelContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DefaultPauseAndCancelContext();
                }
                return _instance;
            }
        }

        protected List<Tuple<TaskCompletionSource<bool>,Coroutine>> waitTaskList;
        protected List<Coroutine> unFinishedCoroutines;
        protected List<CancelableCallback> cancelableCallbackList;
        public bool IsPaused { get; set; }
        
        public void AddWaitTask(TaskCompletionSource<bool> task,Coroutine coroutine)
        {
            if (waitTaskList == null)
            {
                waitTaskList = new List<Tuple<TaskCompletionSource<bool>, Coroutine>>();
            }
            
            waitTaskList.Add(new Tuple<TaskCompletionSource<bool>, Coroutine>(task,coroutine));
        }

        public void AddCancelableCallback(CancelableCallback callback)
        {
            if (cancelableCallbackList == null)
            {
                cancelableCallbackList = new List<CancelableCallback>();
            }
            cancelableCallbackList.Add(callback);   
        }

        public void RemoveCancelableCallback(CancelableCallback callback)
        {
            if(cancelableCallbackList != null && cancelableCallbackList.Contains(callback))
                cancelableCallbackList.Remove(callback);
        }

        public void RemoveTask(TaskCompletionSource<bool> task)
        {
            if (waitTaskList != null)
            {
                for (var i = 0; i < waitTaskList.Count; i++)
                {
                    if (waitTaskList[i].Item1 == task)
                    {
                        var item = waitTaskList[i];
                        waitTaskList.RemoveAt(i);
                       
                        if(item.Item2 != null)
                            IEnumeratorTool.instance.StopCoroutine(item.Item2);
                        break;
                    }
                }
            }
        }

        public void CancelAllWaitTask()
        {
            if (waitTaskList != null)
            {
                for (var i = 0; i < waitTaskList.Count; i++)
                {
                    waitTaskList[i].Item1.TrySetCanceled();
                   
                    if(waitTaskList[i].Item2 != null)
                        IEnumeratorTool.instance.StopCoroutine(waitTaskList[i].Item2);
                }
                
                waitTaskList.Clear();
            }
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            if (unFinishedCoroutines == null)
            {
                unFinishedCoroutines = new List<Coroutine>();
            }
            unFinishedCoroutines.Add(coroutine);
        }
        
        public void RemoveCoroutine(Coroutine coroutine)
        {
            unFinishedCoroutines.Remove(coroutine);
        }

        public void StopAllUnFinishedCoroutine()
        {
            if (unFinishedCoroutines == null)
            {
                return;
            }

            for (var i = 0; i < unFinishedCoroutines.Count; i++)
            {
                IEnumeratorTool.instance.StopCoroutine(unFinishedCoroutines[i]);
            }

            unFinishedCoroutines.Clear();
        }

        private List<Tweener> listTweeners = new List<Tweener>();
        public void AddTweener(Tweener tweener)
        {
            if (!listTweeners.Contains(tweener))
            {
                listTweeners.Add(tweener);
            }
        }

        public void RemoveTweener(Tweener tweener)
        {
            listTweeners.Remove(tweener);
        }

        public void KillAllUnFinishedTweener()
        {
            int count = listTweeners.Count;
            for (int i = 0; i < count; i++)
            {
                listTweeners[i].Kill();
            }
            listTweeners.Clear();
        }
        
        public void RemoveAllCancelableContext() 
        {
            if (cancelableCallbackList != null)
            {
                for (var i = 0; i < cancelableCallbackList.Count; i++)
                {
                    cancelableCallbackList[i].CancelCallback();
                }
                
                cancelableCallbackList.Clear();
            }    
        }

        public virtual void CleanUp()
        {
            StopAllUnFinishedCoroutine();
            
            CancelAllWaitTask();
            
            KillAllUnFinishedTweener();
            
            RemoveAllCancelableContext();
        }
    }
}