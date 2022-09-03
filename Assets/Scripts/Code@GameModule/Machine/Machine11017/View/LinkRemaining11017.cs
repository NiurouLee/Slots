using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkRemaining11017 : TransformHolder
    {
        [ComponentBinder("GroupReSpin/txtReSpinCount")]
        protected TextMesh txtSpinCount;

        [ComponentBinder("Spins")] protected Transform tranSpins;

        [ComponentBinder("Spin")] protected Transform tranSpin;

        protected Animator animatorSpin;

        public LinkRemaining11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animatorSpin = transform.GetComponent<Animator>();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }

        public void RefreshReSpinCount(bool isStartSpin)
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();
            if (reSpinState.IsInRespin || reSpinState.ReSpinTriggered)
            {
                if (reSpinState.ReSpinCount == 0)
                {
                    txtSpinCount.text = (reSpinState.ReSpinCount).ToString();
                }
                else
                {
                    txtSpinCount.text = (reSpinState.ReSpinCount - 1).ToString();
                }

                if (reSpinState.ReSpinCount == 0 || reSpinState.ReSpinCount - 1 <= 1)
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
        }

        public void ShowReSpinRemaining()
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();
            if (reSpinState.IsInRespin || reSpinState.ReSpinTriggered)
            {
                txtSpinCount.text = (reSpinState.ReSpinCount).ToString();
                if (reSpinState.ReSpinCount == 0 || reSpinState.ReSpinCount - 1 <= 1)
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
        }
    }
}