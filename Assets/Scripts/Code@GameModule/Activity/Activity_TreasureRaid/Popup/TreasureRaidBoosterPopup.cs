using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidBoosterPortalItemView : TreasureRaidBoosterItemView
    {
        public override void RefreshUI()
        {
            base.RefreshUI();

            var buffItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.MonopolyActivityPortal);
            var amount = buffItem.MonopolyActivityPortal.Amount;
            currentBuff.SetText($"+{amount}");
            var onSale = buffItem.MonopolyActivityPortal.AmountWhenNoAddition != amount;
            
            mainOriginalGroup.gameObject.SetActive(onSale);
            mainSaleTag.gameObject.SetActive(onSale);

            if (onSale)
            {
                var more = (amount - buffItem.MonopolyActivityPortal.AmountWhenNoAddition) / buffItem.MonopolyActivityPortal.AmountWhenNoAddition * 100;
                mainSaleText.SetText($"{more}%");
                originalBuff.SetText($"+{buffItem.MonopolyActivityPortal.AmountWhenNoAddition}");
            }
        }
        
        public override void PlayAni()
        {
            base.PlayAni();
            var buffItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.MonopolyActivityPortal);
            var onSale = buffItem.MonopolyActivityPortal.AmountWhenNoAddition != buffItem.MonopolyActivityPortal.Amount;
            var playAniName = onSale ? "Show" : "idle";
            XUtility.PlayAnimation(mainCellAni, playAniName);
        }
    }

    
    public class TreasureRaidBoosterWeaponItemView : TreasureRaidBoosterItemView
    {
        public override void RefreshUI()
        {
            base.RefreshUI();

            var buffItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.MonopolyActivityBuffMoreDamage);
            var countDown = buffItem.MonopolyActivityBuffMoreDamage.Amount;
            currentBuff.SetText($"{countDown} MINS");
            var onSale = buffItem.MonopolyActivityBuffMoreDamage.AmountWhenNoAddition != countDown;
            
            mainOriginalGroup.gameObject.SetActive(onSale);
            mainSaleTag.gameObject.SetActive(onSale);

            if (onSale)
            {
                var more = (countDown - buffItem.MonopolyActivityBuffMoreDamage.AmountWhenNoAddition) / buffItem.MonopolyActivityBuffMoreDamage.AmountWhenNoAddition * 100;
                mainSaleText.SetText($"{more}%");
                originalBuff.SetText($"{buffItem.MonopolyActivityBuffMoreDamage.AmountWhenNoAddition} MINS");
            }
        }
        
        public override void PlayAni()
        {
            base.PlayAni();
            var buffItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.MonopolyActivityBuffMoreDamage);
            var onSale = buffItem.MonopolyActivityBuffMoreDamage.AmountWhenNoAddition != buffItem.MonopolyActivityBuffMoreDamage.Amount;
            var playAniName = onSale ? "Show" : "idle";
            XUtility.PlayAnimation(mainCellAni, playAniName);
        }
    }
    
    public class TreasureRaidBoosterEnergyItemView : TreasureRaidBoosterItemView
    {
        public override void RefreshUI()
        {
            base.RefreshUI();

            var buffItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.MonopolyActivityBuffMoreTicket);
            var countDown = buffItem.MonopolyActivityBuffMoreTicket.Amount;
            currentBuff.SetText($"{countDown} MINS");

            var onSale = buffItem.MonopolyActivityBuffMoreTicket.AmountWhenNoAddition != countDown;
            
            mainOriginalGroup.gameObject.SetActive(onSale);
            mainSaleTag.gameObject.SetActive(onSale);

            if (onSale)
            {
                var more = (countDown - buffItem.MonopolyActivityBuffMoreTicket.AmountWhenNoAddition) / buffItem.MonopolyActivityBuffMoreTicket.AmountWhenNoAddition * 100;
                mainSaleText.SetText($"{more}%");
                originalBuff.SetText($"{buffItem.MonopolyActivityBuffMoreTicket.AmountWhenNoAddition} MINS");
            }
        }

        public override void PlayAni()
        {
            base.PlayAni();
            var buffItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.MonopolyActivityBuffMoreTicket);
            var onSale = buffItem.MonopolyActivityBuffMoreTicket.AmountWhenNoAddition != buffItem.MonopolyActivityBuffMoreTicket.Amount;
            var playAniName = onSale ? "Show" : "idle";
            XUtility.PlayAnimation(mainCellAni, playAniName);
        }
    }
    public class TreasureRaidBoosterItemView : View
    {
        [ComponentBinder("CommoditiesGroup/MainCell/InformationButton")]
        protected Button informationBtn;

        [ComponentBinder("BubbleGroup")]
        protected Transform bubbleGroup;

        public CommonTextBubbleView bubbleView;
        
        [ComponentBinder("CommoditiesGroup/MainCell")]
        protected Animator mainCellAni;
        
        [ComponentBinder("CommoditiesGroup/MainCell/OriginalContentGroup")]
        protected Transform mainOriginalGroup;

        [ComponentBinder("CommoditiesGroup/MainCell/SaleTag")]
        protected Transform mainSaleTag;
        [ComponentBinder("CommoditiesGroup/MainCell/SaleTag/SaleText")]
        protected Text mainSaleText;
        
        [ComponentBinder("CommoditiesGroup/MainCell/OriginalContentGroup/BuffQuantityText")]
        protected Text originalBuff;
        [ComponentBinder("CommoditiesGroup/MainCell/CurrentContentGroup/BuffQuantityText")]
        protected Text currentBuff;
        
        [ComponentBinder("CommoditiesGroup/ExtraCell1")]
        protected Animator extraCellAni1;
        
        [ComponentBinder("CommoditiesGroup/ExtraCell1/SaleContentGroup")]
        protected Transform coinOriginalGroup;

        [ComponentBinder("CommoditiesGroup/ExtraCell1/SaleTag")]
        protected Transform coinSaleTag;
        [ComponentBinder("CommoditiesGroup/ExtraCell1/SaleTag/SaleText")]
        protected Text coinSaleText;
        
        [ComponentBinder("CommoditiesGroup/ExtraCell1/SaleContentGroup/BuffQuantityText")]
        protected Text originalCoins;
        [ComponentBinder("CommoditiesGroup/ExtraCell1/CurrentContentGroup/BuffQuantityText")]
        protected Text currentCoins;
        
        [ComponentBinder("CommoditiesGroup/ExtraCell1")]
        protected Animator extraCellAni2;

        [ComponentBinder("CommoditiesGroup/ExtraCell2/SaleContentGroup01")]
        protected Transform ticketOriginalGroup;

        [ComponentBinder("CommoditiesGroup/ExtraCell2/SaleContentGroup01/BuffQuantityText")]
        protected Text originalTickets;
        [ComponentBinder("CommoditiesGroup/ExtraCell2/CurrentContentGroup/BuffQuantityText")]
        protected Text currentTickets;
        
        //促销相关
        [ComponentBinder("BottomGroup/SalePriceButton")]
        private Button saleBtn;
        [ComponentBinder("BottomGroup/SalePriceButton/OriginalGroup/OriginalPriceText")]
        private Text originalPrice;
        [ComponentBinder("BottomGroup/SalePriceButton/CurrentGroup/CurrentPriceText")]
        private Text currentPrice;

        [ComponentBinder("BottomGroup/PriceButton")]
        private Button noSaleBtn;
        [ComponentBinder("BottomGroup/PriceButton/CurrentGroup/CurrentPriceText")]
        private Text noSalePrice;
        
        
        [ComponentBinder("BottomGroup/PurchaseButton")]
        private Button benefitBtn;

        protected ShopItemConfig config;

        public void UpdateContent(ShopItemConfig inConfig)
        {
            config = inConfig;
            
            saleBtn.onClick.AddListener(OnBuyBtnClicked);
            noSaleBtn.onClick.AddListener(OnBuyBtnClicked);
            benefitBtn.onClick.AddListener(OnBtnExtraClicked);
            informationBtn.onClick.AddListener(OnBtnInformationClicked);
            bubbleView = AddChild<CommonTextBubbleView>(bubbleGroup);
        }

        private void OnBtnInformationClicked()
        {
            bubbleView.ShowBubble(3);
        }

        public virtual void RefreshUI()
        {
            var onSale = config.OldPrice != 0;
            saleBtn.gameObject.SetActive(onSale);
            
            noSaleBtn.gameObject.SetActive(!onSale);

            coinOriginalGroup.gameObject.SetActive(false);
            ticketOriginalGroup.gameObject.SetActive(false);

            coinSaleTag.gameObject.SetActive(false);

            var coinItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.Coin);
            
            var ticketItem = XItemUtility.GetItem(config.SubItemList, Item.Types.Type.MonopolyActivityTicket);
            currentTickets.SetText($"+{ticketItem.MonopolyActivityTicket.Amount}");

            if (onSale)
            {
                originalPrice.SetText($"${config.OldPrice}");
                currentPrice.SetText($"${config.Price}");
                var coinSale = coinItem.ShopCoin != null && coinItem.ShopCoin.AdditionAmount != coinItem.ShopCoin.Amount;
                coinOriginalGroup.gameObject.SetActive(coinSale);
                coinSaleTag.gameObject.SetActive(coinSale);
                if (coinSale)
                {
                    originalCoins.SetText(coinItem.ShopCoin.Amount.GetAbbreviationFormat());
                    currentCoins.SetText(coinItem.ShopCoin.AdditionAmount.GetAbbreviationFormat());
                    coinSaleText.SetText($"{Mathf.Floor((coinItem.ShopCoin.AdditionAmount - coinItem.ShopCoin.Amount) / coinItem.ShopCoin.Amount * 100)}%");
                }
                else
                {
                    currentCoins.SetText($"{coinItem.Coin.Amount.GetAbbreviationFormat()}");
                }
            }
            else
            {
                currentCoins.SetText($"{coinItem.Coin.Amount.GetAbbreviationFormat()}");
                noSalePrice.SetText($"${config.Price}");
            }
        }
        private void OnBuyBtnClicked()
        {
            var parentView = GetParentView() as TreasureRaidBoosterPopup;
            if (parentView != null)
            {
                parentView.currentConfig = config;
            }
            Client.Get<IapController>().BuyProduct(config);
        }

        public virtual void PlayAni()
        {
            XUtility.PlayAnimation(extraCellAni1, "idle");
            XUtility.PlayAnimation(extraCellAni2, "idle");
        }

        private async void OnBtnExtraClicked()
        {
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.MonopolyActivityTicket);
            skipList.Add(Item.Types.Type.Coin);
            purchaseBenefitsView.SetUpBenefitsView(config.SubItemList, skipList);
        }
    }

    [AssetAddress("UITreasureRaidCommodities")]
    public class TreasureRaidBoosterPopup : Popup<TreasureRaidBoosterPopupController>
    {
        [ComponentBinder("Root/MainGroup/EnergyBundle")]
        public Transform energyItem;
        
        [ComponentBinder("Root/MainGroup/WeaponBundle")]
        public Transform weaponItem;
        
        [ComponentBinder("Root/MainGroup/ToAnyWhereBundle")]
        public Transform portalItem;
        
        [ComponentBinder("Root/TopGroup/TitleSaleImage")]
        public Transform saleImage;
        
        [ComponentBinder("Root/TopGroup/TitleImage")]
        public Transform titleImage;

        public ShopItemConfig currentConfig;

        public TreasureRaidBoosterPopup(string address) : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdaptScaleTransform(transform.Find("Root"), ViewResolution.designSize);
        }

        public override void AdaptScaleTransform(Transform transformToScale, Vector2 preferSize)
        {
            var viewSize = ViewResolution.referenceResolutionLandscape;
            if (viewSize.x < preferSize.x)
            {
                var scale = viewSize.x / preferSize.x;
                transformToScale.localScale =  new Vector3(scale, scale, scale);
            }
        }
    }

    public class TreasureRaidBoosterPopupController : ViewController<TreasureRaidBoosterPopup>
    {
        private SMonopolyTicketPaymentInfo paymentInfo;

        private List<TreasureRaidBoosterItemView> _itemViews;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidPurchaseFinish>(PurchaseSuccess);
            SubscribeEvent<EventActivityExpire>(OnActivityExpired);
        }

        private void OnActivityExpired(EventActivityExpire obj)
        {
            view.Close();
        }

        private void PurchaseSuccess(EventTreasureRaidPurchaseFinish obj)
        {
            if (view.currentConfig != null)
            {
                var item1 = XItemUtility.GetItem(view.currentConfig.SubItemList,
                    Item.Types.Type.MonopolyActivityPortal);
                var item2 = XItemUtility.GetItem(view.currentConfig.SubItemList,
                    Item.Types.Type.MonopolyActivityBuffMoreDamage);
                var item3 = XItemUtility.GetItem(view.currentConfig.SubItemList,
                    Item.Types.Type.MonopolyActivityBuffMoreTicket);
                if (item1 != null)
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidBoosterBuy3);
                }
                else if (item2 != null)
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidBoosterBuy2);
                }
                else if (item3 != null)
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidBoosterBuy1);
                }
            }
            view.Close();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            paymentInfo = inExtraAsyncData as SMonopolyTicketPaymentInfo;
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _itemViews = new List<TreasureRaidBoosterItemView>();
            if (paymentInfo.BoosterPayItems != null && paymentInfo.BoosterPayItems.Count > 0)
            {
                foreach (var item in paymentInfo.BoosterPayItems)
                {
                    switch (item.ProductType)
                    {
                        case "doubleticket":
                            var energyItemView = view.AddChild<TreasureRaidBoosterEnergyItemView>(view.energyItem);
                            energyItemView.UpdateContent(item);
                            _itemViews.Add(energyItemView);
                            break;
                        case "doubledamage":
                            var weaponItemView = view.AddChild<TreasureRaidBoosterWeaponItemView>(view.weaponItem);
                            weaponItemView.UpdateContent(item);
                            _itemViews.Add(weaponItemView);
                            break;
                        case "flash":
                            var portalItemView = view.AddChild<TreasureRaidBoosterPortalItemView>(view.portalItem);
                            portalItemView.UpdateContent(item);
                            _itemViews.Add(portalItemView);
                            break;
                    }
                }

                foreach (var itemView in _itemViews)
                {
                    itemView.RefreshUI();
                }

                var onSale = paymentInfo.BoosterPayItems[0].OldPrice != 0;
                view.saleImage.gameObject.SetActive(onSale);
                view.titleImage.gameObject.SetActive(!onSale);
            }
        }
        
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            foreach (var itemView in _itemViews)
            {
                itemView.PlayAni();
            }
        }
    }
}