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
    public class LinkBackLineView11031 : TransformHolder
    {

        public LinkBackLineView11031(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public void ShowLinkLine(bool enable)
        {
            transform.gameObject.SetActive(enable);
        }
    }
}