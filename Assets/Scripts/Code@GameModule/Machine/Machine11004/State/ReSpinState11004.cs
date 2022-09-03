namespace GameModule
{
    public class ReSpinState11004: ReSpinState
    {
        public ReSpinState11004(MachineState state) : base(state)
        {
        }

        public override ulong GetRespinTotalWin()
        {
          return (ulong)machineState.machineContext.state.Get<ExtraState11004>().GetRespinTotalWin();
        }
    }
}