// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/08/14:13
// Ver : 1.0.0
// Description : AlbumLobbyEntranceView.cs
// ChangeLog :
// **********************************************

using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    public class AlbumLobbyEntranceView:View<AlbumLobbyEntranceViewController>
    {
        [ComponentBinder("LockState")] 
        public Transform lockState;
        
        [ComponentBinder("ReminderGroup")] 
        public Transform transReminder;
        
        [ComponentBinder("Content/Icon")] 
        public Image icon;
        
        [ComponentBinder("Content/LockState/Icon")] 
        public Image lockIcon;
         
        [ComponentBinder("LobbyTextBubbleL")] 
        public Transform lockTips;

        public CommonTextBubbleView bubbleView;
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            bubbleView = AddChild<CommonTextBubbleView>(lockTips);
        }
    }

    public class AlbumLobbyEntranceViewController : ViewController<AlbumLobbyEntranceView>
    {
        private AlbumController _albumController;

        private AssetReference _assetReference;

        public override void OnViewEnabled()
        {
            _albumController = Client.Get<AlbumController>();
            base.OnViewEnabled();

            UpdateLockState();
            
            UpdateRedDotNumber();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventUpdateAlbumRedDotReminder>(UpdateAlbumRedDotReminder);
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventSeasonEnd);
        }

        protected  void UpdateLockState()
        {
            if (!_albumController.IsOpen() || !_albumController.IsUnlocked())
            {
                view.lockState.gameObject.SetActive(true);
            }
            else
            {
                view.lockState.gameObject.SetActive(false);
            }
          
        }
        
        protected void UpdateAlbumRedDotReminder(EventUpdateAlbumRedDotReminder evt)
        {
            UpdateRedDotNumber();
        }

        protected void OnEventSeasonEnd(EventAlbumSeasonEnd evt)
        {
            UpdateLockState();
            UpdateRedDotNumber();
        }
         
        protected void UpdateRedDotNumber()
        {
            var redDotNumber = _albumController.GetLobbyAlbumEntranceRedDotNumber();
         
            if (redDotNumber <= 0)
            {
                view.transReminder.gameObject.SetActive(false);
            }
            else
            {
                view.transReminder.gameObject.SetActive(true);
                var tmpText = view.transReminder.Find("NoticeText").GetComponent<TMP_Text>();
                tmpText.text = redDotNumber.ToString();
            }
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            var button = view.transform.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnEntranceButtonClicked);
            
            var seasonId = Client.Get<AlbumController>().GetSeasonId();

            if (seasonId > 0)
            {
                _assetReference = AssetHelper.PrepareAsset<SpriteAtlas>($"AlbumLobbyEntranceIconSeason{seasonId}Atlas", (assetReference) =>
                {
                    if (assetReference != null)
                    {
                        var iconSpriteAtlas = assetReference.GetAsset<SpriteAtlas>();
                        view.icon.sprite = iconSpriteAtlas.GetSprite($"UI_lobby_Album_Season{seasonId}");
                        view.lockIcon.sprite = iconSpriteAtlas.GetSprite($"UI_lobby_Album_Season{seasonId}_Lock");
                    }
                });
            }
            

            var content = view.transform.Find("Content");
            var pointerEventCustomHandler = view.transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown((eventData) => { content.localScale = Vector3.one * 0.95f; });
            pointerEventCustomHandler.BindingPointerUp((eventData) => { content.localScale = Vector3.one; });
        }

        public void OnEntranceButtonClicked()
        {
            SoundController.PlayButtonClick();

            if (!_albumController.IsOpen())
            {
                view.bubbleView.SetText($"Coming soon");
                view.bubbleView.ShowBubble(3);
            }
            else if (!_albumController.IsUnlocked())
            {
                view.bubbleView.SetText($"Unlock At LEVEL {_albumController.GetUnlockLevel()}");
                view.bubbleView.ShowBubble(3);
            }
            else
            {
               // PopupStack.ShowPopupNoWait<TravelAlbumStartPopup>();
               // PopupStack.ShowPopupNoWait<TravelAlbumNearSeasonFinishPopup>();
                
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(AlbumCollectionPopup), "Lobby")));
            }
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            
            if (_assetReference != null)
            {
                _assetReference.ReleaseOperation();
            }
        }
    }
}