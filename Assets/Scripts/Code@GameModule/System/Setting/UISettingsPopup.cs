using Code;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UISetPanel")]
    public class UISettingsPopup : Popup<UISettingsPopupController>
    {

        [ComponentBinder("Root/ScrollView/Viewport/Content/AppleCell")]
        public Transform transformApple;

        [ComponentBinder("Root/ScrollView/Viewport/Content/AppleCell/AppleButton")]
        public Button buttonApple;


        [ComponentBinder("Root/ScrollView/Viewport/Content/FacebookCell")]
        public Transform transformFacebook;

        [ComponentBinder("Root/ScrollView/Viewport/Content/FacebookCell/FacebookButton")]
        public Button buttonFacebook;

        [ComponentBinder("Root/ScrollView/Viewport/Content/MusicCell/MusicButton")]
        public Button buttonMusic;

        [ComponentBinder("Root/ScrollView/Viewport/Content/MusicCell/MusicButton")]
        public Animator animatorButtonMusic;

        [ComponentBinder("Root/ScrollView/Viewport/Content/SoundCell/SoundButton")]
        public Button buttonSound;

        [ComponentBinder("Root/ScrollView/Viewport/Content/SoundCell/SoundButton")]
        public Animator animatorButtonSound;

        [ComponentBinder("Root/ScrollView/Viewport/Content/NotificationsCell/NotificationsButton")]
        public Button buttonNotification;

        [ComponentBinder("Root/ScrollView/Viewport/Content/NotificationsCell/NotificationsButton")]
        public Animator animatorButtonNotification;

        [ComponentBinder("Root/BottomGroup/PricacyPolicyGroup/PrivacyPolicyButton")]
        public Button buttonPrivacyPolicy;

        [ComponentBinder("Root/BottomGroup/ServiceGroup/ServiceButton")]
        public Button buttonTerms;

        [ComponentBinder("Root/ScrollView/Viewport/Content/RateUsCell/RateUsButton")]
        public Button buttonRateUs;

        [ComponentBinder("Root/ScrollView/Viewport/Content/HelpCell/HelpButton")]
        public Button buttonHelp;

        [ComponentBinder("Root/BottomGroup/UIDGroup/IDText")]
        public TMP_Text tmpTextUID;

        [ComponentBinder("Root/BottomGroup/VersionGroup/VersionText")]
        public TMP_Text tmpTextVersion;

        private const string ToOn = "ToOn";
        private const string IsOn = "IsOn";
        private const string ToOff = "ToOff";
        private const string IsOff = "IsOff";

        public UISettingsPopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1365, 768);
        }

        public void SetButtonState(Animator buttonAnimator, bool enabled)
        {
            if (buttonAnimator == null) { return; }
            var stateName = enabled ? IsOn : IsOff;
            if (buttonAnimator.HasState(0, Animator.StringToHash(stateName)))
            {
                buttonAnimator.Play(stateName);
            }
        }

        public void PlayButtonAnimation(Animator buttonAnimator, bool enable)
        {
            if (buttonAnimator == null) { return; }
            var stateName = enable ? ToOn : ToOff;
            if (buttonAnimator.HasState(0, Animator.StringToHash(stateName)))
            {
                buttonAnimator.Play(stateName);
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventSettingOpen);
        }

        public void Set()
        {
            var isMusicEnabled = PreferenceController.IsMusicEnabled();
            SetButtonState(animatorButtonMusic, isMusicEnabled);

            var isSoundEnabled = PreferenceController.IsSoundEnabled();
            SetButtonState(animatorButtonSound, isSoundEnabled);

            var isNotificationEnabled = PreferenceController.IsJackpotNotificationEnabled();
            SetButtonState(animatorButtonNotification, isNotificationEnabled);

            var playerID = Client.Get<UserController>().GetUserId();
            tmpTextUID.text = DragonU3DSDK.Utils.PlayerIdToString(playerID);

            var packageVersion = VersionSetting.Version;
            var bundleRootFolderName = BundleFolderSetting.bundleRootFolderName;
            var bundleVersion = GlobalSetting.bundleVersion;
            tmpTextVersion.text = packageVersion + $" (root:{bundleRootFolderName} res:{bundleVersion}) sv:{GetServerVersion()}";

            SetAccountState();
        }

        public string GetServerVersion()
        {
            if (string.IsNullOrEmpty(ServerVersion.ServerConnectVersion))
            {
                return "";
            }
#if PRODUCTION_PACKAGE
            if (ConfigurationController.Instance.API_Server_URL_Release.Contains(ServerVersion.ServerConnectVersion))
            {
                return ServerVersion.ServerConnectVersion;
            }
#else 
            if (ConfigurationController.Instance.API_Server_URL_Beta.Contains(ServerVersion.ServerConnectVersion))
            {
                return ServerVersion.ServerConnectVersion;
            }
#endif
            return "";
        }

        public void SetAccountState()
        {
            var hasBindFacebook = AccountManager.Instance.HasBindFacebook();
            transformFacebook.gameObject.SetActive(!hasBindFacebook);

#if UNITY_IOS
            transformApple.gameObject.SetActive(AppleLoginManager.Instance.CanShowAppleLogin());
#else
            transformApple.gameObject.SetActive(false);
#endif
        }
    }

    public class UISettingsPopupController : ViewController<UISettingsPopup>
    {
        private const string TermsOfServiceURL = "https://www.casualjoygames.com/terms/";


        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            SubscribeEvent<EventAccountUpdate>(OnEventAccountUpdate);

            view.buttonApple.onClick.AddListener(OnAppleButtonClick);
            view.buttonFacebook.onClick.AddListener(OnFacebookButtonClick);

            view.buttonMusic.onClick.AddListener(OnMusicButtonClick);
            view.buttonSound.onClick.AddListener(OnSoundButtonClick);
            view.buttonNotification.onClick.AddListener(OnNotificationButtonClick);
            view.buttonPrivacyPolicy.onClick.AddListener(OnPrivacyPolicyButtonClick);
            view.buttonTerms.onClick.AddListener(OnTermsButtonClick);
            view.buttonRateUs.onClick.AddListener(OnRateUsButtonClick);
            view.buttonHelp.onClick.AddListener(OnHelpButtonClick);
        }

        private void OnAppleButtonClick()
        {
            AccountController.AppleLogin();
        }

        private void OnEventAccountUpdate(EventAccountUpdate obj)
        {
            view.SetAccountState();
        }

        private void OnFacebookButtonClick()
        {
            AccountController.FacebookLogin();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventBindFacebookBind, ("source", "settings"));
        }

        private void OnNotificationButtonClick()
        {
            var enable = !PreferenceController.IsJackpotNotificationEnabled();
            view.PlayButtonAnimation(view.animatorButtonNotification, enable);
            PreferenceController.SetJackpotNotificationEnabled(enable);
        }

        private void OnSoundButtonClick()
        {
            var enable = !PreferenceController.IsSoundEnabled();
            view.PlayButtonAnimation(view.animatorButtonSound, enable);
            PreferenceController.SetSoundEnabled(enable);
        }

        private void OnMusicButtonClick()
        {
            var enable = !PreferenceController.IsMusicEnabled();
            view.PlayButtonAnimation(view.animatorButtonMusic, enable);
            PreferenceController.SetMusicEnabled(enable);
        }

        private void OnPrivacyPolicyButtonClick()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventPrivacyClick);

            Application.OpenURL(ConfigurationController.Instance.PrivacyPolicyURL);
        }

        private void OnTermsButtonClick()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTermServiceClick);

            Application.OpenURL(TermsOfServiceURL);
        }

        private async void OnRateUsButtonClick()
        {
            await PopupStack.ShowPopup<UIRateUsPopup>();
        }

        private async void OnHelpButtonClick()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventContactUs, ("source", "settings"));

            await PopupStack.ShowPopup<UIContactUsPopup>();
        }
    }
}