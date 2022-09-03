using System.Data.SqlTypes;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LinkLogicProxy11312 : LinkLogicProxy
    {
        private ExtraState11312 ExtraState;
        private LinkWheelState11312 LinkWheelState;
        private FreeSpinState freeSpinState;
        private WheelLinkGameView11312 LinkGameView;
        private WheelSmallGame11312 SmallGame;
        private WheelsActiveState11312 wheelsActiveState;

        public LinkLogicProxy11312(MachineContext context) : base(context)
        {
            ExtraState = machineContext.state.Get<ExtraState11312>();
            LinkWheelState = machineContext.state.Get<LinkWheelState11312>();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
            LinkGameView = machineContext.view.Get<WheelLinkGameView11312>();
            reSpinState = machineContext.state.Get<ReSpinState11312>();
            wheelsActiveState = machineContext.state.Get<WheelsActiveState11312>();
        }
        /// <summary>
        /// 断线重连恢复逻辑
        /// </summary>
        protected override void RecoverLogicState()
        {
            Constant11312.IsReconnection = true;
            if (IsLinkTriggered())
            {
                wheelsActiveState.UpdateRunningWheel(new List<string>() { Constant11312.WheelName[0] }, true);
                if (reSpinState.triggerPanels != null && reSpinState.triggerPanels.Count > 0)
                {
                    var triggerPanel = reSpinState.triggerPanels[0];
                    var baseWheel = wheelsActiveState.GetRunningWheel()[0];
                    baseWheel.wheelState.UpdateWheelStateInfo(triggerPanel);
                    baseWheel.ForceUpdateElementOnWheel();    
                }
              
                return;
            }
            if (reSpinState.IsInRespin)
            {
                LinkGameView.SetAllRollLockedStatus();
                if (ExtraState.AddedToReels > 0)
                {
                    LinkGameView.SetLowerIsShow(true);
                    LinkGameView.RefreshLeftGoldNums(ExtraState.AddedToReels);
                }
            }
            machineContext.view.Get<Background11312>().PlayBgAnim(2);
            LinkGameView.lastLimitCount = reSpinState.ReSpinLimit;
            RefreshLinkLockedItems();
            RefreshValidFrames();
            RefreshUsedFrame();
            // 刷新次数
            LinkGameView.InitRefreshLinkCount(reSpinState.ReSpinCount, reSpinState.ReSpinLimit);
            wheelsActiveState.InitSmallWheel();
            if (IsCoinInFrames())
            {
                Constant11312.LinkIsSmall = true;
                wheelsActiveState.UpdateSmallRunningWheel(true);
            }
            else
            {
                Constant11312.LinkIsSmall = false;
            }
            // 恢复轮盘高度
            if (ExtraState.PanelHeight >= 4)
            {
                var resNum = (int)ExtraState.PanelHeight - 4;
                LinkGameView.LinkWheelLengthToChange(resNum);
                machineContext.view.Get<JackpotView11312>().RefreshJackpotScale(ExtraState.PanelHeight);
                LinkGameView.PlayAnim(resNum, true);
            }
            controlPanel.UpdateControlPanelState(false, false);
            // XDebug.Log($"<color=#15E9AC>==blinkRows:{LitJson.JsonMapper.ToJsonField( )}:{i}</color>");
        }
        //当次是否触发了Link
        protected override bool IsLinkTriggered()
        {
            if (reSpinState.ReSpinCount == 0 && Constant11312.IsReconnection)
                return true;

            return reSpinState.ReSpinTriggered;
        }
        protected override async void HandleCustomLogic()
        {
            //处理触发Link：开始弹板或者过场动画
            if (IsLinkTriggered())
            {
                StopBackgroundMusic();
                await HandleReSpinStartLogic();
                await HandleLinkGameTrigger();
                await HandleLinkBeginPopup();
                await HandleLinkBeginCutSceneAnimation();
            }

            //处理Link逻辑：锁图标或者其他
            if (IsInRespinProcess())
            {
                await HandleLinkGame();
            }

            //是否Link结束：处理结算过程
            if (IsLinkSpinFinished())
            {
                StopBackgroundMusic();
                await HandleLinkReward();
            }
            //Link结算完成，恢复Normal
            if (NeedSettle())
            {
                StopBackgroundMusic();
                await HandleLinkFinishPopup();
                await HandleLinkFinishCutSceneAnimation();
                await HandleLinkHighLevelEffect();
            }
            Proceed();
        }

        protected override async Task HandleLinkBeginPopup()
        {
            if (machineContext.assetProvider.GetAsset<GameObject>(GetLinkBeginAddress()) != null)
            {
                var task = GetWaitTask();
                var startLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinStartPopUp11312>(GetLinkBeginAddress());
                if (startLinkPopup != null)
                {
                    startLinkPopup.Init(ExtraState.AddedToReels > 0);
                    startLinkPopup.SetPopUpCloseAction(() =>
                    {
                        SetAndRemoveTask(task);
                    });
                    if (startLinkPopup.IsAutoClose())
                    {
                        await machineContext.WaitSeconds(GetLinkBeginPopupDuration());
                        startLinkPopup.Close();
                    }
                }
                else
                {
                    SetAndRemoveTask(task);
                }

                await task.Task;
            }
            await Task.CompletedTask;
        }


        protected override async Task HandleLinkReward()
        {
            if (reSpinState.ReSpinNeedSettle && !Constant11312.IsReconnection)
            {
                await AllLockedSymbolAddTotalWin();
            }

            await Task.CompletedTask;
        }

        protected override async Task HandleLinkGame()
        {
            // 当为触发时，并且为金币玩法时，先播一遍金币飞到目标地玩法
            if (IsLinkTriggered() && ExtraState.AddedToReels > 0)
            {
                await machineContext.WaitSeconds(1);
                await GoldFlyTargetPos();
            }

            if (!IsCoinInFrames())
                RefreshLinkLockedItems();
            else
                ShowSmallGameFx();

            // 需要恢复jackpot弹板
            if (ExtraState.JackpotInfo != null)
            {
                var totalWin = ExtraState.JackpotInfo.TotalWin;
                var jackpotId = ExtraState.JackpotInfo.JackpotId;
                await JackpotViewShow((int)jackpotId, totalWin, true);
            }

            await NewFramesShow();
            RefreshLinkLastCount();
            if (reSpinState.NextIsReSpin)
                Constant11312.IsReconnection = false;
            await base.HandleLinkGame();

        }
        protected override async Task HandleLinkBeginCutSceneAnimation()
        {
            var animName = "Base";
            if (freeSpinState.IsInFreeSpin)
                animName = "Free";
            await ShowLinkCut(animName, () =>
            {
                machineContext.view.Get<Background11312>().PlayBgAnim(2);
                wheelsActiveState.UpdateRunningWheelState(null);
                LinkGameView.SetAllRollLockedStatus();
                RefreshLinkLockedItems();
                wheelsActiveState.InitSmallWheel();
                if (ExtraState.AddedToReels > 0)
                {
                    RefreshLinkGoldItems();
                    LinkGameView.SetLowerIsShow(true);
                }
                machineContext.view.Get<JackpotView11312>().RefreshJackpotScale(ExtraState.PanelHeight);
                controlPanel.UpdateControlPanelState(false, false);
            });
            await base.HandleLinkBeginCutSceneAnimation();
        }
        protected override async Task HandleLinkFinishCutSceneAnimation()
        {
            await ShowLinkCut("Link", () =>
            {
                if (!freeSpinState.IsOver)
                    machineContext.view.Get<Background11312>().PlayBgAnim(1);
                else
                    machineContext.view.Get<Background11312>().PlayBgAnim(0);

                wheelsActiveState.InitSmallWheel(false);
                wheelsActiveState.UpdateRunningWheelState(null);
                RestoreTriggerWheelElement();
                ClearAllState();
                ClearAllExtraUINode();

            });

            await Task.CompletedTask;
        }
        protected override void RestoreTriggerWheelElement()
        {
            var triggerPanels = reSpinState.triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
            {
                var wheelState = machineContext.state.Get<WheelState>();
                if (freeSpinState.NextIsFreeSpin)
                    wheelState = machineContext.state.Get<WheelState>(1);
                wheelState.UpdateWheelStateInfo(triggerPanels[0]);
                var wheel = wheelsActiveState.GetRunningWheel()[0];
                wheel.ForceUpdateElementOnWheel();
            }
        }

        protected ElementContainer UpdateRunningElements(uint elementId, int rollIndex, CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.LinkItem value = null)
        {
            var wheel = machineContext.view.Get<Wheel>(2);
            var lockRoll = wheel.GetRoll(rollIndex);
            var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
            var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(elementId), machineContext);
            ElementContainer elementContainer = lockRoll.GetVisibleContainer(0);
            if (elementContainer != null)
            {
                elementContainer.UpdateElement(seqElement, true);
                if (true)
                {
                    elementContainer.ShiftSortOrder(true);
                    elementContainer.UpdateElementMaskInteraction(true);
                }
                var element = elementContainer.GetElement() as ElementCoin11312;
                if (value != null)
                {
                    // 如果为jackpot图标时
                    if (Constant11312.ListCoinElementIdsJackot.Contains(value.SymbolId))
                    {
                        // 如果为jackpot翻倍时
                        if (value.WinRate == 0 && value.JackpotCount > 1)
                        {
                            SetElementJPSprite(element);
                            var curJPText = element.transform.Find("AnimRoot/JPGroup/JPSprite2");
                            curJPText.gameObject.SetActive(true);
                            curJPText.transform.Find("MutiText").GetComponent<TextMesh>().text = "X" + value.JackpotCount;
                        }
                        // 如果为jackpot winrate不为0的时候，则为累加
                        else if (value.WinRate != 0 && value.JackpotCount <= 1)
                        {
                            SetElementJPSprite(element);
                            var curJPText = element.transform.Find("AnimRoot/JPGroup/JPSprite3");
                            curJPText.gameObject.SetActive(true);
                            var chips = machineContext.state.Get<BetState>().GetPayWinChips(value.WinRate);
                            var AddText = curJPText.transform.Find("AddText");
                            AddText.GetComponent<TextMesh>().text = "+" + chips.GetAbbreviationFormat(1);
                            AddText.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 8);
                        }
                        // 如果为jackpot winrate不为0的时候，并且jackpot翻倍数不为0时。
                        else if (value.WinRate != 0 && value.JackpotCount > 1)
                        {
                            SetElementJPSprite(element);
                            var curJPText = element.transform.Find("AnimRoot/JPGroup/JPSprite4");
                            curJPText.gameObject.SetActive(true);
                            var chips = machineContext.state.Get<BetState>().GetPayWinChips(value.WinRate);
                            var AddText = curJPText.transform.Find("AddText");
                            AddText.GetComponent<TextMesh>().text = "+" + chips.GetAbbreviationFormat(1);
                            AddText.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 8);
                            curJPText.transform.Find("MutiText").GetComponent<TextMesh>().text = "X" + value.JackpotCount;
                        }
                    }
                    else
                    {
                        var winRate = value.WinRate;
                        // element.sequenceElement.config.defaultInteraction = SpriteMaskInteraction.None;
                        var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                        if (chips > 0)
                            element.txtCoin.text = chips.GetAbbreviationFormat(1);
                    }
                }

            }
            return elementContainer;
        }
        private void SetElementJPSprite(ElementCoin11312 element)
        {
            // 需要进行单独显示
            for (int i = 1; i <= 4; i++)
            {
                element.transform.Find("AnimRoot/JPGroup/JPSprite" + i).gameObject.SetActive(false);
            }
        }
        private async Task ShowLinkCut(string anim, Action callback)
        {
            AudioUtil.Instance.PlayAudioFx("Feature_Transition");
            // 过场时间长 5.467f
            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
            machineContext.transform.Find("ZhenpingAnim").GetComponent<Animator>().Play("Open");
            var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("LinkCut");
            transitionAnimation.transform.SetParent(machineContext.transform);
            machineContext.WaitSeconds(4f, () =>
            {
                callback?.Invoke();
            });
            await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), anim, machineContext);
           // machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
            GameObject.Destroy(transitionAnimation);

        }

        /// <summary>
        /// 需要更新紫框之前播这个预告动画过场
        /// </summary>
        /// <returns></returns>
        private async Task ShowSeaWaveCut()
        {
            AudioUtil.Instance.PlayAudioFx("Transition");
            var SeaWave = machineContext.assetProvider.InstantiateGameObject("SeaWave_link");
            SeaWave.transform.SetParent(machineContext.transform.Find("ZhenpingAnim/checkerboard/Wheels"));
            SeaWave.transform.localScale = Vector3.one;
            SeaWave.transform.localPosition = new Vector3(0, 0.7f, 0);
            var maskIndex = ExtraState.PanelHeight - 3;
            SeaWave.transform.Find("root/mask" + maskIndex).gameObject.SetActive(true);
            SeaWave.gameObject.GetComponent<Animator>().Play("open");
            await machineContext.WaitSeconds(2);
            GameObject.Destroy(SeaWave.gameObject);
        }

        private void ShowSmallGameFx()
        {
            string animName = "white";
            if (Constant11312.ListCoinGoldElementIds.Contains(ExtraState.CoinInFrame[0].SymbolId))
                animName = "golden";
            //判断是否为金币转轮
            LinkGameView.ShowSmallGameFx(animName);
            AudioUtil.Instance.PlayAudioFx("FeatureWheel_Spin");
        }

        /// <summary>
        /// 每次respin判断金币是否在框内
        /// </summary>
        /// <returns></returns>
        private bool IsCoinInFrames()
        {
            if (ExtraState.CoinInFrame.Count != 0)
                Constant11312.LastRespinIsHasSmall = true;
            else
                Constant11312.LastRespinIsHasSmall = false;
            return ExtraState.CoinInFrame != null && ExtraState.CoinInFrame.Count != 0;
        }
        public void ClearAllState()
        {
            for (int i = 0; i < 35; i++)
            {
                LinkWheelState.SetRollLockState(i, false);
            }
            LinkGameView.RefreshLinkCountShow(0, 3);
            LinkGameView.ClearAllRespinRolls();
        }
        public void ClearAllExtraUINode()
        {
            LinkGameView.ClearAllFrames();
            LinkGameView.ResetReawrdValue();
            machineContext.view.Get<JackpotView11312>().SetJackpotBaseValue();
            LinkGameView.SetLowerIsShow(false);
        }
        /// <summary>
        /// 金币玩法时，则需要展示金币
        /// </summary>
        public void RefreshLinkGoldItems()
        {
            foreach (var item in ExtraState.StartCoins)
            {
                var rollIndex = item.Key;
                var symbolId = item.Value;
                UpdateRunningElements(symbolId, (int)rollIndex);
            }
        }
        /// <summary>
        /// 刷新已经锁定的数据
        /// </summary>
        public void RefreshLinkLockedItems()
        {
            foreach (var item in ExtraState.LinkItems)
            {
                var rollIndex = item.Key;
                var value = item.Value;
                UpdateRunningElements(value.SymbolId, (int)rollIndex, value);
                LinkWheelState.SetRollLockState((int)rollIndex, true);
            }
        }
        /// <summary>
        /// 刷新respin次数
        /// </summary>
        public void RefreshLinkLastCount()
        {
            if (IsCoinInFrames())
                return;

            if (reSpinState.ReSpinCount >= reSpinState.ReSpinLimit)
                reSpinState.ReSpinCount = reSpinState.ReSpinLimit;
            else
                reSpinState.ReSpinCount += 1;
            if(!(reSpinState as ReSpinState11312).JudgeIsEarlySettle())
                LinkGameView.RefreshLinkCountShow(reSpinState.ReSpinCount, reSpinState.ReSpinLimit);
        }
        /// <summary>
        /// 刷新剩下的紫框展示
        /// </summary>
        public void RefreshValidFrames()
        {
            LinkGameView.ClearAllFrames();
            var curWheel = machineContext.view.Get<Wheel>(2);
            if (ExtraState.ValidFrames.Count != 0)
            {
                for (int i = 0; i < ExtraState.ValidFrames.Count; i++)
                {
                    var index = ExtraState.ValidFrames[i];
                    if (Constant11312.IsReconnection && ExtraState.NewFrames.Count != 0 && ExtraState.NewFrames.Contains(index))
                    {
                        continue;
                    }
                    var posArr = Constant11312.ConversionDataToPos((int)index, 7);
                    var col = posArr[0];
                    var rol = posArr[1];
                    var curRollNode = curWheel.GetRoll((int)index) as SoloRoll;
                    var frameNode = machineContext.assetProvider.InstantiateGameObject("Frame");
                    frameNode.transform.SetParent(LinkGameView.FrameGroup.transform);
                    frameNode.transform.position = curRollNode.GetVisibleContainerPosition(0);
                    frameNode.transform.localScale = Vector3.one;
                    frameNode.name = "frame_" + index;
                    var sort = frameNode.AddComponent<SortingGroup>();
                    sort.sortingLayerName = "LocalUI";
                    LinkGameView.Frames.Add(frameNode);
                }
            }

        }
        // 刷新已经用过的框蓝框
        public void RefreshUsedFrame()
        {
            var curWheel = machineContext.view.Get<Wheel>(2);
            if (ExtraState.UsedFrames.Count != 0)
            {
                for (int i = 0; i < ExtraState.UsedFrames.Count; i++)
                {
                    var index = ExtraState.UsedFrames[i];
                    var posArr = Constant11312.ConversionDataToPos((int)index, 7);
                    var col = posArr[0];
                    var rol = posArr[1];
                    var curRollNode = curWheel.GetRoll((int)index) as SoloRoll;
                    var frameNode = machineContext.assetProvider.InstantiateGameObject("Static_Frame");
                    frameNode.transform.SetParent(LinkGameView.FrameGroup.transform);
                    frameNode.transform.position = curRollNode.GetVisibleContainerPosition(0);
                    frameNode.transform.localScale = Vector3.one;
                    frameNode.name = "UseFrame_" + index;
                    var sort = frameNode.AddComponent<SortingGroup>();
                    sort.sortingLayerName = "Wheel";
                    LinkGameView.Frames.Add(frameNode);

                }
            }
        }

        /// <summary>
        /// 新的框展示
        /// </summary>
        public async Task NewFramesShow()
        {
            if (ExtraState.NewFrames.Count != 0)
            {
                await ShowSeaWaveCut();
                RefreshValidFrames();
                RefreshUsedFrame();

                AudioUtil.Instance.PlayAudioFx("Add_Boder");
                var curWheel = machineContext.view.Get<Wheel>(2);
                for (int i = 0; i < ExtraState.NewFrames.Count; i++)
                {
                    var index = ExtraState.NewFrames[i];
                    XDebug.Log("新紫框位置:" + index);
                    var posArr = Constant11312.ConversionDataToPos((int)index, 7);
                    var col = posArr[0];
                    var rol = posArr[1];
                    var curRollNode = curWheel.GetRoll((int)index) as SoloRoll;
                    var frameNode = machineContext.assetProvider.InstantiateGameObject("Frame");
                    frameNode.transform.SetParent(LinkGameView.FrameGroup.transform);
                    frameNode.transform.position = curRollNode.GetVisibleContainerPosition(0);
                    frameNode.transform.localScale = Vector3.one;
                    frameNode.name = "frame_" + index;
                    var sort = frameNode.AddComponent<SortingGroup>();
                    sort.sortingLayerName = "LocalUI";
                    LinkGameView.Frames.Add(frameNode);
                    XUtility.PlayAnimation(frameNode.GetComponent<Animator>(), "Appear");
                }
                await machineContext.WaitSeconds(1);
            }
            await Task.CompletedTask;
        }

        List<int> linkItemID;
        List<CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.LinkItem> linkItemValue;
        int time = 0;
        List<GameObject> tempList;
        Action callBack;
        TaskCompletionSource<bool> waitTask;
        /// <summary>
        /// 所有锁定的图标的金额加到totalWin里
        /// </summary>
        /// <returns></returns>
        public async Task AllLockedSymbolAddTotalWin(bool isPayOut = false)
        {
            waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);

            linkItemID = new List<int>();
            linkItemValue = new List<CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.LinkItem>();
            foreach (var item in ExtraState.LinkItems)
            {
                if (isPayOut && Constant11312.ListCoinGoldElementIds.Contains(item.Value.SymbolId))
                    continue;
                linkItemID.Add((int)item.Key);
                linkItemValue.Add(item.Value);

            }
            tempList = new List<GameObject>();
            time = 0;
            LockedSymbolSettlement();
            callBack = new Action(() =>
            {
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });
            await waitTask.Task;
        }
        private  void LockedSymbolSettlement()
        {
            bool hasCalled = false;
            bool hasAdded = false;
           
            var flySequence = DOTween.Sequence(); ; 
            flySequence.target = machineContext.transform;
            //TODO:
            // TweenCanNotPause.AddTween(flySequence);
            //   machineContext.AddTweener(flySequence);

            var rollIndex = linkItemID[time];
            var value = linkItemValue[time];
            var controlPanel = machineContext.view.Get<ControlPanel11312>();
            var curWheel = machineContext.view.Get<Wheel>(2);
            var curRollNode = curWheel.GetRoll((int)rollIndex) as SoloRoll;
            var betState = machineContext.state.Get<BetState>();
            ulong winChipJackpot = 0;
            //处理jackpot
             if (value.JackpotId != 0)
             {
                 // 需要判断当前jackpot图标是否被加强过 --- 待完成
                 winChipJackpot = betState.GetPayWinChips(value.JackpotPay);
                 ulong winChipJackpotLast = 0;
                 // 翻倍后的值
                 if (value.JackpotCount > 0)
                     winChipJackpotLast = winChipJackpot / value.JackpotCount;
                 flySequence.AppendCallback(() =>
                 {
                     JackpotViewShows((int)value.JackpotId, winChipJackpotLast, false, value.JackpotCount);
                 });
             
             
                 if (value.JackpotCount > 1)
                 {
                     flySequence.AppendInterval(7.73f);
             
                 }
                 else
                 {
                     flySequence.AppendInterval(4f);
                 }
             }
             // 粒子飞
             flySequence.AppendCallback(() =>
             {
                 if (!hasCalled)
                 {
                     LiziFlyToControl(curRollNode);
                 }
                 hasCalled = true;
             });
             
             flySequence.AppendInterval(0.5f);
             flySequence.AppendCallback(() =>
             {
                 if (!hasAdded)
                 {
                     var winRate = value.WinRate;
                     var winChips = betState.GetPayWinChips(winRate);
                     if (winChipJackpot != 0)
                         winChips += winChipJackpot;
                     AddWinChipsToControlPanels(winChips);
                     controlPanel.ShowScoreBox();
                 }
                 hasAdded = true;
             });
             flySequence.AppendInterval(0.4f);
             flySequence.AppendCallback(() =>
             {
                 time++;
                 if (time >= linkItemID.Count)
                 {
                     foreach (var item in tempList)
                     {
                         GameObject.Destroy(item.gameObject);
                     }
                     callBack();
                 }
                 else
                 {
                     //继续循环
                     LockedSymbolSettlement();
                 }
             });
            
             flySequence.AppendCallback(() =>
             {
              
                 //machineContext.RemoveTween(flySequence);
             });
             
             flySequence.Play();

             
            
            //-------------------------重写原来的流程---------------------
        }
        private void AddWinChipsToControlPanels(ulong winChips, float configDuration = 0f, bool withAudio = true, bool withAutoAni = true, bool noWinOutAnimation = false)
        {
            if (winChips > 0)
            {
                machineContext.state.Get<WinState>().AddCurrentWin(winChips);

                var winLevel = machineContext.state.Get<BetState>().GetSmallWinLevel((long)winChips);


                string audioName = null;
                string stopAudioName = null;
                float effectDuration = 0.5f;

                // if (winLevel != WinLevel.NoWin)
                // {
                //     if (winLevel == WinLevel.SmallWin)
                //     {
                audioName = "Symbol_SmallWin_" + machineContext.assetProvider.AssetsId;
                // stopAudioName = "Symbol_SmallWinEnding_" + machineContext.assetProvider.AssetsId;
                effectDuration = 1.0f;
                //     }
                //     else
                //     {
                //         audioName = "Symbol_Win_" + machineContext.assetProvider.AssetsId;
                //         stopAudioName = "Symbol_WinEnding_" + machineContext.assetProvider.AssetsId;
                //         effectDuration = 2.0f;
                //     }
                // }

                if (!withAudio)
                {
                    audioName = String.Empty;
                    stopAudioName = String.Empty;
                }

                if (configDuration > 0)
                {
                    effectDuration = configDuration;
                }
                machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long)machineContext.state.Get<WinState>().currentWin,
                    effectDuration, false, audioName, stopAudioName);
            }
        }
        private async void JackpotViewShows(int jackpotId, ulong jackpotValue = 0, bool isInit = false, uint multiple = 0)
        {
            AudioUtil.Instance.PlayAudioFx("Jackpot_Open");
            // 根据jackpotId 打开对应的弹窗
            var jackpotPanel = PopUpManager.Instance.ShowPopUp<UIJackpotPanel11312>();
            jackpotPanel.InitBgAndTitle((int)jackpotId, isInit, (int)multiple);
            jackpotPanel.SetJackpotWinNum(jackpotValue);
            // 如果翻倍不为0 则弹出弹窗后进行播放翻倍动画
            if (multiple > 1)
            {
                await machineContext.WaitSeconds(0.3f);
                jackpotValue = jackpotValue * multiple;
                jackpotPanel.SetJackpotWinNum(jackpotValue);
            }
        }
        public void LiziFlyToControl(SoloRoll curRollNode)
        {
            AudioUtil.Instance.PlayAudioFx("Add_Prize");
            // 粒子飞
            var Antipatine = machineContext.assetProvider.InstantiateGameObject("Antipatine");
            Antipatine.transform.position = curRollNode.GetVisibleContainerPosition(0);
            Antipatine.transform.localScale = Vector3.one;
            Antipatine.transform.SetParent(LinkGameView.transform);
            Antipatine.gameObject.SetActive(false);
            var sort = Antipatine.AddComponent<SortingGroup>();
            sort.sortingLayerName = "UI";
            sort.sortingOrder = 10;
            tempList.Add(Antipatine);
            // var screenPos = Camera.main.WorldToScreenPoint(controlPanel.winTexts.transform.position);
            // var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            var endPos = controlPanel.GetWinTextRefWorldPosition(Vector3.zero);
            Antipatine.gameObject.SetActive(true);
            Antipatine.GetComponent<Animator>().Play("Open");
            curRollNode.GetVisibleContainer(0).PlayElementAnimation("Blink");
            // await XUtility.FlyAsync(Antipatine.transform, Antipatine.transform.position, endPos, 0, 0.5f);
            XUtility.Fly(Antipatine.transform, Antipatine.transform.position, endPos, 0, 0.5f, null, Ease.Linear, machineContext);

        }
        /// <summary>
        /// jackpot弹窗
        /// </summary>
        /// <param name="index"></param>
        /// <param name="jackpotValue"></param>
        /// <param name="multiple"></param> 翻倍
        /// <returns></returns>
        public async Task JackpotViewShow(int jackpotId, ulong jackpotValue = 0, bool isInit = false, uint multiple = 0)
        {
            // 根据jackpotId 打开对应的弹窗
            AudioUtil.Instance.PlayAudioFx("Jackpot_Open");
            var jackpotPanel = PopUpManager.Instance.ShowPopUp<UIJackpotPanel11312>();
            jackpotPanel.InitBgAndTitle((int)jackpotId, isInit, (int)multiple);
            jackpotPanel.SetJackpotWinNum(jackpotValue);
            // 如果翻倍不为0 则弹出弹窗后进行播放翻倍动画
            if (multiple > 1)
            {
                await machineContext.WaitSeconds(0.3f);
                jackpotValue = jackpotValue * multiple;
                jackpotPanel.SetJackpotWinNum(jackpotValue);
            }
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);
            jackpotPanel.SetPopUpCloseAction(() =>
            {
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });
            await waitTask.Task;
        }

        /// <summary>
        /// 金币飞左边目的地
        /// </summary>
        /// <returns></returns>
        public async Task GoldFlyTargetPos()
        {
            var time = 0;
            var curWheel = machineContext.view.Get<Wheel>(2);
            await machineContext.WaitSeconds(1);
            uint winRate = 0;
            foreach (var item in ExtraState.StartCoins)
            {
                var id = item.Key;
                var symbolId = item.Value;
                if (!Constant11312.ListCoinGoldElementIds.Contains(symbolId))
                    continue;
                var curRollNode = curWheel.GetRoll((int)id) as SoloRoll;
                var container = curRollNode.GetVisibleContainer(0);

                container.RemoveElement();

                var goldSymbolIndex = symbolId - 29;
                var winRates = Constant11312.GlodSymbolWinRate[(int)goldSymbolIndex];
                winRate += Constant11312.GlodSymbolWinRate[(int)goldSymbolIndex];
                var resWinRate = winRate;
                var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRates);

                var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(symbolId), machineContext);
                var goldNode = machineContext.assetProvider.InstantiateGameObject(seqElement.config.activeAssetName);
                var IntegralText = goldNode.transform.Find("AnimRoot/IntegralGroup/IntegralText");
                IntegralText.GetComponent<TextMesh>().text = "" + chips.GetAbbreviationFormat(1);
                IntegralText.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 8);
                goldNode.transform.SetParent(LinkGameView.transform);
                goldNode.transform.position = container.transform.position;
                goldNode.transform.localScale = Vector3.one;
                goldNode.AddComponent<SortingGroup>().sortingLayerName = "LocalFx";
                machineContext.WaitSeconds(0.75f * time, () =>
                {
                    AudioUtil.Instance.PlayAudioFx("YellowCoin_Remove");
                    XUtility.Fly(goldNode.transform, goldNode.transform.position, LinkGameView.RewardGroup.transform.position, 0, 0.5f, () =>
                    {
                        GameObject.Destroy(goldNode.gameObject);
                        // XDebug.Log("resWinRate："+resWinRate);
                        LinkGameView.RefreshLeftGoldNums(resWinRate);

                        var blast_dx = machineContext.assetProvider.InstantiateGameObject("blast_dx");
                        blast_dx.transform.SetParent(LinkGameView.RewardGroup.transform);
                        blast_dx.transform.localPosition = Vector3.zero;
                        blast_dx.transform.localScale = Vector3.one;
                        blast_dx.AddComponent<SortingGroup>().sortingLayerName = "LocalFx";

                        machineContext.WaitSeconds(1,() =>
                        {
                            GameObject.Destroy(blast_dx.gameObject);
                        });
                    });
                });
                time++;

            }
            await machineContext.WaitSeconds(0.75f * time + 1.5f + 1.5f);
        }
    }
}
