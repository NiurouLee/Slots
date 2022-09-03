using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class WheelState11030 : WheelState
	{
		public WheelState11030(MachineState state)
		:base(state)
		{

	
		}
		public void UpdateStateOnReceiveBonusInitSpin(SBonusProcess spinResult)
		{
			haveFiveOfKinWinLine = false;
			var wheelSpinResult = spinResult.GameResult.Panels[0];
			UpdateWheelStateInfo(wheelSpinResult);
		}
	}
}