namespace GameModule
{
    public class WinLineBlinkProxy11020: WinLineBlinkProxy
    {
        public WinLineBlinkProxy11020(MachineContext context) : base(context)
        {
           
        }

        protected override void HandleCommonLogic()
        {
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (freeSpinState.IsTriggerFreeSpin || freeSpinState.NewCount > 0)
            {
                AudioUtil.Instance.PlayAudioFx("Fire_Scatter_Trigger");
            }

            base.HandleCommonLogic();
        }
    }
}
