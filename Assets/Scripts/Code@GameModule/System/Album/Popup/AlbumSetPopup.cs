// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/08/14:12
// Ver : 1.0.0
// Description : AlbumSetPopup.cs
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
    [AssetAddress("UIAlbumSetPopup")]
    public class AlbumSetPopup:Popup<AlbumSetPopupViewController>
    {
        [ComponentBinder("Root/CardContent/Viewport/Content")]
        public Transform cardContainer;

        
        [ComponentBinder("Root")]
        public Transform root;
        
        [ComponentBinder("AlbumSetCover")]
        public Image albumSetCover;  
        
        [ComponentBinder("SetCompletedimg")]
        public Transform setCompletedImg; 
        
        [ComponentBinder("SetWin")]
        public Transform setWin;
        
        [ComponentBinder("AlbumSetWin")]
        public Text albumSetWinText;

        [ComponentBinder("ToggleLeft")] public Button toggleLeft;
        
        [ComponentBinder("ToggleRight")] public Button toggleRight;

        public List<AlbumCardView> cardViews;

        public AlbumSetPopup(string address)
            : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            var childCount = cardContainer.childCount;

            cardViews = new List<AlbumCardView>(10);
            for (int i = 0; i < childCount; i++)
            {
                var child = cardContainer.GetChild(i);

                var cardView = AddChild<AlbumCardView>(child);
                cardViews.Add(cardView);
            }
            
            AdaptScaleTransform(root, ViewResolution.designSize);
        }

        public override float GetPopUpMaskAlpha()
        {
            return 0;
        }
        
    }

    public class AlbumSetPopupViewController : ViewController<AlbumSetPopup>
    {
        protected AlbumController albumController;
        protected AssetReference cardSetAtlasRef;

        protected int currentCardSetIndex = 0;
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            albumController = Client.Get<AlbumController>();
            
            view.toggleLeft.onClick.AddListener(OnToggleLeftClicked);
            view.toggleRight.onClick.AddListener(OnToggleRightClicked);
        }

        public override void OnViewEnabled()
        {
            if (albumController.GetGuideStep() == 2)
            {
                ShowGuideStep3();
            }
            else if (albumController.GetGuideStep() == 3)
            {
                ShowGuideStep4();
            }

            base.OnViewEnabled();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        protected void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();
        }

        public async void ShowGuideStep3()
        {
            var guideStep1View = await View.CreateView<AlbumGuideStepView>("UITravelAlbumGuide_Step3", view.root);
            
            var canvas = view.cardViews[1].transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 5;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            guideStep1View.transform.SetAsLastSibling();
            guideStep1View.SetGuideClickHandler(()=>
            {
                GameObject.Destroy(canvas);
                albumController.IncreaseGuideStep(null);
                ShowGuideStep4();
            });
        }

        public async void ShowGuideStep4()
        {
            var maskStep4 = new GameObject("MaskStep4");
            var rectTransform = maskStep4.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(3000, 3000);
            maskStep4.transform.SetParent(view.root, false);
            maskStep4.transform.SetSiblingIndex(3);
            var image = maskStep4.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.7f);

            var guideStep4View = await View.CreateView<AlbumGuideStepView>("UITravelAlbumGuide_Step4", view.root);
            guideStep4View.SetGuideClickHandler(() =>
            {
                albumController.IncreaseGuideStep(null);
                EventBus.Dispatch(new EventOnShowAlbumGuide4Finished());
                view.Close();
            });
        }
        protected void OnToggleLeftClicked()
        {
            SoundController.PlayButtonClick();
            DoTransition(currentCardSetIndex - 1);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionAlbumCardcheck, ("Operation", "ToggleLeft"));
        }

        protected void DoTransition(int cardSetIndex)
        {
            view.toggleLeft.interactable = false;
            view.toggleRight.interactable = false;
            view.closeButton.interactable = false;

            var animator = view.transform.GetComponent<Animator>();
            animator.Play("Next", 0, 0);
            
            WaitNFrame(4, () =>
            {
                SetUpCardViews(cardSetIndex,false);
            });

            WaitNFrame(35, UpdateButtonState);
        }
        
        public void UpdateButtonState()
        {
            view.toggleRight.gameObject.SetActive(currentCardSetIndex < albumController.GetCardSetCount() - 1);
            view.toggleLeft.gameObject.SetActive(currentCardSetIndex > 0);
            
            view.toggleLeft.interactable = true;
            view.toggleRight.interactable = true;
            view.closeButton.interactable = true;
        }
        
        protected void OnToggleRightClicked()
        {
            SoundController.PlayButtonClick();
            DoTransition(currentCardSetIndex + 1);
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionAlbumCardcheck, ("Operation", "ToggleRight"));

        }
        
        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {albumController.GetAlbumSpriteAtlasName()};
            await base.LoadExtraAsyncAssets();
           
            cardSetAtlasRef = GetAssetReference(albumController.GetAlbumSpriteAtlasName());
        }

        public void SetUpCardViews(int cardSetIndex, bool updateButtonState = true)
        {
            var cardSetInfo = albumController.GetCardSetInfoByIndex(cardSetIndex);
            currentCardSetIndex = cardSetIndex;

            if (cardSetInfo != null)
            {
                var albumSpriteAtlas = cardSetAtlasRef.GetAsset<SpriteAtlas>();
             
                for (var i = 0; i < cardSetInfo.Cards.Count; i++)
                {
                    var card = cardSetInfo.Cards[i];
                    view.cardViews[i].viewController.SetUpCard(card, albumSpriteAtlas,true,true,true);
                }

                if (cardSetInfo.RewardForCollectAllStat == CardsRewardStat.Received)
                {
                    view.setCompletedImg.gameObject.SetActive(true);
                    view.setWin.gameObject.SetActive(false);
                }
                else
                {
                    view.setCompletedImg.gameObject.SetActive(false);
                    view.setWin.gameObject.SetActive(true);

                    var item = XItemUtility.GetItem(cardSetInfo.RewardForCollectAll.Items, Item.Types.Type.Coin);
                    if (item != null)
                    {
                        view.albumSetWinText.text = item.Coin.Amount.GetCommaFormat();
                    }
                }
                
                view.albumSetCover.sprite = albumSpriteAtlas.GetSprite($"{cardSetInfo.SetId}_Cover");
                
                albumController.ResetNewTag(cardSetInfo.SetId);
            }

            if (updateButtonState)
            {
                UpdateButtonState();
            }
        }
    }
}