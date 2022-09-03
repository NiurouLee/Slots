using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIWhatsUp")]
    public class UIWhatsUp : Popup
    {
        [ComponentBinder("Root/FaceBookGroup/OkGroup/OkButton")]
        public Button buttonFacebookOK;

        [ComponentBinder("Root/AppleGroup/CollectGroup/CollectButton")]
        public Button buttonAppleOK;

        [ComponentBinder("Root/FaceBookGroup")]
        public Transform transformFacebookGroup;

        [ComponentBinder("Root/AppleGroup")]
        public Transform transformAppleGroup;

        public UIWhatsUp(string address) : base(address)
        {
            contentDesignSize = new Vector2(1134, 653);
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            buttonFacebookOK.onClick.AddListener(OnOK);
            buttonAppleOK.onClick.AddListener(OnOK);
        }

        private void OnOK()
        {
            Close();
        }

        public void Set(string type)
        {
            switch (type)
            {
                case "facebook":
                    transformFacebookGroup.gameObject.SetActive(true);
                    transformAppleGroup.gameObject.SetActive(false);
                    break;
                case "apple":
                    transformFacebookGroup.gameObject.SetActive(false);
                    transformAppleGroup.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
