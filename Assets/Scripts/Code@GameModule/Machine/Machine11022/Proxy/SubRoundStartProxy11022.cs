namespace GameModule
{
    public class SubRoundStartProxy11022:SubRoundStartProxy
    {
        private ExtraState11022 extraState;
        public SubRoundStartProxy11022(MachineContext context)
            : base(context)
        {
            extraState = context.state.Get<ExtraState11022>();
        }
        protected override void PlayBgMusic()
        {
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState11022>();
            //Debug.LogError($"=======NextIsReSpin:{machineContext.state.Get<ReSpinState>().NextIsReSpin}");
            if (wheelsActiveState.IsLinkWheel || wheelsActiveState.IsSingleWheel)
            {
                //Debug.LogError("====play");
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
            }
            else if (wheelsActiveState.IsFreeWheel)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
            }
            else
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
            }
        }
    }
}