using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIValentinesDay2022Notice")]
    public class UIActivity_ValentinesDay_MainPopup : Popup<UIActivity_Valentine2022_MainPopupController>
    {
        [ComponentBinder("Root/BottomGroup/PlayButton")]
        public Button buttonGet;

        [ComponentBinder("Root/TopGroup/TimerGroup/TimerText")]
        public TMP_Text textTimer;

        public UIActivity_ValentinesDay_MainPopup(string address) : base(address)
        {
            // contentDesignSize = new Vector2(1024, 768);
        }
    }

    public class UIActivity_Valentine2022_MainPopupController : ViewController<UIActivity_ValentinesDay_MainPopup>
    {
        private Activity_ValentinesDay _activity;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            EnableUpdate(1);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonGet.onClick.AddListener(OnButtonGet);
        }

        private async void OnButtonGet()
        {
            view.buttonGet.interactable = false;
            // await _activity.PrepareMainPageData();
            // if (_activity.sGetValentinePayItemInfo == null) { return; }
            // if (_activity.sGetValentineMainPageInfo == null) { return; }
            await PopupStack.ShowPopup<UIActivity_ValentinesDay_MapPopup>();
            PopupStack.ClosePopup<UIActivity_ValentinesDay_MainPopup>();
            view.buttonGet.interactable = true;
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventValentinesdayEnter, ("source", "1"));
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            view.buttonGet.interactable = true;
            var popupArgs = extraData as PopupArgs;
            if (popupArgs != null)
            {
                _activity = popupArgs.extraArgs as Activity_ValentinesDay;
            }
            Refresh();
        }

        public override void Update()
        {
            base.Update();
            Refresh();
        }

        private void Refresh()
        {
            if (_activity == null) { return; }

            view.textTimer.text =
                XUtility.GetTimeText(XUtility.GetLeftTime((ulong) _activity.config.EndTimestamp * 1000));
        }
    }
}
