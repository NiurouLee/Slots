using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public enum AmazingHatRewardState
    {
        Standard,
        Disable,
    }
    
    public class AmazingHatRewardItem : View<AmazingHatRewardItemController>
    {
        [ComponentBinder("StandardType")]
        public Transform standard;

        [ComponentBinder("StandardType/RewardIcon")]
        public Image standard_iconImage;
        
        [ComponentBinder("StandardType/CountText")]
        public TextMeshProUGUI standard_countText;
        
        [ComponentBinder("DisableType")]
        public Transform disable;
        
        [ComponentBinder("DisableType/RewardIcon")]
        public Image disable_iconImage;
        
        [ComponentBinder("DisableType/CountText")]
        public TextMeshProUGUI disable_countText;
        
        [ComponentBinder("MoveObj")]
        public Transform moveTr;

        [ComponentBinder("MoveObj/RewardIcon")]
        public Image moveObj_iconImage;
        
        [ComponentBinder("MoveObj/CountText")]
        public TextMeshProUGUI moveObj_countText;
        
        public void SetItemState(AmazingHatRewardState state)
        {
            if (moveTr.gameObject.activeSelf)
            {
                moveTr.gameObject.SetActive(false);
            }
            switch (state)
            {
                case AmazingHatRewardState.Standard:
                    standard.gameObject.SetActive(true);
                    disable.gameObject.SetActive(false);
                    break;
                case AmazingHatRewardState.Disable:
                    standard.gameObject.SetActive(false);
                    disable.gameObject.SetActive(true);
                    break;
            }
        }

        public void RefreshRewardUI(Item item)
        {
            Sprite rewardIcon = XItemUtility.GetItemSprite(item.Type, item);
            standard_iconImage.sprite = rewardIcon;
            disable_iconImage.sprite = rewardIcon;
            var items = XItemUtility.GetItems(viewController._controller.Rewards, item.Type);
            if (item.Type == Item.Types.Type.CardPackage)
            {
                var cardItems = XItemUtility.SeparateCardPackageToDifferentGroupBackDictionary(items);
                var countText = String.Empty;
                if (cardItems.ContainsKey(item.CardPackage.PackageConfig.TypeForShow.ToString()))
                {
                    countText = string.Format("+{0}",
                        cardItems[item.CardPackage.PackageConfig.TypeForShow.ToString()].Count);
                }
                standard_countText.text = countText;
                disable_countText.text = countText;
            }
            else
            {
                var reward = viewController._controller.Rewards[0];
                for (int i = 0; i < reward.Items.Count; i++)
                {
                    if (reward.Items[i].Type == item.Type)
                    {
                        standard_countText.text = XItemUtility.GetItemDefaultDescText(reward.Items[i]);
                        disable_countText.text = XItemUtility.GetItemDefaultDescText(reward.Items[i]);
                        break;
                    }
                }
            }
            SetItemState(AmazingHatRewardState.Standard);
        }

        public void RefreshState(bool hasReward)
        {
            standard.gameObject.SetActive(hasReward);
            disable.gameObject.SetActive(false);
        }
        public void RefreshMoveObjUI(Item item)
        {
            Sprite rewardIcon = XItemUtility.GetItemSprite(item.Type, item);
            moveObj_iconImage.sprite = rewardIcon;
            moveObj_countText.text = XItemUtility.GetItemDefaultDescText(item);
            moveTr.gameObject.SetActive(true);
            
        }
        
        public void RefreshReConnectItemUI(Item item)
        {
            moveTr.gameObject.SetActive(false);
            RefreshRewardUI(item);
        }
    }

    public class AmazingHatRewardItemController : ViewController<AmazingHatRewardItem>
    {
        public AmazingHatController _controller;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _controller = Client.Get<AmazingHatController>();
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        /// <param name="pos">起始位置，世界坐标</param>
        public async void StartMoving(Vector3 pos, Item item, bool hasReward)
        {
            view.moveTr.gameObject.SetActive(true);
            var rectTr = view.moveTr.GetComponent<RectTransform>();
            view.moveTr.position = pos;
            //以相对坐标移动到Vector3.zero
            rectTr.localScale = Vector3.zero;
            rectTr.DOScale(Vector3.one * 1.5f, 0.3f);
            rectTr.DOLocalMoveY( rectTr.localPosition.y + 40f, 0.3f);
            await WaitForSeconds(1f);
            //TODO 播放打开动画
            var parentView = view.GetParentView() as AmazingHatMainPopup;
            if (!hasReward)
            {
                parentView?.viewController.MoveRewardContainer();
            }
            rectTr.DOScale(Vector3.one, 0.5f);
            SoundController.PlaySfx("Amazing_Win_fly");
            var tweener = rectTr.DOLocalMove(Vector3.zero, 0.5f).OnComplete(() =>
            {
                view.moveTr.gameObject.SetActive(false);
                view.RefreshRewardUI(item);
            });
            AddTweener(tweener);
        }
    }

    public class AmazingHatCellView : View<AmazingHatCellViewController>
    {
        [ComponentBinder("SelectButton")]
        public Button selectBtn;
        
        [ComponentBinder("RewardGroup/AmazingInTheHatCell/StandardType/RewardIcon")]
        public Image rewardIcon;
        
        [ComponentBinder("RewardGroup/AmazingInTheHatCell/StandardType/CountText")]
        public TextMeshProUGUI countText;

        public uint index;

        private bool isRabbit = false;

        public bool IsRabbit
        {
            get => isRabbit;
            set => isRabbit = value;
        }

        public void SetIndex(uint index)
        {
            this.index = index;
        }

        public void SetRewardUI(Item item)
        {
            Sprite rewardIconSprite = XItemUtility.GetItemSprite(item.Type, item);
            rewardIcon.sprite = rewardIconSprite;
            countText.text = XItemUtility.GetItemDefaultDescText(item);
        }
    }

    public class AmazingHatCellViewController : ViewController<AmazingHatCellView>
    {
        private Animator _animator;
        private AmazingHatController _controller;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _controller = Client.Get<AmazingHatController>();
            _animator = view.transform.GetComponent<Animator>();
            view.selectBtn.onClick.AddListener(OnSelectBtnClicked);
        }

        private async void OnSelectBtnClicked()
        {
            SoundController.PlaySfx("Amazing_Conjure");
            var name = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (name == "HatCell_Jin_idle")
                XUtility.PlayAnimation(_animator, "HatCell_Jin_Show");
            else
                XUtility.PlayAnimation(_animator, "HatCell_Yin_Show");
            
            var parentView = view.GetParentView() as AmazingHatMainPopup;
            if (parentView != null && parentView.firstPlayGuideGo.gameObject.activeSelf)
            {
                parentView.firstPlayGuideGo.gameObject.SetActive(false);
            }
            parentView?.DisableAllBtn();
            SHatGameSelect selectInfo = await _controller.HatGameSelect(view.index);
            if (selectInfo == null)
                return;

            if (selectInfo.RabbitAppear)
            {
                SoundController.PlaySfx("Amazing_Pick_Fail");
                view.IsRabbit = true;
                if (name == "HatCell_Jin_idle")
                    XUtility.PlayAnimation(_animator, "HatCell_Jin_Show_select_tuzi");
                else
                    XUtility.PlayAnimation(_animator, "HatCell_Yin_Show_select_tuzi");

                await WaitForSeconds(1f);
                // 出现兔子弹复活窗口
                var collectRewardView = await PopupStack.ShowPopup<UIAmazingHatStageGetRewardBreakOffPopup>();
                collectRewardView.InitRewardContent();
                collectRewardView.SetReviveHatGameCallBack(() =>
                {
                    parentView?.viewController.ResetHandleDeadState();
                    parentView?.viewController.SetLeaveBtnState(true);
                });
            }
            else
            {
                SoundController.PlaySfx("Amazing_Pick_Win");
                //策划定每选一个帽子，只出现一个Item，这里只选择第一个
                parentView?.viewController.ShowSelectAni(view.selectBtn.transform.position, selectInfo.Reward.Items[0], selectInfo.HatGameInfo.HatGameStat == HatGameInfo.Types.HatGameStat.Cleared);
                parentView?.viewController.ShowOtherRewardAni(view, selectInfo.RewardsOthers);
            }
        }

        public void ResetViewState()
        {
            if (!view.transform.gameObject.activeSelf)
            {
                view.transform.gameObject.SetActive(true);
            }
            view.IsRabbit = false;
            if (_controller.HatColor == HatGameInfo.Types.HatColor.Gold)
                XUtility.PlayAnimation(_animator, "HatCell_Jin_idle");
            else
                XUtility.PlayAnimation(_animator, "HatCell_Yin_idle");
            view.selectBtn.interactable = true;
        }

        public void SetBtnInteractable(bool interactable)
        {
            view.selectBtn.interactable = interactable;
        }
        
        public void ShowReward(Reward reward)
        {
            if (!view.transform.gameObject.activeSelf)
            {
                return;
            }
            // 播放 animator
            view.SetRewardUI(reward.Items[0]);
            
            var name = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (name == "HatCell_Jin_idle")
                XUtility.PlayAnimation(_animator, "HatCell_Jin_Show_NoSelect");
            else if(name == "HatCell_Yin_idle")
                XUtility.PlayAnimation(_animator, "HatCell_Yin_Show_NoSelect");
        }

        public void ShowRabbit()
        {
            if (!view.transform.gameObject.activeSelf)
            {
                return;
            }
            // 播放 animator
            var name = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (name == "HatCell_Jin_idle")
                XUtility.PlayAnimation(_animator, "HatCell_Jin_Show_NoSelect_Rabbit");
            else if(name == "HatCell_Yin_idle")
                XUtility.PlayAnimation(_animator, "HatCell_Yin_Show_NoSelect_Rabbit");
        }

        public void HideCellView()
        {
            view.IsRabbit = true;
            view.transform.gameObject.SetActive(false);
        }
    }
    
    [AssetAddress("UIAmazingInTheHatMain")]
    public class AmazingHatMainPopup : Popup<AmazingHatMainPopupController>
    {
        /// <summary>
        /// 等级父节点
        /// </summary>
        [ComponentBinder("Root/BGGroup/MainBGGroup/Scale/LeftBG/LeftGroup/LevelGroup/Content")]
        public Transform content;
        [ComponentBinder("Root/BGGroup/MainBGGroup/Scale/LeftBG/LeftGroup/LevelGroup")]
        public Animator levelAni;

        [ComponentBinder("Root")]
        public Transform root;
        
        private List<GameObject> levelCompleteGoList;
        private List<GameObject> frameGoList;

        /// <summary>
        /// 帽子父节点
        /// </summary>
        [ComponentBinder("Root/BGGroup/MainPlayGroup")]
        public Transform hatContainer;

        private List<AmazingHatCellView> hatCellViewList;
        
        /// <summary>
        /// 生成的预制体都放在此parent内
        /// </summary>
        [ComponentBinder("Root/RewardGroup")]
        public Transform rewardContainer;

        /// <summary>
        /// 奖励元素的预制体
        /// </summary>
        [ComponentBinder("Root/RewardGroup/AmazingInTheHatCell")]
        public Transform hatRewardCell;

        [ComponentBinder("Root/BottomGroup/PlayButton")]
        public Button playBtn;

        [ComponentBinder("Root/BottomGroup/TakeAndLeaveButton")]
        public Button leaveBtn;
        
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        public Button collectBtn;
        
        [ComponentBinder("Root/BGGroup/MainBGGroup/Scale/TopBG/CloseButton")]
        public Button closeBtn;
        
        //LuckyStageNoticeBubble
        [ComponentBinder("Root/TipsGroup/LuckyStageNoticeBubble")]
        public Transform noticeBubble;
        
        //RibbitAppearWarningBubble
        [ComponentBinder("Root/TipsGroup/RibbitAppearWarningBubble")]
        public Transform warningBubble;
        
        /// <summary>
        /// 第一次进入所展示的UI
        /// </summary>
        [ComponentBinder("Root/UIAmazingInTheHatMainInformation")]
        public Transform firstShowUIGo;
        
        /// <summary>
        /// 第一次选择所展示的引导
        /// </summary>
        [ComponentBinder("Root/UIAmazingInTheHatMainGuideGroup")]
        public Transform firstPlayGuideGo;

        public AmazingHatMainPopup(string address) : base(address)
        {
            
        }

        public override bool NeedForceLandscapeScreen()
        {
            return true;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            hatCellViewList = new List<AmazingHatCellView>();
            for (int i = 0; i < hatContainer.childCount; i++)
            {
                var hatTr = hatContainer.GetChild(i).GetChild(0);
                var hatCellView = AddChild<AmazingHatCellView>(hatTr);
                hatCellView.SetIndex((uint)i);
                hatCellViewList.Add(hatCellView);
            }

            levelCompleteGoList = new List<GameObject>();
            frameGoList = new List<GameObject>();
            for (int i = (content.childCount - 1); i >= 0 ; i--)
            {
                var completeGo = content.GetChild(i).Find("CompleteIcon").gameObject;
                var frameGo = content.GetChild(i).Find("Eff_waikuang").gameObject;
                levelCompleteGoList.Add(completeGo);
                frameGoList.Add(frameGo);
            }
        }

        private void RefreshTipUI(bool hideRabbit)
        {
            var controller = viewController._controller;
            var level = controller.Level;
            for (int i = 0; i < levelCompleteGoList.Count; i++)
            {
                levelCompleteGoList[i].SetActive((i + 1) < level);
                frameGoList[i].SetActive((i + 1) == level);
            }
            if (hideRabbit)
            {
                warningBubble.gameObject.SetActive(false);
            }
            else if (warningBubble.gameObject.activeSelf != controller.HasRabitTip)
            {
                warningBubble.gameObject.SetActive(controller.HasRabitTip);
            }
            if (noticeBubble.gameObject.activeSelf != controller.HasLuckyCardTip)
            {
                noticeBubble.gameObject.SetActive(controller.HasLuckyCardTip);
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            AdaptScaleTransform(root, new Vector2(1350,768));
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

        public void ShowLastCompleteGo()
        {
            levelCompleteGoList[levelCompleteGoList.Count - 1].SetActive(true);
            frameGoList[levelCompleteGoList.Count - 1].SetActive(false);
        }

        public void RefreshHatListUI()
        {
            RefreshTipUI(false);
            foreach (var hat in hatCellViewList)
            {
                hat.viewController.ResetViewState();
            }
        }

        public void DisableAllBtn()
        {
            foreach (var hat in hatCellViewList)
            {
                hat.viewController.SetBtnInteractable(false);
            }
            leaveBtn.interactable = false;
        }
        
        public void EnableAllBtn()
        {
            foreach (var hat in hatCellViewList)
            {
                hat.viewController.SetBtnInteractable(true);
            }
            leaveBtn.interactable = true;
        }
        
        public void ShowOtherHatReward(AmazingHatCellView current, RepeatedField<Reward> otherRewards)
        {
            int index = 0;
            foreach (var hat in hatCellViewList)
            {
                if (current != hat)
                {
                    if (index < otherRewards.Count && !hat.IsRabbit)
                    {
                        hat.viewController.ShowReward(otherRewards[index]);
                        index++;
                    }
                    else
                    {
                        hat.viewController.ShowRabbit();
                    }
                }
            }
        }

        public void ResetHandleDeadState(int selectIndex)
        {
            RefreshTipUI(true);
            for (int i = 0; i < hatCellViewList.Count; i++)
            {
                if (i == selectIndex)
                {
                    hatCellViewList[i].viewController.HideCellView();
                }
                else
                {
                    hatCellViewList[i].viewController.ResetViewState();
                }
            }
        }

        public override void Close()
        {
            base.Close();
            SoundController.RecoverLastMusic();
        }
    }

    public class AmazingHatMainPopupController : ViewController<AmazingHatMainPopup>
    {
        private const string StorageStateName = "AmazingHatState";

        private Dictionary<Item.Types.Type, AmazingHatRewardItem> _rewardItems;
        private Dictionary<string, AmazingHatRewardItem> _cardItems;

        private const int MoveHorDis = 66;

        public AmazingHatController _controller;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _controller = Client.Get<AmazingHatController>();
            _rewardItems = new Dictionary<Item.Types.Type, AmazingHatRewardItem>();
            _cardItems = new Dictionary<string, AmazingHatRewardItem>();
            view.playBtn.onClick.AddListener(OnPlayBtnClicked);
            view.leaveBtn.onClick.AddListener(OnCollectBtnClicked);
            view.closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        private void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();
        }

        private void OnCloseBtnClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatStart, ("Operation:", "ClickClose"),("OperationId","2"));
            SoundController.PlayButtonClick();
            view.Close();
        }

        public override void OnViewEnabled()
        {
            SoundController.PlayBgMusic("AmazingInTheHat_BGM");
            // Client.Storage.DeleteItem(StorageStateName);
            view.hatRewardCell.gameObject.SetActive(false);
            //这里判断是否是第一次进入，本地记录
            var level = _controller.Level;
            if (level == 1)
            {
                ShowFirstInGameHowToPlayUI();
            }
            else
            {
                ReconnectUI();
            }
            XUtility.PlayAnimation(view.levelAni, string.Format("Level{0}", level));
        }

        
        /// <summary>
        /// 恢复出现兔子复活的UI
        /// </summary>
        public void ResetHandleDeadState()
        {
            if (_controller.SelectedHatIndex == null || _controller.SelectedHatIndex.Count <= 0)
            {
                view.RefreshHatListUI();
            }
            else
            {
                view.ResetHandleDeadState((int)_controller.SelectedHatIndex[0]);
            }
        }

        public void SetLeaveBtnState(bool interactable)
        {
            view.leaveBtn.interactable = interactable;
        }

        private async void OnCollectBtnClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatTake);
            SoundController.PlayButtonClick();
            // 弹出领取所有奖励页面
            var collectRewardView = await PopupStack.ShowPopup<AmazingHatTakeAndLeaveRewardPopup>();
            collectRewardView.InitRewardContent();
            

            var task = new TaskCompletionSource<bool>();

            collectRewardView.ShowRewardCollect(async () =>
            {
                view.Close();
                task.TrySetResult(true);
            });

            await task.Task;
        }

        /// <summary>
        /// TODO 最后一个select调用
        /// </summary>
        public async void OnFinalRewardCollect()
        {
            // view.RefreshHatListUI();
            // 弹出领取最后奖励页面
            var collectRewardView = await PopupStack.ShowPopup<AmazingHatFinalRewardPopup>();
            collectRewardView.InitRewardContent();

            var task = new TaskCompletionSource<bool>();

            collectRewardView.ShowRewardCollect(async () =>
            {
                view.Close();
                task.TrySetResult(true);
            });

            await task.Task;
        }

        private async void ReconnectUI()
        {
            view.firstShowUIGo.gameObject.SetActive(false);
            view.playBtn.gameObject.SetActive(false);
            view.collectBtn.gameObject.SetActive(false);
            view.closeBtn.gameObject.SetActive(false);
            view.leaveBtn.gameObject.SetActive(_controller.Level != 1 && _controller.Level != 15);

            //TODO 短线重连逻辑在这里写
            var rewards = _controller.Rewards;
            if (rewards.Count > 0)
            {
                foreach (var reward in rewards)
                {
                    if (reward.Items.Count > 0)
                    {
                        for (var i = 0; i < reward.Items.Count; i++)
                        {
                            var item = reward.Items[i];
                            if (item.Type == Item.Types.Type.CardPackage)
                            {
                                if (!_cardItems.ContainsKey(item.CardPackage.PackageConfig.TypeForShow.ToString()))
                                {
                                    var rewardTr = GameObject.Instantiate(view.hatRewardCell, view.rewardContainer);
                                    AmazingHatRewardItem rewardView = view.AddChild<AmazingHatRewardItem>(rewardTr.transform);
                                    rewardView.RefreshReConnectItemUI(item);
                                    _cardItems.Add(item.CardPackage.PackageConfig.TypeForShow.ToString(), rewardView);
                                }
                            }
                            else
                            {
                                var rewardTr = GameObject.Instantiate(view.hatRewardCell, view.rewardContainer);
                                AmazingHatRewardItem rewardView = view.AddChild<AmazingHatRewardItem>(rewardTr.transform);
                                rewardView.RefreshReConnectItemUI(item);
                                _rewardItems.Add(item.Type, rewardView);
                            }
                        }
                    }
                }

                view.rewardContainer.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(575 - MoveHorDis * (_rewardItems.Count + _cardItems.Count - 1), -176);
            }
            switch (_controller.HatGameStat)
            {
                case HatGameInfo.Types.HatGameStat.Dead:
                    // 出现兔子弹复活窗口
                    var collectRewardView = await PopupStack.ShowPopup<UIAmazingHatStageGetRewardBreakOffPopup>();
                    collectRewardView.InitRewardContent();
                    ResetHandleDeadState();
                    break;
                case HatGameInfo.Types.HatGameStat.Live:
                    ResetHandleDeadState();
                    break;
                case HatGameInfo.Types.HatGameStat.Cleared:
                    OnFinalRewardCollect();
                    break;
            }
            if (_controller.Level == 1)
            {
                await XUtility.PlayAnimationAsync(view.animator, "refresh");
            }
            view.hatContainer.gameObject.SetActive(true);
            int state = Client.Storage.GetItem(StorageStateName, 0);
            if (state == 0 && _controller.Level == 1)
            {
                view.DisableAllBtn();
                Client.Storage.SetItem(StorageStateName, 1);
                await XUtility.WaitSeconds(0.5f);
                view.firstPlayGuideGo.gameObject.SetActive(true);
                view.EnableAllBtn();
            }
        }
        private async void OnPlayBtnClicked()
        {
            SoundController.PlayButtonClick();
            ReconnectUI();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatStart, ("Operation:", "ClickPlay"),("OperationId","1"));
        }

        public void ShowFirstInGameHowToPlayUI()
        {
            view.hatContainer.gameObject.SetActive(false);
            view.firstShowUIGo.gameObject.SetActive(true);
            view.playBtn.gameObject.SetActive(true);
            view.collectBtn.gameObject.SetActive(false);
            view.leaveBtn.gameObject.SetActive(false);
        }

        public void ShowOtherRewardAni(AmazingHatCellView currentView, RepeatedField<Reward> otherRewards)
        {
            view.ShowOtherHatReward(currentView, otherRewards);
        }
        
        public async void ShowSelectAni(Vector3 pos, Item item, bool showFinalCollect)
        {
            var level = _controller.Level;
            if (view.leaveBtn.gameObject.activeSelf && level == 15)
            {
                view.leaveBtn.gameObject.SetActive(false);
            }

            view.leaveBtn.interactable = false;
            if (showFinalCollect)
            {
                view.ShowLastCompleteGo();
            }
            else
            {
                //根据level的移动，到6开始移动，每次移动一格子
                XUtility.PlayAnimation(view.levelAni, string.Format("Level{0}", level));
            }
            AmazingHatRewardItem rewardView;
            bool hasReward = false;
            if (item.Type == Item.Types.Type.CardPackage)
            {
                hasReward = _cardItems.ContainsKey(item.CardPackage.PackageConfig.TypeForShow.ToString());
                if (!hasReward)
                {
                    var rewardTr = GameObject.Instantiate(view.hatRewardCell, view.rewardContainer);
                    rewardView = view.AddChild<AmazingHatRewardItem>(rewardTr);
                    _cardItems.Add(item.CardPackage.PackageConfig.TypeForShow.ToString(), rewardView);
                }
                else
                {
                    rewardView = _cardItems[item.CardPackage.PackageConfig.TypeForShow.ToString()];
                }
            }
            else
            {
                hasReward = _rewardItems.ContainsKey(item.Type);
                if (!hasReward)
                {
                    var rewardTr = GameObject.Instantiate(view.hatRewardCell, view.rewardContainer);
                    rewardView = view.AddChild<AmazingHatRewardItem>(rewardTr);
                    _rewardItems.Add(item.Type, rewardView);
                }
                else
                {
                    rewardView = _rewardItems[item.Type];
                }
            }
            rewardView.RefreshState(hasReward);
            await WaitNFrame(4);
            rewardView.RefreshMoveObjUI(item);
            rewardView.viewController.StartMoving(pos, item, hasReward);
            await WaitForSeconds(1f);
            SoundController.PlaySfx("Amazing_Curtain_Open");
            await XUtility.PlayAnimationAsync(view.animator, "refresh");
            if (showFinalCollect)
            {
                OnFinalRewardCollect();
            }
            else
            {
                view.RefreshHatListUI();
                view.leaveBtn.interactable = true;
            }
            if (!view.leaveBtn.gameObject.activeSelf && level != 15)
            {
                view.leaveBtn.gameObject.SetActive(true);
            }
        }

        public void MoveRewardContainer()
        {
            view.rewardContainer.DOLocalMoveX(575 - MoveHorDis * (_rewardItems.Count + _cardItems.Count - 1), 0.5f);
        }
    }
}