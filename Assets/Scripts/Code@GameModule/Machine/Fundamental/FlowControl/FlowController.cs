// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:52 AM
// Ver : 1.0.0
// Description : FlowController.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class FlowController
    {
        protected LogicStep step;

        public FlowController()
        {
            step = null;
        }

        public FlowController(LogicStep inStep)
        {
            step = inStep;
        }

        public virtual void EnterFlow(LogicStepType preLogicStep)
        {
            step.HandleStepLogic(preLogicStep);
        }

        public virtual void SetUp()
        {
            step.SetUpLogicStep();
        }

        public virtual LogicStepControllerProceedType Proceed()
        {
            if (step.IsConditionStep() && step.IsMatchCondition())
                return LogicStepControllerProceedType.TYPE_BREAK;

            return LogicStepControllerProceedType.TYPE_PROCEED;
        }

        public virtual void OnUpdate()
        {
            step.LogicUpdate();
        }

        public virtual LogicStepType OnLeaveFlow()
        {
            return step.StepType;
        }

        public virtual void OnDestroy()
        {
            step.OnDestroy();
        }

        public virtual bool HasLogicStep(LogicStepType stepType)
        {
            return step.StepType == stepType;
        }

        public virtual void JumpToLogicStep(LogicStepType stepType, LogicStepType preLogicStepType)
        {
            if (step.StepType == stepType)
            {
                EnterFlow(preLogicStepType);
            }
        }
    }
}