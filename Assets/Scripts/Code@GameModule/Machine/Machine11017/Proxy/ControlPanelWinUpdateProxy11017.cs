//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-21 16:23
//  Ver : 1.0.0
//  Description : ControlPanelWinUpdateProxy11028.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class ControlPanelWinUpdateProxy11017:ControlPanelWinUpdateProxy
    {
        public ControlPanelWinUpdateProxy11017(MachineContext context)
            : base(context)
        {
        }

        protected override void HandleCustomLogic()
        {
            bool playWinOutAnim = machineContext.state.Get<WinState>().winLevel < (int)WinLevel.BigWin;
            var _extraState = machineContext.state.Get<ExtraState>();
            if (_extraState.HasSpecialBonus() || machineContext.state.Get<ReSpinState>().IsInRespin)
            {
                playWinOutAnim = false;
            }
            var winLevel = WinLevel.NiceWin;
            if (_extraState.HasSpecialBonus())
            {
                 winLevel = AddWinChipsToControlPanel(machineContext.state.Get<WinState11017>().GetWheelWin(), 1, true,playWinOutAnim,"Symbol_SmallWin_11017");
            }
            else
            {
                winLevel = AddWinChipsToControlPanel(machineContext.state.Get<WinState11017>().GetWheelWin(), 0,true, playWinOutAnim);
            }
            if (winLevel == WinLevel.NiceWin && machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime() > 0)
            {
                if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin ||
                    !machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    machineContext.view.Get<ControlPanel>().ShowStopButton(true);
            }
            else
            {
                Proceed();
            }
        }
    }
}