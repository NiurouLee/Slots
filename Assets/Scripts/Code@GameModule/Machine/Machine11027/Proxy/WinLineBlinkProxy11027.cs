namespace GameModule
{
    public class WinLineBlinkProxy11027: WinLineBlinkProxy
    {
        public WinLineBlinkProxy11027(MachineContext context) : base(context)
        {
        }
        
        protected override void HandleCommonLogic()
        {
            var wheels = wheelsRunningStatusState.GetRunningWheel();

            if (wheels != null && wheels.Count > 0)
            {
                machineContext.AddWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                for (var i = 0; i < wheels.Count; i++)
                {
                    wheels[i].winLineAnimationController.BlinkAllWinLine(() =>
                    {
                        machineContext.RemoveWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                    });
                }

                CheckAndShowFiveOfKindAnimation(wheels);
            }
        }
    }
}