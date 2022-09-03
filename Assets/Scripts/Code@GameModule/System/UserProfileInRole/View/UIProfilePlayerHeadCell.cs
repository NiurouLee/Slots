using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameModule
{
    public class UIProfilePlayerHeadCell : View
    {
        [ComponentBinder("Background/AvatarMask/IconEnable")]
        public RawImage rawImage;
        
        [ComponentBinder("Background/ReminderGroup")]
        public Transform reminderGroup;

        [ComponentBinder("")]
        public Toggle toggle;

        public uint avatarId;

        public void Set(Texture2D texture, uint inAvatarId)
        {
            rawImage.texture = texture;
            avatarId = inAvatarId;
            
            reminderGroup.gameObject.SetActive(Client.Get<UserController>().IsNewAvatar(inAvatarId));
        }

        public bool isOn
        {
            get { return toggle.isOn; }
            set { toggle.isOn = value; }
        }

        public void SetOnValueChanged(UnityAction<bool> onValueChanged)
        {
            toggle.onValueChanged.RemoveAllListeners();
            if (onValueChanged != null)
            {
                toggle.onValueChanged.AddListener(onValueChanged);
            }
        }
    }
}
