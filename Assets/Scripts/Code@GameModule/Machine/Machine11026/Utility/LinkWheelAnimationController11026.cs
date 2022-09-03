// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/07/14:21
// Ver : 1.0.0
// Description : LinkWheelAnimationController11026.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class LinkWheelAnimationController11026:WheelAnimationController
    {
        protected override void OnBlinkAnimationFinished(ElementContainer elementContainer, string appearKey,int rollIndex,int rowIndex)
        {
            listWheelAppears.Remove(appearKey);

            elementContainer.ShiftSortOrder(true);
            elementContainer.PlayElementAnimation("Idle");

            if (containerPlayBlinkAnimation.Count == 0 && canCheckBlinkFinished)
            {
                CheckAndStopBlinkAnimation();
            }

            CheckAllBlinkFinished();
        }
    }
}