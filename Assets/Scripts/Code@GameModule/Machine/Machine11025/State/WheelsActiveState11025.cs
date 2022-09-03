using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
	public class WheelsActiveState11025 : WheelsActiveState
	{
		public WheelsActiveState11025(MachineState machineState)
		:base(machineState)
		{

	
		}
		public override void UpdateRunningWheelState(GameResult gameResult)
		{
			var extraInfo = ProtocolUtils.GetAnyStruct<AmalgamationGameResultExtraInfo>(gameResult.ExtraInfoPb);

			if (gameResult.IsFreeSpin)
			{
				UpdateFreeWheelState();
				// UpdateRunningWheel(new List<string>() {Constant11025.WheelFreeGameName},false);
			}
			else
			{
				UpdateBaseWheelState();
				// UpdateRunningWheel(new List<string>() {Constant11025.WheelBaseGameName},false);
			}
		}
		public void UpdateBaseWheelState()
		{
			ToggleJackpotPanel(true);
			UpdateRunningWheel(new List<string>() {Constant11025.WheelBaseGameName});
			machineState.machineContext.view.Get<BackgroundView11025>().OpenBase();
			var normalData = machineState.Get<ExtraState11025>().GetNowNormalData();
			var baseWheel = machineState.machineContext.view.Get<WheelBase11025>();
			baseWheel.InitShopLockState();
			baseWheel.wheelState.UpdateWheelStateInfo(normalData.Panels[0]);
			baseWheel.ForceUpdateElementOnWheel();
			baseWheel.UpdateChameleonState(normalData.StickyColumnReSpinCounts, true);
			baseWheel.UpdateNormalDataPanel(normalData.StickyItems,normalData.FullIndexes,normalData.FailedIndexes,true);
		}
		public void UpdateFreeWheelState()
		{
			ToggleJackpotPanel(false);
			UpdateRunningWheel(new List<string>() {Constant11025.WheelFreeGameName});
			machineState.machineContext.view.Get<BackgroundView11025>().OpenFree();
			var freeData = machineState.Get<ExtraState11025>().GetFreeData();
			var freeWheel = machineState.machineContext.view.Get<WheelFree11025>();
			freeWheel.UpdateNormalDataPanel(freeData.StickyItems,null,null,true);
			for (var i = 0; i < freeWheel.rollCount; i++)
			{
				freeWheel.SetRollMaskColor(i,RollMaskOpacityLevel11025.None);
			}

			var freeMultiWheel = machineState.machineContext.view.Get<FreeMultiWheel11025>();
			freeMultiWheel.InitState();
		}
		private void ToggleJackpotPanel(bool visible)
		{
			machineState.machineContext.view.Get<JackPotPanel11025>().transform.gameObject.SetActive(visible);
			// var blackLayer = machineState.machineContext.view.Get<JackPotPanel11025>().transform.Find("WheelTrainBGGroup");
			// blackLayer.gameObject.SetActive(!visible);
			// var tempSortingGroup = machineState.machineContext.view.Get<JackPotPanel11025>().transform.GetComponent<SortingGroup>();
			// if (visible)
			// {
			// 	tempSortingGroup.sortingLayerName = "Wheel";
			// 	tempSortingGroup.sortingOrder = 10;
			// }
			// else
			// {
			// 	tempSortingGroup.sortingLayerName = "LocalUI";
			// 	tempSortingGroup.sortingOrder = 5;
			// }
		}
		public override string GetReelNameForWheel(Wheel wheel)
		{
			if (wheel.wheelName == "WheelFreeGame")
			{
				var freeState = machineState.Get<FreeSpinState>();
				if (Constant11025.ShopSpecialFreeId.Contains(freeState.freeSpinId))
				{
					var reelsName = "SuperFree"+(freeState.freeSpinId-2)+"Reels";
					return reelsName;
				}
				return "FreeReels";
			}
			return "BaseFeatureReels";
		}
	}
}