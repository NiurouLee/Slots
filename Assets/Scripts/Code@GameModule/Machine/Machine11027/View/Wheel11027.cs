using UnityEngine;

namespace GameModule
{
    public class Wheel11027: Wheel
    {
        public Wheel11027(Transform transform) : base(transform)
        {
        }
        
        public override void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState,
            string inWheelElementSortingLayerName = "Element")
        {
            base.BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(inWheelState, inWheelElementSortingLayerName);
            wheelMask.gameObject.SetActive(false);
        }
    }
}