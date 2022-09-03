using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkRemaining11031 : TransformHolder
    {
        [ComponentBinder("Nums/UIRespinRemaining_Num0")]
        protected Transform txtSpinCount0;

        [ComponentBinder("Nums/UIRespinRemaining_Num1")]
        protected Transform txtSpinCount1;

        [ComponentBinder("Nums/UIRespinRemaining_Num2")]
        protected Transform txtSpinCount2;

        [ComponentBinder("Nums/UIRespinRemaining_Num3")]
        protected Transform txtSpinCount3;

        [ComponentBinder("UIRespinsRemaining")]
        protected Transform tranSpins;

        [ComponentBinder("UIRespinRemaining")] protected Transform tranSpin;

        protected Animator animatorSpin;

        protected Transform[] remainGroupNodes;

        public LinkRemaining11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animatorSpin = transform.GetComponent<Animator>();
            remainGroupNodes = new[] {txtSpinCount0, txtSpinCount1, txtSpinCount2, txtSpinCount3};
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }

        public async void ChangeReSpinCount()
        {
            await RefreshReSpinCount(true);
        }

        public async Task RefreshReSpinCount(bool isStartSpin, bool playAnimation = true)
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();

            if (reSpinState.IsInRespin || reSpinState.ReSpinTriggered)
            {
                if (isStartSpin)
                {
                    var count = reSpinState.ReSpinCount - 1;
                    for (int i = 0; i < 4; i++)
                    {
                        if (count == i)
                        {
                            remainGroupNodes[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            remainGroupNodes[i].gameObject.SetActive(false);
                        }
                    }

                    if (reSpinState.ReSpinCount - 1 == 1)
                    {
                        tranSpin.gameObject.SetActive(true);
                        tranSpins.gameObject.SetActive(false);
                    }
                    else
                    {
                        tranSpin.gameObject.SetActive(false);
                        tranSpins.gameObject.SetActive(true);
                    }
                }
                else
                {
                    var count = reSpinState.ReSpinCount;
                    for (int i = 0; i < 4; i++)
                    {
                        if (count == i)
                        {
                            remainGroupNodes[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            remainGroupNodes[i].gameObject.SetActive(false);
                        }
                    }

                    if (reSpinState.ReSpinCount == 1)
                    {
                        tranSpin.gameObject.SetActive(true);
                        tranSpins.gameObject.SetActive(false);
                    }
                    else
                    {
                        tranSpin.gameObject.SetActive(false);
                        tranSpins.gameObject.SetActive(true);
                    }

                    if (reSpinState.ReSpinLimit == reSpinState.ReSpinCount && playAnimation)
                    {
                        AudioUtil.Instance.PlayAudioFxOneShot("Respins_Refresh");
                        await XUtility.PlayAnimationAsync(animatorSpin, "UIRespinNum_Active", context);
                    }

                    if (reSpinState.ReSpinLimit != reSpinState.ReSpinCount && playAnimation)
                    {
                        //AudioUtil.Instance.PlayAudioFxOneShot("Respin_Alert");
                    }
                }
            }
        }

        public void ShowLinkRemains(bool enable)
        {
            transform.gameObject.SetActive(enable);
        }
    }
}