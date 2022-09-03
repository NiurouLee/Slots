//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-25 12:25
//  Ver : 1.0.0
//  Description : SoloRoll11011.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class SoloRoll11011:SoloRoll
    {
        public SoloRoll11011(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
            
        }
        
        public override void PreComputeContainerInitializePosition()
        {
            base.PreComputeContainerInitializePosition();
            containerInitPos[0] += 0.5f;
        }
    }
}