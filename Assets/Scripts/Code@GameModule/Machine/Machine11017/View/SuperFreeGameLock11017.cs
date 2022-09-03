using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class SuperFreeGameLock11017: TransformHolder
    {
        private Animator _animator;
        private bool _isLockState = true;
        public SuperFreeGameLock11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            _animator = transform.GetComponent<Animator>();
            if(_animator)
                _animator.keepAnimatorControllerStateOnDisable = true;
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
           
            var pointerEventCustomHandler = transform.Find("Suobg").gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnUnlockFeatureClicked);
        }

        public void OnUnlockFeatureClicked(PointerEventData pointerEventData)
        {
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, 0);
        }

        public void LockSuperFree(bool isLock, bool playAnimation = true)
        {
            if (_animator == null)
                return;
            if (isLock && !_isLockState)
            {
                if (playAnimation)
                {
                    _animator.Play("LockSFG");
                    AudioUtil.Instance.PlayAudioFxOneShot("Lock");
                }
                else
                {
                    _animator.Play("LockSFGIdle");
                }

                _isLockState = true;
            }
            else if (!isLock && _isLockState)
            {
                if (playAnimation)
                {
                    AudioUtil.Instance.PlayAudioFxOneShot("Unlock");
                    _animator.Play("UnLock");
                }
                else
                {
                    _animator.Play("UnLockIdle");
                }

                _isLockState = false;
            }
        }
        
        //从free/link转场的时候调用此方法
        public void LockSuperFreeIdle(bool isLock)
        {
            if (isLock)
            {
                _animator.Play("LockSFGIdle");
                _isLockState = true;
            }
            else
            {
                _animator.Play("UnLockIdle");
                _isLockState = false;
            }
        }
    }
}