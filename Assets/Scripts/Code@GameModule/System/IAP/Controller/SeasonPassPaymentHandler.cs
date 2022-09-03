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
    public class SeasonPassPaymentHandler: PaymentHandler
    {
        public SeasonPassPaymentHandler():base(ShopType.SeasonPass)
        {

        }
        public override void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,async (success, fulfillExtraInfo, collectCallback) =>
                    {
                        EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
                        var preLevel = Client.Get<SeasonPassController>().Level;
                        await Client.Get<SeasonPassController>().GetSeasonPassData();
                        var rewardLevel = Client.Get<SeasonPassController>().Level-preLevel;
                        collectCallback?.Invoke();
                        if (verifyExtraInfo.Item.ProductType == "missionpass_unlock")
                        {
                            BiManagerGameModule.Instance.SendGameEvent(
                                BiEventFortuneX.Types.GameEventType.GameEventMissionPassUnlockPurchaseSuccess,
                                ("PaymentId", verifyExtraInfo.Item.PaymentId.ToString()),
                                ("Price", verifyExtraInfo.Item.Price.ToString()),
                                ("MissionPassLevel",Client.Get<SeasonPassController>().Level.ToString()));
                            EventBus.Dispatch(new EventSeasonPassCloseBuyGolden());
                            var popup = await PopupStack.ShowPopup<SeasonPassUnlockGolden>();
                            popup.SubscribeCloseAction(async () =>
                            {
                                if (verifyExtraInfo.Item.ProductId.Contains("missionpass5"))
                                {
                                    var popupLevel = await PopupStack.ShowPopup<SeasonPassLevelUp>();
                                    popupLevel.InitWith((int)rewardLevel);
                                    popupLevel.SubscribeCloseAction(() =>
                                    {
                                        EventBus.Dispatch(new EventSeasonPassUpdate(true));
                                    });
                                }
                                else
                                {
                                   EventBus.Dispatch(new EventSeasonPassUpdate(true)); 
                                }
                            });
                        }
                        else
                        {
                            BiManagerGameModule.Instance.SendGameEvent(
                                BiEventFortuneX.Types.GameEventType.GameEventMissionPassBuyLevelPurchaseSuccess,
                                ("PaymentId", verifyExtraInfo.Item.PaymentId.ToString()),
                                ("Price", verifyExtraInfo.Item.Price.ToString()),
                                ("MissionPassLevel",Client.Get<SeasonPassController>().Level.ToString()));
                            var popup = await PopupStack.ShowPopup<SeasonPassLevelUp>();
                            popup.InitWith((int)rewardLevel);
                        }
                    });
            }
    }
}