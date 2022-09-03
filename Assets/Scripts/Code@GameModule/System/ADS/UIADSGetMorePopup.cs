using System;
using TMPro;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIADSCollectRewardGetMore", "UIADSCollectRewardGetMoreV")]
    public class UIADSGetMorePopup : Popup
    {
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        public Button buttonCollect;

        [ComponentBinder("Root/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        private Action _onClick;

        public UIADSGetMorePopup(string address) : base(address) { }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            if (buttonCollect != null)
            {
                buttonCollect.onClick.AddListener(OnClick);
            }
        }

        public void Set(ulong count, Action onClick)
        {
            if (tmpTextIntegral != null)
            {
                tmpTextIntegral.text = count.GetCommaFormat();
            }

            _onClick = onClick;

            buttonCollect.interactable = true;
        }

        private void OnClick()
        {
            _onClick?.Invoke();

            _onClick = null;

            buttonCollect.interactable = false;
        }
    }
}
