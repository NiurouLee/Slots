using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class Activity_ValentinesDay_PaymentHandler : PaymentHandler
    {
        public Activity_ValentinesDay_PaymentHandler() : base(ShopType.Activity_Valentine2022) { }

        public override void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                    {
                        EventBus.Dispatch(new Event_Activity_Valentine2022_PurchaseFinish());
                    });
        }
    }
}