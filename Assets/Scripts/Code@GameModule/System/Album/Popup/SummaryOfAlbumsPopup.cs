// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/13:57
// Ver : 1.0.0
// Description : SummaryOfAlbumsPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UISummaryOfAlbumPopup")]
    public class SummaryOfAlbumsPopup : Popup<SummaryOfAlbumsPopupViewController>
    {
        [ComponentBinder("ConfirmButton")] public Button confirmButton;
        [ComponentBinder("Root/Content/Content1/StarLayout")] public Transform normalCardLayout;
        [ComponentBinder("Root/Content/Content2/StarLayout")] public Transform goldCardLayout;
        [ComponentBinder("Root/Content/Content3/StarLayout")] public Transform luckyCardLayout;
          
        public SummaryOfAlbumsPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1200, 768);
        }
    }

    public class SummaryOfAlbumsPopupViewController : ViewController<SummaryOfAlbumsPopup>
    {
        protected AlbumController albumController;

        private List<CardUpdateInfo> _cardUpdateInfos;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            albumController = Client.Get<AlbumController>();
        }
       
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
          //  EnableUpdate(2);
          SetUpViewContent();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
           
            if (inExtraData is PopupArgs popupArgs)
            {
                _cardUpdateInfos = popupArgs.extraArgs as List<CardUpdateInfo>;
            }
        }

        public void SetUpViewContent()
        {
            List<int> goldenCount = new List<int>() {0, 0, 0, 0, 0};
            List<int> normalCount = new List<int>() {0, 0, 0, 0, 0};
            List<int> luckyCount = new List<int>() {0, 0, 0, 0, 0};

            if (_cardUpdateInfos != null)
            {
                for (var i = 0; i < _cardUpdateInfos.Count; i++)
                {
                    var cardAcquired = _cardUpdateInfos[i].cardAcquired;

                    for (var c = 0; c < cardAcquired.Count; c++)
                    {
                        if (cardAcquired[c].Type == Card.Types.CardType.Golden)
                        {
                            goldenCount[(int) cardAcquired[c].Star - 1]++;
                        }
                        else if (cardAcquired[c].Type == Card.Types.CardType.Normal)
                        {
                            normalCount[(int) cardAcquired[c].Star - 1]++;
                        }
                        else if (cardAcquired[c].Type == Card.Types.CardType.Lucky)
                        {
                            luckyCount[(int) cardAcquired[c].Star - 1]++;
                        }
                    }
                }
            }

            for (var i = 0; i < 5; i++)
            {
                var text = view.normalCardLayout.Find($"Star{i + 1}/Num").GetComponent<Text>();
                if (normalCount[i] > 0)
                    text.color = new Color(0.9f, 0.8f, 0.1f, 1);
                text.text = "x" + normalCount[i];
                
                text = view.luckyCardLayout.Find($"Star{i + 1}/Num").GetComponent<Text>();
                if (luckyCount[i] > 0)
                    text.color = new Color(0.9f, 1.0f, 0.1f, 1);
                text.text = "x" + luckyCount[i];
                
                text = view.goldCardLayout.Find($"Star{i + 1}/Num").GetComponent<Text>();
                if (goldenCount[i] > 0)
                    text.color = new Color(0.9f, 0.8f, 0.1f, 1);
                text.text = "x" + goldenCount[i];
            }
        }

        protected void OnConfirmClicked()
        {
            view.Close();
        }
    }
}