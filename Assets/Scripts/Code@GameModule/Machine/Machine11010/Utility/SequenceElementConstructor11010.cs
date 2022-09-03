//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-09 21:50
//  Ver : 1.0.0
//  Description : SequenceElementConstructor11010.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class SequenceElementConstructor11010: SequenceElementConstructor
    {
        public SequenceElementConstructor11010(MachineContext inMachineContext)
            : base(inMachineContext)
        {
            
        }
        protected override ReelSequence GetFixedReelSequence(List<ReelSequence> sequences, int column, WheelState wheelState)
        {
            if (wheelState.wheelName.Contains("Link"))
            {
                return sequences[0];
            }
            return base.GetFixedReelSequence(sequences, column, wheelState);
        }
    }
}