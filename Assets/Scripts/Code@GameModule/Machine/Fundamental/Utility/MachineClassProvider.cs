using System;
using System.Collections.Generic;

namespace GameModule
{
    public class MachineClassProvider
    {
        public Dictionary<Type, List<object>> classDict;

        public MachineClassProvider()
        {
            classDict = new Dictionary<Type, List<object>>();
        }
        public T InstantiateClassObject<T>()
        {
            var type = typeof(T);
            if (!classDict.ContainsKey(type))
            {
                classDict[type] = new List<object>();
            }
            var classList = classDict[type];
            if (classList.Count > 0)
            {
                return (T)classList.Pop();
            }
            else
            {
                var newObject = Activator.CreateInstance<T>();
                return newObject;
            }
        }
        public void RecycleClassObject(object classObject)
        {
            var type = classObject.GetType();
            if (!classDict.ContainsKey(type))
            {
                classDict[type] = new List<object>();
            }
            var classList = classDict[type];
            classList.Add(classObject);
        }
    }
}