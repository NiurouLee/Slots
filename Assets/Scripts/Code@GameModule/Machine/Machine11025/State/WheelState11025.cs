using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class WheelState11025 : WheelState
	{
		private ExtraState11025 _extraState;
		public ExtraState11025 extraState
		{
			get
			{
				if (_extraState == null)
				{
					_extraState =  machineState.machineContext.state.Get<ExtraState11025>();
				}
				return _extraState;
			}
		}
		public WheelState11025(MachineState state)
		:base(state)
		{

	
		}
		public override bool HasAnticipationAnimationInRollIndex(int rollIndex)
		{
			return AntiByChip(rollIndex) >= 0 || AntiByScatter(rollIndex);
		}
		public override int GetAnticipationAnimationStartRollIndex()
		{
			for (var i = 0; i < anticipationInColumn.Count; i++)
			{
				if (HasAnticipationAnimationInRollIndex(i))
					return i;
			}
			return rollCount;
		}

		public bool AntiByScatter(int rollIndex)
		{
			return anticipationInColumn[rollIndex];
		}

		public int AntiByChip(int rollIndex)
		{
			if (wheelName == Constant11025.WheelBaseGameName)
			{
				var antiRollList = extraState.GetNowNormalData().StickyAnticipation;
				for (var i = 0; i < antiRollList.Count; i++)
				{
					if (antiRollList[i].X == rollIndex)
					{
						return (int)antiRollList[i].Y;
					}
				}
				return -1;
			}
			else
			{
				var antiRollList = extraState.GetFreeData().StickyAnticipation;
				for (var i = 0; i < antiRollList.Count; i++)
				{
					if (antiRollList[i].X == rollIndex)
					{
						return (int)antiRollList[i].Y;
					}
				}
				return -1;
			}
			// var wheel = machineState.machineContext.view.Get<Wheel11025>(wheelName);
			// var stickyMap = wheel.StickyMap;
			// var stickyDictionary = stickyMap[rollIndex];
			// var rollHeight = Constant11025.RollHeightList[rollIndex];
			// int emptyContainerCount = 0;
			// for (var i = 0; i < rollHeight; i++)
			// {
			// 	if (!stickyDictionary[(uint) i].HasContainer())
			// 	{
			// 		emptyContainerCount++;
			// 		if (emptyContainerCount > 1)
			// 		{
			// 			return false;
			// 		}
			// 	}
			// }
			//
			// if (emptyContainerCount == 1)
			// {
			// 	return true;
			// }
			// return false;
		}
		
		public override List<SequenceElement> GetSpinResultSequenceElement(Roll roll)
		{
			return base.GetSpinResultSequenceElement(roll);
		}
		
		
		public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
		{
			sequenceElementConstructor = machineState.machineContext.sequenceElementConstructor;

			if (!wheelIsActive)
				return;

			var gameResult = gameEnterInfo.GameResult;
			if (gameResult.IsFreeSpin)
			{
				if (gameResult.Panels.Count > resultIndex)
				{
					currentSequenceName = gameEnterInfo.GameResult.Panels[resultIndex].ReelsId;
					UpdateReelSequences();
					// currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

					var panel = gameEnterInfo.GameResult.Panels[resultIndex];

					UpdateWheelStateInfo(panel);
				}
				else
				{
					UpdateReelSequences();
					// currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);
				}
			}
		}
	}
}