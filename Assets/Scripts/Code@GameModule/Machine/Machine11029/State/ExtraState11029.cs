using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using GameModule;

namespace GameModule
{
	public class ExtraState11029 : ExtraState<EyeOfMedusaGameResultExtraInfo>
	{
		private MapField<uint, uint> ReelMappingTemp;
		private bool active =true;

		public ExtraState11029(MachineState state)
		:base(state)
		{

	
		}
		
		public override bool HasSpecialEffectWhenWheelStop()
		{
			return true;
		}
		
		public void SetActive(bool isActive)
		{
			active = isActive;
		}
		
		public bool  GetIsActive()
		{
			return active;
		}
		
		
		public override bool HasBonusGame()
		{
			return GetIsWheel();
		}

		//地图等级
		public uint GetMapLevel()
		{
			return extraInfo.Level;
		}

		//bag等级
		public uint GetBagLevel()
		{
			return extraInfo.BagLevel;
		}

		//地图等级
		public uint GetMapPoint()
		{
			return extraInfo.Point;
		}

		//地图等级
		public uint GetMapMaxPoint()
		{
			return extraInfo.MaxPoint;
		}
		
		//map 玩法倍数
		public RepeatedField<uint> GetRandomWildIdsGetMapMultipliers()
		{
			return extraInfo.MapMultipliers;
		} 
		
		//map玩法中 随机wild的位置
		public RepeatedField<EyeOfMedusaGameResultExtraInfo.Types.RepeatedPositions>  GetRandomWildIds()
		{
			return extraInfo.MapRandomWilds;
		}
		
		//map玩法中 movingwild的位置
		public RepeatedField<EyeOfMedusaGameResultExtraInfo.Types.RepeatedMovingPositionIds> GetMovingWildIds()
		{
			return extraInfo.MapMovingWilds;
		}
		
		//随机产生的J系列
		public RepeatedField<EyeOfMedusaGameResultExtraInfo.Types.PositionId>  GetRandomGems()
		{
			return extraInfo.RandomPositionedGems;
		}
		
		//map玩法中 固定wild的位置
		public RepeatedField<EyeOfMedusaGameResultExtraInfo.Types.RepeatedPositionIds>  GetStickyWildIds()
		{
			return extraInfo.MapStickyWilds;
		}

		//map玩法中 固定wild的位置
		public RepeatedField<uint> GetMulText()
		{
			return extraInfo.MapMultipliers;
		}

		//free玩法中 随机wild的位置
		public RepeatedField<EyeOfMedusaGameResultExtraInfo.Types.Position>  GetFreeRandomWildIds()
		{
			return extraInfo.NormalFreeRandomWilds;
		}

		//wheel数据
		public EyeOfMedusaGameResultExtraInfo.Types.Wheel GetWheelData()
		{
			return extraInfo.Wheel;
		}
		
		public bool GetIsWheel()
		{
			return extraInfo.Wheel.Started;
		}

		public bool GetIsWheelSettle()
		{
			return extraInfo.Wheel.ToSettle;
		}

		public bool GetIsDrag()
		{
			return extraInfo.IsFeature;
		}
		
		public MapField<uint, int>  GetDragPos()
		{
			return extraInfo.DragReelPositionMap;
		}
		
		public bool GetIsWheelOver()
		{
			return extraInfo.Wheel.ToSettle;
		}
		
		public bool GetIsWheelStart()
		{
			return extraInfo.Wheel.Started;
		}

		public bool NeedWheelSettle()
		{
			return GetIsWheelOver() && GetIsWheelStart();
		}

		public ulong GetMapPreWin()
		{
			return extraInfo.MapFreePreWin;
		}
	}
}