namespace GameModule
{
    public class SingleWheelAnimationController11022:WheelAnimationController
    {
        public override void PlayBlinkSound(ElementContainer container, int rollIndex, int rowIndex)
        {
            // AudioUtil.Instance.PlayAudioFx(GetBlinkSoundName(container, rollIndex, rollIndex));
        }
        public override void PlayReelStop(int rollIndex = -1)
        {
            // // Debug.Log("needPlayReelStop" + needPlayReelStop + "||" + EarlyStopTriggered);
            // if (!wheel.wheelState.playerQuickStopped || rollIndex < 0)
            // {
            //     var reelStopSoundName = wheel.wheelState.GetEasingConfig().GetReelStopSoundName();
            //     AudioUtil.Instance.PlayAudioFxOneShot(reelStopSoundName);
            // }
        }
    }
}