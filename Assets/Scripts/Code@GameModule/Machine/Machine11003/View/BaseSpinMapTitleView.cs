// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/23/20:40
// Ver : 1.0.0
// Description : BaseSpinMapTitleView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class BaseSpinMapTitleView : TransformHolder
    {
        [ComponentBinder("StateGroup")] 
        private Transform _lockState; 
        
        [ComponentBinder("InformationButton")] 
        private Transform _informationButton;
        
        [ComponentBinder("FillArea")] 
        private SpriteRenderer _progressBar;
        
        [ComponentBinder("BG")] 
        private Transform _bg;
        
        [ComponentBinder("Tips")] 
        private Transform _tips;
        
        [ComponentBinder("RewardIcons")] 
        private Transform _rewardIcons;

        [ComponentBinder("PigIcon")] 
        private Transform _pigIcon;
        
        [ComponentBinder("ep_UI_CommonClick")] 
        private Transform _pigCoinCollectFx;

        private bool _buttonResponseEnabled = true;

        private bool _isFeatureLocked = false;
         
        public BaseSpinMapTitleView(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
            var animator = transform.GetComponent<Animator>();
            
            animator.keepAnimatorControllerStateOnDisable = true;
            
            _tips.transform.SetParent(transform.parent);
            
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            var pointerEventCustomHandler = _bg.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnTitleBarClicked);

            pointerEventCustomHandler = _informationButton.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnInfoClicked);
            
            _lockState.gameObject.SetActive(false);
        }

        public void ShowCollectFx()
        {
            var animator = transform.GetComponent<Animator>();
            animator.Play("CollectGold",-1,0);
        }
        
        public void ShowUpgradeAnimation(Action finishCallback)
        {
            var animator = transform.GetComponent<Animator>();
            AudioUtil.Instance.PauseMusic();
            AudioUtil.Instance.PlayAudioFx("Map_Trigger");
            XUtility.PlayAnimation(animator, "Upgrade", finishCallback);
        }

        public void UpdateCollectionProgress(bool hasAnimation)
        {
            var extraState = context.state.Get<ExtraState11003>();
            var progress = extraState.GetCoinCollectProgress();
            var y = _progressBar.size.y;
            if (hasAnimation)
            {
                float currentValue = _progressBar.size.x;
                DOTween.To(() => currentValue, x => {  _progressBar.size = new Vector2(x, y);}, 5.83f * progress, 0.5f);
            }
            else
            {
                for (var i = 0; i < _rewardIcons.childCount; i++)
                {
                    _rewardIcons.GetChild(i).gameObject.SetActive(false);
                }
                
                var level = extraState.GetPotLevel();

                var bankLevelIndex = new List<uint> {2, 6, 11, 17};
                string iconName = "BoxIcon";

                if (bankLevelIndex.Contains(level))
                {
                    var index = bankLevelIndex.IndexOf(level);
                    iconName = "Bank" + (index + 1);
                }

                _rewardIcons.Find(iconName).gameObject.SetActive(true);
                
                _progressBar.size = new Vector2(5.83f * progress, y);
            }
        }

        public void LockFeatureUI(bool lockFeature, bool hasAnimation = true, bool force = false)
        {
            if (_isFeatureLocked == lockFeature && !force)
                return;
            
            var animator = transform.GetComponent<Animator>();

            if (lockFeature)
            {
                if (hasAnimation)
                {
                    AudioUtil.Instance.PlayAudioFx("Map_Lock");
                }
                animator.Play(hasAnimation ? "Locking" : "Lock");
            }
            else
            {
                if (hasAnimation)
                {
                    AudioUtil.Instance.PlayAudioFx("Map_Unlock");
                }
                animator.Play(hasAnimation ? "Unlock" : "Idle");

                if (hasAnimation)
                {
                    if (!_tips.gameObject.activeSelf)
                    {
                        XUtility.ShowTipAndAutoHide(_tips.transform, 4, 0.2f, true, context);
                    }
                }
            }

            _isFeatureLocked = lockFeature;
        }

        public void ShowCollectTip()
        {
            if (!_tips.gameObject.activeSelf)
            {
                XUtility.ShowTipAndAutoHide(_tips.transform, 4, 0.2f, true, context);
            }
        }
        
        public void OnTitleBarClicked(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled)
                return;

            if (_isFeatureLocked)
            {
                context.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, 2);
                LockFeatureUI(false, true, false);
            }
            else
            {
                if (!_tips.gameObject.activeSelf)
                {  
                    AudioUtil.Instance.PlayAudioFx("MapTips_Open");
                    XUtility.ShowTipAndAutoHide(_tips.transform, 4, 0.2f, true, context);
                }
                else
                {
                    AudioUtil.Instance.PlayAudioFx("MapTips_Close");
                }
            }
        }

        private float lastResponseTime = 0;
       
        public void OnInfoClicked(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled)
                return;
            
            if (Time.realtimeSinceStartup - lastResponseTime > 0.2f)
            {
                lastResponseTime = Time.realtimeSinceStartup;
               
                var mapView11003 = context.view.Get<MapView11003>();
                if (mapView11003.isVisible)
                {
                  
                    mapView11003.HideMap();
                }
                else
                {
                  
                    context.view.Get<MapView11003>().ShowMap();
                }
            }
        }
        
        public Transform GetPigIcon()
        {
            return _pigIcon;
        }

        public void EnableButtonResponse(bool enable)
        {
            _buttonResponseEnabled = enable;
            
            _informationButton.GetComponent<SpriteRenderer>().color = enable ? Color.white : Color.white * 0.8f;
        }
    }
}