using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FreeRemaining11017: TransformHolder
    {
        [ComponentBinder("GroupReSpin/txtReSpinCount")]
        protected TextMesh txtSpinCount;
        
        [ComponentBinder("Spins")]
        protected Transform tranSpins;

        [ComponentBinder("Spin")]
        protected Transform tranSpin;
        
        protected Animator animatorSpin;
        
        public FreeRemaining11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }

        public void RefreshReSpinCount()
        {
            FreeSpinState freeSpinState = context.state.Get<FreeSpinState>();
            if (freeSpinState.IsInFreeSpin || freeSpinState.IsTriggerFreeSpin)
            {
                if (freeSpinState.LeftCount == 0)
                {
                     txtSpinCount.text = (freeSpinState.LeftCount).ToString();
                }
                else
                {
                     txtSpinCount.text = (freeSpinState.LeftCount-1).ToString();
                }

                if (freeSpinState.LeftCount - 1 <= 1 || freeSpinState.LeftCount == 0) 
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
        public void ShowRemaining()
        {
            FreeSpinState freeSpinState = context.state.Get<FreeSpinState>();
           
            if (freeSpinState.IsInFreeSpin || freeSpinState.IsTriggerFreeSpin)
            {
                txtSpinCount.text = (freeSpinState.LeftCount).ToString();
                if (freeSpinState.LeftCount-1<= 1) 
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