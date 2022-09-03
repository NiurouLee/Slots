// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/24/15:10
// Ver : 1.0.0
// Description : MachineFeature.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MachineFeature
    {
        private readonly List<Feature> features;

        private MachineContext context;
        
        public MachineFeature(MachineContext inContext)
        {
            context = inContext;
            features = new List<Feature>(15);
        }
        
        public T Get<T>(int index = 0) where T : Feature
        {
            int count = 0;
            for (var i = 0; i < features.Count; i++)
            {
                if (features[i] is T)
                {
                    if (count == index)
                        return (T) features[i];
                    count++;
                }
            }

            return null;
        }

        public T Get<T>(string filter) where T : Feature
        {
            for (var i = 0; i < features.Count; i++)
            {
                if (features[i] is T)
                {
                    if (features[i].MatchFilter(filter))
                        return (T) features[i];
                }
            }
            
            return null;
        }

        public List<T> GetAll<T>() where T : Feature
        {
            List<T> list = new List<T>();
            for (var i = 0; i < features.Count; i++)
            {
                if (features[i] is T)
                {
                    list.Add(features[i] as T);
                }
            }
            
            return list;
        }

        public T Add<T>(string featureName) where T : Feature
        {
            Feature feature = (T) Activator.CreateInstance(typeof(T), featureName);

            features.Add(feature);
            return (T) feature;
        }

        public T Add<T>() where T : Feature
        {
            Feature feature = null;
            feature = Activator.CreateInstance<T>();
            feature.Initialize(context);
            features.Add(feature);
            return (T) feature;
        }
        
        public void OnDestroy()
        {
            var count = features.Count;
            for (int i = 0; i < count; i++)
            {
                features[i].OnDestroy();
            }
        }
    }
}