using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidDailyTask")]
    public class TreasureRaidDailyTaskPopup: Popup<TreasureRaidDailyTaskPopupController>
    {
        [ComponentBinder("Root/MainGroup/ContentGroup/CompleteContent")]
        private Transform completeGroup;
        
        [ComponentBinder("Root/MainGroup/ContentGroup/TaskGroup/TargetText")]
        private Text targetText;

        [ComponentBinder("Root/MainGroup/ContentGroup/TaskGroup")]
        private Transform taskGroup;
        
        [ComponentBinder("Root/MainGroup/ContentGroup/RewardContent")]
        private Transform rewardGroup;

        [ComponentBinder("Root/MainGroup/TitleGroup/headline")]
        private Transform numberGroup;

        [ComponentBinder("Root/MainGroup/BottonmGroup/LetGoBtn")]
        private Button letGoBtn;
        
        [ComponentBinder("Root/MainGroup/BottonmGroup/WellDownBtn")]
        private Button wellDownBtn;
        
        [ComponentBinder("Root/MainGroup/schedule_BG/Dynamic_schedule")]
        private Image taskProgress;
        
        [ComponentBinder("Root/MainGroup/TimerGroup/LeftTime")]
        private TextMeshProUGUI leftTime;

        public TreasureRaidDailyTaskPopup(string address)
            : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdaptScaleTransform(transform.Find("Root"), ViewResolution.designSize);
            letGoBtn.onClick.AddListener(OnCloseClicked);
            wellDownBtn.onClick.AddListener(OnCloseClicked);
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

        public void SetBtnState(bool interactable)
        {
            closeButton.interactable = interactable;
            letGoBtn.interactable = interactable;
            wellDownBtn.interactable = interactable;
        }

        public void UpdateLeftTime()
        {
            var dailyTask = viewController.dailyTask;
            var left = XUtility.GetLeftTime((ulong) dailyTask.ExpireAt * 1000);
            left = left < 0 ? 0 : left;
            leftTime.SetText(XUtility.GetTimeText(left));
        }

        public void RefreshUI()
        {
            var dailyTask = viewController.dailyTask;
            for (int i = 0; i < numberGroup.childCount; i++)
            {
                numberGroup.GetChild(i).gameObject.SetActive((i + 1) == dailyTask.TaskIndex);
            }

            if (dailyTask.TaskType == MonopolyDailyTask.Types.TaskType.Default)
            {
                taskGroup.gameObject.SetActive(false);
                completeGroup.gameObject.SetActive(true);
                letGoBtn.gameObject.SetActive(false);
                wellDownBtn.gameObject.SetActive(true);
                taskProgress.fillAmount = 1;
            }
            else
            {
                var isComplete = dailyTask.Status == 1;
                taskGroup.gameObject.SetActive(!isComplete);
                completeGroup.gameObject.SetActive(isComplete);
                letGoBtn.gameObject.SetActive(!isComplete);
                wellDownBtn.gameObject.SetActive(isComplete);
                if (!isComplete)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var task = taskGroup.GetChild(i);
                        bool isCurrent = (int) dailyTask.TaskType == (i + 1);
                        task.gameObject.SetActive(isCurrent);
                        if (isCurrent)
                        {
                            var target = dailyTask.TargetValue.ToString();
                            if (dailyTask.TaskType == MonopolyDailyTask.Types.TaskType.GetCoins)
                            {
                                target = dailyTask.TargetValue.GetAbbreviationFormat();
                            }
                            task.Find("TaskTarget").GetComponent<Text>().SetText(target);
                        }
                    }

                    var value = (float)dailyTask.CurrentValue / dailyTask.TargetValue;
                    taskProgress.fillAmount = value;
                }
                else
                {
                    taskProgress.fillAmount = 1;
                }
            }

            if (dailyTask.TaskType == MonopolyDailyTask.Types.TaskType.GetCoins)
            {
                targetText.SetText($"{dailyTask.CurrentValue.GetAbbreviationFormat()}/{dailyTask.TargetValue.GetAbbreviationFormat()}");
            }
            else
            {
                targetText.SetText($"{dailyTask.CurrentValue}/{dailyTask.TargetValue}");
            }
        }

        public async void ShowCollectAni()
        {
            SetBtnState(false);
            // 做一个收集满的动画
            await XUtility.WaitSeconds(1);
            var collectView = await PopupStack.ShowPopup<TreasureRaidDailyTaskCollectPopup>();
            collectView.InitRewardContent(viewController.dailyTask.TaskRewardsGot, viewController.dailyTask.TaskIndex);
            collectView.ShowRewardCollect( async () =>
            {
                if (transform == null)
                    return;
                viewController.dailyTask = viewController.activityTreasureRaid.GetDailyTask();
                viewController.showAni = false;
                RefreshUI();
                SetBtnState(true);
            });
        }
        
        public void InitRewardContent(Reward reward)
        {
            RepeatedField<Reward> list = new RepeatedField<Reward>();
            list.Add(reward);
            XItemUtility.InitRewardsUI(rewardGroup, list, rewardGroup.Find("RewardCell"), GetItemDefaultDescText, "SpacingType");
        }

        public static string GetItemDefaultDescText(Item item)
        {
            switch (item.Type)
            {
                case Item.Types.Type.Coin:
                    return ((long)item.Coin.Amount).GetAbbreviationFormat(0);
            }

            return "";
        }
    }

    public class TreasureRaidDailyTaskPopupController : ViewController<TreasureRaidDailyTaskPopup>
    {
        public bool showAni;

        public MonopolyDailyTask dailyTask;
        public Activity_TreasureRaid activityTreasureRaid;
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
            SubscribeEvent<EventTreasureRaidDailyTaskRefresh>(DailyTaskRefresh);
        }

        private void DailyTaskRefresh(EventTreasureRaidDailyTaskRefresh obj)
        {
            if (!showAni)
            {
                dailyTask = activityTreasureRaid.GetDailyTask();
                view.RefreshUI();
            }
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.Close();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                    Activity_TreasureRaid;
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            var args = inExtraData as PopupArgs;
            if (args?.extraArgs != null)
            {
                dailyTask = args.extraArgs as MonopolyDailyTask;
                showAni = true;
            }
            else
            {
                dailyTask = activityTreasureRaid?.GetDailyTask();
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            view.UpdateLeftTime();
            view.RefreshUI();
            if (showAni)
            {
                view.ShowCollectAni();
            }
            EnableUpdate(1);
            view.InitRewardContent(dailyTask.TaskRewards);
        }

        public override void Update()
        {
            base.Update();
            view.UpdateLeftTime();
        }
    }
}