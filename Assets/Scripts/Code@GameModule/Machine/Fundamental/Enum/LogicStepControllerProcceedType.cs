// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : LogicStepControllerProceedType.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public enum LogicStepControllerProceedType
    {
        /// <summary>
        /// 保持不变
        /// </summary>
        TYPE_RETAIN,
        /// <summary>
        /// 进入下一个LogicStep
        /// </summary>
        TYPE_PROCEED,
        /// <summary>
        /// 结束当前LogicStep LOOP循环，
        /// </summary>
        TYPE_BREAK,
    }
}