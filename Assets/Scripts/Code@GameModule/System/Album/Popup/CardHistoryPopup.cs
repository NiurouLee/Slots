// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/21/19:14
// Ver : 1.0.0
// Description : AlbumHistoryPopup.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule 
{
    public class CardHistoryItemView : View
    {
        [ComponentBinder("NameText")]
        public Text nameText;

        [ComponentBinder("PositionText")]
        public Text positionText;

        [ComponentBinder("TimeText")]
        public Text timeText;

        [ComponentBinder("Card")]
        public RectTransform card;

        public void SetUpCardHistoryView(Card carInfo,  SpriteAtlas spriteAtlas, ulong time, string source)
        {
            positionText.text = source;
            nameText.text = carInfo.Name;

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(time).ToLocalTime();
            timeText.text = dateTime.ToString("MM/dd/yyyy hh:mm tt");

            DateTimeOffset.FromUnixTimeMilliseconds((long) time * 1000);
            var cardView = AddChild<AlbumCardView>(card.GetChild(0));
            cardView.viewController.SetUpCard(carInfo, spriteAtlas);   
        }

    }
    
    [AssetAddress("UICardHistory")]
    public class CardHistoryPopup: Popup<CardHistoryPopupViewController>
    {
        [ComponentBinder("Root/CardHistoryView")]
        public ScrollRect cardHistoryView;

        [ComponentBinder("Root/Slider")]
        public Slider slider;

        [ComponentBinder("Root/CardHistoryView/Viewport/Content/Item")]
        public RectTransform item;

        [ComponentBinder("Root/CardHistoryView/Viewport/Content")]
        public RectTransform content;

        public CardHistoryPopup(string address)
            :base(address)
        {
        }
    }
    public class CardHistoryPopupViewController: ViewController<CardHistoryPopup>
    {
        protected AlbumController albumController;
        protected AssetReference cardSetAtlasRef;

        protected SGetCardGotRecords sGetCardGotRecords;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            albumController = Client.Get<AlbumController>();
        }
        
        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {albumController.GetAlbumSpriteAtlasName()};
            await base.LoadExtraAsyncAssets();
           
            cardSetAtlasRef = GetAssetReference(albumController.GetAlbumSpriteAtlasName());
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
          
            SubscribeEvent<EventAlbumSeasonEnd>((evt) =>
            {
                view.Close();
            });
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            
            sGetCardGotRecords  =inExtraAsyncData as SGetCardGotRecords;
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            view.slider.value = 0;
            view.cardHistoryView.onValueChanged.AddListener(OnScrollViewValueChanged);
            view.slider.onValueChanged.AddListener(OnSliderValueChanged);

            SetUpHistoryItem();

            AdataUI();
        }

        public void AdataUI()
        {
            var viewSize = ViewResolution.referenceResolutionLandscape;

            if (viewSize.x < 1100)
            {
                var scale = viewSize.x / 1100;
                view.cardHistoryView.transform.localScale =  new Vector3(scale, scale, scale);
                var rectTransform = (RectTransform)view.cardHistoryView.transform;
                var sizeDelta = rectTransform.sizeDelta;
                sizeDelta.y *= 1 / scale;
                rectTransform.sizeDelta = sizeDelta;
            }
        }

        public void SetUpHistoryItem()
        {
            int historyItemCount = sGetCardGotRecords.CardGotRecords.Count;

            var spriteAtlas = cardSetAtlasRef.GetAsset<SpriteAtlas>();
            for (var i = 0; i < historyItemCount; i++)
            {
                var record = sGetCardGotRecords.CardGotRecords[i];
                var item = GameObject.Instantiate(view.item.gameObject, view.content);
                var cardHistoryItemView = view.AddChild<CardHistoryItemView>(item.transform);
                cardHistoryItemView.SetUpCardHistoryView(record.Card, spriteAtlas, record.GotTime, record.Source);
            }
            
            view.item.gameObject.SetActive(false);
        }

        public void OnSliderValueChanged(float value)
        {
            if (Mathf.Abs(view.cardHistoryView.verticalNormalizedPosition - (1 - value)) > 1e-6)
            {
                view.cardHistoryView.verticalNormalizedPosition = (1 - value);
            }
        }
        
        public  void OnScrollViewValueChanged(Vector2 value)
        {
            if (Mathf.Abs(view.slider.value - (1 - value.y)) > 1e-6)
            {
                view.slider.value = (1 - value.y);
            }
        }
    }
}