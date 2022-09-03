using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ExtraState11022 : ExtraState<AmalgamationGameResultExtraInfo>
	{
		// private AmalgamationGameResultExtraInfo lastExtraInfo;
		public ExtraState11022(MachineState state)
		:base(state)
		{
		}

		public bool IsChoosingGameType()
		{
			return extraInfo.ToChoose;
		}

		public bool IsLinkFromChoose()
		{
			return extraInfo.LinkFromChoose;
		}
		public bool IsLinkNeedInitialized()
		{
			return extraInfo.LinkNeedInitialized;
		}
		public bool IsLastLinkNeedInitialized()
		{
			return lastExtraInfo.LinkNeedInitialized;
		}
		
		public uint GetChoosingFreeSpinCount()
		{
			return extraInfo.FreeSpinCount;
		}

		public bool IsFullTrigger()
		{
			return extraInfo.NoReSpinNeeded;
		}
		public int GetFreeSpinTriggerCount()
		{
			var freeSpinCountChangeToTriggerCountDictionary = new Dictionary<uint,int>() {{10,3},{15,4},{25,5}};
			return freeSpinCountChangeToTriggerCountDictionary[GetChoosingFreeSpinCount()];
		}

		public uint GetChoosingReSpinCount()
		{
			return extraInfo.ReSpinCount;
		}
		public int GetDragReelPosition(uint reelIndex)
		{
			return extraInfo.DragReelPositionMap[reelIndex];
		}
		public MapField<uint, int> GetDragReelPositionMap()
		{
			return extraInfo.DragReelPositionMap;
		}
		public AmalgamationGameResultExtraInfo.Types.LinkData GetLinkData()
		{
			return extraInfo.LinkData;
		}
		public RepeatedField<AmalgamationGameResultExtraInfo.Types.ShapeItem> GetShapeItems()
		{
			return extraInfo.Shapes;
		}
		public RepeatedField<AmalgamationGameResultExtraInfo.Types.ShapeItem> GetLastShapeItems()
		{
			return lastExtraInfo.Shapes;
		}

		public bool IsInLinkCollect()
		{
			return extraInfo.IsShapeSettling;
		}

		public int GetNextCollectShapeIndex()
		{
			var shapes = GetShapeItems();
			for (int i=0;i<shapes.Count;i++)
			{
				if (!shapes[i].IsOver)
				{
					return i;
				}
			}
			return -1;
		}

		public override bool HasBonusGame()
		{
			return IsChoosingGameType();
		}

		public override bool HasSpecialEffectWhenWheelStop()
		{
			var runningWheelName = machineState.Get<WheelsActiveState11022>().GetRunningWheel()[0].wheelName;
			if (runningWheelName == "WheelFreeGame")
			{
				bool HasDragReel = false;
				var keys = new List<uint>  (extraInfo.DragReelPositionMap.Keys);
				for (uint i = 0; i < keys.Count; i++)
				{
					if (extraInfo.DragReelPositionMap[keys[(int)i]] != 0)
					{
						HasDragReel = true;
						break;
					}
				}

				return HasDragReel;	
			}
			// else if (runningWheelName.Contains("X"))
			// {
			// 	return true;
			// }
			return false;
		}
		public Google.ilruntime.Protobuf.Collections.RepeatedField<AmalgamationGameResultExtraInfo.Types.LinkData.Types.Item> GetLinkItems()
		{
			return extraInfo.LinkData.Items;
		}
	}
}