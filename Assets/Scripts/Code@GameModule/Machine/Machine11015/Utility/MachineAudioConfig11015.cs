namespace GameModule
{
    public class MachineAudioConfig11015: MachineAudioConfig
    {

        protected MachineContext machineContext;
        public MachineAudioConfig11015(string inPostFix,MachineContext context) : base(inPostFix)
        {
            machineContext = context;
        }

        public override string GetLinkBackgroundMusicName()
        {
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (freeSpinState.IsInFreeSpin)
            {
                return GetFreeBackgroundMusicName();
            }
            else
            {
                return GetBaseBackgroundMusicName();
            }
        }
    }
}