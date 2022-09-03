using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticExtend
{
    /// <summary>
    /// 获取Transform下指定路径下的组件
    /// </summary>
    /// <typeparam name="T">Component</typeparam>
    /// <param name="t"></param>
    /// <param name="path">指定路径</param>
    /// <param name="forceCreate">是否强制创建</param>
    /// <returns></returns>
    public static T Bind<T>(this Transform t, string path, bool forceCreate = false) where T : Component
    {
        if (t == null)
        {
            Debug.LogError("t is Null");
            return null;
        }

        T component = null;


        var temp = t.Find(path);

        if (temp == null)
        {
            Debug.LogError(string.Format("Find{0}/{1}Failed", t.name, path));
            return null;
        }

        component = temp.GetComponent<T>();

        if (forceCreate && component == null)
        {
            component = temp.gameObject.AddComponent<T>();
        }

        return component;
    }

    public static T Bind<T>(this Transform t, bool forceCreate = false) where T : Component
    {
        T component = null;

        component = t.GetComponent<T>();

        if (component == null && forceCreate)
        {
            component = t.gameObject.AddComponent<T>();
        }

        return component;
    }
}