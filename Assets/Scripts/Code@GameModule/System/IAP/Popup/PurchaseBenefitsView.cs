// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/02/10:23
// Ver : 1.0.0
// Description : PurchaseBenefitsView.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BiEventFortuneX = DragonU3DSDK.Network.API.Protocol.BiEventFortuneX;

namespace GameModule
{
    [AssetAddress("UICommonRewardPreview","UICommonRewardPreviewV")]
    public class PurchaseBenefitsView:Popup
    {
        [ComponentBinder("RewardCell")] private Transform rewardCell;
        [ComponentBinder("ContentGroup")] private Transform contentGroup;

        public PurchaseBenefitsView()
        {
            
        }
        
        public override void OnAlphaMaskClicked()
        {
            Close();
        }

        public PurchaseBenefitsView(string address)
        :base(address)
        {
            
        }
        
        public void SetUpBenefitsView(RepeatedField<Item> items)
        {
            var childCount = contentGroup.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var child = contentGroup.GetChild(i);
                if (child != rewardCell && child.gameObject.name != "BG")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            var count = 0;
            for (var i = 0; i < items.Count; i++)
            {
                if (IsItemTypeSupport(items[i].Type))
                {
                    Transform cell = rewardCell;
                
                    if (i != items.Count - 1)
                    {
                        cell = GameObject.Instantiate(rewardCell, contentGroup);
                        cell.gameObject.SetActive(true);
                        count += 1;
                    }
                    
                    cell.SetAsLastSibling();
                    
                    UpdatePurchaseBenefitsInfo(cell, items[i]);
                }
                else if(i == items.Count - 1)
                {
                    rewardCell.gameObject.SetActive(false);
                }
            }

            SetGridLayoutGroup(count);
        }

        private void SetGridLayoutGroup(int count)
        {
            var layoutGroup = contentGroup.GetComponent<GridLayoutGroup>();
            if (ViewManager.Instance.IsPortrait && count >= 2)
            {
                layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layoutGroup.constraintCount = 2;
            }
            else
            {
                layoutGroup.constraint = GridLayoutGroup.Constraint.Flexible;
            }
        }
        
        /// <summary>
        /// 这个是可以忽略一些特殊Item
        /// </summary>
        /// <param name="items"></param>
        /// <param name="skipTypes"></param>
        public void SetUpBenefitsView(RepeatedField<Item> items, List<Item.Types.Type> skipTypes)
        {
            var childCount = contentGroup.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var child = contentGroup.GetChild(i);
                if (child != rewardCell && child.gameObject.name != "BG")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            var count = 0;
            for (var i = 0; i < items.Count; i++)
            {
                if (IsSpecialItemTypeSupport(items[i].Type) && !skipTypes.Contains(items[i].Type))
                {
                    Transform cell = rewardCell;
                
                    if (i != items.Count - 1)
                    {
                        cell = GameObject.Instantiate(rewardCell, contentGroup);
                        cell.gameObject.SetActive(true);
                        count += 1;
                    }
                    
                    cell.SetAsLastSibling();
                    
                    UpdatePurchaseBenefitsInfo(cell, items[i]);
                }
                else if(i == items.Count - 1)
                {
                    rewardCell.gameObject.SetActive(false);
                }
            }
            SetGridLayoutGroup(count);
        }
        
        /// <summary>
        /// 这个是可以展示金币的
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public bool IsSpecialItemTypeSupport(Item.Types.Type itemType)
        {
            return itemType == Item.Types.Type.VipPoints
                   || itemType == Item.Types.Type.ShopGiftBox
                   || itemType == Item.Types.Type.CardPackage
                   || itemType == Item.Types.Type.Avatar
                   || itemType == Item.Types.Type.MonopolyActivityTicket
                   || itemType == Item.Types.Type.Coin;
        }

        
        public bool IsItemTypeSupport(Item.Types.Type itemType)
        {
            return itemType == Item.Types.Type.VipPoints
                   || itemType == Item.Types.Type.ShopGiftBox
                   || itemType == Item.Types.Type.CardPackage
                   || itemType == Item.Types.Type.Avatar
                   || itemType == Item.Types.Type.MonopolyActivityTicket;
        }

