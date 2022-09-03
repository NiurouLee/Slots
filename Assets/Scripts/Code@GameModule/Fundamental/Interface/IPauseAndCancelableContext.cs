// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/07/21:15
// Ver : 1.0.0
// Description : IPauseableContext.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DG.Tweening;
using Tool;
using UnityEngine;

namespace GameModule
{
    public class CancelableCallback
    {
        public Action callback;
        public bool callBackCanceled = false;
        private Coroutine _coroutine;
        private IPauseAndCancelableContext _context;
        public CancelableCallback(Action inCallback)
        {
            callback = inCallback;
        }

        public void BindingContextAndCoroutine(Coroutine coroutine, IPauseAndCancelableContext context)
        {
            _coroutine = coroutine;
            _context = context;
        }

        public void CancelCallback()
        {
            callBackCanceled = true;
            
            if (_coroutine != null)
            {
                IEnumeratorTool.instance.StopCoroutine(_coroutine);
                _context.RemoveCancelableCallback(this);
            }
        }
    }
    
    public interface IPauseAndCancelableContext
    {
        bool IsPaused { get; set; }
        void AddWaitTask(TaskCompletionSource<bool> task, Coroutine coroutine);

        void AddCancelableCallback(CancelableCallback callback);

        void RemoveCancelableCallback(CancelableCallback callback);
        
        void RemoveTask(TaskCompletionSource<bool> task);

        void CancelAllWaitTask();

        void AddCoroutine(Coroutine coroutine);
        
        void RemoveCoroutine(Coroutine coroutine);
        
        void StopAllUnFinishedCoroutine();
 
        void AddTweener(Tweener tweener);

        void RemoveTweener(Tweener tweener);

        void KillAllUnFinishedTweener();

        void CleanUp();
    }
}