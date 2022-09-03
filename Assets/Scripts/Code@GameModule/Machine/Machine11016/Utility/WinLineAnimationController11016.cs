//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-25 11:46
//  Ver : 1.0.0
//  Description : WinLineAnimationController11016.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class WinLineAnimationController11016:WinLineAnimationController
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
                        if (Constant11016.IsFeaturedSlotElement(container.sequenceElement.config.id))
                        {
                            var element = container.GetElement() as Element11016;
                            if (element.ShiftOrder>0)
                            {
                                element.ShiftMaskAndSortOrder(-element.ShiftOrder);
                            }
                            container.EnableSortingGroup(true);   
                        }
                        container.UpdateAnimationToStatic();
                    }
                    HideTrail(container);
                    container.ShiftSortOrder(false);
                }
            }

            winFrameLayer?.HideAllWinFrame();
            payLineLayer?.HideAllPayLines();
        }
        
        public override void StopElementWinAnimation(uint rollIndex, uint rowIndex)
        {
            base.StopElementWinAnimation(rollIndex, rowIndex);
            HideTrail(rollIndex, rowIndex);
        }

        private void HideTrail(ElementContainer container)
        {
            if (wheel.wheelName.Contains("Free"))
            {
                var element = container.GetElement() as Element11016;
                element.HideTrail();   
            }   
        }

        private void HideTrail(uint rollIndex, uint rowIndex)
        {
            if (wheel.wheelName.Contains("Free"))
            {
                var container = wheel.GetWinLineElementContainer((int) rollIndex, (int) rowIndex);
                var element = container.GetElement() as Element11016;
                element.HideTrail();
            }
        }
    }
}