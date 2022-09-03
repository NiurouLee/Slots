using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using ILRuntime.Runtime;

namespace GameModule
{
	public class SequenceElementConstructor11025 : SequenceElementConstructor
	{
		public SequenceElementConstructor11025(MachineContext inMachineContext)
		:base(inMachineContext)
		{

	
		}
		public override List<ReelSequence> ConstructSpinResultReelSequence(WheelState wheelState, DragonU3DSDK.Network.API.ILProtocol.Panel panel)
		{
			var columnsCount = panel.Columns.Count;
			var sequences = new List<ReelSequence>(columnsCount);
			var reelSequences = GetReelSequences(panel.ReelsId);
			var maxElementHeight = wheelState.GetWheelConfig().elementMaxHeight;
			var maxExtraTopElementCount = wheelState.GetWheelConfig().extraTopElementCount;
          
			for (var i = 0; i < columnsCount; i++)
			{
				var resultSequence = new ReelSequence(elementConfigSet, panel.Columns[i], machineContext);
                
				var flowerList = machineContext.state.Get<ExtraState11025>().GetFlowerList();
				for (var j = 0; j < flowerList.Count; j++)
				{
					if (flowerList[j].X == i)
					{
						resultSequence.sequenceElements[(int) flowerList[j].Y].elementCustomData =
							flowerList[j].Credit.ToInt32();// new FlowerElementCustomDataType11025(flowerList[i].Credit);
					}
				}
				
				resultSequence = AppendExtraSequenceElement(GetFixedReelSequence(reelSequences, i, wheelState), resultSequence, (int )panel.Columns[i].StopPosition,maxElementHeight-1,i,maxExtraTopElementCount);
                
				sequences.Add(resultSequence);
			}

			return sequences;
		}
	}
}