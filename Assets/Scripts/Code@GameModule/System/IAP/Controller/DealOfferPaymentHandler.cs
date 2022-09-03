// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/03/19:00
// Ver : 1.0.0
// Description : DealOfferPaymentHandler.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;


namespace GameModule
{
    public class DealOfferPaymentHandler:PaymentHandler
    {

        public DealOfferPaymentHandler()
            : base("deal")
        {
            
        }

        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs,
            VerifyExtraInfo verifyExtraInfo)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventSpecialOfferPurchaseSuccess,
                ("paymentId", verifyExtraInfo.Item.PaymentId.ToString()),
                ("price", verifyExtraInfo.Item.Price.ToString()),
                ("productType", verifyExtraInfo.Item.ProductType));
            
            var collectPopUp = await PopupStack.ShowPopup<DealOfferCollectPopup>();
 
            collectPopUp.viewController.SetUpViewContent(verifyExtraInfo, (callback) =>
            {
                Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                    {
                        collectPopUp.SubscribeCloseAction(() =>
                        {
                            collectCallback?.Invoke();
                            EventBus.Dispatch(new EventDealOfferConsumeComplete());
                        });
                        callback(success, fulfillExtraInfo);
                    });
            });
        }
    }
}