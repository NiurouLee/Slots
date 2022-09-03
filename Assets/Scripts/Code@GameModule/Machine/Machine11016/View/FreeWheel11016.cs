//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-06 15:29
//  Ver : 1.0.0
//  Description : FreeWheel11016.cs
//  ChangeLog :
//  **********************************************

using ILRuntime.Runtime;
using UnityEngine;

namespace GameModule
{
    public class FreeWheel11016:Wheel
    {
        public FreeWheel11016(Transform transform) : base(transform)
        {

        }
        protected override void InitializeWheelMaskSortOrder()
        {
            var index = wheelName[wheelName.Length-1]-'0';
            wheelMask.backSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingOrder = (index+1)*200;
            wheelMask.backSortingOrder = index * 200-1;
        }
    }
}