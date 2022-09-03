// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/12:12
// Ver : 1.0.0
// Description : TravelAlbumStartPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine.UI;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIAdvertisingSeasonXCountdown")]
    [AlbumRuntimeUpdateAddress]
    public class TravelAlbumStartPopup : Popup<ViewController>
    {
        [ComponentBinder("CompleteNow")] public Button completeNowButton;
        [ComponentBinder("SeeNewAlbum")] public Button seeNewAlbum;

        [ComponentBinder("Root/RotateAll/money/CountdownText")]
        public Text textCountDown;

        public TravelAlbumStartPopup(string address)
            : base(address)
        {
        }
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            seeNewAlbum.gameObject.SetActive(true);
            textCountDown.transform.parent.gameObject.SetActive(false);
            completeNowButton.gameObject.SetActive(false);
            seeNewAlbum.onClick.AddListener(OnSetNewAlbumClicked);

            Client.Get<AlbumController>().RestSetIsNewSeason();

            AdaptScaleTransform(transform.Find("Root"), new Vector2(1200, 768));

            var countDown = Client.Get<AlbumController>().GetSeasonFinishCountDown();

            if (countDown > 0)
            {
                viewController.WaitForSeconds(countDown, () => { Close(); });
            }
            
            
        }

        protected void OnSetNewAlbumClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(AlbumCollectionPopup), "SeasonStart")));
            Close();
        }
    }
}