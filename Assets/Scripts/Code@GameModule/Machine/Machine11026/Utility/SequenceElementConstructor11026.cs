using UnityEngine;

namespace GameModule
{
    public class SequenceElementConstructor11026:SequenceElementConstructor
    {
        public SequenceElementConstructor11026(MachineContext inMachineContext) : base(inMachineContext)
        {
        }
        
            protected override ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence,
            ReelSequence resultSequence, int stopPosition, int extraElementAppend,int column,int extraTopElementAppend)
        {
            var results =
                base.AppendExtraSequenceElement(reelSequence, resultSequence, stopPosition, extraElementAppend,column, extraTopElementAppend);

            if (Constant11026.ListShowBigLevelAllElementIds.Contains(results.sequenceElements[0].config.id))
            {
                var length = Constant11026.ListShoLowLevelAllElementIds.Count;
                var index = Random.Range(0, length);
                results.sequenceElements[0] = new SequenceElement(elementConfigSet.GetElementConfig(Constant11026.ListShoLowLevelAllElementIds[index]), machineContext)
                    ;
            }
            
            return results;
        }
    }
}