// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/12:23
// Ver : 1.0.0
// Description : TravelAlbumNearSeasonFinish.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIAdvertisingSeasonXCountdown")]
    [AlbumRuntimeUpdateAddress]
    public class TravelAlbumNearSeasonFinishPopup : Popup<TravelAlbumNearSeasonFinishPopupViewController>
    {
        [ComponentBinder("CompleteNow")] public Button completeNowButton;
        [ComponentBinder("SeeNewAlbum")] public Button seeNewAlbum;

        [ComponentBinder("Root/RotateAll/money/CountdownText")]
        public Text textCountDown;

        public TravelAlbumNearSeasonFinishPopup(string address)
            : base(address)
        {
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            
            AdaptScaleTransform(transform.Find("Root"), new Vector2(1200,768));
        }
    }

    public class TravelAlbumNearSeasonFinishPopupViewController : ViewController<TravelAlbumNearSeasonFinishPopup>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            albumController = Client.Get<AlbumController>();
         
            view.seeNewAlbum.gameObject.SetActive(false);
            view.textCountDown.transform.parent.gameObject.SetActive(true);
            view.completeNowButton.gameObject.SetActive(true);
            view.completeNowButton.onClick.AddListener(OnCompleteNowClicked);
           
            Update();
        }

        protected AlbumController albumController;

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(2);
        }
        protected void OnCompleteNowClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(AlbumCollectionPopup), "SeasonNearFinish")));
            view.Close();
        }

        public override void Update()
        {
            base.Update();
            var countDown = albumController.GetSeasonFinishCountDown();
            if (countDown > 0)
            {
                view.textCountDown.text = XUtility.GetTimeText(albumController.GetSeasonFinishCountDown(), true);
            }
            else
            {
                view.Close();
            }
        }
    }
}