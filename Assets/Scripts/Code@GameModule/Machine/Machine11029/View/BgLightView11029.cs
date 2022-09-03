using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class BgLightView11029 : TransformHolder
    {
       private Animator _animator;

        public BgLightView11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            _animator = transform.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
        }

        public void PlayBgLight()
        {
            transform.gameObject.SetActive(true);
            _animator.gameObject.SetActive(true);
            XUtility.PlayAnimation(_animator, "Trail");
        }
        public void HideBgLight()
        {
            transform.gameObject.SetActive(false);
        }
    }
}