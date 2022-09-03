#if !UNITY_EDITOR
#define NO_LOG
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class XDebug
    {
        public static void Log(object msg)
        {
#if !PRODUCTION_PACKAGE
            UnityEngine.Debug.Log(msg);
#endif
        }
        
        public static void LogOnExceptionHandler(string msg)
        {
#if !PRODUCTION_PACKAGE
            UnityEngine.Debug.Log("[[ShowOnExceptionHandler]]" + msg);
#endif
        }

        public static void LogWarning(object msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        public static void LogError(object msg)
        {
            UnityEngine.Debug.LogError(msg);
        }
    }
}