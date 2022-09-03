namespace GameModule
{
    public class SubRoundFinishProxy11301: SubRoundFinishProxy
    {
        public SubRoundFinishProxy11301(MachineContext context) : base(context)
        {
        }


        protected async override void HandleCustomLogic()
        {
            var freeSpin = machineContext.state.Get<FreeSpinState>();

            if (freeSpin.IsInFreeSpin)
            {
                var winState = machineContext.state.Get<WinState>();
                if (winState.displayCurrentWin == 0)
                {
                    await machineContext.WaitSeconds(0.5f);
                }
            }

            Constant11301.IsSpining = false;

            base.HandleCustomLogic();
        }
    }
}