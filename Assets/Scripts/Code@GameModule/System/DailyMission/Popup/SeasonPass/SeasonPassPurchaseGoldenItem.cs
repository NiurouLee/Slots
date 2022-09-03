//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-22 15:50
//  Ver : 1.0.0
//  Description : SeasonPassBuyLevelItem.cs
//  ChangeLog :
//  **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassPurchaseGoldenItem:View
    {
        public ShopItemConfig ShopItemConfig;

        [ComponentBinder("TextGroup/IntegralText")]
        private Text txtTotalValue;
        [ComponentBinder("BottomGroup/PriceButton/OriginalPriceText")]
        private Text txtOriginalPrice;
        [ComponentBinder("BottomGroup/PriceButton/CurrentPriceText")]
        private Text txtCurrentPrice;
        
        [ComponentBinder("BottomGroup/ExtraContentsButton")]
        public Button btnExtra;

        public Action<SeasonPassPurchaseGoldenItem> onBtnBuyClick;
        public Action<SeasonPassPurchaseGoldenItem> onBtnExtraClick;

        public SeasonPassPurchaseGoldenItem()
        {
            
        }
        
        public void UpdateContent(ShopItemConfig shopItemConfig)
        {
            ShopItemConfig = shopItemConfig;
            if (txtTotalValue)
            {
                txtTotalValue.text = "$" + shopItemConfig.Extra;
            }
            txtCurrentPrice.text = "$" + shopItemConfig.Price;
            txtOriginalPrice.text = "$" + shopItemConfig.OldPrice;
        }

        [ComponentBinder("PriceButton")]
        private void OnBtnBuyClicked()
        {
            onBtnBuyClick.Invoke(this);
        }
        [ComponentBinder("ExtraContentsButton")]
        private void OnBtnExtraClicked()
        {
            onBtnExtraClick.Invoke(this);
        }
    }
}