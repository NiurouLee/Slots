using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class LinkLockView11301: TransformHolder
    {
        
        [ComponentBinder("BGGlow")]
        protected Animator animatorReSpin;

        [ComponentBinder("CountText")]
        protected TextMesh txtReSpinCount;

        [ComponentBinder("SpinSprite")]
        protected Transform tranSpinTip;

        [ComponentBinder("SpinsSprite")]
        protected Transform tranSpinTips;

        [ComponentBinder("LastGroup")]
        protected Transform tranLastTip;
        
        
        public LinkLockView11301(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            //animatorReSpin = transform.GetComponent<Animator>();
            
        }
        
        
        public void Open()
        {
            transform.gameObject.SetActive(true);
            RefreshReSpinCount(false);
        }


        public void Close()
        {
            transform.gameObject.SetActive(false);
        }
        
        
        public async Task RefreshReSpinCount(bool isStartSpin)
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();

            if (reSpinState.IsInRespin || reSpinState.ReSpinTriggered)
            {
                uint displayCount = 0;
            
                if (isStartSpin)
                {
                    displayCount = reSpinState.ReSpinCount - 1;
                }
                else
                {
                    displayCount = reSpinState.ReSpinCount;
                
                }
            
                txtReSpinCount.gameObject.SetActive(false);
                tranSpinTip.gameObject.SetActive(false);
                tranSpinTips.gameObject.SetActive(false);
                tranLastTip.gameObject.SetActive(false);
                if (displayCount > 1)
                {
                    txtReSpinCount.gameObject.SetActive(true);
                    tranSpinTips.gameObject.SetActive(true);
                }
                else if (displayCount == 1)
                {
                    txtReSpinCount.gameObject.SetActive(true);
                    tranSpinTip.gameObject.SetActive(true);
                }
                else
                {
                    tranLastTip.gameObject.SetActive(true);
                }



                txtReSpinCount.text =  displayCount.ToString();
                    
                if (!isStartSpin && reSpinState.ReSpinLimit == reSpinState.ReSpinCount)
                {
                    if (!reSpinState.ReSpinTriggered)
                    {
                        AudioUtil.Instance.PlayAudioFx("Link_SpinRefresh");
                        animatorReSpin.gameObject.SetActive(true);
                        await XUtility.PlayAnimationAsync(animatorReSpin, "BG_Glow", context);
                    }
                    else
                    {
                        animatorReSpin.gameObject.SetActive(false);
                    }
                }
                else if (!isStartSpin)
                {
                    await context.WaitSeconds(1.2f);
                }
            }

            


        }

    }
}