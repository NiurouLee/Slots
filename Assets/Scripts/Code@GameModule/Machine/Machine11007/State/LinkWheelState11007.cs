//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-17 14:36
//  Ver : 1.0.0
//  Description : LinkWheelState11007.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class LinkWheelState11007: WheelState
    {
        public LinkWheelState11007(MachineState state) : base(state)
        {
            wheelIsActive = false;
            currentSequenceName = Constant11007.LinkReels;
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
        
        protected override void UpdateStartAndStopIndex(Panel panel)
        {
            currentSequenceName = panel.ReelsId;
            currentActiveSequence = sequenceElementConstructor.GetReelSequences(currentSequenceName);

            int index = 0;
            var seqCount = currentActiveSequence.Count;
            for (var i = 0; i < rollCount; i++)
            {
                if (IsRollLocked(i)) continue;
                var maxIndex = currentActiveSequence[Math.Min(index,seqCount-1)].sequenceElements.Count;
                rollStartReelIndex[i] = ((int) panel.Columns[i].StopPosition - 1 + maxIndex) % maxIndex;
                anticipationInColumn[i] = false;
                index++;
            }
        }
        public override List<SequenceElement> GetActiveSequenceElement(Roll roll)
        {
            return currentActiveSequence[0].sequenceElements;
        }
        
        
        public override List<SequenceElement> GetActiveSequenceElement(int rollIndex)
        {
            return currentActiveSequence[0].sequenceElements;
        }
        

        public void ResetLinkWheelState()
        {
            for (int i = 0; i < rollCount; i++)
            {
                SetRollLockState(i, false);
            }
        }
    }
}