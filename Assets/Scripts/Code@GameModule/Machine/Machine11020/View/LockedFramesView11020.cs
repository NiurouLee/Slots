
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Rendering;

using Random = UnityEngine.Random;

namespace GameModule
{
    // message LavaBountyGameResultExtraInfo {
    //     repeated uint32 start_wild_frames = 1;  // 起点wild
    //     repeated uint32 wild_frames = 2;        // 变成wild图标的框
    //     repeated uint32 random_frames = 3;      // 本次spin随机出现的框
    //     repeated uint32 random_lions = 4;       // 随机出现的狮子
    //     uint32 bonus_symbol_count = 5;          // 本次要随机出狮子图标的数量，freeGame才有
    //     int32 roulette_id = 6;                  // super bonus锁定的轮盘id
    //     map<uint32, LockedFrame> frames = 7;            // 普通锁定的框
    //     LockedFrame super_bonus_locked_frames = 8;      // super bonus锁定的框
    //     uint32 new_frame_count = 9;             // 本次spin新增的框的数量
    // }

    /*
        * start_wild_frames 扩散 到框 变成wild图标框
        * 每次spin前清除 wild图标框 panel 的52 图标 变框
        * 每次spin完成 
           1 . 随机出现的狮子 添加显示新增随机框
           2.  start_wild_frames 扩散 wild_frames变成wild图标的框
    */
    public class LockedFramesView11020 : TransformHolder
    {
        private static string baseLockElementName   = "Active_frame";
        private static string bonusLockElementName  = "Active_frame2";

        private uint wildElementId = Constant11020.wildElement;
        

        private Dictionary<uint, GameObject> baseLockElements;
        private Dictionary<uint, GameObject> bonusLockElements;

        private ExtraState11020 extraState;
        private WheelsActiveState11020 wheelsActiveState;


        public Wheel currentWheel;
        public bool  isSuperBonusGame;

        private Animator fireHillAnimator;

        private Transform rollsTransform;

        public LockedFramesView11020(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            baseLockElements = new Dictionary<uint, GameObject>();
            bonusLockElements = new Dictionary<uint, GameObject>();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            extraState = context.state.Get<ExtraState11020>();

            wheelsActiveState = context.state.Get<WheelsActiveState11020>();
        }

        public void StartWheel(Wheel wheel, bool restoreOld)
        {
            if (currentWheel == null)
            {
                return;
            }
            
            fireHillAnimator = wheel.transform.GetComponent<Animator>();

            currentWheel = wheel;

            rollsTransform = wheel.transform.Find("Rolls");

            isSuperBonusGame = wheel.wheelName == Constant11020.superBonusWheelName;

            wildElementId = isSuperBonusGame ? Constant11020.buleWildElement : Constant11020.wildElement;
            
            RepeatedField<uint> frames = null;

            if (restoreOld)
            {
                var curFrames = extraState.GetBetLockedFrames();

                frames = new RepeatedField<uint>();

                if (baseLockElements != null)
                {
                    foreach (var id in baseLockElements.Keys)
                    {
                        if (curFrames != null && curFrames.Contains(id) || IsElementAtCell(id, Constant11020.wildElement))
                        {
                            frames.Add(id);
                        }
                    }
                }

                if (bonusLockElements != null)
                {
                    foreach (var id in bonusLockElements.Keys)
                    {
                        if (!frames.Contains(id))
                        {
                            if (curFrames.Contains(id) || IsElementAtCell(id, Constant11020.wildElement))
                            {
                                frames.Add(id);
                            }
                        }
                    }
                }
            }

            Clear();
            UpdateLockedFames(frames);
        }
        

        public async Task HandleSpinStopLogic()
        {
            // await context.WaitSeconds(0.2f);

            await StartWildExtends();

            // extraState.dump();

            // await context.WaitSeconds(1.5f);
        }

        public async void HandleSubRoundStart()
        {
            FireBallSymbolsToFramesOfResoult();

            ClearWildSymbolFrames();
            UpdateAllLockFrameCorners();
        }

