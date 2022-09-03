using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionView11021 : TransformHolder
    {
        [ComponentBinder("ZhenpingAnim")]
        protected Animator animatorZhenping;
        
        public TransitionView11021(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            
        }

        
        public async Task ShowWinShanks()
        {
            await XUtility.PlayAnimationAsync(animatorZhenping, "Win", context);
        }

        public async Task ShowFreeShanks()
        {
            
            await XUtility.PlayAnimationAsync(animatorZhenping, "Open", context);
        }
        
        public async Task ShowPre1Shanks()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("AnticipationSound02");
            await XUtility.PlayAnimationAsync(animatorZhenping, "Pre1", context);
        }
        
        public async Task ShowPre2Shanks()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("AnticipationSound");
            await XUtility.PlayAnimationAsync(animatorZhenping, "Pre2", context);
        }
        
        public async Task ShowPreFreeShanks()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("AnticipationSound");
            await XUtility.PlayAnimationAsync(animatorZhenping, "PreFree", context);
        }
        
    }
}