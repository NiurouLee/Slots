// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/25/16:51
// Ver : 1.0.0
// Description : SettingMenuView.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    public class SettingMenuView : View<SettingMenuViewController>
    {
        [ComponentBinder("MusicButton")] public Button musicButton;

        [ComponentBinder("MenuButtonState")] public Button menuButton;

        [ComponentBinder("SoundButton")] public Button soundButton;

        [ComponentBinder("SettingButton")] public Button settingButton;

        [ComponentBinder("PayTableButton")] public Button payTableButton;
        
        [ComponentBinder("PayTableGroup")] public Transform payTableGroup;
        
        [ComponentBinder("FanPageButton")] public Button fanPageButton;
        
        [ComponentBinder("ContactUsButton")] public Button contactUsButton;

        [ComponentBinder("RedDot")] public Transform redDotOnMenu;

        [ComponentBinder("RedDotContactUs")] public Transform redDotOnContactUs;

        public Animator animator;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }
    }

    public class SettingMenuViewController : ViewController<SettingMenuView>
    {
        public override void OnViewEnabled()
        {
            var soundEnabled = PreferenceController.IsSoundEnabled();

            view.soundButton.transform.Find("EnableState").gameObject.SetActive(soundEnabled);
            view.soundButton.transform.Find("DisableState").gameObject.SetActive(!soundEnabled);

            var musicEnabled = PreferenceController.IsMusicEnabled();

            view.musicButton.transform.Find("EnableState").gameObject.SetActive(musicEnabled);
            view.musicButton.transform.Find("DisableState").gameObject.SetActive(!musicEnabled);
 
            var hasNewMessage = Client.Get<ContactUsController>().HasNewUnReadMessage();
           
            view.redDotOnMenu.gameObject.SetActive(hasNewMessage);
            view.redDotOnContactUs.gameObject.SetActive(hasNewMessage);

            if (ViewManager.SwitchingSceneType == SceneType.TYPE_LOBBY)
            {
                view.payTableGroup.gameObject.SetActive(false);
            }
        }

        protected override void SubscribeEvents()
        {
            view.soundButton.onClick.AddListener(OnSoundClicked);
            view.musicButton.onClick.AddListener(OnMusicClicked);
            view.settingButton.onClick.AddListener(OnSettingClicked);
            view.payTableButton.onClick.AddListener(OnPayTableClicked);
            view.menuButton.onClick.AddListener(OnMenuButtonClicked);
            view.fanPageButton.onClick.AddListener(OnFanPageClicked);
            view.contactUsButton.onClick.AddListener(OnContactUsClicked);

            SubscribeEvent<EventMusicEnabled>(OnMusicEnabled);
            SubscribeEvent<EventSoundEnabled>(OnSoundEnabled);
            SubscribeEvent<EventContactUsHasNewMessage>(OnEventContactUsHasNewMessage);
            
            var selectEventCustomHandler = view.transform.gameObject.AddComponent<SelectEventCustomHandler>();

            selectEventCustomHandler.BindingDeselectedAction(async (eventData) =>
            {
                await WaitNFrame(1);
                if (view.animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                {
                    if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(view.transform))
                    {
                        CloseMenu();
                    }
                }
            });
        }

        public void OnEventContactUsHasNewMessage(EventContactUsHasNewMessage evt)
        {
            view.redDotOnMenu.gameObject.SetActive(evt.hasNewMessage);
            view.redDotOnContactUs.gameObject.SetActive(evt.hasNewMessage);
        }
        
        protected void OnMusicEnabled(EventMusicEnabled musicEnabled)
        {
            
            view.musicButton.transform.Find("EnableState").gameObject.SetActive(musicEnabled.enabled);
            view.musicButton.transform.Find("DisableState").gameObject.SetActive(!musicEnabled.enabled);
        }

        protected void OnSoundEnabled(EventSoundEnabled soundEnabled)
        {
            view.soundButton.transform.Find("EnableState").gameObject.SetActive(soundEnabled.enabled);
            view.soundButton.transform.Find("DisableState").gameObject.SetActive(!soundEnabled.enabled);
        }

        protected void OnMenuButtonClicked()
        {
            SoundController.PlaySfx("General_DropdownWindow");
            ToggleMenuState();
        }

        protected void OnSoundClicked()
        {
            PreferenceController.SetSoundEnabled(!PreferenceController.IsSoundEnabled());
            EventSystem.current.SetSelectedGameObject(view.transform.gameObject);
            SoundController.PlayButtonClick();
        }

        protected void OnMusicClicked()
        {
            var enabled = PreferenceController.IsMusicEnabled();
            PreferenceController.SetMusicEnabled(!enabled);
            SoundController.PlayButtonClick();
            EventSystem.current.SetSelectedGameObject(view.transform.gameObject);
        }

        protected async void OnSettingClicked()
        {
            var popup = await PopupStack.ShowPopup<UISettingsPopup>();
            popup.Set();
            SoundController.PlayButtonClick();
            CloseMenu();
        }

        protected void OnPayTableClicked()
        {
            EventBus.Dispatch(new EventShowPayTable());
            SoundController.PlayButtonClick();
            CloseMenu();
        }

        protected void CloseMenu()
        {
            if (view.animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                view.animator.Play("Close");
            }
        }

        public void ToggleMenuState()
        {
            if (view.animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                view.animator.Play("Close");
            }
            else if (view.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                view.animator.Play("Open");
                EventSystem.current.SetSelectedGameObject(view.transform.gameObject);
            }
        }

        public void OnFanPageClicked()
        {
            Application.OpenURL(GlobalSetting.fanPageUrl);
            CloseMenu();
        }
        
        public void OnContactUsClicked()
        {
            PopupStack.ShowPopupNoWait<UIContactUsPopup>();
            
            Client.Get<ContactUsController>().ResetNewMessageState();
            view.redDotOnMenu.gameObject.SetActive(false);
            view.redDotOnContactUs.gameObject.SetActive(false);
            CloseMenu();
        }
    }
}