        private void UpdateLockedFames(RepeatedField<uint> frames)
        {
            if (frames == null)
            {
                frames = extraState.GetBetLockedFrames();
            }
            
            if (frames != null)
            {
                for (var i = 0; i < frames.Count; ++i)
                {
                    AddFrameElement(baseLockElementName, frames[i]);
                }

                UpdateAllLockFrameCorners();
            }
        }

        public async void ShowSuperBonusLockedFrames()
        {
            if (!isSuperBonusGame)
            {
                return;
            }

            var bonusframes = extraState.GetSuperBonusLockedFrames();
            if (bonusframes != null)
            {
                for (var i = 0; i < bonusframes.Count; ++i)
                {
                    if (HasLockedFrame(bonusframes[i]))
                    {
                        RemoveFrameElement(bonusframes[i]);
                    }
                    AddFrameElement(bonusLockElementName, bonusframes[i], true);
                }
            }
            UpdateAllLockFrameCorners();
        } 

        private void FireBallSymbolsToFramesOfResoult()
        {
            var panel = wheelsActiveState.GetPanel();
            if (panel == null)
            {
                return;
            }

            var betFrames = extraState.GetBetLockedFrames(); 

            var bonusframes = isSuperBonusGame ? extraState.GetSuperBonusLockedFrames() : null;
            
            uint index = 1;
            uint id    = 0;

            var columns = panel.Columns;

            var bUpdateCorner = false;

            for (var i = 0; i < columns.Count; ++i)
            {
                var symbols = columns[i].Symbols;

                for (var k = 0; k < symbols.Count; ++k)
                {
                    id = symbols[k];

                    if (id == Constant11020.lionElement && betFrames != null && betFrames.Contains(index))
                    {
                        if ((!isSuperBonusGame || bonusframes == null || !bonusframes.Contains(index)) && !HasLockedFrame(id))
                        {
                            AddFrameElement(baseLockElementName, index, true);

                            bUpdateCorner = true;
                        }
                    }

                    ++index;
                }
            }

            if (bUpdateCorner)
            {   
                AudioUtil.Instance.PlayAudioFx("Turns_Into_Frame");
                UpdateAllLockFrameCorners();
            }            
        }

        public int GetRandomFrameCount()
        {
            var frames = extraState.GetRandomFrames();
            if (frames == null || frames.Count < 1)
            {
                return 0;
            }

            return frames.Count;
        }

        public async void ShowRandomFrames(Action actionEnd)
        {
            var frames = extraState.GetRandomFrames();
            if (frames == null || frames.Count < 1)
            {
                actionEnd?.Invoke();
                return;
            }

            AudioUtil.Instance.PlayAudioFx("Random_Eruption");

            var animator = context.transform.GetComponent<Animator>();
            if (animator != null)
            {
                XUtility.PlayAnimation(animator, "Shake");
            }

            XUtility.PlayAnimation(fireHillAnimator, "Open", ()=>{
                XUtility.PlayAnimation(fireHillAnimator, "Idle");
            });

            await context.WaitSeconds(0.7f);

            bool bRandom = true;

            for (var i = 0; i < frames.Count; ++i)
            {   
                ShowOneRandomFrame(frames[i]);

                await context.WaitSeconds(1.0f);
            }
            await context.WaitSeconds(2.0f);
        
            actionEnd?.Invoke();
        }

