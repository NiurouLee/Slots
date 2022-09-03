using System;
using System.Collections.Generic;

namespace GameModule
{
    public class SequenceElementConstructor11022:SequenceElementConstructor
    {
        public SequenceElementConstructor11022(MachineContext inMachineContext) : base(inMachineContext)
        {
        }
        protected override ReelSequence GetFixedReelSequence(List<ReelSequence> sequences, int column, WheelState wheelState)
        {
            //Link玩法取第0个
            if (Constant11022.SingleWheelReelIndexDictionary.ContainsKey(wheelState.wheelName))
                return sequences[Constant11022.SingleWheelReelIndexDictionary[wheelState.wheelName]];
            if (wheelState.wheelName == "WheelLinkGame")
            {
                var extraState = machineContext.state.Get<ExtraState11022>();
                var reelMapping = extraState.GetLinkData().ReelMapping;
                int realIndex = column;
        
                if (extraState != null)
                {
                    if (extraState.IsLinkNeedInitialized())
                    {
                        realIndex = column / 3;
                    }
                    else
                    {
                        if (reelMapping.ContainsKey((uint) column))
                        {
                            realIndex = (int) reelMapping[(uint) column];
                        }
                        else
                        {
                            realIndex = 0;
                        }
                    }
                }

                try
                {
                    ReelSequence reelSequence = sequences[realIndex]; 
                    return reelSequence;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (column >= sequences.Count)
                return sequences[0];

            return sequences[column];
        }
        protected override ReelSequence AppendExtraSequenceElement(ReelSequence reelSequence, ReelSequence resultSequence, int stopPosition, int extraElementAppend,int column,int extraTopElementAppend)
        {
            var elementCountInReelSequence = reelSequence.sequenceElements.Count;

            int visibleSymbolCount = resultSequence.sequenceElements.Count;
            var nextElementIndex = (stopPosition - 1 + elementCountInReelSequence) %
                                   elementCountInReelSequence;
            
            var nextElement = reelSequence.sequenceElements[nextElementIndex];
            
            //AppendHeadElement
            resultSequence.sequenceElements.Insert(0, nextElement);
            if (extraTopElementAppend > 0)
            {
                for (var a = 0; a < extraTopElementAppend; a++)
                {
                    var tempNextElementIndex = (stopPosition - 1 - (1+a) + elementCountInReelSequence * (a+2)) %
                                           elementCountInReelSequence;
            
                    var tempNextElement = reelSequence.sequenceElements[tempNextElementIndex];
            
                    //AppendHeadElement
                    resultSequence.sequenceElements.Insert(0, tempNextElement);
                }
            }
            var tempWheelsActiveState = machineContext.state.Get<WheelsActiveState11022>();
            //AppendExtraTailElement
            if (extraElementAppend > 0)
            {
                var firstPos = stopPosition + visibleSymbolCount;
                for (var a = 0; a < extraElementAppend; a++)
                {
                    var appendElement = reelSequence.sequenceElements[
                        (int)(firstPos + a) % elementCountInReelSequence];
                    if (tempWheelsActiveState.IsLinkWheel && a == 0 && (appendElement.config.id == 3 || appendElement.config.id == 12) )
                    {
                        appendElement = new SequenceElement(machineContext.machineConfig.GetElementConfigSet().GetElementConfig((uint)UnityEngine.Random.Range(4,11)), machineContext);
                    }
                    if ((tempWheelsActiveState.IsFreeWheel || tempWheelsActiveState.IsBaseWheel) && a == 0 && (appendElement.config.id == 13 || appendElement.config.id == 12 || appendElement.config.id == 3) )
                    {
                        appendElement = new SequenceElement(machineContext.machineConfig.GetElementConfigSet().GetElementConfig((uint)UnityEngine.Random.Range(4,11)), machineContext);
                    }
                    resultSequence.sequenceElements.Add(appendElement);
                }
            }
            // XDebug.Log("rollIndex:" + column + " stopPosition:" + stopPosition);
            // for (int i = 0; i < resultSequence.sequenceElements.Count; i++)
            // {
            //     XDebug.Log(resultSequence.sequenceElements[i].config.id);
            // }
            return resultSequence;
        }
    }
    
}