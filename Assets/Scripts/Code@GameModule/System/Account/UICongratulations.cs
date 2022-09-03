using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICongratulations")]
    public class UICongratulations : Popup
    {
        [ComponentBinder("Root/FaceBookGroup")]
        public Transform transformFacebook;
        [ComponentBinder("Root/FaceBookGroup/CollectGroup/CollectButton")]
        public Button buttonCollectFacebook;

        [ComponentBinder("Root/FaceBookGroup/MainGroup/Integral/IntegralText")]
        public TMP_Text textCountFacebook;


        [ComponentBinder("Root/AppleLoginGroup")]
        public Transform transformApple;

        [ComponentBinder("Root/AppleLoginGroup/CollectGroup/CollectButton")]
        public Button buttonCollectApple;

        [ComponentBinder("Root/AppleLoginGroup/MainGroup/Integral/IntegralText")]
        public TMP_Text textCountApple;

        private long _coin;

        private UserProfile _profile;

        public UICongratulations(string address) : base(address)
        {
            contentDesignSize = new Vector2(1134, 653);
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            buttonCollectFacebook.onClick.AddListener(OnButtonFacebookCollectClick);
            buttonCollectApple.onClick.AddListener(OnButtonAppleCollectClick);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            buttonCollectFacebook.interactable = true;
            buttonCollectApple.interactable = true;
        }

        private async void OnButtonFacebookCollectClick()
        {
            buttonCollectFacebook.interactable = false;

            if (_profile != null)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(_profile));
                await XUtility.FlyCoins(buttonCollectFacebook.transform, new EventBalanceUpdate(_coin, "ConnectToFacebook"));
                buttonCollectFacebook.interactable = true;
            }

            Close();
        }

        private async void OnButtonAppleCollectClick()
        {
            buttonCollectApple.interactable = false;

            if (_profile != null)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(_profile));
                await XUtility.FlyCoins(buttonCollectApple.transform, new EventBalanceUpdate(_coin, "ConnectToApple"));
                buttonCollectApple.interactable = true;
            }

            Close();
        }

        public void Set(RepeatedField<Reward> rewards, UserProfile profile, string type)
        {
            _coin = 0;
            if (rewards != null && rewards.count > 0)
            {
                var reward = rewards[0];
                var coinItem = XItemUtility.GetCoinItem(reward);
                if (coinItem != null && coinItem.Coin != null)
                {
                    _coin = (long)coinItem.Coin.Amount;
                }
            }

            _profile = profile;

            switch (type)
            {
                case "apple":
                    transformApple.gameObject.SetActive(true);
                    transformFacebook.gameObject.SetActive(false);
                    textCountApple.text = _coin.GetCommaFormat();
                    break;
                case "facebook":
                    transformApple.gameObject.SetActive(false);
                    transformFacebook.gameObject.SetActive(true);
                    textCountFacebook.text = _coin.GetCommaFormat();
                    break;
            }
        }
    }
}
