//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-12-10 14:46
//  Ver : 1.0.0
//  Description : WinLineAnimationController11020.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class WinLineAnimationController11020: WinLineAnimationController
    {
        public override void StopAllElementAnimation(bool force = false)
        {
            repeatOperationId++;
            repeatOperationId %= 10;

            for (var rollIndex = 0; rollIndex < wheel.rollCount; rollIndex++)
            {
                var roll = wheel.GetRoll(rollIndex);
                var rollRowCount = roll.containerCount;

                for (var row = 0; row < rollRowCount; row++)
                {
                    var container = roll.GetContainer(row);

                    if (!container.sequenceElement.config.keepStateWhenStopAllAnimation || force)
                    {
                        if (!container.GetElement().isStaticElement)
                        {
                            container.UpdateAnimationToStatic();   
                        }
                    }

                    container.ShiftSortOrder(false);
                }
            }

            winFrameLayer?.HideAllWinFrame();
            payLineLayer?.HideAllPayLines();
        }
    }
}