        private async void ShowOneRandomFrame(uint frameAt)
        {
            // var tim = Random.Range(0, 1000);

            // if (tim > 50)
            // {
            //     //await context.WaitSeconds(tim/1000.0f);
            // }

            var position = context.transform.Find($"Wheels/FrameContainers/Container{frameAt}").transform.position;
            var p = new Vector3(position.x,position.y,position.z);
            p.y += 15.0f;
            var t = new Vector3(p.x, position.y, p.z);

            AudioUtil.Instance.PlayAudioFx("Random_Fire");
            FallFireBall(p, t, null);
            var animator = context.transform.GetComponent<Animator>();
            if (animator != null)
            {
                XUtility.PlayAnimation(animator, "Shake3");
            }
            await context.WaitSeconds(0.6f);

            AudioUtil.Instance.PlayAudioFx("Firs_EruptionChange");
            AddFrameElement(baseLockElementName, frameAt, true);

            // if (fireHillAnimator != null)
            // {
            //     XUtility.PlayAnimation(fireHillAnimator, "Shock", 
            //         ()=>{
            //             XUtility.PlayAnimation(fireHillAnimator, "Idle");
            //         }
            //     );
            // }

            UpdateAllLockFrameCorners();
        }

        public void ClearWildSymbolFrames()
        {
            RepeatedField<uint> wildFrames;
            if (!extraState.isAfterSettle)
            {
                wildFrames = extraState.GetWildFrames();
            }
            else
            {
                wildFrames = extraState.GetLastWildFrames();
            }

            if (wildFrames == null)
            {
                return;
            }

            var frames = isSuperBonusGame ? extraState.GetSuperBonusLockedFrames() : extraState.GetBetLockedFrames();

            if (frames != null)
            {
                uint id = 0;

                for (var i = 0; i < wildFrames.Count; ++i)
                {
                    id = wildFrames[i];
                    if (!frames.Contains(id))
                    {
                        RemoveFrameElement(id);
                    }
                }
            }
        }

        public void Clear()
        {
            ClearFrames();
            ClearWildSymbolFrames();
        }

        public void ClearFrames()
        {
            if (baseLockElements.Count > 0)
            {
                foreach( var t in baseLockElements.Values)
                {
                    context.assetProvider.RecycleGameObject(baseLockElementName, t);
                }
                
                baseLockElements.Clear();
            }

            if (bonusLockElements.Count > 0)
            {
                foreach( var t in bonusLockElements.Values)
                {
                    context.assetProvider.RecycleGameObject(bonusLockElementName, t);
                }

                bonusLockElements.Clear();
            }
        }

        public bool HasLockedFrame(uint id)
        {

            if (baseLockElements.ContainsKey(id))
            {
                return true;
            }

            if (bonusLockElements.ContainsKey(id))
            {
                return true;
            }

            return false;
        }

        private void AddFrameElement(string name, uint id, bool appearAnimation = false)
        {
            if (HasLockedFrame(id))
            {
                return;
            }

            if (context.state.Get<WheelsActiveState11020>().CurTotalBet == context.state.Get<BetState>().totalBet && 
                context.state.Get<WheelsActiveState11020>().ListNewIds.Count > 0 &&
                context.state.Get<WheelsActiveState11020>().ListNewIds.IndexOf(id)>=0)
            {
                return;
            }

            var elem = context.assetProvider.InstantiateGameObject(name, true);
            var gameObjElem = elem.transform;
            var cornerL = gameObjElem.Find("CornerL");
            var cornerR = gameObjElem.Find("CornerR");
            var cornerBoth = gameObjElem.Find("CornerBoth");
            SetTransformActive(cornerL, false);
            SetTransformActive(cornerR, false);
            SetTransformActive(cornerBoth, false);

            var cornerLUp = gameObjElem.Find("CornerLUp");
            var cornerRUp = gameObjElem.Find("CornerRUp");
            var cornerBothUp = gameObjElem.Find("CornerBothUp");
            
            SetTransformActive(cornerLUp, false);
            SetTransformActive(cornerRUp, false);
            SetTransformActive(cornerBothUp, false);
            if (elem != null)
            {
                elem.name = name + id;

                int rollIndex = 0;
                int rowIndex  = 0;

                GetElementContainer(id, out rollIndex, out rowIndex);
        
                var p = context.transform.Find($"Wheels/FrameContainers/Container{id}").transform;
                elem.transform.SetParent(p.transform,false);
                elem.transform.localScale = Vector3.one;
                elem.transform.localPosition = Vector3.zero;

                var sortingGroup = elem.GetComponent<SortingGroup>();
                // sortingGroup.sortingOrder = ((rollIndex == 1 || rollIndex == 3) ? 200 : 1000) + (rollIndex+1)*200-100+1;

                
                // sortingGroup.sortingOrder = 200 + (rollIndex+1)*200-100+1;
                rowIndex = 10-rowIndex;

                if (baseLockElementName == name)
                {
                    sortingGroup.sortingOrder = 1800 - rowIndex;

                    baseLockElements[id] = elem;
                }
                else
                {
                    sortingGroup.sortingOrder = 1801 - rowIndex;
                    bonusLockElements[id] = elem;
                }

                var animator = elem.GetComponent<Animator>();
                if (animator != null)
                {
                    if (appearAnimation)
                    {
                        XUtility.PlayAnimation(animator, "Win",
                            ()=>{
                                XUtility.PlayAnimation(animator, "Idle");
                            }
                        );
                    }
                    else
                    {
                        XUtility.PlayAnimation(animator, "Idle");
                    }
                }
            }
        }


