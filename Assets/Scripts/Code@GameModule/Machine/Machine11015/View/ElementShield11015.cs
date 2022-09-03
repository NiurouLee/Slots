using UnityEngine;

namespace GameModule
{
    public class ElementShield11015: Element
    {
        public ElementShield11015(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
        }


        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            var listPic = containerTransform.GetComponentsInChildren<SpriteRenderer>(true);

            var context = sequenceElement.machineContext;
            var freeSpinState = context.state.Get<FreeSpinState>();
            foreach (var picAlpha in listPic)
            {
                if (picAlpha.gameObject.name == "SymbolAlpha")
                {

                    picAlpha.gameObject.SetActive(!freeSpinState.IsInFreeSpin);
                    
                    picAlpha.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                }
            }
        }
    }
}