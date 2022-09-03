namespace GameModule
{
    public class EarlyHighLevelWinEffectProxy11021:EarlyHighLevelWinEffectProxy
    {
        public EarlyHighLevelWinEffectProxy11021(MachineContext context) : base(context)
        {
        }


        public override bool CheckCurrentStepHasLogicToHandle()
        {
            _extraState = machineContext.state.Get<ExtraState>();
            if (_extraState != null && _extraState.HasSpecialBonus())
            {
                return false;
            }

            return _winState.winLevel >= (int) WinLevel.BigWin;
        }
    }
}