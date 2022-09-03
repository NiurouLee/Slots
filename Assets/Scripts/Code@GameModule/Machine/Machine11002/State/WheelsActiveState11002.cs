// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/23/16:45
// Ver : 1.0.0
// Description : WheelsActiveState11003.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelsActiveState11002 : WheelsActiveState
    {
        public WheelsActiveState11002(MachineState machineState)
            : base(machineState)
        {
            spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;
        }

        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            if (!gameResult.IsFreeSpin && !gameResult.IsBonusGame)
            {
                UpdateRunningWheel(new List<string>() { "Wheel0" }, true);
            }
            else if (gameResult.IsFreeSpin)
            {
                UpdateRunningWheel(new List<string>() { "Wheel1" }, true);
            }
        }

        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "Wheel0")
                return "Reels";
            return "FreeReels";
        }
    }
}