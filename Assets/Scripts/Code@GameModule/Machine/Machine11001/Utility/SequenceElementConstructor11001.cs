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

using UnityEngine;

namespace GameModule
{
    public class SequenceElementConstructor11001:SequenceElementConstructor
    {
        public SequenceElementConstructor11001(MachineContext inMachineContext)
        :base(inMachineContext)
        {
        
        }

        protected override ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence,
            ReelSequence resultSequence, int stopPosition, int extraElementAppend,int column,int extraTopElementAppend)
        {
            var results =
                base.AppendExtraSequenceElement(reelSequence, resultSequence, stopPosition, extraElementAppend,column,extraTopElementAppend);

            if (results.sequenceElements[0].config.id == Constant11001.b01)
            {
                var length = Constant11001.lowLevelElement.Count;
                var index = Random.Range(0, length);
                results.sequenceElements[0] = new SequenceElement(elementConfigSet.GetElementConfig(Constant11001.lowLevelElement[index]), machineContext)
                    ;
            }
            
            return results;
        }

    }
}