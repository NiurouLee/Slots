// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 10:32 PM
// Ver : 1.0.0
// Description : MachineView.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System;
using UnityEngine;

namespace GameModule
{
    public class MachineView
    {
        private readonly List<TransformHolder> views;

        private MachineContext context;
        public MachineView(MachineContext inContext)
        {
            context = inContext;
            views = new List<TransformHolder>(15);
        }
        
        public T Get<T>(int index = 0) where T : TransformHolder
        {
            int count = 0;
            for (var i = 0; i < views.Count; i++)
            {
                if (views[i] is T)
                {
                    if (count == index)
                        return (T) views[i];
                    count++;
                }
            }

            return null;
        }

        public T Get<T>(string filter) where T : TransformHolder
        {
            for (var i = 0; i < views.Count; i++)
            {
                if (views[i] is T)
                {
                    if (views[i].MatchFilter(filter))
                        return (T) views[i];
                }
            }
            
            return null;
        }

        public List<T> GetAll<T>() where T : TransformHolder
        {
            List<T> list = new List<T>();
            for (var i = 0; i < views.Count; i++)
            {
                if (views[i] is T)
                {
                    list.Add(views[i] as T);
                }
            }
            
            return list;
        }

        public T QuickAdd<T>(string transformPath) where T : TransformHolder
        {
            var transform = context.transform.Find(transformPath);
            if (transform)
            {
                return Add<T>(transform);
            }
            
            return null;
        }
        public T Add<T>(Transform transform)  where T: TransformHolder
        {
            TransformHolder view = null;

            var constructor = typeof(T).GetConstructor(new[] {typeof(Transform)});
            if (constructor != null)
            {
                view = (TransformHolder) constructor.Invoke(new object[] {transform});
                view.Initialize(context);
                views.Add(view);
            }

            return (T)view;
        }


        private int viewsCount = 0;
        public void Update()
        {
            viewsCount = views.Count;
            for (int i = 0; i < viewsCount; i++)
            {
                views[i].Update();
            }
        }


        public void OnDestroy()
        {
            viewsCount = views.Count;
            for (int i = 0; i < viewsCount; i++)
            {
                views[i].OnDestroy();
            }
        }
    }
}