using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidBuyTickets")]
    public class TreasureRaidBuyTicketPopup : Popup<TreasureRaidBuyTicketPopupController>
    {
        [ComponentBinder("Root/BottomGroup/PriceButton")]
        public Button buyBtn;

        [ComponentBinder("Root/BottomGroup/DetailButton")] 
        public Button benifitBtn;
        
        [ComponentBinder("Root/BGGroup/BG/MainGroup/CountText")] 
        public Text ticketCountText;

        [ComponentBinder("Root/BottomGroup/PriceButton/PriceText")] 
        public Text priceText;
        
        [ComponentBinder("Root/BGGroup/BG/MainGroup/CountGroup")] 
        public Transform countGroup;

        public TreasureRaidBuyTicketPopup(string address) : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
        
        public void SetBtnState(bool interactable)
        {
            buyBtn.interactable = interactable;
            benifitBtn.interactable = interactable;
        }

        protected override void OnCloseClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidPurchaseWindow, ("OperationId", "1"));
            base.OnCloseClicked();
        }
    }

    public class TreasureRaidBuyTicketPopupController : ViewController<TreasureRaidBuyTicketPopup>
    {
        private SMonopolyTicketPaymentInfo paymentInfo;

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
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidPurchaseWindow, ("OperationId", "2"));
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
            view.buyBtn.onClick.AddListener(OnBuyBtnClicked);
            view.benifitBtn.onClick.AddListener(OnBtnExtraClicked);
            view.countGroup.gameObject.SetActive(false);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            var activityTicket = XItemUtility.GetItem(paymentInfo.PayItem.SubItemList, Item.Types.Type.MonopolyActivityTicket);
            view.ticketCountText.SetText($"+{activityTicket.MonopolyActivityTicket.Amount}");
            view.priceText.SetText($"${paymentInfo.PayItem.Price}");
        }

        private void OnBuyBtnClicked()
        {
            view.SetBtnState(false);
            Client.Get<IapController>().BuyProduct(paymentInfo.PayItem);
            view.SetBtnState(true);
        }

        private async void OnBtnExtraClicked()
        {
            view.SetBtnState(false);
            ShopItemConfig shopItemConfig = paymentInfo.PayItem;
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.MonopolyActivityTicket);
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList, skipList);
            view.SetBtnState(true);
        }
    }
}