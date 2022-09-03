using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class SingleWheelState11022:WheelState
    {
        public SingleWheelState11022(MachineState state) : base(state)
        {
            wheelIsActive = false;
            currentSequenceName = Constant11022.ShapeLinkReels;
        }
        
        public override void UpdateReelSequences()
        {
            var tempReelSequences = sequenceElementConstructor?.GetReelSequences(currentSequenceName);
            if (tempReelSequences != null)
            {
                currentActiveSequence = new List<ReelSequence>() {
                    tempReelSequences[Constant11022.SingleWheelReelIndexDictionary[wheelName]]
                };
            }
        }
        public override IRollUpdaterEasingConfig GetEasingConfig()
        {
            return machineState.machineConfig.GetEasingConfig("SingleWheel");
        }
    }
}