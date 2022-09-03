// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/21/16:55
// Ver : 1.0.0
// Description : SuperSpinXPaymentHandler.cs
// ChangeLog :
// **********************************************
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;


namespace GameModule
{
    public class SuperSpinXPaymentHandler:PaymentHandler
    {
        public SuperSpinXPaymentHandler()
        :base(ShopType.SuperSpinX)
        {
            
        }

        private void ProcessFulfillExtraInfo(FulfillExtraInfo fulfillExtraInfo)
        {
            var superSpinXGame = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.SuperSpinxGame);
            if (superSpinXGame != null && superSpinXGame.SuperSpinxGame != null &&
                superSpinXGame.SuperSpinxGame.GameInfo.Result != null)
            {
                var reward = superSpinXGame.SuperSpinxGame.GameInfo.Result.Reward;
                if (reward != null && reward.Items.count > 0)
                {
                    fulfillExtraInfo.RewardItems.Remove(superSpinXGame);

                    for (var i = 0; i < reward.Items.count; i++)
                    {
                        fulfillExtraInfo.RewardItems.Insert(i, reward.Items[i]);
                    }
                }
            }
        }
        
        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            if (purchaseCallbackArgs.isReplenishmentOrder)
            {
                var item = XItemUtility.GetItem(verifyExtraInfo.Item.SubItemList, Item.Types.Type.SuperSpinxGame);
               
                if (item == null || item.SuperSpinxGame == null || item.SuperSpinxGame.GameInfo == null || item.SuperSpinxGame.GameInfo.Result == null)
                {
                    Client.Get<IapController>()
                        .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,
                            (success, fulfillExtraInfo, collectCallback) =>
                            {
                                collectCallback.Invoke();
                            });
                            
                    XDebug.LogOnExceptionHandler("SuperSpinX Exception Payment:" + purchaseCallbackArgs.productId);
                    return;
                }
                 
                var collectPopUp = await PopupStack.ShowPopup<SuperSpinXPopup>();
                collectPopUp.viewController.SetPurchaseContent(verifyExtraInfo, (callback) =>
                {
                    Client.Get<IapController>()
                        .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                        {
                            if(success)
                                ProcessFulfillExtraInfo(fulfillExtraInfo);
                            
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
                EventBus.Dispatch(new EventOnSuperSpinXPurchaseSucceed(verifyExtraInfo, (callback) =>
                {
                    Client.Get<IapController>()
                        .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                        {
                            if(success)
                                ProcessFulfillExtraInfo(fulfillExtraInfo);
                            
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