using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public enum GameType11024
	{
		Base,
		Free,
		Link,
	}
	public class WheelsActiveState11024 : WheelsActiveState
	{
		public GameType11024 gameType = GameType11024.Base;
		public WheelsActiveState11024(MachineState machineState)
		:base(machineState)
		{

	
		}
		public override void UpdateRunningWheelState(GameResult gameResult)
		{
			var extraInfo = ProtocolUtils.GetAnyStruct<PigGameResultExtraInfo>(gameResult.ExtraInfoPb);

			if (gameResult.IsFreeSpin)
			{
				UpdateFreeWheelState();
			}
			else if (gameResult.IsReSpin)
			{
				UpdateLinkWheelState();
			}
			else
			{
				UpdateBaseWheelState();
			}
		}
		
		public void UpdateBaseWheelState()
		{
			gameType = GameType11024.Base;
			ToggleJackpotPanel(true);
			ToggleSmallJackpotPanel(false);
			UpdateRunningWheel(new List<string>() {"WheelBaseGame"});
			machineState.machineContext.view.Get<CollectBarView11024>().InitState();
			machineState.machineContext.view.Get<PigGroupView11024>().InitView();
			machineState.machineContext.view.Get<BackgroundView11024>().OpenBase();
			machineState.machineContext.view.Get<JackPotSmallPanel11024>().StopRollJP();
			machineState.Get<JackpotInfoState>().LockJackpot = false;
			machineState.machineContext.view.Get<MachineSystemWidgetView>().Show();
		}
		
		public void UpdateLinkWheelState()
		{
			machineState.Get<JackpotInfoState>().LockJackpot = true;
			machineState.machineContext.view.Get<JackPotSmallPanel11024>().StartRollJP();
			gameType = GameType11024.Link;
			var extraState = machineState.Get<ExtraState11024>();
			List<string> wheelNameList;
			if (extraState.HasReSpinType(1))
			{
				ToggleJackpotPanel(false);
				ToggleSmallJackpotPanel(true);
				wheelNameList = new List<string>() {"WheelLinkGame2_1","WheelLinkGame2_2"};
			}
			else
			{
				ToggleJackpotPanel(true);
				ToggleSmallJackpotPanel(false);
				wheelNameList = new List<string>() {"WheelLinkGame1"};
			}
			UpdateRunningWheel(wheelNameList);
			var wheelList = GetRunningWheel();
			for (var i = 0; i < wheelList.Count; i++)
			{
				var linkWheel = (WheelLink11024) wheelList[i];
				linkWheel.CleanWheel();
			}
			if (extraState.HasReSpinType(1))
			{
				machineState.machineContext.view.Get<LinkSmallPigGroupView11024>().RefreshView();
			}
			else
			{
				machineState.machineContext.view.Get<LinkBigPigGroupView11024>().RefreshView();
			}
			machineState.machineContext.view.Get<BackgroundView11024>().OpenBonus();
			machineState.machineContext.view.Get<MachineSystemWidgetView>().Hide();
		}
		public void UpdateFreeWheelState()
		{
			gameType = GameType11024.Free;
			ToggleJackpotPanel(false);
			ToggleSmallJackpotPanel(false);
			machineState.machineContext.view.Get<BackgroundView11024>().OpenFree();
			UpdateRunningWheel(new List<string>() {"WheelFreeGame1","WheelFreeGame2","WheelFreeGame3"});
			var wheelList = GetRunningWheel();
			for (var i = 0; i < wheelList.Count; i++)
			{
				var linkWheel = (WheelFree11024) wheelList[i];
				linkWheel.InitMidReel();
			}
			machineState.machineContext.view.Get<MachineSystemWidgetView>().Hide();
		}
		private void ToggleJackpotPanel(bool visible)
		{
			machineState.machineContext.view.Get<JackPotPanel11024>().transform.gameObject.SetActive(visible);
		}

		public void ToggleSmallJackpotPanel(bool visible)
		{
			machineState.machineContext.view.Get<JackPotSmallPanel11024>().transform.gameObject.SetActive(visible);
		}
		public override string GetReelNameForWheel(Wheel wheel)
		{
			var extraState = machineState.Get<ExtraState11024>();
			if (wheel.wheelName.Contains("WheelFreeGame"))
			{
				var level = extraState.GetMapLevel() / 5;
				return "MapFree"+level+"Reels";
			}

			if (wheel.wheelName.Contains("WheelLinkGame"))
			{
				var linkReelName = "Reels";
				for (var i = 0; i < 3; i++)
				{
					if (extraState.HasReSpinType(i))
					{
						linkReelName += i + 1;
					}
				}
				return linkReelName;
			}
			return "Reels";
		}
	}
}