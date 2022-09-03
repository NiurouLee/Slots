namespace GameModule
{
    public class EarlyHighLevelWinEffectProxy11013 : EarlyHighLevelWinEffectProxy
    {
        public EarlyHighLevelWinEffectProxy11013(MachineContext context) : base(context)
        {
        }

        protected async override void HandleCustomLogic()
        {
            await WinEffectHelper.ShowBigWinEffectAsync(_winState.winLevel, _winState.displayCurrentWin,
                machineContext);


            Proceed();
        }
    }
}