// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/09/20:49
// Ver : 1.0.0
// Description : StoreRewardCollectPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class EmeraldCommodityClaimView:View
    {
        [ComponentBinder("IconGroup")]
        public Transform iconGroup;

        [ComponentBinder("CountText")]
        public TextMeshProUGUI countText;
        public void SetUpView(ShopItemConfig itemConfig)
        {
            var index = itemConfig.Image;
            var childCount = iconGroup.childCount;

            for (var i = 0; i < childCount; i++)
            {
                iconGroup.GetChild(i).gameObject.SetActive(index == (i+1).ToString());
            }

            countText.text = itemConfig.SubItemList[0].ShopEmerald.AdditionAmount.GetCommaFormat();
        }
    }
    
    public class CoinCommodityClaimView:View
    {
        [ComponentBinder("IconGroup")]
        public Transform iconGroup;

        [ComponentBinder("CountText")]
        public TextMeshProUGUI countText;
        
        public void SetUpView(ShopItemConfig itemConfig)
        {
            var index = itemConfig.Image;
            var childCount = iconGroup.childCount;

            for (var i = 0; i < childCount; i++)
            {
                iconGroup.GetChild(i).gameObject.SetActive(index == (i+1).ToString());
            }

            countText.text = itemConfig.SubItemList[0].ShopCoin.AdditionAmount.GetCommaFormat();
        }
    }
    
    public class BoostLevelUpCommodityClaimView:View
    {
        [ComponentBinder("DayText")]
        public TextMeshProUGUI dayText;

        public void SetUpView(ShopItemConfig itemConfig)
        {
            var leveUpBurst = itemConfig.SubItemList[0].LevelUpBurst;
          
            if (leveUpBurst != null)
            {
                var day = TimeSpan.FromMinutes(leveUpBurst.Amount).TotalDays;
             
                if (day > 1)
                    dayText.text = $"{day} DAYS";
                else
                {
                    dayText.text = $"{day} DAY";
                }
            }
        }
    }
    
    public class BoostWheelBonusCommodityClaimView:View
    {
        [ComponentBinder("DayText")]
        public TextMeshProUGUI dayText;
       
        public void SetUpView(ShopItemConfig itemConfig)
        {
            var superWheel = itemConfig.SubItemList[0].SuperWheel;
            if (superWheel != null)
            {
                var day = TimeSpan.FromMinutes(superWheel.Amount).TotalDays;
             
                if (day > 1)
                    dayText.text = $"{day} DAYS";
                else
                {
                    dayText.text = $"{day} DAY";
                }
            }
        }
    }

    public class ExtraItemClaimView : View
    {
        [ComponentBinder("StoreCell")]
        private Transform _extraItemTemplate;
       
        public void SetUpView(ShopItemConfig itemConfig)
        {
            var subItem = itemConfig.SubItemList;

            for (var i = 1; i < subItem.Count; i++)
            {
                var subItemUi = _extraItemTemplate;
               
                if(i != subItem.Count - 1)
                    subItemUi = GameObject.Instantiate(_extraItemTemplate.gameObject, transform).transform;
                
                subItemUi.SetAsLastSibling();
                subItemUi.gameObject.SetActive(true);
                
                XItemUtility.InitItemUI(subItemUi, subItem[i],null, "ExtraRewardType2");
            }
        }
    }
    
    [AssetAddress("UIStoreRewardH","UIStoreRewardV")]
    public class StoreRewardCollectPopup : Popup<StoreRewardCollectPopupController>
    {
        [ComponentBinder("Coin")] public Transform coin;

        [ComponentBinder("Diamond")] public Transform diamond;

        [ComponentBinder("BoostLevelUp")] public Transform boostLevelUp;

        [ComponentBinder("BoostWheelBonus")] public Transform boostWheelBonus;

        [ComponentBinder("ConfirmButton")] public Button confirmButton;
        
        [ComponentBinder("ExtraItem")] public Transform extraItemTransform;

        public CoinCommodityClaimView coinCommodityClaimView;
        
        public EmeraldCommodityClaimView emeraldCommodityClaimView;
        
        public BoostWheelBonusCommodityClaimView boostWheelBonusCommodity;
       
        public BoostLevelUpCommodityClaimView boostLevelUpCommodityClaimView;
        public ExtraItemClaimView extraItemClaimView;

        public StoreRewardCollectPopup(string address)
            : base(address)
        {
            
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
         
            coinCommodityClaimView = AddChild<CoinCommodityClaimView>(coin);
            emeraldCommodityClaimView = AddChild<EmeraldCommodityClaimView>(diamond);
            boostWheelBonusCommodity = AddChild<BoostWheelBonusCommodityClaimView>(boostWheelBonus);
            boostLevelUpCommodityClaimView = AddChild<BoostLevelUpCommodityClaimView>(boostLevelUp);
            extraItemClaimView = AddChild<ExtraItemClaimView>(extraItemTransform);
        }
    }
    
    public class StoreRewardCollectPopupController:ViewController<StoreRewardCollectPopup>
    {
        
        private Action<Action<bool, FulfillExtraInfo>> _collectActionHandler;
        
        
        private string _productType;
        private VerifyExtraInfo _verifyExtraInfo;
        // public override void OnViewDidLoad()
        // {
        //     base.OnViewDidLoad();
        // }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            view.confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        public void SetUpViewContent(VerifyExtraInfo verifyExtraInfo, Action<Action<bool, FulfillExtraInfo>> collectActionHandler)
        {
            if (verifyExtraInfo == null || verifyExtraInfo.Item.SubItemList.Count < 0)
                return;

            _verifyExtraInfo = verifyExtraInfo;
            
            _collectActionHandler = collectActionHandler;
            
            string productType = verifyExtraInfo.Item.ProductType;

            _productType = productType;
 
            view.coinCommodityClaimView.Hide();
            view.emeraldCommodityClaimView.Hide();
            view.boostLevelUpCommodityClaimView.Hide();
            view.boostWheelBonusCommodity.Hide();
 
            view.extraItemClaimView.SetUpView(verifyExtraInfo.Item);
            
            switch (productType)
            {
                case "shopcoin":
                    view.coinCommodityClaimView.SetUpView(verifyExtraInfo.Item);
                    view.coinCommodityClaimView.Show();
                    break;
                case "shopemerald":
                    view.emeraldCommodityClaimView.SetUpView(verifyExtraInfo.Item);
                    view.emeraldCommodityClaimView.Show();
                    break;
                case "levelupburst":
                    view.boostLevelUpCommodityClaimView.SetUpView(verifyExtraInfo.Item);
                    view.boostLevelUpCommodityClaimView.Show();
                    break;
                case "superwheel":
                    view.boostWheelBonusCommodity.SetUpView(verifyExtraInfo.Item);
                    view.boostWheelBonusCommodity.Show();
                    break;
            }
        }

        public void OnConfirmButtonClicked()
        {
            view.confirmButton.interactable = false;
            
            _collectActionHandler.Invoke((succeeded,fulfillExtraInfo) =>
            {
                if (succeeded)
                {
                    OnFulfilledSucceeded(fulfillExtraInfo);
                }
                else
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError("FulfillPaymentFailed"));
                    view.Close();
                }
            });
        }

        public async void OnFulfilledSucceeded(FulfillExtraInfo fulfillExtraInfo)
        {
            EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
            EventBus.Dispatch(new EventPurchasedInStore());
            
            if (_productType == "shopcoin")
            {
                var shopCoinItem = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.ShopCoin);
                await XUtility.FlyCoins(view.confirmButton.transform,
                    new EventBalanceUpdate((long) shopCoinItem.ShopCoin.AdditionAmount, "IapStore"));
                view.Close();
            } else if (_productType == "shopemerald")
            {
                var shopEmeraldItem = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.ShopEmerald);
                EventBus.Dispatch(new EventEmeraldBalanceUpdate((long) shopEmeraldItem.ShopEmerald.AdditionAmount, "IapStore"));
                view.Close();
            }
            else
            {
                //Buff之类的数据更新，需要重新从服务器拉去状态
                ItemSettleHelper.SettleItem(fulfillExtraInfo.RewardItems[0], null);
                view.Close();
            }
        }
    }
}