using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
namespace GameModule{
    public class FreeSpinState11301 : FreeSpinState
    {
        public bool IsAvgBet;
        public FreeSpinState11301(MachineState state) : base(state)
        {
        }
        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStatePreRoomSetUp(gameEnterInfo);
            IsAvgBet = gameEnterInfo.GameResult.FreeSpinInfo.IsAvgBet;
        }
 

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            IsAvgBet = spinResult.GameResult.FreeSpinInfo.IsAvgBet;
        }
        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            base.UpdateStateOnBonusProcess(sBonusProcess);
            IsAvgBet = sBonusProcess.GameResult.FreeSpinInfo.IsAvgBet;
        }
    }
}

