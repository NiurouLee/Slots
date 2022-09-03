// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/06/16:19
// Ver : 1.0.0
// Description : StoreCommodityView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class StoreCommodityView : View
    {
        [ComponentBinder("Viewport/Content")] 
        public Transform contentTransform;

        [ComponentBinder("Viewport/Content/UIStoreBonusCell")]
        public Transform commodityBonusTemplateTransform;

        [ComponentBinder("Viewport/Content/UIStoreCommodityCoinCell")]
        public Transform commodityCoinsTemplateTransform;

        [ComponentBinder("Viewport/Content/UIStoreCommodityDiamondCell")]
        public Transform commodityDiamondTemplateTransform;

        [ComponentBinder("Viewport/Content/UIStoreCommodityBoostLevelUpCell")]
        public Transform commodityBoostLevelUpTemplateTransform;

        [ComponentBinder("Viewport/Content/UIStoreCommodityBoostWheelBonusCell")]
        public Transform commodityBoostWheelBonusTemplateTransform;

        [ComponentBinder("Viewport/Content/UIStoreDiamondBlock")]
        public RectTransform commodityDiamondBlock;

        [ComponentBinder("Viewport/Content/UIStoreBoostBlock")]
        public Transform commodityBoostBlock;

        public ScrollRect scrollRect;

        public CommodityBonusView commodityBonusView;

        public List<CommodityCoinsView> commodityCoinsViews;
        public List<CommodityDiamondView> commodityDiamondViews;
        public CommodityBoostLevelUpView commodityBoostLevelUpView;
        public CommodityBoostWheelBonusView commodityBoostWheelBonusView;

        public RectTransform firstCommodityCoin;
        public RectTransform firstCommodityDiamond;
        public RectTransform firstCommodityBoost;

        public Action<CommodityPageType> OnPageChangeEvent;

        private CommodityPageType _currentPage;

        private bool _isVerticalView = false;
        public enum CommodityPageType
        {
            Coin = 0,
            Diamond = 1,
            Boost = 2
        }

        protected override void SetUpExtraView()
        {
            HideTemplateView();
            _currentPage = CommodityPageType.Coin;
        }

        protected void HideTemplateView()
        {
            commodityBonusTemplateTransform.gameObject.SetActive(false);
            commodityCoinsTemplateTransform.gameObject.SetActive(false);
            commodityDiamondTemplateTransform.gameObject.SetActive(false);
            commodityBoostLevelUpTemplateTransform.gameObject.SetActive(false);
            commodityBoostWheelBonusTemplateTransform.gameObject.SetActive(false);
        }

        protected void OnScrollValueChanged(Vector2 value)
        {
            var viewportWidth = scrollRect.viewport.rect.size.x;
            var contentWidth = scrollRect.content.sizeDelta.x;
            var totalWidth = contentWidth - viewportWidth;

            var coinStartPosition = firstCommodityCoin.anchoredPosition.x - firstCommodityCoin.sizeDelta.x * 0.5f;
            var diamondStartPosition =
                firstCommodityDiamond.anchoredPosition.x - firstCommodityDiamond.sizeDelta.x * 0.5f - commodityDiamondBlock.sizeDelta.x - 50f;
            var boostStartPosition = firstCommodityBoost.anchoredPosition.x + firstCommodityBoost.sizeDelta.x * 0.5f;

            var currentRightAnchoredPosition = value.x * totalWidth + viewportWidth;
            var newestPage = _currentPage;
            
            if (_isVerticalView)
            {
                viewportWidth = scrollRect.viewport.rect.size.y;
                contentWidth = scrollRect.content.sizeDelta.y;
                totalWidth = contentWidth - viewportWidth;

                coinStartPosition = -(firstCommodityCoin.anchoredPosition.y + firstCommodityCoin.sizeDelta.y * 0.5f);
                diamondStartPosition =-(
                firstCommodityDiamond.anchoredPosition.y + firstCommodityDiamond.sizeDelta.y * 0.5f);
                boostStartPosition = -(firstCommodityBoost.anchoredPosition.y + firstCommodityBoost.sizeDelta.y * 0.5f);

                currentRightAnchoredPosition = (1 - Math.Min(1,Math.Max(value.y,0))) * totalWidth + viewportWidth;
            }
            
            //value.x += viewportWidth / totalWidth;
            if (currentRightAnchoredPosition > boostStartPosition && currentRightAnchoredPosition - viewportWidth > diamondStartPosition)
            {
                newestPage = CommodityPageType.Boost;
            }

            else if (currentRightAnchoredPosition > diamondStartPosition)
            {
                newestPage = CommodityPageType.Diamond;
            }
            else if (currentRightAnchoredPosition > coinStartPosition)
            {
                newestPage = CommodityPageType.Coin;
            }

            if (newestPage != _currentPage)
            {
                _currentPage = newestPage;
                OnPageChangeEvent?.Invoke(_currentPage);
            }
        }

        public void TurnToPage(CommodityPageType pageType,bool showScrollAnimation = true)
        {
            if (_isVerticalView)
            {
                TurnToPageV(pageType, showScrollAnimation);
            }
            else
            {
                TurnToPageH(pageType, showScrollAnimation);
            }
        }
        public void TurnToPageH(CommodityPageType pageType, bool showScrollAnimation)
        {
            var viewportWidth = scrollRect.viewport.rect.size.x;
            var contentWidth = scrollRect.content.sizeDelta.x;
            var totalWidth = contentWidth - viewportWidth;
            var diamondStartPosition = firstCommodityDiamond.anchoredPosition.x -
                                       firstCommodityDiamond.sizeDelta.x * 0.5f - commodityDiamondBlock.sizeDelta.x - 50f;
            var boostStartPosition = firstCommodityBoost.anchoredPosition.x - firstCommodityBoost.sizeDelta.x * 0.5f;
            
            var startPos = scrollRect.horizontalNormalizedPosition;
          
            if(_turnPageTurn != null)
                _turnPageTurn.Kill();
          
            float targetPos = 0.0f;
            
            switch (pageType)
            {
                case CommodityPageType.Coin:
                    targetPos = 0.0f;
                    break;
                case CommodityPageType.Diamond:
                    targetPos = diamondStartPosition / totalWidth;
                    break;
                case CommodityPageType.Boost:
                    targetPos = Math.Min(1, boostStartPosition / totalWidth);
                    break;
            }
            
            if(showScrollAnimation)
                _turnPageTurn = DOTween.To(() =>startPos, (p) => { scrollRect.horizontalNormalizedPosition = p; }, targetPos, 0.3f);
            else
            {
                scrollRect.horizontalNormalizedPosition = targetPos;
            }
        }

        private Tween _turnPageTurn = null;
        public void TurnToPageV(CommodityPageType pageType, bool showScrollAnimation)
        {
            var viewportWidth = scrollRect.viewport.rect.size.y;
            var contentWidth = scrollRect.content.sizeDelta.y;
            var totalWidth = contentWidth - viewportWidth;
            var diamondStartPosition = firstCommodityDiamond.anchoredPosition.y +
                                       firstCommodityDiamond.sizeDelta.y * 0.5f + commodityDiamondBlock.sizeDelta.y +
                                       30;
            var boostStartPosition = firstCommodityBoost.anchoredPosition.y + firstCommodityBoost.sizeDelta.y * 0.5f;
           
            var startPos = scrollRect.verticalNormalizedPosition;
          
            if(_turnPageTurn != null)
                _turnPageTurn.Kill();
            
            float targetPos = 0.0f;
            
            switch (pageType)
            {
                case CommodityPageType.Coin:
                    targetPos = 1.0f;
                    break;
                case CommodityPageType.Diamond:
                    targetPos = ((contentWidth + diamondStartPosition) - viewportWidth) / totalWidth;
                    break;
                case CommodityPageType.Boost:
                    targetPos = Math.Max(0, boostStartPosition / totalWidth);
                    break;
            }

            if (showScrollAnimation)
            {
                _turnPageTurn = DOTween.To(() => startPos, (p) => { scrollRect.verticalNormalizedPosition = p; },
                    targetPos, 0.3f);
            }
            else
            {
                scrollRect.verticalNormalizedPosition = targetPos;
            }
        }

        public void SetUpCommodityView(RepeatedField<ShopItemConfig> commodityList)
        {
            var coinCommodityList = new List<ShopItemConfig>(6); 
            var diamondCommodityList = new List<ShopItemConfig>(6); 
            var boostWheelBonusCommodityList = new List<ShopItemConfig>(6); 
            var boostLevelUpCommodityList = new List<ShopItemConfig>(6); 

            int commodityCoinsCount = 6;
            int commodityDiamondCount = 6;
            _isVerticalView = parentView.GetAssetAddressName().Contains("V");
            
            if (commodityList != null)
            {
                for (var i = 0; i < commodityList.Count; i++)
                {
                    switch (commodityList[i].ProductType)
                    {
                        case "shopcoin": 
                            coinCommodityList.Add(commodityList[i]);
                            break;
                        case "shopemerald": 
                            diamondCommodityList.Add(commodityList[i]);
                            break;
                        case "levelupburst":
                            boostLevelUpCommodityList.Add(commodityList[i]);
                            break;
                        case "superwheel":
                            boostWheelBonusCommodityList.Add(commodityList[i]);
                            break;
                    }
                }

                commodityCoinsCount = coinCommodityList.Count;
                commodityDiamondCount = diamondCommodityList.Count;
            }
             
            var siblingIndex = 0;
            
            commodityBonusView = GetCommodityView<CommodityBonusView>(commodityBonusTemplateTransform,true);
            commodityBonusView.transform.SetSiblingIndex(siblingIndex++);

            commodityCoinsViews = new List<CommodityCoinsView>(commodityCoinsCount);
         
            for (var i = 0; i < commodityCoinsCount; i++)
            {
                var coinView = GetCommodityView<CommodityCoinsView>(commodityCoinsTemplateTransform, i == commodityCoinsCount - 1);
                coinView.transform.SetSiblingIndex(siblingIndex++);
                coinView.UpdateView(i, coinCommodityList[i],_isVerticalView);
                commodityCoinsViews.Add(coinView);
                
                if (i == 0)
                {
                    firstCommodityCoin = coinView.rectTransform;
                }
            }

            commodityDiamondBlock.SetSiblingIndex(siblingIndex++);
            commodityDiamondViews = new List<CommodityDiamondView>(commodityDiamondCount);
            for (var i = 0; i < commodityDiamondCount; i++)
            {
                var diamondView = GetCommodityView<CommodityDiamondView>(commodityDiamondTemplateTransform, i == commodityDiamondCount - 1);
                diamondView.transform.SetSiblingIndex(siblingIndex++);
                diamondView.UpdateView(i, diamondCommodityList[i], _isVerticalView);
                commodityDiamondViews.Add(diamondView);
                
                if (i == 0)
                {
                    firstCommodityDiamond = diamondView.rectTransform;
                }
            }

            commodityBoostBlock.SetSiblingIndex(siblingIndex++);

            commodityBoostLevelUpView =
                GetCommodityView<CommodityBoostLevelUpView>(commodityBoostLevelUpTemplateTransform, true);
            commodityBoostLevelUpView.transform.SetSiblingIndex(siblingIndex++);
            commodityBoostLevelUpView.UpdateView(boostLevelUpCommodityList, _isVerticalView);

            firstCommodityBoost = commodityBoostLevelUpView.rectTransform;

            if (boostWheelBonusCommodityList.Count > 0)
            {
                commodityBoostWheelBonusView =
                    GetCommodityView<CommodityBoostWheelBonusView>(commodityBoostWheelBonusTemplateTransform, true);
                commodityBoostWheelBonusView.transform.SetSiblingIndex(siblingIndex++);
                commodityBoostWheelBonusView.UpdateView(boostWheelBonusCommodityList, _isVerticalView);
            }

            if (_isVerticalView)
            {
                var placeHolder = new GameObject("PlaceHolder", new[] {typeof(RectTransform)});
                placeHolder.transform.SetParent(contentTransform,false);
                placeHolder.transform.SetSiblingIndex(siblingIndex);
                ((RectTransform) placeHolder.transform).sizeDelta = new Vector2(768, 100);
            }
            
            scrollRect = transform.GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        public void UpdateCommodityView(RepeatedField<ShopItemConfig> commodityList)
        {
            var coinCommodityList = new List<ShopItemConfig>(6); 
            var diamondCommodityList = new List<ShopItemConfig>(6); 
            var boostWheelBonusCommodityList = new List<ShopItemConfig>(6); 
            var boostLevelUpCommodityList = new List<ShopItemConfig>(6); 

         
            _isVerticalView = parentView.GetAssetAddressName().Contains("V");
            
            if (commodityList != null)
            {
                for (var i = 0; i < commodityList.Count; i++)
                {
                    switch (commodityList[i].ProductType)
                    {
                        case "shopcoin": 
                            coinCommodityList.Add(commodityList[i]);
                            break;
                        case "shopemerald": 
                            diamondCommodityList.Add(commodityList[i]);
                            break;
                        case "levelupburst":
                            boostLevelUpCommodityList.Add(commodityList[i]);
                            break;
                        case "superwheel":
                            boostWheelBonusCommodityList.Add(commodityList[i]);
                            break;
                    }
                }

                var commodityCoinsCount = coinCommodityList.Count;
                var commodityDiamondCount = diamondCommodityList.Count;

                for (var i = 0; i < commodityCoinsCount; i++)
                {
                    commodityCoinsViews[i].UpdateView(i, coinCommodityList[i], _isVerticalView);
                }

                for (var i = 0; i < commodityDiamondCount; i++)
                {
                    commodityDiamondViews[i].UpdateView(i, diamondCommodityList[i], _isVerticalView);
                }

                commodityBoostLevelUpView.UpdateView(boostLevelUpCommodityList, _isVerticalView);

                if(boostWheelBonusCommodityList.Count > 0 && commodityBoostWheelBonusView != null)
                    commodityBoostWheelBonusView.UpdateView(boostWheelBonusCommodityList, _isVerticalView);
            }
        }

        public T GetCommodityView<T>(Transform template,bool useTemplate = false) where T : View
        {
            var viewGo = template.gameObject;
            
            if(!useTemplate)
                viewGo = GameObject.Instantiate(template.gameObject, contentTransform);
            
            viewGo.SetActive(true);
            var view = this.AddChild<T>(viewGo.transform);
            return view;
        }
    }
}