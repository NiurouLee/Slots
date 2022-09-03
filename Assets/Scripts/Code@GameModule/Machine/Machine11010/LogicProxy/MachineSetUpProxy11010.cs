//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-09 15:19
//  Ver : 1.0.0
//  Description : MachineSetUpProxy11010.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class MachineSetUpProxy11010:MachineSetUpProxy
    {
        public MachineSetUpProxy11010(MachineContext context)
            : base(context)
        {
            
        }

        protected override void PrepareElement(SEnterGame gameEnterInfo)
        {
            base.PrepareElement(gameEnterInfo);
            var elementConfigSet = machineContext.machineConfig.GetElementConfigSet();
            for (int i = 0; i < elementConfigSet.elementNum; i++)
            {
                elementConfigSet.GetElementConfigByIndex(i).createBigElementParts = true;
            }
        }
        
        protected override void UpdateViewWhenRoomSetUp()
        {
            UpdateRunningWheelElement();
            UpdateControlPanelBet();
            UpdateControlPanelWinChips();
            
            var reSpinState = machineContext.state.Get<ReSpinState>();
            if (reSpinState != null && reSpinState.NextIsReSpin)
            {
                UpdateLinkJackpotWinChips();
            }
            else
            {
                UpdateControlPanelState();
            }
        }
        protected void UpdateLinkJackpotWinChips()
        {
            var winState = machineContext.state.Get<WinState>();
            var controlPanel = machineContext.view.Get<ControlPanel>();
            var extraState11010 = machineContext.state.Get<ExtraState11010>();
            controlPanel.UpdateWinLabelChips((long)(extraState11010.GetJackpotsWinChips() + winState.displayTotalWin));
        }
    }
}