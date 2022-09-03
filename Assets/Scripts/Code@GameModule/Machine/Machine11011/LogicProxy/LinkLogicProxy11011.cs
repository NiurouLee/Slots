//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 16:23
//  Ver : 1.0.0
//  Description : LinkLogicProxy11011.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Item = DragonU3DSDK.Network.API.ILProtocol.RisingFortuneGameResultExtraInfo.Types.LinkData.Types.Item;

namespace GameModule
{
    public class LinkLogicProxy11011: LinkLogicProxy
    {
        private uint _respinLimitCount;
        private uint _respinCurCount;
        private List<Item> _listWrapAll;
        private List<Item> _listWrapLock;
        private List<Item> _listWrapSpin;
        public LinkLogicProxy11011(MachineContext context)
            : base(context)
        {
            _listWrapLock = new List<Item>();
            _listWrapAll = new List<Item>();
            _listWrapSpin = new List<Item>();
        }

        protected override async Task HandleLinkGameTrigger()
        {
            await Task.CompletedTask;
        }

        protected override async Task HandleLinkBeginCutSceneAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("Video");
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            machineContext.view.Get<FeatureCutView11011>().PlayCutScreen();
            await machineContext.WaitSeconds(0.5f);
            InitLinkUI();
            await machineContext.WaitSeconds(1.5f);
            machineContext.view.Get<FeatureCutView11011>().ToggleVisible(false);
        }
        

        protected override async Task  HandleLinkGame()
        {
            _listWrapLock.Clear();
            _listWrapAll.Clear();
            _listWrapSpin.Clear();
            if (IsFromMachineSetup())
            {
                InitLinkUI();
            }
            UpdateLinkWheelLockElements();
            await CheckAndPlayAddSpin();
            await CheckAndPlayWrapElement();
            await machineContext.WaitSeconds(1);
            ResetOnlyAddOrderLayer();
            UpdateRespinCount(reSpinState.ReSpinCount, reSpinState.ReSpinLimit);
        }

        private void InitLinkUI()
        {
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            _respinLimitCount = reSpinState.ReSpinLimit;
            controlPanel.UpdateControlPanelState(true,false);
            UpdateRespinCount(reSpinState.ReSpinCount, reSpinState.ReSpinLimit, true);
            machineContext.state.Get<WheelsActiveState11011>().UpdateLinkWheelState();
            ResetLinkWheels();
            UpdateEachAndNextWin();   
            UpdateLinkWheelLockElements();
        }

        public void UpdateRespinCount(uint reSpinCount, uint reSpinLimit, bool isEnterRoom=false)
        {
            _respinCurCount = Math.Max(reSpinLimit - reSpinCount,0) + 1;
            if (isEnterRoom)
            {
                _respinCurCount = Math.Max(reSpinLimit - reSpinCount,0);
            }
            controlPanel.UpdateFreeSpinCountText(Math.Max(reSpinLimit - reSpinCount,0),reSpinLimit, isEnterRoom);   
        }
        
        protected override bool CheckIsTriggerElement(ElementContainer container)
        {
            var elementId = container.sequenceElement.config.id;
            return Constant11011.IsLinkElement(elementId) || Constant11011.IsWrapElement(elementId);
        }
        
