// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/19/15:46
// Ver : 1.0.0
// Description : BannerIconListView.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
      public class BannerIconListViewController : ViewController<BannerIconListView>
    {
        public int listItemTotalCount;
        public int bannerCount;
        public int machineCount;
        public int separatorCount;
        public const int VisibleItemCount = 4;

        public Dictionary<uint, List<Advertisement>> scrollableAdvertisement;

        public int GetListItemCount()
        {
            machineCount = Client.Get<MachineLogicController>().GetValidMachineCount();

            scrollableAdvertisement = Client.Get<BannerController>().GetLobbyScrollableAdvertisement();
            bannerCount = 0;
           
            if (scrollableAdvertisement != null)
            {
                bannerCount = scrollableAdvertisement.Count;
            }
            
            separatorCount = Mathf.CeilToInt(machineCount / 6f);
            var machineColumn = (machineCount / 6) * 3;
          
            if (machineCount % 6 < 3)
            {
                machineColumn += machineCount % 3;
            }
            else
            {
                machineColumn += 3;
            }
            //后面+2是scroll rect 前面和后面分别加的占位符，避免内容太靠边
            return separatorCount + machineColumn + bannerCount + 2;
        }
 
        public void UpdateLobbyBannerIconContent()
        {
            view.UpdateListViewContent(GetListItemCount());
        }

        public int GetIconIndex(int itemIndex)
        {
            itemIndex = itemIndex - (bannerCount + 1);
            itemIndex = itemIndex - (itemIndex + 1) / VisibleItemCount;
            return itemIndex;
        }

        public void SetUpListViewContent()
        {
            listItemTotalCount = GetListItemCount();
 
            view.InitListView(listItemTotalCount, OnGetItemByIndex);

            if (Client.Get<GuideController>().GetEnterMachineGuide() != null)
            {
                var index = Client.Get<MachineLogicController>().GetGameIndexById(Client.Get<GuideController>().GetEnterMachineId());
                var itemIndex = 1 + bannerCount + index / 6 * 4 + (index % 3 + 1);
                
                view.FocusOnItem(itemIndex,0);
            }
            else
            {
                view.RestoreToLastPosition();
            }
        }

        public bool IsSeparatorLine(int index)
        {
            index = index - bannerCount;
          
            if (index < 0)
                return false;
            
            if (index % VisibleItemCount == 0)
            {
                return true;
            }

            return false;
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= listItemTotalCount)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            
            if (index == 0 || index == listItemTotalCount - 1)
            {
                item = listView.NewListViewItem("PlaceHolder");
                return item;
            } 
 
            index--;
            if (index < bannerCount)
            {
                item = listView.NewListViewItem("BannerContainer");
           
                if (index == bannerCount - 1)
                {
                    item.Padding = 30;
                }
                 
                MonoCustomDataProxy proxy = item.GetComponent<MonoCustomDataProxy>();

                LobbyBannerContainerView bannerView = proxy.GetCustomData<LobbyBannerContainerView>();

                if (bannerView == null)
                {
                    bannerView = View.CreateView<LobbyBannerContainerView>(item.transform);
                    proxy.SetCustomData(bannerView);
                    item.transform.Bind<MonoBasicEventProxy>(true).BindDestroyEvent(() =>
                    {
                        proxy.SetCustomData(null);
                        if (bannerView != null)
                            bannerView.Destroy();
                    });
                }

                var advertisementPosition = scrollableAdvertisement.Keys.ToList();
                advertisementPosition.Sort();
               
                if (index < advertisementPosition.Count)
                {
                    var key = advertisementPosition[index];
                    var advertisements = scrollableAdvertisement[key];
                    bannerView.viewController.UpdateViewContent(index , advertisements);
                }
            }
            else if (index == bannerCount)
            {
                item = listView.NewListViewItem("Separator");
            }
            else if (IsSeparatorLine(index))
            {
                item = listView.NewListViewItem("GroupSeparator");
            }
            else
            {
                item = listView.NewListViewItem("IconContainer");

                MonoCustomDataProxy proxy = item.GetComponent<MonoCustomDataProxy>();
                LobbyIconContainerView iconContainerView = proxy.GetCustomData<LobbyIconContainerView>();

                if (iconContainerView == null)
                {
                    iconContainerView = View.CreateView<LobbyIconContainerView>(item.transform);
                    proxy.SetCustomData(iconContainerView);

                    var baseEventProxy = item.gameObject.AddComponent<MonoBasicEventProxy>();
                    baseEventProxy.BindDestroyEvent(() =>
                    {
                        proxy.SetCustomData(null);
                        if (iconContainerView != null)
                            iconContainerView.Destroy();
                    });
                }

                iconContainerView.UpdateContent(index, GetIconIndex(index), machineCount);
            }

            return item;
        }
    }

    public class BannerIconListView : View<BannerIconListViewController>
    {
        private LoopListView2 _listView;
        
        public static int lastItemIndex = -1;
        public static Vector2 lastItemOffset = Vector2.zero;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            
            UpdateContentSize();
            
            SetUpLoopListItemData();
        }

        public void FocusOnItem(int itemIndex, float offset)
        {
            _listView.MovePanelToItemIndex(itemIndex, offset);
            var scrollRect = _listView.GetComponent<ScrollRect>();
            scrollRect.enabled = false;
        }

        public void RestoreToLastPosition()
        {
            if (lastItemIndex >= 0)
            {
                if (lastItemIndex < _listView.ItemTotalCount)
                {
                    _listView.MovePanelToItemIndex(lastItemIndex, lastItemOffset.x);
                }
            }
        }
        
        protected void UpdateContentSize()
        {
            var sizeDelta = rectTransform.sizeDelta;

            var viewResolution = ViewResolution.referenceResolutionLandscape;
            sizeDelta.x = viewResolution.x - 342;
            rectTransform.sizeDelta = sizeDelta;
        }

        public void InitListView(int itemTotalCount,
            System.Func<LoopListView2, int, LoopListViewItem2> onGetItemByIndex)
        {
            var initParam = new LoopListViewInitParam();
            initParam.mDistanceForRecycle0 = 1000;
            initParam.mDistanceForRecycle1 = 1000;
            initParam.mDistanceForNew0 = 900;
            initParam.mDistanceForNew1 = 900;
            
            _listView.InitListView(itemTotalCount, onGetItemByIndex, initParam);
        }

        public void UpdateListViewContent(int totalCount)
        {
            _listView.SetListItemCount(totalCount);
        }

        protected void SetUpLoopListItemData()
        {
            _listView = transform.gameObject.AddComponent<LoopListView2>();
            _listView.Init();
            _listView.ArrangeType = ListItemArrangeType.LeftToRight;
            
            ItemPrefabConfData item0 = new ItemPrefabConfData();
            item0.mItemPrefab = transform.Find("Viewport/Content/PlaceHolder").gameObject;
            item0.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item0.mInitCreateCount = 1;
            item0.mPadding = 10;
            
            ItemPrefabConfData item1 = new ItemPrefabConfData();
            item1.mItemPrefab = transform.Find("Viewport/Content/BannerContainer").gameObject;
            item1.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item1.mInitCreateCount = 10;
            item1.mPadding = 40;

            ItemPrefabConfData item2 = new ItemPrefabConfData();
            item2.mItemPrefab = transform.Find("Viewport/Content/IconContainer").gameObject;
            item2.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item2.mInitCreateCount = 10;
            item2.mPadding = 40;

            ItemPrefabConfData item3 = new ItemPrefabConfData();
            item3.mItemPrefab = transform.Find("Viewport/Content/Separator").gameObject;
            item3.mInitCreateCount = 2;
            item3.mPadding = 40;
            
            ItemPrefabConfData item4 = new ItemPrefabConfData();
            item4.mItemPrefab = transform.Find("Viewport/Content/GroupSeparator").gameObject;
            item4.mInitCreateCount = 2;
            item4.mPadding = 40;

            _listView.mItemPrefabDataList.Add(item0);
            _listView.mItemPrefabDataList.Add(item1);
            _listView.mItemPrefabDataList.Add(item2);
            _listView.mItemPrefabDataList.Add(item3);
            _listView.mItemPrefabDataList.Add(item4);

            _listView.mCurSnapData = new LoopListView2.SnapData();
        }

        public override void Destroy()
        {
            lastItemIndex = _listView.GetViewOffset(out lastItemOffset);
            base.Destroy();
        }
    }
}