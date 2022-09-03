using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIProfilePopupProfileGroup : View<UIProfilePopupProfileGroupViewController>
    {
        [ComponentBinder("BasicInformationGroup/PlayerHeadGroup/PlayerHeadButton")]
        public Button buttonEdit;

        [ComponentBinder("BasicInformationGroup/UIDGroup/IDNumberText")]
        public TMP_Text tmpTextID;

        [ComponentBinder("BasicInformationGroup/PlayerHeadGroup/NameGroup/NameText")]
        public Text tmpTextName;

        [ComponentBinder("BasicInformationGroup/PlayerHeadGroup/PlayerHeadButton/LevelGroup/LevelText")]
        public TMP_Text tmpTextLevel;

        [ComponentBinder("PropertyGroup/CoinsGroup/IntegralText")]
        public TMP_Text tmpTextCoin;
        
        [ComponentBinder("BasicInformationGroup/PlayerHeadGroup/PlayerHeadButton/EditorIcon")]
        public Transform editorIcon;
        
        [ComponentBinder("BasicInformationGroup/PlayerHeadGroup/PlayerHeadButton/EditorNoticeIcon")]
        public Transform editorNoticeIcon;
 
        [ComponentBinder("PropertyGroup/EmeraldsGroup/IntegralText")]
        public TMP_Text tmpTextEmeralds;

        [ComponentBinder("PropertyGroup/VIPGroup/ProgressBar")]
        public Slider sliderVIP;

        [ComponentBinder("PropertyGroup/VIPGroup/ProgressBar/IntegralText")]
        public TMP_Text tmpTextVIP;


        [ComponentBinder("BasicInformationGroup/PlayerHeadGroup/PlayerHeadButton/AvatarMask/Icon")]
        public RawImage rawImageAvatar;

        [ComponentBinder("PropertyGroup/VIPGroup/Icon")]
        public Image imageVIP;

        [ComponentBinder("PropertyGroup/CoinsGroup/PlusButton")]
        public Button buttonAdd1;

        [ComponentBinder("PropertyGroup/EmeraldsGroup/PlusButton")]
        public Button buttonAdd2;

        [ComponentBinder("PropertyGroup/FacebookButton")]
        public Button buttonFacebook;

        [ComponentBinder("PropertyGroup/FacebookButton/FBBindAccountSucceed")]
        public Transform transformFacebookButtonSuccess;

        [ComponentBinder("PropertyGroup/AppleButton")]
        public Button buttonApple;

        [ComponentBinder("PropertyGroup/AppleButton/BG/AppleBindAccountSucceed")]
        public Transform transformAppleButtonSuccess;


        [ComponentBinder("PropertyGroup/OnlyFacebookButton")]
        public Button buttonOnlyFacebook;

        [ComponentBinder("PropertyGroup/OnlyFacebookButton/FBBindAccountSucceed")]
        public Transform transformOnlyFacebookButtonSuccess;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            buttonEdit.onClick.AddListener(OnEditButtonClick);
            buttonAdd1.onClick.AddListener(OnAddCoinClicked);
            buttonAdd2.onClick.AddListener(OnAddDiamondClicked);
            buttonFacebook.onClick.AddListener(OnButtonFacebookClick);
            buttonOnlyFacebook.onClick.AddListener(OnButtonFacebookClick);
            buttonApple.onClick.AddListener(OnButtonAppleClick);
        }

        private void OnButtonFacebookClick()
        {
            AccountController.FacebookLogin();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventBindFacebookBind, ("source", "profile"));
        }

        private void OnButtonAppleClick()
        {
            AccountController.AppleLogin();
        }

        private async void OnEditButtonClick()
        {
            var popup = await PopupStack.ShowPopup<UIProfileEditorPopup>();
        }

        private void OnAddCoinClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "ProfileUIAddCoin")));
        }

        private void OnAddDiamondClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), StoreCommodityView.CommodityPageType.Diamond, "ProfileUIAddDiamond")));
        }

        private string GetProgressText()
        {
            float prograss = GetProgress();
            return $"{(int)(prograss * 100)}%";
        }

        private float GetProgress()
        {
            var controller = Client.Get<VipController>();
            return controller.GetProgress();
        }

        private float GetVIPLevel()
        {
            var controller = Client.Get<VipController>();
            return controller.GetVipLevel();
        }

        public void Refresh()
        {
            var controller = Client.Get<UserController>();
            var userName = controller.GetUserName();
            var playerID = controller.GetUserId();
            var avatarID = controller.GetUserAvatarID();
            var coin = controller.GetCoinsCount();
            var emeralds = controller.GetDiamondCount();
            var userLevel = controller.GetUserLevel();

            if (rawImageAvatar != null)
            {
                rawImageAvatar.texture = AvatarController.defaultAvatar;

                AvatarController.GetSelfAvatar(avatarID, (t) =>
                {
                    if (rawImageAvatar != null && controller != null && avatarID == controller.GetUserAvatarID())
                    {
                        rawImageAvatar.texture = t;
                    }
                });
            }

            bool hasNewAvatar = Client.Get<UserController>().HasNewAvatar();
            editorNoticeIcon.gameObject.SetActive(hasNewAvatar);
            editorIcon.gameObject.SetActive(!hasNewAvatar);

            if (tmpTextID != null) { tmpTextID.text = DragonU3DSDK.Utils.PlayerIdToString(playerID); }

            if (tmpTextName != null) { tmpTextName.text = userName; }

            if (tmpTextLevel != null) { tmpTextLevel.text = "LV:" + userLevel; }

            if (tmpTextCoin != null) { tmpTextCoin.text = coin.GetCommaFormat(); }

            if (tmpTextEmeralds != null) { tmpTextEmeralds.text = emeralds.GetCommaFormat(); }

            var vipLevel = GetVIPLevel();

            if (tmpTextVIP != null)
            {
                tmpTextVIP.text = vipLevel >= 7 ? "MAX" : GetProgressText();
            }

            if (imageVIP != null)
            {
                imageVIP.sprite = (parentView as UIProfilePopup).atlas.GetSprite($"UI_VIP_icon_{vipLevel}");
            }

            if (sliderVIP != null)
            {
                sliderVIP.value = vipLevel >= 7 ? 1 : GetProgress();
            }

            buttonFacebook.gameObject.SetActive(!AccountManager.Instance.HasBindFacebook());

            SetAccountState();
        }

        private void SetAccountState()
        {
            var hasBindFacebook = AccountManager.Instance.HasBindFacebook();

            // #if UNITY_IOS
            //             var supportAppleLogin = AppleLoginManager.Instance.IsCurrentPlatformSupported();
            //             if (supportAppleLogin)
            //             {
            //                 var hasBindApple = AccountManager.Instance.HasBindApple();
            //                 buttonOnlyFacebook.gameObject.SetActive(false);
            //                 buttonFacebook.gameObject.SetActive(true);
            //                 buttonApple.gameObject.SetActive(true);

            //                 buttonApple.interactable = !hasBindApple;
            //                 buttonFacebook.interactable = !hasBindFacebook;
            //                 transformAppleButtonSuccess.gameObject.SetActive(hasBindApple);
            //                 transformFacebookButtonSuccess.gameObject.SetActive(hasBindFacebook);
            //             }
            //             else
            //             {
            //                 buttonOnlyFacebook.gameObject.SetActive(true);
            //                 buttonFacebook.gameObject.SetActive(false);
            //                 buttonApple.gameObject.SetActive(false);

            //                 buttonOnlyFacebook.interactable = !hasBindFacebook;
            //                 transformOnlyFacebookButtonSuccess.gameObject.SetActive(hasBindFacebook);
            //             }
            // #else
            buttonOnlyFacebook.gameObject.SetActive(true);
            buttonFacebook.gameObject.SetActive(false);
            buttonApple.gameObject.SetActive(false);

            buttonOnlyFacebook.interactable = !hasBindFacebook;
            transformOnlyFacebookButtonSuccess.gameObject.SetActive(hasBindFacebook);
            // #endif
        }

        public override void Show()
        {
            base.Show();
            Refresh();
        }
    }

    public class UIProfilePopupProfileGroupViewController : ViewController<UIProfilePopupProfileGroup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAccountUpdate>(OnEventAccountUpdate);
            SubscribeEvent<EventUserNewAvatarStateChanged>(OnEventUserNewAvatarStateChanged);
        }
        
        private void OnEventUserNewAvatarStateChanged(EventUserNewAvatarStateChanged evt)
        {
            view.Refresh();
        }
        private void OnEventAccountUpdate(EventAccountUpdate obj)
        {
            view.Refresh();
        }
    }
}
