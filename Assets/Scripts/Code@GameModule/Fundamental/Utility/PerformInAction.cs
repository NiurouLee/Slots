// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/15/10:46
// Ver : 1.0.0
// Description : PerformInAction.cs
// 添加一个类来记录当前进行中的表演，方便我们来确定表演是否结束与否
// ChangeLog :
// **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public enum PerformCategory
    {
        None,
        LevelRush,    
    }

    public interface IPerform
    {
        bool IsPerformStillValid();
        PerformCategory GetPerformCategory();
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class PerformInAction:LogicController
    {
        public PerformInAction(Client client) : base(client)
        {
            
        }
        
        public static Dictionary<PerformCategory, List<IPerform>> performs;
        protected override void Initialization()
        {
            performs = new Dictionary<PerformCategory, List<IPerform>>();
            base.Initialization();
        }

        /// <summary>
        /// 添加一个表演
        /// </summary>
        /// <param name="perform"></param>
        public static void AddPerform(IPerform perform)
        {
            if (perform.IsPerformStillValid())
            {
                if (performs == null)
                {
                    performs = new Dictionary<PerformCategory, List<IPerform>>();
                }

                if (!performs.ContainsKey(perform.GetPerformCategory()))
                {
                    performs[perform.GetPerformCategory()] = new List<IPerform>();
                }

                performs[perform.GetPerformCategory()].Add(perform);
            }
        }
        
        /// <summary>
        /// 移除指定的表演
        /// </summary>
        /// <param name="perform"></param>
        
        public static void RemovePerform(IPerform perform)
        {
            if (performs != null && performs.ContainsKey(perform.GetPerformCategory()))
            {
                PerformCategory category = perform.GetPerformCategory();
                var performInCategory = performs[category];
                if (performInCategory.Count > 0)
                {
                    performInCategory.Remove(perform);
                    EventBus.Dispatch(new EventPerformRemoved(category));
                }
            }
        }

        /// <summary>
        /// 检查指定类型的表演是否全部完成
        /// </summary>
        /// <param name="performCategory"></param>
        public static bool CheckHasPerformInAction(PerformCategory performCategory)
        {
            if (performs != null && performs.ContainsKey(performCategory))
            {
                var performInCategory = performs[performCategory];
                if (performInCategory != null && performInCategory.Count > 0)
                {
                    for (var i = performInCategory.Count - 1; i >= 0; i--)
                    {
                        if (!performInCategory[i].IsPerformStillValid())
                        {
                            performInCategory.RemoveAt(i);
                        }
                    }

                    if (performInCategory.Count > 0)
                    {
                        return true;
                    }
                }
            }
            
            return PopupStack.HasPerformNotStart(performCategory);
        }

        public override void CleanUp()
        {
            base.CleanUp();
            
            if (performs != null)
            {
                performs.Clear();
            }
        }
    }
}