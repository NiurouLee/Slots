using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using GameModule;

namespace GameModule
{
	public class ExtraState11026 : ExtraState<DragonRisingGameResultExtraInfo>
	{
		private MapField<uint, uint> ReelMappingTemp;

		public ExtraState11026(MachineState state)
		:base(state)
		{

	
		}

		public override bool HasSpecialEffectWhenWheelStop()
		{
			return true;
		}
		
		public RepeatedField<DragonRisingGameResultExtraInfo.Types.Position>  GetFreeRandomWildIds()
		{
			return extraInfo.RandomWilds;
		}

		public RepeatedField<DragonRisingGameResultExtraInfo.Types.Position> GetFreeStickyWildIds()
		{
			return extraInfo.StickyWilds;
		}

		public DragonRisingGameResultExtraInfo.Types.LinkData GetLeftLinkData()
		{
			return extraInfo.LinkDataLeft;
		}
		
		public DragonRisingGameResultExtraInfo.Types.LinkData GetCenterLinkData()
		{
			return extraInfo.LinkDataCenter;
		}

		public DragonRisingGameResultExtraInfo.Types.LinkData GetRightLinkData()
		{
			return extraInfo.LinkDataRight;
		}

		public DragonRisingGameResultExtraInfo.Types.LinkData.Types.Item GetLeftLinkJackpotPay(int positionId)
		{

			if (extraInfo.LinkDataLeft.Items.Count > 0)
			{
				return extraInfo.LinkDataLeft.Items[positionId];
			}
			
			return null;
		}
		
		public DragonRisingGameResultExtraInfo.Types.LinkData.Types.Item GetRightLinkJackpotPay(int positionId)
		{

			if (extraInfo.LinkDataRight.Items.Count > 0)
			{
				return extraInfo.LinkDataLeft.Items[positionId];
			}
			
			return null;
		}
		
		public DragonRisingGameResultExtraInfo.Types.LinkData.Types.Item GetCenterLinkJackpotPay(int positionId)
		{

			if (extraInfo.LinkDataCenter.Items.Count > 0)
			{
				return extraInfo.LinkDataLeft.Items[positionId];
			}
			
			return null;
		}

		public uint GetLevel()
        {
            return extraInfo.Level;
        }
		
		public uint GetRowsMore()
		{
			return extraInfo.RowsMore;
		}
		
		public uint GetRowsMoreOld()
		{
			return extraInfo.RowsMoreOld;
		}
		
		public uint GetAllWinMultiplier()
		{
			return extraInfo.AllWinMultiplier;
		}
		
		public uint GetAllWinMultiplierOld()
		{
			return extraInfo.AllWinMultiplierOld;
		}

		public bool GetIsMega()
		{
			return extraInfo.IsMega;
		}
		
		public bool GetIsSuper()
		{
			return extraInfo.IsSuper;
		}

		public void SetReelMappingTemp(MapField<uint, uint> temp)
		{
			ReelMappingTemp = (temp!=null && temp.Count>0) ? temp : null;
		}
		
		public MapField<uint, uint> GetReelMappingTemp()
		{
			return ReelMappingTemp;
		}
	}
}