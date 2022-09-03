// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/16/16:33
// Ver : 1.0.0
// Description : WinEffectPopUp.cs
// ChangeLog :
// **********************************************

using System;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    public class WinEffectPopUp : MachinePopUp
    {
        [ComponentBinder("WinChipText")]
        private TextMeshProUGUI _winChipText;   
         
        [ComponentBinder("Label")]
        private TextMeshProUGUI _getMoreText;
         
        private Action _winEffectShowEndAction;
      //  private WinInfo winInfo;
        private AudioSource fxSource;
        private float timeStart;

        [ComponentBinder("AdsButton")] 
        private Button _adsButton;
        
        [ComponentBinder("CloseButton")] 
        private Button _closeButton;

        private uint _winLevel;
        private ulong _winChips;

        private bool needShowRvButton = false;
        
        public WinEffectPopUp(Transform transform)
            : base(transform)
        {
            _winChipText.text = "0";
         
            var proxy = transform.gameObject.AddComponent<MonoAnimationEventProxy>();
            proxy.AddEndEventToAllAnimation();
            proxy.SetEndEventCallback(OnAnimationEnd);

            var pointerEventCustomHandler = transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnUserInterrupt);

            if (ViewManager.Instance.IsPortrait)
            {
                transform.localScale = Vector3.one * 0.9f;
            }
            
            _adsButton.onClick.AddListener(OnAdsButtonClick);
            _closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        static readonly string[] EffectAddress =
        {
            "WinEffectBigWin",
            "WinEffectHugeWin",
            "WinEffectMassiveWin",
            "WinEffectColossalWin",
            "WinEffectApocalypticWin"
        };

        public void OnUserInterrupt(PointerEventData eventData)
        {
            var duration = Time.realtimeSinceStartup - timeStart;
            if (duration < 1.5)
                return;
 
            if (!needShowRvButton)
            {
                if (animator && animator.GetCurrentAnimatorStateInfo(0).IsName("Loop"))
                {
                    DOTween.Kill(_winChipText);
                    _winChipText.text = _winChips.GetCommaFormat();
                    animator.Play("Disappear");

                    if (fxSource != null && fxSource.time < 6)
                    {
                        fxSource.time = 6;
                    }
                }
            }
            else
            {
                if (animator && animator.GetCurrentAnimatorStateInfo(0).IsName("Loop"))
                {
                    if (fxSource != null && fxSource.time < 6)
                    {
                        fxSource.time = 6;
                    }

                    DOTween.Kill(_winChipText);
                    _winChipText.text = _winChips.GetCommaFormat();
                    animator.Play("Waiting_ad");

                    ShowAdButton();
                }
            }
        }

        public void OnAnimationEnd(string animationName)
        {
            if (animationName.Contains("Disappear"))
            {
                Close();
                _winEffectShowEndAction?.Invoke();
            }
        }

        public void ShowAdButton()
        {
            _adsButton.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);
                    
            _adsButton.transform.parent.gameObject.SetActive(true);
            _closeButton.transform.parent.gameObject.SetActive(true);
        }

        public void SetUpRv()
        {
            var adStrategy = context.state.Get<AdStrategyState>().GetAdStrategy();

            // adStrategy = new AdStrategy();
            // adStrategy.Type = AdStrategy.Types.Type.ExtraWinBonus;
            // adStrategy.ExtraWinBonusPercent = 50;
            // adStrategy.ExtraWinBonusBase = 100;
            // adStrategy.ExtraWinBonusWin = 200;
            
            needShowRvButton = false;
            
            if (adStrategy != null && AdController.Instance.ShouldShowRV(eAdReward.ExtraWinBonus,false))
            {
                if (adStrategy.Type == AdStrategy.Types.Type.ExtraWinBonus)
                {
                    var shortText = adStrategy.ExtraWinBonusWin.GetAbbreviationFormat(1);
                    
                    if (shortText.Length > 4)
                    {
                        shortText = adStrategy.ExtraWinBonusWin.GetAbbreviationFormat(0);
                    }

                    _getMoreText.text = shortText + " MORE!";
                    needShowRvButton = true;
                    return;
                }
            }
            _adsButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
        }
        
        public void InitWith(uint winLevel, ulong winChips, string address, Action inWinEffectShowEndAction)
        {
            _winLevel = winLevel;
            _winChips = winChips;
          
            _winEffectShowEndAction = inWinEffectShowEndAction;
           
            _winChipText.DOCounter(0, (long)_winChips, 6f).OnComplete(() =>
            {
                _winChipText.text = _winChips.GetCommaFormat();

                if (!needShowRvButton && animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear"))
                {
                    animator.Play("Disappear");
                }
                else
                {
                    animator.Play("Waiting_ad");
                    ShowAdButton();
                }
            });

            SetUpRv();
            
            animator.Play("Appear");

            AudioUtil.Instance.PlayAudioFx(address + "Voice");
            
            fxSource = AudioUtil.Instance.PlayAudioFx("WinEffectSound");
            
            timeStart = Time.realtimeSinceStartup;
          //play autio
        }

        protected void OnCloseButtonClick()
        {
            if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear"))
            {
                animator.Play("Disappear");
            }
        }
        
        protected void OnAdsButtonClick()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.ExtraWinBonus,false))
            {
                _closeButton.interactable = false;
                _adsButton.interactable = false;
                
                AdController.Instance.TryShowRewardedVideo(eAdReward.ExtraWinBonus, OnWatchRvFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
        }

        private async void OnWatchRvFinished(bool success, string reason)
        {
            if (success)
            {
                XDebug.Log("WatchRV return Success");
                var adStrategy = context.state.Get<AdStrategyState>().GetAdStrategy();
 
                // adStrategy = new AdStrategy();
                // adStrategy.Type = AdStrategy.Types.Type.ExtraWinBonus;
                // adStrategy.ExtraWinBonusPercent = 50;
                // adStrategy.ExtraWinBonusBase = 100;
                // adStrategy.ExtraWinBonusWin = 200;
                
                var totalWin = adStrategy.ExtraWinBonusWin;

                var c = new CFulfillExtraWinBonus();
                var s  = await  APIManagerGameModule.Instance.SendAsync<CFulfillExtraWinBonus, SFulfillExtraWinBonus>(c);
                
                if (s.ErrorCode == ErrorCode.Success)
                {
                    XDebug.Log("WatchRV server Return Success");
                    EventBus.Dispatch(new EventUserProfileUpdate(s.Response.UserProfile));
                    var rewardPopup = await PopupStack.ShowPopup<ExtraBonusWinRewardClaimPopup>();
                    rewardPopup.viewController.SetUpClaimUI(totalWin, eAdReward.MultipleCoin, OnRvClaimFinished);
                }
                else
                {
                     XDebug.Log("WatchRV server Return Failed");
                    _closeButton.interactable = true;
                    _adsButton.interactable = true;
                }
            }
            else
            {
                XDebug.Log("WatchRV Return Failed");
                _closeButton.interactable = true;
                _adsButton.interactable = true;
            }
        }

        private  void OnRvClaimFinished()
        {
            if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear"))
            {
                animator.Play("Disappear");
            }
        }
        
        public static string GetEffectAddress(uint level)
        {
            var index = (int)level - 4;

            if (index >= 0 && index < EffectAddress.Length)
            {
                return EffectAddress[index];
            }

            return null;
        }
    }
}