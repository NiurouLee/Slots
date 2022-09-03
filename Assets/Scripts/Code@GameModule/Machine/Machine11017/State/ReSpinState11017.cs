namespace GameModule
{
    public class ReSpinState11017: ReSpinState
    {
        public ReSpinState11017(MachineState state) : base(state)
        {
        }
        
        public override ulong GetRespinTotalWin()
        {
            return ReSpinTototalWin;
        }
    }
}