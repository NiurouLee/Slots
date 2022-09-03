using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using UnityEngine.Rendering;

namespace GameModule
{
	public class WheelsActiveState11030 : WheelsActiveState
	{
		public bool IsBaseWheel;
		public bool IsFreeWheel;
		public bool IsChooseTrainWheel;
		public bool IsTrainWheel;
		public WheelsActiveState11030(MachineState machineState)
		:base(machineState)
		{
			IsBaseWheel = true;
			IsFreeWheel = false;
			IsChooseTrainWheel = false;
			IsTrainWheel = false;
		}
		
		public override void UpdateRunningWheelState(GameResult gameResult)
		{
			var extraInfo = ProtocolUtils.GetAnyStruct<AmalgamationGameResultExtraInfo>(gameResult.ExtraInfoPb);

			if (gameResult.IsFreeSpin)
			{
				UpdateFreeWheelState();
				// UpdateRunningWheel(new List<string>() {Constant11030.WheelFreeGameName},false);
			}
			else if (machineState.Get<ExtraState11030>().IsInTrain() && machineState.Get<ExtraState11030>().IsTrainFromChoose())
			{
				UpdateChooseTrainWheelState();
				// UpdateRunningWheel(new List<string>() {Constant11030.WheelLineFeatureGameName},false);
			}
			else
			{
				UpdateBaseWheelState();
				// UpdateRunningWheel(new List<string>() {Constant11030.WheelBaseGameName},false);
			}
		}
		public void UpdateTrainWheelState()
		{
			// IsBaseWheel = false;
			// IsFreeWheel = false;
			// IsChooseTrainWheel = false;
			IsTrainWheel = true;
			ToggleJackpotPanel(false);
			// UpdateRunningWheel(new List<string>() {});
			GetRunningWheel()[0].transform.gameObject.SetActive(false);
			machineState.machineContext.view.Get<BackgroundView11030>().OpenBonus();
		}
		public void UpdateBackFromTrainWheelState()
		{
			// IsBaseWheel = false;
			// IsFreeWheel = false;
			// IsChooseTrainWheel = false;
			IsTrainWheel = false;
			ToggleJackpotPanel(true);
			// UpdateRunningWheel(new List<string>() {});
			GetRunningWheel()[0].transform.gameObject.SetActive(true);
			if (IsBaseWheel)
			{
				machineState.machineContext.view.Get<BackgroundView11030>().OpenBase();
			}
			else if (IsFreeWheel)
			{
				machineState.machineContext.view.Get<BackgroundView11030>().OpenFree();
			}
			else if (IsChooseTrainWheel)
			{
				if (!machineState.Get<FreeSpinState11030>().IsOver)
				{
					machineState.machineContext.view.Get<BackgroundView11030>().OpenFree();
				}
				else
				{
					machineState.machineContext.view.Get<BackgroundView11030>().OpenBase();
				}
			}
		}
		
		public void UpdateChooseTrainWheelState()
		{
			IsBaseWheel = false;
			IsFreeWheel = false;
			IsChooseTrainWheel = true;
			IsTrainWheel = false;
			ToggleJackpotPanel(true);
			UpdateRunningWheel(new List<string>() {Constant11030.WheelLineFeatureGameName});
			if (!machineState.Get<FreeSpinState11030>().IsOver)
			{
				machineState.machineContext.view.Get<BackgroundView11030>().OpenFree();
			}
			else
			{
				machineState.machineContext.view.Get<BackgroundView11030>().OpenBase();
			}
		}
		public void UpdateBaseWheelState()
		{
			IsBaseWheel = true;
			IsFreeWheel = false;
			IsChooseTrainWheel = false;
			IsTrainWheel = false;
			ToggleJackpotPanel(true);
			UpdateRunningWheel(new List<string>() {Constant11030.WheelBaseGameName});
			machineState.machineContext.view.Get<BackgroundView11030>().OpenBase();
		}
		public void UpdateFreeWheelState()
		{
			IsBaseWheel = false;
			IsFreeWheel = true;
			IsChooseTrainWheel = false;
			IsTrainWheel = false;
			ToggleJackpotPanel(true);
			UpdateRunningWheel(new List<string>() {Constant11030.WheelFreeGameName});
			machineState.machineContext.view.Get<BackgroundView11030>().OpenFree();
		}
		private void ToggleJackpotPanel(bool visible)
		{
			// machineState.machineContext.view.Get<JackPotPanel11030>().transform.gameObject.SetActive(visible);
			var blackLayer = machineState.machineContext.view.Get<JackPotPanel11030>().transform.Find("WheelTrainBGGroup");
			blackLayer.gameObject.SetActive(!visible);
			var tempSortingGroup = machineState.machineContext.view.Get<JackPotPanel11030>().transform.GetComponent<SortingGroup>();
			if (visible)
			{
				tempSortingGroup.sortingLayerName = "Wheel";
				tempSortingGroup.sortingOrder = 10;
			}
			else
			{
				tempSortingGroup.sortingLayerName = "LocalUI";
				tempSortingGroup.sortingOrder = 5;
			}
		}
	}
}