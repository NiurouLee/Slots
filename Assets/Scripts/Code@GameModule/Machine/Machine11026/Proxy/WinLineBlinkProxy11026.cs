using System.Threading.Tasks;

namespace GameModule
{
    public class WinLineBlinkProxy11026: WinLineBlinkProxy
    {
        public WinLineBlinkProxy11026(MachineContext context) : base(context)
        {
        }

        protected override async void HandleCustomLogic()
        {
            // if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            // {
            //     var bonusWinView = machineContext.view.Get<BonusWinView11019>(Constant11019.FreeBonusWinViewName);
            //     var winState = machineContext.state.Get<WinState>();
            //     bonusWinView.RefreshWinAsync((long)winState.displayTotalWin,false);
            //
            //     
            // }
            base.HandleCustomLogic();
        }
    }
}