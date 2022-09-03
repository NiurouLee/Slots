// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-21 11:51 AM
// Ver : 1.0.0
// Description : FreeSpinStartPopUp.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FreeSpinStartPopUp : MachinePopUp
    {
        [ComponentBinder("CountText")]
        protected Text freeSpinCountText;

        [ComponentBinder("StartButton")]
        protected Button startButton;
        
        [ComponentBinder("CloseButton")]
        protected Button closeButton;
        
        [ComponentBinder("AdsStartButton")]
        protected Button adStartButton;

        protected Action startAction;
 
        public FreeSpinStartPopUp(Transform transform)
            : base(transform)
        {
            if (startButton)
            {
                startButton.onClick.AddListener(OnStartClicked);
            }

            if (closeButton)
            {
                closeButton.onClick.AddListener(OnStartClicked);
            }

            if (adStartButton)
            {
                adStartButton.onClick.AddListener(OnAdStartClicked);
            }
        }

        public void OnAdStartClicked()
        {
            adStartButton.interactable = false;
            
            if (AdController.Instance.ShouldShowRV(eAdReward.ExtraFreeSpin, false))
            {
                if(closeButton != null)
                    closeButton.interactable = false;
                AdController.Instance.TryShowRewardedVideo(eAdReward.ExtraFreeSpin, OnRvWatchFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
               
                adStartButton.interactable = true;
            }
        }

        public async void OnRvWatchFinished(bool isSuccess, string reason)
        {
            XDebug.Log("OnRvWatchFinished:With isSuccess" + isSuccess);
           
            if (isSuccess)
            {
                //TODO Request FreeSpin+1;

                //从服务器领取额外的FreeSpin次数
                await context.state.Get<AdStrategyState>().ClaimMultipleFreeSpin();

                var popup = await PopupStack.ShowPopup<RvAddFreeSpinPopup>();
                
                popup.SubscribeCloseAction(() =>
                {
                    if (freeSpinCountText)
                        freeSpinCountText.text = context.state.Get<FreeSpinState>().LeftCount.ToString();

                    context.WaitSeconds(1, () =>
                    {
                        Close();
                        startAction?.Invoke();
                    });
                });
            }
            else
            {
                XDebug.Log("OnRvWatchFinished:With isSuccess" + isSuccess);
               
                if(adStartButton)
                    adStartButton.interactable = true;
                if(closeButton)
                    closeButton.interactable = true;
            }
        }

        public void OnStartClicked()
        {
            AudioUtil.Instance.PlayAudioFx("Close");
            startAction?.Invoke();
            Close();
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("FreeGameStart_Open");
            base.OnOpen();
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if (freeSpinCountText)
                freeSpinCountText.SetText(context.state.Get<FreeSpinState>().LeftCount.ToString());

            if (adStartButton != null)
            {
                if (AdController.Instance.ShouldShowRV(eAdReward.ExtraFreeSpin, false)
                && context.state.Get<AdStrategyState>().HasWatchAdWinExtraFreeSpin())
                {
                    adStartButton.gameObject.SetActive(true);
                    startButton.gameObject.SetActive(false);
                    if (closeButton)
                    {
                        closeButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    startButton.gameObject.SetActive(true);
                    adStartButton.gameObject.SetActive(false);
                    
                    if (closeButton)
                    {
                        closeButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void BindStartAction(Action inStartAction)
        {
            startAction = inStartAction;
        }
        
        public bool IsAutoClose()
        {
            return startButton == null;
        }
    }
}