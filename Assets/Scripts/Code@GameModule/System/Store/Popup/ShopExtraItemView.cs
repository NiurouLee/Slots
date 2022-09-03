// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/18/20:32
// Ver : 1.0.0
// Description : ShopExtraItemView.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using System.Collections.Generic;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class ShopExtraItemView:View
    {
        [ComponentBinder("ContentGroup")]
        public Transform contentGroup; 
        
        [ComponentBinder("SeeMoreContent")]
        public Transform seeMoreContent;

        public void InitializeView(RepeatedField<Item> items)
        {
            //UI会复用，所以先隐藏所有的Item;
            var childCount = contentGroup.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var childI = contentGroup.GetChild(i);
                if (contentGroup.GetChild(i).name != "BG")
                {
                    childI.gameObject.SetActive(false);
                }
            }
            
            for (var i = 0; i < items.Count; i++)
            {
                var itemTransform = contentGroup.Find(items[i].Type.ToString());

                if (itemTransform == null)
                {
                    itemTransform = seeMoreContent.Find(items[i].Type.ToString());
                    if(itemTransform != null)
                        itemTransform.SetParent(contentGroup, false);
                }

                if (itemTransform != null)
                {
                    itemTransform.gameObject.SetActive(true);
                    UpdateItemUI(itemTransform, items[i]);
                }
            }
        }
        public void UpdateItemUI(Transform itemTransform, Item item)
        {
            switch (item.Type)
            {
                case Item.Types.Type.VipPoints:
                    itemTransform.Find("ValueText").GetComponent<TextMeshProUGUI>().text = "+" + item.VipPoints.Amount.GetCommaFormat();
                    break;
                case Item.Types.Type.ShopGiftBox:
                    itemTransform.Find("ValueText").GetComponent<TextMeshProUGUI>().text = "+" + item.ShopGiftBox.Amount;
                    break;
            }
        }
    }
}