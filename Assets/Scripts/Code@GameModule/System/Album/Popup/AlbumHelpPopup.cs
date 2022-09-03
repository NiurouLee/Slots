// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/21/19:14
// Ver : 1.0.0
// Description : AlbumHelpPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("TravelAlbumSeasonXHelp")]
    [AlbumRuntimeUpdateAddress]
    public class TravelAlbumHelpPopup : Popup<TravelAlbumHelpPopupViewController>
    {
        [ComponentBinder("Root/MainGroup")] public RectTransform mainGroup;

        [ComponentBinder("Root/ToggleGroup/BtnLeft")]
        public Button btnLeft;

        [ComponentBinder("Root/ToggleGroup/BtnRight")]
        public Button btnRight;

        public TravelAlbumHelpPopup(string address)
            : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
    }

    public class TravelAlbumHelpPopupViewController : ViewController<TravelAlbumHelpPopup>
    {
        protected int currentIndex = 0;

        protected List<Transform> pages;

        protected override void SubscribeEvents()
        {
            view.btnLeft.onClick.AddListener(OnBtnLeftClicked);
            view.btnRight.onClick.AddListener(OnBtnRightClicked);
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        public void ShowPage(int pageIndex)
        {
            for (var i = 0; i < pages.Count; i++)
            {
                pages[i].gameObject.SetActive(i == pageIndex);
            }

            view.btnLeft.gameObject.SetActive(false);
            view.btnRight.gameObject.SetActive(false);
        }
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            var childCount = view.mainGroup.childCount;
            pages = new List<Transform>();

            for (var i = 0; i < childCount; i++)
            {
                var page = view.mainGroup.GetChild(i);
                pages.Add(page);
                pages[i].gameObject.SetActive(i == 0);
            }
            

            view.btnLeft.gameObject.SetActive(false);
            view.btnRight.gameObject.SetActive(true);
        }

        public void OnBtnRightClicked()
        {
            if (currentIndex < pages.Count - 1)
            {
                currentIndex++;
                pages[currentIndex - 1].gameObject.SetActive(false);
                pages[currentIndex].gameObject.SetActive(true);
 
                view.btnRight.gameObject.SetActive(currentIndex < pages.Count - 1);
                view.btnLeft.gameObject.SetActive(true);
            }
        }

        public void OnBtnLeftClicked()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                pages[currentIndex + 1].gameObject.SetActive(false);
                pages[currentIndex].gameObject.SetActive(true);
 
                view.btnLeft.gameObject.SetActive(currentIndex > 0);
                view.btnRight.gameObject.SetActive(true);
            }
        }
        
        protected void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();
        }
    }
}