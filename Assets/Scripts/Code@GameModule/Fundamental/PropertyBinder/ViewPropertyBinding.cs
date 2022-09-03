// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/03/17:40
// Ver : 1.0.0
// Description : ViewPropertyBinding.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameModule
{
    public static class ViewPropertyBinding
    {
        public static void BindingProperty(View view, ViewController viewController)
        {
            var fieldInfo = GetAllFieldInfo(viewController.GetType());
            
            
        }

        private static Dictionary<string, FieldInfo> GetAllFieldInfo(Type type)
        {
            var listInfo = new Dictionary<string, FieldInfo>();

            var listFiledInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            int count = listFiledInfo.Length;

            for (int i = 0; i < count; i++)
            {
                if (listFiledInfo[i].FieldType.IsGenericType && listFiledInfo[i].FieldType.GetGenericTypeDefinition() ==
                    typeof(BindableProperty<,,>))
                    listInfo[listFiledInfo[i].Name] = listFiledInfo[i];
            }
            
            return listInfo;
        }
    }
}