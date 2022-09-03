// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/14:59
// Ver : 1.0.0
// Description : JackpotState.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class JackpotWinInfo
    {
        public uint jackpotId;
        public ulong jackpotPay;

        public JackpotWinInfo(uint inJackpotId, ulong inJackpotPay)
        {
            jackpotId = inJackpotId;
            jackpotPay = inJackpotPay;
        }
    }
    public class JackpotInfoState:SubState
    {
        protected Dictionary<uint, Jackpot> jackpotDict;

        protected JackpotWinInfo jackpotWinInfo;

        protected bool serverLockedJackpot = false;
        public bool LockJackpot { get; set; } = false;

        public JackpotInfoState(MachineState machineState)
        :base(machineState)
        {
            jackpotDict = new Dictionary<uint, Jackpot>();
        }
        
        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
           base.UpdateStateOnRoomSetUp(gameEnterInfo);

           serverLockedJackpot = gameEnterInfo.GameResult.IsJackpotStatic;
           XDebug.Log("serverLockedJackpot UpdateStateOnRoomSetUp "+serverLockedJackpot);
            var jackpotConfigs = gameEnterInfo.GameConfigs[0].JackpotConfigs;
         
            if (jackpotConfigs != null && jackpotConfigs.Count > 0)
            {
                for (var i = 0; i < jackpotConfigs.Count; i++)
                { 
                    Jackpot jackpot = new Jackpot(jackpotConfigs[i], gameEnterInfo.GameResult.JackpotStates[i]);
                    jackpotDict[jackpotConfigs[i].JackpotId] = jackpot;
                }
            }
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            var jackpotStates = spinResult.GameResult.JackpotStates;
            if (jackpotStates.Count > 0)
            {
                for (var i = 0; i < jackpotStates.Count; i++)
                {
                    jackpotDict[jackpotStates[i].JackpotId].SyncJackpotState(jackpotStates[i]);
                }
            }

            serverLockedJackpot = spinResult.GameResult.IsJackpotStatic;
            XDebug.Log("serverLockedJackpot UpdateStateOnReceiveSpinResult "+serverLockedJackpot);
            jackpotWinInfo = null;
            
            if (spinResult.GameResult.JackpotPay > 0)
            {
                jackpotWinInfo = new JackpotWinInfo(spinResult.GameResult.JackpotId, spinResult.GameResult.JackpotPay);
            }
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            var jackpotStates = sBonusProcess.GameResult.JackpotStates;
            if (jackpotStates.Count > 0)
            {
                for (var i = 0; i < jackpotStates.Count; i++)
                {
                    jackpotDict[jackpotStates[i].JackpotId].SyncJackpotState(jackpotStates[i]);
                }
            }

            serverLockedJackpot = sBonusProcess.GameResult.IsJackpotStatic;
            XDebug.Log("serverLockedJackpot UpdateStateOnBonusProcess "+serverLockedJackpot);
            jackpotWinInfo = null;
            
            if (sBonusProcess.GameResult.JackpotPay > 0)
            {
                jackpotWinInfo = new JackpotWinInfo(sBonusProcess.GameResult.JackpotId, sBonusProcess.GameResult.JackpotPay);
            }
        }
        
        public override void UpdateStateOnSpecialProcess(SSpecialProcess specialProcess)
        {
            var jackpotStates = specialProcess.GameResult.JackpotStates;
            if (jackpotStates.Count > 0)
            {
                for (var i = 0; i < jackpotStates.Count; i++)
                {
                    jackpotDict[jackpotStates[i].JackpotId].SyncJackpotState(jackpotStates[i]);
                }
            }

            serverLockedJackpot = specialProcess.GameResult.IsJackpotStatic;
            XDebug.Log("serverLockedJackpot UpdateStateOnSpecialProcess "+serverLockedJackpot);
            jackpotWinInfo = null;
            
            if (specialProcess.GameResult.JackpotPay > 0)
            {
                jackpotWinInfo = new JackpotWinInfo(specialProcess.GameResult.JackpotId, specialProcess.GameResult.JackpotPay);
            }
        }


        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            var jackpotStates = settleProcess.GameResult.JackpotStates;
            if (jackpotStates.Count > 0)
            {
                for (var i = 0; i < jackpotStates.Count; i++)
                {
                    jackpotDict[jackpotStates[i].JackpotId].SyncJackpotState(jackpotStates[i]);
                }
            }

            serverLockedJackpot = settleProcess.GameResult.IsJackpotStatic;
            XDebug.Log("serverLockedJackpot UpdateStateOnSettleProcess "+serverLockedJackpot);
            jackpotWinInfo = null;
            
            if (settleProcess.GameResult.JackpotPay > 0)
            {
                jackpotWinInfo = new JackpotWinInfo(settleProcess.GameResult.JackpotId, settleProcess.GameResult.JackpotPay);
            }
        }

        public bool HasJackpotWin()
        {
            return jackpotWinInfo != null;
        }

        public JackpotWinInfo GetJackpotWinInfo()
        {
            return jackpotWinInfo;
        }
        
        public ulong GetJackpotValue(uint jackpotId)
        {
           ulong totalBet = machineState.Get<BetState>().totalBet;

           ulong jackpotValue = totalBet;
           
           if (jackpotDict.TryGetValue(jackpotId, out Jackpot jackpot))
           {
               if (LockJackpot && serverLockedJackpot)
                   return jackpot.GetLastServerJackpotValue(totalBet);
               
               jackpotValue = jackpot.GetJackpotValue(totalBet);
           }
           
           return jackpotValue;
        }

        public override void UpdateStateOnSubRoundFinish()
        {
            jackpotWinInfo = null;
        }
        
        
        //===================added by james====================
        //某些信息被服务器放到了jackpotConfig中,例如jackpot解锁的bet数据
        public Jackpot GetJackpotInfo(uint jackpotId)
        {
            jackpotDict.TryGetValue(jackpotId, out var ret);
            return ret;
        }
        //===================added by james====================
    }
}
