using UnityEngine;

namespace GameModule
{
    public class Element11012:Element
    {
        public Element11012(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
        }
        
        
        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11012>();
            UpdateShowGrayLayer(activeState.IsInLink);
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            
        }
    }
    
}