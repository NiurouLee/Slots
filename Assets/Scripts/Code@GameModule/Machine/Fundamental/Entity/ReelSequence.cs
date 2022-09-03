// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : ReelSequence.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class ReelSequence
    {
        public List<SequenceElement> sequenceElements;


        public ReelSequence(List<SequenceElement> list,List<uint> changeId,SequenceElement element)
        {
            sequenceElements = new List<SequenceElement>();
            for (int i = 0; i < list.Count; i++)
            {
                if (changeId != null && 
                    changeId.Contains(list[i].config.id) &&
                    element!=null)
                {
                    sequenceElements.Add(element);
                }
                else
                {
                    sequenceElements.Add(list[i]);
                }
            }
        }

        //通过服务器数据生成卷轴序列
        public ReelSequence(ElementConfigSet elementConfigSet, DragonU3DSDK.Network.API.ILProtocol.Reel reel, MachineContext machineContext)
        {
            var symbolCount = reel.Symbols.Count;
            sequenceElements = new List<SequenceElement>(symbolCount);

            for (var i = 0; i < symbolCount; i++)
            {
                SequenceElement seqElement = new SequenceElement(elementConfigSet.GetElementConfig(reel.Symbols[i]), machineContext);
                sequenceElements.Add(seqElement);
            }
        }
        
        public ReelSequence(ElementConfigSet elementConfigSet, DragonU3DSDK.Network.API.ILProtocol.Column column, MachineContext machineContext)
        {
            var symbolCount = column.Symbols.Count;
            sequenceElements = new List<SequenceElement>(symbolCount);

            for (var i = 0; i < symbolCount; i++)
            {
                SequenceElement seqElement = new SequenceElement(elementConfigSet.GetElementConfig(column.Symbols[i]), machineContext);
                sequenceElements.Add(seqElement);
            }
        }
        
        public ReelSequence(ElementConfigSet elementConfigSet, int elementLength, MachineContext machineContext)
        {
            sequenceElements = new List<SequenceElement>(elementLength);
            for (var i = 0; i < elementLength; i++)
            {
                var index = Random.Range(0, elementConfigSet.elementNum);
                SequenceElement seqElement = new SequenceElement(elementConfigSet.GetElementConfigByIndex(index), machineContext);
                sequenceElements.Add(seqElement);
            }
        }

        public ReelSequence(ElementConfig elementConfig, int elementLength, MachineContext machineContext)
        {
            sequenceElements = new List<SequenceElement>(elementLength);
            for (var i = 0; i < elementLength; i++)
            {
                SequenceElement seqElement = new SequenceElement(elementConfig, machineContext);
                sequenceElements.Add(seqElement);
            }
        }
    }
}