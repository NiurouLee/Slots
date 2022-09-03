using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class TreasureRaidPaymentHandler : PaymentHandler
    {
        public TreasureRaidPaymentHandler() : base(ShopType.TreasureRaidDeal)
        {
            
        }

        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            var collectPopUp = await PopupStack.ShowPopup<CommonPurchaseSuccessfulPopup>();
            
            collectPopUp.viewController.SetUpViewContent(verifyExtraInfo, (callback) =>
            {
                Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                    {
                        collectPopUp.SubscribeCloseAction( async () =>
                        {
                            collectCallback?.Invoke();
                            EventBus.Dispatch(new EventTreasureRaidPurchaseFinish());
                        });
                        callback(success, fulfillExtraInfo);
                    }, true, 0);
            });
        }
    }
}