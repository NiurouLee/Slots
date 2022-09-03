//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-31 17:43
//  Ver : 1.0.0
//  Description : LogicProxy.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LinkLogicProxy11010: LinkLogicProxy
    {
        private LinkCounter11010 linkCounter;
        private ConnectionRegion11010 _region11010;
        private ElementContainer[] arrayLinkTrigger;
        private Vector3[] arrayLinkPosition;
        private LinkWheelState11010 linkWheelState;
        private Animator animatorLinkStartCut;
        private Transform transBorderContainer;
        private Transform transLinkElementBg;
        private Dictionary<int, Transform> dictLinkElementBg;
        private int curJackpotCount;
        public LinkLogicProxy11010(MachineContext context)
            : base(context)
        {
            strLinkTriggerSound = "J01_Trigger";
            _region11010 = new ConnectionRegion11010();
            arrayLinkTrigger = new ElementContainer[15];
            arrayLinkPosition = new Vector3[15];
            dictLinkElementBg = new Dictionary<int, Transform>();
            transLinkElementBg = machineContext.transform.Find("Wheels/WheelLinkGame/BorderContainer/LinkSpriteBg");
            transBorderContainer = machineContext.transform.Find("Wheels/WheelLinkGame/BorderContainer");
            animatorLinkStartCut = machineContext.transform.Find("Wheels/LinkStartCutPopup").GetComponent<Animator>();
        }
        
        public override void SetUp()
        {
            base.SetUp();
            linkWheelState = machineContext.state.Get<LinkWheelState11010>();
        }


        protected override async Task HandleLinkBeginPopup()
        {
            await base.HandleLinkBeginPopup();
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
        }
        protected override async Task HandleLinkBeginCutSceneAnimation()
        {
            //leftCol rightCol row
            var listPositions = new List<Tuple<int, int, int>>();
            bool needMove = GetStartCutSceneData(listPositions);
            animatorLinkStartCut.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorLinkStartCut, "Transform");
            await machineContext.WaitSeconds(1);
            DoElementsMoveAnimation(listPositions);   
            await machineContext.WaitSeconds(needMove ? 1.5f:1f);
            XUtility.PlayAnimation(animatorLinkStartCut, "Out");
            
            machineContext.state.Get<ReSpinState>().IsInRespin = true;
            for (int i = 0; i < arrayLinkTrigger.Length; i++)
            {
                if (arrayLinkTrigger[i] != null)
                {
                    var position = arrayLinkPosition[i];
                    arrayLinkTrigger[i].transform.position = new Vector3(position.x, position.y, position.z);
                }
                arrayLinkTrigger[i] = null;
                arrayLinkPosition[i] = Vector3.zero;
            }
            machineContext.state.Get<WheelsActiveState11010>().UpdateLinkWheelState();
            ResetLinkWheels();
            AudioUtil.Instance.PlayAudioFx("Bonus_Refresh");
            await Task.CompletedTask;
        }
        
        protected override async Task HandleLinkGameTrigger()
        {
            var wheels = GetLinkGameTriggerWheels();
            for (int i = 0; i < wheels.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    List<ElementContainer> countainers = new List<ElementContainer>();
                    for (int k = 0; k < 5; k++)
                    {
                        var elementContainer = wheels[i].GetRoll(k).GetVisibleContainer(j);
                        if (CheckIsTriggerElement(elementContainer))
                        {
                            countainers.Add(elementContainer);
                        }
                    }

                    if (countainers.Count>=3)
                    {
                        AudioUtil.Instance.PlayAudioFx(strLinkTriggerSound);
                        for (var m = 0; m < countainers.Count; m++)
                        {
                            countainers[m].PlayElementAnimation(GetElementTriggerAnimation());
                            countainers[m].ShiftSortOrder(true);
                        }

                        await XUtility.WaitSeconds(GetElementTriggerDuration(), machineContext);
                        for (var m = 0; m< countainers.Count; m++)
                        {
                            countainers[m].UpdateAnimationToStatic();
                        }
                    }
                }
            }
            machineContext.view.Get<ControlPanel>().ShowStopButton(true);
        }
        
        
        
        private bool  GetStartCutSceneData(in List<Tuple<int, int, int>> listPosition)
        {
            bool needMove = false;
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            for (int row = 0; row < 3; row++)
            {
                int leftCol = -1;
                int rightCol = -1;
                int linkCount = 0;
                for (int col = 0; col < wheel.rollCount; col++)
                {
                    var roll = wheel.GetRoll(col);
                    var container = roll.GetVisibleContainer(row);
                    if (Constant11010.IsLinkElement(container.sequenceElement.config.id))
                        linkCount++;
                }
                if (linkCount < Constant11010.MIN_LINK_TRIGGER_COUNT) 
                    continue;
                var listLinkCols = new List<int>();
                for (int col = 0; col < wheel.rollCount; col++)
                {
                    var roll = wheel.GetRoll(col);
                    var container = roll.GetVisibleContainer(row);
                    var index = col*roll.rowCount+row;
                    if (Constant11010.IsLinkElement(container.sequenceElement.config.id) && arrayLinkTrigger[index] == null)
                    {
                        if (col == 0)
                            leftCol = col;
                        if (col == wheel.rollCount-1)
                            rightCol = col;
                        container.PlayElementAnimation("Idle");
                        container.ShiftSortOrder(true);
                        var element = container.GetElement() as Element11010;
                        element?.ChangeLinkState();
                        arrayLinkTrigger[index] = container;
                        var position = container.transform.position;
                        arrayLinkPosition[index] = new Vector3(position.x, position.y, position.z);
                        listLinkCols.Add(col);
                    }
                }

                if (listLinkCols.Count >= 3)
                {
                    if (!needMove)
                    {
                        var count = listLinkCols.Count;
                        needMove = (listLinkCols[count-1] - listLinkCols[0])>count-1;
                    }
                    if (leftCol != -1 || rightCol != -1)
                    {
                        listPosition.Add(new Tuple<int, int, int>(leftCol,rightCol,row));   
                    }   
                }
            }

            return needMove;
        }

        private void DoElementsMoveAnimation(List<Tuple<int, int, int>> listPosition)
        {
            for (int i = 0; i < listPosition.Count; i++)
            {
                int col = -1;
                int step = 0;
                int posIndex = -1;
                var tuplePos = listPosition[i];
                if (tuplePos.Item1 != -1)
                {
                    step = 3;
                    posIndex = tuplePos.Item3;
                }
                else if (tuplePos.Item2 != -1)
                {
                    step = -3;
                    posIndex = tuplePos.Item3 + 12;
                }
                if (posIndex >= 0)
                {
                    int startCol = posIndex / 3;
                    for ( ; posIndex >= 0 && posIndex < arrayLinkTrigger.Length; posIndex+=step)
                    {
                        var item = arrayLinkTrigger[posIndex];
                        if (item != null)
                        {
                            item.transform.DOMove(GetElementPosition(startCol,tuplePos.Item3),0.5f);
                            startCol = step > 0 ? startCol+1 : startCol - 1;
                        }
                    }   
                }
            }
        }

        protected async Task DoJackpotsFly(RepeatedField<LockItLinkDiamondGameResultExtraInfo.Types.LinkJackpot> jackpots)
        {
            int index = 0;
            var items = machineContext.state.Get<ExtraState11010>().GetLinkItems();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                if (Constant11010.IsJackpotElement(item.SymbolId))
                {
                    var taskWait = new TaskCompletionSource<bool>();
                    var jackpotAudioName =
                        $"Bonus_{new[] {"Mini", "Minor", "Major", "Grand"}[item.SymbolId - 15]}_Sample";
                    var winChips = machineContext.state.Get<BetState>().GetPayWinChips(jackpots[index++].JackpotPay);
                    AudioUtil.Instance.PlayAudioFx(jackpotAudioName);
                    var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase11010>(Constant11010.GetJackpotAddress(item.SymbolId));
                    view.SetWaitTask(taskWait);
                    view.SetJackpotWinNum(winChips);
                    await taskWait.Task;
                    view.Close();
                    AudioUtil.Instance.StopAudioFx(jackpotAudioName);
                    AudioUtil.Instance.PlayAudioFx($"Bonus_{new []{"Mini","Minor","Major","Grand"}[item.SymbolId-15]}_Sample_Finish");
                    await machineContext.WaitSeconds(1.5f);
                    AddWinChipsToControlPanel(winChips,1f,true,false);
                    await machineContext.WaitSeconds(1.1f);
                    var elementContainer = GetRunningElementContainer(id);
                    elementContainer.PlayElementAnimation("Dis");
                    await machineContext.WaitSeconds(1f);
                }
            }
            await Task.CompletedTask;
        }
        protected override async Task HandleLinkGame()
        {
            if (IsFromMachineSetup())
            {
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
                machineContext.state.Get<WheelsActiveState11010>().UpdateLinkWheelState();
            }
            var needUpdate = UpdateLinkBlocks();
            UpdateLinkWheelLockElements(IsLinkTriggered() || IsFromMachineSetup());
            if (IsFromMachineSetup() || IsLinkTriggered())
            {
                var limit = (int)reSpinState.ReSpinLimit;
                linkCounter = machineContext.view.Get<LinkCounter11010>(limit-3);
                linkCounter.InitializeWith(limit);
                linkCounter.transform.gameObject.SetActive(true);
            }
            if (IsLinkTriggered())
            {
                await machineContext.WaitSeconds(0.8f);
                animatorLinkStartCut.gameObject.SetActive(false);
                var jackpots = machineContext.state.Get<ExtraState11010>().GetLinkJackpots();
                if (curJackpotCount<jackpots.Count)
                {
                    await DoJackpotsFly(jackpots);
                    curJackpotCount = jackpots.Count;
                }    
            }
            await DrawConnectionRegion(needUpdate);
            linkCounter.UpdateRespinCount((int)reSpinState.ReSpinCount);
            await machineContext.WaitSeconds(0.5f);
            await UpdateLinkElementChips(needUpdate);
            await machineContext.WaitSeconds(0.5f);
            List<ConnectionRegion11010.Block> newBlocks = _region11010.GetNewBlocks();
            for (int i = 0; i < newBlocks.Count; i++)
            {
                newBlocks[i].IsNewBlock = false;
            }
            linkCounter.UpdateRespinCount((int)reSpinState.ReSpinCount-1);
        }

        private async Task DrawConnectionRegion(bool needUpdate)
        {
            if (needUpdate)
            {
                PlayAllLockedSymbolLoop();
                List<ConnectionRegion11010.Block> newBlocks = _region11010.GetNewBlocks();
                for (int i = 0; i < newBlocks.Count; i++)   //连通域个数
                {
                    AudioUtil.Instance.PlayAudioFx("Bonus_ring");
                    var block = newBlocks[i];
                    var frameCount = block.ListFrames.Count;
                    for (int j = 0; j < frameCount; j++)    //连通域的外框和内框
                    {
                        var frame = block.ListFrames[j]; 
                        var listTupleBorders = frame.ListBorders;
                        var borderCount = listTupleBorders.Count;
                        var isOutLine = frame.Type == ConnectionRegion11010.Block.Frame.BorderType.Outer;
                        for (int k = 0; k < borderCount; k++)//连通域的框的边
                        {
                            var tuple = listTupleBorders[k];
                            var id = tuple.Item1;
                            var direction = tuple.Item2;
                            var start = false;
                            var end = false;
                            if (isOutLine)
                            {
                                start = k == 0;
                                end = k == borderCount - 1;
                            }
                            var goLine = GetDirectionLine(direction, start, end);
                            frame.AddBorderGameobject(goLine);
                            goLine.transform.SetParent(transBorderContainer.transform,false);
                            goLine.transform.position = GetDrawLineWorldPosition(id, direction);
                            if (goLine.name.Contains("VerticalEnd"))
                            {
                                AudioUtil.Instance.StopAudioFx("Bonus_ring");
                                AudioUtil.Instance.PlayAudioFx("Bonus_Lock");
                            }

                            if (k < borderCount-1 || (!isOutLine && k == borderCount - 1))
                            {
                                var index = k + 1 == borderCount ? 0 : k+1;
                                var nextDirection = listTupleBorders[index].Item2;
                                var goCorner = GetCornerDirectionLine(direction, nextDirection);
                                frame.AddBorderGameobject(goCorner);
                                goCorner.transform.SetParent(transBorderContainer.transform,false);
                                goCorner.transform.position = GetDrawLineCornerWorldPosition(id, direction, isOutLine);
                                goCorner.transform.localEulerAngles = new Vector3(0, 0, GetCornerRotation(direction, nextDirection, isOutLine));
                            }
                            if (!IsFromMachineSetup())
                            {
                                await machineContext.WaitSeconds(0.03f);   
                            }
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }
        
        private void PlayAllLockedSymbolLoop()
        {
            var items = machineContext.state.Get<ExtraState11010>().GetLinkItems();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                var elementContainer = GetRunningElementContainer(id);
                if (linkWheelState.IsRollLocked(id) && item.SymbolId>0 && _region11010.IsBlockId(id))
                {
                    var element = elementContainer.GetElement() as Element11010;
                    var transElementBg = GetElementLinkBg(id);
                    transElementBg.gameObject.SetActive(true);
                    transElementBg.position = element.TransLinkBg.position;
                    elementContainer.PlayElementAnimation("LoopInLink");
                }
            }
        }

        private async Task UpdateLinkElementChips(bool needUpdate)
        {
            if (!IsLinkTriggered() && !IsFromMachineSetup())
            {
                if (needUpdate)
                {
                    await DoUpdateLinkElementChips(0.3f);
                }

                if (IsTriggerGrand())
                {
                    var view = PopUpManager.Instance.ShowPopUp<MachinePopUp>("UIAllWin11010");
                    await machineContext.WaitSeconds(2);
                    PopUpManager.Instance.ClosePopUp(view);
                    await machineContext.WaitSeconds(1f);
                    var items = machineContext.state.Get<ExtraState11010>().GetLinkItems();
                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items[i];
                        item.WinRate *= 2;
                        int id = (int) item.PositionId;
                        var elementContainer = GetRunningElementContainer(id);
                        if (linkWheelState.IsRollLocked(id) && item.SymbolId>0)
                        {
                            var element = elementContainer.GetElement() as Element11010;
                           
                            elementContainer.PlayElementAnimation("Double");
                         
                            machineContext.WaitSeconds(0.66f, () =>
                            {
                                element?.UpdateElementContent((int) item.WinRate);
                            });
                        }
                    }
                    await machineContext.WaitSeconds(1.5f);
                    PlayAllLockedSymbolLoop();
                    await DoUpdateLinkElementChips(0.5f);
                }
            }
            await Task.CompletedTask;  
        }

        private async Task DoUpdateLinkElementChips(float delay)
        {
            List<ConnectionRegion11010.Block> newBlocks = _region11010.GetNewBlocks();
            if (newBlocks.Count>0)
            {
                await machineContext.WaitSeconds(0.5f);
            }
            for (int i = 0; i < newBlocks.Count; i++) //连通域个数
            {
                var block = newBlocks[i];
                var listIds = block.ListCoords;
                for (int j = 0; j < listIds.Count; j++)
                {
                    var id = listIds[j];
                    var posId = id.Item1 * 3 + id.Item2;
                    var item = machineContext.state.Get<ExtraState11010>().GetLinkItemByRollId(posId);
                    if (item.SymbolId>0)
                    {
                        var needAnim = Mathf.Abs(delay) > Mathf.Epsilon;
                        var container = GetRunningElementContainer(posId);
                        var element = container.GetElement() as Element11010;
                        needAnim = needAnim && element.NeedChangeAnim((int) item.WinRate);
                        element?.UpdateWinRate((int)item.WinRate, needAnim);
                        var transElementBg = GetElementLinkBg(posId);
                        transElementBg.gameObject.SetActive(true);
                        transElementBg.position = element.TransLinkBg.position;
                        if (needAnim)
                        {
                            element.DoChangeAnim();
                            await machineContext.WaitSeconds(delay);      
                        }
                    }
                }
            }  
        }

        private Transform GetElementLinkBg(int posId)
        {
            if (!dictLinkElementBg.ContainsKey(posId))
            {
                var goElementBg = GameObject.Instantiate(transLinkElementBg);
                goElementBg.SetParent(transBorderContainer,false);
                dictLinkElementBg.Add(posId, goElementBg);
            }
            return dictLinkElementBg[posId];
        }

        private GameObject GetDirectionLine(ConnectionRegion11010.Block.Frame.BorderDirection direction, bool start, bool end)
        {
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom || 
                direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top)
            {
                return machineContext.assetProvider.InstantiateGameObject(start?"HorizontalStart":"Horizontal");   
            }
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right || 
                direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left)
            {
                return machineContext.assetProvider.InstantiateGameObject(end?"VerticalEnd":"Vertical");   
            }
            return null;
        }
        
        private GameObject GetCornerDirectionLine(ConnectionRegion11010.Block.Frame.BorderDirection direction, ConnectionRegion11010.Block.Frame.BorderDirection directionNext)
        {
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top && direction == directionNext || 
                direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom && direction == directionNext)
            {
                return machineContext.assetProvider.InstantiateGameObject("HorizontalLine");   
            }
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left && direction == directionNext || 
                direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right && direction == directionNext)
            {
                return machineContext.assetProvider.InstantiateGameObject("VerticalLine");   
            }
            return machineContext.assetProvider.InstantiateGameObject("Corner");  
        }
        
        private float GetCornerRotation(ConnectionRegion11010.Block.Frame.BorderDirection direction, ConnectionRegion11010.Block.Frame.BorderDirection directionNext, bool isOutLine)
        {
            if (isOutLine)
            {
                if ((direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Left) || 
                    (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom))
                {
                    return 270;
                }

                if ((direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Top) || 
                    (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Left))
                {
                    return 180;
                }
                if ((direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Right) ||
                    (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Top))
                {
                    return 90;
                }                 
            }
            else
            {
                if ((direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom && 
                    directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Left) || 
                    (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Top))
                {
                    return 0;
                }

                if ((direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom && 
                    directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Right) || 
                    (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left && 
                    directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Top))
                {
                    return 270;
                }

                if ((direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left && 
                    directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom) || 
                    (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top && 
                     directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Right))
                {
                    return 180;
                }

                if ((direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right && 
                    directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom) || 
                    (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top && 
                    directionNext == ConnectionRegion11010.Block.Frame.BorderDirection.Left))
                {
                    return 90;
                }
            }

            return 0;
        }

        private Vector3 GetDrawLineWorldPosition(int id, ConnectionRegion11010.Block.Frame.BorderDirection direction)
        {
            var elementContainer = GetRunningElementContainer(id);
            var localPosition = GetElementLocalPosition(id);
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var contentSize = wheel.GetRoll(id).GetContentSize();
            var offset = Vector2.zero;
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom)
            {
                offset = new Vector2(0, -0.5f);
            }
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left)
            {
                offset = new Vector2(-0.5f, 0);
            }
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right)
            {
                offset = new Vector2(0.5f, 0);
            }
            if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top)
            {
                offset = new Vector2(0, 0.5f);
            }
            localPosition = new Vector3(localPosition.x+contentSize.x*offset.x, localPosition.y + contentSize.y*offset.y, localPosition.z);
            return elementContainer.transform.TransformPoint(localPosition);
        }

        private Vector3 GetDrawLineCornerWorldPosition(int id, ConnectionRegion11010.Block.Frame.BorderDirection direction, bool isOuterLine)
        {
            var elementContainer = GetRunningElementContainer(id);
            var localPosition = GetElementLocalPosition(id);
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var contentSize = wheel.GetRoll(id).GetContentSize();
            var offset = Vector2.zero;
            if (isOuterLine)
            {
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom)
                {
                    offset = new Vector2(-0.5f, -0.5f);
                }
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left)
                {
                    offset = new Vector2(-0.5f, 0.5f);
                }
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right)
                {
                    offset = new Vector2(0.5f, -0.5f);
                }
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top)
                {
                    offset = new Vector2(0.5f, 0.5f);
                }   
            }
            else
            {
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Bottom)
                {
                    offset = new Vector2(0.5f, -0.5f);
                }
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Left)
                {
                    offset = new Vector2(-0.5f, -0.5f);
                }
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Right)
                {
                    offset = new Vector2(0.5f, 0.5f);
                }
                if (direction == ConnectionRegion11010.Block.Frame.BorderDirection.Top)
                {
                    offset = new Vector2(-0.5f, 0.5f);
                }
            }
            localPosition = new Vector3(localPosition.x+contentSize.x*offset.x, localPosition.y + contentSize.y*offset.y, localPosition.z);
            return elementContainer.transform.TransformPoint(localPosition);
        }

        private void UpdateLinkWheelLockElements(bool isInit = false)
        {
            if (IsFromMachineSetup())
                ResetLinkWheels();
            var items = machineContext.state.Get<ExtraState11010>().GetLinkItems();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                var elementContainer = GetRunningElementContainer(id);
                if (!linkWheelState.IsRollLocked(id) && item.SymbolId>0)
                {
                    elementContainer.transform.position = GetElementPosition(id);
                    UpdateRunningElement(item.SymbolId, id,0,true);
                    if (isInit && _region11010.IsBlockId(id))
                    {
                        if (IsTriggerGrand())
                        {
                            item.WinRate *= 2;
                        }
                        var element = elementContainer.GetElement() as Element11010;
                        element?.UpdateElementContent((int) item.WinRate);
                        if (IsLinkTriggered())
                        {
                            elementContainer.PlayElementAnimation("Idle");
                        }

                        if (IsFromMachineSetup())
                        {
                            elementContainer.PlayElementAnimation("IdleCollect");    
                        }
                    }
                    linkWheelState.SetRollLockState(id, true);
                    var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                    var lockRoll = wheel.GetRoll(id) as SoloRoll;
                    lockRoll.ShiftRollMaskAndSortOrder(2100);
                }
            }
        }

        private bool UpdateLinkBlocks()
        {
            bool hasNewBlock = false;
            var regions = machineContext.state.Get<ExtraState11010>().GetLinkRegions();
            for (int i = 0; i < regions.count; i++)
            {
                if (_region11010.HasBlock(regions[i].ConnectedPositionIds))
                    continue;
                hasNewBlock = _region11010.CreateBlock(regions[i].ConnectedPositionIds);
            }

            return hasNewBlock;
        }

        private Vector3 GetElementPosition(int rollIndex, int rowIndex = 0)
        {
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            return wheel.GetRoll(rollIndex).GetVisibleContainer(rowIndex).transform.position;
        }
        
        private Vector3 GetElementLocalPosition(int rollIndex, int rowIndex=0)
        {
            var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            return wheel.GetRoll(rollIndex).GetVisibleContainer(rowIndex).transform.localPosition;
        }
        
        protected override async  Task HandleLinkReward()
        {
            if (linkCounter != null)
            {
                linkCounter.Close();
                AudioUtil.Instance.PlayAudioFx("Bonus_Complete");
                await machineContext.WaitSeconds(0.5f);
                linkCounter.transform.gameObject.SetActive(false);   
            }
            if (!IsFromMachineSetup())
            {
                ulong jackpotWin = machineContext.state.Get<ExtraState11010>().GetJackpotsWinChips();
                machineContext.state.Get<WinState>().AddCurrentWin(jackpotWin);
                var newListCoords = new List<Tuple<int, int>>();
                var blocks = _region11010.ListBlocks;
                for (int i = 0; i < blocks.Count; i++)
                {
                    newListCoords.AddRange(blocks[i].ListCoords);
                }
                int index = 1;
                newListCoords.Sort(ConnectionRegion11010.Block.Comparater);
                for (int i = 0; i < newListCoords.Count; i++)
                {
                    var coord = newListCoords[i];
                    var posId = coord.Item1 * 3 + coord.Item2;
                    var elementContainer = GetRunningElementContainer(posId);
                    var item = machineContext.state.Get<ExtraState11010>().GetLinkItemByRollId(posId);
                    if (item.SymbolId>0 && item.WinRate>0 && elementContainer!=null)
                    {
                        AudioUtil.Instance.PlayAudioFx($"Bonus_J01_Fly{index}");
                        var startPos = elementContainer.transform.position;
                        ulong winChips = machineContext.state.Get<BetState>().GetPayWinChips(item.WinRate);
                        var flyContainer = machineContext.assetProvider.InstantiateGameObject("diamondGlowFly",true);
                        flyContainer.gameObject.SetActive(true);
                        flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
                        flyContainer.transform.SetParent(machineContext.transform,false);
                        var endPos = machineContext.view.Get<ControlPanel>().WinTextRefWorldPosition;
                        await XUtility.FlyLocalAsync(flyContainer.transform, startPos, endPos, 0, 0.5f,0.4f);
                        machineContext.assetProvider.RecycleGameObject("diamondGlowFly",flyContainer);
                        flyContainer.gameObject.SetActive(false);
                        var totalWineffect = machineContext.assetProvider.InstantiateGameObject("TotaLWinEffetcs", true);
                        totalWineffect.transform.SetParent(machineContext.view.Get<ControlPanel>().transform, false);
                        totalWineffect.SetActive(true);
                        AddWinChipsToControlPanel(winChips, 0.6f,false,false);
                        elementContainer.PlayElementAnimation(Constant11010.IsJackpotElement(item.SymbolId) ? "IdleCollect": "Idle");
                        await machineContext.WaitSeconds(0.7f);
                        machineContext.assetProvider.RecycleGameObject("TotaLWinEffetcs",totalWineffect);
                        elementContainer.transform.localScale = Vector3.one;
                        index++;
                        index = index % 5 + 1;
                    }
                }   
            }
            await Task.CompletedTask;
        }
        protected override async  Task HandleLinkFinishCutSceneAnimation()
        {
            curJackpotCount = 0;
            await machineContext.state.Get<ReSpinState>().SettleReSpin();

            var linkWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            machineContext.state.Get<WheelsActiveState11010>().UpdateBaseWheelState();

            for (int i = 0; i < 15; i++)
            {
                if (linkWheelState.IsRollLocked(i))
                {
                    var lockRoll = linkWheel.GetRoll(i) as SoloRoll;
                    lockRoll.ShiftRollMaskAndSortOrder(-2100);
                }
                linkWheelState.SetRollLockState(i, false);
                if (dictLinkElementBg.ContainsKey(i))
                {
                    dictLinkElementBg[i].gameObject.SetActive(false);
                }
            }
            _region11010.ForceClearUpBlock();
            await base.HandleLinkFinishCutSceneAnimation();
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            
        }

        private void ResetLinkWheels()
        {
            var items = machineContext.state.Get<ExtraState11010>().GetLinkItems();
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                if (!Constant11010.IsNormalElementId(GetRunningElementId(i)))
                {
                    UpdateRunningElement(Constant11010.NextNormalElementId(), i);
                }
            }
        }
        
        protected override bool IsTriggerGrand()
        {
            return machineContext.state.Get<ExtraState11010>().IsTriggerGrand();
        }
        protected override bool CheckIsTriggerElement(ElementContainer container)
        {
            return Constant11010.IsLinkElement(container.sequenceElement.config.id);
        }
    }
}