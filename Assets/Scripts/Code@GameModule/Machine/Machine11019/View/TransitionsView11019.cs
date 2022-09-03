using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class TransitionsView11019: TransformHolder
    {

        [ComponentBinder("Free")]
        protected Animator animatorFree;

        [ComponentBinder("Link")]
        protected Animator animatorLink;

        [ComponentBinder("SpineMan")]
        protected Animator animatorSpineMan;
        
        public TransitionsView11019(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }




        public async Task PlayFreeTransition()
        {
            animatorFree.gameObject.SetActive(true);
            // XUtility.PlayAnimation(animatorFree, "Free", () =>
            // {
            //     if (animatorFree != null)
            //     {
            //         animatorFree.gameObject.SetActive(false);
            //     }
            // }, context);
            // await context.WaitSeconds(1.6f);

            AudioUtil.Instance.PlayAudioFx("Free_Video");
            await XUtility.PlayAnimationAsync(animatorFree, "Free", context);
            animatorFree.gameObject.SetActive(false);
        }
        
        
        public async Task PlayLinkTransition()
        {
            animatorLink.gameObject.SetActive(true);
            // XUtility.PlayAnimation(animatorLink, "Link", () =>
            // {
            //     if (animatorLink != null)
            //     {
            //         animatorLink.gameObject.SetActive(false);
            //
            //     }
            // }, context);
            // await context.WaitSeconds(1.6f);

            AudioUtil.Instance.PlayAudioFx("Link_Video");
            await XUtility.PlayAnimationAsync(animatorLink, "Link", context);
            animatorLink.gameObject.SetActive(false);
        }


        public void OpenSpineMan()
        {
            animatorSpineMan.gameObject.SetActive(true); 
        }

        public void CloseSpineMan()
        {
            animatorSpineMan.gameObject.SetActive(false);
        }

        public async Task PlayManFree()
        {
            AudioUtil.Instance.PlayAudioFx("Npc_Play");
            await XUtility.PlayAnimationAsync(animatorSpineMan, "Free", context);
            
        }
        
        public async Task PlayManLink()
        {
            await XUtility.PlayAnimationAsync(animatorSpineMan, "Link", context);

        }
        
        public async Task PlayManBase()
        {
            await XUtility.PlayAnimationAsync(animatorSpineMan, "Base", context);
            XUtility.PlayAnimation(animatorSpineMan,"Idle");
        }
    }
}