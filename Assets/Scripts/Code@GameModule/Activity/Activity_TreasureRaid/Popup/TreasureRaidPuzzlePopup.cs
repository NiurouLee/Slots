using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidPuzzleTipView : View
    {
        [ComponentBinder("SeasonText/SeasonGroup")]
        private Transform seasonGroup;

        [ComponentBinder("SeasonText")]
        private Transform seasonText;

        [ComponentBinder("CompleteText")]
        private Transform completeText;

        [ComponentBinder("RewardGroup")]
        private Transform rewardGroup;

        public void RefreshUI(MonopolyPuzzle puzzle)
        {
            for (int i = rewardGroup.childCount - 1; i >= 0 ; i--)
            {
                if (i != rewardGroup.childCount - 1)
                {
                    GameObject.Destroy(rewardGroup.GetChild(i).gameObject);
                }
            }
            completeText.gameObject.SetActive(puzzle.Status != 0);
            seasonText.gameObject.SetActive(puzzle.Status == 0);
            rewardGroup.gameObject.SetActive(puzzle.Status == 0);
            if (seasonText.gameObject.activeSelf)
            {
                for (int i = 0; i < seasonGroup.childCount; i++)
                {
                    seasonGroup.GetChild(i).gameObject.SetActive(puzzle.Num == (i + 1));
                }
                XItemUtility.InitItemsUI(rewardGroup, puzzle.Reward.Items, rewardGroup.Find("PuzzleRewardCell"));
            }
        }
    }
    
    public class TreasureRaidJigsawView : View
    {
        [ComponentBinder("Root/Complete")]
        private Animator complete;
        [ComponentBinder("Root/Complete/Complete_mask/Complete_image")]
        private Image completeImage;
        
        [ComponentBinder("Root/LockGroup")]
        private Transform lockGroup;
        [ComponentBinder("Root/LockGroup/__Mask")]
        private Image lockComplete;

        [ComponentBinder("Root/GetGroup")]
        private Transform getGroup;
        [ComponentBinder("Root/BGRoup")]
        private Transform darkGroup;

        private List<Image> puzzleLightList;
        private List<Image> puzzleDarktList;

        private MonopolyPuzzle puzzleInfo;

        private TreasureRaidPuzzlePopupController parentViewController;

        private int jigsawIndex;

        public void SetViewContent(int inJigsawIndex, MonopolyPuzzle inPuzzleInfo)
        {
            jigsawIndex = inJigsawIndex;
            puzzleInfo = inPuzzleInfo;

            var parentView = GetParentView() as TreasureRaidPuzzlePopup;
            parentViewController = parentView?.viewController;
            puzzleLightList = new List<Image>();
            puzzleDarktList = new List<Image>();
            for (int i = 0; i < getGroup.childCount; i++)
            {
                puzzleLightList.Add(getGroup.GetChild(i).GetComponent<Image>());
                puzzleDarktList.Add(darkGroup.GetChild(i).GetComponent<Image>());
            }
            complete.gameObject.SetActive(true);
            InitUI();
            
            RefreshUI();
        }

        /// <summary>
        /// 初始化图片
        /// </summary>
        private void InitUI()
        {
            var atlas = parentViewController.GetAtlas();
            for (int i = 1; i <= 9; i++)
            {
                var sprite = atlas.GetSprite(GetJigsawNameLight(i));
                // XDebug.LogOnExceptionHandler(sprite.name);
                puzzleLightList[i - 1].sprite = sprite;

                var darkSprite = atlas.GetSprite(GetJigsawNameDark(i));
                // XDebug.LogOnExceptionHandler(darkSprite.name);
                puzzleDarktList[i - 1].sprite = darkSprite;
            }

            completeImage.sprite = atlas.GetSprite(GetJigsawCompleteName(jigsawIndex));
            lockComplete.sprite = atlas.GetSprite(GetJigsawCompleteLockName(jigsawIndex));
        }

        private string GetJigsawNameLight(int index)
        {
            return $"ui_treasureRaid_puzzleCell_image{jigsawIndex}0{index}_1";
        }

        private string GetJigsawNameDark(int index)
        {
            return $"ui_treasureRaid_puzzleCell_image{jigsawIndex}0{index}_2";
        }

        private string GetJigsawCompleteName(int index)
        {
            return $"ui_treasureRaid_puzzleCell_image{index}";
        }
        
        private string GetJigsawCompleteLockName(int index)
        {
            return $"ui_treasureRaid_puzzleCell_image{index}_Mask";
        }
        public void RefreshUI()
        {
            // lockGroup刷新
            lockGroup.gameObject.SetActive(jigsawIndex > parentViewController.GetLastListInfoCurrentIndex());
            // complete.gameObject.SetActive(puzzleInfo.Status != 0);
            complete.Play(puzzleInfo.Status != 0 ? "idle" : "Normal");
            for (int i = 0; i < puzzleLightList.Count; i++)
            {
                // 以后写成控制动画
                var aniName = puzzleInfo.PositionsIsFill[i] ? "PuzzleCellCollectIdle" : "PuzzleCellEmptyIdle";
                puzzleLightList[i].GetComponent<Animator>().Play(aniName);
            }
        }

        /// <summary>
        /// 用于完成当前拼图后刷新拼图的状态
        /// </summary>
        /// <param name="inPuzzleInfo"></param>
        public void UpdatePuzzleInfo(MonopolyPuzzle inPuzzleInfo)
        {
            puzzleInfo = inPuzzleInfo;
        }

        public async Task ShowCollectAni(MonopolyPuzzle inPuzzleInfo, Action callback)
        {
            puzzleInfo = inPuzzleInfo;
            for (int i = 0; i < puzzleLightList.Count; i++)
            {
                // 以后写成控制动画
                var ani = puzzleLightList[i].GetComponent<Animator>();
                if (puzzleInfo.PositionsIsFill[i] && !ani.GetCurrentAnimatorStateInfo(0).IsName("PuzzleCellCollectIdle"))
                {
                    puzzleLightList[i].transform.SetAsLastSibling();
                    await XUtility.PlayAnimationAsync(ani, "PuzzleCellCollect");
                }
            }
            callback?.Invoke();
            lockGroup.gameObject.SetActive(!puzzleInfo.IsCurrent);
            if (puzzleInfo.Status != 0)
            {
                await XUtility.PlayAnimationAsync(complete, "show");
            }
        }

        public Canvas AddCanvasToGuide()
        {
            for (int i = 0; i < puzzleLightList.Count; i++)
            {
                // 以后写成控制动画
                var ani = puzzleLightList[i].GetComponent<Animator>();
                if (puzzleInfo.PositionsIsFill[i] && !ani.GetCurrentAnimatorStateInfo(0).IsName("PuzzleCellCollect"))
                {
                    var canvas = ani.transform.gameObject.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 10;
                    canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                    return canvas;
                }
            }
            return null;
        }
    }

    [AssetAddress("UITreasureRaidPuzzleMainPanel")]
    public class TreasureRaidPuzzlePopup : Popup<TreasureRaidPuzzlePopupController>
    {
        [ComponentBinder("Root/MainGroup/MiddleGroup/TagGroup")]
        private Transform tagGroup;

        [ComponentBinder("Root/MainGroup/MiddleGroup")]
        private Transform adaptGroup;

        [ComponentBinder("Root/MainGroup/MiddleGroup/TipGroup")]
        public Transform tipGroup;

        [ComponentBinder("Root/MainGroup/MiddleGroup/PuzzleGroup/__Puzzle")]
        public Transform puzzleContainer;

        [ComponentBinder("Root/MainGroup/MiddleGroup/TipGroup/RewardGroup")]
        private Transform tipRewardGroup;

        [ComponentBinder("Root/MainGroup/MiddleGroup/ScheduleGroup/__Text")]
        private Text currentProgressText;

        [ComponentBinder("Root/MainGroup/MiddleGroup/ScheduleGroup/__FillImage")]
        private Image currentProgress;

        [ComponentBinder("Root/MainGroup/MiddleGroup/HelpButton")]
        private Button helpBtn;

        [ComponentBinder("Root/MainGroup/MiddleGroup/CloseButton")]
        private Button closeBtn;
        
        [ComponentBinder("Root/MainGroup/MiddleGroup/CutoverGroup/RightButton")]
        private Button rigthBtn;

        [ComponentBinder("Root/MainGroup/MiddleGroup/CutoverGroup/LeftButton")]
        private Button leftBtn;
        
        private List<Animator> tagList;
        public List<TreasureRaidJigsawView> puzzleList;

        private TreasureRaidPuzzleTipView _tipView;

        private Vector3 offset = new Vector3(200, 0, 0);

        public bool showGuide = false;
        public TreasureRaidPuzzlePopup(string address)
            : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdaptScaleTransform(adaptGroup, new Vector2(1400, 768));
            helpBtn.onClick.AddListener(OnHelpBtnClicked);
            closeBtn.onClick.AddListener(OnCloseClicked);
            rigthBtn.onClick.AddListener(OnRightBtnClicked);
            leftBtn.onClick.AddListener(OnLeftBtnClicked);
            tagList = new List<Animator>();
            for (int i = 1; i <= 6; i++)
            {
                tagList.Add(tagGroup.Find($"Tag{i}").GetComponent<Animator>());
            }
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

        public void SetBtnStatus(bool interactable)
        {
            rigthBtn.interactable = interactable;
            leftBtn.interactable = interactable;
            helpBtn.interactable = interactable;
            closeBtn.interactable = interactable;
        }
        
        private async void OnRightBtnClicked()
        {
            SetBtnStatus(false);
            var lastPage = viewController.currentPageIndex;
            viewController.currentPageIndex++;
            RefreshUI();

            // 播放放大缩小动画，移动拼图
            var lastPuzzle = puzzleList[lastPage - 1].transform;

            var curPuzzle = puzzleList[viewController.currentPageIndex - 1].transform;
            curPuzzle.localPosition = lastPuzzle.localPosition + offset;

            lastPuzzle.DOLocalMoveX(-200f, 0.3f);
            lastPuzzle.DOScale(Vector3.one * 0.1f, 0.3f).OnComplete(() =>
            {
                lastPuzzle.gameObject.SetActive(false);
            });

            await XUtility.WaitSeconds(0.1f);
            curPuzzle.localScale = Vector3.one * 0.1f;
            curPuzzle.gameObject.SetActive(true);
            puzzleList[viewController.currentPageIndex - 1].RefreshUI();
            curPuzzle.DOScale(Vector3.one, 0.3f);
            curPuzzle.DOLocalMoveX(0, 0.3f).OnComplete(() =>
            {
                SetBtnStatus(true);
                tagList[lastPage - 1].Play("noselect");
                tagList[viewController.currentPageIndex - 1].Play("select");
            });
        }

        private async void OnLeftBtnClicked()
        {
            SetBtnStatus(false);
            var lastPage = viewController.currentPageIndex;
            viewController.currentPageIndex--;
            RefreshUI();

            // 播放放大缩小动画，移动拼图
            var lastPuzzle = puzzleList[lastPage - 1].transform;

            var curPuzzle = puzzleList[viewController.currentPageIndex - 1].transform;
            curPuzzle.localPosition = lastPuzzle.localPosition - offset;

            lastPuzzle.DOLocalMoveX(200f, 0.3f);
            lastPuzzle.DOScale(Vector3.one * 0.1f, 0.3f).OnComplete(() =>
            {
                lastPuzzle.gameObject.SetActive(false);
            });

            await XUtility.WaitSeconds(0.1f);
            curPuzzle.localScale = Vector3.one * 0.1f;
            curPuzzle.gameObject.SetActive(true);
            puzzleList[viewController.currentPageIndex - 1].RefreshUI();
            curPuzzle.DOScale(Vector3.one, 0.3f);
            curPuzzle.DOLocalMoveX(0, 0.3f).OnComplete(() =>
            {
                SetBtnStatus(true);
                tagList[lastPage - 1].Play("noselect");
                tagList[viewController.currentPageIndex - 1].Play("select");
            });
        }

        private void OnHelpBtnClicked()
        {
            SoundController.PlayButtonClick();
            PopupStack.ShowPopupNoWait<TreasureRaidPuzzleHelpPopup>();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            puzzleList = new List<TreasureRaidJigsawView>();
            for (int i = 0; i < puzzleContainer.childCount; i++)
            {
                var jigsawView = AddChild<TreasureRaidJigsawView>(puzzleContainer.GetChild(i));
                jigsawView.SetViewContent(i + 1, viewController.lastListInfo.PuzzleList[i]);
                jigsawView.transform.gameObject.SetActive(viewController.currentPageIndex == i + 1);
                puzzleList.Add(jigsawView);
            }
            _tipView = AddChild<TreasureRaidPuzzleTipView>(tipGroup);
            RefreshUI();
            for (int i = 1; i <= 6; i++)
            {
                tagList[i - 1].Play(viewController.currentPageIndex == i ? "Tag_select_loop" : "noselect");
            }
            if (viewController.showAni)
            {
                ShowAni();
            }
        }

        private async void ShowAni()
        {
            SetBtnStatus(false);
            await XUtility.WaitSeconds(0.5f);

            void RefreshProgress()
            {
                var curProgress = 0;
                for (int i = 0; i < viewController.curListInfo.PuzzleList[viewController.currentPageIndex - 1].PositionsIsFill.Count; i++)
                {
                    if (viewController.curListInfo.PuzzleList[viewController.currentPageIndex - 1].PositionsIsFill[i])
                    {
                        curProgress++;
                    }
                }

                var max = viewController.curListInfo.PuzzleList[viewController.currentPageIndex - 1].PositionsIsFill.Count;
                currentProgressText.SetText($"{curProgress}/{max}");
                currentProgress.DOKill();
                currentProgress.DOFillAmount((float) curProgress / max, 0.3f);
            }

            var puzzleView = puzzleList[viewController.currentPageIndex - 1];
            await puzzleView.ShowCollectAni(viewController.curListInfo.PuzzleList[viewController.currentPageIndex - 1], RefreshProgress);

            var reward = CheckHasRewardToCollect();
            if (reward != null)
            {
                var collectView = await PopupStack.ShowPopup<TreasureRaidPuzzleCollectPopup>();
                collectView.InitRewardContent(reward, viewController.currentPageIndex);
                collectView.ShowRewardCollect( async (listInfo) =>
                {
                    viewController.lastListInfo = listInfo;
                    viewController.curListInfo = listInfo;
                    if (viewController.currentPageIndex < 6)
                    {
                        var nextPuzzleView = puzzleList[viewController.currentPageIndex];
                        nextPuzzleView.UpdatePuzzleInfo(
                            viewController.curListInfo.PuzzleList[viewController.currentPageIndex]);

                        await XUtility.WaitSeconds(0.5f);
                        // 刷新
                        OnRightBtnClicked();
                        await XUtility.WaitSeconds(0.6f);
                    }
                    SetBtnStatus(true);
                });
            }
            else
            {
                showGuide = viewController.CheckShowGuide();
                if (showGuide)
                {
                    // 开始新手引导
                    viewController.ShowGuideStep8();
                }
                else
                {
                    SetBtnStatus(true);
                }
                viewController.lastListInfo = viewController.curListInfo;
                var nextPuzzleView = puzzleList[viewController.currentPageIndex - 1];
                nextPuzzleView.UpdatePuzzleInfo(
                    viewController.curListInfo.PuzzleList[viewController.currentPageIndex - 1]);
            }
        }

        private Reward CheckHasRewardToCollect()
        {
            for (int i = 0; i < viewController.curListInfo.PuzzleList.Count; i++)
            {
                var puzzle = viewController.curListInfo.PuzzleList[i];
                if (puzzle.Status == 1)
                {
                    return puzzle.Reward;
                }
            }
            return null;
        }

        private void RefreshUI()
        {
            var curProgress = 0;
            for (int i = 0; i < viewController.lastListInfo.PuzzleList[viewController.currentPageIndex - 1].PositionsIsFill.Count; i++)
            {
                if (viewController.lastListInfo.PuzzleList[viewController.currentPageIndex - 1].PositionsIsFill[i])
                {
                    curProgress++;
                }
            }

            var max = viewController.lastListInfo.PuzzleList[viewController.currentPageIndex - 1].PositionsIsFill.Count;
            currentProgressText.SetText($"{curProgress}/{max}");
            currentProgress.DOKill();
            currentProgress.fillAmount = (float) curProgress / max;

            if (viewController.currentPageIndex == 1)
            {
                leftBtn.gameObject.SetActive(false);
                rigthBtn.gameObject.SetActive(true);
            }
            else if (viewController.currentPageIndex == 6)
            {
                leftBtn.gameObject.SetActive(true);
                rigthBtn.gameObject.SetActive(false);
            }
            else
            {
                leftBtn.gameObject.SetActive(true);
                rigthBtn.gameObject.SetActive(true);
            }
            _tipView.RefreshUI(viewController.lastListInfo.PuzzleList[viewController.currentPageIndex - 1]);
        }

        public override void Destroy()
        {
            if (showGuide)
            {
                var popup = PopupStack.GetPopup<TreasureRaidMainPopup>();
                popup.viewController.ShowGuideStep10();
            }
            base.Destroy();
        }
    }

    public class TreasureRaidPuzzlePopupController : ViewController<TreasureRaidPuzzlePopup>
    {
        public Activity_TreasureRaid activityTreasureRaid;
        //从1开始
        public int currentPageIndex = 1;

        private AssetReference atlasReference;
        private SpriteAtlas atlas;

        public SGetMonopolyPuzzleListInfo curListInfo;
        public SGetMonopolyPuzzleListInfo lastListInfo;

        private TreasureRaidGuideStepView guideStep8View;
        private TreasureRaidGuideStepView guideStep9View;

        public bool showAni = false;

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {"UITreasureRaidPuzzleCellAtlas"};
            await base.LoadExtraAsyncAssets();
           
            atlasReference = GetAssetReference("UITreasureRaidPuzzleCellAtlas");
            if (atlasReference != null)
            {
                atlas = atlasReference.GetAsset<SpriteAtlas>();
            }
        }

        public SpriteAtlas GetAtlas()
        {
            return atlas;
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
            SubscribeEvent<EventActivityExpire>(OnCloseGuideStepView);
        }

        private void OnCloseGuideStepView(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.TreasureRaid)
                return;
            if (guideStep8View != null && guideStep8View.transform != null)
            {
                guideStep8View.Destroy();
            }
            if (guideStep9View != null && guideStep9View.transform != null)
            {
                guideStep9View.Destroy();
            }
        }
        
        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.BindingView(inView, inExtraData, inExtraAsyncData);

            if (inExtraData != null)
            {
                if (inExtraData is PopupArgs args)
                {
                    if (args.extraArgs != null)
                    {
                        showAni = (bool) args.extraArgs;
                    }
                }
            }
            
            curListInfo = inExtraAsyncData as SGetMonopolyPuzzleListInfo;
            lastListInfo = activityTreasureRaid.GetLastPuzzleListInfo();
            if (lastListInfo == null)
            {
                lastListInfo = curListInfo;
            }

            if (lastListInfo != null)
            {
                currentPageIndex = GetLastListInfoCurrentIndex();
            }
        }

        public int GetLastListInfoCurrentIndex()
        {
            for (int i = 0; i < lastListInfo.PuzzleList.Count; i++)
            {
                if (lastListInfo.PuzzleList[i].IsCurrent)
                {
                     return i + 1;
                }
            }
            return 6;
        }

        public bool CheckShowGuide()
        {
            for (int i = 0; i < lastListInfo.PuzzleList.Count; i++)
            {
                var puzzle = lastListInfo.PuzzleList[i];
                for (int j = 0; j < puzzle.PositionsIsFill.Count; j++)
                {
                    if (puzzle.PositionsIsFill[j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.Close();
        }

        public override void OnViewDestroy()
        {
            activityTreasureRaid.SetLastPuzzleListInfo(curListInfo);
            if (showAni)
            {
                EventBus.Dispatch(new EventTreasureRaidRefreshPuzzleView());
            }
            base.OnViewDestroy();
        }
        
        public async void ShowGuideStep8()
        {
            guideStep8View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide8", view.transform);
            var guideRoot = guideStep8View.transform.Find("Root");
            view.AdaptScaleTransform(guideRoot, new Vector2(1400, 768));
            SoundController.PlaySfx("TreasureRaid_Bubble");

            var puzzleView = view.puzzleList[currentPageIndex - 1];
            var canvas = puzzleView.AddCanvasToGuide();
            var components = new List<Component>();
            components.Add(canvas);

            guideRoot.position = canvas.transform.position - new Vector3(15, 0, 0);
            var guideRootCanvas = guideRoot.gameObject.AddComponent<Canvas>();
            guideRootCanvas.overrideSorting = true;
            guideRootCanvas.sortingOrder = 11;
            guideRootCanvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            
            guideStep8View.SetGuideClickHandler(ShowGuideStep9, components);
        }

        private async void ShowGuideStep9()
        {
            guideStep9View = await View.CreateView<TreasureRaidGuideStepView>("UITreasureRaidGuide9", view.transform);
            var guideRoot = guideStep9View.transform.Find("Root");
            view.AdaptScaleTransform(guideRoot, new Vector2(1400, 768));

            var canvas = view.tipGroup.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
            
            var guideRootCanvas = guideRoot.gameObject.AddComponent<Canvas>();
            guideRootCanvas.overrideSorting = true;
            guideRootCanvas.sortingOrder = 11;
            guideRootCanvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");

            var components = new List<Component>();
            components.Add(canvas);
            SoundController.PlaySfx("TreasureRaid_Bubble");
            guideStep9View.SetGuideClickHandler(() =>
            {
                view.SetBtnStatus(true);
            }, components);
        }
    }
}
