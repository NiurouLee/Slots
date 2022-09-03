using UnityEngine;

namespace GameModule
{
    public class SequenceElementConstructor11017:SequenceElementConstructor
    {
        public SequenceElementConstructor11017(MachineContext inMachineContext)
        :base(inMachineContext)
        {
        
        }

        protected override ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence,
            ReelSequence resultSequence, int stopPosition, int extraElementAppend,int column,int extraTopElementAppend)
        {
            var results =
                base.AppendExtraSequenceElement(reelSequence, resultSequence, stopPosition, extraElementAppend,column,extraTopElementAppend);

            if (results.sequenceElements[0].config.id == Constant11017.ScatterElementId
            || results.sequenceElements[0].config.id == Constant11017.SmallGoldElementId
            || results.sequenceElements[0].config.id == Constant11017.PuePleElementId)
            {
                var length = Constant11017.ListLowLevelAllElementIds.Count;
                var index = Random.Range(0, length);
                results.sequenceElements[0] = new SequenceElement(elementConfigSet.GetElementConfig(Constant11017.ListLowLevelAllElementIds[index]), machineContext)
                    ;
            }
            
            return results;
        }

    }
}