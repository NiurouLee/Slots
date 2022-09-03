namespace GameModule
{
    public class LateHighLevelWinEffectProxy11021:LateHighLevelWinEffectProxy
    {
        public LateHighLevelWinEffectProxy11021(MachineContext context) : base(context)
        {
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.HasSpecialBonus())
            {
                if (_winState.winLevel >= (int)WinLevel.BigWin)
                    return true;
            }

            return false;
        }
    }
}