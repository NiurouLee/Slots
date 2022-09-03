//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-06 15:33
//  Ver : 1.0.0
//  Description : FreeRoll11016.cs
//  ChangeLog :
//  **********************************************

using ILRuntime.Runtime;
using UnityEngine;

namespace GameModule
{
    public class FreeRoll11016:Roll
    {
        public FreeRoll11016(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder,inElementSortLayerName)
        {
        }
        
        public override void PreComputeContainerBaseSortOrder()
        {
            var name = transform.parent.parent.name;
            var index = name[name.Length-1] - '0';
            containerBaseSortOrder = new int[containerCount + 1];
            
            for (var i = 0; i <= containerCount; i++)
            {
                containerBaseSortOrder[i] = rollIndex * containerCount + (topRowHasHighSortOrder ?  containerCount - i: i) + index*200;
            }
        }
        
        public override int  GetSpinningDurationMultiplier(int wheelStartIndex, int updaterIndex, int updaterRollStopIndex)
        {
            return updaterIndex%3;
        }
        
        public override int  GetSpinningDurationMultiplier(int wheelStartIndex, int preUpdaterIndex, int preRollIndex, int preUpdaterStopIndex, int updaterIndex, int updaterRollStopIndex)
        {
            return (updaterIndex - preUpdaterIndex - 1)%3;
        }
    }
}