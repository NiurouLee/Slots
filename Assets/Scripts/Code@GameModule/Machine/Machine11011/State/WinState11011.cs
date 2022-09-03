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
    public class WinState11011:WinState
    {
        public WinState11011(MachineState machineState) : base(machineState)
        {
            
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            
            var isInFreeSpin = spinResult.GameResult.IsFreeSpin;
            if (isInFreeSpin && !(spinResult.GameResult.FreeSpinInfo.FreeSpinTotalCount != 0 &&
                spinResult.GameResult.FreeSpinInfo.FreeSpinCount > 0))
            {
                RisingFortuneGameResultExtraInfo extraInfo =  ProtocolUtils.GetAnyStruct<RisingFortuneGameResultExtraInfo>(spinResult.GameResult.ExtraInfoPb);
                currentWin = displayTotalWin - displayCurrentWin - machineState.Get<BetState>().GetPayWinChips(extraInfo.FreeGameCoinTotalWinRate);
            }

        }
    }
}