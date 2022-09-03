//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 11:08
//  Ver : 1.0.0
//  Description : MachineSetUpProxy11016.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class MachineSetUpProxy11016:MachineSetUpProxy
    {
        public MachineSetUpProxy11016(MachineContext context)
            : base(context)
        {
            
        }
        
        protected override void UpdateViewWhenRoomSetUp()
        {
            var extraState = machineContext.state.Get<ExtraState11016>();
            if (extraState.HasBonusGame())
            {
                var winState = machineContext.state.Get<WinState>();
                for (int i = 0; i < extraState.MiniProgress; i++)
                {
                    var panel = extraState.MiniPanels[i];
                    winState.currentWin -= machineContext.state.Get<BetState>().GetPayWinChips(panel.WinRate);
                    winState.displayTotalWin -= machineContext.state.Get<BetState>().GetPayWinChips(panel.WinRate);
                    winState.displayCurrentWin -= machineContext.state.Get<BetState>().GetPayWinChips(panel.WinRate);
                }
            }
            base.UpdateViewWhenRoomSetUp();
        }
    }
}