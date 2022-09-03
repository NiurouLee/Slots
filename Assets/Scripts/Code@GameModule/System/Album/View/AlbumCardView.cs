// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/08/14:11
// Ver : 1.0.0
// Description : AlbumCardView.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    public class AlbumCardView : View<AlbumCardViewController>
    {
        [ComponentBinder("CardBg/Normal")] public Transform normalCardBg;
        [ComponentBinder("CardBg/Lucky")] public Transform luckyCardBg;
        [ComponentBinder("CardBg/Golden")] public Transform goldenCardBg;
        
        [ComponentBinder("Content/Normal")] public Transform normalCardContent;
        [ComponentBinder("Content/Lucky")] public Transform luckyCardContent;
        [ComponentBinder("Content/Golden")] public Transform goldenCardContent;
        
        [ComponentBinder("StarGroup")] public Transform starGroup;

        [ComponentBinder("ExtraInfo")] public Transform extraInfo;

        [ComponentBinder("Button")] public Button button;
    }
    public class AlbumCardViewController : ViewController<AlbumCardView>
    {
        private Card _card;

        private bool _showNew = false;
        private bool _showCount = false;
        
        protected SpriteAtlas _albumCardAtlas;
        
        public void SetUpCard(Card card, SpriteAtlas albumCardAtlas, bool clickable = false, bool inShowNew = false, bool inShowCount = false)
        {
            _card = card;
            _albumCardAtlas = albumCardAtlas;
            _showNew = inShowNew;
            _showCount = inShowCount;
             
            SetUpCardBg();
            SetUpCardContent();
            SetUpCardStarGroup();
            SetUpCardExtraInfo();
            
            view.button.onClick.RemoveAllListeners();
            
            if(clickable)
                view.button.onClick.AddListener(OnCardClicked);
        }

        protected async void OnCardClicked()
        {
           var popup = await PopupStack.ShowPopup<CardInfoPopup>();
           popup.SetUpCardInfoPopup(_card,_albumCardAtlas);

           _showNew = false;
           SetUpCardExtraInfo();
        }

        protected void SetUpCardBg()
        {
            view.goldenCardBg.gameObject.SetActive(_card.Type == Card.Types.CardType.Golden);
            view.luckyCardBg.gameObject.SetActive(_card.Type == Card.Types.CardType.Lucky);
            view.normalCardBg.gameObject.SetActive(_card.Type == Card.Types.CardType.Normal);
            
            Transform cardBgTransform = null;
            switch (_card.Type)
            {
                case Card.Types.CardType.Golden:
                    cardBgTransform = view.goldenCardBg;
                    break;
                case Card.Types.CardType.Lucky:
                    cardBgTransform = view.luckyCardBg;
                    break;
                case Card.Types.CardType.Normal:
                    cardBgTransform = view.normalCardBg;
                    break;
            }

            if (cardBgTransform != null)
            {
                cardBgTransform.Find("Inactive").gameObject.SetActive(_card.Count == 0);
                cardBgTransform.Find("Active").gameObject.SetActive(_card.Count > 0);
            }
        }

        protected void SetUpCardContent()
        {
            view.goldenCardContent.gameObject.SetActive(_card.Type == Card.Types.CardType.Golden);
            view.luckyCardContent.gameObject.SetActive(_card.Type == Card.Types.CardType.Lucky);
            view.normalCardContent.gameObject.SetActive(_card.Type == Card.Types.CardType.Normal);

            Transform contentTransform = null;
            string postFix = "Normal";
            switch (_card.Type)
            {
                case Card.Types.CardType.Golden:
                    contentTransform = view.goldenCardContent;
                    postFix = "Gold";
                    break;
                case Card.Types.CardType.Lucky:
                    contentTransform = view.luckyCardContent;
                    postFix = "Lucky";
                    break;
                case Card.Types.CardType.Normal:
                    contentTransform = view.normalCardContent;
                    break;
            }

            if (contentTransform != null)
            {
                var iconInactive = contentTransform.Find("IconInActive");
                iconInactive.gameObject.SetActive(_card.Count == 0);
                iconInactive.GetComponent<Image>().sprite =  _albumCardAtlas.GetSprite($"{_card.CardId/100}_{postFix}");
               
                contentTransform.Find("IconActive").gameObject.SetActive(_card.Count > 0);
                contentTransform.Find("IconActive").GetComponent<Image>().sprite =
                    _albumCardAtlas.GetSprite($"{_card.CardId}");
                var nameInActive = contentTransform.Find("NameInActive");
                var nameActive = contentTransform.Find("NameActive");
                nameInActive.gameObject.SetActive(_card.Count == 0);
                nameActive.gameObject.SetActive(_card.Count > 0);

                nameActive.GetComponent<Text>().text = _card.Name;
                nameInActive.GetComponent<Text>().text = _card.Name;
            }
        }

        protected void SetUpCardStarGroup()
        {
            var starCount = _card.Star;
            var childCount = view.starGroup.childCount;

            for (var i = 1; i <= childCount; i++)
            {
                var star = view.starGroup.Find($"Star{i}");
                
                star.Find("Active").gameObject.SetActive(_card.Count > 0);
                star.Find("Inactive").gameObject.SetActive(_card.Count == 0);
                
                star.gameObject.SetActive(starCount >= i);
            }
        }

        public void UpdateUseCardNum(int usedCount)
        {
            var cardCountText = view.extraInfo.Find("NumBg/Count").GetComponent<Text>();
            var currentCount = (_card.Count - usedCount - 1);
            cardCountText.text = "+" + currentCount;
            view.extraInfo.Find("NumBg").gameObject.SetActive(currentCount > 0 && _showCount);
        }

        protected void SetUpCardExtraInfo()
        {
            view.extraInfo.gameObject.SetActive(_showNew || _showCount);

            if (_showNew || _showCount)
            {
                var cardCount = _card.Count;
                view.extraInfo.Find("NumBg").gameObject.SetActive(cardCount > 1 && _showCount);

                if (cardCount > 1 && _showCount)
                {
                    var cardCountText = view.extraInfo.Find("NumBg/Count").GetComponent<Text>();
                    cardCountText.text = "+" + (cardCount - 1);
                }

                if (_showCount)
                {
                    view.extraInfo.Find("New").gameObject.SetActive(_card.IsNew  && _showNew);
                }
                else
                {
                    view.extraInfo.Find("New").gameObject.SetActive(_card.IsNew && _card.Count == 1 && _showNew);   
                }
            }
        }
    }
}