// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/16/20:12
// Ver : 1.0.0
// Description : BannerPageView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class PageView : View
    {
        public AssetReference bannerAssetReference;

        protected Advertisement holdAdvertisement;

        protected GameObject attachBannerGameObject;

        [ComponentBinder("Loading")] 
        protected Transform loading;
         
        protected Button button;
        

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnButtonClicked);
        }

        
        public void OnButtonClicked()
        {
            SoundController.PlayButtonClick();
            Client.Get<BannerController>().ResponseAdvertisement(holdAdvertisement);
        }
        
        public void SetUpContent(Advertisement advertisement)
        {
            holdAdvertisement = advertisement;

            CleanUpContent();
            
            LoadBannerAsset(() =>
            {
                if (advertisement != holdAdvertisement)
                    return;

                if (transform == null)
                {
                    return;
                }
                 
                loading.gameObject.SetActive(false);
            
                var bannerGameObject = bannerAssetReference.InstantiateAsset<GameObject>();
                bannerGameObject.transform.SetParent(transform, false);
                
                Client.Get<BannerController>().UpdateBannerContent(holdAdvertisement,this, bannerGameObject.transform);
            });
        }
        
        public void CleanUpContent()
        {
            if (attachBannerGameObject != null)
            {
                GameObject.Destroy(attachBannerGameObject);
            }
            
            loading.gameObject.SetActive(true);
        }

        public void LoadBannerAsset(Action finishCallback)
        {
            if (bannerAssetReference != null)
            {
                bannerAssetReference.ReleaseOperation();
                bannerAssetReference = null;
            }

            if (holdAdvertisement.Advertisement_ != null)
            {
                bannerAssetReference = AssetHelper.PrepareAsset<GameObject>(holdAdvertisement.Advertisement_,
                    reference =>
                    {
                        if (reference != null)
                            finishCallback.Invoke();
                    });
            }
        }

        public override void Destroy()
        {
            if (bannerAssetReference != null)
            {
                bannerAssetReference.ReleaseOperation();
            }
            
            base.Destroy();
        }
    }
    
    public class BannerPageView:View<BannerPageViewController>
    {
        [ComponentBinder("PageView")] 
        public ScrollRect pageView;  
        
        [ComponentBinder("DefaultPage")] 
        public Transform defaultPage;
        
        [ComponentBinder("Content")] 
        public Transform content;
          
        [ComponentBinder("PageTemplate")] 
        public Transform pageTemplate;

        [ComponentBinder("ShiftGroup")] 
        public Transform indicators;

        private List<PageView> _pageViews;
        private List<Toggle> _toggles;

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            _pageViews = new List<PageView>();
            _toggles = new List<Toggle>();
            
            for (var i = 0; i < indicators.childCount; i++)
            {
                _toggles.Add(indicators.GetChild(i).GetComponent<Toggle>());
                _toggles[i].gameObject.SetActive(false);
            }
        }

        public void AddPage(Advertisement advertisement)
        {
            if (_pageViews.Count >= _toggles.Count)
            {
                return;
            }
            
            var pageGameObject = GameObject.Instantiate(pageTemplate.gameObject, content);
            pageGameObject.transform.SetAsLastSibling();
            pageGameObject.SetActive(true);
            
            var page = AddChild<PageView>(pageGameObject.transform);
            page.SetUpContent(advertisement);
            
            _pageViews.Add(page);
            
            _toggles[_pageViews.Count-1].gameObject.SetActive(true);
        }
        
        public void SetPageActive(int index)
        {
            for (var i = 0; i < _pageViews.Count; i++)
                _toggles[i].isOn = i == index;
        }

        public void DestroyAllPages()
        {
            for (var i = 0; i < _pageViews.Count; i++)
            {
                RemoveChild(_pageViews[i]);
                _toggles[i].gameObject.SetActive(false);
            }
            _pageViews.Clear();
        }
    }

    public class BannerPageViewController : ViewController<BannerPageView>
    {
        private List<Advertisement> _advertisements;
        private int currentPageIndex = 0;

        private bool abortPageTurn = false;

        private bool _isTweenAnimation = false;

        private BannerController _bannerController;


        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.pageView.onValueChanged.AddListener(OnPageChanged);
            view.defaultPage.GetComponent<Button>().onClick.AddListener(OnDefaultPageClicked);
            
            _bannerController = Client.Get<BannerController>();
        }

        public void OnDefaultPageClicked()
        {
            SoundController.PlayButtonClick();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), "DefaultBanner")));
        }

        public void OnPageChanged(Vector2 pos)
        {
            if (_isTweenAnimation)
            {
                return;
            }
            
            var index = (int)Math.Round(pos.x * (_advertisements.Count - 1));

            if (index != currentPageIndex)
            {
                currentPageIndex = index;
                view.SetPageActive(currentPageIndex);
            }
            
            if (!abortPageTurn)
            {
                if (turnTween != null)
                {
                    turnTween.Kill();
                }
                abortPageTurn = true;
                CheckAndTurnPage();
            }
        }

        protected void CheckAndTurnPage()
        {
            if (abortPageTurn)
            {
                WaitForSeconds(3, ()=>
                {
                    abortPageTurn = false;
                    if (view.transform)
                    {
                        TurnPage();
                    }
                });
            }
            else
            {
                TurnPage();
            }
        }

        public void SetUpContent(List<Advertisement> advertisement)
        {
           // _advertisements = Client.Get<BannerController>().GetLobbyFixedAdvertisement();

           if (_advertisements != null && _advertisements.SequenceEqual(advertisement))
               return;
            
           view.DestroyAllPages();
            
           _advertisements = advertisement;
           
            if (_advertisements != null && _advertisements.Count > 0)
            {
                view.defaultPage.gameObject.SetActive(false);

                for (var i = 0; i < _advertisements.Count; i++)
                {
                    view.AddPage(_advertisements[i]);
                }
            }
            else
            {
                view.defaultPage.gameObject.SetActive(true);
            }
            
           TurnPage();
           
           EnableUpdate(2);
        }
        
        private Tween turnTween;
        public void TurnPage()
        {
            if (_advertisements.Count <= 1)
            {
                return;
            }
            
            if (abortPageTurn)
            {
                return;
            }
            
            currentPageIndex++;
           
            if (currentPageIndex >= _advertisements.Count)
            {
                currentPageIndex = 0;
            }

            var targetPos = (float)currentPageIndex / (_advertisements.Count - 1);
            var startPos = view.pageView.horizontalNormalizedPosition;
            
          //  view.pageView.horizontalNormalizedPosition = 0;
            _isTweenAnimation = true;
            turnTween = DOTween.To(() =>startPos, (p) => { view.pageView.horizontalNormalizedPosition = p; }, targetPos, 0.3f).OnComplete(
                () =>
                {
                    _isTweenAnimation = false;
                });
            
            view.SetPageActive(currentPageIndex);

            WaitForSeconds(3, () =>
            {
                if (!abortPageTurn && view.transform)
                    TurnPage();
            });
        }
    }
}