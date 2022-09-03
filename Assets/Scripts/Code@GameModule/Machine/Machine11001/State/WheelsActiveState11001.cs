// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/12/15:17
// Ver : 1.0.0
// Description : WheelsActiveState11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class WheelsActiveState11001:WheelsActiveState
    {
        public WheelsActiveState11001(MachineState machineState) : base(machineState)
        {
            
        }
       
        public override string GetReelNameForWheel(Wheel wheel)
        {
            return "Reels";
        }
    }
}