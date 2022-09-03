// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-06 7:36 PM
// Ver : 1.0.0
// Description : IStepMedator.cs
// ChangeLog :
// **********************************************

using System;

namespace GameModule
{
    public interface ILogicStepProxy
    {
        void SetUp();
  
        bool CheckCurrentStepHasLogicToHandle();

        void HandleStepLogic(LogicStepType preLogicStepType, Action handleEndCallback);
 
        bool IsConditionStep();

        bool IsMatchCondition();

        void LogicUpdate();

        void OnDestroy();

        void OnMachineInternalEvent(MachineInternalEvent internalEvent,  params object[] args);
    }
}