        private bool RemoveFrameElement(uint id)
        {
            if (baseLockElements.ContainsKey(id))
            {
                context.assetProvider.RecycleGameObject(baseLockElementName, baseLockElements[id]);

                baseLockElements.Remove(id);

                return true;
            }

            if (bonusLockElements.ContainsKey(id))
            {
                context.assetProvider.RecycleGameObject(bonusLockElementName, bonusLockElements[id]);

                bonusLockElements.Remove(id);

                return true;
            }

            return false;
        }

        private void makeFrameToBonusFrame(uint id)
        {
            if (!isSuperBonusGame)
            {
                return;
            }

            if (bonusLockElements.ContainsKey(id))
            {
                return;
            }

            if (!baseLockElements.ContainsKey(id))
            {
                return;
            }

            context.assetProvider.RecycleGameObject(baseLockElementName, baseLockElements[id]);
            baseLockElements.Remove(id);

            AddFrameElement(bonusLockElementName, id);
            UpdateAllLockFrameCorners();
        }

        private ElementContainer GetElementContainer(uint id, out int rollIndex, out int rowIndex)
        {
            rollIndex = 0;
            rowIndex  = (int)id - 1;

            if (id >= 5 && id <= 9)
            {
                rollIndex = 1;
                rowIndex  = (int)id - 5;
            }
            else if (id >= 10 && id <= 13)
            {
                rollIndex = 2;
                rowIndex  = (int)id - 10;
            }
            else if (id >= 14 && id <= 18)
            {
                rollIndex = 3;
                rowIndex  = (int)id - 14;
            }
            else if (id >= 19 && id <= 22)
            {
                rollIndex = 4;
                rowIndex  = (int)id - 19;
            }

            var elementContainer = currentWheel.GetRoll(rollIndex).GetVisibleContainer(rowIndex);
            
            // var elementContainer = currentWheel.GetWinLineElementContainer(rollIndex, rowIndex);

            return elementContainer;
        }
        

        private bool IsElementAtCell(uint id, uint elemId)
        {
            int rollIndex = 0;
            int rowIndex  = 0;

            var elementContainer = GetElementContainer(id, out rollIndex, out rowIndex);
            if (elementContainer != null)
            {
                if (elementContainer.sequenceElement.config.id == elemId)
                {
                    return true;
                }
            }
            return false;
        }

