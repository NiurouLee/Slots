//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-17 15:21
//  Ver : 1.0.0
//  Description : SequenceElementConstructor11007.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class SequenceElementConstructor11007: SequenceElementConstructor
    {
        public SequenceElementConstructor11007(MachineContext inMachineContext)
        : base(inMachineContext)
        {
            
        }
        
        protected override ReelSequence GetFixedReelSequence(List<ReelSequence> sequences, int column, WheelState wheelState)
        {
            if (wheelState.wheelName.Contains("Link"))
            {
                return sequences[0];
            }
            column = Math.Min(column, sequences.Count - 1);
            return base.GetFixedReelSequence(sequences, column, wheelState);
        }
    }
}