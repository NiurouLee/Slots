using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIContactUsDialogView : View
    {
        [ComponentBinder("Content/BG/Text")]
        public Text tmpTextContent;

        [ComponentBinder("TimerText")]
        public TMP_Text tmpTextTime;

        [ComponentBinder("PlayerAvatar/AvatarMask/Icon")]
        public RawImage rawImageAvatar;

        [ComponentBinder("Content")]
        public RectTransform rectTransformContent;


        public void Set(UserComplainMessage messageData)
        {
            var content = messageData.Message;
            tmpTextContent.text = content;

            var time = messageData.CreatedAt;
            tmpTextTime.text = ContactUsController.GetDateTimeFromMilliseconds(time).ToString();

            if (messageData.MessageType == UserComplainMessage.Types.MessageType.Complain)
            {
                var avatarID = Client.Get<UserController>().GetUserAvatarID();
                if (rawImageAvatar != null)
                {
                    rawImageAvatar.texture = AvatarController.defaultAvatar;

                    AvatarController.GetSelfAvatar(avatarID, (t) =>
                    {
                        var controller = Client.Get<UserController>();
                        if (rawImageAvatar != null && controller != null && avatarID == controller.GetUserAvatarID())
                        {
                            rawImageAvatar.texture = t;
                        }
                    });
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformContent);
            var height = rectTransformContent.rect.height;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 60);
        }
    }
}
