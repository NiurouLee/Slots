// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/16/13:59
// Ver : 1.0.0
// Description : LinkWheel11301.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class LinkWheel11031 : Wheel
    {
        public LockElementLayer11031 lockLayer;
        public int wheelIndex = 0;
        
        public LinkWheel11031(Transform transform) : base(transform)
        {
        
        }

        public void SetWheelIndex(int index)
        {
            wheelIndex = index;
        }
        
        public void AttachLockLayer()
        {
            lockLayer = context.view.Add<LockElementLayer11031>(transform);
            lockLayer.BindingWheel(this);
            lockLayer.SetSortingGroup("Element", 201);
        }

        public void UpdateElementContainerSize(float scaleSize)
        {
            for (int j = 0; j < rollCount; j++)
            {
                var roll = GetRoll(j);
                for (int k = 0; k < roll.containerCount; k++)
                {
                    var elementContainer = roll.GetContainer(k);
                    elementContainer.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
                }
            }
        }
    }
}