using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class HighLightView11029 : TransformHolder
    {
       private Animator _animator;

        public HighLightView11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            _animator = transform.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
        }

        public void PlayHighLight( int rollIndex)
        {
            transform.gameObject.SetActive(true);
            switch (rollIndex)
            {
                case 0:
                    AudioUtil.Instance.PlayAudioFx("J02_Anticipation");
                    _animator.Play("Open");
                    break;
                case 1:
                    AudioUtil.Instance.PlayAudioFx("J02_Anticipation");
                    _animator.Play("Open1");
                    break;
                case 2:
                    AudioUtil.Instance.PlayAudioFx("J02_Anticipation");
                    _animator.Play("Open2");
                    break;
                case 3:
                    AudioUtil.Instance.PlayAudioFx("J02_Anticipation");
                    _animator.Play("Open3");
                    break;
            }
        }
        
        public void HideHighLight()
        {
            transform.gameObject.SetActive(false);
        }
    }
}