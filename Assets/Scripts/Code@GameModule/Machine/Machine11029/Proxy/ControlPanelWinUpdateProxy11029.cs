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
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class ControlPanelWinUpdateProxy11029:ControlPanelWinUpdateProxy
    {
        public ControlPanelWinUpdateProxy11029(MachineContext context)
            : base(context)
        {
        }

        protected override void HandleCustomLogic()
        {
            bool isBag = false;
            bool playWinOutAnim = machineContext.state.Get<WinState>().winLevel < (int)WinLevel.BigWin;
            var winLevel = WinLevel.NoWin;
            
            //判断是否中了bag，当中了bag并且winlevel大于bigwin的时候，赢钱线不需要音效。
            var bagWinLines = new List<WinLine>();
            var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            var bonusWinLines = wheel.wheelState.GetBonusWinLine();
            for (int i = 0; i < bonusWinLines.Count; i++)
            {
                if (bonusWinLines[i].BonusGameId == 4001)
                {
                    bagWinLines.Add(bonusWinLines[i]);
                }
            }
            if (bagWinLines.Count > 0)
            {
                if (bagWinLines[0].Pay > 0 && machineContext.state.Get<WinState>().winLevel >= (int)WinLevel.BigWin)
                {
                    isBag = true;
                }
            }

            if(machineContext.state.Get<FreeSpinState>().freeSpinId == 2 && (machineContext.state.Get<FreeSpinState>().IsInFreeSpin))
            {
                winLevel = AddWinChipsToControlPanel(machineContext.state.Get<WinState>().wheelWin, 0,true, playWinOutAnim,null,true,"Map_Classic_Win");
            }
            else if (isBag)
            {
                winLevel = AddWinChipsToControlPanel(machineContext.state.Get<WinState>().wheelWin, 0.5f,false, playWinOutAnim);
            }
            else
            {
                winLevel = AddWinChipsToControlPanel(machineContext.state.Get<WinState>().wheelWin, 0,true, playWinOutAnim);
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