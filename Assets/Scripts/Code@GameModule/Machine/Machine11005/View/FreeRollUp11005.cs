using UnityEngine;

namespace GameModule
{
    public class FreeRollUp11005: FreeRoll11005
    {
        public FreeRollUp11005(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName) : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
        }
        
        public override void PreComputeContainerBaseSortOrder()
        {
            containerBaseSortOrder = new int[containerCount + 1];
            
            for (var i = 0; i <= containerCount; i++)
            {
                containerBaseSortOrder[i] = 201 + rollIndex * containerCount + (topRowHasHighSortOrder ?  containerCount - i: i);
            }
        }
    }
}