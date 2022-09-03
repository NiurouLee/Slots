using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class JulyCarnivalPaymentHandler : PaymentHandler
    {
        public JulyCarnivalPaymentHandler() : base(ShopType.JulyCarnival)
        {
            
        }

        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            Client.Get<IapController>()
                .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo, async (success, fulfillExtraInfo, collectCallback) =>
                {
                    var activity = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.JulyCarnival) as
                        Activity_JulyCarnival;
                    if (activity == null)
                        return;
                    BiManagerGameModule.Instance.SendGameEvent(
                        BiEventFortuneX.Types.GameEventType.GameEventFourthofjulyPassunlock);
                    var pageInfo = await activity.GetIndependenceDayMainPageInfoData();
                    // 短线重连
                    var collectView = PopupStack.GetPopup<JulyCarnivalRewardCollectPopup>();
                    var mainView = PopupStack.GetPopup<JulyCarnivalMainPopup>();
                    if (mainView != null)
                    {
                        mainView.HideBuyBtn();
                    }
                    if (collectView != null)
                    {
                        collectView.viewController.SetCurrentPageInfoAndRefreshUI(pageInfo);
                    }
                    else if (mainView != null)
                    {
                        mainView.SetBtnStatus(false);
                        mainView.viewController.currentData = pageInfo;
                        if (mainView.CheckHasRewardToCollect())
                        {
                            mainView.ShowUnlockAni();
                            await XUtility.WaitSeconds(0.5f);
                            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(JulyCarnivalRewardCollectPopup), pageInfo)));
                        }
                        else
                        {
                            mainView.viewController.lastData = pageInfo;
                            mainView.RefreshUI();
                            mainView.SetBtnStatus(true);
                        }
                    }
                });
        }
    }
}