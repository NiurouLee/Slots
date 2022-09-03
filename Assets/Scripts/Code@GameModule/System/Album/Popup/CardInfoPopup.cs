// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/10:45
// Ver : 1.0.0
// Description : CardInfoPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.U2D;

namespace GameModule
{
    [AssetAddress("UICardInfoPopup")]
    public class CardInfoPopup:Popup<ViewController>
    {
        [ComponentBinder("Root/Card/Card")]
        public Transform cardView;   
        
        [ComponentBinder("Root/TopGroup/HowToGetIt")]
        public Transform title;
        
        [ComponentBinder("Root/InactiveDesc")]
        public Transform inactiveDesc;
        
        [ComponentBinder("Root/ActiveDesc")]
        public Transform activeDesc;

        public AlbumCardView albumCardView;

        public CardInfoPopup(string address)
        :base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            albumCardView = AddChild<AlbumCardView>(cardView);

            viewController.SubscribeEvent<EventAlbumSeasonEnd>((evt) =>
            {
                this.Close();
            });
        }

        public void SetUpCardInfoPopup(Card cardInfo, SpriteAtlas spriteAtlas)
        {
            albumCardView.viewController.SetUpCard(cardInfo, spriteAtlas, false, false);

            activeDesc.gameObject.SetActive(cardInfo.Count > 0);
            inactiveDesc.gameObject.SetActive(cardInfo.Count == 0);
            title.gameObject.SetActive(cardInfo.Count == 0);
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionAlbumCardcheck, 
                ("Operation", "CardInfo"),("Status", cardInfo.Count > 0?"Get":"NotGet"),("CardId", cardInfo.CardId.ToString()));

        }
    }
}