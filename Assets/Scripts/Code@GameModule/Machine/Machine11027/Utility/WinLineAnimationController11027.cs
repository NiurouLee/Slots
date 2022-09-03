using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WinLineAnimationController11027 : WinLineAnimationController
    {
        public override async Task BlinkWinLineAsync(WinLine winLine)
        {
             wheel.GetContext().state.Get<WheelsActiveState11027>().ShowRollsMasks(wheel);
             await base.BlinkWinLineAsync(winLine);
        }
    }
}