// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/18/20:53
// Ver : 1.0.0
// Description : CommodityShopItemView.cs
// ChangeLog :
// **********************************************
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
  public class CommodityShopItemView : View
  {
        [ComponentBinder("PriceButton")] public Button priceButton;

        [ComponentBinder("PriceText")] public Text priceText;

        [ComponentBinder("AttachContentButton")]
        public Button attachContentButton;    
        
        [ComponentBinder("SeeMoreGroup")]
        public Transform seeMoreGroup;   
  
        protected bool isVertical;
        
        public ShopItemConfig shopItemConfig;
 
        public Action<CommodityShopItemView> onPrizeButtonClick;
        public Action<CommodityShopItemView> onContentButtonClick;

        protected override void SetUpExtraView()
        {
            priceButton.onClick.AddListener(OnPrizeButtonClicked);
            if (attachContentButton)
                attachContentButton.onClick.AddListener(OnContentButtonClicked);

            if (seeMoreGroup)
            {
                var seeMoreButton = seeMoreGroup.GetComponent<Button>();
                if (seeMoreButton)
                {
                    seeMoreButton.onClick.AddListener(OnContentButtonClicked);
                }
            }
        }

        protected void UpdateView(bool inIsVertical)
        {
            isVertical = inIsVertical;
        }
        
        protected void SetUpPageContent(Transform pageContentTransform, ShopItemConfig inShopItemConfig)
        {
            var contentCell = pageContentTransform.Find("StoreCell");
            
            var itemCount = inShopItemConfig.SubItemList.count - 1;
            var limitCount = isVertical ? 3 : 2;

            var attachPoint1 = pageContentTransform.Find("AttachPoint1");
            var attachPoint2 = pageContentTransform.Find("AttachPoint2");

            //UI刷新的时候，需要先将Item还回去，
            if (attachPoint1.childCount > 0)
            {
                for (var i = attachPoint1.childCount -1 ; i >= 0; i--)
                {
                    var child = attachPoint1.GetChild(i);
                    GameObject.Destroy(child.gameObject);
                }
            }
            
            if (attachPoint2.childCount > 0)
            {
                for (var i = attachPoint2.childCount -1 ; i >= 0; i--)
                {
                    var child = attachPoint2.GetChild(i);
                    GameObject.Destroy(child.gameObject);
                }
            }
            
            if (itemCount > 0)
            {
                var subItemList = inShopItemConfig.SubItemList;
                var attachCount = 0;

                //横版的时候，如果只有一个额外的Item，就要居中
                if (itemCount == 1)
                {
                    if (isVertical)
                    {
                        pageContentTransform.Find("AttachPoint2").gameObject.SetActive(false);
                    }
                    else
                    {
                        var attachPoint1RectTransform = (RectTransform) attachPoint1;
                        var anchoredPosition = attachPoint1RectTransform.anchoredPosition;
                        attachPoint1RectTransform.anchoredPosition = new Vector2(0, anchoredPosition.y);
                        pageContentTransform.Find("BlockLine").gameObject.SetActive(false);
                    }
                }

                for (var i = 1; i <= itemCount; i++)
                {
                    string rootTransformName = "AttachPoint" + (attachCount == 2 ? "2": (attachCount % 2 + 1).ToString());
                  
                    var cell = GameObject.Instantiate(contentCell.gameObject,
                        pageContentTransform.Find(rootTransformName));
                    cell.gameObject.SetActive(true);

                    XItemUtility.InitItemUI(cell.transform, subItemList[i], XItemUtility.GetItemRewardSimplyDescText, isVertical ? "StandardWithBGTypeV": "StandardWithBGType");
 
                    attachCount++;
 
                    if (attachCount >= limitCount)
                    {
                        break;
                    }
                }
            }

            if (itemCount <= limitCount)
            {
              //  seeMoreGroup.gameObject.SetActive(false);
                //
                // if(attachContentButton != null)
                //     attachContentButton.interactable = false;

                // if (!isVertical)
                // {
                //     var attachPointRectTransform = (RectTransform) pageContentTransform.Find("AttachPoint1");
                //     var anchoredPosition = attachPointRectTransform.anchoredPosition;
                //     attachPointRectTransform.anchoredPosition = new Vector2(anchoredPosition.x, -5);
                //     
                //     attachPointRectTransform = (RectTransform) pageContentTransform.Find("AttachPoint2");
                //     anchoredPosition = attachPointRectTransform.anchoredPosition;
                //     attachPointRectTransform.anchoredPosition = new Vector2(anchoredPosition.x, -5);
                // }
            }
        }
        
        // protected void InitSubItemUI(Transform itemTransform, Item itemInfo)
        // {
        //     switch (itemInfo.Type)
        //     {
        //         case Item.Types.Type.ShopGiftBox:
        //             var label = itemTransform.Find("QuantityGroup/AttachQuantityText").GetComponent<TextMeshProUGUI>();
        //             label.text = "+" + itemInfo.ShopGiftBox.Amount.ToString();
        //             break;
        //         case Item.Types.Type.VipPoints:
        //             var vipAmountLabel = itemTransform.Find("QuantityGroup/AttachQuantityText").GetComponent<TextMeshProUGUI>();
        //             vipAmountLabel.text = "+" + itemInfo.VipPoints.Amount.ToString();
        //             break;     
        //     }
        // }

        protected void OnPrizeButtonClicked()
        {
            SoundController.PlayButtonClick();
            onPrizeButtonClick?.Invoke(this);
        }

        protected void OnContentButtonClicked()
        {
            SoundController.PlayButtonClick();
            onContentButtonClick?.Invoke(this);
        }
    }
}