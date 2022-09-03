namespace GameModule
{
    public class EarlyHighLevelWinEffectProxy11017: EarlyHighLevelWinEffectProxy
    {
        public EarlyHighLevelWinEffectProxy11017(MachineContext context) : base(context)
        {
        }
        
        public override bool CheckCurrentStepHasLogicToHandle()
        {
            _extraState = machineContext.state.Get<ExtraState>();
            if (_extraState.HasSpecialBonus() || machineContext.state.Get<ReSpinState>().IsInRespin)
            {
                return false;
            }
            if (_extraState == null || !_extraState.HasSpecialBonus())
            {
                if(_winState.winLevel >= (int)WinLevel.BigWin)
                    return true;
            }
            return _winState.winLevel >= (int)WinLevel.BigWin;
        }
    }
}