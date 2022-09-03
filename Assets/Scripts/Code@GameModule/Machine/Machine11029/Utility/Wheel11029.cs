using UnityEngine;

namespace GameModule
{
    public class Wheel11029: Wheel
    {
        public Wheel11029(Transform transform) : base(transform)
        {
        }
        
        public override void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState,
            string inWheelElementSortingLayerName = "Element")
        {
            base.BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(inWheelState, inWheelElementSortingLayerName);
            wheelMask.gameObject.SetActive(false);
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