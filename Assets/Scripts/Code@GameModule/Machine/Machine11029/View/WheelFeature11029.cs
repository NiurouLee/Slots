using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelFeature11029 : TransformHolder
    {
        private Animator animator;
        public WheelFeature11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task Temp()
        {
            await context.WaitSeconds(2.0f);
            transform.gameObject.SetActive(false);
        }

        public async Task Open()
        {
            AudioUtil.Instance.PlayAudioFx("WheelStart_Open");
            transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animator, "Open");
            await context.WaitSeconds(1.0f);
        }

        public void Close()
        {
             transform.gameObject.SetActive(false);
            // await context.WaitSeconds(2.0f);
            // await XUtility.PlayAnimationAsync(animator, "Close");
        }
    }
}
