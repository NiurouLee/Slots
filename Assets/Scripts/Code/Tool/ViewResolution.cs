// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/16:07
// Ver : 1.0.0
// Description : ViewResolution.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewResolution
{
    /// <summary>
    /// 设计分辨率
    /// </summary>
    public static Vector2 designSize = new Vector2(1365, 768);
    
    /// <summary>
    /// 竖屏参考分辨率
    /// </summary>
    public static Vector2 referenceResolutionPortrait;
    
    /// <summary>
    /// 横屏参考分辨率
    /// </summary>
    public static Vector2 referenceResolutionLandscape;
   
    
    public static void SetUpViewResolution()
    {
        var screenWidth = Math.Max(Screen.width, Screen.height);
        var screenHeight = Math.Min(Screen.width, Screen.height);

        var referenceSizeY = designSize.y;
        var referenceWidth = (float) (screenWidth) / screenHeight * referenceSizeY;

        referenceResolutionLandscape = new Vector2(referenceWidth, referenceSizeY);
        referenceResolutionPortrait = new Vector2(referenceSizeY, referenceWidth);
    }

    public static Vector3 GetCameraPositionByViewResolution(Camera camera, bool isPortrait)
    {
        var referenceUnit = referenceResolutionLandscape.y * 0.005f;
      
        if (isPortrait)
        {
            referenceUnit = referenceResolutionPortrait.y * 0.005f;
        }
        
        var z = referenceUnit / Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5f);
        return new Vector3(0, 0, (float) -z);
    }
}
 