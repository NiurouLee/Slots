// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:17 AM
// Ver : 1.0.0
// Description : LoopFlowController.cs
// ChangeLog :
// **********************************************


namespace GameModule
{
    public class LoopFlowController : ChainFlowController
    {
        public override LogicStepControllerProceedType Proceed()
        {
            var proceedType = controlList[currentIndex].Proceed();

            if (proceedType == LogicStepControllerProceedType.TYPE_PROCEED)
            {
                var args = controlList[currentIndex].OnLeaveFlow();

                currentIndex++;

                if (currentIndex >= controlList.Count)
                    currentIndex = 0;

                controlList[currentIndex].EnterFlow(args);
            }
            else if (proceedType == LogicStepControllerProceedType.TYPE_BREAK)
            {
                return LogicStepControllerProceedType.TYPE_PROCEED;
            }

            return LogicStepControllerProceedType.TYPE_RETAIN;
        }
    }
}