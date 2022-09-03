using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
	public class ReSpinState11301 : ReSpinState
	{
		public bool IsAvgBet;
		public ulong ReSpinBet;
		public ReSpinState11301(MachineState state)
		:base(state)
		{

	
		}
		public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStatePreRoomSetUp(gameEnterInfo);
			IsAvgBet = gameEnterInfo.GameResult.ReSpinInfo.IsAvgBet;
			ReSpinBet = gameEnterInfo.GameResult.ReSpinInfo.ReSpinBet;
        }
		public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
			IsAvgBet = spinResult.GameResult.ReSpinInfo.IsAvgBet;
			ReSpinBet = spinResult.GameResult.ReSpinInfo.ReSpinBet;
        }
		public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            base.UpdateStateOnBonusProcess(sBonusProcess);
			IsAvgBet = sBonusProcess.GameResult.ReSpinInfo.IsAvgBet;
			ReSpinBet = sBonusProcess.GameResult.ReSpinInfo.ReSpinBet;
        }
		public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            base.UpdateStateOnSettleProcess(settleProcess);
			ReSpinBet = settleProcess.GameResult.ReSpinInfo.ReSpinBet;
        }
		public override ulong GetRespinTotalWin()
		{
			return (ulong)machineState.machineContext.state.Get<ExtraState11301>().GetRespinTotalWin();
			//return this.ReSpinTototalWin;
		}

	}
}