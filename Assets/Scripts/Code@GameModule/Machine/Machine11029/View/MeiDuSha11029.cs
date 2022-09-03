using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class MeiDuSha11029 : TransformHolder
    {
        private Animator animator;
        [ComponentBinder("Meidusha")]
        protected Transform MeiDuSha;

        // [ComponentBinder("Fx_FreeGlow")] protected Transform freeGlow;

        [ComponentBinder("Fx_FreeLight")] protected Transform freeLight;

        [ComponentBinder("Fx_Anticpation")] protected Transform fxAni;

        [ComponentBinder("Fx_AnticpationLight")]
        protected Transform fxAniLight;

        public MeiDuSha11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = MeiDuSha.transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        public void ShowMeiDuSha(bool enable)
        {
            transform.gameObject.SetActive(enable);
        }
        
        public void ShowBaseBonusAni(bool enable)
        {
            fxAni.transform.gameObject.SetActive(enable);
        }
        
        public void ShowBaseBonusLight(bool enable)
        {
            fxAniLight.transform.gameObject.SetActive(enable);
        }

        //
        public async Task ShowFreeGlow()
        {
            // freeGlow.transform.gameObject.SetActive(true);
            await context.WaitSeconds(2.5f);
            // freeGlow.transform.gameObject.SetActive(false);
        }

        public void PlayShowFire()
        {
            AudioUtil.Instance.PlayAudioFx("FreeGame_Character_Video1");
            animator.Play("FreeAppear");
        }
        
        public void PlayShowAppear()
        {
            AudioUtil.Instance.PlayAudioFx("FreeGame_Character_Video1");
            animator.Play("Appear");
        }
        
        public void PlayOpenMeiDuSha()
        {
            animator.Play("Open");
        }
        
        public void PlayIdle()
        {
            animator.Play("Idle");
        }
        
        public void PlayInFree()
        {
            freeLight.transform.gameObject.SetActive(false);
            animator.Play("FreeMeidusha");
        }
        
        public void ShowLight()
        {
            freeLight.transform.gameObject.SetActive(true);
        }
        
        public void HideLight()
        {
            freeLight.transform.gameObject.SetActive(false);
        }

        public void PlayInBase()
        {
             // freeGlow.transform.gameObject.SetActive(false);
             freeLight.transform.gameObject.SetActive(false);
             animator.Play("Idle");
        }
        
        public void PlayInClose()
        {
             animator.Play("Close");
        }
    }
}
