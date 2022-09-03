using System;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidDailyTaskView : View<TreasureRaidDailyTaskViewController>
    {
        [ComponentBinder("waikuang/Progress")] private Image progress;
        [ComponentBinder("waikuang/ProgressText")] private TextMeshProUGUI progressText;
        [ComponentBinder("waikuang/Complete")] private Transform completeTr;
        [ComponentBinder("NumberGroup")] public Transform numberGroup;

        private Button goBtn;

        private Tweener _tweener;
        private float lastBlood;
        private ulong lastValue;

        public void RefreshDailyTask(bool showAni, Action callback = null, MonopolyDailyTask dailyTask = null, bool showBooster = true, bool hasPuzzle = false)
        {
            if (transform == null)
                return;

            if (dailyTask == null)
            {
                dailyTask = viewController.activityTreasureRaid.GetDailyTask();
            }

            if (_tweener != null)
            {
                _tweener.Kill();
            }

            var value1 = (double)dailyTask.CurrentValue / dailyTask.TargetValue;

            var value = (float)Math.Floor(value1 * 100);
            var complete = dailyTask.Status == 1;
            progress.DOKill();
            if (showAni)
            {
                if (dailyTask.CurrentValue != lastValue)
                {
                    _tweener = DOTween.To( cur =>
                    {
                        progressText.SetText($"{(int)cur}%");
                    }, lastBlood, value, 0.3f);

                    progress.DOFillAmount(value/100, 0.3f).OnComplete(() =>
                    {
                        if (transform == null)
                            return;

                        progressText.gameObject.SetActive(!complete);
                        completeTr.gameObject.SetActive(complete);
                        CheckDailyTaskComplete(callback, dailyTask, showBooster, hasPuzzle);
                    });
                }
                else
                {
                    CheckDailyTaskComplete(callback, dailyTask, showBooster, hasPuzzle);
                }
            }
            else
            {
                progress.fillAmount = value/100;
                progressText.SetText($"{value}%");
                progressText.gameObject.SetActive(!complete);
                completeTr.gameObject.SetActive(complete);
                
                var curDailyTask = viewController.activityTreasureRaid.GetDailyTask();
                for (int i = 0; i < numberGroup.childCount; i++)
                {
                    numberGroup.GetChild(i).gameObject.SetActive(i + 1 == curDailyTask.TaskIndex);
                }
                
                callback?.Invoke();
            }
            lastBlood = value;
            lastValue = dailyTask.CurrentValue;
        }
        
        private void CheckDailyTaskComplete(Action callback, MonopolyDailyTask dailyTask = null, bool showBooster = true, bool hasPuzzle = false)
        {
            if (dailyTask == null)
            {
                dailyTask = viewController.activityTreasureRaid.GetDailyTask();
            }

            void EndCallback()
            {
                if (hasPuzzle)
                {
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidPuzzlePopup),true, callback)));
                }
                else
                {
                    callback?.Invoke();
                }
                if (viewController.needToRefresh)
                {
                    viewController.needToRefresh = false;
                    RefreshDailyTask(false);
                }
            }

            if (dailyTask.TaskRewardsGot != null)
            {
                // 展示UI
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidDailyTaskPopup), dailyTask, EndCallback, "Lobby")));
            }
            else
            {
                if (viewController.activityTreasureRaid.TicketCount <= 0 && showBooster && !hasPuzzle)
                {
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidBoosterPopup))));
                }
                EndCallback();
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            goBtn = transform.GetComponent<Button>();
            goBtn.onClick.AddListener(OnDailyTaskBtnClicked);
        }

        private void OnDailyTaskBtnClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidDailyTaskPopup))));
        }
    }

    public class TreasureRaidDailyTaskViewController : ViewController<TreasureRaidDailyTaskView>
    {
        public Activity_TreasureRaid activityTreasureRaid;
        private CancelableCallback _cancelableCallback;

        public bool needToRefresh = false;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            var dailyTask = activityTreasureRaid.GetDailyTask();
            var left = XUtility.GetLeftTime((ulong) dailyTask.ExpireAt * 1000);
            left = left < 0 ? 0 : left;
            if (left > 0)
            {
                _cancelableCallback = WaitForSeconds(left, async () =>
                {
                    await activityTreasureRaid.GetMonopolyDailyTask();
                    if (!activityTreasureRaid.GetInRequestState())
                    {
                        view.RefreshDailyTask(false);
                    }
                    else
                    {
                        needToRefresh = true;
                    }
                });
            }
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.RefreshDailyTask(false);
        }

        public override void OnViewDestroy()
        {
            if (_cancelableCallback != null)
            {
                _cancelableCallback.CancelCallback();
            }
            base.OnViewDestroy();
        }
    }
}