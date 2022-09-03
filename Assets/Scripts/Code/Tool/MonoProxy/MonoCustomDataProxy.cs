/**********************************************
Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-11-24 21:59:03
Ver : 1.0.0
Description : 
ChangeLog :  添加基础功能的MonoBehaviour方便以后做功能扩展
**********************************************/


using System;
using System.Collections.Generic;
using UnityEngine;

public class MonoCustomDataProxy : MonoBehaviour
{
    private object customData;
    private Dictionary<string, object> customDataDict;
   
    public void SetCustomData(object inCustomData)
    {
        customData = inCustomData;
    }

    public object GetCustomData()
    {
        return customData;
    }
    
    public bool HasCustomData(string key)
    {
        return customDataDict != null && customDataDict.ContainsKey(key);
    }

    public void SetCustomData(string key, object value)
    {
        if (customDataDict == null)
        {
            customDataDict = new Dictionary<string, object>();
        }

        customDataDict[key] = value;
    }
    
    public T GetCustomData<T>(string key)
    {
        if (customDataDict != null && customDataDict.ContainsKey(key))
            return (T) customDataDict[key];
        return default(T);
    }

    public T GetCustomData<T>() where T : class
    {
        return customData as T;
    }
}