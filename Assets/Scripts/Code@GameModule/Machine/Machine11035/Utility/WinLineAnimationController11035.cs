using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WinLineAnimationController11035 : WinLineAnimationController
    {
        public override void PlayElementWinAnimation(uint rollIndex, uint rowIndex, bool showWinFrame = true)
        {
            return;
        }

        public override void BlinkWinLine(WinLine winLine)
        {
            return;
        }

        public override Task BlinkWinLineAsync(WinLine winLine)
        {
            return Task.Delay(2000);
        }
    }
}