using System.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11312 : WheelStopSpecialEffectProxy
    {
        public ExtraState11312 extraState;
        public ReSpinState11312 respinState;
        private WheelsActiveState11312 wheelsActiveState;
        private WheelLinkGameView11312 linkGameView;
        private ElementContainer CurGoldContainer;
        private int lastCoinInFrameId = -1;
        public WheelStopSpecialEffectProxy11312(MachineContext machineContext) : base(machineContext)
        {
            extraState = machineContext.state.Get<ExtraState11312>();
            wheelsActiveState = machineContext.state.Get<WheelsActiveState11312>();
            respinState = machineContext.state.Get<ReSpinState11312>();
            linkGameView = machineContext.view.Get<WheelLinkGameView11312>();
        }
        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = machineContext.state.Get<ExtraState>();
            if (extraState != null && extraState.HasSpecialEffectWhenWheelStop())
            {
                return true;
            }
            if (respinState.NextIsReSpin || respinState.JudgeIsEarlySettle())
                return true;
            if (Constant11312.LastHasRandomS01)
                return true;
            return false;
        }
        protected override void HandleCustomLogic()
        {
            StopLogic(() =>
            {
                base.HandleCustomLogic();
            });
        }
        private async void StopLogic(Action callBack)
        {
            var linkLogic = machineContext.GetLogicStepProxy(LogicStepType.STEP_RE_SPIN) as LinkLogicProxy11312;
            var winState = machineContext.state.Get<WinState>();
            if (Constant11312.LastHasRandomS01 && winState.winLevel >= 3)
            {
                AudioUtil.Instance.PlayAudioFx("BigWin");
                machineContext.transform.Find("ZhenpingAnim").GetComponent<Animator>().Play("Gold");
                var ExplosiveGoldCoin = machineContext.assetProvider.InstantiateGameObject("ExplosiveGoldCoin");
                ExplosiveGoldCoin.transform.SetParent(machineContext.transform.Find("ZhenpingAnim/checkerboard/Wheels"));
                ExplosiveGoldCoin.transform.localScale = UnityEngine.Vector3.one;
                ExplosiveGoldCoin.transform.localPosition = UnityEngine.Vector3.zero;
                machineContext.WaitSeconds(4, () =>
                {
                    GameObject.Destroy(ExplosiveGoldCoin.gameObject);
                });
                await machineContext.WaitSeconds(3);
            }
            Constant11312.LastHasRandomS01 = false;
            if (respinState.NextIsReSpin && !Constant11312.LinkIsSmall)
                await machineContext.WaitSeconds(1);
            if (respinState.NextIsReSpin || respinState.JudgeIsEarlySettle())
            {
                // 判断本次是否为小轮盘结果
                if (Constant11312.LinkIsSmall)
                {
                    AudioUtil.Instance.PlayAudioFx("FeatureWheel_Stop");
                    RefreshUsedFrame(lastCoinInFrameId);
                    if (CurGoldContainer != null)
                    {
                        CurGoldContainer.GetContainzerGroup().sortingLayerName = "Element";
                        CurGoldContainer.PlayElementAnimation("Loop");
                        CurGoldContainer = null;
                    }
                    await linkGameView.ShowSmallGameFx("Select");
                    wheelsActiveState.UpdateSmallRunningWheel(false);
                    Constant11312.LinkIsSmall = false;
                    // 处理小轮盘结果
                    await DealLinkSmallLogic();
                }

                // 判断下次是否为小轮盘玩法
                if (NextSpinIsCoinInFrames())
                {
                    Constant11312.LinkIsSmall = true;
                    wheelsActiveState.UpdateSmallRunningWheel(true);
                    await PlayCoinInFrameAnim();
                }
            }
            callBack();
        }
        private void RefreshUsedFrame(int lastCoinInFrameId = -1)
        {
            var curWheel = machineContext.view.Get<Wheel>(2);
            // 让当前位置已有的紫框消失
            if (lastCoinInFrameId != -1)
            {
                for (int j = 0; j < linkGameView.Frames.Count; j++)
                {
                    var strArr = linkGameView.Frames[j].name.Split('_');
                    var name = strArr[0];
                    var frameIndex = int.Parse(strArr[1]);
                    if (name != "UseFrame" && lastCoinInFrameId == frameIndex)
                    {
                        linkGameView.Frames[j].gameObject.SetActive(false);
                    }
                }
                // 刷新当前位置用过的框
                if (extraState.UsedFrames.Count != 0)
                {
                    for (int i = 0; i < extraState.UsedFrames.Count; i++)
                    {
                        var index = extraState.UsedFrames[i];
                        if (index != lastCoinInFrameId) continue;
                        var posArr = Constant11312.ConversionDataToPos((int)index, 7);
                        var col = posArr[0];
                        var rol = posArr[1];
                        var curRollNode = curWheel.GetRoll((int)index) as SoloRoll;
                        var frameNode = machineContext.assetProvider.InstantiateGameObject("Static_Frame");
                        frameNode.transform.SetParent(linkGameView.FrameGroup.transform);
                        frameNode.transform.position = curRollNode.GetVisibleContainerPosition(0);
                        frameNode.transform.localScale = UnityEngine.Vector3.one;
                        frameNode.name = "UseFrame_" + index;
                        var sort = frameNode.AddComponent<SortingGroup>();
                        sort.sortingLayerName = "Wheel";
                        linkGameView.Frames.Add(frameNode);

                    }
                }
            }

        }
        // link  small-Result
        private bool NextSpinIsCoinInFrames()
        {
            if (extraState.CoinInFrame.Count != 0)
                lastCoinInFrameId = (int)extraState.CoinInFrame[0].Id;
            else
                lastCoinInFrameId = -1;
            return extraState.CoinInFrame != null && extraState.CoinInFrame.Count != 0;
        }
        /// <summary>
        /// 播放在框内的金币动效
        /// </summary>
        /// <returns></returns>
        private async Task PlayCoinInFrameAnim()
        {
            var item = extraState.CoinInFrame[0];
            var id = item.Id;
            var symbolId = item.SymbolId;
            var curWheel = machineContext.view.Get<Wheel>(2);
            CurGoldContainer = curWheel.GetRoll((int)id).GetVisibleContainer(0);
            CurGoldContainer.GetContainzerGroup().sortingLayerName = "LocalFx";
            linkGameView.RefreshLinkCountShow(respinState.ReSpinCount, respinState.ReSpinLimit);
            AudioUtil.Instance.PlayAudioFx("Coin_Border_Trigger");
            await CurGoldContainer.PlayElementAnimationAsync("Trigger_box");
        }
        /// <summary>
        /// 筛选是否link中有逻辑
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task DealLinkSmallLogic()
        {
            // 当前轮盘停止时的结果
            var panels = wheelsActiveState.GetPanel();
            // 当前转盘结果是否为小转盘
            if (!Constant11312.SmallSeqWheelName.Contains(panels.ReelsId) && panels != null)
            {
                return;
            }


            var columns = panels.Columns;
            var symbolId = columns[0].Symbols[1];

            // symbolId = 40;
            if (symbolId >= 32 && symbolId <= 35)
            {
                await RandomOneBlueSymbolWinRateMultiple((int)symbolId);
            }
            else if (symbolId >= 36 && symbolId <= 37)
            {
                await LinkWheelLengthToChange((int)symbolId);
            }
            else if (symbolId == 38)
            {
                await AllLockedBlueSymbolToTotalPay();
            }
            else if (symbolId == 39)
            {
                await RefreshLinkCount();
            }
            else if (symbolId >= 40 && symbolId <= 45)
            {
                await AllLockedBlueSymbolWinRateAdd((int)symbolId);
            }
            else if (symbolId >= 46 && symbolId <= 48)
            {
                await GoldSymbolWinRateAdd((int)symbolId);
            }
            else if (symbolId >= 49 && symbolId <= 51)
            {
                await GoldSymbolEnterJackpot((int)symbolId);
            }
        }
        /// <summary>
        /// 随机一个蓝币图标，赢钱数翻倍额外值
        /// </summary>
        /// <param name="symbolId"></param>
        public async Task RandomOneBlueSymbolWinRateMultiple(int symbolId)
        {
            // 获取随机Link的id位置 进行翻倍
            var index = symbolId - 32;
            var curWheel = machineContext.view.Get<Wheel>(2);
            var pickIndex = (int)extraState.RandomPickIndex;
            var mutilNum = Constant11312.SmallWheelResForRandom[index];
            // pickIndex = 0;
            var curRollNode = curWheel.GetRoll(pickIndex) as SoloRoll;
            string str = "X" + mutilNum;
            await AddFlyspinToTargetPos(str, curRollNode.GetVisibleContainerPosition(0));

            var blast_dx = machineContext.assetProvider.InstantiateGameObject("blast_dx");
            blast_dx.transform.SetParent(linkGameView.transform);
            blast_dx.transform.position = curRollNode.GetVisibleContainerPosition(0);
            blast_dx.AddComponent<SortingGroup>().sortingLayerName = "LocalFx";

            var container = curRollNode.GetVisibleContainer(0);
            var element = container.GetElement() as ElementCoin11312;
            var curSymbolId = curRollNode.GetVisibleContainer(0).sequenceElement.config.id;
            // 筛选出linkitem中某一个随机的数据
            DragonU3DSDK.Network.API.ILProtocol.CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.LinkItem value = null;
            foreach (var item in extraState.LinkItems)
            {
                var rollIndex = item.Key;
                if (rollIndex == pickIndex)
                    value = item.Value;
            }
            if (value != null)
            {
                if (Constant11312.ListCoinElementIdsJackot.Contains(curSymbolId))
                {
                    // 如果为jackpot翻倍大于1时
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
                    // 如果为jackpot winrate不为0的时候，并且jackpot翻倍数大于1时。
                    else if (value.WinRate != 1 && value.JackpotCount > 1)
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
                    // 需要知道翻多少倍数
                    var winRate = value.WinRate;
                    var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                    if (chips > 0)
                        element.txtCoin.text = chips.GetAbbreviationFormat(1);
                }
            }

            await machineContext.WaitSeconds(1f);
            GameObject.Destroy(blast_dx.gameObject);
        }
        private void SetElementJPSprite(ElementCoin11312 element)
        {
            // 需要进行单独显示
            for (int i = 1; i <= 4; i++)
            {
                element.transform.Find("AnimRoot/JPGroup/JPSprite" + i).gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// link轮盘高度增加
        /// </summary>
        public async Task LinkWheelLengthToChange(int symbolId)
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);
            bool hasPlay = false;
            var rollNum = symbolId - 35;


            // var sequence = DOTween.Sequence();
            // TweenCanNotPause.AddTween(sequence);
            // //TODO:
            // //       machineContext.AddTween(sequence);
            // sequence.AppendCallback(() =>
            // {
            //     if (!hasPlay)
            //     {
            //         //行数
            //         linkGameView.LinkWheelLengthToChange(rollNum);
            //         machineContext.view.Get<JackpotView11312>().RefreshJackpotScale(extraState.PanelHeight, true);
            //         linkGameView.PlayAnim(rollNum);
            //     }
            //     hasPlay = true;
            // });
            // sequence.AppendInterval(2);
            // sequence.AppendCallback(() =>
            // {
            //     TweenCanNotPause.RemoveTween(sequence);
            //     //TODO:
            //     //machineContext.RemoveTween(sequence);
            //     machineContext.RemoveTask(waitTask);
            //     waitTask.SetResult(true);
            // });
            // sequence.Play();
            // await waitTask.Task;


            ///-------重写----------
            if (!hasPlay)
            {
                linkGameView.LinkWheelLengthToChange(rollNum);
                machineContext.view.Get<JackpotView11312>().RefreshJackpotScale(extraState.PanelHeight, true);
                linkGameView.PlayAnim(rollNum);
            }
            hasPlay = true;
            await machineContext.WaitSeconds(2);

            //———————————————————————
        }


        int rollIndex;
        Action CallBack;
        /// <summary>
        /// link所有锁定的蓝币 进行结算一次 以行为单位
        /// </summary>
        public async Task AllLockedBlueSymbolToTotalPay()
        {

            // rollIndex = 0;
            // TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            // machineContext.AddWaitTask(waitTask,null);
            // OneRollToPay(()=>{
            //     machineContext.RemoveTask(waitTask);
            //     waitTask.SetResult(true);
            // });
            // await waitTask.Task;
            var linkLogic = machineContext.GetLogicStepProxy(LogicStepType.STEP_RE_SPIN) as LinkLogicProxy11312;
            await linkLogic.AllLockedSymbolAddTotalWin(true);
        }
        // 一行一行进行结算
        private async void OneRollToPay(Action callback)
        {
            if (callback != null)
                CallBack = callback;
            var curWheel = machineContext.view.Get<Wheel>(2);
            var controlPanel = machineContext.view.Get<ControlPanel11312>();
            var betState = machineContext.state.Get<BetState>();
            var linkLogic = machineContext.GetLogicStepProxy(LogicStepType.STEP_RE_SPIN) as LinkLogicProxy11312;


            // 先判断是否有jackpot，有则展示jackpot
            bool isShowJackpot = false;
            KeyValuePair<uint, CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.LinkItem> items;
            foreach (var item in extraState.LinkItems)
            {
                var idIndex = item.Key;
                var value = item.Value;
                var posArr = Constant11312.ConversionDataToPos((int)idIndex, 7);
                // 如果行相同，并且有jackpot的话
                if (rollIndex == posArr[1] && value.JackpotId != 0)
                {
                    isShowJackpot = true;
                    items = item;
                }
            }
            if (isShowJackpot)
            {
                var winChipJackpot = betState.GetPayWinChips(items.Value.JackpotPay);
                ulong winChipJackpotLast = 0;
                // 翻倍后的值
                if (items.Value.JackpotCount != 0)
                    winChipJackpotLast = winChipJackpot / items.Value.JackpotCount;
                // 展示完后，需要判断是否需要翻倍
                await linkLogic.JackpotViewShow((int)items.Value.JackpotId, winChipJackpotLast, false, items.Value.JackpotCount);
                AddWinChipsToControlPanel(winChipJackpot);
                controlPanel.ShowScoreBox();
            }

            // 后当前行进行全部播放特效结算winrate
            foreach (var item in extraState.LinkItems)
            {
                var idIndex = item.Key;
                var value = item.Value;
                var posArr = Constant11312.ConversionDataToPos((int)idIndex, 7);
                // 如果行相同，则进行结算
                if (rollIndex == posArr[1])
                {
                    var curRollNode = curWheel.GetRoll((int)idIndex) as SoloRoll;
                    curRollNode.GetVisibleContainer(0).PlayElementAnimation("Blink");
                    if (value.WinRate != 0)
                    {
                        var winChips = betState.GetPayWinChips(value.WinRate);
                        AddWinChipsToControlPanel(winChips);
                        // controlPanel.ShowScoreBox();
                    }
                }
            }
            await machineContext.WaitSeconds(0.667f);
            rollIndex++;
            if (rollIndex >= extraState.PanelHeight)
            {
                CallBack();
            }
            else
            {
                OneRollToPay(null);
            }
        }

        /// <summary>
        /// link 刷新当前respin次数
        /// </summary>
        public async Task RefreshLinkCount()
        {
            string str = "+2";
            await AddFlyspinToTargetPos(str, linkGameView.RespinEffects.transform.position);
            AudioUtil.Instance.PlayAudioFx("Add_Spin");
            linkGameView.RefreshLinkCountShow(respinState.ReSpinCount, respinState.ReSpinLimit);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 所有的蓝币图标，赢钱数累加额外值
        /// </summary>
        public async Task AllLockedBlueSymbolWinRateAdd(int symbolId)
        {
            var index = symbolId - 40;
            var curWheel = machineContext.view.Get<Wheel>(2);
            for (int rol = 0; rol < extraState.PanelHeight; rol++)
            {
                bool isShow = false;
                List<GameObject> addGolds = new List<GameObject>();
                foreach (var item in extraState.LinkItems)
                {
                    if (Constant11312.ListCoinGoldElementIds.Contains(item.Value.SymbolId))
                        continue;
                    var rollIndex = item.Key;
                    var posArr = Constant11312.ConversionDataToPos((int)rollIndex, 7);
                    if (rol == posArr[1])
                    {
                        isShow = true;
                        var value = item.Value;
                        var curRollNode = curWheel.GetRoll((int)rollIndex) as SoloRoll;
                        var container = curRollNode.GetVisibleContainer(0);
                        var element = container.GetElement() as ElementCoin11312;
                        // 如果为jackpot图标时
                        if (Constant11312.ListCoinElementIdsJackot.Contains(value.SymbolId))
                        {
                            // 如果为jackpot翻倍大于1时
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
                            // 如果为jackpot winrate不为0的时候，并且jackpot翻倍数大于1时。
                            else if (value.WinRate != 1 && value.JackpotCount > 1)
                            {
                                SetElementJPSprite(element);
                                var curJPText = element.transform.Find("AnimRoot/JPGroup/JPSprite4");
                                curJPText.gameObject.SetActive(true);
                                var chips = machineContext.state.Get<BetState>().GetPayWinChips(value.WinRate);
                                curJPText.transform.Find("AddText").GetComponent<TextMesh>().text = "+" + chips.GetAbbreviationFormat(1);
                                curJPText.transform.Find("MutiText").GetComponent<TextMesh>().text = "X" + value.JackpotCount;
                            }
                        }
                        else
                        {
                            var winRate = value.WinRate;
                            var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                            if (chips > 0)
                                element.txtCoin.text = chips.GetAbbreviationFormat(1);
                        }
                        // 添加特效
                        var Add_gold = machineContext.assetProvider.InstantiateGameObject("Add_gold");
                        Add_gold.transform.SetParent(linkGameView.transform);
                        var sort = Add_gold.AddComponent<SortingGroup>();
                        sort.sortingLayerName = "LocalFx";
                        addGolds.Add(Add_gold);
                        Add_gold.transform.position = curRollNode.GetVisibleContainerPosition(0);
                        Add_gold.GetComponent<Animator>().Play("Open");
                    }
                }
                if (isShow)
                {
                    AudioUtil.Instance.PlayAudioFx("Add_Prize_Blue");
                    await machineContext.WaitSeconds(0.667f);
                    foreach (var item in addGolds)
                    {
                        GameObject.Destroy(item.gameObject);
                    }
                }

            }

            await machineContext.WaitSeconds(0.667f * extraState.PanelHeight);
        }

        /// <summary>
        /// 金币图标，赢钱数累加额外值
        /// </summary>
        public async Task GoldSymbolWinRateAdd(int symbolId)
        {
            AudioUtil.Instance.PlayAudioFx("Prize_Trigger");
            // 获取随机Link的id位置 进行翻倍
            var index = symbolId - 46;
            var curWheel = machineContext.view.Get<Wheel>(2);
            var pickIndex = (int)extraState.RandomPickIndex;
            var smallWheel = machineContext.view.Get<Wheel>(3);
            var smallWheelElement = smallWheel.GetRoll(0).GetVisibleContainer(1).GetElement() as ElementCoin11312;

            var curRollNode = curWheel.GetRoll(pickIndex) as SoloRoll;

            var container = curRollNode.GetVisibleContainer(0);
            var element = container.GetElement() as ElementCoin11312;
            var curSymbolId = curRollNode.GetVisibleContainer(0).sequenceElement.config.id;

            var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
            var seqElement = new SequenceElement(elementConfigSet.GetElementConfig((uint)symbolId), machineContext);
            var fxSymbol = machineContext.assetProvider.InstantiateGameObject(seqElement.config.activeAssetName);
            fxSymbol.transform.SetParent(linkGameView.transform);
            fxSymbol.transform.localScale = UnityEngine.Vector3.one;
            fxSymbol.transform.position = linkGameView.transform.Find("SpinRemainingGroup/FeatureGroupR/FrameGroup").position;
            fxSymbol.AddComponent<SortingGroup>().sortingLayerName = "LocalFx";
            
            var integralText = fxSymbol.transform.Find("AnimRoot/Frspin03/IntegralGroup/IntegralText");
            integralText.GetComponent<TextMesh>().text = smallWheelElement.txtCoin.text;
            integralText.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 8);
            
            await XUtility.FlyAsync(fxSymbol.transform, fxSymbol.transform.position, curRollNode.GetVisibleContainerPosition(0), 0, 1);
            GameObject.Destroy(fxSymbol.gameObject);

            // 筛选出linkitem中某一个随机的数据
            DragonU3DSDK.Network.API.ILProtocol.CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.LinkItem value = null;
            foreach (var item in extraState.LinkItems)
            {
                var rollIndex = item.Key;
                if (rollIndex == pickIndex)
                    value = item.Value;
            }
            if (value != null)
            {
                // 需要知道翻多少倍数
                var winRate = value.WinRate;
                var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                    element.txtCoin.text = chips.GetAbbreviationFormat(1);
            }

        }

        /// <summary>
        /// 金币图标，结算jackpot
        /// </summary>
        public async Task GoldSymbolEnterJackpot(int symbolId)
        {
            var index = symbolId - 49;
            var jackpotId = (int)Constant11312.SmallWheelResForGoldJackpot[index];
            var totalWin = extraState.JackpotInfo.TotalWin;
            await (machineContext.GetLogicStepProxy(LogicStepType.STEP_RE_SPIN) as LinkLogicProxy11312).JackpotViewShow(jackpotId, totalWin, true);
            AddWinChipsToControlPanel(totalWin);
            machineContext.view.Get<ControlPanel11312>().ShowScoreBox();
            await machineContext.WaitSeconds(0.5f);
        }

        /// <summary>
        /// 添加增加轮盘高度特效预制体飞到目标点
        /// </summary>
        /// <returns></returns>
        private async Task AddFlyspinToTargetPos(string num, UnityEngine.Vector3 targetPos)
        {
            AudioUtil.Instance.PlayAudioFx("Multiplier_Trigger");
            var flySpin = machineContext.assetProvider.InstantiateGameObject("Flyspin");
            flySpin.transform.SetParent(linkGameView.transform);
            flySpin.transform.localScale = UnityEngine.Vector3.one;
            flySpin.transform.position = linkGameView.transform.Find("SpinRemainingGroup/FeatureGroupR/FrameGroup").position;
            var sort = flySpin.AddComponent<SortingGroup>();
            sort.sortingLayerName = "LocalFx";
            sort.sortingOrder = 10;
            flySpin.transform.Find("Root/FlyspinText").GetComponent<TextMesh>().text = "" + num;
            flySpin.transform.Find("Root/FlyspinText").GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 8);
            await XUtility.FlyAsync(flySpin.transform, flySpin.transform.position, targetPos, 0, 1);
            GameObject.Destroy(flySpin.gameObject);
        }
    }
}