        protected void UpdatePurchaseBenefitsInfo(Transform cell, Item item)
        {
            var nameTextUI = cell.Find("RewardNameText").GetComponent<TextMeshProUGUI>();
            var descTextUI = cell.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            var iconImage = cell.Find("RewardIcon").GetComponent<Image>();
            var rawImage = cell.Find("RawIcon").GetComponent<RawImage>();
            var cardStar = cell.Find("CardStar");
            
            if(rawImage != null)
                rawImage.gameObject.SetActive(false);

            string name = "";
            string descText = "";
            Sprite iconSprite = null;
          
            switch (item.Type)
            {
                case Item.Types.Type.VipPoints:
                    name = "VIP POINTS";
                    descText = "+" + item.VipPoints.Amount.GetCommaFormat();
                    iconSprite = XItemUtility.GetItemSprite(Item.Types.Type.VipPoints, item);
                    break;
                case Item.Types.Type.ShopGiftBox:
                    name = "GIFT BOX";
                    descText = "+" + item.ShopGiftBox.Amount.GetCommaFormat();
                    iconSprite = XItemUtility.GetItemSprite(Item.Types.Type.ShopGiftBox, item);
                    break;
                
                case Item.Types.Type.CardPackage:
                    if (XItemUtility.IsMultCard(item.CardPackage.PackageConfig.TypeForShow))
                    {
                        if (item.CardPackage.PackageConfig.TypeForShow == 16)
                            name = $"MIN {item.CardPackage.PackageConfig.MiniStarCount} OF";
                        descText = "";
                     
                        var minStar = item.CardPackage.PackageConfig.MiniStar;
                        for (var i = 0; i < minStar; i++)
                        {
                            cardStar.GetChild(i).gameObject.SetActive(true);
                        }
                    } else if (XItemUtility.IsCardPackage(item.CardPackage.PackageConfig.TypeForShow))
                    {
                        name = item.CardPackage.PackageConfig.TypeForShow == 20 ? "LUCKY PACK" : "NORMAL PACK";
                        descText = "+1";
                    }
                    else
                    {
                        if (item.CardPackage.PackageConfig.TypeForShow < 5)
                        {
                            name = "NORMAL CARD";
                        } else if (item.CardPackage.PackageConfig.TypeForShow < 10)
                        {
                            name = "GOLDEN CARD";
                        }  if (item.CardPackage.PackageConfig.TypeForShow < 15)
                        {
                            name = "LUCKY CARD";
                        }

                        var minStar = (item.CardPackage.PackageConfig.TypeForShow-1) % 5 + 1;
                        for (var i = 0; i < minStar; i++)
                        {
                            cardStar.GetChild(i).gameObject.SetActive(true);
                        }
                        descText = "";
                    }
                    
                    iconSprite = XItemUtility.GetItemSprite(Item.Types.Type.CardPackage, item);
                    break;

                case Item.Types.Type.Avatar:
                    var avatarAddress = "PlayerAvatar_" + item.Avatar.Id;
                    var texture = AssetHelper.GetResidentAsset<Texture2D>(avatarAddress);
                    if (texture)
                    {
                        rawImage.texture = texture;
                        rawImage.gameObject.SetActive(true);
                    }

                    name = "HEAD PORTRAIT";
                    descText = "+1";
                    break;
                case Item.Types.Type.MonopolyActivityTicket:
                    name = "TREASURE RAID";
                    descText = $"+{item.MonopolyActivityTicket.Amount} WHEEL SPINS";
                    iconSprite = XItemUtility.GetItemSprite(Item.Types.Type.MonopolyActivityTicket, item);
                    break;
                case Item.Types.Type.Coin:
                    name = "COINS";
                    var coins = (long) item.Coin.Amount;
                    descText = $"+{coins.GetCommaOrSimplify(6)}";
                    iconSprite = XItemUtility.GetItemSprite(Item.Types.Type.Coin, item);
                    break;
            }

            nameTextUI.text = name;
            descTextUI.text = descText;
          
            if (iconSprite == null)
            {
                iconImage.gameObject.SetActive(false);
            }
            else
            {
                iconImage.sprite = iconSprite;
            }
        }
    }
}