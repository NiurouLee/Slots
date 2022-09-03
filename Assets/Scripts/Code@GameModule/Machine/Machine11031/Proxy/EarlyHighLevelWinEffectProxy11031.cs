namespace GameModule
{
    public class EarlyHighLevelWinEffectProxy11031 : EarlyHighLevelWinEffectProxy
    {
        public EarlyHighLevelWinEffectProxy11031(MachineContext context) : base(context)
        {
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            if (machineContext.state.Get<ReSpinState>().IsInRespin && machineContext.state.Get<ReSpinState>().NextIsReSpin)
            {
                return false;
            }

            _extraState = machineContext.state.Get<ExtraState>();

            if (_extraState == null || !_extraState.HasSpecialBonus())
            {
                if (_winState.winLevel >= (int) WinLevel.BigWin)
                    return true;
            }

            return _winState.winLevel >= (int) WinLevel.BigWin;
        }
    }
}