//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-09 16:21
//  Ver : 1.0.0
//  Description : LinkWheelState11010.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class LinkWheelState11010: WheelState
    {
        public LinkWheelState11010(MachineState state) : base(state)
        {
            currentSequenceName = Constant11007.LinkReels;
        }
        public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
        {
            return currentActiveSequence[0].sequenceElements;
        }

        protected override void UpdateStartAndStopIndex(Panel panel)
        {
            currentSequenceName = panel.ReelsId;
            currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

            int index = 0;
            for (var i = 0; i < rollCount; i++)
            {
                if (IsRollLocked(i)) continue;
                var seqIndex = Mathf.Clamp(index,0,currentActiveSequence.Count-1);
                var maxIndex = currentActiveSequence[seqIndex].sequenceElements.Count;
                rollStartReelIndex[i] = ((int) panel.Columns[i].StopPosition - 1 + maxIndex) % maxIndex;
                anticipationInColumn[i] = false;
                index++;
            }
        }
        
        public override void UpdateCurrentActiveSequence(string sequenceName, List<int> startIndex = null)
        {
            currentSequenceName = sequenceName;
            if(sequenceElementConstructor == null)
                sequenceElementConstructor = machineState.machineContext.sequenceElementConstructor;
           
            currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

            if (startIndex != null && startIndex.Count == rollCount)
            {
                rollStartReelIndex = startIndex;
            }
            else
            {
                for (var i = 0; i < rollCount; i++)
                {
                    if (IsRollLocked(i)) continue;
                    rollStartReelIndex[i] = Random.Range(0, currentActiveSequence[0].sequenceElements.Count);
                }
            }
        }
        
        
        public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
        {
            return currentActiveSequence[0].sequenceElements;
        }
    }
}