using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class SequenceElementConstructor11029:SequenceElementConstructor
    {
        public SequenceElementConstructor11029(MachineContext inMachineContext)
        :base(inMachineContext)
        {
        
        }

        public override List<ReelSequence> ConstructSpinResultReelSequence(WheelState wheelState, DragonU3DSDK.Network.API.ILProtocol.Panel panel)
        {
            var sequences = base.ConstructSpinResultReelSequence(wheelState, panel);
            if (wheelState.wheelName == "WheelMagicBonusGame")
            {
                if (sequences[0].sequenceElements.Count != sequences[1].sequenceElements.Count)
                {
                    var countToAdd = sequences[1].sequenceElements.Count - sequences[0].sequenceElements.Count;
                   
                    for (var i = 0; i < countToAdd; i++)
                    {
                        sequences[0].sequenceElements.Insert(0, sequences[0].sequenceElements[0]);
                    }
                }
            }
            return sequences;
        }
        
        // protected override ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence,
        //     ReelSequence resultSequence, int stopPosition, int extraElementAppend,int column,int extraTopElementAppend)
        // {
        //     var results =
        //         base.AppendExtraSequenceElement(reelSequence, resultSequence, stopPosition, extraElementAppend,column,extraTopElementAppend);
        //
        //     if (results.sequenceElements[0].config.id == Constant11017.ScatterElementId
        //     || results.sequenceElements[0].config.id == Constant11017.SmallGoldElementId
        //     || results.sequenceElements[0].config.id == Constant11017.PuePleElementId)
        //     {
        //         var length = Constant11017.ListLowLevelAllElementIds.Count;
        //         var index = Random.Range(0, length);
        //         results.sequenceElements[0] = new SequenceElement(elementConfigSet.GetElementConfig(Constant11017.ListLowLevelAllElementIds[index]), machineContext)
        //             ;
        //     }
        //     
        //     return results;
        // }

    }
}