using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelFeature11027 : TransformHolder
    {
        private Animator animator;
        public WheelFeature11027(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task PlayToWheel()
        {
            AudioUtil.Instance.PlayAudioFx("Wheel_DropDown");
            transform.gameObject.SetActive(true);
            animator.Play("ToWheel");
        }
        
        public async Task PlayToBase()
        {
            AudioUtil.Instance.PlayAudioFx("Wheel_PullUp");
            transform.gameObject.SetActive(true);
            animator.Play("BackWheel");
        }
        
        public void PlayWheelIdle()
        {
            transform.gameObject.SetActive(true);
            animator.Play("WheelIdle");
        }
         
        public void  PlayBaseIdle()
        {
            transform.gameObject.SetActive(true);
            animator.Play("BaseIdle");
        }
        
        public void PlayPickIdle()
        {
            transform.gameObject.SetActive(true);
            animator.Play("PickIdle");
        }
    }
}
