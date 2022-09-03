namespace GameModule
{
    public class ReSpinState11019: ReSpinState
    {
        public ReSpinState11019(MachineState state) : base(state)
        {
        }
        
        public override ulong GetRespinTotalWin()
        {
            return (ulong)machineState.machineContext.state.Get<ExtraState11019>().GetRespinTotalWin();
            //return this.ReSpinTototalWin;
        }
    }
}