// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/28/14:46
// Ver : 1.0.0
// Description : QuestPayPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class QuestPayItemView : View
    {
        [ComponentBinder("ContentGroup/QuestPayCell1/StandardType/CountText")]
        public TextMeshProUGUI coinCountText1;

        [ComponentBinder("ContentGroup/QuestPayCell2/StandardType/CountText")]
        public TextMeshProUGUI buffCountText;
 
        [ComponentBinder("DetailButton")] public Button detailButton;

        [ComponentBinder("PriceButton/PriceText")]
        public Text priceText;

        [ComponentBinder("PriceButton")] public Button priceButton;

        private Action<int> _onBuyClickAction;
        private Action<int> _benefitsClickAction;

        private int _index;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            detailButton.onClick.AddListener(OnDetailClicked);
            priceButton.onClick.AddListener(OnBuyButtonClicked);
        }

        protected void OnDetailClicked()
        {
            _benefitsClickAction?.Invoke(_index);
        }

        protected void OnBuyButtonClicked()
        {
            _onBuyClickAction?.Invoke(_index);
        }

        public void SetUpItemView(ShopItemConfig shopItemConfig, int index, Action<int> onBuyClickAction,
            Action<int> onBenefitClickAction)
        {
            var coin = XItemUtility.GetItem(shopItemConfig.SubItemList, Item.Types.Type.Coin);
            var newBieQuestBoost = XItemUtility.GetItem(shopItemConfig.SubItemList, Item.Types.Type.NewbieQuestBoost);
            _index = index;

            if (coin != null)
            {
                coinCountText1.text = coin.Coin.Amount.GetAbbreviationFormat();
            }
            else
            {
                coinCountText1.text = "0";
            }

            if (newBieQuestBoost != null)
            {
                buffCountText.text = XItemUtility.GetItemDefaultDescText(newBieQuestBoost);
            }

            priceText.text = "$" + shopItemConfig.Price;

            _onBuyClickAction = onBuyClickAction;
            _benefitsClickAction = onBenefitClickAction;
        }
    }

    [AssetAddress("UIQuestPay")]
    public class QuestPayPopup : Popup<QuestPayPopupViewController>
    {
        [ComponentBinder("Root/TopGroup")]
        public Transform topGroup;
        
        [ComponentBinder("Root/MainGroup")]
        public Transform mainGroup;
        
        [ComponentBinder("Root/MainGroup/Commodity1")]
        public Transform commodity1;

        [ComponentBinder("Root/MainGroup/Commodity2")]
        public Transform commodity2;

        [ComponentBinder("Root/MainGroup/Commodity3")]
        public Transform commodity3;

        [ComponentBinder("Root/TopGroup/TimerGroup/TimerText")]
        public TextMeshProUGUI timerText;   
        
      
        public List<QuestPayItemView> questPayItemViews;

        public QuestPayPopup(string address)
            : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            questPayItemViews = new List<QuestPayItemView>();
            questPayItemViews.Add(AddChild<QuestPayItemView>(commodity1));
            questPayItemViews.Add(AddChild<QuestPayItemView>(commodity2));
            questPayItemViews.Add(AddChild<QuestPayItemView>(commodity3));

            contentDesignSize = ViewResolution.designSize;
        }
    }

    public class QuestPayPopupViewController : ViewController<QuestPayPopup>
    {
        private SGetNewbieQuestPaymentItems _paymentItems;

        private bool _isPurcahsed = false;
        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            _paymentItems = inExtraAsyncData as SGetNewbieQuestPaymentItems;
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            for (var i = 0; i < _paymentItems.Items.Count; i++)
            {
                view.questPayItemViews[i]
                    .SetUpItemView(_paymentItems.Items[i], i, OnBuyButtonClicked, OnPurchaseBenefitsClicked);
            }

            EnableUpdate(2);
            
            var source = GetTriggerSource();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventNewbiequestBoostPopup, ("Source:",source));
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventCommonPaymentComplete>(OnQuestBoostPurchaseConsumeEnd);
        }

        protected void OnQuestBoostPurchaseConsumeEnd(EventCommonPaymentComplete evt)
        {
            if (evt.shopType == ShopType.NewBieQuestBoost)
            {
                _isPurcahsed = true;
                view.Close();
            }
        }

        public async void OnPurchaseBenefitsClicked(int itemIndex)
        {
            if (_paymentItems.Items.Count > itemIndex)
            {
                var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
                purchaseBenefitsView.SetUpBenefitsView(_paymentItems.Items[itemIndex].SubItemList);
            }
        }

        public void OnBuyButtonClicked(int itemIndex)
        {
            var shopItemConfig = _paymentItems.Items[itemIndex];

            Client.Get<IapController>().BuyProduct(shopItemConfig);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventNewbiequestBoostBuy, ("Prize:",shopItemConfig.Price.ToString()));

        }

        public override void Update()
        {
            var countDown = Client.Get<NewBieQuestController>().GetQuestCountDown();
            if (countDown <= 0)
            {
                view.Close();
                return;
            }

            view.timerText.text = XUtility.GetTimeText(countDown, true);
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            
            if (!_isPurcahsed)
            {
                if (AdController.Instance.ShouldShowRV(eAdReward.NewbieQuestBoost, false))
                {
                    PopupStack.ShowPopupNoWait<QuestBuffSpeedUpPopup>();
                }
            }
        }
    }
}