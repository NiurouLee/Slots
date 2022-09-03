using Dlugin;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    public class UIVIPEntranceView : View<UIVIPEntranceViewController>
    {
        [ComponentBinder("Icon")] private Image imgVipIcon;
        
        public  void RefreshVip()
        {
            var atlas = AssetHelper.GetResidentAsset<SpriteAtlas>("CommonUIAtlas");
            var vipController = Client.Get<VipController>();
            uint vipLevel = vipController.GetVipLevel();
            imgVipIcon.sprite = atlas.GetSprite($"UI_VIP_icon_{vipLevel}");
        }
    }

    public class UIVIPEntranceViewController : ViewController<UIVIPEntranceView>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            RefreshVip();

            var button = view.transform.GetComponent<Button>();
            button.onClick.AddListener(OnVipButtonClick);

            var content = view.transform.Find("Content");
            var pointerEventCustomHandler = view.transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown((eventData) => { content.localScale = Vector3.one * 0.95f; });
            pointerEventCustomHandler.BindingPointerUp((eventData) => { content.localScale = Vector3.one; });
        }
        
        protected void OnVipButtonClick()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(UIVIPMainView), (object)"Lobby")));
        }
        
        public void RefreshVip()
        {
            view.RefreshVip();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventOnVipLevelUp>(OnVipLevelUp);
        }

        private void OnVipLevelUp(EventOnVipLevelUp obj)
        {
            RefreshVip();
        }
    }
}