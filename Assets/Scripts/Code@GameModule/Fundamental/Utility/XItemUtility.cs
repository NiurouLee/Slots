// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/24/13:35
// Ver : 1.0.0
// Description : XItemUtility.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    public static class XItemUtility
    {
        public static Item GetCoinItem(Reward reward)
        {
            if (reward != null && reward.Items.Count > 0)
            {
                for (var i = 0; i < reward.Items.Count; i++)
                {
                    if (reward.Items[i].Type == Item.Types.Type.Coin)
                    {
                        return reward.Items[i];
                    }
                }
            }

            return null;
        }

        public static Item GetItem(RepeatedField<Item> items, Item.Types.Type itemType)
        {
            if (items != null && items.Count > 0)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    if (items[i].Type == itemType)
                    {
                        return items[i];
                    }
                }
            }

            return null;
        }


        public static Item GetCoinItem(RepeatedField<Item> items)
        {
            if (items != null && items.Count > 0)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    if (items[i].Type == Item.Types.Type.Coin)
                    {
                        return items[i];
                    }
                }
            }

            return null;
        }

        public static RepeatedField<Item> GetItems(RepeatedField<Item> items, List<Item.Types.Type> skipTypes,
            int startIndex = 0)
        {
            RepeatedField<Item> listItems = new RepeatedField<Item>();
            if (items != null && items.Count > 0)
            {
                for (var i = startIndex; i < items.Count; i++)
                {
                    if (!skipTypes.Contains(items[i].Type))
                    {
                        listItems.Add(items[i]);
                    }
                }
            }

            return listItems;
        }

        public static RepeatedField<Reward> GetRewards(RepeatedField<Reward> rewards, List<Item.Types.Type> skipTypes,
            int startIndex = 0)
        {
            RepeatedField<Reward> listRewards = new RepeatedField<Reward>();
            if (rewards != null && rewards.count
                > 0)
            {
                var reward = new Reward();
                for (int i = startIndex; i < rewards.count; i++)
                {
                    var items = GetItems(rewards[i].Items, skipTypes);
                    for (int j = 0; j < items.count; j++)
                    {
                        reward.Items.Add(items[j]);
                    }
                }
                listRewards.Add(reward);
            }
            return listRewards;
        }


        public static RepeatedField<Item> GetItems(RepeatedField<Item> items, Item.Types.Type itemType,
            int startIndex = 0)
        {
            RepeatedField<Item> listItems = new RepeatedField<Item>();
            if (items != null && items.Count > 0)
            {
                for (var i = startIndex; i < items.Count; i++)
                {
                    if (items[i].Type == itemType)
                    {
                        listItems.Add(items[i]);
                    }
                }
            }

            return listItems;
        }

        public static List<Item> GetItems(RepeatedField<Reward> rewards, Item.Types.Type itemType)
        {
            List<Item> listItems = new List<Item>();
            if (rewards != null && rewards.Count > 0)
            {
                for (int i = 0; i < rewards.Count; i++)
                {
                    var reward = rewards[i];
                    if (reward.Items != null && reward.Items.Count > 0)
                    {
                        for (var j = 0; j < reward.Items.Count; j++)
                        {
                            if (reward.Items[j].Type == itemType)
                            {
                                listItems.Add(reward.Items[j]);
                            }
                        }
                    }
                }
            }

            return listItems;
        }

        public static RepeatedField<Item> GetItems(RepeatedField<Reward> rewards)
        {
            RepeatedField<Item> listItems = new RepeatedField<Item>();
            if (rewards != null && rewards.Count > 0)
            {
                for (int i = 0; i < rewards.Count; i++)
                {
                    var reward = rewards[i];
                    if (reward.Items != null && reward.Items.Count > 0)
                    {
                        for (var j = 0; j < reward.Items.Count; j++)
                        {
                            listItems.Add(reward.Items[j]);
                        }
                    }
                }
            }

            return listItems;
        }

        public static RepeatedField<Item> FilterOutItemsByType(RepeatedField<Reward> rewards,
            List<Item.Types.Type> list)
        {
            var result = new RepeatedField<Item>();

            for (var i = 0; i < rewards.Count; i++)
            {
                for (var c = 0; c < rewards[i].Items.Count; c++)
                {
                    if (list != null && list.Contains(rewards[i].Items[c].Type))
                    {
                        continue;
                    }

                    result.Add(rewards[i].Items[c]);
                }
            }

            return result;
        }

        public static void FilterOutItemsByType(RepeatedField<Item> items, List<Item.Types.Type> list)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                if (list.Contains(items[i].Type))
                {
                    items.RemoveAt(i);
                }
            }
        }

        public static void ClearCreatedItemsUI(Transform rewardGroup, string itemTemplateName)
        {
            if (rewardGroup.childCount > 0)
            {
                var childCount = rewardGroup.childCount;
                for (var i = childCount - 1; i >= 0; i--)
                {
                    var child = rewardGroup.GetChild(i);
                    if (child.name.Contains(itemTemplateName) && child.name != itemTemplateName)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }

        public static void RefreshItemsUI(Transform rewardGroup, RepeatedField<Item> items, Transform itemTemplate,
            Func<Item, string> descFunc = null, string uiType = "StandardType",
            List<Item.Types.Type> skipTypeList = null)
        {
            if (rewardGroup.childCount > 0)
            {
                RepeatedField<Item> validList = null;

                if (skipTypeList != null)
                {
                    validList = new RepeatedField<Item>();
                    var count = items.Count;
                    for (var i = 0; i < count; i++)
                    {
                        if (!skipTypeList.Contains(items[i].Type))
                        {
                            validList.Add(items[i]);
                        }
                    }
                }
                else
                {
                    validList = items;
                }

                var itemCount = validList.Count;

                int currentChildIndex = 0;
                var childCount = rewardGroup.childCount;

                for (var i = 0; i < itemCount; i++)
                {
                    Transform rewardUI = null;

                    while (currentChildIndex < childCount)
                    {
                        var child = rewardGroup.GetChild(currentChildIndex);
                        currentChildIndex++;

                        if (child.name.Contains(itemTemplate.name))
                        {
                            rewardUI = child;
                            break;
                        }
                    }

                    if (rewardUI != null)
                    {
                        rewardUI.gameObject.SetActive(true);
                        InitItemUI(rewardUI, validList[i], descFunc, uiType);
                    }
                }
            }
        }

        public static Transform GetItemUI(Transform rewardGroup, Item.Types.Type itemType)
        {
            var childCount = rewardGroup.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var child = rewardGroup.GetChild(i);

                var monoCustomDataProxy = child.GetComponent<MonoCustomDataProxy>();

                if (monoCustomDataProxy != null)
                {
                    var item = monoCustomDataProxy.GetCustomData<Item>("Item");
                    if (item != null)
                    {
                        if (item.Type == itemType)
                            return child;
                    }
                }
            }

            return null;
        }

        public static void InitItemsUI(Transform rewardGroup, RepeatedField<Item> items, Transform itemTemplate = null,
            Func<Item, string> descFunc = null, string uiType = "StandardType",
            List<Item.Types.Type> skipTypeList = null)
        {
            if (itemTemplate == null)
            {
                itemTemplate = rewardGroup.Find("CommonCell");
            }

            if (itemTemplate == null)
            {
                return;
            }

            RepeatedField<Item> validList = null;

            if (skipTypeList != null)
            {
                validList = new RepeatedField<Item>();
                var count = items.Count;
                for (var i = 0; i < count; i++)
                {
                    if (!skipTypeList.Contains(items[i].Type))
                    {
                        validList.Add(items[i]);
                    }
                }
            }
            else
            {
                validList = items;
            }

            var itemCount = validList.Count;

            for (var i = 0; i < itemCount; i++)
            {
                var rewardUI = itemTemplate;

                if (i != itemCount - 1)
                {
                    rewardUI = GameObject.Instantiate(itemTemplate, rewardGroup);
                }

                rewardUI.SetAsLastSibling();
                rewardUI.gameObject.SetActive(true);

                InitItemUI(rewardUI, validList[i], descFunc, uiType);
            }
        }

        public static string GetItemRewardFullDescText(Item item)
        {
            switch (item.Type)
            {
                case Item.Types.Type.Coin:
                    return item.Coin.Amount.GetCommaFormat();
                case Item.Types.Type.ShopCoin:
                    return item.ShopCoin.AdditionAmount.GetCommaFormat();
                case Item.Types.Type.Emerald:
                    return item.Emerald.Amount.GetCommaFormat();
                case Item.Types.Type.ShopEmerald:
                    return item.ShopEmerald.AdditionAmount.GetCommaFormat();
                case Item.Types.Type.DoubleExp:
                    return GetBuffDescText(item.DoubleExp.Amount);
                case Item.Types.Type.LevelUpBurst:
                    return GetBuffDescText(item.LevelUpBurst.Amount);

                case Item.Types.Type.CashBackBuffsBet:
                case Item.Types.Type.CashBackBuffsNowin:
                case Item.Types.Type.CashBackBuffsWin:

                    return GetBuffDescText(item.CashBackBuff.Amount);
                case Item.Types.Type.VipPoints:
                    return "+" + item.VipPoints.Amount.GetCommaFormat();
                case Item.Types.Type.ShopGiftBox:
                    return "+" + item.ShopGiftBox.Amount.GetCommaFormat();
                case Item.Types.Type.SuperWheel:
                    return GetBuffDescText(item.SuperWheel.Amount);
                case Item.Types.Type.ValentineActivityPoint:
                    return "+" + item.ValentineActivityPoint.Amount;
                case Item.Types.Type.IndependenceDayActivityPoint:
                    return "+" + item.IndependenceActivityPoint.Amount;
                case Item.Types.Type.GoldHammer:
                    return "+" + item.GoldHammer.Amount;
                case Item.Types.Type.NewbieQuestBoost:
                    return item.NewbieQuestBoost.Amount + " MINS";
                case Item.Types.Type.Avatar:
                    return "+" + item.Avatar.Count;
                case Item.Types.Type.CardPackage:
                    if (item.CardPackage.PackageConfig.TypeForShow == 16 ||
                        item.CardPackage.PackageConfig.TypeForShow == 17 ||
                        item.CardPackage.PackageConfig.TypeForShow == 18)
                    {
                        return "+" + item.CardPackage.PackageConfig.CardCount;
                    }

                    return "+1";
                case Item.Types.Type.BonusCoupon:
                    return "+1";
                case Item.Types.Type.MonopolyActivityTicket:
                    return "+" + item.MonopolyActivityTicket.Amount;
            }

            return "";
        }

        public static string GetItemRewardSimplyDescText(Item item)
        {
            switch (item.Type)
            {
                case Item.Types.Type.Coin:
                    return item.Coin.Amount.GetAbbreviationFormat();
                case Item.Types.Type.ShopCoin:
                    return item.ShopCoin.AdditionAmount.GetAbbreviationFormat();
                case Item.Types.Type.Emerald:
                    return item.Emerald.Amount.GetAbbreviationFormat();
                case Item.Types.Type.ShopEmerald:
                    return item.ShopEmerald.AdditionAmount.GetAbbreviationFormat();
                case Item.Types.Type.DoubleExp:
                    return GetBuffDescText(item.DoubleExp.Amount);
                case Item.Types.Type.LevelUpBurst:
                    return GetBuffDescText(item.LevelUpBurst.Amount);
                case Item.Types.Type.CashBackBuffsBet:
                case Item.Types.Type.CashBackBuffsNowin:
                case Item.Types.Type.CashBackBuffsWin:
                    return GetBuffDescText(item.CashBackBuff.Amount);
                case Item.Types.Type.VipPoints:
                    return "+" + item.VipPoints.Amount.GetCommaFormat();
                case Item.Types.Type.ShopGiftBox:
                    return "+" + item.ShopGiftBox.Amount.GetCommaFormat();
                case Item.Types.Type.SuperWheel:
                    return GetBuffDescText(item.SuperWheel.Amount);
                case Item.Types.Type.ValentineActivityPoint:
                    return "+" + item.ValentineActivityPoint.Amount;
                case Item.Types.Type.IndependenceDayActivityPoint:
                    return "+" + item.IndependenceActivityPoint.Amount;
                case Item.Types.Type.GoldHammer:
                    return "+" + item.GoldHammer.Amount;
                case Item.Types.Type.NewbieQuestBoost:
                    return item.NewbieQuestBoost.Amount + " MINS";
                case Item.Types.Type.Avatar:
                    return "+" + item.Avatar.Count;
                case Item.Types.Type.CardPackage:
                    if (item.CardPackage.PackageConfig.TypeForShow == 16 ||
                        item.CardPackage.PackageConfig.TypeForShow == 17 ||
                        item.CardPackage.PackageConfig.TypeForShow == 18)
                    {
                        return "+" + item.CardPackage.PackageConfig.CardCount;
                    }

                    return "+1";
                case Item.Types.Type.BonusCoupon:
                    return "+1";
                case Item.Types.Type.MonopolyActivityTicket:
                    return "+" + item.MonopolyActivityTicket.Amount;
            }

            return "";
        }

        public static string GetItemDefaultDescText(Item item)
        {
            switch (item.Type)
            {
                case Item.Types.Type.Coin:
                    return ((long) item.Coin.Amount).GetCommaOrSimplify(7);
                case Item.Types.Type.ShopCoin:
                    return ((long) item.ShopCoin.AdditionAmount).GetCommaOrSimplify(7);
                case Item.Types.Type.Emerald:
                    return item.Emerald.Amount.GetCommaFormat();
                case Item.Types.Type.ShopEmerald:
                    return ((long) item.ShopEmerald.AdditionAmount).GetCommaOrSimplify(7);
                case Item.Types.Type.DoubleExp:
                    return GetBuffDescText(item.DoubleExp.Amount);
                case Item.Types.Type.LevelUpBurst:
                    return GetBuffDescText(item.LevelUpBurst.Amount);
                case Item.Types.Type.CashBackBuffsBet:
                case Item.Types.Type.CashBackBuffsNowin:
                case Item.Types.Type.CashBackBuffsWin:
                    return GetBuffDescText(item.CashBackBuff.Amount);
                case Item.Types.Type.VipPoints:
                    return "+" + item.VipPoints.Amount.GetCommaFormat();
                case Item.Types.Type.ShopGiftBox:
                    return "+" + item.ShopGiftBox.Amount.GetCommaFormat();
                case Item.Types.Type.MissionStar:
                    return item.MissionStar.Amount.GetCommaFormat();
                case Item.Types.Type.MissionPoints:
                    return item.MissionPoints.Amount.GetCommaFormat();
                case Item.Types.Type.SuperWheel:
                    return GetBuffDescText(item.SuperWheel.Amount);
                case Item.Types.Type.SeasonQuestStar:
                    return ((long) item.SeasonQuestStar.Amount).GetCommaOrSimplify(7);
                case Item.Types.Type.ValentineActivityPoint:
                    return "+" + item.ValentineActivityPoint.Amount;
                case Item.Types.Type.IndependenceDayActivityPoint:
                    return "+" + item.IndependenceActivityPoint.Amount;
                case Item.Types.Type.GoldHammer:
                    return "+" + item.GoldHammer.Amount;
                case Item.Types.Type.NewbieQuestBoost:
                    return item.NewbieQuestBoost.Amount + " MINS";
                case Item.Types.Type.Avatar:
                    return "+" + item.Avatar.Count;
                case Item.Types.Type.CardPackage:
                    if (item.CardPackage.PackageConfig.TypeForShow == 16 ||
                        item.CardPackage.PackageConfig.TypeForShow == 17 ||
                        item.CardPackage.PackageConfig.TypeForShow == 18)
                    {
                        return "+" + item.CardPackage.PackageConfig.CardCount;
                    }

                    return "+1";
                case Item.Types.Type.BonusCoupon:
                    return "+1";
                case Item.Types.Type.MonopolyActivityTicket:
                    return "+" + item.MonopolyActivityTicket.Amount;
                case Item.Types.Type.MonopolyActivityBuffMoreTicket:
                    return $"+{item.MonopolyActivityBuffMoreTicket.Amount} MINES";
                case Item.Types.Type.MonopolyActivityBuffMoreDamage:
                    return $"+{item.MonopolyActivityBuffMoreDamage.Amount} MINES";
                case Item.Types.Type.MonopolyActivityPortal:
                    return "+" + item.MonopolyActivityPortal.Amount;
            }

            return "";
        }

        public static string GetBuffDescText(ulong minutes)
        {
            if (minutes >= 60 * 24)
            {
                var days = minutes / (24.0f * 60.0f);
                return days + " DAYS";
            }

            if (minutes >= 60)
            {
                var hours = minutes / 60.0f;
                if (hours > 1)
                {
                    return hours + " HRS";
                }

                return hours + " HR";
            }
            else
            {
                return minutes + " MINS";
            }
        }

        public static void InitItemUI(Transform itemCell, Item item, Func<Item, string> descFunc = null,
            string uiType = "StandardType")
        {
            //先隐藏所有的除了制定的之外的所有UiType
            var childCount = itemCell.childCount;
            Transform activeUITransform = null;
            for (var i = 0; i < childCount; i++)
            {
                var child = itemCell.GetChild(i);
                if (child.name == uiType)
                {
                    child.gameObject.SetActive(true);
                    activeUITransform = child;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            if (activeUITransform == null)
            {
                XDebug.LogError("InitItemUI Can't Not Find Match UI Type!");
                return;
            }

            Sprite rewardIcon = GetItemSprite(item.Type, item);
            string descText = "";
            if (descFunc == null)
            {
                descText = GetItemDefaultDescText(item);
            }
            else
            {
                descText = descFunc.Invoke(item);
            }

            var rewardIconTransform = activeUITransform.Find("RewardIcon");
            if (rewardIconTransform != null && rewardIcon)
            {
                var iconImage = rewardIconTransform.GetComponent<Image>();
                if (iconImage)
                {
                    iconImage.sprite = rewardIcon;
                }

                rewardIconTransform.gameObject.SetActive(true);
            }

            var rawIcon = activeUITransform.Find("RawIcon");
            if (item.Type == Item.Types.Type.Avatar)
            {
                if (rawIcon != null)
                {
                    var rawImage = rawIcon.GetComponent<RawImage>();

                    var avatarAddress = "PlayerAvatar_" + item.Avatar.Id;
                    var texture = AssetHelper.GetResidentAsset<Texture2D>(avatarAddress);

                    rawIcon.gameObject.SetActive(true);

                    if (rewardIconTransform != null)
                    {
                        rewardIconTransform.gameObject.SetActive(false);
                    }

                    if (rawImage && texture != null)
                    {
                        rawImage.texture = texture;
                    }
                }
            }
            else if (rawIcon != null)
            {
                rawIcon.gameObject.SetActive(false);
            }

            var countText = activeUITransform.Find("CountText");

            if (countText != null)
            {
                var countTextMeshProUGUI = countText.GetComponent<TextMeshProUGUI>();
                if (countTextMeshProUGUI != null)
                {
                    countTextMeshProUGUI.text = descText;
                }
                else
                {
                    var textCountText = countText.GetComponent<Text>();
                    textCountText.text = descText;
                }
            }

            var customDataProxy = itemCell.GetComponent<MonoCustomDataProxy>();
            if (customDataProxy == null)
            {
                customDataProxy = itemCell.gameObject.AddComponent<MonoCustomDataProxy>();
            }

            customDataProxy.SetCustomData("Item", item);
        }

        public static Sprite GetItemSprite(Item.Types.Type itemType, Item item, string postfix = "_1")
        {
            if (itemType == Item.Types.Type.ShopEmerald)
                itemType = Item.Types.Type.Emerald;

            if (itemType == Item.Types.Type.ShopCoin)
                itemType = Item.Types.Type.Coin;

            if (item.Type == Item.Types.Type.CardPackage)
            {
                var showType = item.CardPackage.PackageConfig.TypeForShow == 0
                    ? 19
                    : item.CardPackage.PackageConfig.TypeForShow;
                postfix = "_" + showType;
            }

            if (item.Type == Item.Types.Type.BonusCoupon)
            {
                postfix = "_" + item.BonusCoupon.BonusPercentage;
            }

            string spriteName = "UI_" + itemType.ToString() + postfix;

            if (item.Type == Item.Types.Type.CashBackBuffsBet
                || item.Type == Item.Types.Type.CashBackBuffsNowin
                || item.Type == Item.Types.Type.CashBackBuffsWin)
            {
                spriteName = "UI_CashBack_1";
            }

            var spriteAtlas = AssetHelper.GetResidentAsset<SpriteAtlas>("ItemIconAtlas");
            if (spriteAtlas != null)
            {
                var itemSprite = spriteAtlas.GetSprite(spriteName);
                return itemSprite;
            }

            return null;
        }

        public static int InitItemsUI(Transform rewardGroup, RepeatedField<Reward> rewards,
            Transform itemTemplate = null,
            Func<List<Item>, string> descFunc = null, string uiType = "StandardType")
        {
            if (itemTemplate == null)
            {
                itemTemplate = rewardGroup.Find("CommonCell");
            }

            if (itemTemplate == null)
            {
                return 0;
            }

            itemTemplate.gameObject.SetActive(false);

            var uiCount = 0;

            foreach (Item.Types.Type itemType in Enum.GetValues(typeof(Item.Types.Type)))
                //for  (var i  = 0; i < itemTypes.Count; i++)
            {
                var items = GetItems(rewards, itemType);

                if (items != null && items.Count > 0)
                {
                    if (itemType == Item.Types.Type.CardPackage)
                    {
                        var itemGroup = SeparateCardPackageToDifferentGroup(items);

                        for (var g = 0; g < itemGroup.Count; g++)
                        {
                            var ui = GameObject.Instantiate(itemTemplate, rewardGroup);
                            ui.SetAsLastSibling();
                            ui.gameObject.SetActive(true);
                            InitItemsUI(ui, itemGroup[g], descFunc, uiType);
                            uiCount++;
                        }
                    }
                    else if (itemType == Item.Types.Type.BonusCoupon)
                    {
                        var itemGroup = SeparateBonusCouponToDifferentGroup(items);

                        for (var g = 0; g < itemGroup.Count; g++)
                        {
                            var ui = GameObject.Instantiate(itemTemplate, rewardGroup);
                            ui.SetAsLastSibling();
                            ui.gameObject.SetActive(true);
                            InitItemsUI(ui, itemGroup[g], descFunc, uiType);
                            uiCount++;
                        }
                    }
                    else
                    {
                        var rewardUI = itemTemplate;

                        rewardUI = GameObject.Instantiate(itemTemplate, rewardGroup);

                        rewardUI.SetAsLastSibling();
                        rewardUI.gameObject.SetActive(true);
                        InitItemsUI(rewardUI, items, descFunc, uiType);
                        uiCount++;
                    }
                }
            }

            return uiCount;
        }

        public static bool IsMultCard(uint typeForShow)
        {
            return (typeForShow == 16 || typeForShow == 17 || typeForShow == 18);
        }

        public static bool IsCardPackage(uint typeForShow)
        {
            return (typeForShow == 19 || typeForShow == 20 || typeForShow == 0);
        }

        public static List<List<Item>> SeparateCardPackageToDifferentGroup(List<Item> items)
        {
            Dictionary<string, List<Item>> groups = new Dictionary<string, List<Item>>();

            for (var i = 0; i < items.Count; i++)
            {
                var key = items[i].CardPackage.PackageConfig.TypeForShow.ToString();
                if (!groups.ContainsKey(items[i].CardPackage.PackageConfig.TypeForShow.ToString()))
                {
                    groups.Add(key, new List<Item>());
                }

                groups[key].Add(items[i]);
            }

            return groups.Values.ToList();
        }

        public static List<List<Item>> SeparateBonusCouponToDifferentGroup(List<Item> items)
        {
            Dictionary<string, List<Item>> groups = new Dictionary<string, List<Item>>();

            for (var i = 0; i < items.Count; i++)
            {
                var key = items[i].BonusCoupon.BonusPercentage;
                if (!groups.ContainsKey(key.ToString()))
                {
                    groups.Add(key.ToString(), new List<Item>());
                }

                groups[key.ToString()].Add(items[i]);
            }

            return groups.Values.ToList();
        }


        public static Dictionary<string, List<Item>> SeparateCardPackageToDifferentGroupBackDictionary(List<Item> items)
        {
            Dictionary<string, List<Item>> groups = new Dictionary<string, List<Item>>();

            for (var i = 0; i < items.Count; i++)
            {
                var key = items[i].CardPackage.PackageConfig.TypeForShow.ToString();
                if (!groups.ContainsKey(items[i].CardPackage.PackageConfig.TypeForShow.ToString()))
                {
                    groups.Add(key, new List<Item>());
                }

                groups[key].Add(items[i]);
            }

            return groups;
        }

        public static void InitItemsUI(Transform itemCell, List<Item> items, Func<List<Item>, string> descFunc = null,
            string uiType = "StandardType")
        {
            //先隐藏所有的除了制定的之外的所有UiType
            var childCount = itemCell.childCount;
            Transform activeUITransform = null;
            for (var i = 0; i < childCount; i++)
            {
                var child = itemCell.GetChild(i);
                if (child.name == uiType)
                {
                    child.gameObject.SetActive(true);
                    activeUITransform = child;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            if (activeUITransform == null)
            {
                XDebug.LogError("InitItemUI Can't Not Find Match UI Type!");
                return;
            }

            var item = items[0];
            Sprite rewardIcon = GetItemSprite(item.Type, item);
            string descText = "";
            if (descFunc == null)
            {
                descText = GetItemDefaultDescText(item);
            }
            else
            {
                descText = descFunc.Invoke(items);
            }

            var rewardIconTransform = activeUITransform.Find("RewardIcon");
            if (rewardIconTransform != null && rewardIcon)
            {
                var iconImage = rewardIconTransform.GetComponent<Image>();
                if (iconImage)
                {
                    iconImage.sprite = rewardIcon;
                }
            }

            var rawIcon = activeUITransform.Find("RawIcon");

            if (item.Type == Item.Types.Type.Avatar)
            {
                if (rawIcon != null)
                {
                    var rawImage = rawIcon.GetComponent<RawImage>();

                    var avatarAddress = "PlayerAvatar_" + item.Avatar.Id;
                    var texture = AssetHelper.GetResidentAsset<Texture2D>(avatarAddress);

                    rawIcon.gameObject.SetActive(true);

                    if (rewardIconTransform != null)
                    {
                        rewardIconTransform.gameObject.SetActive(false);
                    }

                    if (rawImage && texture != null)
                    {
                        rawImage.texture = texture;
                    }
                }
            }
            else if (rawIcon != null)
            {
                rawIcon.gameObject.SetActive(false);
            }


            var countText = activeUITransform.Find("CountText");
            if (countText == null)
            {
                countText = activeUITransform.Find("CountGroup/CountText");
            }

            if (countText != null)
            {
                var countTextMeshProUGUI = countText.GetComponent<TextMeshProUGUI>();
                if (countTextMeshProUGUI != null)
                {
                    countTextMeshProUGUI.text = descText;
                }
                else
                {
                    var countUnityText = countText.GetComponent<Text>();
                    if (countUnityText != null)
                    {
                        countUnityText.text = descText;
                    }
                }
            }

            var customDataProxy = itemCell.GetComponent<MonoCustomDataProxy>();
            if (customDataProxy == null)
            {
                customDataProxy = itemCell.gameObject.AddComponent<MonoCustomDataProxy>();
            }

            customDataProxy.SetCustomData("Item", items[0]);
        }


        public static void InitRewardsUI(Transform rewardGroup, RepeatedField<Reward> rewards, Transform itemTemplate,
            Func<Item, string> descFunc, string uiType = "StandardType")
        {
            if (itemTemplate == null)
            {
                return;
            }

            var itemCount = rewards.Count;

            for (var i = 0; i < itemCount; i++)
            {
                var rewardUI = itemTemplate;

                if (i != itemCount - 1)
                {
                    rewardUI = GameObject.Instantiate(itemTemplate, rewardGroup);
                }

                rewardUI.SetAsLastSibling();
                rewardUI.gameObject.SetActive(true);

                InitRewardUI(rewardUI, rewards[i], descFunc, uiType);
            }
        }

        public static void InitRewardUI(Transform itemCell, Reward reward, Func<Item, string> descFunc,
            string uiType = "StandardType")
        {
            //先隐藏所有的除了制定的之外的所有UiType
            var childCount = itemCell.childCount;
            Transform activeUITransform = null;
            uiType = $"{uiType}_{reward.Items.Count}";
            for (var i = 0; i < childCount; i++)
            {
                var child = itemCell.GetChild(i);
                if (child.name == uiType)
                {
                    child.gameObject.SetActive(true);
                    activeUITransform = child;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            if (activeUITransform == null)
            {
                XDebug.LogError("InitItemUI Can't Not Find Match UI Type!");
                return;
            }

            Sprite rewardIcon = GetRewardSprite(reward);

            var rewardIconTransform = activeUITransform.Find("RewardIcon");
            if (rewardIconTransform != null && rewardIcon)
            {
                var iconImage = rewardIconTransform.GetComponent<Image>();
                if (iconImage)
                {
                    iconImage.sprite = rewardIcon;
                }

                rewardIconTransform.gameObject.SetActive(true);
            }

            var textContents = activeUITransform.Find("CountContent");
            if (textContents != null)
            {
                var countTextMeshProUGUIs = textContents.GetComponentsInChildren<TextMeshProUGUI>();
                if (reward.Items.Count != countTextMeshProUGUIs.Length)
                {
                    XDebug.LogError("reward items count != reward gameobject text count");
                }

                if (countTextMeshProUGUIs.Length > 0)
                {
                    for (int i = 0; i < reward.Items.Count; i++)
                    {
                        var descText = "";
                        if (descFunc != null)
                        {
                            descText = descFunc.Invoke(reward.Items[i]);
                        }
                        else
                        {
                            descText = GetItemDefaultDescText(reward.Items[i]);
                        }

                        countTextMeshProUGUIs[i].text = descText;
                    }
                }
                else
                {
                    var textCountTexts = textContents.GetComponentsInChildren<Text>();
                    if (reward.Items.Count != textCountTexts.Length)
                    {
                        XDebug.LogError("reward items count != reward gameobject text count");
                    }

                    for (int i = 0; i < reward.Items.Count; i++)
                    {
                        var descText = "";
                        if (descFunc != null)
                        {
                            descText = descFunc.Invoke(reward.Items[i]);
                        }
                        else
                        {
                            descText = GetItemDefaultDescText(reward.Items[i]);
                        }

                        textCountTexts[i].text = descText;
                    }
                }
            }
        }

        public static Sprite GetRewardSprite(Reward reward)
        {
            var spriteName = "UI";

            for (int i = 0; i < reward.Items.Count; i++)
            {
                var item = reward.Items[i];
                var itemType = item.Type;

                var postfix = "_1";

                if (itemType == Item.Types.Type.ShopEmerald)
                    itemType = Item.Types.Type.Emerald;

                if (itemType == Item.Types.Type.ShopCoin)
                    itemType = Item.Types.Type.Coin;

                if (item.Type == Item.Types.Type.CardPackage)
                {
                    var showType = item.CardPackage.PackageConfig.TypeForShow == 0
                        ? 19
                        : item.CardPackage.PackageConfig.TypeForShow;
                    postfix = "_" + showType;
                }

                if (item.Type == Item.Types.Type.BonusCoupon)
                {
                    postfix = "_" + item.BonusCoupon.BonusPercentage;
                }

                if (item.Type == Item.Types.Type.CashBackBuffsBet
                    || item.Type == Item.Types.Type.CashBackBuffsNowin
                    || item.Type == Item.Types.Type.CashBackBuffsWin)
                {
                    spriteName = "UI_CashBack_1";
                    postfix = "";
                }

                spriteName = $"{spriteName}_{itemType.ToString()}{postfix}";
            }

            var spriteAtlas = AssetHelper.GetResidentAsset<SpriteAtlas>("ItemIconAtlas");
            if (spriteAtlas != null)
            {
                XDebug.LogWarning(spriteName);
                var itemSprite = spriteAtlas.GetSprite(spriteName);
                return itemSprite;
            }

            return null;
        }
    }
}