namespace GameModule
{
    public class RoundFinishProxy11022:RoundFinishProxy
    {
        public RoundFinishProxy11022(MachineContext context)
            : base(context)
        {
            
        }
        protected override void HandleCommonLogic()
        {
            machineContext.state.UpdateStateBeforeCallRoundFinish();
            
            machineContext.state.UpdateStateOnRoundFinish();

            var autoSpinState = machineContext.state.Get<AutoSpinState>();
             
            
            if (autoSpinState.IsAutoSpin)
            {
                machineContext.view.Get<ControlPanel>()
                    .UpdateAutoSpinLeftCount(machineContext.state.Get<AutoSpinState>().AutoLeftCount);
            }
            
            //AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());

            UpdateBalance();

            var winState = machineContext.state.Get<WinState>();
            var betState = machineContext.state.Get<BetState>();

            if (!Constant11022.debugType)
            {
                if (winState.totalWin > 0)
                {
                    EventBus.Dispatch(
                        new EventSpinRoundEnd((float) winState.totalWin / betState.totalBet, winState.totalWin,
                            winState.winLevel, machineContext.state.Get<AdStrategyState>().GetAdStrategy()),
                        () => { XDebug.Log("SystemLogicProcessFinished"); }
                    );
                }
                else
                {
                    EventBus.Dispatch(new EventSpinRoundEnd(0, 0, 0),
                        () => { XDebug.Log("SystemLogicProcessFinished"); }
                    );
                }   
            }
        }
    }
}