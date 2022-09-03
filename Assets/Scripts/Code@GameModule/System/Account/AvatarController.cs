using System;
using DragonU3DSDK.Account;
using Facebook.Unity;
using UnityEngine;

namespace GameModule
{
    public class AvatarController : LogicController
    {
        private Texture2D _selfAvatar;
        private Texture2D _defaultAvatar;

        public AvatarController(Client client) : base(client) { }

        public static Texture2D defaultAvatar
        {
            get
            {
                var controller = Client.Get<AvatarController>();
                if (controller._defaultAvatar == null)
                {
                    controller._defaultAvatar = AssetHelper.GetResidentAsset<Texture2D>("PlayerAvatar_default_1");
                }

                return controller._defaultAvatar;
            }
        }

        private void InnerGetFacebookAvatar(string facebookID, Action<Texture2D> onFinish)
        {
            var selfFacebookID = AccountManager.Instance.GetFacebookId();
            if (selfFacebookID == facebookID && _selfAvatar != null)
            {
                onFinish?.Invoke(_selfAvatar);
            }
            else
            {
                FB.API($"{facebookID}/picture?height={128}&width={128}", Facebook.Unity.HttpMethod.GET,
                    (result) =>
                    {
                        if (AccountManager.Instance.GetFacebookId() == facebookID)
                        {
                            _selfAvatar = result.Texture;
                        }
                        onFinish?.Invoke(result.Texture);
                    });
            }
        }

        public static void GetFacebookAvatar(string facebookID, Action<Texture2D> onFinish)
        {
            Client.Get<AvatarController>().InnerGetFacebookAvatar(facebookID, onFinish);
        }

        public static void GetSelfAvatar(uint avatarID, Action<Texture2D> onFinish)
        {
            var selfFacebookID = AccountManager.Instance.GetFacebookId();
            GetAvatar(avatarID, selfFacebookID, onFinish);
        }

        public static void GetAvatar(uint avatarID, string facebookID, Action<Texture2D> onFinish)
        {
            if (avatarID == 9999)//use facebook avatar
            {
                Client.Get<AvatarController>().InnerGetFacebookAvatar(facebookID, onFinish);
            }
            else
            {
                var avatarAddress = "PlayerAvatar_" + avatarID;
                var texture = AssetHelper.GetResidentAsset<Texture2D>(avatarAddress);
                onFinish?.Invoke(texture);
            }
        }
    }
}
