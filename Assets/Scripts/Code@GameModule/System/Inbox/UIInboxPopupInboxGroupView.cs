using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class UIInboxPopupInboxGroupView : View<UIInboxPopupInboxGroupViewController>
    {
        [ComponentBinder("ScrollView")] public Transform transformLoopList;

        [ComponentBinder("ScrollView/Viewport/Content/CommonCell")]
        public Transform transformCommonCell;

        [ComponentBinder("ScrollView/Viewport/Content/OperationCell")]
        public Transform transformOperationCell;

        [ComponentBinder("ScrollView/Viewport/Content/VIPCell")]
        public Transform transformVIPCell;

        [ComponentBinder("ScrollView/Viewport/Content/WatchVideoCell")]
        public Transform transformWatchVideoCell;

        [ComponentBinder("ScrollView/Viewport/Content/SeasonPassCell")]
        public Transform transformSeasonPassCell;

        [ComponentBinder("ScrollView/Viewport/Content/ActivityCashBackTimeCell")]
        public Transform transformActivityCashBackTimeCell;

        [ComponentBinder("ScrollView/Viewport/Content/InboxGoldCouponCell")]
        public Transform transformGoldCouponCell;

        [ComponentBinder("ScrollView/Viewport/Content/InboxQuestSeasonOneCell")]
        public Transform transformInboxQuestSeasonOneCell;

        [ComponentBinder("ScrollView/Viewport/Content/AlbumSeasonFinishCell")]
        public Transform albumSeasonFinishCellTransform;

        [ComponentBinder("ScrollView/Viewport/Content/LottoBonusCell")]
        public Transform lottoGameCellTransform;

        [ComponentBinder("ScrollView/Viewport/Content/TreasureRaidCell")]
        public Transform treasureRaidCellTransform;

        public LoopListView2 mailContentList;

        [ComponentBinder("NoMessage")] public Transform transformNoMessage;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            mailContentList = transformLoopList.gameObject.AddComponent<LoopListView2>();
            mailContentList.Init();
            mailContentList.ArrangeType = ListItemArrangeType.TopToBottom;

            transformCommonCell.gameObject.SetActive(false);

            var inboxItemUITemplate = new List<Transform>()
            {
                transformOperationCell,
                transformVIPCell,
                transformWatchVideoCell,
                transformSeasonPassCell,
                transformActivityCashBackTimeCell,
                transformGoldCouponCell,
                transformInboxQuestSeasonOneCell,
                albumSeasonFinishCellTransform,
                lottoGameCellTransform,
                treasureRaidCellTransform
            };

            for (var i = 0; i < inboxItemUITemplate.Count; i++)
            {
                ItemPrefabConfData item = new ItemPrefabConfData();

                item.mItemPrefab = inboxItemUITemplate[i].gameObject;
                item.mItemPrefab.gameObject.AddComponent<MonoCustomDataProxy>();
                item.mInitCreateCount = 1;
                mailContentList.mItemPrefabDataList.Add(item);
            }

            mailContentList.InitListView(0, viewController.OnGetItemByIndex);
        }
    }

    public class UIInboxPopupInboxGroupViewController : ViewController<UIInboxPopupInboxGroupView>
    {
        private InboxController _inboxController;

        private List<InboxItem> _inboxItems;

        /// <summary>
        /// 先临时这么写
        /// </summary>
        private Dictionary<int, string> _itemNameMap = new Dictionary<int, string>
        {
            {-2, "InboxGoldCouponCell"},
            {-1, "WatchVideoCell"},
            {0, "OperationCell"},
            {1, "VIPCell"},
            {2, "WatchVideoCell"},
            {3, "SeasonPassCell"},
            {4, "ActivityCashBackTimeCell"},
            {5, "InboxQuestSeasonOneCell"},
            {6, "AlbumSeasonFinishCell"},
            {7, "LottoBonusCell"},
            {8, "TreasureRaidCell"},
        };

        public override void OnViewDidLoad()
        {
            _inboxController = Client.Get<InboxController>();
            base.OnViewDidLoad();
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            _inboxItems = _inboxController.GetAllInboxItem();

            RefreshInboxContent();
        }

        public void RefreshInboxContent()
        {
            _inboxItems = _inboxController.GetAllInboxItem();

            view.transformNoMessage.gameObject.SetActive(_inboxItems.Count == 0);

            view.mailContentList.SetListItemCount(_inboxItems.Count);

            view.mailContentList.RefreshAllShownItem();
        }

        public string GetItemName(int key)
        {
            string result = null;
            _itemNameMap.TryGetValue(key, out result);
            return result;
        }

        public LoopListViewItem2 OnGetItemByIndex(LoopListView2 loopList, int index)
        {
            var count = _inboxItems.Count;

            if (index < 0 || index >= count)
            {
                return null;
            }

            view.transformNoMessage.gameObject.SetActive(false);

            var itemData = _inboxItems[index];

            var itemName = GetItemName(itemData.typeID);

            if (string.IsNullOrWhiteSpace(itemName))
            {
                return null;
            }

            var item = loopList.NewListViewItem(itemName);

            UpdateItemAttachView(item, itemData, index);

            return item;
        }

        public void UpdateItemAttachView(LoopListViewItem2 loopListViewItem2, InboxItem inboxItem, int index)
        {
            var proxy = loopListViewItem2.GetComponent<MonoCustomDataProxy>();

            var inboxCell = proxy.GetCustomData<UIInboxCellView>();

            if (inboxCell == null)
            {
                switch (inboxItem.typeID)
                {
                    case -2:
                        inboxCell = View.CreateView<UIInboxCellView_GoldenCoupon>(loopListViewItem2.transform);
                        break;
                    case -1:
                        inboxCell = View.CreateView<UIInboxCellView_WatchVideo>(loopListViewItem2.transform);
                        break;
                    case 0:
                        inboxCell = View.CreateView<UIInboxCellView_Operation>(loopListViewItem2.transform);
                        break;
                    case 1:
                        inboxCell = View.CreateView<UIInboxCellView_VIP>(loopListViewItem2.transform);
                        break;
                    case 2:
                        break;
                    case 3:
                        inboxCell = View.CreateView<UIInboxCellView_SeasonPass>(loopListViewItem2.transform);
                        break;
                    case 4:
                        inboxCell = View.CreateView<UIInboxCellView_ActivityCashBack>(loopListViewItem2.transform);
                        break;
                    case 5:
                        inboxCell = View.CreateView<UIInboxCellView_SeasonQuestRankReward>(loopListViewItem2.transform);
                        break;
                    case 6:
                        inboxCell =
                            View.CreateView<UIInboxCellView_AlbumSeasonFinishReward>(loopListViewItem2.transform);
                        break;
                    case 7:
                        inboxCell = View.CreateView<UIInboxCellView_FreeLottoBonus>(loopListViewItem2.transform);
                        break;
                    case 8:
                        inboxCell = View.CreateView<UIInboxCellView_TreasureRaidRankReward>(loopListViewItem2.transform);
                        break;
                }

                proxy.SetCustomData(inboxCell);
            }

            if (inboxCell != null)
            {
                inboxCell.Set(inboxItem);

                var baseEventProxy = loopListViewItem2.gameObject.AddComponent<MonoBasicEventProxy>();

                baseEventProxy.BindDestroyEvent(() =>
                {
                    proxy.SetCustomData(null);
                    if (inboxCell != null)
                        inboxCell.Destroy();
                });
            }
        }
    }
}