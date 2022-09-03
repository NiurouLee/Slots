using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
namespace GameModule{
    public class BetState11312 : BetState
    {
        public int BetLevel;
        public BetState11312(MachineState state) : base(state)
        {
        }
        public List<ulong> GetCurrentUnlockBetConfig(){
            return currentUnlockBetConfig;
        }
        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStatePreRoomSetUp(gameEnterInfo);
            BetLevel = betLevel;
        }
    }
}

