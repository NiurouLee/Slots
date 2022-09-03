// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/14/15:44
// Ver : 1.0.0
// Description : PiggyPaymentHandler.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;


namespace GameModule
{
    public class PiggyPaymentHandler:PaymentHandler
    {
        public PiggyPaymentHandler()
        :base(ShopType.PiggyBank)
        {
            
        }
        
        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            var collectPopUp = await PopupStack.ShowPopup<PiggyBankRewardPopup>();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPiggyBankPurchaseSuccess,
                ("paymentId", verifyExtraInfo.Item.PaymentId.ToString()),
                ("price", verifyExtraInfo.Item.Price.ToString()),
                ("productType", verifyExtraInfo.Item.ProductType),
                ("isFirst",Client.Get<PiggyBankController>().FirstBonus>0?"true":"false"));
            

            collectPopUp.viewController.SetUpViewContent(verifyExtraInfo,(callback) =>
            {
                Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                    {
                        collectPopUp.SubscribeCloseAction(() =>
                        {
                            collectCallback?.Invoke();
                            EventBus.Dispatch(new EventPiggyConsumeComplete());
                        });
                        callback(success, fulfillExtraInfo);
                    });
            });
        }
    }
}