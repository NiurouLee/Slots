// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/19/15:50
// Ver : 1.0.0
// Description : LobbyBannerContainerView.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using Event = Spine.Event;

namespace GameModule
{
    public class LobbyBannerContainerView : View<LobbyBannerContainerViewController>
    {
        [ComponentBinder("SinglePage")] public Transform singlePage;
        [ComponentBinder("MachinePage")] public Transform machinePage;
        [ComponentBinder("PagesView")] public Transform pagesView;

        public MachineBannerView machineBannerView;
        public SingleBannerView singleBannerView;
        public BannerPageView bannerPageView;

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            machineBannerView = AddChild<MachineBannerView>(machinePage);
            singleBannerView = AddChild<SingleBannerView>(singlePage);
            bannerPageView = AddChild<BannerPageView>(pagesView);
        }
    }

    public class LobbyBannerContainerViewController : ViewController<LobbyBannerContainerView>
    {
        protected int bannerListViewItemIndex;
        private List<Advertisement> advertisements;
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
           // SubscribeEvent<EventCollectGameIndexOnBannerViewList>(OnCollectGameIndexOnBannerViewList);
         
        }
        
        // protected void OnCollectGameIndexOnBannerViewList(EventCollectGameIndexOnBannerViewList evt)
        // {
        //     if (!view.transform.gameObject.activeInHierarchy)
        //         return;
        //     
        //     if (advertisements != null && advertisements.Count > 0)
        //     {
        //         var adv = advertisements[0];
        //
        //         if (adv.Type == AdvertisementType.Quickmachine && adv.Jump == evt.machineId)
        //         {
        //             EventBus.Dispatch(new EventNoticeGameIndexOnBannerViewList(evt.machineId, bannerListViewItemIndex));
        //         }
        //     }
        // }

        public void UpdateViewContent(int inBannerListViewItemIndex, List<Advertisement> inAdvertisements)
        {
            advertisements = inAdvertisements;
            bannerListViewItemIndex = inBannerListViewItemIndex;
            
            if (advertisements.Count == 1)
            {
                var advertisement = advertisements[0];

                if (advertisement.Type == AdvertisementType.Quickmachine)
                {
                    view.singleBannerView.Hide();
                    view.bannerPageView.Hide();
                    view.machineBannerView.Show();
                    view.machineBannerView.viewController.SetUpContent(advertisement);
                }
                else
                {
                    view.machineBannerView.Hide();
                    view.bannerPageView.Hide();
                    view.singleBannerView.Show();
                    view.singleBannerView.viewController.SetUpContent(advertisement);
                }
            }
            
            if (advertisements.Count > 1)
            {
                view.machineBannerView.Hide();
                view.singleBannerView.Hide();
                view.bannerPageView.Show();
                view.bannerPageView.viewController.SetUpContent(advertisements);
            }
        }
    }
}