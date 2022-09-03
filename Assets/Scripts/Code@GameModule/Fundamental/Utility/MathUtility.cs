//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-30 17:38
//  Ver : 1.0.0
//  Description : MathUtility.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public static class MathUtility
    {
        /// <summary>
        /// 获取不重复的随机条目
        /// </summary>
        /// <param name="list"></param>
        /// <param name="needItemCount"></param>
        /// <returns></returns>
        public static List<T> GetNoRepeatRandomList<T>(List<T> list, int needItemCount=1)
        {
            List<T> newList = new List<T>(list);
            List<T> retList = new List<T>();

            for (int i = 0; i < needItemCount; i++)
            {
                int index = UnityEngine.Random.Range(0, newList.Count);
                while (newList.Count>0 && retList.Contains(newList[index]))
                {
                    newList.RemoveAt(index);
                    index = UnityEngine.Random.Range(0, newList.Count);
                }
                if (newList.Count>0)
                {
                    retList.Add(newList[index]);
                    newList.RemoveAt(index);   
                }
            }
            return retList;
        }
        
        public static void Shuffle<T>(this IList<T> ts) {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                (ts[i], ts[r]) = (ts[r], ts[i]);
            }
        }

    }
}