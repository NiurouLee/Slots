using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;


[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class OffineComponentBinderAttribute : Attribute
{
    public string mPath = string.Empty;
    public string mParent = string.Empty;

    public OffineComponentBinderAttribute(string path, string parentName = "")
    {
        mPath = path;
        mParent = parentName;
    }
}

public class OffineComponentBinder
{
    public static void BindingComponent(object binder, Transform transform)
    {
        var allComponentDict = new Dictionary<string, List<Transform>>();

        CollectAllComponent(transform, allComponentDict);

        var fieldInfoDict = new Dictionary<string, FieldInfo>();

        GetAllFieldInfo(binder.GetType(), fieldInfoDict);

        Type typeAttr = typeof(OffineComponentBinderAttribute);

        foreach (var item in fieldInfoDict)
        {
            //OffineComponentBinderAttribute attr = Attribute.GetCustomAttribute(item.Value,typeAttr) as OffineComponentBinderAttribute;
            //ILRuntime的bug，只能这么取attribute
            var objAttr = item.Value.GetCustomAttributes(typeAttr, false);
            OffineComponentBinderAttribute attr = null;

            if (objAttr.Length > 0)
            {
                attr = objAttr[0] as OffineComponentBinderAttribute;
            }

            if (attr != null)
            {
                var v = FindMatchComponent(transform, attr.mPath, attr.mParent, item.Value.FieldType, allComponentDict);
                item.Value.SetValue(binder, v);
            }
        }
        
        //实现Button函数绑定
        var listMethodInfo = binder.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in listMethodInfo)
        {
            var objAttr = method.GetCustomAttributes(typeAttr, false);
            if (objAttr.Length == 0) continue;
            var attribute = (OffineComponentBinderAttribute) objAttr[0];
            if(attribute == null)
                continue;
            var paramInfos = method.GetParameters();
            if (paramInfos.Length != 0) 
                continue;
            var button = FindMatchComponent(transform, attribute.mPath, attribute.mParent, typeof(Button), allComponentDict) as Button;
            if (button == null) 
                continue;
            button.onClick.AddListener(()=>method.Invoke(binder,new object[]{}));
        }
    }

    private static Component FindMatchComponent(Transform transform, string path, string parentName, Type type,
        Dictionary<string, List<Transform>> componentDict)
    {
        Transform tran = transform.Find(path);
        if (tran != null && string.IsNullOrEmpty(parentName))
        {
            return tran.GetComponent(type);
        }

        if (!path.Contains("/"))
        {
            if (componentDict.TryGetValue(path, out var list))
            {
                if (string.IsNullOrEmpty(parentName))
                {
                    //找第一个
                    Component component = list[0].GetComponent(type);
                    if (component == null && path == transform.name)
                    {
                        component = transform.GetComponent(type);
                    }

                    return component;
                }
                else
                {
                    //匹配父物体
                    foreach (var item in list)
                    {
                        if (item.parent != null && item.parent.name == parentName)
                        {
                            return item.GetComponent(type);
                        }
                    }
                }
            }
        }

        return null;
    }


    private static void GetAllFieldInfo(Type type, Dictionary<string, FieldInfo> listInfo)
    {
        if (listInfo == null)
        {
            listInfo = new Dictionary<string, FieldInfo>();
        }

        var listFiledInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        int count = listFiledInfo.Length;

        for (int i = 0; i < count; i++)
        {
            listInfo[listFiledInfo[i].Name] = listFiledInfo[i];
        }

        if (type.BaseType != null)
        {
            GetAllFieldInfo(type.BaseType, listInfo);
        }
    }

    private static void CollectAllComponent(Transform transform, Dictionary<string, List<Transform>> allComponentDict)
    {
        var listAllComponent = transform.gameObject.GetComponentsInChildren<Transform>(true);

        foreach (var item in listAllComponent)
        {
            List<Transform> list = null;

            if (!allComponentDict.TryGetValue(item.name, out list))
            {
                list = new List<Transform>();
                allComponentDict[item.name] = list;
            }

            list.Add(item);
        }
    }
}