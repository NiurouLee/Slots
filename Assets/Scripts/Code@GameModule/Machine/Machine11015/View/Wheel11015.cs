using UnityEngine;

namespace GameModule
{
    public class Wheel11015: Wheel
    {
        
        protected Vector3 longPos = new Vector3(0.019f,4.63f,0);
        protected Vector3 longScale = new Vector3(13, 27.360f, 1);
        
        
        protected Vector3 shortPos = new Vector3(0.019f,0.17f,0);
        protected Vector3 shortScale = new Vector3(13, 9.12f, 1);

        
        public Wheel11015(Transform transform) : base(transform)
        {
        }


        public override void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState,
            string inWheelElementSortingLayerName = "Element")
        {
            base.BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(inWheelState, inWheelElementSortingLayerName);

            Transform tranMask = wheelMask.transform;
            tranMask.localPosition = shortPos;
            tranMask.localScale = shortScale;
        }
    }
}