// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 7:31 PM
// Ver : 1.0.0
// Description : IFlowControlContext.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public interface IFlowControlContext
    {
        LogicStep GetRunningStep();
        void SetStepToRunning(LogicStep process);
         
        /// <summary>
        /// 处理完了当前LogicStep的所有逻辑，进入预先定义好的下一个流程
        /// </summary>
        void Proceed();
        ILogicStepProxy GetLogicStepProxy(LogicStepType stepType);
    }
}