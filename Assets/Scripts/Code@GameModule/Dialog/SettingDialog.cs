// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.UIElements;
// using Button = UnityEngine.UI.Button;
//
// namespace GameModule.UI
// {
//     public class SettingDialog : BaseDialog
//     {
//         private Button logInBtn;
//         private Button gameRulesBtn;
//
//         private TextMeshProUGUI textId;
//         private TextMeshProUGUI textVersion;
//
//         private Button soundOnButton;
//         private Button soundOffButton;
//         private Button musicOnButton;
//         private Button musicOffButton;
//
//         private Button jackpotOnButton;
//         private Button jackpotOffButton;
//
//         private Button notificationOnButton;
//         private Button notificationOffButton;
//
//         private Button faceBookConnectButton;
//         private Button faceBookLogOutButton;
//
//         private Button fanPageButton;
//
//         private Button rateUsButton;
//
//         private Button supportButton;
//
//         private Button privacyButton, termButton;
//
//         public SettingDialog(string url) : base(url)
//         {
//             
//         }
//
//         public override void BindingViewVariable()
//         {
//             base.BindingViewVariable();
//
//             if (!ViewManager.Instance.IsPortrait)
//             {
//                 // if (ViewController.deviceReferenceResolution.x < 1220)
//                 // {
//                 //     transform.parent.localScale = new Vector3(0.8f, 0.8f, 0.8f);
//                 // }
//             }
//  
//             //暂时禁用ScrollRect，目前不需要，以后加了更多设置的时候再打开
//             // var scrollRect = transform.Find("ScrollView").GetComponent<ScrollRect>();
//             // scrollRect.enabled = false;
//             // transform.Find("ScrollView/Scrollbar Vertical").gameObject.SetActive(false);
//             
//             //logInBtn = transform.Find("loginBtn").GetComponent<Button>();
//             //logInBtn.onClick.AddListener(ClickLogInBtn);
//
//             textId = transform.Find("ScrollView/Viewport/Content/TextId").GetComponent<TextMeshProUGUI>();
//             textVersion = transform.Find("TextVersion").GetComponent<TextMeshProUGUI>();
//
//             var copyButton = transform.Find("ScrollView/Viewport/Content/CopyBtn").GetComponent<Button>();
//             copyButton.onClick.AddListener(OnClickCopyUserId);
//
//             soundOnButton = transform.Find("ScrollView/Viewport/Content/SoundOnBtn").GetComponent<Button>();
//             soundOnButton.onClick.AddListener(OnSoundOnClicked);
//
//             soundOffButton = transform.Find("ScrollView/Viewport/Content/SoundOffBtn").GetComponent<Button>();
//             soundOffButton.onClick.AddListener(OnSoundOffClicked);
//
//             soundOnButton.gameObject.SetActive(Client.Player.SoundEnabled);
//             soundOffButton.gameObject.SetActive(!Client.Player.SoundEnabled);
//             
//             musicOffButton = transform.Find("ScrollView/Viewport/Content/MusicOffBtn").GetComponent<Button>();
//             musicOffButton.onClick.AddListener(OnMusicOffClicked);
//
//             musicOnButton = transform.Find("ScrollView/Viewport/Content/MusicOnBtn").GetComponent<Button>();
//             musicOnButton.onClick.AddListener(OnMusicOnClicked);
//
//             musicOnButton.gameObject.SetActive(Client.Player.MusicEnabled);
//             musicOffButton.gameObject.SetActive(!Client.Player.MusicEnabled);
//             
//             musicOnButton.interactable = Client.Player.SoundEnabled;
//             musicOffButton.interactable = Client.Player.SoundEnabled;
//  
//             jackpotOffButton = transform.Find("ScrollView/Viewport/Content/JackpotOffBtn").GetComponent<Button>();
//             jackpotOffButton.onClick.AddListener(OnJackpotNotificationOffClicked);
//
//             jackpotOnButton = transform.Find("ScrollView/Viewport/Content/JackpotOnBtn").GetComponent<Button>();
//             jackpotOnButton.onClick.AddListener(OnJackpotNotificationOnClicked);
//
//             jackpotOnButton.gameObject.SetActive(Client.Player.JackpotNotificationEnabled);
//             jackpotOffButton.gameObject.SetActive(!Client.Player.JackpotNotificationEnabled);
//
//             // notificationOffButton =
//             //     transform.Find("ScrollView/Viewport/Content/NotificationOffBtn").GetComponent<Button>();
//             // notificationOffButton.onClick.AddListener(OnSystemNotificationOffClicked);
//             //
//             // notificationOnButton =
//             //     transform.Find("ScrollView/Viewport/Content/NotificationOnBtn").GetComponent<Button>();
//             // notificationOnButton.onClick.AddListener(OnSystemNotificationOnClicked);
//
//             // notificationOnButton.gameObject.SetActive(Client.Player.SystemNotificationEnabled);
//             // notificationOffButton.gameObject.SetActive(!Client.Player.SystemNotificationEnabled);
//
//             fanPageButton = transform.Find("ScrollView/Viewport/Content/VisitBtn").GetComponent<Button>();
//             fanPageButton.onClick.AddListener(OnFanPageClicked);
//
//             faceBookConnectButton = transform.Find("ScrollView/Viewport/Content/FbBtn").GetComponent<Button>();
//             faceBookConnectButton.onClick.AddListener(OnFaceBookConnectedClicked);
//
//             faceBookLogOutButton = transform.Find("ScrollView/Viewport/Content/LogOutBtn").GetComponent<Button>();
//             faceBookLogOutButton.onClick.AddListener(OnFaceBookLogoutClicked);
//
//             var fbLogin = PTS.FaceBook.IsLoginIn();
//             faceBookConnectButton.gameObject.SetActive(!fbLogin);
//             faceBookLogOutButton.gameObject.SetActive(fbLogin);
//
//             rateUsButton = transform.Find("ScrollView/Viewport/Content/RateBtn").GetComponent<Button>();
//             rateUsButton.onClick.AddListener(OnRateUsClicked);
//
//             supportButton = transform.Find("ScrollView/Viewport/Content/SupportBtn").GetComponent<Button>();
//             supportButton.onClick.AddListener(OnSupportClicked);
//
//             privacyButton = transform.Find("PrivacyBtn").GetComponent<Button>();
//             privacyButton.onClick.AddListener(OnPrivacyClicked);
//             
//             termButton = transform.Find("TermBtn").GetComponent<Button>();
//             termButton.onClick.AddListener(OnTermClicked);
//             
//             UpdateViewContent();
//         }
//
//         public override void OnViewReady()
//         {
//             SubscribeEvent<EventBindStatusChanged>(OnAccountBindingStatusChanged);
//         }
//
//         public void OnAccountBindingStatusChanged(EventBindStatusChanged evt)
//         {
//             faceBookConnectButton.gameObject.SetActive(!SdkManager.instance.fbAdaptor.IsLoginIn());
//             faceBookLogOutButton.gameObject.SetActive(SdkManager.instance.fbAdaptor.IsLoginIn());
//         }
//         
//         public void UpdateViewContent()
//         {
//             textId.text = "ID: " + Client.Player.Id;
//             textVersion.text = "Version:" + PackageSetting.packageVersion + $" ({GlobalSetting.bundleVersion})";
//         }
//         
//         private void OnRateUsClicked()
//         {
//             Log.LogUiClickEvent(UiClick.SettingRateUs);
//             SoundManager.PlaySlotClickSound();
//             Application.OpenURL(GlobalSetting.storeUrl);
//         }
//
//         private void OnMusicOnClicked()
//         {
//             musicOnButton.gameObject.SetActive(false);
//             musicOffButton.gameObject.SetActive(true);
//             Client.Player.SetMusicEnabled(false);
//             EventBus.Dispatch(new EventMusicEnabled(false));
//             Log.LogUiClickEvent(UiClick.SettingMusicOn);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnMusicOffClicked()
//         {
//             musicOnButton.gameObject.SetActive(true);
//             musicOffButton.gameObject.SetActive(false);
//             Client.Player.SetMusicEnabled(true);
//             EventBus.Dispatch(new EventMusicEnabled(true));
//             Log.LogUiClickEvent(UiClick.SettingMusicOff);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnSoundOnClicked()
//         {
//             soundOnButton.gameObject.SetActive(false);
//             soundOffButton.gameObject.SetActive(true);
//           //  ViewController.EnableAudio(false);
//             Client.Player.SetSoundEnabled(false);
//             
//             musicOnButton.interactable = false;
//             musicOffButton.interactable = false;
//             Log.LogUiClickEvent(UiClick.SettingSoundOn);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnSoundOffClicked()
//         {
//             soundOnButton.gameObject.SetActive(true);
//             soundOffButton.gameObject.SetActive(false);
//          //   ViewController.EnableAudio(true);
//             Client.Player.SetSoundEnabled(true);
//             
//             musicOnButton.interactable = true;
//             musicOffButton.interactable = true;
//             Log.LogUiClickEvent(UiClick.SettingSoundOff);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnJackpotNotificationOnClicked()
//         {
//             jackpotOnButton.gameObject.SetActive(false);
//             jackpotOffButton.gameObject.SetActive(true);
//             Client.Player.SetJackpotNotificationEnabled(false);
//             EventBus.Dispatch(new EventJackpotNotificationEnabled(false));
//             Log.LogUiClickEvent(UiClick.SettingJackpotNotificationOn);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnJackpotNotificationOffClicked()
//         {
//             jackpotOnButton.gameObject.SetActive(true);
//             jackpotOffButton.gameObject.SetActive(false);
//             Client.Player.SetJackpotNotificationEnabled(true);
//             EventBus.Dispatch(new EventJackpotNotificationEnabled(true));
//             Log.LogUiClickEvent(UiClick.SettingJackpotNotificationOff);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnSystemNotificationOnClicked()
//         {
//             notificationOffButton.gameObject.SetActive(true);
//             notificationOnButton.gameObject.SetActive(false);
//             Client.Player.SetSystemNotificationEnabled(false);
//             Log.LogUiClickEvent(UiClick.SettingJackpotNotificationOn);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnSystemNotificationOffClicked()
//         {
//             notificationOffButton.gameObject.SetActive(false);
//             notificationOnButton.gameObject.SetActive(true);
//             Client.Player.SetSystemNotificationEnabled(true);
//             Log.LogUiClickEvent(UiClick.SettingJackpotNotificationOff);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnFanPageClicked()
//         {
//             Log.LogUiClickEvent(UiClick.SettingFanPage);
//             MiscUtil.OpenUrl(GlobalSetting.fanPageUrl);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnSupportClicked()
//         {
//             Log.LogUiClickEvent(UiClick.SettingContactUs);
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnFaceBookConnectedClicked()
//         {
//             Log.LogUiClickEvent(UiClick.SettingFaceBookConnected);
//             EventBus.Dispatch(new EventBindingFaceBook());
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnFaceBookLogoutClicked()
//         {
//             Log.LogUiClickEvent(UiClick.SettingFaceBookLogout);
//             SoundManager.PlaySlotClickSound();
//             MiscUtil.ShowCommonDialog(() =>
//             {
//                 //通知服务器玩家主动logout
//                 var proto = new ProtocolManualLogOut();
//                 proto.Send(null);
//                 
//                 PTS.FaceBook.LogOut();
//                 
//                 Log.LogStateEvent(State.SettingFbLogOut);
//                 
//                 EventBus.Dispatch(new EventRequestGameRestart());
//             }, () => { }, "UI_FB_LOGOUT_WARNING");
//         }
//         
//         private void OnClickCopyUserId()
//         {
//             //DialogManager.ShowDialog<GameRuleDialog>("GameRuleDialog");
//             // base.OnCloseClicked();
//
//             UnityEngine.GUIUtility.systemCopyBuffer = Client.Player.Id.ToString();
//            
//             MiscUtil.ShowSystemTextTips("Your UID has been copied.");
//             
//             Log.LogUiClickEvent(UiClick.SettingCopyUid);
//             //  GUIUtility.systemCopyBuffer = Client.Player.Id.ToString();
//             SoundManager.PlaySlotClickSound();
//         }
//
//         private void OnPrivacyClicked()
//         {
//             MiscUtil.OpenUrl(GlobalSetting.GetPolicyVersionUrl());
//             Log.LogUiClickEvent(UiClick.SettingOpenPrivacyPolicy);
//             SoundManager.PlaySlotClickSound();
//         }
//         
//         private void OnTermClicked()
//         {
//             MiscUtil.OpenUrl(GlobalSetting.GetServicePageUrl());
//             Log.LogUiClickEvent(UiClick.SettingOpenService);
//             SoundManager.PlaySlotClickSound();
//         }
//         
//         protected override void OnCloseClicked()
//         {
//             base.OnCloseClicked();
//             Log.LogUiClickEvent(UiClick.SettingClose);
//         }
//     }
// }