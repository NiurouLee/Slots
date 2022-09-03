// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/27/17:21
// Ver : 1.0.0
// Description : UIGuaranteedPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIGuaranteed")]
    public class UIBankruptPopup:Popup<UIBankruptPopupController>
    {
        [ComponentBinder("CoinRewardText")] 
        public TextMeshProUGUI coinRewardText;
        
        [ComponentBinder("CollectButton")]
        public Button collectButton;
        
        [ComponentBinder("BuyMoreButton")]
        public Button buyMoreButton; 
        
        [ComponentBinder("Root/MainGroup/CoinRewardGroup/Icon")]
        public Transform coinIcon;


        public UIBankruptPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1100,768);
        }
    }

    public class UIBankruptPopupController : ViewController<UIBankruptPopup>
    {
        private SBankrupt _sBankrupt;
        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            _sBankrupt = inExtraAsyncData as SBankrupt;
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.collectButton.onClick.AddListener(OnCollectButtonClicked);
            view.buyMoreButton.onClick.AddListener(OnBuyButtonClicked);

            if (_sBankrupt.Coins > 0)
            {
                view.coinRewardText.text = _sBankrupt.Coins.GetCommaFormat();
            }
        }

        protected async void OnCollectButtonClicked()
        {
            view.collectButton.interactable = false;
            view.buyMoreButton.interactable = false;
            await XUtility.FlyCoins(view.coinIcon, new EventBalanceUpdate(_sBankrupt.Coins, "BankruptGift"));
            view.Close();
        }
        
        protected async void OnBuyButtonClicked()
        {
            view.collectButton.interactable = false;
            view.buyMoreButton.interactable = false;
            await XUtility.FlyCoins(view.coinIcon, new EventBalanceUpdate(_sBankrupt.Coins, "BankruptGift"));
            
            view.Close();
            
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "Bankrupt")));
        }
    }
}