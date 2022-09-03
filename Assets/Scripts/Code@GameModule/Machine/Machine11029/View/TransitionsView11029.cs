using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionsView11029 : TransformHolder
    {
        [ComponentBinder("Free")] protected Animator animatorFree;

        [ComponentBinder("Map")] protected Animator animatorMap;

        public TransitionsView11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public void PlayFreeTransition()
        {
            animatorFree.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1"); 
            XUtility.PlayAnimation(animatorFree, "Free");
        }

        public void HideFreeTransition()
        {
            animatorFree.gameObject.SetActive(false);
        }

        public async Task PlayMapPointTransition()
        {
            animatorMap.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            await XUtility.PlayAnimationAsync(animatorMap, "Map", context);
            animatorMap.gameObject.SetActive(false);
        }
        
        public void PlayMapTransition()
        {
            AudioUtil.Instance.PlayAudioFx("Wheel_Video1");
            animatorMap.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorMap, "Map");
        }
        
        public void HideMapTransition()
        {
            animatorMap.gameObject.SetActive(false);
            XUtility.PlayAnimation(animatorMap, "Map");
        }
    }
}