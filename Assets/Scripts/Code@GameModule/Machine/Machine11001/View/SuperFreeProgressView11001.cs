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
    public class SuperFreeProgressView11001: TransformHolder
    {
        [ComponentBinder("BG/Indicator")] 
        private Transform _indicator;
        
        [ComponentBinder("LockState")] 
        private Transform _lockState;
         
        private List<Transform> potList;

        private ExtraState11001 _extraState11001;

        private bool _isLockState = true;

        private Animator _animator;
        public SuperFreeProgressView11001(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);

            _animator = transform.GetComponent<Animator>();
            if(_animator)
                _animator.keepAnimatorControllerStateOnDisable = true;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            var bg = transform.Find("BG");
            var childCount = bg.childCount;
            potList = new List<Transform>(childCount - 1);

            for (var i = 1; i < childCount; i++)
            {
                var pot = bg.GetChild(i);
                potList.Add(pot);
            }

            _extraState11001 = context.state.Get<ExtraState11001>();

            var pointerEventCustomHandler = transform.Find("LockState/BG").gameObject.AddComponent<PointerEventCustomHandler>();
            
            pointerEventCustomHandler.BindingPointerClick(OnUnlockFeatureClicked);
        }

        public void OnUnlockFeatureClicked(PointerEventData pointerEventData)
        {
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, 0);
        }

        public void UpdateIndicatorSortOrder(bool highOrder)
        {
            _indicator.Find("Flag").GetComponent<SpriteRenderer>().sortingOrder = highOrder ? 311 : 5;
            
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
                    AudioUtil.Instance.PlayAudioFxOneShot("Lock");
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
                    AudioUtil.Instance.PlayAudioFxOneShot("UnLock");
                    _animator.Play("Unlocking");
                }
                else
                {
                    _animator.Play("UnlockState");
                }

                _isLockState = false;
            }
        }
        
        public void UpgradePotLevel()
        {
            int potLevel = (int)_extraState11001.GetPotLevel();

            for (var i = 0; i < potList.Count; i++)
            {
                bool unlocked = i < potLevel;
                
                potList[i].Find("Enable").gameObject.SetActive(unlocked);
                potList[i].Find("Normal").gameObject.SetActive(!unlocked);
            }

            //PlayOpenAnimation
            //potList[potLevel].
            //Jump To Current Pot;
            
            // if (potLevel < 0)
            // {
            //     _indicator.gameObject.SetActive(false);
            // }
            // else
            // {
            //     _indicator.localPosition = potList[potLevel].localPosition;
            // }
        }
        
        public void UpdateProgress(bool animation = false)
        {
            int potLevel = (int)_extraState11001.GetPotLevel() - (animation ? 0 : 1);

            for (var i = 0; i < potList.Count; i++)
            {
                bool unlocked = i <= potLevel;
                
                potList[i].Find("Enable").gameObject.SetActive(unlocked);
                potList[i].Find("Normal").gameObject.SetActive(!unlocked);
            }

            if (potLevel < 0)
            {
                _indicator.gameObject.SetActive(false);
            }
            else
            {
                _indicator.gameObject.SetActive(true);
                _indicator.localPosition = potList[potLevel].localPosition;

                var animator = potList[potLevel].GetComponent<Animator>();
                if (animator)
                    animator.Play("Open");
            }
        }
        
    }
}