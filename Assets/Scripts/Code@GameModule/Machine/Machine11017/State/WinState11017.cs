//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-30 17:26
//  Ver : 1.0.0
//  Description : WinState11011.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WinState11017:WinState
    {
        public ulong totalElimateWin;
        public ulong totalCrystalWin;
        public WinState11017(MachineState machineState) : base(machineState)
        {
            
        }
        
         public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);

            var panels = spinResult.GameResult.Panels;
            
            ulong totalWinRate = 0;
            for (var i = 0; i < panels.Count; i++)
            {
                totalWinRate += spinResult.GameResult.Panels[i].WinRate;
                var winLines = spinResult.GameResult.Panels[i].WinLines;
                for (var l = 0; l < winLines.Count; l++)
                {
                    if (winLines[l].JackpotId > 0)
                    {
                        totalWinRate -= winLines[l].Pay;
                    }
                    else if (winLines[l].BonusGameId >= 1000)
                    {
                        totalWinRate -= winLines[l].Pay;
                    }
                }
            }
            
            //当消除时totalwin取panels[0].WinRate * spinResult.GameResult.Bet / 100
            if (panels.Count > 1)
            {
                wheelWin = spinResult.GameResult.DisplayCurrentWin - panels[panels.Count - 1].WinRate * spinResult.GameResult.Bet / 100;
                totalElimateWin = panels[panels.Count - 1].WinRate * spinResult.GameResult.Bet / 100;
            }
            else
            {
                //只有当respin为true的时候，才记录totalCrystalWin的值。
                if (spinResult.GameResult.IsReSpin)
                {
                    totalCrystalWin = panels[0].WinRate * spinResult.GameResult.Bet / 100;
                }
                else
                {
                    totalCrystalWin = 0;
                }
                wheelWin =  totalWinRate * spinResult.GameResult.Bet / 100;
            }

            displayCurrentWin = spinResult.GameResult.DisplayCurrentWin;
            displayTotalWin = spinResult.GameResult.DisplayTotalWin;
            totalWin = spinResult.GameResult.TotalWin;
            winLevel = spinResult.GameResult.WinLevel;
            
            currentWin = displayTotalWin - displayCurrentWin;
            
            
            //Debug.LogError($"=======Server currentWin:{currentWin}");
            
#if !PRODUCTION_PACKAGE       
            XDebug.Log("Server:UpdateStateOnReceiveSpinResult\n" 
                       + $"Server:DisplayCurrentWin:{displayCurrentWin}\n"
                       + $"Server:DisplayTotalWin:{displayTotalWin}\n"
                       + $"Server:WinLevel:{winLevel}\n"
                       + $"Server:TotalWin:{totalWin}\n"
                       + $"wheelWin:{wheelWin}");
 #endif
        }

         public ulong GetWheelWin()
         {
             return wheelWin;
         }
    }
    
}