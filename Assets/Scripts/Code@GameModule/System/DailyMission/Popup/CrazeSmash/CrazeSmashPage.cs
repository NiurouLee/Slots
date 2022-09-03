
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class CrazeSmashPage : View<CrazeSmashPageController>
    {
        [ComponentBinder("TopGroup/InformationButton")]
        public Button buttonInfo;

        [ComponentBinder("MainGroup")]
        public Transform transformMain;

        [ComponentBinder("CrazeSmashGameSilverEgg")]
        public Transform transformSilverGame;

        [ComponentBinder("CrazeSmashGameGoldenEgg")]
        public Transform transformGoldGame;

        [ComponentBinder("Blocker")]
        public Transform transformBlocker;

        public UICrazeSmashMain main;
        public UICrazeSmashGame_Silver silverGame;
        public UICrazeSmashGame_Gold goldGame;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            main = AddChild<UICrazeSmashMain>(transformMain);
            silverGame = AddChild<UICrazeSmashGame_Silver>(transformSilverGame);
            goldGame = AddChild<UICrazeSmashGame_Gold>(transformGoldGame);
            transformMain.gameObject.SetActive(false);
            transformSilverGame.gameObject.SetActive(false);
            transformGoldGame.gameObject.SetActive(false);
            main.silverGroup.buttonPlay.onClick.AddListener(SetToSilverGame);
            main.goldGroup.buttonPlay.onClick.AddListener(SetToGoldGame);
            silverGame.buttonBack.onClick.AddListener(SetToMain);
            goldGame.buttonBack.onClick.AddListener(SetToMain);
        }

        private async void ShowBlocker()
        {
            transformBlocker.gameObject.SetActive(true);
            await XUtility.WaitSeconds(1.0f, viewController);
            transformBlocker.gameObject.SetActive(false);
        }

        public void SetToMain()
        {
            Client.Get<CrazeSmashController>().canRefreshEggInfo = true;
            transformMain.gameObject.SetActive(true);
            transformSilverGame.gameObject.SetActive(false);
            transformGoldGame.gameObject.SetActive(false);
            Set();
            transformBlocker.gameObject.SetActive(false);
        }

        public void SetToSilverGame()
        {
            transformMain.gameObject.SetActive(false);
            transformSilverGame.gameObject.SetActive(true);
            transformGoldGame.gameObject.SetActive(false);
            var controller = Client.Get<CrazeSmashController>();
            controller.playGoldGame = false;
            controller.canRefreshEggInfo = false;
            Set();
            ShowBlocker();
        }

        public void SetToGoldGame()
        {
            transformMain.gameObject.SetActive(false);
            transformSilverGame.gameObject.SetActive(false);
            transformGoldGame.gameObject.SetActive(true);
            var controller = Client.Get<CrazeSmashController>();
            controller.canRefreshEggInfo = false;
            controller.playGoldGame = true;
            Set();
            ShowBlocker();
        }

        public void RefreshTime() { main?.RefreshTime(); }

        public void Refresh()
        {
            if (transformMain.gameObject.activeSelf == true) { main?.Refresh(); }

            if (transformSilverGame.gameObject.activeSelf == true) { silverGame?.Refresh(); }

            if (transformGoldGame.gameObject.activeSelf == true) { goldGame?.Refresh(); }
        }

        public void Set()
        {
            if (transformMain.gameObject.activeSelf == true) { main?.Refresh(); }

            if (transformSilverGame.gameObject.activeSelf == true) { silverGame?.Set(); }

            if (transformGoldGame.gameObject.activeSelf == true) { goldGame?.Set(); }
        }
    }


    public class CrazeSmashPageController : ViewController<CrazeSmashPage>
    {
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            view.SetToMain();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonInfo.onClick.AddListener(async () =>
            {
                await PopupStack.ShowPopup<UICrazeSmashHelpPopup>();
            });

            SubscribeEvent<Event_CrazeSmash_GameFinish>(OnEvent_CrazeSmash_GameFinish);
            SubscribeEvent<Event_CrazeSmash_EggInfoChanged>(OnEvent_CrazeSmash_EggInfoChanged);
            SubscribeEvent<Event_CrazeSmash_Expire>(OnEvent_CrazeSmash_Expire);
        }

        private void OnEvent_CrazeSmash_Expire(Event_CrazeSmash_Expire obj)
        {
            view.SetToMain();
        }

        private void OnEvent_CrazeSmash_EggInfoChanged(Event_CrazeSmash_EggInfoChanged obj)
        {
            view.Refresh();
        }

        private void OnEvent_CrazeSmash_GameFinish(Event_CrazeSmash_GameFinish obj)
        {
            view.SetToMain();
        }
    }
}