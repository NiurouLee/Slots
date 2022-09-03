using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WheelState11026:WheelState
    {
        private ReelSequence reelSequence;
        
        public WheelState11026(MachineState state) : base(state)
        {
            
        }

        public uint GetElementId(int rollIndex, int rowIndex)
        {
            var sequenceElements = spinResultElementDef[rollIndex].sequenceElements;
            return sequenceElements[rowIndex+1].config.id;
        }
    }
}