using UnityEngine;

namespace GameModule
{
    public class SequenceElementConstructor11019:SequenceElementConstructor
    {
        public SequenceElementConstructor11019(MachineContext inMachineContext) : base(inMachineContext)
        {
        }
        
        
        protected override ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence,
            ReelSequence resultSequence, int stopPosition, int extraElementAppend,int column,int extraTopElementAppend)
        {
            var results =
                base.AppendExtraSequenceElement(reelSequence, resultSequence, stopPosition, extraElementAppend, column, extraTopElementAppend);

            if ( Constant11019.ListBonusAllElementIds.Contains(results.sequenceElements[0].config.id))
            {
                var length = Constant11019.ListLowLevelElementIds.Count;
                var index = Random.Range(0, length);
                results.sequenceElements[0] = new SequenceElement(elementConfigSet.GetElementConfig(Constant11019.ListLowLevelElementIds[index]), machineContext);
            }
            
            return results;
        }
    }
}