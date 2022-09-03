using System;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{

    public abstract class UIInboxCellView_Mail : UIInboxCellView
    {
        protected Mail mailData;
        protected InboxController inboxController;


        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            inboxController = Client.Get<InboxController>();
        }

        public override void ParseData()
        {
            base.ParseData();
           
            if (itemData != null && itemData.data != null)
            {
                mailData = itemData.data as Mail;
            }
        }

        public override void UpdateView()
        {
            base.UpdateView();
            UpdateTimeLeft();
        }
 
        protected virtual void OnButtonClick()
        {
            if (mailData != null)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventInboxCollect, ("type", $"{mailData.MailSubType}"));
            }

            OnClaimMail();
        }
 
        protected virtual void OnClaimMail()
        {
            inboxController.ClaimMail(mailData, OnClaimFinishFromServer);
        }

        protected virtual void OnClaimFinishFromServer(SClaimMail sClaimMail)
        {
            
        }
        
        public override void Update()
        {
            if(itemData != null)
                UpdateTimeLeft();
        }

        public virtual void UpdateTimeLeft()
        {
            if (mailData != null)
            {
                var leftTime = XUtility.GetLeftTime(mailData.Expire);

                if (leftTime > 0)
                {
                    UpdateTimerText(XUtility.GetTimeText(leftTime));
                }
                else
                {
                    inboxController.RemoveMail(itemData);
                    EventBus.Dispatch(new EventInBoxItemUpdated());
                }
            }
        }

        public virtual void UpdateTimerText(string remainString) { }
    }
}
