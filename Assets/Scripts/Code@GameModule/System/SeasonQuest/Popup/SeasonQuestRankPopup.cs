// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/17/15:16
// Ver : 1.0.0
// Description : SeasonPassRankPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonQuestRankItemView : View
    {
        [ComponentBinder("NameText")]
        public Text nameText;  
        
        [ComponentBinder("NO.1")]
        public Transform rank1;
        
        [ComponentBinder("NO.2")]
        public Transform rank2;
        
        [ComponentBinder("NO.3")]
        public Transform rank3;
        
        [ComponentBinder("4Text")]
        public Transform rank4;    
        
        [ComponentBinder("StarNumText")]
        public Text starNumText;
        
        [ComponentBinder("Avatar")]
        public RawImage avatar;

        public void DisableRank()
        {
            rank1.gameObject.SetActive(false);
            rank2.gameObject.SetActive(false);
            rank3.gameObject.SetActive(false);
            rank4.gameObject.SetActive(false);
        }

        private SSeasonQuestLeaderboard.Types.Rank rankInfo;

        public void UpdateContent(SSeasonQuestLeaderboard.Types.Rank inRankInfo)
        {
            rankInfo = inRankInfo;

            if (rankInfo == null)
            {
                transform.gameObject.SetActive(false);
                return;
            }
            
            transform.gameObject.SetActive(true);
            
            DisableRank();
            
            switch (rankInfo.Rank_)
            {
                case 1:
                    rank1.gameObject.SetActive(true);
                    break;
                case 2:
                    rank2.gameObject.SetActive(true);
                    break;
                case 3:
                    rank3.gameObject.SetActive(true);
                    break;
                default:
                    rank4.gameObject.SetActive(true);
                    if (rankInfo.Rank_ <= 0)
                    {
                        rank4.GetComponent<Text>().text = $"Not\nRanked";
                    }
                    else
                    {
                        rank4.GetComponent<Text>().text = $"{rankInfo.Rank_}";
                    }
                    break;
            }
            
            starNumText.text = rankInfo.Stars.GetCommaFormat();

            
            nameText.text = rankInfo.Name;
            
            if (rankInfo.PlayerId == Client.Get<UserController>().GetUserId())
            {
                nameText.text = Client.Get<UserController>().GetUserName();
            } 
            
            avatar.texture = AvatarController.defaultAvatar;

            var avatarId = rankInfo.AvatarId;
            
            AvatarController.GetAvatar(avatarId, rankInfo.AvatarUrl,(t) =>
            {
                if (avatar != null && avatarId == rankInfo.AvatarId)
                {
                    avatar.texture = t;
                }
            });
        }
    }
    
    public class SeasonQuestRewardItemView : View
    {
        [ComponentBinder("NameText")]
        public Transform nameText;  
        
        [ComponentBinder("NO.1")]
        public Transform rank1;
        
        [ComponentBinder("NO.2")]
        public Transform rank2;
        
        [ComponentBinder("NO.3")]
        public Transform rank3;
        
        [ComponentBinder("4Text")]
        public Transform rank4;    
        
        [ComponentBinder("GoldText")]
        public Text coinText;
        
        [ComponentBinder("RewardGroup")] 
        public Transform rewardGroup;

        public void DisableRank()
        {
            rank1.gameObject.SetActive(false);
            rank2.gameObject.SetActive(false);
            rank3.gameObject.SetActive(false);
            rank4.gameObject.SetActive(false);
        }
        public void UpdateContent(SSeasonQuestLeaderboard.Types.RankReward rankReward)
        {
            DisableRank();

            if (rankReward.From == rankReward.To)
            {
                switch (rankReward.From)
                {
                    case 1:
                        rank1.gameObject.SetActive(true);
                        break;
                    case 2:
                        rank2.gameObject.SetActive(true);
                        break;
                    case 3:
                        rank3.gameObject.SetActive(true);
                        break;
                }
            }
            else
            {
                rank4.gameObject.SetActive(true);
                rank4.GetComponent<Text>().text = $"{rankReward.From}-{rankReward.To}";
            }

            var coinReward = XItemUtility.GetItem(rankReward.Reward.Items, Item.Types.Type.Coin);

            if (coinReward != null)
            {
                coinText.text = coinReward.Coin.Amount.GetCommaFormat();
            }
            
            rewardGroup.Find("AwardIconCell").gameObject.SetActive(false);
            
            XItemUtility.InitItemsUI(rewardGroup, rankReward.Reward.Items, rewardGroup.Find("AwardIconCell"), skipTypeList: new List<Item.Types.Type>(){Item.Types.Type.Coin});
        }
    }

    public class SeasonQuestRankPage : View
    {
        [ComponentBinder("RankView")]
        private Transform _rankView;
        
        [ComponentBinder("LeaderboardMe")]
        private Transform _leaderboardMe;
        
        [ComponentBinder("TopAttachNode")]
        private Transform _topAttachNode; 
        
        [ComponentBinder("BottomAttachNode")]
        private Transform _bottomAttachNode;

        private LoopListView2 _listView;

        private int rankTotalItemCount;

        private ScrollRect _scrollRect;

        private int indexOfMe = -1;

        private SeasonQuestRankItemView _rankMeItemView;
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            SetUpLoopListItemData();
            
            _rankMeItemView = AddChild<SeasonQuestRankItemView>(_leaderboardMe);
        }

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index > rankTotalItemCount)
            {
                return null;
            }

            LoopListViewItem2 item = null;

            if (index == indexOfMe)
            {
                item = listView.NewListViewItem("RankMe");
            }
            else
            {
                item = listView.NewListViewItem("RankingItem");
            }
            
            var proxy = item.GetComponent<MonoCustomDataProxy>();
            var rankItemView = proxy.GetCustomData<SeasonQuestRankItemView>();

            if (rankItemView == null)
            {
                rankItemView = View.CreateView<SeasonQuestRankItemView>(item.transform);
                proxy.SetCustomData(rankItemView);

                var baseEventProxy = item.gameObject.AddComponent<MonoBasicEventProxy>();

                baseEventProxy.BindDestroyEvent(() =>
                {
                    proxy.SetCustomData(null);
                    if (rankItemView != null)
                        rankItemView.Destroy();
                });
            }
            
            var seasonQuestRankPopup = parentView as SeasonQuestRankPopup;
            if (seasonQuestRankPopup != null)
            {
                var rankInfo = seasonQuestRankPopup.viewController.GetRankInfo(index);
                rankItemView.UpdateContent(rankInfo);
            }
            return item;
        }

        public void InitListView(int itemTotalCount, int inIndexOfMe)
        {
            var initParam = new LoopListViewInitParam();
            initParam.mDistanceForRecycle0 = 20;
            initParam.mDistanceForRecycle1 = 20;
            initParam.mDistanceForNew0 = 10;
            initParam.mDistanceForNew1 = 10;

            indexOfMe = inIndexOfMe;
            
            rankTotalItemCount = itemTotalCount;
 
            _listView.InitListView(itemTotalCount, OnGetItemByIndex, initParam);
        }

        public void InitRankMe(uint rank, ulong star, string name)
        {
            var rankInfo = new SSeasonQuestLeaderboard.Types.Rank();
           
            rankInfo.Rank_ = rank;
            rankInfo.Stars = star;
            rankInfo.Name = name;
            
            rankInfo.AvatarId = Client.Get<UserController>().GetUserAvatarID();
         
            if (rankInfo.AvatarId == 9999 && AccountManager.Instance.HasBindFacebook()) //use facebook avatar
            {
                rankInfo.AvatarUrl = AccountManager.Instance.GetFacebookId();
            }
            
            _rankMeItemView.UpdateContent(rankInfo);

            if (indexOfMe < 0)
            {
                _leaderboardMe.localPosition = _bottomAttachNode.localPosition;
                _leaderboardMe.gameObject.SetActive(true);
            }

            if (indexOfMe > 0)
            {
                _listView.MovePanelToItemIndex(indexOfMe, 0);
            }
        }
        
        protected void SetUpLoopListItemData()
        {
            _listView = _rankView.gameObject.AddComponent<LoopListView2>();
            _listView.Init();
            _listView.ArrangeType = ListItemArrangeType.TopToBottom;

          
            ItemPrefabConfData item0 = new ItemPrefabConfData();
            item0.mItemPrefab = _rankView.Find("Viewport/Content/RankMe").gameObject;
            item0.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item0.mInitCreateCount = 1;
            item0.mPadding = 0;
            
            ItemPrefabConfData item1 = new ItemPrefabConfData();
            item1.mItemPrefab = _rankView.Find("Viewport/Content/RankingItem").gameObject;
            item1.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item1.mInitCreateCount = 50;
            item1.mPadding = 0;
 
            _listView.mItemPrefabDataList.Add(item0);
            _listView.mItemPrefabDataList.Add(item1);
        
            _listView.mCurSnapData = new LoopListView2.SnapData();
            
             _scrollRect = _rankView.GetComponent<ScrollRect>();
            
             _scrollRect.onValueChanged.AddListener(OnScrollValueChange);

        }
        
        protected void  OnScrollValueChange(Vector2 normalizePos)
        {
            if (indexOfMe < 0)
            {
                return;
            }
            
            var item = _listView.GetShownItemByItemIndex(indexOfMe);

            if (item != null)
            {
                var itemY = item.transform.localPosition.y - 46.5f;
                var itemPosY = item.transform.parent.TransformPoint(new Vector3(0, itemY, 0)).y;
              //  var itemPosY = item.transform.position.y + 5.45;
                
                if (itemPosY > _topAttachNode.transform.position.y)
                {
                    _leaderboardMe.localPosition = _topAttachNode.localPosition;
                    _leaderboardMe.gameObject.SetActive(true);
                }
                else if (itemPosY < _bottomAttachNode.transform.position.y)
                {
                    _leaderboardMe.localPosition = _bottomAttachNode.localPosition;
                    _leaderboardMe.gameObject.SetActive(true);
                }
                else
                {
                    _leaderboardMe.gameObject.SetActive(false);
                }
            }
            else
            {
                var firstItem = _listView.GetShownItemByIndex(0);
                var lastItem = _listView.GetShownItemByIndex(_listView.ShownItemCount - 1);

                if (firstItem.ItemIndex > indexOfMe)
                {
                    _leaderboardMe.localPosition = _topAttachNode.localPosition;
                    _leaderboardMe.gameObject.SetActive(true);
                }
                else if (lastItem.ItemIndex < indexOfMe)
                {
                    _leaderboardMe.localPosition = _bottomAttachNode.localPosition;
                    _leaderboardMe.gameObject.SetActive(true);
                }
                else
                {
                    _leaderboardMe.gameObject.SetActive(false);
                }
            }
        }
    }
    public class SeasonQuestRewardPage : View
    {
        [ComponentBinder("RewardView")]
        private Transform _rankView;
        
        private LoopListView2 _listView;

        private int rewardItemCount;
       
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            SetUpLoopListItemData();
        }
        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= rewardItemCount)
            {
                return null;
            }

            LoopListViewItem2 item = null;
            item = listView.NewListViewItem("RewardItem");
            var proxy = item.GetComponent<MonoCustomDataProxy>();
            var rankItemView = proxy.GetCustomData<SeasonQuestRewardItemView>();

            if (rankItemView == null)
            {
                rankItemView = View.CreateView<SeasonQuestRewardItemView>(item.transform);
                proxy.SetCustomData(rankItemView);

                var baseEventProxy = item.gameObject.AddComponent<MonoBasicEventProxy>();

                baseEventProxy.BindDestroyEvent(() =>
                {
                    proxy.SetCustomData(null);
                    if (rankItemView != null)
                        rankItemView.Destroy();
                });
            }
            
            var seasonQuestRankPopup = parentView as SeasonQuestRankPopup;
            if (seasonQuestRankPopup != null)
            {
                var rewardInfo = seasonQuestRankPopup.viewController.GetRewardInfo(index);
                rankItemView.UpdateContent(rewardInfo);
            }
            
            return item;
        }
         
        public void InitListView(int itemTotalCount)
        {
            var initParam = new LoopListViewInitParam();
            initParam.mDistanceForRecycle0 = 1000;
            initParam.mDistanceForRecycle1 = 1000;
            initParam.mDistanceForNew0 = 900;
            initParam.mDistanceForNew1 = 900;

            rewardItemCount = itemTotalCount;
            
            _listView.InitListView(itemTotalCount, OnGetItemByIndex, initParam);
        }
        
        protected void SetUpLoopListItemData()
        {
            _listView = _rankView.gameObject.AddComponent<LoopListView2>();
            _listView.Init();
            _listView.ArrangeType = ListItemArrangeType.TopToBottom;
            
            ItemPrefabConfData item0 = new ItemPrefabConfData();
            item0.mItemPrefab = _rankView.Find("Viewport/Content/RewardItem").gameObject;
            item0.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item0.mInitCreateCount = 10;
            _listView.mItemPrefabDataList.Add(item0);
        
            _listView.mCurSnapData = new LoopListView2.SnapData();
        }
    }
    
    [AssetAddress("UIQuestSeasonOneLeaderboard","","UIQuestSeasonOneLeaderboard_Pad")]
    public class SeasonQuestRankPopup:Popup<SeasonQuestRankPopupController>
    {
        [ComponentBinder("RankingGroup")] public Transform rankingGroup;

        [ComponentBinder("AwardGroup")] public Transform awardGroup;

        [ComponentBinder("HelpButton")] public Button helpButton; 
        
        [ComponentBinder("Root/GoldGroup/GoldText")] public Text goldenText;

        [ComponentBinder("TimeText")] public Text timeText;
        
        [ComponentBinder("Root/ToggleGroup/StateGroup/RankingStateGroup")] 
        public Transform enableRankPage;
        
        [ComponentBinder("Root/ToggleGroup/DisableGroup/RankingDisableState")] 
        public Button disableRankPageButton;
         
        [ComponentBinder("Root/ToggleGroup/StateGroup/AwardStateGroup")] 
        public Transform enableRewardPage;
        [ComponentBinder("Root/ToggleGroup/DisableGroup/AwardDisableState")] 
        public Button disableRewardPageButton;

        public SeasonQuestRankPage rankPage;
        public SeasonQuestRewardPage rewardPage;
        
        [ComponentBinder("CloseButton")] public Button buttonClose; 

        public SeasonQuestRankPopup(string address)
            : base(address)
        {
            
        }
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            rankPage = AddChild<SeasonQuestRankPage>(rankingGroup);
            rewardPage = AddChild<SeasonQuestRewardPage>(awardGroup);
            
            buttonClose.onClick.AddListener(OnCloseClicked);
        }
    }

    public class SeasonQuestRankPopupController:ViewController<SeasonQuestRankPopup>
    {
        private SeasonQuestController _seasonQuestController;

        private  SSeasonQuestLeaderboard _leaderboard;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            _seasonQuestController = Client.Get<SeasonQuestController>();
             
            if (_leaderboard != null)
            {
                var rewardsCount = _leaderboard.RankRewards.Count;
                view.rewardPage.InitListView(rewardsCount);


                var indexInList = -1;
                if (_leaderboard.MyRank > 0)
                {
                    var myPlayerId = Client.Get<UserController>().GetUserId();
                    for (var i = 0; i < _leaderboard.Ranks.Count; i++)
                    {
                        if (_leaderboard.Ranks[i].PlayerId == myPlayerId)
                        {
                            indexInList = i;
                            break;
                        }
                    }
                }
                
                var rankCount = _leaderboard.Ranks.Count;
                if (indexInList < 0)
                {
                    rankCount += 1;
                }
                
                view.rankPage.InitListView(rankCount, indexInList);

                view.rankPage.InitRankMe(_leaderboard.MyRank,
                    _seasonQuestController.GetQuestStarCount(),
                    Client.Get<UserController>().GetUserName());

            }

            view.disableRankPageButton.onClick.AddListener(OnRankPageClicked);
            view.disableRewardPageButton.onClick.AddListener(OnRewardPageClicked);
            view.helpButton.onClick.AddListener(OnHelpButtonClicked);
            
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonRankenter);
        }
 
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(2);
            
            
            //Default Show Rank Page
            view.enableRankPage.gameObject.SetActive(true);
            view.disableRewardPageButton.gameObject.SetActive(true);
            
            view.enableRewardPage.gameObject.SetActive(false);
            view.disableRankPageButton.gameObject.SetActive(false);
            
            view.rankPage.Show();
            view.rewardPage.Hide();
            
            InitUI();
        }

        public void InitUI()
        {
            view.goldenText.SetText(_leaderboard.Coins.GetCommaFormat());
            var countDown = _seasonQuestController.GetQuestCountDown();
            view.timeText.text = XUtility.GetTimeText(countDown);
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
           
            SubscribeEvent<EventSeasonQuestSeasonFinish>((evt) =>
            {
                view.Close();
            });
        }

        public void OnHelpButtonClicked()
        {
            SoundController.PlayButtonClick();
            
            PopupStack.ShowPopupNoWait<SeasonQuestHelpPopUp>();
        }
        public void OnRankPageClicked()
        {
            view.enableRankPage.gameObject.SetActive(true);
            view.disableRewardPageButton.gameObject.SetActive(true);
            
            view.enableRewardPage.gameObject.SetActive(false);
            view.disableRankPageButton.gameObject.SetActive(false);
            
            view.rankPage.Show();
            view.rewardPage.Hide();
            
            SoundController.PlayButtonClick();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            _leaderboard = inExtraData as SSeasonQuestLeaderboard;
        }

        public void OnRewardPageClicked()
        {
            SoundController.PlayButtonClick();
            
            view.enableRankPage.gameObject.SetActive(false);
            view.disableRewardPageButton.gameObject.SetActive(false);
            
            view.enableRewardPage.gameObject.SetActive(true);
            view.disableRankPageButton.gameObject.SetActive(true);
            
            view.rankPage.Hide();
            view.rewardPage.Show();
            
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonRankaward);
        }
        
        public override void Update()
        {
            var countDown = _seasonQuestController.GetQuestCountDown();
          
            if (countDown <= 0)
            {
                DisableUpdate();
                return;
            }
                
            view.timeText.text = XUtility.GetTimeText(countDown);
        }


        public SSeasonQuestLeaderboard.Types.Rank GetRankInfo(int index)
        {
            if (_leaderboard != null && _leaderboard.Ranks.Count > index)
            {
                return _leaderboard.Ranks[index];
            }
            
            return null;
        }
        
        public SSeasonQuestLeaderboard.Types.RankReward GetRewardInfo(int index)
        {
            if (_leaderboard != null && _leaderboard.RankRewards.Count > index)
            {
                return _leaderboard.RankRewards[index];
            }
            return null;
        }
    }
}