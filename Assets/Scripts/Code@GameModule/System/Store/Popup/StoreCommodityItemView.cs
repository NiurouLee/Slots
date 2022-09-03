// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/06/16:16
// Ver : 1.0.0
// Description : StoreCommodityItemView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class PriceButtonExtraItemView: View<ViewController>
    {
        [ComponentBinder("ExtraGroup")] 
        public Transform extraGroup;   
        
        [ComponentBinder("SuperSpinX")] 
        public Transform superSpinX;

        public List<View> activeExtraItem;

        public View superSpinXView;

        public ShopItemConfig shopItemConfig;

        protected override void SetUpExtraView()
        {
            activeExtraItem = new List<View>();
            superSpinXView = AddChild<View>(superSpinX);
            superSpinXView.Hide();
            base.SetUpExtraView();
        }

        public void RefreshExtraItem(ShopItemConfig inShopItemConfig)
        {
            bool diff = shopItemConfig.SuperSpinBehind != inShopItemConfig.SuperSpinBehind;
            shopItemConfig = inShopItemConfig;

            if (diff)
            {
                if (shopItemConfig.SuperSpinBehind)
                {
                    activeExtraItem.Add(superSpinXView);

                    if (nextSwitchAction == null)
                    {
                        CheckAndStartSwitchAnimation();
                    }
                }
                else
                {
                    bool needPlaySwitchAnimation = false;

                    if (currentActiveIndex >= 0 && activeExtraItem.Count > currentActiveIndex)
                    {
                        if (activeExtraItem[currentActiveIndex] == superSpinXView)
                            needPlaySwitchAnimation = true;
                    }

                    activeExtraItem.Remove(superSpinXView);

                    if (needPlaySwitchAnimation)
                    {
                        CheckAndStartSwitchAnimation();
                    }
                }
            }
        }

        public void CollectExtraItem(ShopItemConfig inShopItemConfig)
        {
            shopItemConfig = inShopItemConfig;

            if (shopItemConfig.SuperSpinBehind)
            {
                activeExtraItem.Add(superSpinXView);
            }

            EventBus.Dispatch(new EventStorePrizeButtonAttachExtraItem(this), CheckAndStartSwitchAnimation);
        }

        public void AddExtraItem(View view)
        {
            AddChild(view);
            view.transform.SetParent(extraGroup,false);
            activeExtraItem.Add(view);
        }

        public void RemoveExtraItemView(View view)
        {
            RemoveChild(view);

            bool needPlaySwitchAnimation = false;

            if (currentActiveIndex >= 0 && activeExtraItem.Count > currentActiveIndex)
            {
                if (activeExtraItem[currentActiveIndex] == view)
                    needPlaySwitchAnimation = true;
            }

            activeExtraItem.Remove(view);

            if (needPlaySwitchAnimation)
            {
                CheckAndStartSwitchAnimation();
            }
        }

        public int currentActiveIndex = 0;
        
        public CancelableCallback nextSwitchAction;
        public void CheckAndStartSwitchAnimation()
        {
            if (nextSwitchAction != null)
            {
                nextSwitchAction.CancelCallback();
                nextSwitchAction = null;
            }
            
            if (activeExtraItem.Count > 0)
            {
                var totalCount = activeExtraItem.Count;
                currentActiveIndex++;
                currentActiveIndex %= totalCount;
                if (activeExtraItem.Count > currentActiveIndex)
                {
                    activeExtraItem[(currentActiveIndex - 1 + totalCount) % totalCount].Hide();
                    activeExtraItem[currentActiveIndex].Show();
                    nextSwitchAction = viewController.WaitForSeconds(3, CheckAndStartSwitchAnimation);
                }
            }
            else
            {
                currentActiveIndex = 0;
                Hide();
            }
        }
    }
    public class CommodityCoinsView : CommodityShopItemView
    {
        [ComponentBinder("Root/LowerGroup/PriceButton/PlusActivityPoint")]
        public Transform priceButtonActivityRoot;

        [ComponentBinder("Root/GoldCouponPoint/UIGoldCouponStoreIcon")]
        public Transform transformGoldCoupon;

        [ComponentBinder("CurrentQuantityText")]
        public Text currentQuantityText;

        [ComponentBinder("CommodityIconGroup")]
        public Transform commodityIconGroup;

        [ComponentBinder("OriginQuantityText")]
        public TextMeshProUGUI originQuantityText;

        [ComponentBinder("OriginQuantityGroup")]
        public Transform originQuantityGroup;

        [ComponentBinder("PageContent")] public Transform pageContent;

        [ComponentBinder("BestTag")] public Transform bestTag;

        [ComponentBinder("MoreTag")] public Transform moreTag;

        [ComponentBinder("MostTag")] public Transform mostTag;

        private PriceButtonExtraItemView _extraItemView;  

        private UIGoldCouponStoreIconView _goldCoupon;

        private int _positionIndex = 0;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (transformGoldCoupon != null)
            {
                _goldCoupon = AddChild<UIGoldCouponStoreIconView>(transformGoldCoupon);
            }
        }

        protected override void SetUpExtraView()
        {
            if (priceButtonActivityRoot != null)
            {
                _extraItemView = AddChild<PriceButtonExtraItemView>(priceButtonActivityRoot);
            }

            base.SetUpExtraView();
        }

        public void UpdateView(int index, ShopItemConfig inShopItemConfig, bool inIsVertical = false)
        {
            base.UpdateView(inIsVertical);

            if (shopItemConfig == null)
            {
                if(_extraItemView != null)
                    _extraItemView.CollectExtraItem(inShopItemConfig);
            }
            else
            {
                if (_extraItemView != null)
                    _extraItemView.RefreshExtraItem(inShopItemConfig);
            }

            _positionIndex = index;
            shopItemConfig = inShopItemConfig;

            var childCount = commodityIconGroup.childCount;

            for (var i = 0; i < childCount; i++)
            {
                //commodityIconGroup.GetChild(i).gameObject.SetActive((childCount - i - 1) == index);
                commodityIconGroup.GetChild(i).gameObject.SetActive((i + 1).ToString() == shopItemConfig.Image);
            }

            priceText.text = "$" + shopItemConfig.Price;

            mostTag.gameObject.SetActive(index == 0);
            bestTag.gameObject.SetActive(index == 3);

            if (shopItemConfig.ShowDiscount > 0)
            {
                moreTag.gameObject.SetActive(true);
                moreTag.Find("PercentText").GetComponent<TextMeshProUGUI>().text = shopItemConfig.ShowDiscount + "%";
            }
            else
            {
                moreTag.gameObject.SetActive(false);
            }

            if (shopItemConfig != null)
            {
                var primaryItem = shopItemConfig.SubItemList[0];

                UpdatePrimaryItem(primaryItem);

                bestTag.gameObject.SetActive(shopItemConfig.BestValue);
                mostTag.gameObject.SetActive(shopItemConfig.MostPopular);

                priceText.text = "$" + shopItemConfig.Price;

                SetUpPageContent(pageContent, shopItemConfig);


                SetCouponIcon(shopItemConfig.ShowDiscountCoupon);
            }
        }

        private void SetCouponIcon(float coupon)
        {
            if (_goldCoupon == null) { return; }
            if (coupon == 0)
            {
                _goldCoupon.Hide();
            }
            else
            {
                _goldCoupon.Show();
                _goldCoupon.Set(coupon);
            }
        }
        
        protected virtual void UpdatePrimaryItem(Item primaryItem)
        {
            if (primaryItem != null && primaryItem.ShopCoin != null)
            {
                currentQuantityText.SetText(primaryItem.ShopCoin.AdditionAmount.GetCommaFormat());
                originQuantityText.text = primaryItem.ShopCoin.Amount.GetCommaFormat();

                if (primaryItem.ShopCoin.AdditionAmount == primaryItem.ShopCoin.Amount)
                {
                    originQuantityGroup.gameObject.SetActive(false);
                }
            }
        }
    }

    public class CommodityDiamondView : CommodityCoinsView
    {
        protected override void UpdatePrimaryItem(Item primaryItem)
        {
            if (primaryItem != null && primaryItem.ShopEmerald != null)
            {
                currentQuantityText.text = primaryItem.ShopEmerald.AdditionAmount.GetCommaFormat();
                originQuantityText.text = primaryItem.ShopEmerald.Amount.GetCommaFormat();


                if (primaryItem.ShopEmerald.AdditionAmount == primaryItem.ShopEmerald.Amount)
                {
                    originQuantityGroup.gameObject.SetActive(false);
                }
            }
        }
    }

    public class CommodityBoostView : CommodityShopItemView
    {
        [ComponentBinder("BoostInformationButton")]
        public Button boostInformationButton;

        [ComponentBinder("NextButton")] public Button nextButton;

        [ComponentBinder("PreviousButton")] public Button previousButton;

        [ComponentBinder("ScrollView")] public ScrollRect scrollRect;

        [ComponentBinder("CountText")] public TextMeshProUGUI countText;

        [ComponentBinder("UIStoreCommodityBoostBubble")]
        public Transform bubble;

        [ComponentBinder("Root/LowerGroup/ScrollView/Viewport/Content")]
        public RectTransform content;

        public Action<CommodityBoostView> onBoostInfoClicked;

        protected List<ShopItemConfig> _shopItemConfigList;

        protected int currentIndex = -1;

        protected List<Transform> listPageContent;

        protected string[] days = { "1 DAY", "3 DAYS", "7 DAYS" };

        public void UpdateView(List<ShopItemConfig> inShopItemConfigList, bool inIsVertical)
        {
            base.UpdateView(inIsVertical);

            _shopItemConfigList = inShopItemConfigList;

            CalculateDaysText();
            if (currentIndex < 0)
                currentIndex = 1;

            shopItemConfig = inShopItemConfigList[currentIndex];

            var pageContentTemplate = content.Find("PageContent").gameObject;

            priceText.text = "$" + _shopItemConfigList[currentIndex].Price;

            countText.text = days[currentIndex];

            listPageContent = new List<Transform>(3);

            for (var i = content.childCount - 1; i >= 0; i--)
            {
                var child = content.GetChild(i);
                if (child.gameObject.name == "PageContentClone")
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            for (var i = 0; i < _shopItemConfigList.Count; i++)
            {
                var pageContent = pageContentTemplate;

                if (i < _shopItemConfigList.Count - 1)
                {
                    pageContent = GameObject.Instantiate(pageContentTemplate, content);
                    pageContent.name = "PageContentClone";
                }

                pageContent.transform.SetSiblingIndex(i);

                listPageContent.Add(pageContent.transform);
                SetUpPageContent(pageContent.transform, _shopItemConfigList[i]);
            }

            DOVirtual.DelayedCall(0.1f, () =>
            {
                scrollRect.horizontalNormalizedPosition = ((float)currentIndex) / (_shopItemConfigList.Count - 1);
            });
        }

        protected virtual void CalculateDaysText()
        {

        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            boostInformationButton.onClick.AddListener(OnBoostInfoClicked);
            previousButton.onClick.AddListener(OnPreviousButtonClicked);
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        private Tween _turnPageTurn;

        protected void OnPreviousButtonClicked()
        {
            currentIndex = (currentIndex + _shopItemConfigList.Count - 1) % _shopItemConfigList.Count;

            priceText.text = "$" + _shopItemConfigList[currentIndex].Price;
            shopItemConfig = _shopItemConfigList[currentIndex];

            for (var i = 0; i < _shopItemConfigList.Count; i++)
            {
                listPageContent[(currentIndex + i) % _shopItemConfigList.Count].SetSiblingIndex(i);
            }

            scrollRect.horizontalNormalizedPosition = 1.0f / (_shopItemConfigList.Count - 1);

            var start = scrollRect.horizontalNormalizedPosition;
            var target = 0;

            _turnPageTurn = DOTween.To(() => start, (p) => { scrollRect.horizontalNormalizedPosition = p; }, target,
                0.3f);


            countText.text = days[currentIndex];
        }

        protected void OnNextButtonClicked()
        {
            SoundController.PlayButtonClick();
            var itemCount = _shopItemConfigList.Count;
            currentIndex = (currentIndex + 1) % itemCount;

            for (var i = 0; i < itemCount; i++)
            {
                listPageContent[(currentIndex + i) % _shopItemConfigList.Count].SetSiblingIndex(itemCount - i - 1);
            }

            scrollRect.horizontalNormalizedPosition = (float)(itemCount - 2) / (itemCount - 1);

            var start = scrollRect.horizontalNormalizedPosition;
            var target = 1;

            _turnPageTurn = DOTween.To(() => start, (p) => { scrollRect.horizontalNormalizedPosition = p; }, target,
                0.3f);

            priceText.text = "$" + _shopItemConfigList[currentIndex].Price;
            shopItemConfig = _shopItemConfigList[currentIndex];

            countText.text = days[currentIndex];
        }

        protected void OnBoostInfoClicked()
        {
            SoundController.PlayButtonClick();
            onBoostInfoClicked.Invoke(this);
            // HighLightView(!bubble.gameObject.activeSelf);
        }

        public bool IsHighLight()
        {
            return transform.GetComponent<Canvas>().overrideSorting;
        }

        public void HighLightView(bool enableHighlight)
        {
            var canvas = transform.GetComponent<Canvas>();

            if (enableHighlight)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 2;
                canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");

                bubble.gameObject.SetActive(true);
            }
            else
            {
                canvas.overrideSorting = false;
                bubble.gameObject.SetActive(false);
            }
        }
    }

    public class CommodityBoostWheelBonusView : CommodityBoostView
    {
        protected override void CalculateDaysText()
        {
            if (_shopItemConfigList != null && _shopItemConfigList.Count > 0)
            {
                days = new string[_shopItemConfigList.Count];

                for (var i = 0; i < _shopItemConfigList.Count; i++)
                {
                    var day = TimeSpan.FromMinutes(_shopItemConfigList[i].SubItemList[0].SuperWheel.Amount).TotalDays;
                    if (day > 1)
                        days[i] = day + " DAYS";
                    else
                    {
                        days[i] = day + " DAY";
                    }
                }
            }
        }
    }

    public class CommodityBoostLevelUpView : CommodityBoostView
    {
        protected override void CalculateDaysText()
        {
            if (_shopItemConfigList != null && _shopItemConfigList.Count > 0)
            {
                days = new string[_shopItemConfigList.Count];

                for (var i = 0; i < _shopItemConfigList.Count; i++)
                {
                    var day = TimeSpan.FromMinutes(_shopItemConfigList[i].SubItemList[0].LevelUpBurst.Amount).TotalDays;
                    if (day > 1)
                        days[i] = day + " DAYS";
                    else
                    {
                        days[i] = day + " DAY";
                    }
                }
            }
        }
    }
}