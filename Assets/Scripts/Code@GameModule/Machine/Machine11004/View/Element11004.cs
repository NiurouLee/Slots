using UnityEngine;

namespace GameModule
{
    public class Element11004: Element
    {
        public Element11004(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
        }


        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11004>();
            
            UpdateShowGrayLayer(activeState.IsLink);
        }
    }
}