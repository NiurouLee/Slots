using System.Threading.Tasks;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class CrazeSmash_PaymentHandler : PaymentHandler
    {
        public CrazeSmash_PaymentHandler() : base(ShopType.CrazeSmash) { }

        public override void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,async (success, fulfillExtraInfo, collectCallback) =>
                    {
                        var items = fulfillExtraInfo.RewardItems;
                        foreach (var item in items)
                        {
                            switch (item.Type)
                            {
                                case Item.Types.Type.SilverHammer:
                                    var popup1 = await PopupStack.ShowPopup<UICrazeSmashGetHammerPopup_Silver>();
                                    popup1.Set(item.SilverHammer.Amount);
                                    await Task.Delay(3000);
                                    PopupStack.ClosePopup<UICrazeSmashGetHammerPopup_Silver>();
                                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashBuy, ("type", "1"), ("count", $"{item.SilverHammer.Amount}"));
                                    break;
                                case Item.Types.Type.GoldHammer:
                                    var popup2 = await PopupStack.ShowPopup<UICrazeSmashGetHammerPopup_Gold>();
                                    popup2.Set(item.GoldHammer.Amount);
                                    await Task.Delay(3000);
                                    PopupStack.ClosePopup<UICrazeSmashGetHammerPopup_Gold>();
                                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashBuy, ("type", "2"), ("count", $"{item.GoldHammer.Amount}"));
                                    break;
                            }
                        }
                        XDebug.Log("111111111111111111111 craze smash payment success");
                        EventBus.Dispatch(new Event_CrazeSmash_PurchaseFinish());
                    });
        }
    }
}