// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/16/17:31
// Ver : 1.0.0
// Description : SingleBannerView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SingleBannerView:View<SingleBannerViewController>
    {
        [ComponentBinder("Loading")] 
        public Transform loadingImage;

        [ComponentBinder("Button")] 
        public Button button;
        
        [ComponentBinder("OnlyCircle")] 
        public Transform bannerFrame;
    }
    
    public class SingleBannerViewController : ViewController<SingleBannerView>
    {
        public AssetReference bannerAssetReference;

        protected Advertisement holdAdvertisement;

        protected GameObject attachBannerGameObject;
        
        protected TaskCompletionSource<bool> downloadTask;

        protected GameObject bannerGameObject;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            view.button.onClick.AddListener(OnBannerClicked);
        }

        public async void SetUpContent(Advertisement advertisement)
        {
            holdAdvertisement = advertisement;

            CleanUpContent();
            
            LoadBannerAsset(() =>
            {
                if (advertisement != holdAdvertisement || bannerAssetReference == null)
                    return;

                if (bannerGameObject != null)
                {
                    GameObject.Destroy(this.bannerGameObject);
                    bannerGameObject = null;
                }
            
                bannerGameObject = bannerAssetReference.InstantiateAsset<GameObject>();
                bannerGameObject.transform.SetParent(view.transform, false);
                view.loadingImage.gameObject.SetActive(false);
                
                Client.Get<BannerController>().UpdateBannerContent(holdAdvertisement,this.view, bannerGameObject.transform);
                
            
                if (view.bannerFrame)
                {
                    view.bannerFrame.SetAsLastSibling();
                }
                view.button.transform.SetAsLastSibling();
            });
        }

        public void CleanUpContent()
        {
            if (bannerGameObject != null)
            {
                GameObject.Destroy(this.bannerGameObject);
                bannerGameObject = null;
            }
            
            view.loadingImage.gameObject.SetActive(true);
        }

        public void OnBannerClicked()
        {
            Client.Get<BannerController>().ResponseAdvertisement(holdAdvertisement);
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
                        if (view.transform == null)
                        {
                            return;
                        }
                        
                        if (reference != null)
                            finishCallback.Invoke();
                    });
            }
        }

        public override void OnViewDestroy()
        {
            if (bannerAssetReference != null)
            {
                bannerAssetReference.ReleaseOperation();
                bannerAssetReference = null;
            }
            
            base.OnViewDestroy();
        }
    }
}