using UnityEngine;

namespace GameModule
{
    public class Element11031: Element
    {
        public Element11031(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            
        }

        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11031>();
            UpdateShowGrayLayer(activeState.IsInLink);
        }
    }
}