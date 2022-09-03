// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/12:32
// Ver : 1.0.0
// Description : CardSetAllCompleteReward.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIAllAlbumSeasonXCompleteRewardPopup")]
    [AlbumRuntimeUpdateAddress]
    public class CardSetAllCompleteRewardPopup : Popup<CardSetAllCompleteRewardPopupViewController>
    {
        [ComponentBinder("Root/Money/Bonus/Gold")]
        public Text coinRewardText;

        [ComponentBinder("CollectNowButton")]
        public Button collectNowButton;

        public CardSetAllCompleteRewardPopup(string address)
            : base(address)
        {
        }
    }

    public class CardSetAllCompleteRewardPopupViewController : ViewController<CardSetAllCompleteRewardPopup>
    {
        protected RepeatedField<Reward> completeAllSetReward;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);


            if (inExtraData is PopupArgs popupArgs)
            {
                completeAllSetReward = popupArgs.extraArgs as RepeatedField<Reward>;
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            var coinItem = XItemUtility.GetItem(completeAllSetReward[0].Items,
                Item.Types.Type.Coin);

            view.coinRewardText.text = coinItem.Coin.Amount.GetCommaFormat();
            
            SoundController.PlaySfx("Album_All_Set_Win");
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.collectNowButton.onClick.AddListener(OnCollectClicked);
        }

        public async void OnCollectClicked()
        {
            var coinItem = XItemUtility.GetItem(completeAllSetReward[0].Items,
                Item.Types.Type.Coin);

            await XUtility.FlyCoins(view.collectNowButton.transform, new EventBalanceUpdate(coinItem, "CardSetReward"));

            view.Close();
        }
    }
}