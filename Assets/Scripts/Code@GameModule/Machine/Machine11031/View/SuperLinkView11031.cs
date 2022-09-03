using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class SuperLinkView11031 : TransformHolder
    {
        private Animator animator;

        public SuperLinkView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task Open()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("SuperRespin_Start");
            transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animator, "Trans");
            transform.gameObject.SetActive(false);
        }

        public async Task Close()
        {
            // await XUtility.PlayAnimationAsync(animator, "UIRespinStart_Close");
            transform.gameObject.SetActive(false);
        }
    }
}