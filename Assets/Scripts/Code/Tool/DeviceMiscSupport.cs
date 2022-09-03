// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-01-25 12:09 PM
// Ver : 1.0.0
// Description : DeviceMiscSupport.cs
// ChangeLog :
// **********************************************

using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;

public static class DeviceMiscSupport
{
    public static string advertisingId;
    public static string androidId;
    private static bool isTrackingEnabled = true;
  
    
    public static string GetAndroidId()
    {
        if (string.IsNullOrEmpty(androidId))
        {
            androidId = "none";
#if (UNITY_ANDROID && !UNITY_EDITOR) || ANDROID_CODE_VIEW
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject currentActivity =
 unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (AndroidJavaObject contentResolver =
 currentActivity.Call<AndroidJavaObject>("getContentResolver"))
                        {
                            using (AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure"))
                            {
                                androidId = secure.CallStatic<string>("getString", contentResolver, "android_id");
                                if (string.IsNullOrEmpty(androidId))
                                {
                                    androidId = "none";
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
            }
#endif
            return androidId;
        }

        return androidId;
    }

    /// <summary>
    /// return gaid or idfa
    /// </summary>
    public static void GetAdvertisingIdentifierAsync()
    {
        if (string.IsNullOrEmpty(advertisingId) && isTrackingEnabled)
        {
            isTrackingEnabled = Application.RequestAdvertisingIdentifierAsync(
                (string inAdvertisingId, bool trackingEnabled, string error) =>
                {
                    isTrackingEnabled = trackingEnabled;
                    advertisingId = inAdvertisingId;
                   // Debug.Log("advertisingId " + advertisingId + " " + trackingEnabled + " " + error);
                }
            );
        }
    }

    public static string GetAdvertisingIdentifier()
    {
        return advertisingId;
    }
}
     