        private async  Task StartWildExtends()
        {
            var startFrames = extraState.GetStartWildFrames();
            if (startFrames == null || startFrames.Count < 1)
            {
                return;
            }
            
            AudioUtil.Instance.PlayAudioFx("Frame_Animation");

            var num = startFrames.Count;

            var extendedIds = new List<uint>();     

            uint id = 0;

            for (var i = 0; i < num; ++i)
            {
                id = startFrames[i];
                
                extendedIds.Add(id);

                makeFrameToBonusFrame(id);
                XDebug.Log("StartWildExtends_id======:"+id);
                UpdateRollElement(id, wildElementId, true);
            }       

            await context.WaitSeconds(0.8f);
            var count = 0;

            List<uint> points = new List<uint>();
            for (var i = 0; i < num; ++i)
            {
                id = startFrames[i];
                var ps = DoExtends(id, extendedIds);
                for (var k = 0; k < ps.Count; ++k)
                {
                    if (!points.Contains(ps[k]))
                    {
                        points.Add(ps[k]);
                    }
                }
            }

            while(points.Count > 0)
            {
                await context.WaitSeconds(0.4f);

                List<uint> points1 = new List<uint>();

                points.ForEach(i => points1.Add(i));

                points.Clear();

                for (var i = 0; i < points1.Count; ++i)
                {
                    id = points1[i];
                    
                    var ps = DoExtends(id, extendedIds);

                    for (var k = 0; k < ps.Count; ++k)
                    {
                        if (!points.Contains(ps[k]))
                        {
                            points.Add(ps[k]);
                        }
                    }
                }
            }
            await context.WaitSeconds(0.6f);
        }

        private List<uint> DoExtends(uint sid, List<uint> extendedIds)
        {
            var positions = ExtendPositions(sid, extendedIds);
            var num = positions.Count;

            if (num > 0)
            {
                AudioUtil.Instance.PlayAudioFx("Transfering");

                var count = 0;

                var sp = context.transform.Find($"Wheels/FrameContainers/Container{sid}").transform.position;
                
                for (var i = 0 ; i < num; ++i)
                {
                    var id = positions[i];

                    if (!extendedIds.Contains(id))
                    {
                        extendedIds.Add(id);
                    }
                    
                    var p = context.transform.Find($"Wheels/FrameContainers/Container{id}").transform.position;

                    FlyTrail(sp, p, 
                        ()=>{
                            
                            makeFrameToBonusFrame(id);

                            UpdateRollElement(id, wildElementId, false, true);
                        }
                    );
                }
            }

            return positions;
        }

        private List<uint> ExtendPositions(uint sid, List<uint> extendedIds)
        {
            List<uint> list = new List<uint>();

            uint id = 0;
            for (uint i =0; i < 6; ++i)
            {
                if (sid > 0 && sid < 23)
                {
                    id = Constant11020.neighborPositions[sid-1, i];
                    if (id > 0)
                    {
                        if (!extendedIds.Contains(id) && extraState.IsWildFrame(id))
                        {
                            list.Add(id);
                        }
                    }
                }
            }

            return list;
        }

        private async void UpdateRollElement(uint index, uint symbolId, bool fireBallChangedAniation = false, bool checkBonusWild = false)
        {
            int rollIndex = 0;
            int rowIndex  = 0;

            var elementContainer = GetElementContainer(index, out rollIndex, out rowIndex);

            if (elementContainer != null)
            {
                if (checkBonusWild)
                {
                    if (elementContainer.sequenceElement.config.id == Constant11020.bonusElement)
                    {
                        symbolId = Constant11020.bonusWildElement;
                    }
                }

                var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
                var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(symbolId), context);

                if (fireBallChangedAniation)
                {
                    XDebug.Log("elementContainer====:"+elementContainer);
                    elementContainer.PlayElementAnimation("Change", false);

                    await context.WaitSeconds(0.3f);

                    elementContainer.UpdateElement(seqElement, true);
                    elementContainer.PlayElementAnimation("Diffusion", false, 
                        ()=>{
                            elementContainer.PlayElementAnimation("Idle");
                        }
                    );
                }
                else
                {
                    elementContainer.UpdateElement(seqElement, true);
                    elementContainer.PlayElementAnimation(symbolId == Constant11020.bonusWildElement ? "Open" : "Diffusion", false, 
                        ()=>{
                            elementContainer.PlayElementAnimation("Idle");
                        }
                    );
                }
                
                if (symbolId == wildElementId)
                {
                    elementContainer.UpdateExtraSortingOrder(0);
                }
            }
        }

