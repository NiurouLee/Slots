// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/20/14:31
// Ver : 1.0.0
// Description : BindableProperty.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    
    public class BindableProperty<TComponent, TValue, TValueUpdater>
        where TComponent : Component where TValueUpdater : ValueUpdater<TComponent, TValue>
    {
        protected TComponent component;
        protected TValue value = default(TValue);
        protected TValueUpdater updater;

        public TValue Value
        {
            get => value;
            set
            {
                this.value = value;
                updater.UpdateValue(component, value);
            }
        }
 
        public BindableProperty(TComponent inComponent)
        {
            component = inComponent;
        }
    }
}