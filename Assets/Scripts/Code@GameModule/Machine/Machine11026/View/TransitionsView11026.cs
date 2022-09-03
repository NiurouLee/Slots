using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionsView11026: TransformHolder
    {

        [ComponentBinder("Free")]
        protected Animator animatorFree;
        
        [ComponentBinder("Link")]
        protected Animator animatorLink;
        
        [ComponentBinder("Mouth")]
        protected Transform animatorPosition;
        
        [ComponentBinder("LinkTransition")]
        protected Animator animatorLinkTransitionsView;
        protected Animator animatorZhenping;

        public TransitionsView11026(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        public Vector3 GetIntegralPos()
        {
            return animatorPosition.transform.position;
        }

        public async Task PlayFreeTransition()
        {
            animatorFree.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            await XUtility.PlayAnimationAsync(animatorFree, "Free", context);
        }
        
        public void  PlayAnticipation()
        {
            animatorLink.gameObject.SetActive(true);
            var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            animatorZhenping = wheel.GetContext().transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animatorZhenping, "MachineContext");
            XUtility.PlayAnimation(animatorLink, "Anticipation");
        }
        
        public void HideDragon()
        {
             transform.gameObject.SetActive(false);
        }
        
        public void HideLink()
        {
             animatorLink.gameObject.SetActive(false);
        }
        
        public void ShowDragon()
        {
            transform.gameObject.SetActive(true);
            animatorLink.gameObject.SetActive(true);
        }

        public async Task PlaySuperFreeTransition()
        {
            animatorLink.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animatorLink, "Super", context);
        }
        
        public void PlayIdleransition()
        {
            animatorLink.gameObject.SetActive(true);
        }
        
        public void PlayLinkTransition()
        {
            AudioUtil.Instance.PlayAudioFx("LinkSpin_Video1");
            transform.gameObject.SetActive(true);
            animatorLinkTransitionsView.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorLinkTransitionsView, "LinkTransition", ()=>
            {
                animatorLinkTransitionsView.gameObject.SetActive(false);
            },context);
        }
        public void PlayFinishLinkTransition()
        {
            AudioUtil.Instance.PlayAudioFx("LinkSpin_Video1");
            transform.gameObject.SetActive(true);
            animatorLink.gameObject.SetActive(false);
            animatorLinkTransitionsView.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorLinkTransitionsView, "LinkTransition", ()=>
            {
                animatorLinkTransitionsView.gameObject.SetActive(false);
            },context);
        }
    }
}