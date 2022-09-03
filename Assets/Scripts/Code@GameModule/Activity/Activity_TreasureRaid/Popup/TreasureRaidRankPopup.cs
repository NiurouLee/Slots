using System;
using System.Collections.Generic;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidRankItemView : View
    {
        [ComponentBinder("NameText")]
        private Text nameText;  
        
        [ComponentBinder("NO.1")]
        private Transform rank1;
        
        [ComponentBinder("NO.2")]
        private Transform rank2;
        
        [ComponentBinder("NO.3")]
        private Transform rank3;
        
        [ComponentBinder("4Text")]
        private Transform rank4;    
        
        [ComponentBinder("ScoreText")]
        private Text scoreNumText;
        
        [ComponentBinder("Avatar")]
        private RawImage avatar;

        [ComponentBinder("Root/RewardGroup")]
        private Transform rewardGroup;

        [ComponentBinder("Root/RewardGroup/RewardContent/LeftReward/StandardType/RewardIcon")]
        private Image icon;
        
        [ComponentBinder("Root/RewardGroup/RewardContent/LeftReward/StandardType/CountText")]
        private TextMeshProUGUI countText;

        [ComponentBinder("Root/RewardGroup/AddImage")]
        private Transform addImg;
        
        [ComponentBinder("Root/RewardGroup/RightReward")]
        private Transform rightReward;

        [ComponentBinder("Root/TipGroup")]
        private Transform tips;

        [ComponentBinder("Root/TipGroup/RewardGroup")]
        private Transform tipRewardGroup;

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            rightReward.GetComponent<Button>().onClick.AddListener(OnRewardBtnClicked);
        }

        private void OnRewardBtnClicked()
        {
            XUtility.ShowTipAndAutoHide(tips);
        }

        public void DisableRank()
        {
            rank1.gameObject.SetActive(false);
            rank2.gameObject.SetActive(false);
            rank3.gameObject.SetActive(false);
            rank4.gameObject.SetActive(false);
        }

        private SGetMonopolyLeaderboard.Types.Rank rankInfo;

        public void UpdateContent(SGetMonopolyLeaderboard.Types.Rank inRankInfo, Reward reward)
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
                        rank4.GetComponent<TextMeshProUGUI>().text = $"Not\nRanked";
                    }
                    else
                    {
                        rank4.GetComponent<TextMeshProUGUI>().text = $"{rankInfo.Rank_}";
                    }
                    break;
            }
            
            scoreNumText.text = rankInfo.Score.GetCommaFormat();
            nameText.text = rankInfo.Name;//.Normalize();
            // System.Text.Encoding.ASCII.GetString(System.Text.Encoding.UTF8.GetBytes(rankInfo.Name));
            
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

            if (reward != null && reward.Items.Count > 0)
            {
                rewardGroup.gameObject.SetActive(true);
                var item = reward.Items[0];
                var sprite = XItemUtility.GetItemSprite(item.Type, item);
                icon.sprite = sprite;
                if (item.Type == Item.Types.Type.Coin)
                {
                    countText.SetText(((long) item.Coin.Amount).GetAbbreviationFormat(1));
                }
                else
                {
                    countText.SetText(XItemUtility.GetItemDefaultDescText(item));
                }

                addImg.gameObject.SetActive(reward.Items.Count > 1);
                rightReward.gameObject.SetActive(reward.Items.Count > 1);
                if (rightReward.gameObject.activeSelf)
                {
                    var level = rankInfo.Rank_;
                    if (level > 3)
                    {
                        level = 4;
                    }
                    for (int i = 0; i < rightReward.childCount; i++)
                    {
                        rightReward.GetChild(i).gameObject.SetActive(i == ((int) level - 1));
                    }
                    
                    for (int i = tipRewardGroup.childCount - 1; i >= 0 ; i--)
                    {
                        if (i != tipRewardGroup.childCount - 1)
                        {
                            GameObject.Destroy(tipRewardGroup.GetChild(i).gameObject);
                        }
                    }
                    var skipList = new List<Item.Types.Type>();
                    skipList.Add(reward.Items[0].Type);
                    XItemUtility.InitItemsUI(tipRewardGroup, reward.Items, tipRewardGroup.Find("PuzzleRewardCell"), null,
                        "StandardType", skipList);
                }
            }
            else
            {
                rewardGroup.gameObject.SetActive(false);
            }

        }
    }

    public class TreasureRaidRankPage : View
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

        private TreasureRaidRankItemView _rankMeItemView;
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            SetUpLoopListItemData();
            
            _rankMeItemView = AddChild<TreasureRaidRankItemView>(_leaderboardMe);
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
                item = listView.NewListViewItem("RankItem");
            }
            
            var proxy = item.GetComponent<MonoCustomDataProxy>();
            var rankItemView = proxy.GetCustomData<TreasureRaidRankItemView>();

            if (rankItemView == null)
            {
                rankItemView = View.CreateView<TreasureRaidRankItemView>(item.transform);
                proxy.SetCustomData(rankItemView);

                var baseEventProxy = item.gameObject.AddComponent<MonoBasicEventProxy>();

                baseEventProxy.BindDestroyEvent(() =>
                {
                    proxy.SetCustomData(null);
                    if (rankItemView != null)
                        rankItemView.Destroy();
                });
            }
            
            var treasureRaidRankPopup = parentView as TreasureRaidRankPopup;
            if (treasureRaidRankPopup != null)
            {
                var rankInfo = treasureRaidRankPopup.viewController.GetRankInfo(index);
                var reward = treasureRaidRankPopup.viewController.GetRankReward((int) rankInfo.Rank_);
                rankItemView.UpdateContent(rankInfo, reward);
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

        public void InitRankMe(SGetMonopolyLeaderboard.Types.Rank rankInfo, Reward reward)
        {
            if (rankInfo.AvatarId == 9999 && AccountManager.Instance.HasBindFacebook()) //use facebook avatar
            {
                rankInfo.AvatarUrl = AccountManager.Instance.GetFacebookId();
            }
            
            _rankMeItemView.UpdateContent(rankInfo, reward);

            if (indexOfMe < 0)
            {
                _leaderboardMe.localPosition = _bottomAttachNode.localPosition;
                _leaderboardMe.gameObject.SetActive(true);
            }

            // if (indexOfMe > 0)
            // {
            //     _listView.MovePanelToItemIndex(indexOfMe, 0);
            // }
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
            item1.mItemPrefab = _rankView.Find("Viewport/Content/RankItem").gameObject;
            item1.mItemPrefab.AddComponent<MonoCustomDataProxy>();
            item1.mInitCreateCount = 50;
            item1.mPadding = 0;
 
            _listView.mItemPrefabDataList.Add(item0);
            _listView.mItemPrefabDataList.Add(item1);
        
            _listView.mCurSnapData = new LoopListView2.SnapData();
            
             _scrollRect = _rankView.GetComponent<ScrollRect>();
            
           _scrollRect.onValueChanged.AddListener(OnScrollValueChange);
        }
        
        protected void OnScrollValueChange(Vector2 normalizePos)
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
    
    [AssetAddress("UITreasureRaidRankingPanel")]
    public class TreasureRaidRankPopup : Popup<TreasureRaidRankPopupController>
    {
        [ComponentBinder("Root/MainGroup/RankingGroup")] public Transform rankingGroup;
        [ComponentBinder("Root/MainGroup")] public Transform adaptGroup;

        [ComponentBinder("HelpButton")] public Button helpButton; 
        [ComponentBinder("Root/MainGroup/CloseButton")] public Button closeBtn; 
        
        [ComponentBinder("Root/MainGroup/TimerGroup/TimerText")] public TextMeshProUGUI timeText;
        [ComponentBinder("Root/MainGroup/TitleGroup/CionGroup/__NumberText")] public Text prizeText;

        public TreasureRaidRankPage rankPage;

        public TreasureRaidRankPopup(string address)
            : base(address)
        {
            
        }

        public override float GetPopUpMaskAlpha()
        {
            return 0f;
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdaptScaleTransform(adaptGroup, new Vector2(1400, 768));
            helpButton.onClick.AddListener(OnHelpButtonClicked);
            closeBtn.onClick.AddListener(OnCloseClicked);
        }

        public override void AdaptScaleTransform(Transform transformToScale, Vector2 preferSize)
        {
            var viewSize = ViewResolution.referenceResolutionLandscape;
            if (viewSize.x < preferSize.x)
            {
                var scale = viewSize.x / preferSize.x;
                transformToScale.localScale =  new Vector3(scale, scale, scale);
            }
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            rankPage = AddChild<TreasureRaidRankPage>(rankingGroup);
        }

        private void OnHelpButtonClicked()
        {
            SoundController.PlayButtonClick();
            PopupStack.ShowPopupNoWait<TreasureRaidRankHelpPopUp>();
        }
    }

    public class TreasureRaidRankPopupController : ViewController<TreasureRaidRankPopup>
    {
        private Activity_TreasureRaid ActivityTreasureRaid;
        
        private SGetMonopolyLeaderboard _leaderboard;

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.Close();
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            if (_leaderboard != null)
            {
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
                if (_leaderboard.MyRank != 0)
                {
                    var myRank = GetMyRankInfo();
                    if (myRank == null)
                    {
                        myRank = _leaderboard.Ranks[(int) _leaderboard.MyRank - 1];
                    }
                    var reward = GetRankReward((int) _leaderboard.MyRank);
                    view.rankPage.InitRankMe(myRank, reward);
                }
                else
                {
                    // 未上榜
                    var myRank = new SGetMonopolyLeaderboard.Types.Rank();
                    myRank.Name = Client.Get<UserController>().GetUserName();
                    myRank.Rank_ = 999;
                    myRank.Score = ActivityTreasureRaid.GetServerNeedCurrentRoundID() - 1;
                    myRank.AvatarId = Client.Get<UserController>().GetUserAvatarID();
                    view.rankPage.InitRankMe(myRank, null);
                }
                view.prizeText.SetText(_leaderboard.Coins.GetCommaFormat());
            }
        }

        /// <summary>
        /// 存在排名相同的情况，需要自己判断playerId
        /// </summary>
        private SGetMonopolyLeaderboard.Types.Rank GetMyRankInfo()
        {
            var myPlayerId = Client.Get<UserController>().GetUserId();
            for (int i = 0; i < _leaderboard.Ranks.Count; i++)
            {
                if (_leaderboard.Ranks[i].PlayerId == myPlayerId)
                {
                    return _leaderboard.Ranks[i];
                }
            }

            return null;
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            ActivityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            _leaderboard = inExtraAsyncData as SGetMonopolyLeaderboard;
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            Update();
            EnableUpdate(1);
        }

        public SGetMonopolyLeaderboard.Types.Rank GetRankInfo(int index)
        {
            if (_leaderboard != null && _leaderboard.Ranks.Count > index)
            {
                return _leaderboard.Ranks[index];
            }
            
            return null;
        }
        
        public Reward GetRankReward(int index)
        {
            if (_leaderboard == null)
                return null;

            for (int i = 0; i < _leaderboard.RankRewards.Count; i++)
            {
                if (index >= _leaderboard.RankRewards[i].From && index <= _leaderboard.RankRewards[i].To)
                {
                    return _leaderboard.RankRewards[i].Reward;
                }
            }
            return null;
        }
        
        public override void Update()
        {
            if (ActivityTreasureRaid == null)
                return;

            var countDown = ActivityTreasureRaid.GetCountDown();
          
            if (countDown <= 0)
            {
                view.timeText.text = XUtility.GetTimeText(0);
                DisableUpdate();
                return;
            }
            view.timeText.text = XUtility.GetTimeText(countDown, false, true);
        }

        public override void OnViewDestroy()
        {
            var myRank = GetMyRankInfo().Rank_;
            EventBus.Dispatch(new EventTreasureRaidRefreshRankView((uint)myRank));
            base.OnViewDestroy();
        }
    }
}