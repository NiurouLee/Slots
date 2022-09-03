namespace GameModule
{
    public class SubRoundFinishProxy11012: SubRoundFinishProxy
    {
        public SubRoundFinishProxy11012(MachineContext context) : base(context)
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

            

            base.HandleCustomLogic();
        }
    }
}