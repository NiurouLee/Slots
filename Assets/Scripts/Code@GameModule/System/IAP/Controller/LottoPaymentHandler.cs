using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class LottoPaymentHandler : PaymentHandler
    {
        public LottoPaymentHandler()
        : base(ShopType.Lotto)
        {

        }

        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            XDebug.Log("LottoPaymentHandler HandlePaymentSuccess");
            if (PopupStack.GetPopup<LottoBonusPayPopup>() != null)
            {
                var popup = PopupStack.GetPopup<LottoBonusPayPopup>();
                popup.viewController.SetUpViewContent(verifyExtraInfo, (callback) =>
                        {
                            Client.Get<IapController>()
                                .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                                {
                                    callback(success, fulfillExtraInfo);
                                });
                        });
            }
            else
            {
                //补单操作
                if (purchaseCallbackArgs.isReplenishmentOrder)
                {
                    var item = XItemUtility.GetItem(verifyExtraInfo.Item.SubItemList,
                        Item.Types.Type.LevelRushPaidGame);
                    LottoPayInfo lottoPayInfo = new LottoPayInfo(item.LevelRushPaidGame, true);
                    var popup = await PopupStack.ShowPopup<LottoBonusPayPopup>("UILottoBonusPay", lottoPayInfo);
                    // popup.viewController.StartGame(item);
                    popup.viewController.SetUpViewContent(verifyExtraInfo, (callback) =>
                    {
                        Client.Get<IapController>()
                            .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo, (success, fulfillExtraInfo, collectCallback) =>
                            {
                                callback(success, fulfillExtraInfo);
                                // EventBus.Dispatch(new EventPayWheelConsumeComplete(success));
                            });
                    });
                }
            }
        }
    }
}
