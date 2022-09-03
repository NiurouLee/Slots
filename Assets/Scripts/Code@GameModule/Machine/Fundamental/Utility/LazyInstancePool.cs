// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/06/13:43
// Ver : 1.0.0
// Description : InstancePool.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class LazyInstancePool<T> where T : Object
    {
        private T _template;

        private Stack<T> _instanceStack;
        
        public LazyInstancePool(T template)
        {
            _template = template;
            _instanceStack = new Stack<T>();
        }
      
        private T NewInstance()
        {
            return Object.Instantiate(_template);
        }

        public T Acquire()
        {
            if (_instanceStack.Count > 0)
            {
                return _instanceStack.Pop();
            }

            return NewInstance();
        }

        public void Recycle(T instance)
        {
            _instanceStack.Push(instance);
        }
    }
}