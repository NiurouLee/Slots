using UnityEngine;

namespace GameModule
{
    public class Element11019: Element
    {
        public Element11019(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            
        }

        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11019>();
            UpdateShowGrayLayer(activeState.IsInLink);
        }
    }
}