using System.Threading.Tasks;

namespace GameModule
{
    public class WinLineAnimationController11004: WinLineAnimationController
    {
        public override async Task BlinkFreeSpinTriggerLine()
        {
            await AudioUtil.Instance.PlayAudioFxAsync("B01_BeforeTrigger");
            await base.BlinkFreeSpinTriggerLine();
        }
    }
}