// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/07/17:35
// Ver : 1.0.0
// Description : InboxModal.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

namespace GameModule
{
    public class InboxModal : Model<SListMail>
    {
        public readonly List<InboxItem> inboxMailItems = new List<InboxItem>();
        public readonly List<InboxItem> inboxCouponItems = new List<InboxItem>();
        public readonly List<Mail> listDeepLinkMail = new List<Mail>();
        
        public InboxModal()
            : base(ModelType.TYPE_INBOX_MODEL)
        {
            
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            

            await RefreshMailData();
            
            await RefreshCouponData();
        }

        public async Task RefreshCouponData()
        {
            var c = new CGetUserCoupons();
            var handle = await APIManagerGameModule.Instance.SendAsync<CGetUserCoupons, SGetUserCoupons>(c);

            if (handle.Response != null && handle.ErrorCode == 0)
            {
                var s = handle.Response;
                if (s != null)
                {
                    UpdateCouponItemData(s);
                }
            }

            EventBus.Dispatch(new EventReceiveUserCoupons());
        }
        
        public async Task RefreshMailData()
        {
            var tcs = new TaskCompletionSource<bool>();
           
            var cListMail = new CListMail();
            
            APIManagerBridge.Send(cListMail,
                (message) =>
                {
                    var  sListMail = message as SListMail;
                    UpdateModelData(sListMail);
                    tcs.SetResult(true);
                },
                (errorCode, message, response) =>
                {
                    XDebug.LogError("11111111 SListMail Error:" + message);
                    tcs.SetResult(false);
                });

            await tcs.Task;
        }

        public override void UpdateModelData(SListMail inModelData)
        {
            base.UpdateModelData(inModelData);
            
            UpdateMailData();
        }
        
        //在刷新Inbox数据之后获取的的InboxItem才能保证数据的正确性
        public List<InboxItem> GetAllInboxItem()
        {
            var inboxItems = new List<InboxItem>();
             
            if (AdController.Instance.ShouldShowRV(eAdReward.InboxRV, false))
            {
                inboxItems.Add(new InboxItem(){typeID = -1});
            }
            
            inboxItems.AddRange(inboxCouponItems);
            inboxItems.AddRange(inboxMailItems);

            return inboxItems;
        }

        public void UpdateMailData()
        {
            inboxMailItems.Clear();
            listDeepLinkMail.Clear();
            
            if (modelData != null && modelData.Mails.Count > 0)
            {
                for (var i = 0; i < modelData.Mails.Count; i++)
                {
                    if (IsValidInboxMailItem(modelData.Mails[i]))
                    {
                        var inboxItem = new InboxItem();
                        inboxItem.typeID = (int) modelData.Mails[i].MailSubType;
                        inboxItem.data = modelData.Mails[i];
                        inboxMailItems.Add(inboxItem);
                    }
                    //DeepLink奖励作为一种特殊奖励不放在Inbox中显示，而是作为弹窗奖励显示
                    else if (IsValidDeepLinkMail(modelData.Mails[i]))
                    {
                        listDeepLinkMail.Add(modelData.Mails[i]);
                    }
                }
            }
        }
        
        public void UpdateCouponItemData(SGetUserCoupons sGetUserCoupons)
        {
            inboxCouponItems.Clear();

            if (sGetUserCoupons.UserCoupons == null)
                return;

            if (sGetUserCoupons.UserCoupons.Count > 0)
            {
                for (var i = 0; i < sGetUserCoupons.UserCoupons.Count; i++)
                {
                    if (IsValidCouponItem(sGetUserCoupons.UserCoupons[i]))
                    {
                        var inboxItem = new InboxItem();
                        inboxItem.typeID = -2;
                        inboxItem.data = sGetUserCoupons.UserCoupons[i];
                        inboxCouponItems.Add(inboxItem);
                    }
                }
            }
        }

        public bool HasDeepLinkReward()
        {
            return listDeepLinkMail != null && listDeepLinkMail.Count > 0;
        }
        
        public  List<Mail> GetDeepLinkMail()
        {
            return listDeepLinkMail;
        }
        
        private bool IsValidCouponItem(SGetUserCoupons.Types.UserCoupon coupon)
        {
            if (XUtility.GetLeftTime((ulong)coupon.ExpireAt * 1000) <= 0)
            {
                return false;
            }
            
            return true;
        }


        private bool IsValidDeepLinkMail(Mail mail)
        {
            if (XUtility.GetLeftTime(mail.Expire) <= 0)
            {
                return false;
            }
            
            if (mail.MailSubType == 999)
                return true;

            return false;
        }

        private bool IsValidInboxMailItem(Mail mail)
        {
            if (mail.MailSubType == 999)
                return false;

            if (XUtility.GetLeftTime(mail.Expire) <= 0)
            {
                return false;
            }

            return true;
        }

        public SGetUserCoupons.Types.UserCoupon GetCoupon(string couponId)
        {
            if (inboxCouponItems.Count > 0)
            {
                for (var i = 0; i < inboxCouponItems.Count; i++)
                {
                    var userCoupon = inboxCouponItems[i].data as SGetUserCoupons.Types.UserCoupon;
                    if (userCoupon != null && IsValidCouponItem(userCoupon))
                    {
                        if (userCoupon.Id == couponId)
                        {
                            return userCoupon;
                        }
                    }
                }
            }

            return null;
        } 
        
        public SGetUserCoupons.Types.UserCoupon GetLinkedCoupon(string activityId)
        {
            if (inboxCouponItems.Count > 0)
            {
                for (var i = 0; i < inboxCouponItems.Count; i++)
                {
                    var userCoupon = inboxCouponItems[i].data as SGetUserCoupons.Types.UserCoupon;
                    if (userCoupon != null && IsValidCouponItem(userCoupon))
                    {
                        if (userCoupon.LinkedActivityId == activityId)
                        {
                            return userCoupon;
                        }
                    }
                }
            }

            return null;
        }

        public void RemoveMail(Mail mail)
        {
            for (var i = 0; i < inboxMailItems.Count; i++)
            {
                var mailItem = inboxMailItems[i].data as Mail;
                if (mailItem != null && mailItem.Equals(mail))
                {
                    inboxMailItems.Remove(inboxMailItems[i]);
                    return;
                }
            }

            for (var i = listDeepLinkMail.Count - 1; i >= 0; i--)
            {
                var mailItem = listDeepLinkMail[i];
                if (mailItem != null && mailItem.Equals(mail))
                {
                    listDeepLinkMail.RemoveAt(i);
                    return;
                }
            }
        }
    }
}