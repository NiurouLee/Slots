//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-09 18:12
//  Ver : 1.0.0
//  Description : SeasonPassBuyLevelPaymentHandler.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class SeasonQuestPassPaymentHandler: PaymentHandler
    {
        public SeasonQuestPassPaymentHandler():base(ShopType.SeasonQuest)
        {

        }
        public override void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            if (PopupStack.GetPopup<SeasonQuestPassPaymentPopup>() != null)
            {
                var paymentPopup = PopupStack.GetPopup<SeasonQuestPassPaymentPopup>();
                paymentPopup.viewController.OnPaymentSuccess(purchaseCallbackArgs, verifyExtraInfo);
            }
            else
            {
                //补单操作
                if (purchaseCallbackArgs.isReplenishmentOrder)
                {
                    Client.Get<IapController>()
                        .FulfillPayment(purchaseCallbackArgs,verifyExtraInfo,
                            (success, fulfillExtraInfo, collectCallback) =>
                            {
                                collectCallback.Invoke();
                                Client.Get<SeasonQuestController>().RefreshSeasonQuestData();
                            });
                }
            }
            
        }
    }
}