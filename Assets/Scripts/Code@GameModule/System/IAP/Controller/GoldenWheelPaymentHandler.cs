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
using UnityEditor;

namespace GameModule
{
    public class GoldenWheelPaymentHandler:PaymentHandler
    {
        public GoldenWheelPaymentHandler()
        :base(ShopType.GoldenWheel)
        {
            
        }
        
        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            if (purchaseCallbackArgs.isReplenishmentOrder)
            { 
                var collectPopUp = await PopupStack.ShowPopup<TimeBonusWheelBonusPopup>();
                collectPopUp.viewController.SetGoldenWheelPurchaseContent(verifyExtraInfo, (callback) =>
                {
                    Client.Get<IapController>()
                        .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                        {
                            callback(new FulFillCallbackArgs()
                            {
                                isSuccess = success,
                                fulfillExtraInfo = fulfillExtraInfo,
                                fullFillFxEndCallback = collectCallback
                            });
                        });
                }, true);
            }
            else
            {
                EventBus.Dispatch(new EventTimeBonusGoldenWheelPurchaseSucceed(verifyExtraInfo, (callback) =>
                {
                    Client.Get<IapController>()
                        .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                        {
                      
                            callback(new FulFillCallbackArgs()
                            {
                                isSuccess = success,
                                fulfillExtraInfo = fulfillExtraInfo,
                                fullFillFxEndCallback = collectCallback
                            });
                        });
                }));
            }
        }
    }
}