        private void UpdateLinkWheelLockElements()
        {
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var linkWheelState = machineContext.state.Get<WheelState>(1);
            var items = machineContext.state.Get<ExtraState11011>().GetLinkItems();
            var isInit = IsLinkTriggered() || IsFromMachineSetup();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                var elementContainer = GetRunningElementContainer(id);
                var lockRoll = wheel.GetRoll(id) as SoloRoll;
                var element = elementContainer.GetElement() as Element11011;
                if (!isInit && element.sequenceElement.config.id == Constant11011.ElementOnlyAddSpin)
                {
                    _listWrapSpin.Add(item);
                }
                if (!linkWheelState.IsRollLocked(id) && item.SymbolId>0)
                {
                    linkWheelState.SetRollLockState(id, true);
                    UpdateRunningElement(item.SymbolId, id,0,true);
                    elementContainer = GetRunningElementContainer(id);
                    element = elementContainer.GetElement() as Element11011;
                    if (isInit)
                    {
                        element.UpdateElementContent((int)item.WinRate);   
                    }
                    elementContainer.ShiftSortOrder(true);
                    elementContainer.PlayElementAnimation("Idle");
                    if (!isInit && Constant11011.IsAddSpinElement(item.SymbolId) && item.SymbolId != Constant11011.ElementOnlyAddSpin)
                    {
                        elementContainer.PlayElementAnimation("IdleAdd1Spin");
                    }
                    if (isInit)
                    {
                        lockRoll.ShiftRollMaskAndSortOrder(2100);   
                    }
                    if (!isInit && element.TotalWin <= 0)
                    {
                        if (Constant11011.IsAddSpinElement(item.SymbolId))
                        {
                            _listWrapSpin.Add(item);
                        }
                        if (Constant11011.IsWrapLockElement(item.SymbolId))
                        {
                            _listWrapLock.Add(item);
                        }
                        if (Constant11011.IsWrapAllElement(item.SymbolId))
                        {
                            _listWrapAll.Add(item);
                        } 
                    }
                }
            }
        }

        private async Task CheckAndPlayAddSpin()
        {
            if (_listWrapSpin.Count > 0)
            {
                for (int i = 0; i < _listWrapSpin.Count; i++)
                {
                    var item = _listWrapSpin[i];
                    var elementContainer = GetRunningElementContainer((int) item.PositionId);
                    AudioUtil.Instance.PlayAudioFx("+1Spin_Blink_Trigger");
                    await elementContainer.PlayElementAnimationAsync("SpinAdd1");
                    controlPanel.UpdateFreeSpinCountText(_respinCurCount,_respinLimitCount+1, true); 
                    if (item.SymbolId != Constant11011.ElementOnlyAddSpin)
                    {
                        await elementContainer.PlayElementAnimationAsync("Dis");   
                    }
                    await machineContext.WaitSeconds(0.3f);
                    _respinLimitCount++;
                }
            }
            _respinLimitCount = reSpinState.ReSpinLimit;
            await Task.CompletedTask; 
        }

        private void ResetOnlyAddOrderLayer()
        {
            if (_listWrapSpin.Count > 0)
            {
                var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                for (int i = 0; i < _listWrapSpin.Count; i++)
                {
                    var item = _listWrapSpin[i];
                    var id = (int) item.PositionId;
                    if (item.SymbolId >0 )
                        continue;
                    if (item.SymbolId == 0)
                    {
                        var lockRoll = wheel.GetRoll(id) as SoloRoll;
                        lockRoll.ShiftRollMaskAndSortOrder(-2100);   
                    }
                }
            }
        }

        private  async Task  CheckAndPlayWrapElement()
        {
            var extraState = machineContext.state.Get<ExtraState11011>();
            var items = extraState.GetLinkItems();
            if (_listWrapLock.Count > 0)
            {
                for (int i = 0; i < _listWrapLock.Count; i++)
                {
                    ulong totalWinRate = 0;
                    var itemEnd = _listWrapLock[i];
                    int index = 1;
                    for (int j = 0; j < items.Count; j++)
                    {
                        var itemStart = items[j];
                        if (Constant11011.IsWrapElement(itemStart.SymbolId) || itemStart.WinRate <= 0)
                            continue;
                        var containerStart = GetRunningElementContainer((int) itemStart.PositionId);
                        totalWinRate = await FlyItem(index++,containerStart, itemStart, itemEnd, totalWinRate);
                        if (index>6)
                        {
                            index = 1;
                        }
                    }
                }
            }

            if (_listWrapAll.Count > 0)
            {
                var round = extraState.GetGreenMultiplier();
                for (int i = 0; i < _listWrapAll.Count; i++)
                {
                    ulong nextWinRate = 0;
                    for (int j = 0; j < items.Count; j++)
                    {
                        var item = items[j];
                        var container = GetRunningElementContainer((int) item.PositionId);
                        var element = container.GetElement() as Element11011;
                        if (element.WinRate <= 0)
                            continue;
                        nextWinRate += (ulong)element.WinRate;
                    }
                    machineContext.view.Get<FeatureView11011>().UpdateNextWin(nextWinRate * extraState.GetGreenMultiplier());
                    await machineContext.WaitSeconds(0.2f);
                    
                    ulong totalWinRate = 0;
                    for (int k = 0; k < round; k++)
                    {
                        int index = 1;
                        var itemEnd = _listWrapAll[i];
                        //先吸锁定的
                        for (int j = 0; j < items.Count; j++)
                        {
                            var itemStart = items[j];
                            if (!Constant11011.IsLinkElement(itemStart.SymbolId))
                                continue;
                            if (itemEnd.PositionId == itemStart.PositionId)
                                continue;
                            if (itemStart.WinRate <= 0)
                                continue;
                            var containerStart = GetRunningElementContainer((int) itemStart.PositionId);
                            var elementStart = containerStart.GetElement() as Element11011;
                            if (elementStart.TotalWin <= 0)
                                continue;
                            totalWinRate = await FlyItem(index++, containerStart, itemStart, itemEnd, totalWinRate);
                            if (index>6)
                            {
                                index = 1;
                            }
                        }
                        await machineContext.WaitSeconds(0.2f);

                        index = 1;
                        //再吸黄的
                        for (int j = 0; j < items.Count; j++)
                        {
                            var itemStart = items[j];
                            if (!Constant11011.IsWrapLockElement(itemStart.SymbolId))
                                continue;
                            if (itemEnd.PositionId == itemStart.PositionId)
                                continue;
                            if (itemStart.WinRate <= 0)
                                continue;
                            var containerStart = GetRunningElementContainer((int) itemStart.PositionId);
                            var elementStart = containerStart.GetElement() as Element11011;
                            if (elementStart.TotalWin <= 0)
                                continue;
                            totalWinRate = await FlyItem(index++, containerStart, itemStart, itemEnd, totalWinRate);
                            if (index>6)
                            {
                                index = 1;
                            }
                        }
                        await machineContext.WaitSeconds(0.2f);

                        index = 1;
                        //再吸绿的
                        for (int j = 0; j < items.Count; j++)
                        {
                            var itemStart = items[j];
                            if (!Constant11011.IsWrapAllElement(itemStart.SymbolId))
                                continue;
                            if (itemEnd.PositionId == itemStart.PositionId)
                                continue;
                            if (itemStart.WinRate <= 0)
                                continue;
                            var containerStart = GetRunningElementContainer((int) itemStart.PositionId);
                            var elementStart = containerStart.GetElement() as Element11011;
                            if (elementStart.TotalWin <= 0)
                                continue;
                            totalWinRate = await FlyItem(index++, containerStart, itemStart, itemEnd, totalWinRate);
                            if (index>6)
                            {
                                index = 1;
                            }
                        }
                    }
                    await machineContext.WaitSeconds(0.5f);
                }
            }
            UpdateEachAndNextWin();
            await Task.CompletedTask;
        }
        
        protected override float GetElementTriggerDuration()
        {
            return 3f;
        }

        private async Task<ulong> FlyItem(int index, ElementContainer containerStart, Item itemStart, Item itemEnd, ulong totalWinRate)
        {
            var flyName ="BonusFly";
            var elementStart = containerStart.GetElement() as Element11011;
            var startPos = elementStart.GetStartWorldPos();
            var isNormalLink = Constant11011.IsLinkElement(itemStart.SymbolId);
            var flyContainer = machineContext.assetProvider.InstantiateGameObject(flyName,true);
            flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
            flyContainer.transform.SetParent(machineContext.transform,false);
            containerStart.PlayElementAnimation(isNormalLink ? "ToGoldAndGreen" : "ToGreen");   
            if (flyContainer.GetComponent<Animator>())
            {
                XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(), "Fly");
            }
            totalWinRate += itemStart.WinRate;
            var containerEnd = GetRunningElementContainer((int) itemEnd.PositionId);
            var elementEnd = containerEnd.GetElement() as Element11011;
            var endPos = elementEnd.GetStartWorldPos();
            var flyTime = 0.35f;
            if (Constant11011.IsWrapLockElement(itemStart.SymbolId))
            {
                flyTime = 0.45f;
            }
            if (Constant11011.IsWrapAllElement(itemStart.SymbolId))
            {
                flyTime = 0.5f;
            }
            AudioUtil.Instance.PlayAudioFx("B01_Fly");
            await XUtility.FlyAsync(flyContainer.transform, startPos, endPos, 0, flyTime);
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var lockRoll = wheel.GetRoll((int)itemEnd.PositionId) as SoloRoll;
            lockRoll.SetRollMaskScale(1.2f);
            containerEnd.PlayElementAnimation("FromRed");
            elementEnd.UpdateElementContent((int)totalWinRate);
            AudioUtil.Instance.PlayAudioFx($"B02_Accept_{index}");
            machineContext.assetProvider.RecycleGameObject(flyName,flyContainer);
            await machineContext.WaitSeconds(flyTime);
            lockRoll.SetRollMaskScale(1f/1.2f);
            return totalWinRate;
        }

        private void UpdateEachAndNextWin()
        {
            var extraState = machineContext.state.Get<ExtraState11011>();
            machineContext.view.Get<FeatureView11011>().UpdateEachWin(extraState.GetEachWinRate());
            machineContext.view.Get<FeatureView11011>().UpdateNextWin(extraState.GetNextWinRate()*extraState.GetGreenMultiplier());
        }
        
        private void ResetLinkWheels()
        {
            var items = machineContext.state.Get<ExtraState11011>().GetLinkItems();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                if (!Constant11011.IsNormalElementId(GetRunningElementId(i)))
                {
                    UpdateRunningElement(Constant11010.NextNormalElementId(), i);
                }
            }
        }

        protected override async Task HandleLinkFinishCutSceneAnimation()
        {
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var linkWheelState = machineContext.state.Get<WheelState>(1);
            for (int i = 0; i < 15; i++)
            {
                if (linkWheelState.IsRollLocked(i))
                {
                    var lockRoll = wheel.GetRoll(i) as SoloRoll;
                    lockRoll.ShiftRollMaskAndSortOrder(-2100);
                }
                linkWheelState.SetRollLockState(i, false);
            }
            machineContext.state.Get<WheelsActiveState11011>().UpdateBaseWheelState();
            controlPanel.UpdateControlPanelState(false,false);
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            await base.HandleLinkFinishCutSceneAnimation();
        }

        protected override async Task HandleLinkReward()
        {

            var grandWin = machineContext.state.Get<ExtraState11011>().GetGrandJackpotWinRate();
            if (grandWin>0)
            {
                TaskCompletionSource<bool> taskGrandWin = new TaskCompletionSource<bool>();
                var grandWinChips = machineContext.state.Get<BetState>().GetPayWinChips(grandWin);
                var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(Constant11011.GetJackpotAddress(3));
                view.SetJackpotWinNum(grandWinChips);
                view.SetPopUpCloseAction(async () =>
                {
                    taskGrandWin.SetResult(true);
                });
                await taskGrandWin.Task;
                AudioUtil.Instance.PlayAudioFx("B01_Collect");
                AddWinChipsToControlPanel(grandWinChips, 1f);
                await machineContext.WaitSeconds(1.5f);
            }
            ulong winChips = 0;
            var items = machineContext.state.Get<ExtraState11011>().GetLinkItems();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int posId = (int) item.PositionId;
                var elementContainer = GetRunningElementContainer(posId);
                if (item.WinRate >0 && item.SymbolId > 0&& elementContainer!=null)
                {
                    winChips += machineContext.state.Get<BetState>().GetPayWinChips(item.WinRate);
                    elementContainer.PlayElementAnimation("Collect");
                }
            }
            AudioUtil.Instance.PlayAudioFx("B01_Collect");
            AudioUtil.Instance.PlayAudioFx("Link_Show");
            var totalWineffect = machineContext.assetProvider.InstantiateGameObject("TotaLWinEffetcs", true);
            totalWineffect.transform.SetParent(machineContext.view.Get<ControlPanel>().transform, false);
            totalWineffect.SetActive(true);
            AddWinChipsToControlPanel(winChips, 2f);
            await machineContext.WaitSeconds(3.3f);
            machineContext.assetProvider.RecycleGameObject("TotaLWinEffetcs",totalWineffect);
            await Task.CompletedTask;
        }
    }
}