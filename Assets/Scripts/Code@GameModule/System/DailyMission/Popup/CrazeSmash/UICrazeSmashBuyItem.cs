using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.UI;

namespace GameModule
{
    public class UICrazeSmashBuyItem : View
    {
        [ComponentBinder("QuantityText")]
        public Text textCount;

        [ComponentBinder("BuyButton/BuyText")]
        public Text textPrice;

        [ComponentBinder("BuyButton")]
        public Button buttonBuy;

        [ComponentBinder("ExtraContentsButton")]
        public Button buttonBenefit;

        public void Set(uint count, ShopItemConfig config)
        {
            if (config == null) { return; }

            if (textCount != null) { textCount.text = $"+{count}"; }
            if (textPrice != null) { textPrice.text = $"${config.Price}"; }

            buttonBuy.onClick.RemoveAllListeners();
            buttonBuy.onClick.AddListener(() =>
            {
                var controller = Client.Get<CrazeSmashController>();
                var type = controller.playGoldGame ? "2" : "1";
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashBuyEnter, ("type", $"{type}"), ("count", $"{count}"));

                Client.Get<IapController>().BuyProduct(config);
                PopupStack.ClosePopup<UICrazeSmashBuy>();
            });

            buttonBenefit.onClick.RemoveAllListeners();
            buttonBenefit.onClick.AddListener(async () =>
            {
                var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
                purchaseBenefitsView.SetUpBenefitsView(config.SubItemList);
            });
        }
    }
}