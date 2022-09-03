namespace GameModule
{
    public class WinLineAnimationController11015 : WinLineAnimationController
    {
        public override void StopElementWinAnimation(uint rollIndex, uint rowIndex)
        {
            var container = wheel.GetWinLineElementContainer((int)rollIndex, (int)rowIndex);
            var element = container.GetElement();
            var elementZeus = element as ElementZeus11015;
            if (elementZeus != null && elementZeus.IsLock)
            {
                container.PlayElementAnimation("LockIdle");
            }
            else
            {
                base.StopElementWinAnimation(rollIndex, rowIndex);
            }

            
        }


        
    }
}