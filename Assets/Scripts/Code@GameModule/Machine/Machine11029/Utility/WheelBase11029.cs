using UnityEngine;

namespace GameModule
{
    public class WheelBase11029: Wheel
    {
        public WheelBase11029(Transform transform) : base(transform)
        {
        }
        protected override void InitializeWheelMaskSortOrder()
        {
            const int ORDER_RANGE_PER_WHEEL = 700;

            wheelMask.backSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.backSortingOrder = -1;
            wheelMask.frontSortingOrder = ORDER_RANGE_PER_WHEEL;
        }
    }
}