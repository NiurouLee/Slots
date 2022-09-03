using System;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidPuzzleView : View<TreasureRaidPuzzleViewController>
    {
        [ComponentBinder("waikuang/Progress")] private Image progress;
        [ComponentBinder("waikuang/ProgressText")] private TextMeshProUGUI progressText;
        [ComponentBinder("waikuang/Complete")] private Transform completeTr;

        private Button goBtn;

        private float lastBlood;
        
        public void RefreshPuzzle(bool showAni,MonopolyPuzzle puzzleInfo = null, Action callback = null, bool showBooster = true)
        {
            if (transform == null)
                return;

            var puzzleIndex = 1;
            var lastPuzzleListInfo = viewController.activityTreasureRaid.GetLastPuzzleListInfo();
            if (puzzleInfo == null)
            {
                for (int i = 0; i < lastPuzzleListInfo.PuzzleList.Count; i++)
                {
                    if (lastPuzzleListInfo.PuzzleList[i].IsCurrent)
                    {
                        puzzleInfo = lastPuzzleListInfo.PuzzleList[i];
                        puzzleIndex = i + 1;
                    }
                }
            }

            if (puzzleInfo == null)
            {
                XDebug.LogError("Didn't has data to refresh puzzle view");
                return;
            }

            var count = 0;
            for (int i = 0; i < puzzleInfo.PositionsIsFill.Count; i++)
            {
                if (puzzleInfo.PositionsIsFill[i])
                {
                    count++;
                }
            }

            if (showAni)
            {
                count++;
            }
            var value1 = (double)count / puzzleInfo.PositionsIsFill.Count;
            var value = (float)Math.Floor(value1 * 100);
            var complete = puzzleIndex == lastPuzzleListInfo.PuzzleList.Count && count == puzzleInfo.PositionsIsFill.Count;
            progress.DOKill();
            progressText.SetText($"{count}/{puzzleInfo.PositionsIsFill.Count}");
            if (showAni)
            {
                progress.DOFillAmount(value/100, 0.3f).OnComplete(() =>
                {
                    if (transform == null)
                        return;
                   
                    progressText.gameObject.SetActive(!complete);
                    completeTr.gameObject.SetActive(complete);
                });
            }
            else
            {
                progress.fillAmount = value/100;
                progressText.gameObject.SetActive(!complete);
                completeTr.gameObject.SetActive(complete);
                
                callback?.Invoke();
            }
            lastBlood = value;
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            goBtn = transform.GetComponent<Button>();
            goBtn.onClick.AddListener(OnPuzzleBtnClicked);
        }

        private void OnPuzzleBtnClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidPuzzlePopup))));
        }
    }

    public class TreasureRaidPuzzleViewController : ViewController<TreasureRaidPuzzleView>
    {
        public Activity_TreasureRaid activityTreasureRaid;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.BindingView(inView, inExtraData, inExtraAsyncData);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidRefreshPuzzleView>(OnRefreshPuzzleView);
        }

        private void OnRefreshPuzzleView(EventTreasureRaidRefreshPuzzleView obj)
        {
            view.RefreshPuzzle(false);
        }

        public async void CheckHasRewardToCollect(Action beginCallBack, Action endCallback)
        {
            var curListInfo = activityTreasureRaid.GetLastPuzzleListInfo();
            for (int i = 0; i < curListInfo.PuzzleList.Count; i++)
            {
                var puzzle = curListInfo.PuzzleList[i];
                if (puzzle.Status == 1)
                {
                    beginCallBack?.Invoke();
                    activityTreasureRaid.SetActivityState(true);
                    var currentPageIndex = i + 1;
                    var collectView = await PopupStack.ShowPopup<TreasureRaidPuzzleCollectPopup>();
                    collectView.InitRewardContent(puzzle.Reward, currentPageIndex);
                    collectView.ShowRewardCollect( (listInfo) =>
                    {
                        activityTreasureRaid.SetLastPuzzleListInfo(listInfo);
                        endCallback?.Invoke();
                        view.RefreshPuzzle(false);
                        activityTreasureRaid.SetActivityState(false);
                    });
                }
            }
        }
    }
}