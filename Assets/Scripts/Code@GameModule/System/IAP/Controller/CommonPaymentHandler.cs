// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/28/19:32
// Ver : 1.0.0
// Description : CommonPaymentHandler.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class CommonPaymentHandler:PaymentHandler
    {
        public CommonPaymentHandler(string shopType)
            : base(shopType)
        {
            
        }

        public virtual void LogBiEvent(VerifyExtraInfo verifyExtraInfo)
        {
            // BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventSpecialOfferPurchaseSuccess,
            //     ("paymentId", verifyExtraInfo.Item.PaymentId.ToString()),
            //     ("price", verifyExtraInfo.Item.Price.ToString()),
            //     ("productType", verifyExtraInfo.Item.ProductType));

        }
        
        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs,
            VerifyExtraInfo verifyExtraInfo)
        {
        
            var collectPopUp = await PopupStack.ShowPopup<CommonPurchaseSuccessfulPopup>();

            if (verifyExtraInfo.Item.ShopType == ShopType.SlotDeal)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                    .GameEventPiggyBonusGiftboxBuySuccessful);
            }
            else if (verifyExtraInfo.Item.ShopType == ShopType.NewBieQuestBoost)
            {
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventNewbiequestBoostBuySuccessful,
                    ("Prize:", verifyExtraInfo.Item.Price.ToString()));
            }
 
            collectPopUp.viewController.SetUpViewContent(verifyExtraInfo, (callback) =>
            {
                Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                    {
                        collectPopUp.SubscribeCloseAction(() =>
                        {
                            collectCallback?.Invoke();
                            EventBus.Dispatch(new EventCommonPaymentComplete(shopType));
                        });
                        callback(success, fulfillExtraInfo);
                    });
            });
        }
    }
}