// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:52 AM
// Ver : 1.0.0
// Description : ChainFlowController.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class ChainFlowController : FlowController
    {
        protected readonly List<FlowController> controlList;
        protected int currentIndex;

        public ChainFlowController()
        {
            currentIndex = 0;
            controlList = new List<FlowController>();
        }

        public ChainFlowController Add(params object[] controlNodes)
        {
            for (var i = 0; i < controlNodes.Length; i++)
            {
                if (controlNodes[i] is FlowController)
                {
                    var node = (FlowController) (controlNodes[i]);
                    controlList.Add(node);
                }

                if (controlNodes[i] is LogicStep)
                {
                    LogicStep node = (LogicStep) (controlNodes[i]);
                    controlList.Add(new FlowController(node));
                }
            }

            return this;
        }

        public override void EnterFlow(LogicStepType preLogicStep)
        {
            currentIndex = 0;
            controlList[currentIndex].EnterFlow(preLogicStep);
        }

        public override void SetUp()
        {
            for (var i = 0; i < controlList.Count; i++)
            {
                controlList[i].SetUp();
            }
        }

        public override LogicStepControllerProceedType Proceed()
        {
            var proceedType = controlList[currentIndex].Proceed();

            if (proceedType == LogicStepControllerProceedType.TYPE_PROCEED)
            {
                var args = controlList[currentIndex].OnLeaveFlow();

                currentIndex++;

                if (currentIndex >= controlList.Count)
                    return LogicStepControllerProceedType.TYPE_PROCEED;

                controlList[currentIndex].EnterFlow(args);
            }

            return LogicStepControllerProceedType.TYPE_RETAIN;
        }
        
        public override void OnUpdate()
        {
            controlList[currentIndex].OnUpdate();
        }
        public override void OnDestroy()
        {
            foreach (var controller in controlList)
            {
                controller.OnDestroy();
            }
        }
        public override bool HasLogicStep(LogicStepType stepType)
        {
            for (var i = 0; i < controlList.Count; i++)
            {
                if (controlList[i].HasLogicStep(stepType))
                    return true;
            }

            return false;
        }
        public override void JumpToLogicStep(LogicStepType stepType, LogicStepType preStepType)
        {
            for (var i = 0; i < controlList.Count; i++)
            {
                if (controlList[i].HasLogicStep(stepType))
                {
                    currentIndex = i;
                    controlList[i].JumpToLogicStep(stepType, preStepType);
                    break;
                }
            }
        }
        public override LogicStepType OnLeaveFlow()
        {
            return controlList[currentIndex].OnLeaveFlow();
        }
    }
}