// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/23/15:47
// Ver : 1.0.0
// Description : VipLevelUpPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIVIPLeveUpNotice")]
    public class VipLevelUpPopup : Popup<VipLevelUpPopupController>
    {
        [ComponentBinder("IntegralText")] public TextMeshProUGUI integralText;

        [ComponentBinder("VIPText")] public Image vipNameImage;

        [ComponentBinder("CurrentVIPIcon")] public Image vipIconImage;

        [ComponentBinder("ConfirmButton")] public Button confirmButton;
        
        [ComponentBinder("Root/MainGroup/IntegralText/Icon")] public Transform iconImageTransform;
 
        public VipLevelUpPopup(string address)
        :base(address)
        {
            
        }
        public override Vector3 CalculateScaleInfo()
        {
            var width = ViewResolution.referenceResolutionLandscape.x;

            if (ViewManager.Instance.IsPortrait)
            {
                width = ViewResolution.referenceResolutionPortrait.x;
            }

            if (width < 1158)
            {
                var scale = (width - 10) / 1158.0f;
                return new Vector3(scale, scale, scale);
            }

            return Vector3.one;
        }
    }
    public class VipLevelUpPopupController : ViewController<VipLevelUpPopup>
    {
        public ulong rewardCount;
 
        public void SetUpLevelRewardInfo(Item vipItem)
        {
            var coinItem = XItemUtility.GetItem(vipItem.VipPoints.LevelUpRewardItems,  Item.Types.Type.Coin);
            rewardCount = coinItem.Coin.Amount;
            
            view.integralText.text = coinItem.Coin.Amount.GetCommaFormat();
        }
        
       
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            var vipAtlas = AssetHelper.GetResidentAsset<SpriteAtlas>("CommonUIAtlas");
            var vipLevel = Client.Get<VipController>().GetVipLevel();

            view.vipNameImage.sprite = vipAtlas.GetSprite($"UI_VIP_levelText_{vipLevel}");
            view.vipIconImage.sprite = vipAtlas.GetSprite($"UI_VIP_icon_{vipLevel}");
            view.closeButton.onClick.RemoveAllListeners();
            view.closeButton.onClick.AddListener(OnCloseClicked);
            view.confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            
            var popupArgs = extraData as PopupArgs;
            if (popupArgs != null)
            {
                var vipItem = popupArgs.extraArgs as Item;
               
                if(vipItem != null)
                    SetUpLevelRewardInfo(vipItem);
            }
        }
  
        public async void OnConfirmButtonClicked()
        {
            view.confirmButton.interactable = false;
            view.closeButton.interactable = false;
            
            await XUtility.FlyCoins(view.iconImageTransform, new EventBalanceUpdate(rewardCount, "VipLevelUp"));
            
            var closeAction = view.GetCloseAction();
            
            
            
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(UIVIPMainView), closeAction, view.blockLevel)));
            
            EventBus.Dispatch(new EventOnVipLevelUp());
            
            view.ResetCloseAction();
            
            view.Close();
        }
        
        public async void OnCloseClicked()
        {
            view.closeButton.interactable = false;
            view.confirmButton.interactable = false;
            
            await XUtility.FlyCoins(view.iconImageTransform, new EventBalanceUpdate(rewardCount, "VipLevelUp"));
            
            EventBus.Dispatch(new EventOnVipLevelUp());
            view.Close();
        }
    }
}