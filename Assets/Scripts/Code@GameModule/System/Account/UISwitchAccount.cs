using DragonU3DSDK.Account;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FacebookSwitchAccountData
    {
        public string facebookToken;
        public string facebookName;
        public uint userAvatarId;
        public string facebookID;
    }

    [AssetAddress("UISwitchAccount")]
    public class UISwitchAccount : Popup
    {
        [ComponentBinder("Root/AppleGroup")]
        public Transform transformApple;

        [ComponentBinder("Root/AppleGroup/YesGroup/YesButton")]
        public Button buttonAppleYes;

        [ComponentBinder("Root/AppleGroup/NOGroup/NoButton")]
        public Button buttonAppleNo;


        [ComponentBinder("Root/Group")]
        public Transform transformFacebook;

        [ComponentBinder("Root/Group/YesGroup/YesButton")]
        public Button buttonFacebookYes;

        [ComponentBinder("Root/Group/NOGroup/NoButton")]
        public Button buttonFacebookNo;



        [ComponentBinder("Root/Group/MainGroup/ProfileImage")]
        public RawImage rawImageAvatar;

        [ComponentBinder("Root/Group/MainGroup/NameFram/NameText")]
        public Text textName;

        private FacebookSwitchAccountData _data;

        public UISwitchAccount(string address) : base(address)
        {
            contentDesignSize = new Vector2(877, 536);
        }

        public void SetFacebook(FacebookSwitchAccountData data)
        {
            _data = data;
            if (data != null)
            {
                if (rawImageAvatar != null)
                {
                    rawImageAvatar.texture = AvatarController.defaultAvatar;
                }

                AvatarController.GetFacebookAvatar(data.facebookID, (t) =>
                {
                    if (rawImageAvatar != null)
                    {
                        rawImageAvatar.texture = t;
                    }
                });
                textName.text = data.facebookName;
            }

            transformApple.gameObject.SetActive(false);
            transformFacebook.gameObject.SetActive(true);
        }

        public void SetApple()
        {
            transformApple.gameObject.SetActive(true);
            transformFacebook.gameObject.SetActive(false);
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            buttonAppleYes.onClick.AddListener(OnAppleButtonYesClick);
            buttonAppleNo.onClick.AddListener(OnAppleButtonNoClick);

            buttonFacebookYes.onClick.AddListener(OnFacebookButtonYesClick);
            buttonFacebookNo.onClick.AddListener(OnFacebookButtonNoClick);
        }

        private void OnAppleButtonNoClick()
        {
            Close();
        }

        private void OnFacebookButtonNoClick()
        {
            Close();
        }

        private void OnAppleButtonYesClick()
        {
            Close();
            AccountManager.Instance.Clear();
            AccountManager.Instance.BindApple(
                (result) =>
                {
                    XDebug.Log($"1111111111 LoginWithApple state is : {result}");
                    EventBus.Dispatch(new EventRequestGameRestart());
                }
            );
        }

        private void OnFacebookButtonYesClick()
        {
            Close();

            AccountManager.Instance.Clear();
            AccountManager.Instance.BindFacebook(
                (result) =>
                {
                    XDebug.Log($"1111111111 LoginWithApple state is : {result}");
                    EventBus.Dispatch(new EventRequestGameRestart());
                }
            );
        }
    }
}
