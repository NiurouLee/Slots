// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/01/16:16
// Ver : 1.0.0
// Description : CoinDashPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GameModule 
{
    [AssetAddress("UICoinDashPopup","", "UICoinDashPopupPad")]
    public class CoinDashPopup: Popup<CoinDashPopupViewController>
    {
        [ComponentBinder("Root/BottomGroup/EndText")]
        public TextMeshProUGUI endText;

        [ComponentBinder("Root/MainGroup")]
        public RectTransform mainGroup;

        public List<CoinDashItemView> dashItemViews;
        
        public CoinDashPopup(string address)
            :base(address)
        {
            
        }
        
        protected override void SetUpExtraView()
        {
            dashItemViews = new List<CoinDashItemView>();

            var childCount = mainGroup.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = mainGroup.GetChild(i);
                var dashItemView = AddChild<CoinDashItemView>(child);
                dashItemViews.Add(dashItemView);
            }

            base.SetUpExtraView();
            
            AdaptScaleTransform(mainGroup,new Vector2(1602,768));
        }
    }
    public class CoinDashPopupViewController: ViewController<CoinDashPopup>
    {
        protected CoinDashController coinDashController;

        protected bool claimingReward = false;
        
        public override void OnViewEnabled()
        {
            base.OnViewDidLoad();
            
            coinDashController = Client.Get<CoinDashController>();
            var viewCount = coinDashController.GetCoinDashItemCount();
            
            if (view.dashItemViews.Count < viewCount)
            {
                viewCount = view.dashItemViews.Count;
            }

            var activeIndex = coinDashController.GetActiveItemIndex();
            for (var i = 0; i < viewCount; i++)
            {
                view.dashItemViews[i].viewController
                    .SetUpDashItemView(i, coinDashController.GetCoinDashItemInfo(i), activeIndex == i);
            }
            
            EnableUpdate();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventCoinDashDataUpdate>(OnEventCoinDashDataUpdate);
        }

        protected void OnEventCoinDashDataUpdate(EventCoinDashDataUpdate evt)
        {
            for (int i = 0; i < view.dashItemViews.Count; i++)
            {
                view.dashItemViews[i].viewController.OnEventCoinDashDataUpdate();   
            }
        }

        public override void Update()
        {
            var leftTime = coinDashController.GetLeftTime();
            view.endText.text = $"EVENT ENDS IN: {XUtility.GetTimeText(leftTime)}";

            if (leftTime <= 0)
            {
                DisableUpdate();
                if (!claimingReward)
                {
                    view.Close();
                }
            }
        }

        public void OnDashViewClaimRewardFinished(int index)
        {
            claimingReward = false;

            var activeItemIndex = coinDashController.GetActiveItemIndex();
            if (activeItemIndex < view.dashItemViews.Count)
                view.dashItemViews[activeItemIndex].viewController.OnItemActivated();
        }
        
        public void OnDashViewClaimRewardWithError(int index)
        {
            claimingReward = false;
        }

        public void OnDashViewClaimRewardBegin(int index)
        {
            claimingReward = true;
        }
    }
}