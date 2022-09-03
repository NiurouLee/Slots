namespace GameModule
{
    public class ReSpinState11022:ReSpinState
    {
        public ReSpinState11022(MachineState state) : base(state)
        {
            
        }
        public override ulong GetRespinTotalWin()
        {
            var chips = machineState.Get<BetState>().GetPayWinChips(triggerPanels[0].WinRate);
            var linkWin = ReSpinTototalWin - chips;
            return linkWin;
            // var winState = machineState.Get<WinState>();
            // return winState.currentWin;
        }   
    }
}