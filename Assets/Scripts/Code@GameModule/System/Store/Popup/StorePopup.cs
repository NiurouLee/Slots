// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/02/20:34
// Ver : 1.0.0
// Description : StorePopUp.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace GameModule
{
    [AssetAddress("UIStoreH", "UIStoreV", "UIStoreH_Pad")]
    public class StorePopup : Popup<StorePopupViewController>
    {
        [ComponentBinder("Root/MainGroup/ScrollView")]
        public Transform storeCommodityViewTransform;

        [ComponentBinder("Root/BottomGroup/GiftBoxGroup")]
        public Transform giftBoxViewTransform;

        [ComponentBinder("Root/BottomGroup/VIPGroup")]
        public Transform vipBenefitsViewTransform;

        [ComponentBinder("CoinButton")] public Button coinButton;

        [ComponentBinder("DiamondButton")] public Button diamondButton;

        [ComponentBinder("BoostButton")] public Button boostButton;

        [ComponentBinder("GiftBoxInformationButton")] public Button giftInfoButton;

        [ComponentBinder("HighLightMask")] public Image highLightMask;

        [ComponentBinder("UICommonRewardPreview")]
        public Transform commonRewardPreview;

        [ComponentBinder("Root/TopGroup/VIPGroup/ValueText")]
        public TextMeshProUGUI vipBenefitValueText;

        [ComponentBinder("Root/TopGroup/VIPGroup/CurrentLevelIcon")]
        public Image vipLevelIcon;

        [ComponentBinder("Root/TopGroup/ADSGroup")]
        public Transform adsGroup;

        [ComponentBinder("Root")]
        public RectTransform root;

        public StoreCommodityView storeCommodityView;

        public GiftBoxView giftBoxView;
        public PurchaseBenefitsView benefitsView;
        public StorePopup(string address)
            : base(address)
        {
        }

        protected override void SetUpExtraView()
        {
            CreateCommodityView();

            giftBoxView = AddChild<GiftBoxView>(giftBoxViewTransform);
            benefitsView = AddChild<PurchaseBenefitsView>(commonRewardPreview);
            benefitsView.Hide();
        }

        protected void CreateCommodityView()
        {
            storeCommodityView = AddChild<StoreCommodityView>(storeCommodityViewTransform);
        }

        public override string GetOpenAudioName()
        {
            return "General_OpenBank";
        }
    }

    public class StorePopupViewController : ViewController<StorePopup>
    {
        public Action highlightMaskClickAction;
        public SGetShop shopInfo;

        public string openSource;
        public bool purchasedInStore = false;

        public StoreCommodityView.CommodityPageType openPageType = StoreCommodityView.CommodityPageType.Coin;

        public override void BindingView(View inView, object inExtraData, object inAsyncExtraData = null)
        {
            base.BindingView(inView, inExtraData, inAsyncExtraData);

            shopInfo = inAsyncExtraData as SGetShop;

            if (inExtraData != null)
            {
                var popupArgs = (PopupArgs)inExtraData;
                if (popupArgs?.extraArgs != null)
                {
                    openPageType = (StoreCommodityView.CommodityPageType)popupArgs.extraArgs;
                }

                var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

                string sceneType = "Lobby";

                if (machineScene != null)
                {
                    sceneType = Client.Get<MachineLogicController>().LastGameId;
                }

                openSource = popupArgs.source;

                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStoreOpen, ("source", popupArgs.source), ("scene", sceneType));
            }
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventPaymentFinish>(RefreshStoreUI);
            SubscribeEvent<EventReceiveUserCoupons>(OnEventReceiveUserCoupons);
            SubscribeEvent<EventPurchasedInStore>(OnPurchasedInStore);
        }

        private void OnEventReceiveUserCoupons(EventReceiveUserCoupons obj)
        {
            RefreshStoreUI(new EventPaymentFinish());
        }

        protected void OnPurchasedInStore(EventPurchasedInStore evt)
        {
            purchasedInStore = true;
        }

        protected async void RefreshStoreUI(EventPaymentFinish evt)
        {
            shopInfo = await Client.Get<IapController>().GetPaymentHandler<StorePaymentHandler>().GetShopInfo();
          
            if (shopInfo != null && view.transform != null)
            {
                view.storeCommodityView.commodityBonusView.viewController.SetUp(shopInfo.StoreBonus);

                if (shopInfo.StoreBonus.CountdownTime > 0)
                {
                    view.storeCommodityView.commodityBonusView.transform.SetSiblingIndex(6);
                }

                view.storeCommodityView.UpdateCommodityView(shopInfo.ItemList);
                view.giftBoxView.viewController.SetUpView(shopInfo.GiftBox);
                view.vipBenefitValueText.text = (shopInfo.VipCoinAddition * 100) + "%";

                InitAdContent();
                RefreshVipIconNameInfo();
            }
        }

        public void RefreshVipIconNameInfo()
        {
            var vipAtlas = AssetHelper.GetResidentAsset<SpriteAtlas>("CommonUIAtlas");
            if (vipAtlas)
            {
                var vipLevel = Client.Get<VipController>().GetVipLevel();

                view.vipLevelIcon.sprite = vipAtlas.GetSprite($"UI_VIP_icon_{vipLevel}");
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            if (view.GetAssetAddressName().Contains("V"))
            {
                if (ViewResolution.referenceResolutionLandscape.x / ViewResolution.referenceResolutionLandscape.y >
                    2.0f)
                {
                    //                    view.root.anchoredPosition = new Vector2(0, -100);
                    view.root.offsetMax = new Vector2(0, -40);
                }
            }

            if (shopInfo != null && shopInfo.ItemList.Count > 0)
            {
                view.storeCommodityView.SetUpCommodityView(shopInfo.ItemList);

                view.storeCommodityView.commodityBonusView.viewController.SetUp(shopInfo.StoreBonus);

                if (shopInfo.StoreBonus.CountdownTime > 0)
                {
                    view.storeCommodityView.commodityBonusView.transform.SetSiblingIndex(6);
                }

                view.giftBoxView.viewController.SetUpView(shopInfo.GiftBox);

                view.vipBenefitValueText.text = (shopInfo.VipCoinAddition * 100) + "%";

                InitAdContent();

            }

            SetUpNavigationButtonState();

            BindViewEvent();
            
            RefreshVipIconNameInfo();

            //避免玩家在，支付完成之后，没有正常领取到GiftBox，在这里做一个检查
            if (shopInfo != null && shopInfo.GiftBox.ProgressNow >= shopInfo.GiftBox.ProgressMax)
            {
                CheckAndClaimGiftBox();
            }
            else if (openPageType != StoreCommodityView.CommodityPageType.Coin)
            {
                WaitNFrame(1, () =>
                {
                    view.storeCommodityView.TurnToPage(openPageType, false);
                });
            }
        }

        protected void InitAdContent()
        {

            var adContentCount = shopInfo.AdContent.Count;

            if (adContentCount > 0)
            {
                int index = (int)Random.Range(0, adContentCount);

                var adContent = shopInfo.AdContent[index];
                var childCount = view.adsGroup.childCount;
                for (var i = 0; i < childCount; i++)
                {
                    var child = view.adsGroup.GetChild(i);
                    child.gameObject.SetActive(child.name == "Content" + adContent);
                }
            }
        }

        protected void BindViewEvent()
        {
            view.storeCommodityView.OnPageChangeEvent += OnPageChanged;

            var commodityChildCount = view.storeCommodityView.GetChildViewCount();

            for (var i = 0; i < commodityChildCount; i++)
            {
                var commodityView = view.storeCommodityView.GetChildView(i);

                if (commodityView is CommodityShopItemView shopItemView)
                {
                    shopItemView.onContentButtonClick += OnShopItemAttachContentItemClicked;
                    shopItemView.onPrizeButtonClick += OnBuyButtonClicked;
                }

                if (commodityView is CommodityBoostView boostView)
                {
                    boostView.onBoostInfoClicked += OnBoostInfoClicked;
                }
            }

            view.boostButton.onClick.AddListener(OnNavBoostButtonClicked);
            view.diamondButton.onClick.AddListener(OnNavDiamondButtonClicked);
            view.coinButton.onClick.AddListener(OnNavCoinButtonClicked);
            view.giftInfoButton.onClick.AddListener(OnGiftBoxClicked);

            var pointerEventCustomHandler = view.highLightMask.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnHighLightMaskClicked);
        }

        protected async void CheckAndClaimGiftBox()
        {
            var popup = await PopupStack.ShowPopup<GiftBoxRewardPopup>();
            popup.SetUpRewardUI((long)shopInfo.GiftBox.AccumulateCoins);
        }

        protected async void OnGiftBoxClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStoreGiftBoxClick);
            await PopupStack.ShowPopup<GitBoxFaqPopup>();
        }

        protected void OnHighLightMaskClicked(PointerEventData pointerEventData)
        {
            highlightMaskClickAction?.Invoke();
            highlightMaskClickAction = null;
        }

        protected void OnShopItemAttachContentItemClicked(CommodityShopItemView itemView)
        {
            highlightMaskClickAction = () =>
            {
                view.benefitsView.Hide();
                view.highLightMask.gameObject.SetActive(false);
            };

            if (!view.benefitsView.IsActive())
            {
                view.benefitsView.SetUpBenefitsView(itemView.shopItemConfig.SubItemList);

                view.benefitsView.Show();
                view.highLightMask.gameObject.SetActive(true);

                if (itemView is CommodityBoostView)
                {
                    var boosView = itemView as CommodityBoostView;
                    boosView.HighLightView(false);
                }
            }
            else
            {
                view.benefitsView.Hide();
                view.highLightMask.gameObject.SetActive(false);
                highlightMaskClickAction = null;
            }
        }

        protected void OnBoostInfoClicked(CommodityBoostView itemView)
        {
            highlightMaskClickAction = () =>
            {
                view.highLightMask.gameObject.SetActive(false);
                itemView.HighLightView(false);
            };

            if (!itemView.IsHighLight())
            {
                view.highLightMask.gameObject.SetActive(true);
                itemView.HighLightView(true);
            }
            else
            {
                view.highLightMask.gameObject.SetActive(false);
                itemView.HighLightView(false);
                highlightMaskClickAction = null;
            }
        }

        public void OnBuyButtonClicked(CommodityShopItemView itemView)
        {
            if (highlightMaskClickAction != null)
            {
                highlightMaskClickAction.Invoke();
            }

            var shopItemConfig = itemView.shopItemConfig;
            Client.Get<IapController>().BuyProduct(shopItemConfig);

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStorePurchase,
                ("paymentId", shopItemConfig.PaymentId.ToString()),
                ("price", shopItemConfig.Price.ToString()),
                ("productType", shopItemConfig.ProductType));

        }

        public void OnNavCoinButtonClicked()
        {
            SoundController.PlayButtonClick();
            view.storeCommodityView.TurnToPage(StoreCommodityView.CommodityPageType.Coin);
        }

        public void OnNavDiamondButtonClicked()
        {
            SoundController.PlayButtonClick();
            view.storeCommodityView.TurnToPage(StoreCommodityView.CommodityPageType.Diamond);
        }

        public void OnNavBoostButtonClicked()
        {
            SoundController.PlayButtonClick();
            view.storeCommodityView.TurnToPage(StoreCommodityView.CommodityPageType.Boost);
        }

        public void OnPageChanged(StoreCommodityView.CommodityPageType commodityPageType)
        {
            EnableNavigationButton(view.coinButton, commodityPageType != StoreCommodityView.CommodityPageType.Coin);
            EnableNavigationButton(view.diamondButton,
                commodityPageType != StoreCommodityView.CommodityPageType.Diamond);
            EnableNavigationButton(view.boostButton, commodityPageType != StoreCommodityView.CommodityPageType.Boost);
        }

        public void SetUpNavigationButtonState()
        {
            EnableNavigationButton(view.coinButton, false);
            EnableNavigationButton(view.diamondButton, true);
            EnableNavigationButton(view.boostButton, true);
        }

        public void EnableNavigationButton(Button button, bool enable)
        {
            button.transform.Find("Icon").gameObject.SetActive(enable);
            button.transform.Find("DisableIcon").gameObject.SetActive(!enable);
            button.transform.Find("DisableLabel").gameObject.SetActive(!enable);
            button.transform.Find("Label").gameObject.SetActive(enable);
            button.interactable = enable;
        }
  
        public override void OnViewDestroy()
        {
            base.OnViewDestroy();

            EventBus.Dispatch(new EventStoreClose(openSource, purchasedInStore));
        }
    }
}