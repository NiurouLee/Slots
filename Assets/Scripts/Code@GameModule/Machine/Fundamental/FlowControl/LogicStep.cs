// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 7:25 PM
// Ver : 1.0.0
// Description : Step.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    /// <summary>
    /// 对老虎机Spin过程中的各个子步骤的抽象，
    /// 具体的要完成的逻辑代码在对应的LogicStepProxy中实现
    /// LogicStep只完成当前LogicStep相关的逻辑，不用关心别的Step
    /// </summary>
    public class LogicStep
    {
        private readonly IFlowControlContext context;
        private readonly LogicStepType stepType;
        
        private ILogicStepProxy logicProxy;
        
        public LogicStepType StepType => stepType;

        public LogicStep(IFlowControlContext inContext, LogicStepType inStepType)
        {
            context = inContext;
            stepType = inStepType;
        }

        public void SetUpLogicStep()
        {
            logicProxy = context.GetLogicStepProxy(stepType);
            logicProxy?.SetUp();
        }

        public void HandleStepLogic(LogicStepType preLogicStep)
        {
            context.SetStepToRunning(this);
            //XDebug.Log("CurrentStep" + stepType);
            if (logicProxy == null)
            {
                Proceed();
                return;
            }

            if (logicProxy.CheckCurrentStepHasLogicToHandle())
            {
                logicProxy.HandleStepLogic(preLogicStep, Proceed);
            }
            else
            {
                Proceed();
            }
        }

        private void Proceed()
        {
            var logicStep = context.GetRunningStep();
            if (logicStep != this)
            {
                //TODO: Log with log util and Report error to statistic server;
                Debug.LogError(
                    $"Suppose Not To Happen:RunningLogicStep:{logicStep.stepType}/CurrentLogicStep:{stepType}");
            }

            context.Proceed();
        }

        public bool IsConditionStep()
        {
            return logicProxy == null ? false : logicProxy.IsConditionStep();
        }

        public bool IsMatchCondition()
        {
            return logicProxy == null ? false : logicProxy.IsMatchCondition();
        }
 
        public void LogicUpdate()
        {
            logicProxy?.LogicUpdate();
        }

        public void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            logicProxy?.OnMachineInternalEvent(internalEvent, args);
        }

        public void OnDestroy()
        {
            logicProxy?.OnDestroy();
        }
        
    }
}