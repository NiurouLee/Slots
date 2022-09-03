// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/30/17:36
// Ver : 1.0.0
// Description : MiscLogicController.cs
// ChangeLog :
// **********************************************

#if UNITY_EDITOR
using BestHTTP;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.SDKEvents;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class MiscLogicController : LogicController
    {
        // private IEventHandler<DragonU3DSDK.SDKEvents.SocketIOError> _handler;
        // private IEventHandler<DragonU3DSDK.SDKEvents.SocketIODisconnected> _handlerDisconnect;
        private IEventHandler<DragonU3DSDK.SDKEvents.DeepLinkEvent> _deepLinkEventHandler;
        private IEventHandler<DragonU3DSDK.SDKEvents.ProfileConflictEvent> _profileConflictEventHandler;
        private IEventHandler<DragonU3DSDK.SDKEvents.FireBaseOnMessageReceivedEvent> _firebaseMessageReceivedEventHandler;
 
        private Dictionary<string, string> receivedCouponIds;

        private bool waitForClaimingDeepLinkReward = false;
        private bool hasNewCouponIdFromSdk = false;
        private bool needShowClaimCouponErrorInfo = false;
        private ErrorCode deepLinkClaimErrorCode;
        
        public MiscLogicController(Client client)
            : base(client)
        {
            receivedCouponIds = new Dictionary<string, string>();
        }

        protected override void SubscribeEvents()
        {
#if UNITY_EDITOR
            HTTPUpdateDelegator.OnBeforeApplicationQuit = () =>
            {
                CleanUp();
                return true;
            };
#endif
            base.SubscribeEvents();
          
            SubscribeEvent<EventNetworkClosed>(OnNetWorkClosed);
            SubscribeEvent<EventOnUnExceptedServerError>(OnUnExceptedServerError);
            SubscribeEvent<EventGameOnFocus>(OnEventGameOnFocus);
      
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.DeepLinkReward);
            SubscribeEvent<EventLoginSuccess>(OnLoginSuccess);
            // _handler = EventManager.Instance.Subscribe<DragonU3DSDK.SDKEvents.SocketIOError>(OnSocketIOError);
            // _handlerDisconnect =
            //     EventManager.Instance.Subscribe<DragonU3DSDK.SDKEvents.SocketIODisconnected>(OnSocketDisconnect);
            
            _profileConflictEventHandler = EventManager.Instance.Subscribe<ProfileConflictEvent>(OnProfileConflictEvent);
            _deepLinkEventHandler = EventManager.Instance.Subscribe<DeepLinkEvent>(OnDeepLinkReward);
            _firebaseMessageReceivedEventHandler = EventManager.Instance.Subscribe<FireBaseOnMessageReceivedEvent>(OnFireBaseMessageReceived);
        }

        private void OnLoginSuccess(EventLoginSuccess evt)
        {
            XDebug.Log($"[[ShowOnExceptionHandler]] OnLoginSuccessCheckAndRequestDeepLinkReward");
        
            // Test code
            // receivedCouponIds.Add("E6247111E4D5D2C3",null);
            // hasNewCouponIdFromSdk = true;
            
            CheckAndRequestDeepLinkReward();
        }
         
        private void OnProfileConflictEvent(ProfileConflictEvent profileConflictEvent)
        {
            DragonU3DSDK.Storage.StorageManager.Instance.ResolveProfileConfict(profileConflictEvent.ServerProfile, true);
        }

        private void OnUnExceptedServerError(EventOnUnExceptedServerError serverError)
        {
#if PRODUCTION_PACKAGE
            ShowInGameRestartPopup();
#else
            CommonNoticePopup.ShowCommonNoticePopUp( ShowInGameRestartPopup, serverError.errorInfo);
#endif
        }

        private async void OnSceneSwitchEnd(Action handleEndCallback,
            EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {

            CheckAdjustClickLabel();
            
            await CheckAndRequestDeepLinkReward();
              
            if (eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING && eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
            {
                ClaimDeepLinkRewardMail(handleEndCallback);
                XDebug.Log($"[[ShowOnExceptionHandler]] ClaimDeepLinkRewardMail");
                LogPerformanceTackLog();
                
                return;
            }
            
            handleEndCallback.Invoke();
        }

        public void LogPerformanceTackLog()
        {
            var count = PerformanceTracker.trackPoints.Count;

            Dictionary<string, string> dicExtras = new Dictionary<string, string>();
            for (var i = 0; i < count; i++)
            {
                var trackPoint = PerformanceTracker.GetTrackPoint(i);
                dicExtras.Add(trackPoint.trackPointName + "_Cost", trackPoint.costTime.ToString());
                dicExtras.Add(trackPoint.trackPointName + "_Finish", trackPoint.finishTime.ToString());
            }
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLaunchMetrics, dicExtras);
        }
        
        private void OnDeepLinkReward(DeepLinkEvent deepLinkEvent)
        {
            if (deepLinkEvent != null && deepLinkEvent.rawData != null)
            {
                var couponId = deepLinkEvent.rawData.Get("couponId");
                if (!string.IsNullOrEmpty(couponId))
                {
                    if (!receivedCouponIds.ContainsKey(couponId))
                    {
                        receivedCouponIds.Add(couponId, null);
                        hasNewCouponIdFromSdk = true;
                        XDebug.Log($"[[ShowOnExceptionHandler]] NewCoupon:{couponId}");
                    }
                    else
                    {
                        receivedCouponIds[couponId] = null;
                    }
                }
            }
        }

        private async void ClaimDeepLinkRewardMail(Action actionEnd)
        {
            if (needShowClaimCouponErrorInfo)
            {
                List<int> couponCodes = new List<int>() {96, 97, 98, 99};
                needShowClaimCouponErrorInfo = false;
                
                if (couponCodes.IndexOf((int) deepLinkClaimErrorCode) >= 0)
                {
                    CommonNoticePopup.ShowCommonNoticePopUp("COUPON_ERROR_" + (int)deepLinkClaimErrorCode, () => { });
                    return;
                }
            }
              
            if (waitForClaimingDeepLinkReward)
            {
                XDebug.Log($"[[ShowOnExceptionHandler]] waitForClaimingDeepLinkReward");
                actionEnd?.Invoke();
                return;
            }
             
            waitForClaimingDeepLinkReward = true;
 
            var inboxController = Client.Get<InboxController>();
            if (inboxController.HasDeepLinkMail())
            {
                var mails = inboxController.GetDeepLinkMails();

                for (var i = mails.Count - 1; i >= 0; i--)
                {
                    var pbData = mails[i]?.ItemList?.ToByteArray();
                    
                    XDebug.Log("[[ShowOnExceptionHandler]] DeepLink  Mail:" + mails[i].MailId);
                    
                    if (pbData == null)
                    {
                        XDebug.Log("[[ShowOnExceptionHandler]] DeepLink  Mail: PB isNull");
                        continue;
                    }

                    var data = MailRewardsPB.Parser.ParseFrom(pbData);

                    if (data == null || data.Rewards.Count <= 0)
                    {
                        XDebug.Log("[[ShowOnExceptionHandler]] DeepLink  Mail:" + mails[i].MailId);
                        continue;
                    }

                    var popup = await PopupStack.ShowPopup<UICommonGetRewardPopup>();
                    var claimTask = new TaskCompletionSource<bool>();

                    popup.viewController.SetUpReward(data.Rewards, "DeepLinkReward",
                        () => { claimTask.SetResult(true); }, (claimCallback) =>
                        {
                            inboxController.ClaimMail(mails[i], (sClaimMail) =>
                            {
                                if (sClaimMail != null)
                                {
                                    var mailContent = InboxController.ParseMailContent(sClaimMail.Content);
                                    claimCallback.Invoke(mailContent.Item);
                                }
                                else
                                {
                                    XDebug.Log("Claim DeepLinkFailed");
                                    claimCallback.Invoke(null);
                                }
                            });
                        });

                    await claimTask.Task;
                }
            }
            else
            {
                XDebug.Log("[[ShowOnExceptionHandler]] No DeepLink  Mail:");
            }

            waitForClaimingDeepLinkReward = false;
            actionEnd?.Invoke();
        }

        private async Task CheckAndRequestDeepLinkReward()
        {
            if (!hasNewCouponIdFromSdk)
            {
                XDebug.Log($"[[ShowOnExceptionHandler]] CheckAndRequestDeepLinkReward[${hasNewCouponIdFromSdk}");
                return;
            }

            if (!APIManagerGameModule.Instance.WebsocketConnected)
            {
                XDebug.Log($"[[ShowOnExceptionHandler]] WebsocketConnected False");
                return;
            }

            bool needRefreshMail = false;
            
            hasNewCouponIdFromSdk = false;
            
            if (receivedCouponIds.Keys.Count > 0)
            {
                var keys = receivedCouponIds.Keys.ToList();
                foreach (var key in keys)
                {
                    if (receivedCouponIds[key] == null)
                    {
                        var cReceiveRewardsInCoupon = new CReceiveRewardsInCoupon()
                        {
                            CouponId = key
                        };
                        
                        var handler = await APIManagerGameModule.Instance
                            .SendAsync<CReceiveRewardsInCoupon, SReceiveRewardsInCoupon>(
                                cReceiveRewardsInCoupon);
                        
                        if (handler.ErrorCode == ErrorCode.Success)
                        {
                            if (handler.Response.Success)
                            {
                                receivedCouponIds[key] = handler.Response.MailId;
                                needRefreshMail = true;
                                deepLinkClaimErrorCode = 0;
                            }
                            else
                            {
                                receivedCouponIds[key] = "Failed";
                            }
                            
                            XDebug.Log("[[ShowOnExceptionHandler]] CheckAndRequestDeepLinkReward  MailId:" +
                                       receivedCouponIds[key]);
                        }
                        else
                        {
                            deepLinkClaimErrorCode = handler.ErrorCode;
                            needShowClaimCouponErrorInfo = true;
                            XDebug.Log("[[ShowOnExceptionHandler]] CheckAndRequestDeepLinkReward failed with:" +
                                       handler.ErrorCode + "/" + handler.ErrorInfo);
                        }
                    }
                }
                
                if(needRefreshMail) 
                    await Client.Get<InboxController>().RefreshAllInboxItem();
            }
        }
        
        private void OnFireBaseMessageReceived(FireBaseOnMessageReceivedEvent evt)
        {
            if (evt.message != null)
            {
                if (evt.message.Data != null)
                {
                    XDebug.Log($"[[ShowOnExceptionHandler]] OnFireBaseMessageReceived:{evt.message.Data}");
                    if (evt.message.Data.ContainsKey("couponId"))
                    {
                        var couponId = evt.message.Data["couponId"];

                        if (!string.IsNullOrEmpty(couponId))
                        {
                            if (!receivedCouponIds.ContainsKey(couponId))
                            {
                                receivedCouponIds.Add(couponId, null);
                                hasNewCouponIdFromSdk = true;
                                XDebug.Log($"[[ShowOnExceptionHandler]] NewCoupon:{couponId}");
                            }
                        }
                    }
                }
                
                if (evt.message.Notification != null)
                {
                    XDebug.Log(
                        $"[[ShowOnExceptionHandler]] OnFireBaseMessageReceived:{evt.message.Notification.Title}/{evt.message.Notification.Body}");
                }
            }
        }

        private void CheckAdjustClickLabel()
        {
            var adjustAttribution = Dlugin.SDK.GetInstance().adjustPlugin.GetAdjustAttribution();

            if (adjustAttribution != null && adjustAttribution.clickLabel != null)
            {
                XDebug.Log("[[ShowOnExceptionHandler]] AdjustClickLabel:" + adjustAttribution.clickLabel);
            }
            else
            {
                XDebug.Log("[[ShowOnExceptionHandler]] AdjustClickLabel: null");
            }
        }

        private  async void OnEventGameOnFocus(EventGameOnFocus evt)
        {
            XDebug.Log("[[ShowOnExceptionHandler]] OnEventGameOnFocus: null");
             if (!ViewManager.Instance.IsInSwitching()
                 && (ViewManager.Instance.InLobbyScene() || ViewManager.Instance.InMachineScene()))
             {
                 await WaitForSeconds(1.0f);
                 CheckAdjustClickLabel();
                 await CheckAndRequestDeepLinkReward();
                 XDebug.Log("[[ShowOnExceptionHandler]] ClaimDeepLinkRewardMail:OnEventGameOnFocus");
                 ClaimDeepLinkRewardMail(null);
             }
        }
 
        private void ShowInGameRestartPopup()
        {
            if (!ViewManager.Instance.HasHighPriorityView<NetErrorNoticePopup>())
            {
                NetErrorNoticePopup.ShowPopUp("UI_CONNECTION_LOST",
                    () =>
                    {
                        EventBus.Dispatch(new EventRequestGameRestart(true));
                    });
            }
        }

        private void OnNetWorkClosed(EventNetworkClosed evt)
        {
            ShowInGameRestartPopup();
        }
        // private void OnSocketDisconnect(SocketIODisconnected obj)
        // {
        //     if (!isShowSocketInfo && ViewManager.Instance.IsValid)
        //     {
        //         isShowSocketInfo = true;
        //         CommonNoticePopup.ShowCommonNoticePopUp("UI_CONNECTION_LOST",
        //             () =>
        //             {
        //                 isShowSocketInfo = false;
        //                 EventBus.Dispatch(new EventRequestGameRestart(true));
        //             });
        //     }
        // }


        protected bool isShowSocketInfo = false;

        // protected void OnSocketIOError(DragonU3DSDK.SDKEvents.SocketIOError socketIOError)
        // {
        //     if (!isShowSocketInfo && ViewManager.Instance.IsValid)
        //     {
        //         isShowSocketInfo = true;
        //         CommonNoticePopup.ShowCommonNoticePopUp("UI_CONNECTION_LOST",
        //             () =>
        //             {
        //                 isShowSocketInfo = false;
        //                 EventBus.Dispatch(new EventRequestGameRestart(true));
        //             });
        //     }
        // }

        public override void CleanUp()
        {
            base.CleanUp();
            // EventManager.Instance.Unsubscribe(_handler);
            // EventManager.Instance.Unsubscribe(_handlerDisconnect);
            
            EventManager.Instance.Unsubscribe(_deepLinkEventHandler);
            EventManager.Instance.Unsubscribe(_profileConflictEventHandler);
            EventManager.Instance.Unsubscribe(_firebaseMessageReceivedEventHandler);
        }
    }
}