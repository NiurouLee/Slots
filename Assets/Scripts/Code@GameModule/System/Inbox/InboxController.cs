using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.Protobuf;

namespace GameModule
{
    public class InboxController : LogicController
    {
        private CancelableCallback _refreshVipEmailActionCallback;

        private InboxModal _modal;

        public InboxController(Client client) : base(client) { }

        protected override void Initialization()
        {
            base.Initialization();
             _modal = new InboxModal();
        }


        public async Task<SBindCouponToStore> SendCBindCouponToStore(string key)
        {
            var c = new CBindCouponToStore() { CouponId = key };

            var handle = await APIManagerGameModule.Instance.SendAsync<CBindCouponToStore, SBindCouponToStore>(c);

            if (handle.Response != null && handle.ErrorCode == 0)
            {
                return handle.Response;
            }
            else
            {
                return null;
            }
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventPurchasedInStore>(OnEventPurchasedInStore);
        }

        public void OnEventPurchasedInStore(EventPurchasedInStore eventPurchasedInStore)
        {
            _modal.RefreshCouponData();
            EventBus.Dispatch(new EventInBoxItemUpdated());
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetUserCoupons != null && sGetInfoBeforeEnterLobby.SRefreshWeekendVipMail != null)
            {
                UpdateVipMailRefreshData(sGetInfoBeforeEnterLobby.SRefreshWeekendVipMail);
                _modal.UpdateCouponItemData(sGetInfoBeforeEnterLobby.SGetUserCoupons);
                beforeEnterLobbyServerDataReceived = true;
            }
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                RequestWeekendVipMail();
            }

            _modal.FetchModalDataFromServerAsync();

            finishCallback?.Invoke();
        }
 
        public override async void OnPushNotification(FortunexNotification data)
        {
            if (data != null && data.NotificationType == FortunexNotificationType.EmailUpdated)
            {
                await _modal.RefreshMailData();
                EventBus.Dispatch(new EventInBoxItemUpdated());
            }
        }
  
        public List<InboxItem> GetAllInboxItem()
        {
            return _modal.GetAllInboxItem();
        }

        public async void RequestWeekendVipMail()
        {
            var c = new CRefreshWeekendVipMail();
            var s =
                await APIManagerGameModule.Instance.SendAsync<CRefreshWeekendVipMail, SRefreshWeekendVipMail>(c);
            if (s.Response != null && s.ErrorCode == 0)
            {
                UpdateVipMailRefreshData(s.Response);
                //Test
                //_weekendMailRefreshTime = _serverTime + 3000;
              //  XDebug.Log($"11111111 receive RefreshWeekendVipMail data:{_mailCount}");
                EventBus.Dispatch(new EventRefreshWeekendVipMail());
            }
            else
            {
                XDebug.Log($"111111111 Request RefreshWeekendVipMail failed:{s?.ErrorCode}");
            }
        }

        protected void UpdateVipMailRefreshData(SRefreshWeekendVipMail sRefreshWeekendVipMail)
        {
            var refreshTime = XUtility.GetLeftTime(sRefreshWeekendVipMail.WeekendVipMailRefreshTime * 1000) + 5000;
 
            if (_refreshVipEmailActionCallback != null)
            {
                _refreshVipEmailActionCallback.CancelCallback();
                _refreshVipEmailActionCallback = null;
            }

            if (refreshTime > 0)
            {
                _refreshVipEmailActionCallback = new CancelableCallback(async () =>
                {
                    await RefreshAllInboxItem();
                });

                XUtility.WaitSeconds(refreshTime, _refreshVipEmailActionCallback, this);
            }
            else
            {
                RefreshAllInboxItem();
            }
        }
        
        public static ClaimMailContentForProto ParseMailContent(ByteString byteString)
        {
            if (byteString == null || byteString.Length == 0) { return null; }
            
            var result = ClaimMailContentForProto.Parser.ParseFrom(byteString.ToByteArray());

            return result;
        }

        public void ClaimMail(Mail mail, Action<SClaimMail> responseCallback)
        {
            var sendMail = new CClaimMail() {MailId = mail.MailId};

            APIManagerBridge.Send(
                sendMail,
                async (response) =>
                {
                    var result = response as SClaimMail;
                     
                    if (result != null)
                    {
                        var contentData = ParseMailContent(result.Content);

                        if (contentData != null && contentData.UserProfile != null)
                        {
                            EventBus.Dispatch(new EventUserProfileUpdate(contentData.UserProfile));
                        }
                        
                        _modal.RemoveMail(mail);
                        responseCallback.Invoke(result);
                    }
                },
                (errorCode, message, response) => { responseCallback.Invoke(null); }
            );
        }
        
        public void RemoveMail(InboxItem inboxItem)
        {
            if(inboxItem.data != null && inboxItem.data is Mail)
                _modal.RemoveMail(inboxItem.data as Mail);
        }

        public SGetUserCoupons.Types.UserCoupon GetCoupon(string couponId)
        {
            return _modal.GetCoupon(couponId);
        }

        public bool HasDeepLinkMail()
        {
            return _modal.HasDeepLinkReward();
        }
        public List<Mail> GetDeepLinkMails()
        {
            return _modal.GetDeepLinkMail();
        }
        
        public SGetUserCoupons.Types.UserCoupon GetLinkedCoupon(string activityId)
        {
            return _modal.GetLinkedCoupon(activityId);
        }
        
        public async Task<bool> RefreshAllInboxItem()
        {
            await _modal.RefreshMailData();
            await _modal.RefreshCouponData();
            EventBus.Dispatch(new EventInBoxItemUpdated());
            return true;
        }
    }
}