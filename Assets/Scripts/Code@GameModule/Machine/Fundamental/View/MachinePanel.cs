// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:14 PM
// Ver : 1.0.0
// Description : IView.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class MachinePanel
    {
        public Transform transform;
        public MachinePanel(Transform inTransform)
        {
            transform = inTransform;
        }

        public virtual void OnDestroy()
        {
        }
    }
}