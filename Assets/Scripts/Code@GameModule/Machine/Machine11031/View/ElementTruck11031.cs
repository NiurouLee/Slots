using UnityEngine;

namespace GameModule
{
    public class ElementTruck11031 : Element
    {
        public ElementTruck11031(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11031>();
            var _extraState11031 = sequenceElement.machineContext.state.Get<ExtraState11031>();
            var activeWheelCount = _extraState11031.GetLinkActivePanelCount();
            var wheels = sequenceElement.machineContext.state.Get<WheelsActiveState11031>().GetRunningWheel();
            bool isGray = false;
            if (wheels.Count == 4)
            {
                if (wheels[wheels.Count - 1].transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0)
                    .IsName("WheelSuperRespin_ActiveIdle"))
                {
                    isGray = true;
                }
            }

            UpdateShowGrayLayer(activeState.IsInLink && activeWheelCount >= 4 && isGray);
        }
    }
}