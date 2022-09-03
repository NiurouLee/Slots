//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 17:56
//  Ver : 1.0.0
//  Description : PassMissionPage.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassPage : View<SeasonPassPageViewController>
    {
        [ComponentBinder("SeasonNumText")]
        public TextMeshProUGUI seasonNumText;

        [ComponentBinder("BuyLevelsButton")]
        public Button buyLevelsButton;

        [ComponentBinder("UnlockMissionPassButton")]
        public Button btnUnlockMissionPass;

        [ComponentBinder("CollectButton")]
        public Button btnCollect;

        [ComponentBinder("ScrollView")] public ScrollRect ScrollRect;

        [ComponentBinder("ScrollView/Viewport/Content/BGProgressBar")]
        private Slider progressBg;

        [ComponentBinder("ScrollView/Viewport/Content/BGProgressBar")]
        private RectTransform rectProgressBg;
        //[ComponentBinder("ScrollView/Viewport/Content/ProgressBar")]
        //private Slider progressBar;

        [ComponentBinder("ProgressGroup/ProgressBar")]
        private Slider progressBarStar;
        [ComponentBinder("ProgressGroup/ProgressBar/ProgressText")]
        private TextMeshProUGUI txtProgressBarStar;

        [ComponentBinder("ScrollView/Viewport/Content/GoldenChestButtonCell")]
        private Transform transGoldenChest;
        [ComponentBinder("GoldenBubbleGroup")]
        public Canvas canvasGoldenChestTips;

        [ComponentBinder("GoldenBubbleGroup")]
        public Animator animatorGoldenChestTips;



        [ComponentBinder("ScrollView/Viewport/Content/RewardItems/CellSingleRewardTop")]
        public Transform transTopOneReward;
        [ComponentBinder("ScrollView/Viewport/Content/RewardItems/CellSingleLimitedTop")]
        public Transform transTopOneLimited;
        [ComponentBinder("ScrollView/Viewport/Content/RewardItems/CellDoubleRewardTop")]
        public Transform transTopDoubleReward;

        [ComponentBinder("ScrollView/Viewport/Content/RewardItems/CellSingleRewardDown")]
        public Transform transDownOneReward;
        [ComponentBinder("ScrollView/Viewport/Content/RewardItems/CellSingleLimitedDown")]
        public Transform transDownOneLimited;
        [ComponentBinder("ScrollView/Viewport/Content/RewardItems/CellDoubleRewardDown")]
        public Transform transDownDoubleReward;

        [ComponentBinder("ProgressGroup/LevelTag/NormalState")]
        public Transform transNextLevel;
        [ComponentBinder("ProgressGroup/LevelTag/NormalState/LevelNumText")]
        public TextMeshProUGUI txtNextLevel;
        [ComponentBinder("ProgressGroup/LevelTag/GoldenChestState")]
        public Transform transNextLevelGoldenChest;
        [ComponentBinder("ProgressGroup/LevelTag/GoldenChestState/GoldenChestLevelNumText")]
        public TextMeshProUGUI txtNextLevelGoldenChest;

        public SeasonPassRewardListView rewardListView;

        private SeasonPassController _passController;
        public SeasonPassUserGuide UserGuide;
        public DailyMissionMainViewController _DailyMissionMainViewController;

        public SeasonPassPage()
        {
            _passController = Client.Get<SeasonPassController>();
        }
        public SeasonPassPage(string address) : base(address)
        {
            _passController = Client.Get<SeasonPassController>();
        }

        public void InitWith(DailyMissionMainViewController mainViewController)
        {
            _DailyMissionMainViewController = mainViewController;
            UserGuide = AddChild<SeasonPassUserGuide>(transform);
            rewardListView = AddChild<SeasonPassRewardListView>(ScrollRect.transform);
            rewardListView.InitView(viewController);
            RefreshRewardUI();
        }

        public void CheckAndShowUserGuide(Action action, Action finish)
        {
            UserGuide?.viewController.CheckAndShowGuide(action, finish);
        }

        public void DestroyListView()
        {
            rewardListView.viewController.DestroyListView();
        }

        private void RefreshCommonUI()
        {
            btnCollect.interactable = true;
            buyLevelsButton.interactable = true;
            btnUnlockMissionPass.interactable = true;
            seasonNumText.text = _passController.Season.ToString();
            RefreshStarPoint(_passController.Exp * 1f / _passController.ExpTotal, 0.3f);
            btnCollect.gameObject.SetActive(_passController.CollectRewardCount > 0);
            btnUnlockMissionPass.gameObject.SetActive(!_passController.Paid);
            var needShowBox = _passController.IsLevel100 && _passController.Paid;
            transNextLevel.gameObject.SetActive(!needShowBox);
            transNextLevelGoldenChest.gameObject.SetActive(needShowBox);
            buyLevelsButton.gameObject.SetActive(!_passController.IsLevel100);

            if (_passController.IsLevel100)
            {
                txtNextLevelGoldenChest.text = (_passController.Level - 100 + 1).ToString();
            }
            txtNextLevel.text = (_passController.Level + 1).ToString();
            rewardListView.viewController.RefreshAllItems();
        }

        public void RefreshRewardUI()
        {
            RefreshCommonUI();
        }

        public void RefreshStarPoint(float value, float duration = 0)
        {
            progressBarStar.value = Mathf.Max(0.01f, value - 0.01f);
            progressBarStar.DOValue(value, duration);
            txtProgressBarStar.text = $"{_passController.Exp}/{_passController.ExpTotal}";
        }

        [ComponentBinder("TopGroup/InformationButton")]
        private void OnBtnInformationClick()
        {
            PopupStack.ShowPopup<SeasonPassHelp>();
        }

        [ComponentBinder("BottomGroup/BuyLevelsButton")]
        private async void OnBtnBuyLevelsClick()
        {
            buyLevelsButton.interactable = false;
            var popup = await PopupStack.ShowPopup<SeasonPassBuyLevel>();
            popup.SubscribeCloseAction(() =>
            {
                buyLevelsButton.interactable = true;
            });
        }
        [ComponentBinder("BottomGroup/UnlockMissionPassButton")]
        private async void OnBtnUnLockSeasonPassClick()
        {
            btnUnlockMissionPass.interactable = false;
            var popup = await PopupStack.ShowPopup<SeasonPassPurchaseGolden>();
            popup.SubscribeCloseAction(() =>
            {
                btnUnlockMissionPass.interactable = true;
            });
        }
        [ComponentBinder("BottomGroup/CollectButton")]
        private async void OnBtnCollectClick()
        {
            btnCollect.interactable = false;
            if (Client.Get<SeasonPassController>().Paid)
            {
                var popup = await PopupStack.ShowPopup<SeasonPassRewardGolden>();
                popup.InitRewards(null, true);
                popup.SubscribeCloseAction(() =>
                {
                    btnCollect.interactable = true;
                });
            }
            else
            {
                var popup = await PopupStack.ShowPopup<SeasonPassRewardFree>();
                popup.InitRewards(null, false);
                popup.SubscribeCloseAction(() =>
                {
                    btnCollect.interactable = true;
                });
            }
        }
    }

    public class SeasonPassPageViewController : ViewController<SeasonPassPage>
    {
        private bool newDataFetched = false;
        public SeasonPassUserGuide UserGuide => view.UserGuide;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            HideGoldenChestTip();
            EnableUpdate(1);
            BindScrollEvent();
        }

        private void BindScrollEvent()
        {
            view.ScrollRect.onValueChanged.AddListener(x =>
            {
                view._DailyMissionMainViewController.HideBubble();
            });

        }

        public async void FetchNewestPassData(bool force = false)
        {
            if (!newDataFetched || force)
            {
                newDataFetched = true;
                await Client.Get<SeasonPassController>().RefreshSeasonPassData();
                OnSeasonPassUpdate(new EventSeasonPassUpdate());
            }
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSeasonPassUpdate>(OnSeasonPassUpdate);
            SubscribeEvent<EventActivityExpire>(OnEventActivityExpire);
        }

        protected void OnEventActivityExpire(EventActivityExpire evt)
        {
            FetchNewestPassData(true);
        }
        public void OnSeasonPassUpdate(EventSeasonPassUpdate evt)
        {
            view.RefreshRewardUI();
            view.rewardListView.viewController.SetScroll(false);
            view.CheckAndShowUserGuide(() =>
            {
                view.rewardListView.viewController.MoveToCurrentLevel();
            }, () =>
            {
                view.rewardListView.viewController.SetScroll(true);
            });
        }

        private bool _isRefreshing;
        public override async void Update()
        {
            base.Update();
            if (!UserGuide.viewController.CanShowGuide() && !_isRefreshing && view.transform.gameObject.activeSelf && Client.Get<SeasonPassController>().IsNewSeason())
            {
                _isRefreshing = true;
                var popup = await PopupStack.ShowPopup<SeasonPassSeasonBegin>();
                popup.SubscribeCloseAction(() =>
                {
                    _isRefreshing = false;
                    EventBus.Dispatch(new EventSeasonPassUpdate());
                    view.rewardListView.viewController.MoveToCurrentLevel();
                });
            }
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

        public Transform NewCellItem(bool isGolden, int rewardNum, bool isLimited = false)
        {
            if (isGolden)
            {
                if (rewardNum == 2)
                {
                    return GameObject.Instantiate(view.transTopDoubleReward);
                }
                return GameObject.Instantiate(isLimited ? view.transTopOneLimited : view.transTopOneReward);
            }
            if (rewardNum == 2)
            {
                return GameObject.Instantiate(view.transDownDoubleReward);
            }
            return GameObject.Instantiate(isLimited ? view.transDownOneLimited : view.transDownOneReward);
        }

        public void PopupBubble(Transform trans)
        {
            view._DailyMissionMainViewController.PopupBubble(trans);
        }

        public void UpdateBubbleContent(Transform trans, string txtContent)
        {
            view._DailyMissionMainViewController.UpdateBubbleContent(trans, txtContent);
        }

        public void ToggleGoldenChestTips(Vector3 position, bool toTop)
        {
            if (view.animatorGoldenChestTips.gameObject.activeSelf == false)
            {
                view.animatorGoldenChestTips.gameObject.SetActive(true);
            }
            else
            {
                var stateInfo = view.animatorGoldenChestTips.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime >= 0.9f)
                {
                    view.animatorGoldenChestTips.Play("Open", 0, 0);
                }
                else
                {
                    view.animatorGoldenChestTips.Play("Open", 0, 1);
                }
            }

            if (toTop)
            {
                view.canvasGoldenChestTips.overrideSorting = true;
                view.canvasGoldenChestTips.sortingOrder = 21;
                view.canvasGoldenChestTips.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            }
            else
            {
                view.canvasGoldenChestTips.overrideSorting = false;
                view.canvasGoldenChestTips.sortingOrder = 1;
                view.canvasGoldenChestTips.sortingLayerID = SortingLayer.NameToID("UI");
            }
            view.canvasGoldenChestTips.transform.localPosition = view.canvasGoldenChestTips.transform.parent.InverseTransformPoint(position);
        }

        public void HideGoldenChestTip()
        {
            view.animatorGoldenChestTips.gameObject.SetActive(false);
        }
    }
}