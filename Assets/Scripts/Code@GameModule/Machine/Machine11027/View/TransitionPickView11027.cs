using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionPickView11027: TransformHolder
    {
        private Animator animator;
        public TransitionPickView11027(Transform inTransform) : base(inTransform)
        {
            animator = transform.GetComponent<Animator>();
        }

        public async Task PlayCharacterAnimation()
        {
            transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animator,"TransitionCutOutPick");
            transform.gameObject.SetActive(false);
        }
        
        public void PlayPickTransitionAnimation()
        {
            transform.gameObject.SetActive(true); 
            AudioUtil.Instance.PlayAudioFx("LinkSpin_Video1"); 
            XUtility.PlayAnimation(animator, "TransitionCutOutPick", ()=>
            {
                 transform.gameObject.SetActive(false);
            },context);
        }
    }
}