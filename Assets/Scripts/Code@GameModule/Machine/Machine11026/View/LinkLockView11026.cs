using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LinkLockView11026: TransformHolder
    {
        protected Transform Tips;
        private Animator _animator;
        [ComponentBinder("RotationTips/Spinleft")]
        protected Transform spinLeft;
        
        [ComponentBinder("RotationTips/Spinsleft")]
        protected Transform spinsLeft;
        
        [ComponentBinder("Num")]
        protected Transform textRemaining;
        
        [ComponentBinder("RotationTips")]
        protected Transform rotationTip;

        protected int lastRespinCount = -1;

        protected List<LinkLockItem11026> listItem = new List<LinkLockItem11026>();
        public LinkLockView11026(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
             Tips = inTransform;
            _animator = rotationTip.GetComponent<Animator>();
            if(_animator)
                _animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task ShowReSpinCount()
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();

            if (reSpinState.IsInRespin || reSpinState.ReSpinTriggered)
            {
                var count = reSpinState.ReSpinCount;
                for (int i = 0; i < 4; i++)
                {
                    if (count == i)
                    {
                        textRemaining.Find(i+"").gameObject.SetActive(true);
                    }
                    else
                    {
                        textRemaining.Find(i+"").gameObject.SetActive(false);
                    }
                }

                if (reSpinState.ReSpinCount == 0)
                {
                    spinLeft.gameObject.SetActive(false);
                    spinsLeft.gameObject.SetActive(false);
                }
                else
                {
                    if (reSpinState.ReSpinCount <= 1)
                    {
                        spinLeft.gameObject.SetActive(true);
                        spinsLeft.gameObject.SetActive(false);
                    }
                    else
                    {
                        spinLeft.gameObject.SetActive(false);
                        spinsLeft.gameObject.SetActive(true);
                    }
                }
                if (count != lastRespinCount) 
                { 
                    if (lastRespinCount != -1) 
                    { 
                        if (count>lastRespinCount)
                        { 
                            await PlayRefresh();
                        }
                    }
                    lastRespinCount = ((int)count - 1);
                }
            }
        }
        
        public void RefreshReSpinCount()
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();

            if (reSpinState.IsInRespin || reSpinState.ReSpinTriggered)
            {
                
                var count = reSpinState.ReSpinCount;
                if (reSpinState.ReSpinCount == 0)
                {
                    count = 0;
                }
                else
                {
                    count = reSpinState.ReSpinCount-1;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (count == i)
                    {
                        textRemaining.Find(i+"").gameObject.SetActive(true);
                    }
                    else
                    {
                        textRemaining.Find(i+"").gameObject.SetActive(false);
                    }
                }

                if (reSpinState.ReSpinCount == 0)
                {
                    spinLeft.gameObject.SetActive(false);
                    spinsLeft.gameObject.SetActive(false);
                }
                else
                {
                    if (reSpinState.ReSpinCount - 1 <= 1)
                    {
                        if (reSpinState.ReSpinCount - 1 == 0)
                        {
                             spinLeft.gameObject.SetActive(false); 
                             spinsLeft.gameObject.SetActive(false);
                        }
                        else
                        {
                             spinLeft.gameObject.SetActive(true); 
                             spinsLeft.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        spinLeft.gameObject.SetActive(false);
                        spinsLeft.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void HideRotationTips()
        {
            Tips.gameObject.SetActive(false);
        }
        
        public void ShowRotationTips()
        {
            Tips.gameObject.SetActive(true);
        }

        public void Reset()
        {
            lastRespinCount = -1;
        }

        public async Task PlayRefresh()
        {
            AudioUtil.Instance.PlayAudioFx("J01Spins__Raise");
            await XUtility.PlayAnimationAsync(_animator, "RotationTips", context);
            await context.WaitSeconds(0.67f);
        }
    }
}