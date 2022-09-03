//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 17:05
//  Ver : 1.0.0
//  Description : UIDailyMissionMainPopup.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIDailyMissionMainH", "UIDailyMissionMainV")]
    public class DailyMissionMainPopup : Popup<DailyMissionMainViewController>
    {
        [ComponentBinder("RootScale")]
        public Transform transRoot;
        [ComponentBinder("RootScale/Root/MissionContainer")]
        public Transform transMissionContainer;
        [ComponentBinder("RootScale/Root/PassContainer")]
        public Transform transPassContainer;

        [ComponentBinder("RootScale/Root/CrazeSmashContainer")]
        public Transform transCrazeSmashContainer;

        public View viewMissionContainer;
        public View viewPassContainer;
        public View viewCrazeSmashContainer;

        public Transform transRewardNormal;

        [ComponentBinder("RootScale/Cell_MissionPoint/zhuti/CountText")]
        public TextMeshProUGUI txtMissionPoint;

        [ComponentBinder("RootScale/Cell_MissToPass/zhuti/CountText")]
        public TextMeshProUGUI txtMissionStar;

        [ComponentBinder("RootScale/Root/ToggleGroup/MissionPassToggle/BubbleAnchor")]
        public Transform transSeasonPassBubbleAnchor;

        [ComponentBinder("RootScale/Root/ToggleGroup/CrazeSmashToggle/BubbleAnchor")]
        public Transform transCrazeSmashBubbleAnchor;

        [ComponentBinder("RootScale/Root/ToggleGroup/MissionPassToggle/ReminderGroup")]
        public Transform transMissionPassReminder;

        [ComponentBinder("RootScale/Root/ToggleGroup/DailyMissionToggle/ReminderGroup")]
        public Transform transDailyMissionReminder;

        [ComponentBinder("RootScale/Root/ToggleGroup/CrazeSmashToggle/ReminderGroup")]
        public Transform transCrazeSmashReminder;

        [ComponentBinder("RootScale/Root/ToggleGroup/MissionPassToggle/ReminderGroup/NoticeText")]
        public TextMeshProUGUI txtMissionPassNotice;

        [ComponentBinder("RootScale/Root/ToggleGroup/DailyMissionToggle/ReminderGroup/NoticeText")]
        public TextMeshProUGUI txtDailyMissionNotice;

        [ComponentBinder("RootScale/Root/ToggleGroup/CrazeSmashToggle/ReminderGroup/NoticeText")]
        public TextMeshProUGUI txtCrazeSmashNotice;

        [ComponentBinder("RootScale/Root/ToggleGroup/DailyMissionToggle")]
        public Toggle toggleDailyMission;

        [ComponentBinder("RootScale/Root/ToggleGroup/MissionPassToggle")]
        public Toggle toggleSeasonPass;

        [ComponentBinder("RootScale/Root/ToggleGroup/CrazeSmashToggle")]
        public Toggle toggleCrazeSmash;

        [ComponentBinder("RootScale/Root/ToggleGroup/ButtonSeasonPassLock")]
        public Button btnSeasonPassLock;

        [ComponentBinder("RootScale/Root/ToggleGroup/ButtonCrazeSmashLock")]
        public Button btnCrazeSmashLock;

        [ComponentBinder("RootScale/UIDailyMissionUnlockBubbleH")]
        public Animator animatorLockBubble;

        [ComponentBinder("RootScale/UIDailyMissionUnlockBubbleH/Root/ContentGroup/ContentText")]
        public TextMeshProUGUI txtPassLevelUp;
        [ComponentBinder("RootScale/UISeasonPassCommonBubble")]
        public Transform transBubble;
        [ComponentBinder("RootScale/UISeasonPassCommonBubble/Root")]
        public Animator animatorBubble;
        [ComponentBinder("RootScale/UISeasonPassCommonBubble/Root/ContentText")]
        public TextMeshProUGUI txtBubbleContent;

        [ComponentBinder("RootScale/Root/TimerGroup")]
        public Animator animatorTimeLeft;

        [ComponentBinder("RootScale/Root/TimerGroup/Root/TimerText")]
        public TextMeshProUGUI txtTimeLeft;

        [ComponentBinder("RootScale/Root/BGGroup/PanelBG/zhen")]
        public Transform transformZhen;



        [ComponentBinder("RootScale/Cell_MissionPoint")]
        public Canvas canvasMissionPoint;

        [ComponentBinder("RootScale/Cell_MissToPass")]
        public Canvas canvasMissionStar;

        [ComponentBinder("RootScale/CellMissionToSmash")]
        public Canvas canvasHammer;

        [ComponentBinder("RootScale/GrayMask/GrayMask")]
        public Transform transGrayMask;

        public DailyMissionPage DailyMissionPage;
        public SeasonPassPage SeasonPassPage;
        public CrazeSmashPage CrazeSmashPage;

        public DailyMissionRewardNormal DailyMissionRewardNormal;

        public DailyMissionMainPopup(string address) : base(address) { }

        protected override void BindingComponent()
        {
            base.BindingComponent();

            closeButton = transform.Find("RootScale/Root/TopGroup/CloseButton").GetComponent<Button>();
            viewMissionContainer = AddChild<View>(transMissionContainer);
            viewPassContainer = AddChild<View>(transPassContainer);
            viewCrazeSmashContainer = AddChild<View>(transCrazeSmashContainer);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            RefreshUI();
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            transRoot.localScale = CalculateRootScaleInfo();
        }

        public Vector3 CalculateRootScaleInfo()
        {
            if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
            {
                float scale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                return Vector3.one * scale;
            }
            return Vector3.one;
        }

        public void RefreshUI()
        {
            if (SeasonPassPage != null) { SeasonPassPage.RefreshRewardUI(); }

            txtMissionPassNotice.text = "";
            transMissionPassReminder.gameObject.SetActive(false);
            var finishMissionCount = Client.Get<DailyMissionController>().GetFinishedMissionCount();
            transDailyMissionReminder.gameObject.SetActive(finishMissionCount > 0);
            txtDailyMissionNotice.text = finishMissionCount.ToString();
            var collectRewardCount = Client.Get<SeasonPassController>().CollectRewardCount;
            transMissionPassReminder.gameObject.SetActive(collectRewardCount > 0);
            txtMissionPassNotice.text = collectRewardCount.ToString();

            txtCrazeSmashNotice.gameObject.SetActive(false);
            var crazeSmashController = Client.Get<CrazeSmashController>();
            var reminderEnable = crazeSmashController.available
                    && (!crazeSmashController.goldGameFinish || !crazeSmashController.silverGameFinish);
            transCrazeSmashReminder.gameObject.SetActive(reminderEnable);

            if (CrazeSmashPage != null) { CrazeSmashPage.Set(); }
        }

        public void PlayCollectAnimation(ulong point, bool isMissionPoint)
        {
            txtMissionStar.text = point.ToString();
            txtMissionPoint.text = point.ToString();

            string stateName = null;
            var isPortrait = ViewManager.Instance.IsPortrait;
            if (isPortrait)
            {
                stateName = isMissionPoint ? "MissionPoint_OpenV" : "Cell_MissToPassV";
            }
            else
            {
                stateName = isMissionPoint ? "MissionPoint_Open" : "Cell_MissToPass";
            }
            XUtility.PlayAnimation(animator, stateName);
        }

        public void PlayHammerAnimation()
        {
            var isPortrait = ViewManager.Instance.IsPortrait;
            var stateName = isPortrait ? "Cell_MissToSmashV" : "Cell_MissToSmash";
            XUtility.PlayAnimation(animator, stateName);
        }

        public void ModifyMissionGroupSortingOrder(bool toTop, bool isMissionPoint)
        {
            var canvas = isMissionPoint ? canvasMissionPoint : canvasMissionStar;
            if (toTop)
            {
                canvas.gameObject.SetActive(true);
                canvas.overrideSorting = true;
                canvas.sortingOrder = 6;
                canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                return;
            }

            canvas.overrideSorting = false;
            canvas.sortingOrder = 1;
            canvas.sortingLayerID = SortingLayer.NameToID("UI");
        }

        public override void Close()
        {
            animatorLockBubble.gameObject.SetActive(false);
            SeasonPassPage?.DestroyListView();
            SoundController.RecoverLastMusic();
            base.Close();
        }

        // public override bool NeedForceLandscapeScreen() { return true; }

        public override float GetPopUpMaskAlpha() { return 0f; }
    }

    public class DailyMissionMainViewController : ViewController<DailyMissionMainPopup>
    {
        private Toggle selectedToggle;
        public List<Transform> ListMissionPages;
        public List<Toggle> ListToggles;
        private Transform transformBubblePosition;
        private bool isLoading = false;
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSeasonPassUpdate>(OnSeasonPassUpdate);
            SubscribeEvent<Event_CrazeSmash_EggInfoChanged>(OnEvent_CrazeSmash_EggInfoChanged);
            SubscribeEvent<Event_CrazeSmash_BIG_WIN>(OnEvent_CrazeSmash_BigWin);
        }

        private async void OnEvent_CrazeSmash_BigWin(Event_CrazeSmash_BIG_WIN obj)
        {
            RefreshCrazeSmashNotice();
            var controller = Client.Get<CrazeSmashController>();
            if (controller == null || controller.eggInfo == null) { return; }
            var eggInfo = controller.eggInfo;
            ulong missionStar = 0;
            if (controller.playGoldGame)
            {
                if (eggInfo.GoldTotalFinalReward != null)
                {
                    foreach (var item in eggInfo.GoldTotalFinalReward.Items)
                    {
                        if (item.Type == Item.Types.Type.MissionStar)
                        {
                            missionStar = item.MissionStar.Amount;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (eggInfo.SilverTotalFinalReward != null)
                {
                    foreach (var item in eggInfo.SilverTotalFinalReward.Items)
                    {
                        if (item.Type == Item.Types.Type.MissionStar)
                        {
                            missionStar = item.MissionStar.Amount;
                            break;
                        }
                    }
                }
            }
            if (missionStar == 0) { return; }
            await FlyStarOrPoint(missionStar, false);
        }

        private void OnEvent_CrazeSmash_EggInfoChanged(Event_CrazeSmash_EggInfoChanged obj)
        {
            RefreshCrazeSmashNotice();
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            EnableUpdate(1);
            BindToggles();
            view.animatorLockBubble.gameObject.SetActive(false);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            SoundController.PlayBgMusic("MissionBgMusic");
        }

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>();
           
            if (view.GetAssetAddressName().Contains("UIDailyMissionMainH"))
            {
                extraAssetNeedToLoad.Add("UISeasonPassMainGroupH");
                extraAssetNeedToLoad.Add("DailyMissionMainGroup");
                extraAssetNeedToLoad.Add("CrazeSmashMainGroup");
            }
            else
            {
                extraAssetNeedToLoad.Add("UISeasonPassMainGroupV");
                extraAssetNeedToLoad.Add("DailyMissionMainGroupV");
                extraAssetNeedToLoad.Add("CrazeSmashMainGroupV");
            }

            // extraAssetNeedToLoad.Add("UISeasonPassHelpH");
            // extraAssetNeedToLoad.Add("UISeasonPassHelpV");
            // extraAssetNeedToLoad.Add("UIDailyMissionHelpH");
            // extraAssetNeedToLoad.Add("UIDailyMissionHelpV");
            // extraAssetNeedToLoad.Add("UICrazeSmashHelp");
            // extraAssetNeedToLoad.Add("UICrazeSmashHelpV");

            await base.LoadExtraAsyncAssets();
            // CheckAndSetLockCrazeSmashUI();
            // await CheckAndSetLockSeasonPassUI();
            await InitializeState();
            view.btnSeasonPassLock.onClick.AddListener(OnBtnSeasonPassLockClick);
            view.btnCrazeSmashLock.onClick.AddListener(OnBtnCrazeSmashLockClick);
        }

        private void BindToggles()
        {
            ListMissionPages = new List<Transform>();
            string[] states = { "EnableState", "DisableState" };
            string[] pages = { "DailyMissionToggle", "MissionPassToggle", "CrazeSmashToggle" };
            for (int i = 0; i < states.Length; i++)
            {
                for (int j = 0; j < pages.Length; j++)
                {
                    ListMissionPages.Add(view.transform.Find($"RootScale/Root/ToggleGroup/{pages[j]}/{states[i]}"));
                }
            }
            ListToggles = new List<Toggle>();
            for (int i = 0; i < pages.Length; i++)
            {
                var toggle = view.transform.Find($"RootScale/Root/ToggleGroup/{pages[i]}").GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(x => OnSelectItem(toggle));
                ListToggles.Add(toggle);
            }
        }

        public async void CheckAndCollectFinishedNormalMission()
        {
            await XUtility.WaitSeconds(0.5f, this);
            for (int i = 0; i < 3; i++)
            {
                var mission = Client.Get<DailyMissionController>().GetNormalMission(i);
                if (mission.IsFinish() && !mission.IsClaimed())
                {
                    TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
                    view.DailyMissionPage.ClaimMission(i, false, () =>
                    {
                        task.TrySetResult(true);
                    });
                    await task.Task;
                }
            }
        }

        public async Task CheckAndCollectStageReward()
        {
            var stageRewards = Client.Get<DailyMissionController>().GetStageRewards();
            for (int i = 0; i < stageRewards.Count; i++)
            {
                TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();
                var stageReward = stageRewards[i];
                var popup = await PopupStack.ShowPopup<DailyMissionRewardWeek>();
                popup.Init(this);
                popup.InitializeReward(stageReward);
                popup.SubscribeCloseAction(() =>
                {
                    ItemSettleHelper.SettleItems(stageReward.Reward.Items);
                    task.SetResult(true);
                });
                await task.Task;
            }
        }

        public async void OnSelectItem(Toggle toggle)
        {
            if (!toggle.isOn) return;
            if (isLoading) return;
            if (toggle != selectedToggle)
            {
                SoundController.PlaySwitchFx();
            }
            else
            {
                return;
            }

            selectedToggle = toggle;
            for (int i = 0; i < ListToggles.Count; i++)
            {
                var toggleItem = ListToggles[i];
                ListMissionPages[i].gameObject.SetActive(toggleItem == toggle);
                ListMissionPages[i + ListToggles.Count].gameObject.SetActive(!(toggleItem == toggle));
            }

            await LoadPageView(toggle);

            view.animatorTimeLeft.gameObject.SetActive(view.toggleCrazeSmash != toggle);
            if (view.transformZhen != null)
            {
                view.transformZhen.gameObject.SetActive(view.toggleCrazeSmash != toggle);
            }

            if (toggle == view.toggleCrazeSmash)
            {
               // var crazeSmashController = Client.Get<CrazeSmashController>();
              //  await crazeSmashController.SendCGetEggInfo();
                RefreshCrazeSmashNotice();
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashPop, ("source", "2"));

                if (toggle == view.toggleCrazeSmash)
                {
                    view.CrazeSmashPage.SetToMain();
                }
            }
            else if (toggle == view.toggleSeasonPass)
            {
                EventBus.Dispatch(new EventSeasonPassUpdate());
            }
            else if (toggle == view.toggleDailyMission)
            {
                CheckAndCollectFinishedNormalMission();
            }

            if (view.SeasonPassPage != null)
            {
                view.SeasonPassPage.transform.gameObject.SetActive(view.toggleSeasonPass == toggle);
            }

            if (view.DailyMissionPage != null)
            {
                view.DailyMissionPage.transform.gameObject.SetActive(view.toggleDailyMission == toggle);
            }

            if (view.CrazeSmashPage != null)
            {
                view.CrazeSmashPage.transform.gameObject.SetActive(view.toggleCrazeSmash == toggle);
            }
        }

        private AssetReference GetSeasonPassPageAsset()
        {
            var isPortrait = ViewManager.Instance.IsPortrait;
            var address = isPortrait ? "UISeasonPassMainGroupV" : "UISeasonPassMainGroupH";
            return GetAssetReference(address);
        }


        private AssetReference GetDailyMissionPageAsset()
        {
            var isPortrait = ViewManager.Instance.IsPortrait;
            var address = isPortrait ? "DailyMissionMainGroupV" : "DailyMissionMainGroup";
            return GetAssetReference(address);
        }


        private AssetReference GetCrazeSmashPageAsset()
        {
            var isPortrait = ViewManager.Instance.IsPortrait;
            var address = isPortrait ? "CrazeSmashMainGroupV" : "CrazeSmashMainGroup";
            return GetAssetReference(address);
        }

        private async Task LoadPageView(Toggle toggle)
        {
            SetCloseBtnEnabled(false);
            if (view.toggleSeasonPass == toggle && view.SeasonPassPage == null)
            {
                isLoading = true;
                // var assetReference = GetAssetReference("UISeasonPassMainGroupH");
                var assetReference = GetSeasonPassPageAsset();
                var transPage = assetReference.InstantiateAsset<GameObject>().transform;
                transPage.SetParent(view.transPassContainer, false);
                view.SeasonPassPage = view.viewPassContainer.AddChild<SeasonPassPage>(transPage);
               
                view.SeasonPassPage.InitWith(this);
                view.SeasonPassPage.viewController.FetchNewestPassData();
                
                view.RefreshUI();
                isLoading = false;
            }
            if (view.toggleDailyMission == toggle && view.DailyMissionPage == null)
            {
                isLoading = true;
                // var assetReference = GetAssetReference("DailyMissionMainGroup");
                var assetReference = GetDailyMissionPageAsset();

                var transPage = assetReference.InstantiateAsset<GameObject>().transform;
                transPage.SetParent(view.transMissionContainer, false);
                view.DailyMissionPage = view.viewMissionContainer.AddChild<DailyMissionPage>(transPage);
               
                view.DailyMissionPage.InitWith(this);
                view.RefreshUI();
                isLoading = false;
            }

            if (view.toggleCrazeSmash == toggle && view.CrazeSmashPage == null)
            {
                isLoading = true;
                var assetReference = GetCrazeSmashPageAsset();
                var transPage = assetReference.InstantiateAsset<GameObject>().transform;
                transPage.SetParent(view.transCrazeSmashContainer, false);
                view.CrazeSmashPage = view.viewCrazeSmashContainer.AddChild<CrazeSmashPage>(transPage);
                
                view.CrazeSmashPage.SetToMain();
                view.RefreshUI();
                isLoading = false;
            }

            SetCloseBtnEnabled(true);
        }

        private void SetCloseBtnEnabled(bool enable)
        {
            if (view.closeButton) { view.closeButton.enabled = enable; }
        }

        private void CheckAndSetLockCrazeSmashUI()
        {
            view.btnCrazeSmashLock.gameObject.SetActive(false);
            var controller = Client.Get<CrazeSmashController>();
            if (controller == null) { return; }
            if (controller.levelReached == false)
            {
                view.toggleCrazeSmash.interactable = false;
                view.btnCrazeSmashLock.gameObject.SetActive(true);
                SetChildrenImageGray(view.toggleCrazeSmash);
            }
        }

        private async Task InitializeState()
        {
            var extraObj = (string[])((PopupArgs)extraData).extraArgs;
            var type = extraObj[0];
            var source = extraObj[1];

            view.btnSeasonPassLock.gameObject.SetActive(false);
            view.btnCrazeSmashLock.gameObject.SetActive(false);

            var crazeSmashController = Client.Get<CrazeSmashController>();
            if (crazeSmashController.levelReached == false)
            {
                view.toggleCrazeSmash.interactable = false;
                view.btnCrazeSmashLock.gameObject.SetActive(true);
                SetChildrenImageGray(view.toggleCrazeSmash);
            }

            var seasonPassController = Client.Get<SeasonPassController>();
            if (seasonPassController.IsLocked)
            {
                view.toggleSeasonPass.interactable = false;
                view.btnSeasonPassLock.gameObject.SetActive(true);
                SetChildrenImageGray(view.toggleSeasonPass);
            }

            Toggle toggle = view.toggleDailyMission;
            switch (type)
            {
                case "SeasonPass":
                    if (seasonPassController.IsLocked == false)
                    {
                        toggle = view.toggleSeasonPass;
                        BiManagerGameModule.Instance.SendGameEvent(
                            BiEventFortuneX.Types.GameEventType.GameEventMissionPassPop,
                            ("source", source)
                        );
                        await XUtility.WaitNFrame(3);
                    }
                    break;
                case "DailyMission":
                    toggle = view.toggleDailyMission;

                    BiManagerGameModule.Instance.SendGameEvent(
                        BiEventFortuneX.Types.GameEventType.GameEventDailyMissionPop,
                        ("source", source),
                        ("normalSchedule", Client.Get<DailyMissionController>().GetNormalMissionFinishedCount().ToString())
                    );
                    break;
                case "CrazeSmash":
                    if (crazeSmashController.locked == false)
                    {
                        toggle = view.toggleCrazeSmash;
                    }
                    break;
            }

            view.toggleSeasonPass.isOn = view.toggleSeasonPass == toggle;
            view.toggleDailyMission.isOn = view.toggleDailyMission == toggle;
            view.toggleCrazeSmash.isOn = view.toggleCrazeSmash == toggle;

            OnSelectItem(toggle);
        }

        private void SetChildrenImageGray(Component root)
        {
            var images = root.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i].sprite)
                {
                    images[i].color = new Color(0.415f, 0.415f, 0.415f, 1);
                }
            }
        }

        public async Task ShowCollectStateMissionPoint(ulong ulMissionPoint, ulong uStarPoint)
        {
            RefreshAfterClaim();
            await ShowCollectMissionPoint(ulMissionPoint, uStarPoint);
        }

        public async Task ShowCollectHonorMissionPoint(ulong ulMissionPoint, ulong uStarPoint)
        {
            RefreshAfterClaim();
            await ShowCollectMissionPoint(ulMissionPoint, uStarPoint);
            await CheckAndCollectStageReward();
        }
        public async Task ShowCollectNormalMissionPoint(ulong ulMissionPoint, ulong uStarPoint, ulong uHammer)
        {
            RefreshAfterClaim();
            await ShowCollectMissionPoint(ulMissionPoint, uStarPoint);
            if (uHammer > 0)
            {
                await Client.Get<CrazeSmashController>().SendCGetEggInfo();
                await FlyHammer();
                BiManagerGameModule.Instance.SendGameEvent(
                    BiEventFortuneX.Types.GameEventType.GameEventCrazeSmashCollect, ("type", $"{2}"));
            }
            await CheckAndCollectStageReward();
        }

        private void ModifyCollectMissionGroup(bool isTop, bool isMissionPoint)
        {
            view.ModifyMissionGroupSortingOrder(isTop, isMissionPoint);
            view.DailyMissionPage?.ModifyMissionGroupSortingOrder(isTop);
        }

        private void EnableToggles(bool enabled)
        {
            view.closeButton.interactable = enabled;
            view.toggleSeasonPass.interactable = enabled;
            view.toggleDailyMission.interactable = enabled;
        }

        public async Task ShowCollectMissionPoint(ulong ulMissionPoint, ulong uStarPoint)
        {
            EnableToggles(false);
            if (ulMissionPoint > 0)
            {
                await FlyStarOrPoint(ulMissionPoint, true);
            }

            if (uStarPoint > 0)
            {
                await FlyStarOrPoint(uStarPoint, false);
            }

            EventBus.Dispatch(new EventDailyMissionUpdate());
            EnableToggles(true);
        }

        private async Task FlyHammer()
        {
            // TODO : replace sfx name
            SoundController.PlaySfx("Mission_MissionPoint");
            view.PlayHammerAnimation();

            view.canvasHammer.gameObject.SetActive(true);
            view.canvasHammer.overrideSorting = true;
            view.canvasHammer.sortingOrder = 6;
            view.canvasHammer.sortingLayerID = SortingLayer.NameToID("SystemPopup");

            view.transGrayMask.parent.gameObject.SetActive(true);
            view.transGrayMask.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            await XUtility.WaitSeconds(0.3f, this);
            view.transGrayMask.GetComponent<Image>().color = new Color(0, 0, 0, 0.627451f);
            await XUtility.WaitSeconds(1.1f, this);
            view.transGrayMask.parent.gameObject.SetActive(false);

            view.canvasHammer.overrideSorting = false;
            view.canvasHammer.sortingOrder = 1;
            view.canvasHammer.sortingLayerID = SortingLayer.NameToID("UI");
        }

        private async Task FlyStarOrPoint(ulong ulMissionPoint, bool isMissionPoint)
        {
            uint preLevel = 0;
            if (!isMissionPoint)
            {
                preLevel = Client.Get<SeasonPassController>().Level;
                await Client.Get<SeasonPassController>().GetSeasonPassData();
            }
            view.transGrayMask.parent.gameObject.SetActive(true);
            view.transGrayMask.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            await XUtility.WaitSeconds(0.3f, this);
            view.transGrayMask.GetComponent<Image>().color = new Color(0, 0, 0, 0.627451f);
            view.PlayCollectAnimation(ulMissionPoint, isMissionPoint);
            ModifyCollectMissionGroup(true, isMissionPoint);
            SoundController.PlaySfx(isMissionPoint ? "Mission_MissionPoint" : "Mission_PassPoint");
            await XUtility.WaitSeconds(1.1f, this);
            if (isMissionPoint)
            {
                view.DailyMissionPage?.RefreshStagePoint(0.5f);
                await XUtility.WaitSeconds(0.5f, this);
                view.transGrayMask.parent.gameObject.SetActive(false);
            }
            else
            {
                await XUtility.WaitSeconds(0.6f, this);
                view.transGrayMask.parent.gameObject.SetActive(false);
                view.RefreshUI();
                CheckShowSeasonPassLevelUp(preLevel);
            }
            ModifyCollectMissionGroup(false, isMissionPoint);
        }

        public void RefreshAfterClaim()
        {
            view.RefreshUI();
            view.DailyMissionPage?.RefreshItems();
        }

        public override void Update()
        {
            base.Update();
            if (view.txtTimeLeft)
            {
                if (selectedToggle == view.toggleDailyMission)
                {
                    view.txtTimeLeft.text = Client.Get<DailyMissionController>().GetNormalMissionTimeLeft();
                }
                else if (selectedToggle == view.toggleSeasonPass)
                {
                    view.txtTimeLeft.text = Client.Get<SeasonPassController>().GetSeasonPassTimeLeft();
                }
            }

            if (selectedToggle == view.toggleCrazeSmash)
            {
                view.CrazeSmashPage.RefreshTime();
            }

            RefreshCrazeSmashNotice();
        }

        private void RefreshCrazeSmashNotice()
        {
            var controller = Client.Get<CrazeSmashController>();
            if (controller == null) { return; }
            var reminderEnable = controller.available
                    && (!controller.goldGameFinish || !controller.silverGameFinish);

            view.transCrazeSmashReminder.gameObject.SetActive(reminderEnable);
        }

        public void OnSeasonPassUpdate(EventSeasonPassUpdate evt)
        {
            view.RefreshUI();
        }

        public DailyMissionRewardNormal GetNormalRewardPopup()
        {
            return view.DailyMissionRewardNormal;
        }

        public async Task LoadNormalReward()
        {
            if (view.DailyMissionRewardNormal == null)
            {
                var isPortrait = ViewManager.Instance.IsPortrait;
                var address = isPortrait ? "UIDailyMissionRewardCellCommonV" : "UIDailyMissionRewardCellCommonH";
                view.DailyMissionRewardNormal =
                    await view.AddChild<DailyMissionRewardNormal>(address);
            }
        }

        private async void OnBtnSeasonPassLockClick()
        {
            // PopupBubble(view.transPassLockBubblePos.transform);
            var strLock = $"Unlock at level {Client.Get<SeasonPassController>().UnlockLevel}";
            view.animatorLockBubble.transform.position = view.transSeasonPassBubbleAnchor.position;
            view.txtPassLevelUp.text = strLock;
            view.animatorLockBubble.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(view.animatorLockBubble, "Open", this);
            view.animatorLockBubble.gameObject.SetActive(false);
        }

        private async void OnBtnCrazeSmashLockClick()
        {
            var controller = Client.Get<CrazeSmashController>();
            if (controller == null || controller.eggInfo == null) { return; }
            var eggInfo = controller.eggInfo;
            view.animatorLockBubble.transform.position = view.transCrazeSmashBubbleAnchor.position;
            var strLock = $"Unlock at level {eggInfo.UnlockLevel}";
            view.txtPassLevelUp.text = strLock;
            view.animatorLockBubble.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(view.animatorLockBubble, "Open", this);
            view.animatorLockBubble.gameObject.SetActive(false);
        }

        private async void CheckShowSeasonPassLevelUp(uint preLevel)
        {
            if (Client.Get<SeasonPassController>().Level > preLevel && preLevel < 100)
            {
                view.animatorLockBubble.gameObject.SetActive(true);
                view.animatorLockBubble.transform.position = view.transSeasonPassBubbleAnchor.position;
                view.txtPassLevelUp.text = "NICE LEVEL UP!\nCHECK YOUR REWARD HERE!";
                await XUtility.PlayAnimationAsync(view.animatorLockBubble, "Open", this);
                view.animatorLockBubble.gameObject.SetActive(false);
            }
        }

        public async void PopupBubble(Transform trans)
        {
            transformBubblePosition = trans;
            view.transBubble.gameObject.SetActive(true);
            view.transBubble.position = trans.position;
            await XUtility.PlayAnimationAsync(view.animatorBubble, "Open", this);
            transformBubblePosition = null;
        }

        public void UpdateBubbleContent(Transform trans, string txtContent)
        {
            if (transformBubblePosition == trans)
            {
                view.txtBubbleContent.text = txtContent;
            }
        }

        public void HideBubble()
        {
            view.transBubble.gameObject.SetActive(false);
        }
    }
}