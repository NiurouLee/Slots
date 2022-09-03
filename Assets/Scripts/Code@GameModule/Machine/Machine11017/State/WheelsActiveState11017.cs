using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
    public class WheelsActiveState11017 : WheelsActiveState
    {
        public WheelsActiveState11017(MachineState machineState)
        :base(machineState)
        {

    
        }
        
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            UpdateRunningWheelState(gameResult.IsReSpin,gameResult.IsFreeSpin,false);
        }
        
        public bool IsInLink
		{
			get;
			protected set;
		}
        
        public  void UpdateRunningWheelState(bool isLink,bool isFree,bool updateReelSequence = true)
        {

	        IsInLink = false;
			if (isLink)
			{
				IsInLink = true;
				machineState.machineContext.view.Get<FeatureGame11017>().SetFeature();
				UpdateRunningWheel(new List<string>() {"WheelLinkGame"},updateReelSequence);
			}
			else if(isFree)
			{
				machineState.machineContext.view.Get<FeatureGame11017>().SetFeature();
				UpdateRunningWheel(new List<string>() {"WheelFreeGame"},updateReelSequence);
			}
			else
			{
				machineState.machineContext.view.Get<FeatureGame11017>().SetFeature();
				machineState.machineContext.view.Get<SuperFreeGameCoins11017>().SetWheelCoinIdle();
				UpdateRunningWheel(new List<string>() {"WheelBaseGame"}, updateReelSequence);
			}
        }
        

        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName == "WheelBaseGame")
			{
				return "Reels";
			}
			else if(wheel.wheelName == "WheelLinkGame")
			{
				return "LinkReels";
				
			}
			else if(wheel.wheelName == "WheelFreeGame")
			{
				return "FreeReels";
			}

            return "Reels";
        }

    }
}