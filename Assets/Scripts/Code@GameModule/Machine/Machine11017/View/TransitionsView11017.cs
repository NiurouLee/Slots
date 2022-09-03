using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionsView11017: TransformHolder
    {

        [ComponentBinder("Free")]
        protected Animator animatorFree;
        
        [ComponentBinder("Link")]
        protected Animator animatorLink;

        public TransitionsView11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }

        public async Task PlayFreeTransition()
        {
            animatorFree.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("Free_Transitions");
            await XUtility.PlayAnimationAsync(animatorFree, "Free", context);
            animatorFree.gameObject.SetActive(false);
        }
        
        public async Task PlayLinkTransition()
        {
            animatorLink.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("Bonus_Transitions");
            await XUtility.PlayAnimationAsync(animatorLink, "Link", context);
            animatorLink.gameObject.SetActive(false);
        }
    }
}