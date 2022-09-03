

using TMPro;
using UnityEngine;

namespace GameModule
{
    public class FireBallElement11020 : Element
    {   
        public FireBallElement11020(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            transform.gameObject.SetActive(true);
            if (isStaticElement)
            {
                var animator = transform.GetComponent<Animator>();
                if (Constant11020.hasSpined)
                {
                    XUtility.PlayAnimation(animator, "Tail");
                }
                else
                {
                    XUtility.PlayAnimation(animator, "Idle");
                }
            }
        }
    }
}
