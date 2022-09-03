// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/01/18:16
// Ver : 1.0.0
// Description : AlbumSeasonEndPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;
namespace GameModule 
{
    [AssetAddress("UIAlbumSeasonXSeasonEndH")]
    [AlbumRuntimeUpdateAddress]
    public class AlbumSeasonEndHPopup: Popup<AlbumSeasonEndHPopupViewController>
    {
        [ComponentBinder("Root/BottomGroup/AwesomeButton")]
        public Button awesomeButton;

        public AlbumSeasonEndHPopup(string address)
            :base(address)
        {
            //viewAssetAddress = Client.Get<AlbumController>().GetCurrentSeasonAssetAddress(address);
            contentDesignSize = new Vector2(1100, 768);
        }
    }
    public class AlbumSeasonEndHPopupViewController: ViewController<AlbumSeasonEndHPopup>
    {
        protected override void SubscribeEvents()
        {
            view.awesomeButton.onClick.AddListener(OnAwesomeButtonClicked);
        }
        public void OnAwesomeButtonClicked()
        {
            view.Close();
        }
    }
}