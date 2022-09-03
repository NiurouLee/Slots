// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/18/14:47
// Ver : 1.0.0
// Description : SequenceElementConstructor.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class SequenceElementConstructor11312:SequenceElementConstructor
    {
        public SequenceElementConstructor11312(MachineContext inMachineContext)
        :base(inMachineContext)
        {
        
        }
        
        public override void PostProcessSpinResultSequence(List<ReelSequence> sequences, WheelState wheelState,
            Panel panel)
        {
            if (wheelState.wheelName == "WheelFreeGame")
            {
                for (var i = 0; i < sequences.Count; i++)
                {
                    if (!Constant11312.lowLevelSymbol.Contains(sequences[i].sequenceElements[0].config.id) &&
                        sequences[i].sequenceElements.Count >= 4)
                    {
                        var length = Constant11312.lowLevelSymbol.Count;
                        var index = Random.Range(0, length);
                        sequences[i].sequenceElements[0] = new SequenceElement(
                            elementConfigSet.GetElementConfig(Constant11312.lowLevelSymbol[index]), machineContext);
                    }
                }
            }
        }

    }
}