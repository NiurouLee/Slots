// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-01-20 7:00 PM
// Ver : 1.0.0
// Description : Hole.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;

public class Hole : Mask
{
    public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (!isActiveAndEnabled)
            return true;

        return !RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera);
    }
}