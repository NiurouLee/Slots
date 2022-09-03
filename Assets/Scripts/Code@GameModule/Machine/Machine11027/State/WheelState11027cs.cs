using System.Collections.Generic;

namespace GameModule
{
    public class WheelState11027:WheelState
    {
        private ReelSequence reelSequence;
        
        public WheelState11027(MachineState state) : base(state)
        {
            
        }
        
        // public override bool HasAnticipationAnimationInRollIndex(int rollIndex)
        // {
        //     if (rollIndex == 4)
        //     {
        //         if (showFourAnticipationAnimation(rollIndex))
        //         {
        //             return true;
        //         }
        //         else
        //         {
        //             return false;
        //         }
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }
        
        public int CalculateWildNums()
        {
            List<uint> idList = new List<uint>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    var sequenceElements = spinResultElementDef[i].sequenceElements; 
                    var id = sequenceElements[j].config.id;
                    if (Constant11027.ListWildElementIds.Contains(id))
                    {
                        idList.Add(id);
                    }
                }
            }
            return idList.Count;
        }
        
        // private bool showFourAnticipationAnimation(int rollIndex)
        // {
        //     List<uint> twoColumnWildList = new List<uint>();
        //     List<uint> threeColumnWildList = new List<uint>();
        //     if (rollIndex - 2 == 2)
        //     {
        //         for (var i = 1; i < 4; i++)
        //         {
        //             var sequenceElements = spinResultElementDef[rollIndex-2].sequenceElements;
        //             if (Constant11027.ScatterElementId == sequenceElements[i].config.id)
        //             {
        //                 twoColumnWildList.Add(sequenceElements[i].config.id);
        //             }
        //         }
        //     }
        //     if (rollIndex - 1 == 3)
        //     {
        //         for (var i = 1; i < 4; i++)
        //         {
        //             var sequenceElements = spinResultElementDef[rollIndex-1].sequenceElements;
        //             if (Constant11027.ScatterElementId == sequenceElements[i].config.id)
        //             {
        //                 threeColumnWildList.Add(sequenceElements[i].config.id);
        //             }
        //         }
        //     }
        //     if (twoColumnWildList.Count > 0 && threeColumnWildList.Count > 0)
        //     {
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //         ;
        //     }
        // }
    }
}