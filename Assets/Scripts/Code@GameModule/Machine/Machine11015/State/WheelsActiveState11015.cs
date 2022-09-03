using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class WheelsActiveState11015 : WheelsActiveState
	{
		public WheelsActiveState11015(MachineState machineState)
		:base(machineState)
		{

	
		}
		
		
		public override void UpdateRunningWheelState(GameResult gameResult)
		{
			bool isFree = gameResult.IsFreeSpin;
			if (isFree && gameResult.FreeSpinInfo.FreeSpinCount == gameResult.FreeSpinInfo.FreeSpinTotalCount)
			{
				isFree = false;
			}

			UpdateRunningWheelState(isFree,false);
		}

		public bool IsInLink
		{
			get;
			protected set;
		}

		public  void UpdateRunningWheelState(bool isFree,bool updateReelSequence = true)
		{
			if(isFree)
			{
				UpdateRunningWheel(new List<string>() {"WheelFreeGame12X4"},updateReelSequence);
				var transitionView = machineState.machineContext.view.Get<TransitionView11015>();
				transitionView.OpenFreeBackground();
			}
			else
			{
				UpdateRunningWheel(new List<string>() {"WheelBaseGame"},updateReelSequence);
				var transitionView = machineState.machineContext.view.Get<TransitionView11015>();
				transitionView.OpenBaseBackground();
			}
            
			
		}
        

		public override string GetReelNameForWheel(Wheel wheel)
		{


			var reSpinState = machineState.Get<ReSpinState>();
			
			if (wheel.wheelName == "WheelBaseGame")
			{
				if (reSpinState.NextIsReSpin)
				{
					return "BaseFeatureReels";
				}
				else
				{
					return "Reels";
				}

				
			}
			else if(wheel.wheelName == "WheelFreeGame12X4")
			{
				if (reSpinState.NextIsReSpin)
				{
					return "FreeFeatureReels";
				}
				else
				{
					return "FreeReels";
				}
				
			}


			return "Reels";
		}

	}
}