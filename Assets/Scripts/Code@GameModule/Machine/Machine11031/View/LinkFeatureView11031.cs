using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkFeatureView11031 : TransformHolder
    {
        private Animator animator;

        public LinkFeatureView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task Open()
        {
            AudioUtil.Instance.PlayAudioFx("Respin_Start");
            transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animator, "UIRespinStart_open");
            // AudioUtil.Instance.PlayAudioFx("Respin_End");
            await XUtility.PlayAnimationAsync(animator, "UIRespinStart_Close");
        }

        public async Task Close()
        {
            await XUtility.PlayAnimationAsync(animator, "UIRespinStart_Close");
            transform.gameObject.SetActive(false);
        }
    }
}