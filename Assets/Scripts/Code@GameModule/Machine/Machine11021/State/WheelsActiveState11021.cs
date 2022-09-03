using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
    public class WheelsActiveState11021 : WheelsActiveState
    {
        public WheelsActiveState11021(MachineState machineState)
            :base(machineState)
        {

	
        }
		
		
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            UpdateRunningWheelState(gameResult.IsFreeSpin,false);
        }


        public  void UpdateRunningWheelState(bool isFree,bool updateReelSequence = true)
        {
            UpdateRunningWheel(new List<string>() {"WheelBaseGame"},updateReelSequence);
            var titlePrizeView = machineState.machineContext.view.Get<TitlePrizeView>();
            titlePrizeView.ChangeListEnableState(isFree);
            titlePrizeView.RefreshInfo(isFree);
            var wheel = GetRunningWheel()[0].transform;
            if(isFree)
            {
                
            }
            else
            {
            }
            
			
        }
        

        public override string GetReelNameForWheel(Wheel wheel)
        {

            if (!machineState.machineContext.state.Get<FreeSpinState>().IsOver)
            {
                return "FreeReels";
            }


            return "Reels";
        }

    }
}