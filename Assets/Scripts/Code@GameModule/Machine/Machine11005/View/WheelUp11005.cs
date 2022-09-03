using UnityEngine;

namespace GameModule
{
    public class WheelUp11005: Wheel
    {
        public WheelUp11005(Transform transform) : base(transform)
        {
        }
        
        protected override void InitializeWheelMaskSortOrder()
        {
            const int ORDER_RANGE_PER_WHEEL = 400;

            wheelMask.backSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.backSortingOrder = 200;
            wheelMask.frontSortingOrder = ORDER_RANGE_PER_WHEEL;
        }
    }
}