using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
namespace GameModule{
    public class BetState11301 : BetState
    {
        public BetState11301(MachineState state) : base(state)
        {
        }

        public void UpdateTotalBetForState()
        {
            var freeSpin = machineState.Get<FreeSpinState>();
            var reSpin = machineState.Get<ReSpinState11301>();
            
            if (freeSpin.IsInFreeSpin)
            {
                var freeTotalBet = freeSpin.freeSpinBet;
                SetTotalBet(freeTotalBet);
            }

            if (reSpin.NextIsReSpin)
            {
                var reSpinTotalBet = reSpin.ReSpinBet;
                //
                // var betLevel = betList.IndexOf(totalBets);
                // if (betLevel == -1)
                // {
                //     totalBet = totalBets;
                //     XDebug.Log("betstate-设置:"+totalBet);
                //     return;
                // }
                
                SetTotalBet(reSpinTotalBet);
            }
        }
        // public bool SetBetLevels(int inBetLevel)
        // {
        //     
        //     if (inBetLevel < betList.Count && inBetLevel!=-1)
        //     {
        //         betLevel = inBetLevel;
        //         totalBet = betList[betLevel];
        //
        //         var machineId = machineState.machineContext.assetProvider.MachineId;
        //         machineState.machineContext.serviceProvider.StoreRecommendBetLevel(totalBet, machineId);
        //         return true;
        //     }
        //     return false;
        // }
    }
}
