using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionWheelView11027: TransformHolder
    {
        private Animator animator;
        public TransitionWheelView11027(Transform inTransform) : base(inTransform)
        {
            animator = transform.GetComponent<Animator>();
        }

        public async Task PlayCharacterAnimation()
        {
            transform.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("Expect");
            await XUtility.PlayAnimationAsync(animator,"TransitionCutOut");
            transform.gameObject.SetActive(false);
        }
        
        public void PlayWheelTransitionAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            transform.gameObject.SetActive(true);
            // AudioUtil.Instance.PlayAudioFx("Expect"); 
            XUtility.PlayAnimation(animator, "TransitionCutOut", ()=>
            {
                 transform.gameObject.SetActive(false);
            },context);
        }
        
        public void PlayPickTransitionAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("LinkSpin_Video2"); 
            transform.gameObject.SetActive(true);
            // AudioUtil.Instance.PlayAudioFx("Expect"); 
            XUtility.PlayAnimation(animator, "TransitionCutOut", ()=>
            {
                 transform.gameObject.SetActive(false);
            },context);
        }
    }
}