namespace GameModule
{
    public class WinLineAnimationController11013: WinLineAnimationController
    {
        
        
        public override void PlayElementWinAnimation(uint rollIndex, uint rowIndex, bool showWinFrame=true)
        {
            var elementContainer = wheel.GetWinLineElementContainer((int)rollIndex, (int)rowIndex);
           
            winFrameLayer?.ShowWinFrame(elementContainer);
            var lockView = wheel.GetContext().view.Get<LockView11013>();
            lockView.PlayElementAnim((int)rowIndex, (int)rollIndex, "Win");
            
            var element = elementContainer.GetElement();
            if (element!=null && !element.HasAnimState("Win"))
            {
                return;
            }
            
            if (!elementContainer.doneTag)
            {
                elementContainer.ShiftSortOrder(true);
                elementContainer.PlayElementAnimation("Win");
                
                elementContainer.doneTag = true;
            }
        }
        
        
        public override void StopElementWinAnimation(uint rollIndex, uint rowIndex)
        {
            var container = wheel.GetWinLineElementContainer((int)rollIndex, (int)rowIndex);

            var lockView = wheel.GetContext().view.Get<LockView11013>();
            lockView.PlayElementAnim((int)rowIndex, (int)rollIndex, "Normal");
            
            var element = container.GetElement();
            if (element!=null && !element.HasAnimState("Win"))
            {
                return;
            }

            if (container.sequenceElement.config.id == Constant11013.StarElement)
            {
                return;
            }

            if (!container.doneTag)
            {
                //  symbolContainerView.ToggleAnimationShareInstance(false);
                container.UpdateAnimationToStatic();
                container.ShiftSortOrder(false);
                //   panelView.ReelViews[col].UpdateContainerWinSortingOrder(symbolContainerView, false);
                container.doneTag = true;
                
               
            }
        }
        
        
         
    }
}