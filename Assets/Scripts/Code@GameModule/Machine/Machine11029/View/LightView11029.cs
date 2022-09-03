// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/01/14:17
// Ver : 1.0.0
// Description : BackGroundView11001.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class LightView11029 : TransformHolder
    {
        public LightView11029(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public void ShowLight(bool show = false)
        {
            transform.gameObject.SetActive(show);
        }
    }
}