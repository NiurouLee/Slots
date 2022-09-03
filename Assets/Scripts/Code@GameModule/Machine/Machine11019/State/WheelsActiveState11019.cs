using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class WheelsActiveState11019 : WheelsActiveState
	{
		public WheelsActiveState11019(MachineState machineState)
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
				
				UpdateRunningWheel(new List<string>() {"WheelLinkGame"},updateReelSequence);
				machineState.machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);
				machineState. machineContext.view.Get<TransitionsView11019>().CloseSpineMan();
				machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
			}
			else if(isFree)
			{
				UpdateRunningWheel(new List<string>() {"WheelFreeGame"},updateReelSequence);
				machineState.machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(false);
				machineState. machineContext.view.Get<TransitionsView11019>().CloseSpineMan();
				machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
			}
			else
			{
				UpdateRunningWheel(new List<string>() {"WheelBaseGame"},updateReelSequence);
				machineState.machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);
				machineState.machineContext.view.Get<TransitionsView11019>().OpenSpineMan();
				machineState.machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
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