using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
	public class ExtraState11024 : ExtraState<PigGameResultExtraInfo>
	{
		private BetState _betState;
		public BetState betState
		{
			get
			{
				if (_betState == null)
				{
					_betState =  machineState.Get<BetState>();
				}
				return _betState;
			}
		}

		private WheelsActiveState11024 _wheelsActiveState;
		public WheelsActiveState11024 wheelsActiveState
		{
			get
			{
				if (_wheelsActiveState == null)
				{
					_wheelsActiveState =  machineState.machineContext.state.Get<WheelsActiveState11024>();
				}
				return _wheelsActiveState;
			}
		}
		public ExtraState11024(MachineState state)
		:base(state)
		{

	
		}

		public PigGameResultExtraInfo.Types.MapBonus GetMapData()
		{
			return extraInfo.MapBonus;
		}

		public float GetMapCollectProgressPercent()
		{
			return Math.Min((float)GetMapData().Count / GetMapData().MaxCount,1);
		}

		public uint GetMapLevel()
		{
			var mapLevel = GetMapData().Level;
			var progress = GetMapCollectProgressPercent();
			var hasBonus = HasBonus();
			if (progress >= 1 && !hasBonus && Constant11024.IsBigPoint((int) mapLevel+1))
			{
				mapLevel++;
			}
			return mapLevel;
		}
		
		public bool HasBonus()
		{
			return GetMapData().IsStarted;
		}

		public bool HasReSpinType(int pigType)
		{
			return extraInfo.ReSpinType[pigType];
		}

		public int GetReSpinPanelCount()
		{
			return (int)extraInfo.ReSpinPanelCount;
		}

		public int GetReSpinLeftSpin(int panelIndex = 0)
		{
			return (int)extraInfo.ReSpinCounts[panelIndex];
		}

		public RepeatedField<PigGameResultExtraInfo.Types.PositionedWinRates.Types.PositionedWinRate> GetReSpinBoostData(
			int panelIndex = 0)
		{
			return extraInfo.ReSpinRandomBoostWinRateList[panelIndex].List;
		}

		public PigGameResultExtraInfo.Types.LinkData GetLinkData(int panelIndex = 0)
		{
			return extraInfo.LinkDataList[panelIndex];
		}

		public uint GetPigCollectLevel(int pigType)
		{
			return extraInfo.ReSpinTypeShowLevel[pigType];
		}
		
		public override bool HasSpecialEffectWhenWheelStop()
		{
			return true;
		}

		public override bool HasSpecialBonus()
		{
			if (HasBonus())
			{
				return true;
			}
			// if (machineState.machineContext.view.Get<MapView11024>().nowLevel != GetMapLevel())
			// {
			// 	return true;
			// }
			return false;
		}

		public bool HasCollectLink()
		{
			return extraInfo.LinkCalculated;
		}
	}
}