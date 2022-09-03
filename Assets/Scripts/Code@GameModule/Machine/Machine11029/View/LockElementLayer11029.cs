using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LockElementLayer11029 : LockElementLayer
    {
        public List<EyeOfMedusaGameResultExtraInfo.Types.RepeatedPositions> lastRandomWildPosList;
        public List<EyeOfMedusaGameResultExtraInfo.Types.RepeatedPositionIds> lastStickyWildPosList;
        public List<EyeOfMedusaGameResultExtraInfo.Types.RepeatedMovingPositionIds> lastMovingWildPosList;
        public List<EyeOfMedusaGameResultExtraInfo.Types.Position> freeLastRandomWildPosList;
        public List<Element> _newCoinList = new List<Element>();
        public List<Element> _newBagList = new List<Element>();
        public List<GameObject> _FireList = new List<GameObject>();
        public List<Element> _newBigScatter = new List<Element>();
        public List<Element> _newMovingWild1 = new List<Element>();
        public List<Element> _newMovingWild2 = new List<Element>();
        public List<Element> _newMovingWild3 = new List<Element>();
        public List<GameObject> _newFireList = new List<GameObject>();

        public LockElementLayer11029(Transform transform)
            : base(transform)
        {
            freeLastRandomWildPosList = new List<EyeOfMedusaGameResultExtraInfo.Types.Position>();
        } 
        
        //回收产生的随机wild
        public void RecyleRandomWild()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetFreeRandomWildIds();
            if (listWildPos.Count > 0)
            {
                foreach (var wildPos in listWildPos)
                {
                    ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                }
            }
        }
        
        public void RecyleAllMapGameFire(int index = -1)
        {
            for (int m = 0; m < _newFireList.Count; m++)
            {
                context.assetProvider.RecycleGameObject("Fx_W01_Diffusion", _newFireList[m]);
            }
            Debug.LogError($"RecyleAllMapGameFire count = {_newFireList.Count} index = {index}");
            _newFireList.Clear();
        }

        //回收MapGame随机产生的wild
        public void RecyleMapGameRandomWild1()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetRandomWildIds()[0].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                    }
                }
            }
        }

        public void RecyleMapGameRandomWild2()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetRandomWildIds()[1].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                    }
                }
            }
        }

        public void RecyleMapGameRandomWild3()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetRandomWildIds()[2].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                    }
                }
            }
        }

        //回收MapGame随机产生的wild
        public void RecyleMapGameStickyWild1()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetStickyWildIds()[0].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                    }
                }
            }
        }

        public void RecyleMapGameStickyWild2()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetStickyWildIds()[1].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                    }
                }
            }
        }

        public void RecyleMapGameStickyWild3()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetStickyWildIds()[2].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                    }
                }
            }
        }

        //回收MapGame随机产生的wild
        public void RecyleMapGameM0vingWild1()
        {
            if (_newMovingWild1.Count > 0)
            {
                for (int n = _newMovingWild1.Count - 1; n >= 0; n--)
                {
                    _newMovingWild1.Remove(_newMovingWild1[n]);
                }
            }

            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetMovingWildIds()[0].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                        ClearAllLayer((int) wildPos.OldX, (int) wildPos.OldY);
                    }
                }
            }
        }

        public void RecyleMapGameMovingWild2()
        {
            if (_newMovingWild2.Count > 0)
            {
                for (int n = _newMovingWild2.Count - 1; n >= 0; n--)
                {
                    _newMovingWild2.Remove(_newMovingWild2[n]);
                }
            }

            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetMovingWildIds()[1].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                        ClearAllLayer((int) wildPos.OldX, (int) wildPos.OldY);
                    }
                }
            }
        }

        public void RecyleMapGameMovingWild3()
        {
            if (_newMovingWild3.Count > 0)
            {
                for (int n = _newMovingWild3.Count - 1; n >= 0; n--)
                {
                    _newMovingWild3.Remove(_newMovingWild3[n]);
                }
            }

            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetMovingWildIds()[2].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                        ClearAllLayer((int) wildPos.OldX, (int) wildPos.OldY);
                    }
                }
            }
        }

        public void ClearAllLayer(int row, int column)
        {
            ClearElement(row, column);
        }

        //随机投放wild
        public async Task ShowRandomWildElement()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetFreeRandomWildIds();
            if (listWildPos.Count > 0)
            {
                //美杜莎特效
                context.view.Get<MeiDuSha11029>().PlayShowFire();
                //转盘震屏特效
                await context.WaitSeconds(0.53f);
                context.view.Get<MeiDuSha11029>().ShowLight();
                await context.WaitSeconds(0.2f);
                var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                wheel.transform.GetComponent<Animator>().Play("Open");
                //添加黑色遮照
                wheel.GetContext().state.Get<WheelsActiveState11029>().ShowRollsMasks(wheel);
                await context.WaitSeconds(2.5f - 0.53f - 0.2f);
                AudioUtil.Instance.PlayAudioFx("FreeGame_Character_Select");
                foreach (var wildPos in listWildPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    //曝光
                    var objFly = context.assetProvider.InstantiateGameObject("Fx_W01_Diffusion", true);
                    objFly.transform.parent = context.transform;
                    objFly.transform.localScale = context.transform.Find("WheelFeature/Wheels").localScale;
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    var startPos = endPos;
                    objFly.transform.position = startPos;
                    objFly.SetActive(true);
                    if (!objFly.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = objFly.gameObject.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                        sortingGroup.sortingOrder = 12000;
                    }
                    _FireList.Add(objFly);
                }

                await context.WaitSeconds(0.3f);
                foreach (var wildPos in listWildPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int) wildPos.Y);
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    //更换wild
                    LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                        (int) wildPos.X, (int) wildPos.Y, true);
                    var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
                    newWild.transform.position = endPos;
                    newWild.UpdateMaskInteraction(SpriteMaskInteraction.None);
                }
                await context.WaitSeconds(3.0f - 2.8f);
                context.view.Get<MeiDuSha11029>().HideLight();
                context.state.Get<WheelsActiveState11029>().FadeOutRollMask(wheel);
                for (int m = _FireList.Count - 1; m >= 0; m--)
                {
                    context.assetProvider.RecycleGameObject("Fx_W01_Diffusion", _FireList[m]);
                    _FireList.Remove(_FireList[m]);
                }
            }
        }
        
        /*
         *
         * Proxy -> 3 Layer
         *
         * Layer movingWild
         * Layer movingWild
         * Layer movingWild
         * 
         */

        public void ShowMapGameFireElement(Wheel wheel, EyeOfMedusaGameResultExtraInfo.Types.Position wildPos)
        {
            var roll = wheel.GetRoll((int) wildPos.X);
            var container = roll.GetVisibleContainer((int) wildPos.Y);
            //曝光
            var objFly = context.assetProvider.InstantiateGameObject("Fx_W01_Diffusion", true);
            objFly.transform.parent = context.transform;
            objFly.transform.localScale = context.transform.Find("WheelFeature/Wheels").localScale;
            var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
            var startPos = endPos;
            objFly.transform.position = startPos;
            objFly.SetActive(true);
            if (!objFly.GetComponent<SortingGroup>())
            {
                var sortingGroup = objFly.gameObject.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                sortingGroup.sortingOrder = 12000;
            }
            _newFireList.Add(objFly);
        }
        

        public void ShowMapWildElement(Wheel wheel, EyeOfMedusaGameResultExtraInfo.Types.Position wildPos, int index = -1)
        {
            Debug.LogError($"ShowMapWildElement index = {index}");
            var roll = wheel.GetRoll((int) wildPos.X);
            var container = roll.GetVisibleContainer((int) wildPos.Y);
            var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
            //更换wild
            LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                (int) wildPos.X, (int) wildPos.Y, true);
            var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
            newWild.transform.position = endPos;
            newWild.UpdateMaskInteraction(SpriteMaskInteraction.None);
            newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
        }
        
        public void ShowMapGame1StickyWildElement()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetStickyWildIds()[0].Items;
            if (listWildPos.Count > 0)
            {
                var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
                foreach (var wildPos in listWildPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int) wildPos.Y);
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    //更换wild
                    LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                        (int) wildPos.X, (int) wildPos.Y, true);
                    var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
                    newWild.transform.position = endPos;
                    newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                }
            }
        }

        //MapGame1随机投放wild
        public void ShowMapGame2StickyWildElement()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetStickyWildIds()[1].Items;
            if (listWildPos.Count > 0)
            {
                var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[1];
                foreach (var wildPos in listWildPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int) wildPos.Y);
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    //更换wild
                    LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                        (int) wildPos.X, (int) wildPos.Y, true);
                    var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
                    newWild.transform.position = endPos;
                    newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                }
            }
        }

        public void ShowMapGame3StickyWildElement()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetStickyWildIds()[2].Items;
            if (listWildPos.Count > 0)
            {
                var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[2];
                foreach (var wildPos in listWildPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int) wildPos.Y);
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    //更换wild
                    LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                        (int) wildPos.X, (int) wildPos.Y, true);
                    var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
                    newWild.transform.position = endPos;
                    newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                }
            }
        }


        //长图美杜莎
        public void ShowBigScatterElement()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var dragdPos = extraState.GetDragPos();
            var listKeys = new List<uint>(dragdPos.Keys);
            var rollIndex = 0;
            var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            var roll = wheel.GetRoll(0);
            var container = roll.GetVisibleContainer(1);
            var endPos = wheel.GetRoll(rollIndex).GetVisibleContainerPosition(1);
            //更换wild
            LockElement(context.machineConfig.GetSequenceElement(Constant11029.BigScatterElementId, context),
                0, 1, true);
            var newWild = GetElement(0, 1);
            newWild.transform.position = endPos;
            newWild.transform.gameObject.GetComponent<Animator>().Play("Idle");
            _newBigScatter.Add(newWild);
        }

        public void PlayInClose()
        {
            if (_newBigScatter.Count > 0)
            {
                for (int i = 0; i < _newBigScatter.Count; i++)
                {
                    _newBigScatter[i].transform.gameObject.GetComponent<Animator>().Play("Close");
                    _newBigScatter.Remove(_newBigScatter[i]);
                }
            }
        }

        public void PlayInWin()
        {
            if (_newBigScatter.Count > 0)
            {
                for (int i = 0; i < _newBigScatter.Count; i++)
                {
                    _newBigScatter[i].transform.gameObject.GetComponent<Animator>().Play("Win");
                }
            }
        }

        public void PlayInBonusWin()
        {
            if (_newBigScatter.Count > 0)
            {
                for (int i = 0; i < _newBigScatter.Count; i++)
                {
                    _newBigScatter[i].transform.gameObject.GetComponent<Animator>().Play("WinBouns");
                }
            }
        }

        public void RecyleBigScatterElement()
        {
            ClearAllLayer(0, 1);
        }

        //Base收集S01
        public async Task CollectHorse()
        {
            var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            var newHorseList = new List<Element>();
            var endPos = context.view.Get<ProgressBar11029>().GetIntegralPos();
            for (int i = 0; i < wheel.rollCount; i++)
            {
                for (int j = 0; j < wheel.GetRoll(i).rowCount; j++)
                {
                    var elementContainer = wheel.GetRoll(i).GetVisibleContainer(j);
                    if (Constant11029.HorseElementId == elementContainer.sequenceElement.config.id)
                    {
                        var startPos = elementContainer.transform.position;
                        LockElement(context.machineConfig.GetSequenceElement(Constant11029.HorseElementId, context),
                            i, j, true);
                        var newHorse = GetElement(i, j);
                        newHorse.transform.position = startPos;
                        newHorse.transform.gameObject.GetComponent<Animator>().Play("Blink");
                        newHorseList.Add(newHorse);
                    }
                }
            }
            AudioUtil.Instance.PlayAudioFx("Map_Fly");
            await context.WaitSeconds(0.2f);
            var finishCount = 0;
            var horseCount = newHorseList.Count;
            var waitTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(waitTask,null);
            
            for (int i = 0; i < newHorseList.Count; i++)
            {
                var startPos = newHorseList[i].transform.position;
                XUtility.Fly(newHorseList[i].transform, startPos, endPos, 0, 0.6f, ()=>
                {
                    finishCount++;
                    if (finishCount == horseCount)
                    {
                         context.RemoveTask(waitTask);
                         waitTask.SetResult(true);
                    }
                }, context:context);
            }
            await waitTask.Task;
            ClearAllElement();
        }

        //合成bag长图
        public async Task ShowBigBagElement(int rollIndex)
        {
            var extraState = context.state.Get<ExtraState11029>();
            var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            var roll = wheel.GetRoll(rollIndex);
            var container = roll.GetVisibleContainer(1);
            var endPos = wheel.GetRoll(rollIndex).GetVisibleContainerPosition(1);
            //更换wild
            LockElement(context.machineConfig.GetSequenceElement(Constant11029.BigBagElementId, context),
                rollIndex, 1, true);
            var newBag = GetElement(rollIndex, 1);
            newBag.transform.position = endPos;
            _newBagList.Add(newBag);
        }

        public async Task PlayWin()
        {
            var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            var endPos = context.view.Get<MoneyBag11029>().GetIntegralPos();
            AudioUtil.Instance.PlayAudioFx("Wallet_Fly");
            if (_newBagList.Count > 0)
            {
                for (int i = 0; i < _newBagList.Count; i++)
                {
                    _newBagList[i].transform.gameObject.GetComponent<Animator>().Play("Win");
                }
                await context.WaitSeconds(0.2f);
                for (int i = 0; i < _newBagList.Count; i++)
                {
                    var startPos = _newBagList[i].transform.position;
                    XUtility.Fly(_newBagList[i].transform, startPos, endPos, 0, 0.4f, null);
                }
                await context.WaitSeconds(0.4f);
                AudioUtil.Instance.PlayAudioFx("Wallet_Toggle");
                await context.WaitSeconds(0.3f);
                for (int i = 0; i < _newBagList.Count; i++)
                {
                    _newBagList[i].DoRecycle();
                }

                for (int i = 0; i < wheel.rollCount; i++)
                {
                    for (int j = 0; j < wheel.GetRoll(i).rowCount; j++)
                    {
                        ClearAllLayer(i, j);
                    }
                }
            }
        }
        
        public void MovingWild1()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildTotalPos = extraState.GetMovingWildIds()[0];
            var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];

            var listWildPos = listWildTotalPos.Items;
            for (int q = 0; q < listWildPos.Count; q++)
            {
                var startPos = wheel.GetRoll((int) listWildPos[q].OldX).GetVisibleContainerPosition((int) listWildPos[q].OldY);
                var endPos = wheel.GetRoll((int) listWildPos[q].X).GetVisibleContainerPosition((int) listWildPos[q].Y);
                if (listWildPos[q].Moving)
                {
                    for (int i = 0; i < _newMovingWild1.Count; i++)
                    {
                        if (_newMovingWild1[i].transform.position == startPos)
                        {
                             _newMovingWild1[i].transform.DOMove(endPos, 1.75f);
                        }
                    }
                }
            }
        }

         public void MovingWild2()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var listWildTotalPos = extraState.GetMovingWildIds()[1];
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[1];

             var listWildPos = listWildTotalPos.Items;
             for (int q = 0; q < listWildPos.Count; q++)
             {
                 var startPos = wheel.GetRoll((int) listWildPos[q].OldX).GetVisibleContainerPosition((int) listWildPos[q].OldY);
                 var endPos = wheel.GetRoll((int) listWildPos[q].X)
                     .GetVisibleContainerPosition((int) listWildPos[q].Y);
                 if (listWildPos[q].Moving)
                 {
                     for (int i = 0; i < _newMovingWild2.Count; i++)
                     {
                         if (_newMovingWild2[i].transform.position == startPos)
                         {
                             _newMovingWild2[i].transform.DOMove(endPos, 1.75f);
                         }
                     }
                 }
             }
         }

         public void MovingWild3()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var listWildTotalPos = extraState.GetMovingWildIds()[2];
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[2];

             var listWildPos = listWildTotalPos.Items;
             for (int q = 0; q < listWildPos.Count; q++)
             {
                 var startPos = wheel.GetRoll((int) listWildPos[q].OldX).GetVisibleContainerPosition((int) listWildPos[q].OldY);
                 var endPos = wheel.GetRoll((int) listWildPos[q].X)
                     .GetVisibleContainerPosition((int) listWildPos[q].Y);
                 if (listWildPos[q].Moving)
                 {
                     for (int i = 0; i < _newMovingWild3.Count; i++)
                     {
                         if (_newMovingWild3[i].transform.position == startPos)
                         {
                             _newMovingWild3[i].transform.DOMove(endPos, 1.75f);
                         }
                     }
                 }
             }
         }
         
         public void ShowMapGame1WildElement()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
             var listWildTotalPos = extraState.GetMovingWildIds();
             var listWildPos = listWildTotalPos[0].Items;
             if (listWildPos.Count > 0)
             {
                 for (int i = 0; i < listWildPos.Count; i++)
                 {
                     var roll = wheel.GetRoll((int) listWildPos[i].X);
                     var container = roll.GetVisibleContainer((int) listWildPos[i].Y);
                     var endPos = wheel.GetRoll((int) listWildPos[i].X)
                         .GetVisibleContainerPosition((int) listWildPos[i].Y);
                     //更换wild
                     LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                         (int) listWildPos[i].X, (int) listWildPos[i].Y, true);
                     var newWild = GetElement((int) listWildPos[i].X, (int) listWildPos[i].Y);
                     newWild.transform.position = endPos;
                     newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                     _newMovingWild1.Add(newWild);
                 }
             }
         }
         
         public void ShowMapGame2WildElement()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[1];
             var listWildTotalPos = extraState.GetMovingWildIds();
             var listWildPos = listWildTotalPos[1].Items;
             if (listWildPos.Count > 0)
             {
                 for (int i = 0; i < listWildPos.Count; i++)
                 {
                     var roll = wheel.GetRoll((int) listWildPos[i].X);
                     var container = roll.GetVisibleContainer((int) listWildPos[i].Y);
                     var endPos = wheel.GetRoll((int) listWildPos[i].X)
                         .GetVisibleContainerPosition((int) listWildPos[i].Y);
                     //更换wild
                     LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                         (int) listWildPos[i].X, (int) listWildPos[i].Y, true);
                     var newWild = GetElement((int) listWildPos[i].X, (int) listWildPos[i].Y);
                     newWild.transform.position = endPos;
                     newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                     _newMovingWild2.Add(newWild);
                 }
             }
         }

         public void ShowMapGame3WildElement()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[2];
             var listWildTotalPos = extraState.GetMovingWildIds();
             var listWildPos = listWildTotalPos[2].Items;
             if (listWildPos.Count > 0)
             {
                 for (int i = 0; i < listWildPos.Count; i++)
                 {
                     var roll = wheel.GetRoll((int) listWildPos[i].X);
                     var container = roll.GetVisibleContainer((int) listWildPos[i].Y);
                     var endPos = wheel.GetRoll((int) listWildPos[i].X)
                         .GetVisibleContainerPosition((int) listWildPos[i].Y);
                     //更换wild
                     LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                         (int) listWildPos[i].X, (int) listWildPos[i].Y, true);
                     var newWild = GetElement((int) listWildPos[i].X, (int) listWildPos[i].Y);
                     newWild.transform.position = endPos;
                     newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                     _newMovingWild3.Add(newWild);
                 }
             }
         }
         
         public void RecyleMapGameM0vingOldWild1()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetMovingWildIds()[0].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        if (wildPos.Moving)
                        {
                             ClearAllLayer((int) wildPos.OldX, (int) wildPos.OldY);
                        }
                    }
                }
            }
        }

        public void RecyleMapGameMovingOldWild2()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetMovingWildIds()[1].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        if (wildPos.Moving)
                        {
                             ClearAllLayer((int) wildPos.OldX, (int) wildPos.OldY);
                        }
                    }
                }
            }
        }

        public void RecyleMapGameMovingOldWild3()
        {
            var extraState = context.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetMovingWildIds()[2].Items;
            for (var i = 0; i < listWildPos.Count; i++)
            {
                if (listWildPos.Count > 0)
                {
                    foreach (var wildPos in listWildPos)
                    {
                        if (wildPos.Moving)
                        {
                            ClearAllLayer((int) wildPos.OldX, (int) wildPos.OldY);
                        }
                    }
                }
            }
        }
        
         public void ShowMapGame1MovingWildElement()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
             var listWildTotalPos = extraState.GetMovingWildIds();
             var listWildPos = listWildTotalPos[0].Items;
             if (listWildPos.Count > 0)
             {
                 for (int i = 0; i < listWildPos.Count; i++)
                 {
                     if (listWildPos[i].Moving)
                     {
                         var roll = wheel.GetRoll((int) listWildPos[i].X);
                         var container = roll.GetVisibleContainer((int) listWildPos[i].Y);
                         var endPos = wheel.GetRoll((int) listWildPos[i].X)
                             .GetVisibleContainerPosition((int) listWildPos[i].Y);
                         //更换wild
                         LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                             (int) listWildPos[i].X, (int) listWildPos[i].Y, true);
                         var newWild = GetElement((int) listWildPos[i].X, (int) listWildPos[i].Y);
                         newWild.UpdateMaskInteraction(SpriteMaskInteraction.None);
                         newWild.transform.position = endPos;
                         newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                     }
                 }
             }
         }
         
         public void ShowMapGame2MovngWildElement()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[1];
             var listWildTotalPos = extraState.GetMovingWildIds();
             var listWildPos = listWildTotalPos[1].Items;
             if (listWildPos.Count > 0)
             {
                 for (int i = 0; i < listWildPos.Count; i++)
                 {
                     if (listWildPos[i].Moving)
                     {
                         var roll = wheel.GetRoll((int) listWildPos[i].X);
                         var container = roll.GetVisibleContainer((int) listWildPos[i].Y);
                         var endPos = wheel.GetRoll((int) listWildPos[i].X)
                             .GetVisibleContainerPosition((int) listWildPos[i].Y);
                         //更换wild
                         LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                             (int) listWildPos[i].X, (int) listWildPos[i].Y, true);
                         var newWild = GetElement((int) listWildPos[i].X, (int) listWildPos[i].Y);
                         newWild.UpdateMaskInteraction(SpriteMaskInteraction.None);
                         newWild.transform.position = endPos;
                         newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                     }
                 }
             }
         }

         public void ShowMapGame3MovingWildElement()
         {
             var extraState = context.state.Get<ExtraState11029>();
             var wheel = context.state.Get<WheelsActiveState11029>().GetRunningWheel()[2];
             var listWildTotalPos = extraState.GetMovingWildIds();
             var listWildPos = listWildTotalPos[2].Items;
             if (listWildPos.Count > 0)
             {
                 for (int i = 0; i < listWildPos.Count; i++)
                 {
                     if (listWildPos[i].Moving)
                     {
                         var roll = wheel.GetRoll((int) listWildPos[i].X);
                         var container = roll.GetVisibleContainer((int) listWildPos[i].Y);
                         var endPos = wheel.GetRoll((int) listWildPos[i].X)
                             .GetVisibleContainerPosition((int) listWildPos[i].Y);
                         //更换wild
                         LockElement(context.machineConfig.GetSequenceElement(Constant11029.WildElementId, context),
                             (int) listWildPos[i].X, (int) listWildPos[i].Y, true);
                         var newWild = GetElement((int) listWildPos[i].X, (int) listWildPos[i].Y);
                         newWild.UpdateMaskInteraction(SpriteMaskInteraction.None);
                         newWild.transform.position = endPos;
                         newWild.transform.localScale = new Vector3(0.81f, 0.81f, 0.81f);
                     }
                 }
             }
         }
    }
}