// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/11:50
// Ver : 1.0.0
// Description : CardSetRewardPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIAlbumSetRewardPopup")]
    public class CardSetRewardPopup : Popup<CardSetRewardPopupViewController>
    {
        [ComponentBinder("ConfirmButton")] public Button confirmButton;

        [ComponentBinder("Gold")] public Text coinRewardText;
        [ComponentBinder("Root/Bonus/Icon")] public Image cardSetCover;

        public CardSetRewardPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1000, 768);
        }
    }
    
    public class CardSetRewardPopupViewController:ViewController<CardSetRewardPopup> 
    {
        protected AlbumController albumController;
        protected AssetReference cardSetAtlasRef;

        protected SAcceptCardReward.Types.ReceivedCardSetReward receivedCardSetReward;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            albumController = Client.Get<AlbumController>();
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
        

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            
            if (inExtraData is PopupArgs popupArgs)
            {
                receivedCardSetReward = popupArgs.extraArgs as SAcceptCardReward.Types.ReceivedCardSetReward;
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            var setId = receivedCardSetReward.SetId;

            view.cardSetCover.sprite = cardSetAtlasRef.GetAsset<SpriteAtlas>().GetSprite($"{setId}_Cover");

            var coinItem = XItemUtility.GetItem(receivedCardSetReward.Reward.Items,
                Item.Types.Type.Coin);

            view.coinRewardText.text = coinItem.Coin.Amount.GetCommaFormat();
            
            SoundController.PlaySfx("Album_Album_CardWin");
        }

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {albumController.GetAlbumSpriteAtlasName()};
            await base.LoadExtraAsyncAssets();
           
            cardSetAtlasRef = GetAssetReference(albumController.GetAlbumSpriteAtlasName());
        }

        public async void OnConfirmButtonClicked()
        {
            view.confirmButton.interactable = false;
            
            var coinItem = XItemUtility.GetItem(receivedCardSetReward.Reward.Items,
                Item.Types.Type.Coin);

            await XUtility.FlyCoins(view.confirmButton.transform, new EventBalanceUpdate(coinItem, "CardSetReward"));
            
            view.Close();
        }
    }
}