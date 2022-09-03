//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-23 13:40
//  Ver : 1.0.0
//  Description : StepperRoll11007.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class StepperRoll11007:StepperRoll
    {
        public StepperRoll11007(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
        }

        protected override float GetStepSizeScale()
        {
            return 1.2f;
        }
    }
}