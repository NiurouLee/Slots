/**********************************************

Copyright(c) 2021 by com.ustar
All right reserved

Author : Jian.Wang 
Date : 2020-09-18 11:51:13
Ver : 1.0.0
Description : 
ChangeLog :
**********************************************/


using SimpleJson;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace GameModule
{
    public static class JsonHelper
    {
        public static T ParseValue<T>(T defaultValue, object jsonElement)
        {
            if (jsonElement != null)
            {
                Type t = typeof(T);
                if (typeof(IJsonParseable).IsAssignableFrom(t))
                {
                    IJsonParseable instance = (IJsonParseable) CreateInstance(typeof(T));
                    instance.Parse(jsonElement);
                    return (T) instance;
                }

                return (T) Convert.ChangeType(jsonElement, typeof(T));
            }

            return defaultValue;
        }

        public static List<T> ParseListValue<T>(T defaultValue, object jsonElement,
            Func<object, T> parseValueFunc = null)
        {
            if (jsonElement != null)
            {
                JsonArray jsonArray = jsonElement as JsonArray;

                List<T> list = new List<T>();
                if (jsonArray != null)
                {
                    for (var i = 0; i < jsonArray.Count; i++)
                    {
                        if (parseValueFunc != null)
                        {
                            list.Add(parseValueFunc(jsonArray[i]));
                        }
                        else
                        {
                            list.Add(JsonHelper.ParseValue<T>(defaultValue, jsonArray[i]));
                        }
                    }
                }

                return list;
            }

            return null;
        }

        public static T Parse<T>(string key, T defaultValue, object inObject)
        {
            JsonObject jsonObject = inObject as JsonObject;
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                Type t = typeof(T);
                if (typeof(IJsonParseable).IsAssignableFrom(t))
                {
                    IJsonParseable instance = (IJsonParseable) CreateInstance(typeof(T));
                    instance.Parse(jsonObject[key] as JsonObject);
                    return (T) instance;
                }
                
                if(jsonObject[key] != null)
                    return (T) Convert.ChangeType(jsonObject[key], typeof(T));
                //return jsonObject[key];
            }

            return defaultValue;
        }

        public static T ParseField<T>(this object inObject, string key, T defaultValue)
        {
            return Parse<T>(key, defaultValue, inObject);
        }

        public static T Parse<T>(T defaultValue, object inObject)
        {
            JsonObject jsonObject = inObject as JsonObject;
            Type t = typeof(T);
            if (typeof(IJsonParseable).IsAssignableFrom(t))
            {
                IJsonParseable instance = (IJsonParseable) CreateInstance(typeof(T));
                instance.Parse(jsonObject);
                return (T) instance;
            }

            return (T) Convert.ChangeType(jsonObject, typeof(T));

            // return defaultValue;
        }

        public static List<List<List<T>>> ParseListListList<T>(string key, T defaultValue, object inObject)
        {
            JsonObject jsonObject = inObject as JsonObject;
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                JsonArray jsonArray = jsonObject[key] as JsonArray;

                List<List<List<T>>> list = new List<List<List<T>>>();
                if (jsonArray != null)
                {
                    for (var i = 0; i < jsonArray.Count; i++)
                    {
                        list.Add(ParseListListValue<T>(defaultValue, jsonArray[i]));
                    }
                }

                return list;
            }

            return null;
        }

        public static List<List<T>> ParseListListValue<T>(T defaultValue, object inObject)
        {
            JsonArray jsonArray = inObject as JsonArray;

            List<List<T>> list = new List<List<T>>();
            if (jsonArray != null)
            {
                for (var i = 0; i < jsonArray.Count; i++)
                {
                    list.Add(ParseListValue<T>(defaultValue, jsonArray[i]));
                }
            }

            return list;
        }

        public static List<List<T>> ParseListList<T>(string key, T defaultValue, object inObject)
        {
            JsonObject jsonObject = inObject as JsonObject;
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                JsonArray jsonArray = jsonObject[key] as JsonArray;

                List<List<T>> list = new List<List<T>>();
                if (jsonArray != null)
                {
                    for (var i = 0; i < jsonArray.Count; i++)
                    {
                        list.Add(ParseListValue<T>(defaultValue, jsonArray[i]));
                    }
                }

                return list;
            }

            return null;
        }

        public static List<T> ParseList<T>(string key, T defaultValue, object inObject)
        {
            JsonObject jsonObject = inObject as JsonObject;
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                JsonArray jsonArray = jsonObject[key] as JsonArray;

                List<T> list = new List<T>();
                if (jsonArray != null)
                {
                    for (var i = 0; i < jsonArray.Count; i++)
                    {
                        list.Add(JsonHelper.ParseValue<T>(defaultValue, jsonArray[i]));
                    }
                }

                return list;
            }

            return null;
        }

        public static List<T> ParseFieldList<T>(this object inObject, string key, T defaultValue)
        {
            return ParseList<T>(key, defaultValue, inObject);
        }

        public static Dictionary<string, T> ParseDict<T>(string key, T defaultValue, object inObject,
            Func<object, T> func = null)
        {
            JsonObject jsonObject = inObject as JsonObject;
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                JsonObject jsonDict = jsonObject[key] as JsonObject;

                Dictionary<string, T> dict = new Dictionary<string, T>();
                if (jsonDict != null)
                {
                    foreach (var item in jsonDict)
                    {
                        if (func != null)
                        {
                            dict[item.Key] = func(item.Value);
                        }
                        else
                        {
                            dict[item.Key] = JsonHelper.ParseValue<T>(defaultValue, item.Value);
                        }
                    }
                }

                return dict;
            }

            return null;
        }


        public static T ResolveEntity<T>(string key, JsonObject jsonObject)
        {
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                return ResolveEntity<T>(jsonObject[key] as JsonObject);
            }

            return default(T);
        }

        public static T ResolveEntity<T>(JsonObject jsonObject)
        {
            var entity = CreateInstance(typeof(T));
            ResolveEntity(entity, jsonObject);
            return (T) entity;
        }

        public static List<T> ResolveEntityList<T>(string key, JsonObject jsonObject)
        {
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                var list = ResolveEntityList(typeof(List<T>), typeof(T), jsonObject[key] as JsonArray) as List<T>;
                return list;
            }

            return null;
        }

        public static List<T> ResolveEntityList<T>(JsonArray jsonArray)
        {
            var list = ResolveEntityList(typeof(List<T>), typeof(T), jsonArray) as List<T>;
            return list;
        }

        public static Dictionary<string, T> ResolveEntityDict<T>(string key, JsonObject jsonObject)
        {
            if (jsonObject != null && jsonObject.ContainsKey(key))
            {
                var list =
                    ResolveEntityDict(typeof(Dictionary<string, T>), typeof(T), jsonObject[key] as JsonObject) as
                        Dictionary<string, T>;
                return list;
            }

            return null;
        }

        public static Dictionary<string, T> ResolveEntityDict<T>(JsonObject jsonObject)
        {
            var dict = ResolveEntityDict(typeof(Dictionary<string, T>), typeof(T), jsonObject) as Dictionary<string, T>;
            return dict;
        }

        public static object ResolveEntity(Type entityType, JsonObject jsonObject)
        {
            var entity = CreateInstance(entityType);
            ResolveEntity(entity, jsonObject);
            return entity;
        }

        public static object CreateInstance(Type instanceType)
        {
            if (instanceType is ILRuntime.Reflection.ILRuntimeType)
                return ((ILRuntime.Reflection.ILRuntimeType) instanceType).ILType.Instantiate();

            if (instanceType is ILRuntime.Reflection.ILRuntimeWrapperType)
                instanceType = ((ILRuntime.Reflection.ILRuntimeWrapperType) instanceType).RealType;

            //  XDebug.Log("CreateInstance" + instanceType.FullName);

            return Activator.CreateInstance(instanceType);
        }

        public static void ResolveEntity<T>(T entity, JsonObject jsonObject)
        {
            Type type = entity.GetType();
            FieldInfo[] fieldInfo = type.GetFields();

            for (var i = 0; i < fieldInfo.Length; i++)
            {
                if (jsonObject.ContainsKey(fieldInfo[i].Name))
                {
                    //     XDebug.Log("fieldInfo[i].Name:" + fieldInfo[i].Name);
                    Type fieldType = fieldInfo[i].FieldType;

                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type argumentType = GetListItemType(fieldType);

                        fieldInfo[i].SetValue(entity,
                            ResolveEntityList(fieldType, argumentType, jsonObject[fieldInfo[i].Name] as JsonArray));
                    }
                    else if (fieldType.IsGenericType &&
                             fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        Type argumentType = GetDictItemType(fieldType);

                        fieldInfo[i].SetValue(entity,
                            ResolveEntityDict(fieldType, argumentType, jsonObject[fieldInfo[i].Name] as JsonObject));
                    }
                    else if (fieldType.IsPrimitive || fieldType == typeof(string))
                    {
                        fieldInfo[i].SetValue(entity,
                            Convert.ChangeType(jsonObject[fieldInfo[i].Name], fieldInfo[i].FieldType));
                    }
                    else if (fieldType.IsEnum)
                    {
                        fieldInfo[i].SetValue(entity, Enum.Parse(fieldType, jsonObject[fieldInfo[i].Name].ToString()));
                    }
                    else
                    {
                        var fieldValue = CreateInstance(fieldType);
                        fieldInfo[i].SetValue(entity, fieldValue);
                        ResolveEntity(fieldInfo[i].GetValue(entity), jsonObject[fieldInfo[i].Name] as JsonObject);
                    }
                }
            }
        }

        private static Type GetDictItemType(Type dictType)
        {
            if (dictType is ILRuntime.Reflection.ILRuntimeWrapperType)
            {
                // XDebug.Log("Type++++++++++++" + type.FullName + "is ILRuntime.Reflection.ILRuntimeWrapperType");
                var wt = (ILRuntime.Reflection.ILRuntimeWrapperType) dictType;
                return wt.CLRType.GenericArguments[1].Value.ReflectionType;
            }

            return dictType.GenericTypeArguments[1];
        }

        private static Type GetListItemType(Type listType)
        {
            if (listType is ILRuntime.Reflection.ILRuntimeWrapperType)
            {
                // XDebug.Log("Type++++++++++++" + type.FullName + "is ILRuntime.Reflection.ILRuntimeWrapperType");
                var wt = (ILRuntime.Reflection.ILRuntimeWrapperType) listType;
                return wt.CLRType.GenericArguments[0].Value.ReflectionType;
            }

            return listType.GenericTypeArguments[0];
        }

        public static object ResolveEntityList(Type listType, Type itemType, JsonArray jsonArray)
        {
            if (jsonArray != null && jsonArray.Count > 0)
            {
                // XDebug.Log("ResolveEntityList[]" + itemType.Name);

                IList list = (IList) CreateInstance(listType);

                for (var i = 0; i < jsonArray.Count; i++)
                {
                    if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type argumentType = GetListItemType(itemType);
                        // XDebug.Log("ResolveEntityList[ResolveEntityList]" + itemType.Name);      
                        // XDebug.Log("********=>_++++++_+_+_+_+_+_+_+_+" + argumentType.FullName);
                        //
                        list.Add(ResolveEntityList(itemType, argumentType, jsonArray[i] as JsonArray));
                    }
                    else if (itemType.IsGenericType &&
                             itemType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        Type argumentType = GetDictItemType(itemType);
                        // XDebug.Log("ResolveEntityList[ResolveEntityDict]" + itemType.Name);
                        list.Add(ResolveEntityDict(itemType, argumentType, jsonArray[i] as JsonObject));
                    }
                    else if (itemType.IsPrimitive)
                    {
                        list.Add(Convert.ChangeType(jsonArray[i], itemType));
                    }
                    else
                    {
                        // XDebug.Log("ResolveEntityList[ResolveEntity]" + itemType.Name);      
                        list.Add(ResolveEntity(itemType, jsonArray[i] as JsonObject));
                    }
                }

                return list;
            }

            return null;
        }

        public static object ResolveEntityDict(Type dictType, Type valueType, JsonObject jsonObject)
        {
            IDictionary dict = (IDictionary) CreateInstance(dictType);

            foreach (var item in jsonObject)
            {
              //  dict[item.Key] = CreateInstance(valueType);

                if (valueType.IsPrimitive || valueType == typeof(string))
                {
                    dict[item.Key] = Convert.ChangeType(jsonObject[item.Key], valueType);
                }
                else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type argumentType = GetListItemType(valueType);
                    dict[item.Key] = ResolveEntityList(valueType, argumentType, item.Value as JsonArray);
                }
                else if (valueType.IsGenericType &&
                         valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type argumentType = GetDictItemType(valueType);
                    dict[item.Key] = ResolveEntityDict(valueType, argumentType, item.Value as JsonObject);
                }
                else
                {
                    dict[item.Key] = ResolveEntity(valueType, item.Value as JsonObject);
                }
            }

            return dict;
        }
    }
}