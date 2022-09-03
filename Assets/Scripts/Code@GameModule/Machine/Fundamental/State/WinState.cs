// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/17:29
// Ver : 1.0.0
// Description : WinState.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public enum WinLevel
    {
        NoWin = 0,
        SmallWin = 1,
        Win = 2,
        NiceWin = 3,
        BigWin = 4,
        HugeWin = 5,
        MassiveWin = 6,
        ColossalWin = 7,
        ApocalypticWin = 8,
    }
    public class WinState:SubState
    {
        //panel的总赢钱
        public ulong wheelWin;

        //一局结束向玩家balance上加的钱
        public ulong totalWin;
        
        public float finalWinRate;
        
        public uint winLevel;
        /// <summary>
        /// control panel上当前这次spin的赢钱
        /// </summary>
        public ulong displayCurrentWin;
        
        /// <summary>
        /// control panel 从触发特殊玩法到目前的累计赢钱
        /// </summary>
        public ulong displayTotalWin;
        
        /// <summary>
        /// 客户端表现当前累计加的钱
        /// </summary>
        public ulong currentWin;
        
        public WinState(MachineState machineState) : base(machineState)
        {
            
        }

        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
           
            displayCurrentWin = gameEnterInfo.GameResult.DisplayCurrentWin;
            displayTotalWin = gameEnterInfo.GameResult.DisplayTotalWin;
            winLevel = gameEnterInfo.GameResult.WinLevel;
            currentWin = displayTotalWin;

#if !PRODUCTION_PACKAGE       
           XDebug.Log("Server:UpdateStateOnRoomSetUp\n" 
                           + $"Server:DisplayCurrentWin:{displayCurrentWin}\n"
                           + $"Server:DisplayTotalWin:{displayTotalWin}\n"
                           + $"Server:WinLevel:{winLevel}\n"
                           + $"Server:TotalWin:{totalWin}");
#endif
        }

        public void AddCurrentWin(ulong inCurrentWin)
        {
            //Debug.LogError($"==========currentLastWin:{currentWin} inCurrentWin:{inCurrentWin}");
            currentWin += inCurrentWin;
            //Debug.LogError($"++++++++++currentWin:{currentWin}");
        }
        
        public void SetCurrentWin(ulong inCurrentWin)
        {
            currentWin = inCurrentWin;
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
            
            wheelWin =  totalWinRate * spinResult.GameResult.Bet / 100;

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

        public override void UpdateStateOnRoundStart()
        {
            currentWin = 0;
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess settleProcess)
        {
            displayCurrentWin = settleProcess.GameResult.DisplayCurrentWin;
            displayTotalWin = settleProcess.GameResult.DisplayTotalWin;
            totalWin = settleProcess.GameResult.TotalWin;
            winLevel = settleProcess.GameResult.WinLevel;
            
#if !PRODUCTION_PACKAGE       
            XDebug.Log("Server:UpdateStateOnBonusProcess\n" 
                       + $"Server:DisplayCurrentWin:{displayCurrentWin}\n"
                       + $"Server:DisplayTotalWin:{displayTotalWin}\n"
                       + $"Server:WinLevel:{winLevel}\n"
                       + $"Server:TotalWin:{totalWin}");
#endif
        }
        public override void UpdateStateOnSpecialProcess(SSpecialProcess specialProcess)
        {
            displayCurrentWin = specialProcess.GameResult.DisplayCurrentWin;
            displayTotalWin = specialProcess.GameResult.DisplayTotalWin;
            totalWin = specialProcess.GameResult.TotalWin;
            winLevel = specialProcess.GameResult.WinLevel;
            
#if !PRODUCTION_PACKAGE       
            XDebug.Log("Server:UpdateStateOnBonusProcess\n" 
                       + $"Server:DisplayCurrentWin:{displayCurrentWin}\n"
                       + $"Server:DisplayTotalWin:{displayTotalWin}\n"
                       + $"Server:WinLevel:{winLevel}\n"
                       + $"Server:TotalWin:{totalWin}");
#endif
        }
        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            displayCurrentWin = settleProcess.GameResult.DisplayCurrentWin;
            displayTotalWin = settleProcess.GameResult.DisplayTotalWin;
            totalWin = settleProcess.GameResult.TotalWin;
            winLevel = settleProcess.GameResult.WinLevel;
            //currentWin = displayTotalWin;
            
#if !PRODUCTION_PACKAGE    
            XDebug.Log("Server:UpdateStateOnSettleProcess\n" 
                       + $"Server:DisplayCurrentWin:{displayCurrentWin}\n"
                       + $"Server:DisplayTotalWin:{displayTotalWin}\n"
                       + $"Server:WinLevel:{winLevel}\n"
                       + $"Server:TotalWin:{totalWin}");
#endif
        }
    }
}