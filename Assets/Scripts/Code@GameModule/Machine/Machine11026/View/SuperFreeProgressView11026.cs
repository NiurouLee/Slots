// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/14:58
// Ver : 1.0.0
// Description : SuperFreeProgressView11001.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace GameModule
{
    public class SuperFreeProgressView11026: TransformHolder
    {
        [ComponentBinder("LockState")] 
        private Transform _lockState;
        
        [ComponentBinder("BG/BtnTipsBg2")] 
        private Transform _lockTips;
         
        private List<Transform> potList;

        private ExtraState11026 _extraState11026;

        private bool _isLockState = true;
        
        private bool _buttonResponseEnabled = true;
        
         protected Transform LockSFG;

        private Animator _animator;
        public SuperFreeProgressView11026(Transform inTransform)
            : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
                         LockSFG = inTransform;
            _animator = LockSFG.GetComponent<Animator>();
            if(_animator) 
                _animator.keepAnimatorControllerStateOnDisable = true; 
            _lockTips.parent.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(iconClick);
        }
        
        public async void iconClick(PointerEventData pointerEventData)
        {
            if (!_lockTips.gameObject.activeSelf)
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Tips_UnClick");
                XUtility.ShowTipAndAutoHide(_lockTips, 5, 0.2f, true, context);
            }
            else
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Tips_Click");
            }
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            var bg = transform.Find("BG");
            var childCount = 17;
            potList = new List<Transform>(childCount);

            for (var i = 1; i < childCount; i++)
            {
                var pot = bg.GetChild(i);
                potList.Add(pot);
            }

            _extraState11026 = context.state.Get<ExtraState11026>();

            var pointerEventCustomHandler = transform.Find("LockState/BtnTipsBg").gameObject.AddComponent<PointerEventCustomHandler>();
            
            pointerEventCustomHandler.BindingPointerClick(OnUnlockFeatureClicked);
        }

        public void OnUnlockFeatureClicked(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled) 
                return;
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, 0);
        }
        
        public void UpdateIndicatorSortOrder(bool highOrder)
        {
            if(_lockState)
                _lockState.GetComponent<SortingGroup>().sortingOrder = highOrder ? 314 : 6;
        }

        public void LockSuperFree(bool isLock, bool playAnimation = true)
        {
            if (_animator == null)
                return;
            if (isLock && !_isLockState)
            {
                if (playAnimation)
                {
                    _animator.Play("Locking");
                    AudioUtil.Instance.PlayAudioFxOneShot("Tips_Lock");
                }
                else
                {
                    _animator.Play("Idle");
                }

                _isLockState = true;
            }
            else if (!isLock && _isLockState)
            {
                if (playAnimation)
                {
                    AudioUtil.Instance.PlayAudioFxOneShot("Tips_Unlock");
                    _animator.Play("Unlocking");
                }
                else
                {
                    _animator.Play("UnlockState");
                }

                _isLockState = false;
            }
        }
        
        //从free/link转场的时候调用此方法
        public void LockSuperFreeIdle(bool isLock)
        {
            if (isLock)
            {
                _animator.Play("Idle");
                _isLockState = true;
            }
            else
            {
               _animator.Play("UnlockState");
                _isLockState = false;
            }
        }
        
        public void EnableButtonResponse(bool enable)
        {
            _buttonResponseEnabled = enable;
        }
        
        public void UpgradePotLevel()
        {
            int potLevel = (int)_extraState11026.GetLevel();

            for (var i = 0; i < 16; i++)
            {
                bool unlocked = i < potLevel;
                
                potList[i].Find("Currency").gameObject.SetActive(unlocked);
                potList[i].Find("Icon").gameObject.SetActive(!unlocked);
            }
        }
        
        public void UpdateProgress(bool animation = false)
        {
            int potLevel = (int) _extraState11026.GetLevel();
            for (var i = 0; i < potList.Count; i++)
            {
                if (potLevel == 0)
                {
                     potList[i].Find("Icon").gameObject.SetActive(false); 
                     potList[i].Find("Currency").gameObject.SetActive(true);
                }
                else
                { 
                     bool unlocked = i <= potLevel-1; 
                     potList[i].Find("Icon").gameObject.SetActive(unlocked); 
                     potList[i].Find("Currency").gameObject.SetActive(!unlocked);
                }
            }

            if (potLevel > 0)
            { 
                if (animation) 
                { 
                    var animator = potList[potLevel-1].GetComponent<Animator>(); 
                    if (animator) 
                        animator.Play("Open"); }
            }
        }
    }
}