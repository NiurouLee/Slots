using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class RushPassRaidHandler:PaymentHandler
    {
        public RushPassRaidHandler() : base(ShopType.RushPass)
        {
           
        }

        public override void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            base.HandlePaymentSuccess(purchaseCallbackArgs, verifyExtraInfo);
            Client.Get<IapController>()
                .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo, (success, fulfillExtraInfo, collectCallback) =>
                {
                    if (success && fulfillExtraInfo != null && fulfillExtraInfo.RewardItems.Count > 0)
                    {
                        ItemSettleHelper.SettleItems(fulfillExtraInfo.RewardItems, () =>
                        {
                            var activity =
                                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as
                                    Activity_LevelRushRushPass;
                            if (activity != null)
                            {
                                // await activity.GetRushPassData();
                                EventBus.Dispatch(new EventRushPassPaidFinish());
                            }
                        });
                    }
                    else
                    {
                        EventBus.Dispatch(new EventRushPassPaidFail());
                        //TOD fulfill失败的情况
                    }
                });
        }
    }
}