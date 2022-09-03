// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/17:35
// Ver : 1.0.0
// Description : UpdateScheduler.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class UpdateScheduler : IExternalEventScheduler
    {
        private static List<IUpdateable> updateableList;

        private static List<IUpdateable> secondIntervalUpdateableList;
        private static List<IUpdateable> halfSecondIntervalUpdateableList;

        private static List<ILateUpdateable> lateUpdateableList;
        private static List<IFixedUpdateable> fixedUpdateableList;

        private float halfSecondIntervalUpdaterNextActionTime;
        private float secondIntervalUpdaterNextActionTime;
        
        public void AttachToListener()
        {
            UpdateEventListener.OnUpdate = Update;
            UpdateEventListener.OnLateUpdate = LateUpdate;
            UpdateEventListener.OnFixedUpdate = FixedUpdate;
        }

        public void DetachFromListener()
        {
            UpdateEventListener.OnUpdate = null;
            UpdateEventListener.OnLateUpdate = null;
            UpdateEventListener.OnFixedUpdate = null;

            updateableList?.Clear();
            secondIntervalUpdateableList?.Clear();
            halfSecondIntervalUpdateableList?.Clear();
            lateUpdateableList?.Clear();
            fixedUpdateableList?.Clear();
        }

        public void Update()
        {
            if (updateableList == null)
                return;

            if (updateableList != null)
            {
                for (int i = 0; i < updateableList.Count; i++)
                {
                    updateableList[i].Update();
                }
            }

            if (Time.time > secondIntervalUpdaterNextActionTime)
            {
                secondIntervalUpdaterNextActionTime = Time.time + 1.0f;

                if (secondIntervalUpdateableList != null)
                {
                    for (var i = 0; i < secondIntervalUpdateableList.Count; i++)
                    {
                        secondIntervalUpdateableList[i].Update();
                    }
                }
            }

            if (Time.time > halfSecondIntervalUpdaterNextActionTime)
            {
                halfSecondIntervalUpdaterNextActionTime = Time.time + 0.5f;

                if (halfSecondIntervalUpdateableList != null)
                {
                    for (var i = 0; i < halfSecondIntervalUpdateableList.Count; i++)
                    {
                        halfSecondIntervalUpdateableList[i].Update();
                    }
                }
            }
        }

        public void LateUpdate()
        {
            if (lateUpdateableList == null)
                return;
            for (int i = 0; i < lateUpdateableList.Count; i++)
            {
                lateUpdateableList[i].LateUpdate();
            }
        }

        public void FixedUpdate()
        {
            if (fixedUpdateableList == null)
                return;
            for (int i = 0; i < fixedUpdateableList.Count; i++)
            {
                fixedUpdateableList[i].FixedUpdate();
            }
        }

        #region Hook Update

        public static void HookUpdate(IUpdateable updateable)
        {
            if (updateableList == null)
            {
                updateableList = new List<IUpdateable>();
            }

            if (!updateableList.Contains(updateable))
                updateableList.Add(updateable);
        }

        public static void HookSecondUpdate(IUpdateable updateable)
        {
            if (secondIntervalUpdateableList == null)
            {
                secondIntervalUpdateableList = new List<IUpdateable>();
            }

            if (!secondIntervalUpdateableList.Contains(updateable))
                secondIntervalUpdateableList.Add(updateable);
        }

        public static void HookHalfSecondUpdate(IUpdateable updateable)
        {
            if (halfSecondIntervalUpdateableList == null)
            {
                halfSecondIntervalUpdateableList = new List<IUpdateable>();
            }

            if (!halfSecondIntervalUpdateableList.Contains(updateable))
                halfSecondIntervalUpdateableList.Add(updateable);
        }

        public static void UnhookUpdate(IUpdateable updateable)
        {
            if (updateableList != null && updateableList.Contains(updateable))
                updateableList.Remove(updateable);

            if (secondIntervalUpdateableList != null && secondIntervalUpdateableList.Contains(updateable))
                secondIntervalUpdateableList.Remove(updateable);

            if (halfSecondIntervalUpdateableList != null && halfSecondIntervalUpdateableList.Contains(updateable))
                halfSecondIntervalUpdateableList.Remove(updateable);
        }

        public static void HookFixedUpdate(IFixedUpdateable fixedUpdateable)
        {
            if (fixedUpdateableList == null)
            {
                fixedUpdateableList = new List<IFixedUpdateable>();
            }

            if (!fixedUpdateableList.Contains(fixedUpdateable))
                fixedUpdateableList.Add(fixedUpdateable);
        }

        public static void UnhookFixedUpdate(IFixedUpdateable fixedUpdateable)
        {
            fixedUpdateableList?.Remove(fixedUpdateable);
        }

        public static void HookLateUpdate(ILateUpdateable lateUpdateable)
        {
            if (lateUpdateableList == null)
            {
                lateUpdateableList = new List<ILateUpdateable>();
            }

            if (!lateUpdateableList.Contains(lateUpdateable))
                lateUpdateableList.Add(lateUpdateable);

            lateUpdateableList.Add(lateUpdateable);
        }

        public static void UnhookLateUpdate(ILateUpdateable lateUpdateable)
        {
            lateUpdateableList.Remove(lateUpdateable);
        }

        #endregion
    }
}