        private void FlyTrail(Vector3 sp, Vector3 ep, Action actionEnd)
        {
            var fx = context.assetProvider.InstantiateGameObject("FlyTail", true);

            fx.transform.SetParent(rollsTransform,false);
            fx.transform.position = sp;
            
            var dir = new Vector3(ep.x - sp.x, ep.y - sp.y, 0);

            var angle = Vector3.SignedAngle(Vector3.left, dir, Vector3.forward);
            
            fx.transform.rotation = Quaternion.Euler(0, 0, angle);

            XUtility.Fly(fx.transform, 
                fx.transform.position, 
                ep, 
                0, 
                0.3f,
                async () =>
                {
                    // await context.WaitSeconds(0.1f);

                    actionEnd?.Invoke();
                    await context.WaitSeconds(0.3f);
                    context.assetProvider.RecycleGameObject("FlyTail", fx);
                }, 
                Ease.Linear, 
                context
            );
        }

        private void FallFireBall(Vector3 sp, Vector3 ep, Action actionEnd)
        {
            AudioUtil.Instance.PlayAudioFx("Firs_EruptionDown");

            var fx = context.assetProvider.InstantiateGameObject("Wheel02_Trigger", true);

            fx.transform.SetParent(rollsTransform);
            fx.transform.position = sp;
            fx.transform.localScale = Vector3.one;
            
            var dir = new Vector3(ep.x - sp.x, ep.y - sp.y, 0);

            // var angle = Vector3.SignedAngle(Vector3.left, dir, Vector3.forward);
            
            // fx.transform.rotation = Quaternion.Euler(0, 0, angle);

            XUtility.PlayAnimation(fx.transform.GetComponent<Animator>(),"Wheel02_Trigger_Open");
            XUtility.Fly(fx.transform, 
                fx.transform.position, 
                ep, 
                0, 
                0.5f,
                async () =>
                {
                    await context.WaitSeconds(0.5f);
                    context.assetProvider.RecycleGameObject("Wheel02_Trigger", fx);

                    actionEnd?.Invoke();
                }, 
                Ease.Linear, 
                context
            );
        }

