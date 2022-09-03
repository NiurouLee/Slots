//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-29 19:01
//  Ver : 1.0.0
//  Description : SeasonPassRewardListViewController.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassRewardListView : View<SeasonPassRewardListViewController>
    {

        [ComponentBinder("Viewport")]
        public RectTransform Viewport;

        [ComponentBinder("Viewport/Content")]
        public RectTransform Content;

        public SeasonPassPageViewController PageViewController;

        public void InitView(SeasonPassPageViewController controller)
        {
            PageViewController = controller;
            viewController.SetUpLoopListItemData();
        }
    }

    public class SeasonPassRewardListViewController : ViewController<SeasonPassRewardListView>
    {
        private int totalCount = 101;
        private LoopListView2 _loopListView;
        private SeasonPassController _passController;


        public void DestroyListView()
        {
            GameObject.Destroy(_loopListView);
            _loopListView = null;
        }

        public void SetUpLoopListItemData()
        {
            _passController = Client.Get<SeasonPassController>();
            _loopListView = view.transform.gameObject.AddComponent<LoopListView2>();
            _loopListView.Init();


            var isPortrait = ViewManager.Instance.IsPortrait;

            _loopListView.ArrangeType = isPortrait ? ListItemArrangeType.TopToBottom : ListItemArrangeType.LeftToRight;

            ItemPrefabConfData item1 = new ItemPrefabConfData();
            item1.mItemPrefab = view.transform.Find("Viewport/Content/PassNormalCell").gameObject;
            item1.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item1.mInitCreateCount = 10;
            item1.mPadding = 20;

            ItemPrefabConfData item2 = new ItemPrefabConfData();
            item2.mItemPrefab = view.transform.Find("Viewport/Content/PassDoubleCell").gameObject;
            item2.mInitCreateCount = 5;
            item2.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item2.mPadding = 20;

            ItemPrefabConfData item3 = new ItemPrefabConfData();
            item3.mItemPrefab = view.transform.Find("Viewport/Content/GoldenChestButtonCell").gameObject;
            item3.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item3.mInitCreateCount = 1;
            item3.mPadding = 400;

            _loopListView.mItemPrefabDataList.Add(item1);
            _loopListView.mItemPrefabDataList.Add(item2);
            _loopListView.mItemPrefabDataList.Add(item3);

            _loopListView.mCurSnapData = new LoopListView2.SnapData();

            _loopListView.InitListView(101, OnGetItemByIndex);

            MoveToCurrentLevel();
            EnableUpdate(0);

            view.transform.GetComponent<ScrollRect>().onValueChanged.AddListener((pt) =>
            {
                EventBus.Dispatch(new EventSeasonPassChestUpdate("CloseTip"));
            });
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSeasonPassCloseBuyLevel>(OnEventSeasonPassCloseBuyLevel);
        }

        private void OnEventSeasonPassCloseBuyLevel(EventSeasonPassCloseBuyLevel obj)
        {
            MoveToCurrentLevel();
        }

        public void MoveToCurrentLevel()
        {
            _loopListView.MovePanelToItemIndex(Math.Min(99, Math.Max(0, (int)_passController.Level - 1)), 0);
        }

        public void RefreshAllItems()
        {
            if (_loopListView != null)
            {
                _loopListView.RefreshAllShownItem();
            }
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= totalCount)
            {
                return null;
            }

            int level = index + 1;
            LoopListViewItem2 item = null;
            if (level <= 100)
            {
                var isDouble = IsDoubleCell(level);
                var isNextDouble = IsDoubleCell(level + 1);
                var prefabName = isDouble ? "PassDoubleCell" : "PassNormalCell";
                item = listView.NewListViewItem(prefabName);
                item.transform.name = level.ToString();
                MonoCustomDataProxy proxy = item.GetComponent<MonoCustomDataProxy>();
                SeasonPassRewardCell itemView = proxy.GetCustomData<SeasonPassRewardCell>();
                if (itemView == null)
                {
                    itemView = View.CreateView<SeasonPassRewardCell>(item.transform);
                    proxy.SetCustomData(itemView);
                    item.transform.Bind<MonoBasicEventProxy>(true).BindDestroyEvent(() =>
                    {
                        if (itemView != null)
                            itemView.Destroy();
                    });
                }
                itemView.transform.SetAsLastSibling();
                itemView.viewController.InitRewardItem(view.PageViewController, isDouble, isNextDouble, level, level == 100);
            }
            else
            {
                item = listView.NewListViewItem("GoldenChestButtonCell");
                item.transform.name = "101";
                MonoCustomDataProxy proxy = item.GetComponent<MonoCustomDataProxy>();
                SeasonPassRewardCellChest itemView = proxy.GetCustomData<SeasonPassRewardCellChest>();
                if (itemView == null)
                {
                    itemView = View.CreateView<SeasonPassRewardCellChest>(item.transform);
                    proxy.SetCustomData(itemView);
                    item.transform.Bind<MonoBasicEventProxy>(true).BindDestroyEvent(() =>
                    {
                        if (itemView != null)
                            itemView.Destroy();
                    });
                    item.transform.Bind<MonoBasicEventProxy>(true).BindDisableEvent(() =>
                    {
                        if (itemView != null)
                            itemView.viewController.CleanAllSubscribedEvents();
                    });
                }

                itemView.transform.SetAsLastSibling();
                itemView.viewController.InitView(view.PageViewController);
            }
            return item;
        }

        private bool IsDoubleCell(int level)
        {
            return GetFreeRewards(level).Count > 1 || GetGoldenRewards(level).Count > 1;
        }

        public RepeatedField<MissionPassReward> GetFreeRewards(int level)
        {
            var passController = Client.Get<SeasonPassController>();
            RepeatedField<MissionPassReward> rewards = new RepeatedField<MissionPassReward>();
            if (passController.FreeMissionPassRewards.ContainsKey(level))
            {
                rewards = passController.FreeMissionPassRewards[level];
            }
            return rewards;
        }

        public RepeatedField<MissionPassReward> GetGoldenRewards(int level)
        {
            var passController = Client.Get<SeasonPassController>();
            RepeatedField<MissionPassReward> rewards = new RepeatedField<MissionPassReward>();
            if (passController.GoldenMissionPassRewards.ContainsKey(level))
            {
                rewards = passController.GoldenMissionPassRewards[level];
            }
            return rewards;
        }

        public override void Update()
        {
            base.Update();
            if (_loopListView)
            {
                var itemList = _loopListView.ItemList;
                for (int i = 0; i < itemList.Count; i++)
                {
                    var itemTrans = itemList[i].transform;
                    itemTrans.SetSiblingIndex(itemTrans.name.ToInt());
                }
            }
        }

        public void SetScroll(bool enable)
        {
            var scrollRect = _loopListView.GetComponent<ScrollRect>();
            scrollRect.enabled = enable;
        }
    }
}
