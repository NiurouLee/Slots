// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/14/20:06
// Ver : 1.0.0
// Description : AdBigWinPopup.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIADWheelBonusMainH","UIADWheelBonusMainV")]
    public class AdBigWinPopup : Popup<AdBigWinPopupViewController>
    {
        [ComponentBinder("FreeButton")] 
        public Button watchButton;
        
        [ComponentBinder("CollectButton")] 
        public Button collectButton;      
          
        [ComponentBinder("MainGroup")] 
        public Transform mainGroup;     
         
        [ComponentBinder("Root/RewardGroup/WinGroup/LastWinGroup/IntegralText")] 
        public TMP_Text lastWinText;
        
        [ComponentBinder("Root/RewardGroup/WinGroup/MultipleGroup/IntegralText")] 
        public TMP_Text multipleText;   
        
        [ComponentBinder("Root/RewardGroup/IntegralGroup/BGGroup/IntegralText")] 
        public TMP_Text totalWinText;
        
        public AdBigWinPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1365, 1000);
        }

        public override Vector3 CalculateScaleInfo()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                return Vector3.one;
            }
            
            return base.CalculateScaleInfo();
        }
        
        public override string GetCloseAnimationName()
        {
            return viewController.GetCloseAnimationName();
        }
    }

    public class AdBigWinPopupViewController : ViewController<AdBigWinPopup>
    {
        protected AdStrategy _adStrategy;

        protected ulong totalWin;

        protected bool rvWatched = false;
        
        public override void OnViewDidLoad()
        {
            view.watchButton.onClick.AddListener(OnWatchRvClicked);
            view.collectButton.onClick.AddListener(OnCollectRewardCollect);
            
            base.OnViewDidLoad();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            _adStrategy = inExtraData as AdStrategy;
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            totalWin = (_adStrategy.BigWinBase * _adStrategy.Multiples[(int) _adStrategy.MultipleIndex]);

            view.lastWinText.text = _adStrategy.BigWinBase.GetCommaFormat();
            view.multipleText.text = "X" + _adStrategy.Multiples[(int) _adStrategy.MultipleIndex];
            view.totalWinText.text = totalWin.GetCommaFormat();
        }

        protected void OnWatchRvClicked()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.MultipleCoin,false))
            {
                view.watchButton.interactable = false;
                view.closeButton.interactable = false;
                AdController.Instance.TryShowRewardedVideo(eAdReward.MultipleCoin, OnWatchRvFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
        }
        
        protected async void OnWatchRvFinished(bool success, string failed)
        {
            if (success)
            {
                await ClaimWheelBonusRv();
                
                rvWatched = true;
                ShowWheelBonusSpinningAnimation();
            }
            else
            {
                view.watchButton.interactable = true;
                view.closeButton.interactable = true;
            }
        }

        protected async Task ClaimWheelBonusRv()
        {
            var send = new CFulfillBigWinMultiple();
            
            var receive = await APIManagerGameModule.Instance.SendAsync<CFulfillBigWinMultiple, SFulfillBigWinMultiple>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(receive.Response.UserProfile));
                XDebug.Log("ClaimSuccess");
            } 
            
            XDebug.Log("ClaimFailed");
        }

        protected async void ShowWheelBonusSpinningAnimation()
        {
            var wheelAnimator = view.transform.GetComponent<Animator>();
           
            await XUtility.PlayAnimationAsync(wheelAnimator,"Collect",this);
            SoundController.PlaySfx("RVBigWinWheel");
            SetSpinResultIndex();
        }
        protected void SetSpinResultIndex()
        {
            var multipleIndex = _adStrategy.MultipleIndex;
           
            float targetAngle = 360.0f /_adStrategy.Multiples.Count * multipleIndex;
            
            view.mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
        }

        protected async void OnCollectRewardCollect()
        {
           view.collectButton.interactable = false;
           
           await XUtility.FlyCoins(view.collectButton.transform, new EventBalanceUpdate(totalWin, "BigWinWheelBonusRV"));
           
           view.Close();
        }

        public string GetCloseAnimationName()
        {
            return rvWatched ? "Collect_Close" : "Close";
        }
    }
}