        private void ScaleLocal(Transform tran, Action actionEnd = null)
        {
            tran.DOKill();
            tran.DOScale(new Vector3(1.5f, 1.5f, 1.0f), 0.5f).OnComplete(
                async () =>
                {
                    await context.WaitSeconds(0.5f);
                    
                    tran.DOKill();
                    tran.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f).OnComplete(
                        () =>
                        {
                            actionEnd?.Invoke();
                        }
                    );
                }
            );
        }

        private void UpdateAllLockFrameCorners()
        {
            foreach( var id in bonusLockElements.Keys)
            {
                UpdateLockFrameCorner(bonusLockElements[id].transform, id);
            }

            foreach( var id in baseLockElements.Keys)
            {
                UpdateLockFrameCorner(baseLockElements[id].transform, id);
            }
            foreach( var id in baseLockElements.Keys)
            {
                CheckShowBottomCircle(baseLockElements[id].transform, id);
            }
        }

        private void CheckShowBottomCircle(Transform tran, uint sid)
        {
            var nextSid = sid + 1;
            if (sid>=1 && sid<=3 || 
                 sid>=5 && sid<=8 ||
                 sid>=10 && sid<=12 ||
                 sid>=14 && sid<=17 ||
                 sid>=19 && sid<=21)
            {
                var cornerL = tran.Find("CornerL").gameObject;
                var cornerR = tran.Find("CornerR").gameObject;
                var cornerNone = tran.Find("CornerNone").gameObject;
                    
                var cornerLUp = tran.Find("CornerLUp").gameObject;
                var cornerRUp = tran.Find("CornerRUp").gameObject;
                var cornerNoneUp = tran.Find("CornerNoneUp").gameObject;
                var hasNext = baseLockElements.ContainsKey(nextSid) || bonusLockElements.ContainsKey(nextSid);
                if (hasNext)
                {
                    if (cornerL.activeSelf && cornerR.activeSelf)
                    {
                        cornerL.gameObject.SetActive(false);
                        cornerR.gameObject.SetActive(false);
                        cornerLUp.gameObject.SetActive(true);
                        cornerRUp.gameObject.SetActive(true);
                    }
                    else if (cornerNone.activeSelf)
                    {
                        cornerNone.gameObject.SetActive(false);  
                        cornerNoneUp.gameObject.SetActive(true);
                    } 
                    else if (cornerL.activeSelf)
                    {
                        cornerL.gameObject.SetActive(false);  
                        cornerLUp.gameObject.SetActive(true); 
                    }
                    else if (cornerR.activeSelf)
                    {
                        cornerR.gameObject.SetActive(false);  
                        cornerRUp.gameObject.SetActive(true); 
                    }
                }
                else
                {
                    if (cornerLUp.activeSelf && cornerRUp.activeSelf)
                    {
                        cornerL.gameObject.SetActive(true);
                        cornerR.gameObject.SetActive(true);
                        cornerLUp.gameObject.SetActive(false);
                        cornerRUp.gameObject.SetActive(false);
                    }
                    else if (cornerNoneUp.activeSelf)
                    {
                        cornerNone.gameObject.SetActive(true);  
                        cornerNoneUp.gameObject.SetActive(false);
                    } 
                    else if (cornerLUp.activeSelf)
                    {
                        cornerL.gameObject.SetActive(true);  
                        cornerLUp.gameObject.SetActive(false); 
                    }
                    else if (cornerRUp.activeSelf)
                    {
                        cornerR.gameObject.SetActive(true);  
                        cornerRUp.gameObject.SetActive(false); 
                    }
                }
            }
        }

        private void UpdateLockFrameCorner(Transform tran, uint sid)
        {
            uint id = 0;

            var visibleCount = 0;

            var visible = false;
            
            var cornerL = tran.Find("CornerL");
            var cornerR = tran.Find("CornerR");
            var cornerBoth = tran.Find("CornerBoth");
            SetTransformActive(cornerL, false);
            SetTransformActive(cornerR, false);
            SetTransformActive(cornerBoth, false);

            var cornerLUp = tran.Find("CornerLUp");
            var cornerRUp = tran.Find("CornerRUp");
            var cornerBothUp = tran.Find("CornerBothUp");
            SetTransformActive(cornerLUp, false);
            SetTransformActive(cornerRUp, false);
            SetTransformActive(cornerBothUp, false);
            
        
            // left corner
            var corner = tran.Find("CornerL").gameObject;
            if (sid > 4 && sid < 23)
            {
                id = Constant11020.leftCorners[sid-1, 0];
                if (id > 0)
                {
                    visible = baseLockElements.ContainsKey(id) || bonusLockElements.ContainsKey(id);
                }
                if (!visible)
                {
                    id = Constant11020.leftCorners[sid-1, 1];
                    if (id > 0)
                    {
                        visible = baseLockElements.ContainsKey(id) || bonusLockElements.ContainsKey(id);
                    }
                }
                if (visible)
                {
                    ++visibleCount;
                }
            }
          
            corner.SetActive(visible);
            
            visible = false;
            //right corner
            corner = tran.Find("CornerR").gameObject;
            if (sid < 19 && sid > 0)
            {
                id = Constant11020.rightCorners[sid-1, 0];
                if (id > 0)
                {
                    visible = baseLockElements.ContainsKey(id) || bonusLockElements.ContainsKey(id);
                }
                if (!visible)
                {
                    id = Constant11020.rightCorners[sid-1, 1];
                    if (id > 0)
                    {
                        visible = baseLockElements.ContainsKey(id) || bonusLockElements.ContainsKey(id);
                    }
                }
                if (visible)
                {
                    ++visibleCount;                    
                }
            }

            corner.SetActive(visible);

            tran.Find("CornerBoth").gameObject.SetActive(false);
            tran.Find("CornerNone").gameObject.SetActive(visibleCount == 0);
        }
    }
}
