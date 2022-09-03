using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
	public class ExtraState11025 : ExtraState<ChameleonGameResultExtraInfo>
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

		private WheelsActiveState11025 _wheelsActiveState;
		public WheelsActiveState11025 wheelsActiveState
		{
			get
			{
				if (_wheelsActiveState == null)
				{
					_wheelsActiveState =  machineState.machineContext.state.Get<WheelsActiveState11025>();
				}
				return _wheelsActiveState;
			}
		}
		private MapField<ulong, ChameleonGameResultExtraInfo.Types.NormalData> _normalDataMap;
		public ExtraState11025(MachineState state)
		:base(state)
		{

	
		}
		public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
		{
			base.UpdateStateOnRoomSetUp(gameEnterInfo);
			// _normalDataMap = extraInfo.NormalDataMap;
		}

		public ulong GetAverageBet()
		{
			return extraInfo.AvgBet.TotalBet / extraInfo.AvgBet.SpinCount;
		}
		public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
		{
			base.UpdateStatePreRoomSetUp(gameEnterInfo);
			_normalDataMap = extraInfo.NormalDataMap;
		}
		
		public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
		{
			base.UpdateStateOnReceiveSpinResult(spinResult);
			var tempKeyList = extraInfo.NormalDataMap.Keys;
			var normalDataKeyList = new List<ulong>(tempKeyList);
			for (var i = 0; i < normalDataKeyList.Count; i++)
			{
				var key = normalDataKeyList[i];
				_normalDataMap[key] = extraInfo.NormalDataMap[key];
			}
		}
		
		public ChameleonGameResultExtraInfo.Types.NormalData GetNowNormalData()
		{
			var nowBet = betState.totalBet;
			if (_normalDataMap.ContainsKey(nowBet))
			{
				return _normalDataMap[nowBet];
			}
			throw new Exception("没有对应押注的normalData");
		}
		public ChameleonGameResultExtraInfo.Types.FreeData GetFreeData()
		{
			return extraInfo.FreeData;
		}

		public RepeatedField<ChameleonGameResultExtraInfo.Types.PositionedCredit> GetFlowerList()
		{
			if (machineState.Get<WheelsActiveState11025>().GetRunningWheel()[0].wheelName == Constant11025.WheelBaseGameName)
			{
				return extraInfo.PositionedCredits;	
			}
			return new RepeatedField<ChameleonGameResultExtraInfo.Types.PositionedCredit>();
		}
		public ulong GetNowShopCoins()
		{
			return GetShopData().Credits;
		}

		public ChameleonGameResultExtraInfo.Types.Shop GetShopData()
		{
			return extraInfo.Shop;
		}
		
		public int GetPlayingPageIndex()
		{
			var pagesData = GetShopData().Tables;
			for (var i = 0; i < pagesData.Count; i++)
			{
				if (!pagesData[i].SuperTriggered)
				{
					return i;
				}
			}

			return 0;
		}

		public override bool HasSpecialEffectWhenWheelStop()
		{
			return true;
		}
	}
}