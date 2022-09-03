//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-05 20:34
//  Ver : 1.0.0
//  Description : StepperRoll11016.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class StepperRoll11016:StepperRoll
    {
        public StepperRoll11016(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
        }
        
        public override void PreComputeContainerBaseSortOrder()
        {
            containerBaseSortOrder = new int[containerCount + 1];
            
            for (var i = 0; i <= containerCount; i++)
            {
                containerBaseSortOrder[i] = 1050 + rollIndex * containerCount + (topRowHasHighSortOrder ?  containerCount - i: i);
            }
        }

        protected override float GetStepSizeScale()
        {
            return 1.4f;
        }
    }
}