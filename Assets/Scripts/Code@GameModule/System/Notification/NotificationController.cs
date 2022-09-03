using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{

    public class NotificationController : LogicController
    {
        public NotificationController(Client client) : base(client) { }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            APIManagerGameModule.Instance.OnPush<PNotification>(OnPNotification);
        }

        private void OnPNotification(PNotification obj)
        {
            if (obj == null) { return; }
            XDebug.Log($"1111111111111 Receive PNotification:{obj.NotificationType}");
            if (obj.NotificationType != NotificationType.Jackpot || obj.Pb == null || obj.Pb.Data == null) { return; }

            if (PreferenceController.IsJackpotNotificationEnabled() == false) { return; }

            var result = SJackpotNotification.Parser.ParseFrom(obj.Pb.Data.ToByteArray());
            if (result == null) { return; }
            EventBus.Dispatch(new EventPNotification(obj));
        }
    }
}
