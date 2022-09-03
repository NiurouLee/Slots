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
    public class WheelsActiveState11003: WheelsActiveState
    {
        public WheelsActiveState11003(MachineState machineState)
            : base(machineState)
        {
            spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;
        }

        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            var extraInfo = ProtocolUtils.GetAnyStruct<PiggyBankGameResultExtraInfo>(gameResult.ExtraInfoPb);

            if (!gameResult.IsFreeSpin && !gameResult.IsBonusGame)
            {
                UpdateRunningWheel(new List<string>() {"WheelBaseGame", "WheelBaseJackpot"},false);
                
            }
            else if (extraInfo.SuperFreeSpinInfo.Left > 0)
            {
                var wheelName = machineState.Get<ExtraState11003>().GetSuperFreeWheelName(extraInfo);
                
                UpdateRunningWheel(new List<string>() {wheelName},false);
            }
            else if (gameResult.IsFreeSpin)
            {
                UpdateRunningWheel(new List<string>() {"WheelFreeGame", "WheelFreeJackpot"},false);
            }
        }

        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "WheelFreeGame")
                return "FreeReels";

            if (wheel.wheelName.Contains("Jackpot"))
            {
                return "JackpotReels";
            }

            if (wheel.wheelName.Contains("Super"))
            {
                var extraState = machineState.Get<ExtraState11003>();
                if (extraState.IsAddSymbolsBuffer())
                {
                    return "ExtraSuperFreeReels";
                }

                return "SuperFreeReels";
            }

            return "Reels";
        }
    }
}