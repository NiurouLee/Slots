namespace GameModule
{
    public class ReSpinState11031: ReSpinState
    {
        public ReSpinState11031(MachineState state) : base(state)
        {
        }
        
        public override ulong GetRespinTotalWin()
        {
            return ReSpinTototalWin;
        }
    }
}