// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-12-22 12:05 PM
// Ver : 1.0.0
// Description : TweenCanNotPuase.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DG.Tweening;

namespace GameModule
{
    public static class TweenCanNotPause
    {
        private static List<int> list;

        static TweenCanNotPause()
        {
            list = new List<int>();
        }

        public static void AddTween(Tween tween)
        {
            list.Add(tween.intId);
        }

        public static void RemoveTween(Tween tween)
        {
            list.Remove(tween.intId);
        }

        public static bool HasTween(Tween tween)
        {
            return list.Contains(tween.intId);
        }
        
    }
}