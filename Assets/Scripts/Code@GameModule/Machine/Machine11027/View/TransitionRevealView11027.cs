using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionRevealView11027: TransformHolder
    {
        private Animator animator;
        public TransitionRevealView11027(Transform inTransform) : base(inTransform)
        {
            animator = transform.GetComponent<Animator>();
        }

        public async Task PlayCharacterAnimation()
        {
            transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animator,"Reveal");
            transform.gameObject.SetActive(false);
        }
        
        public void PlayFreeTransitionAnimation()
        {
            transform.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            XUtility.PlayAnimation(animator,"TransitionAnimation");
        }
    }
}