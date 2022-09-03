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
    public class SeasonPassBuyLevelItem:View
    {
        public ShopItemConfig ShopItemConfig;
        [ComponentBinder("Root/DescriptionText")]
        private TextMeshProUGUI txtDescribe;

        [ComponentBinder("Root/ExtraContentsButton")]
        public Button btnExtra;
        
        [ComponentBinder("Root/PriceButton/OriginalPriceText")]
        private Text txtOriginalPrice;
        [ComponentBinder("Root/PriceButton/CurrentPriceText")]
        private Text txtCurrentPrice;

        public Action<SeasonPassBuyLevelItem> onBtnBuyClick;
        public Action<SeasonPassBuyLevelItem> onBtnExtraClick;

        public SeasonPassBuyLevelItem()
        {
            
        }
        
        public void UpdateContent(ShopItemConfig shopItemConfig)
        {
            ShopItemConfig = shopItemConfig;
            txtDescribe.text = shopItemConfig.Description;
            txtCurrentPrice.text = "$" + shopItemConfig.Price;
            txtOriginalPrice.text = "$" + shopItemConfig.OldPrice;
        }

        [ComponentBinder("Root/PriceButton")]
        private void OnBtnBuyClicked()
        {
            onBtnBuyClick.Invoke(this);
        }
        
        [ComponentBinder("Root/ExtraContentsButton")]
        private void OnBtnExtraClicked()
        {
            onBtnExtraClick.Invoke(this);
        }
    }
}