using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering;


namespace GameModule
{
    public class LockElementLayer11031 : LockElementLayer
    {
        Dictionary<Element, int> newJackPotLockElementsList = new Dictionary<Element, int>();
        public List<Element> lockedElementList = new List<Element>();
        public List<Element> lockedYellowElementList = new List<Element>();
        ElementConfigSet elementConfigSet = null;

        public LockElementLayer11031(Transform transform)
            : base(transform)
        {
            // _extraState11031 = context.state.Get<ExtraState11031>();
        }

        public void LockLinkElements(int id, uint symbolId, bool isStatic, bool idle)
        {
            var x = id / 3;
            var y = id % 3;
            var superRespinTrigger = context.state.Get<ExtraState11031>().FromOneLinkModeToFourLinkMode() &&
                                     context.state.Get<WheelsActiveState11031>().GetRunningWheel().Count == 1;
            if (superRespinTrigger)
            {
                var wheel = context.state.Get<WheelsActiveState11031>().GetRunningWheel()[0];
                for (int i = 0; i < wheel.rollCount; i++)
                {
                    for (int j = 0; j < wheel.GetRoll(i).rowCount; j++)
                    {
                        var elementContainer = wheel.GetRoll(i).GetVisibleContainer(j);
                        if (Constant11031.ListYellowChilli.Contains(elementContainer.sequenceElement.config.id))
                        {
                            if (i == x && j == y)
                            {
                                symbolId = elementContainer.sequenceElement.config.id;
                            }
                        }
                    }
                }
            }

            bool result = LockElement(context.machineConfig.GetSequenceElement(symbolId, context),
                x, y, true);

            if (!result)
            {
                return;
            }

            var endPos = wheel.GetRoll(x).GetVisibleContainerPosition(y);

            var lockElement = GetElement(x, y);

            UpdateSortingGroup(lockElement, id);

            var scale = wheel.wheelName == "WheeRespinGame" ? 1.0f : 0.5f;
            lockElement.transform.localScale = new Vector3(scale, scale, scale);


            lockElement.transform.position = endPos;

            //避免播放动画的时候穿帮
            if (!isStatic)
            {
                wheel.GetRoll(x).GetVisibleContainer(y).transform.gameObject.SetActive(false);
                context.WaitSeconds(0.6f,
                    () =>
                    {
                        wheel.GetRoll(x).GetVisibleContainer(y).transform.gameObject.SetActive(true);
                        elementConfigSet = context.state.machineConfig.elementConfigSet;
                        var length = Constant11031.ListLowLevelAllElementIds.Count;
                        var index = Random.Range(0, length);
                        var sequenceElement = new SequenceElement(
                            elementConfigSet.GetElementConfig(Constant11031.ListLowLevelAllElementIds[index]),
                            context);
                        wheel.GetRoll(x).GetVisibleContainer(y).UpdateElement(sequenceElement);
                    });
            }

            if (idle)
            {
                lockElement.transform.gameObject.GetComponent<Animator>().Play(isStatic ? "Idle_bg" : "JumpBG");
            }
            else
            {
                lockElement.transform.gameObject.GetComponent<Animator>().Play(isStatic ? "DefaultBG" : "JumpBG");
            }

            lockedElementList.Add(lockElement);
            if (Constant11031.ListYellowChilli.Contains(symbolId))
            {
                lockedYellowElementList.Add(lockElement);
            }
        }

        public void Recycle()
        {
            for (int i = 0; i < lockedElementList.Count; i++)
            {
                lockedElementList[i].DoRecycle();
            }
        }

        public void PlayWin()
        {
            for (int i = 0; i < lockedElementList.Count; i++)
            {
                lockedElementList[i].transform.gameObject.GetComponent<Animator>().Play("Win");
            }
        }

        public void PlayYellowChilliTrigger()
        {
            if (lockedYellowElementList.Count > 0)
            {
                for (int i = 0; i < lockedYellowElementList.Count; i++)
                {
                    lockedYellowElementList[i].transform.gameObject.GetComponent<Animator>().Play("TriggerBG");
                }
            }
        }

        public void ClearLockedYellowElementList()
        {
            lockedYellowElementList.Clear();
        }
        
        public void ClearLockedElementList()
        {
            lockedElementList.Clear();
        }

        public void PLayIdleBg(uint positionId)
        {
            var x = positionId / 3;
            var y = positionId % 3;
            var element = GetElement((int) x, (int) y);

            if (element != null)
            {
                element.transform.gameObject.GetComponent<Animator>().Play("Idle_bg");
            }
        }

        public void PLayTriggerBg(uint positionId)
        {
            var x = positionId / 3;
            var y = positionId % 3;
            var element = GetElement((int) x, (int) y);

            if (element != null)
            {
                element.transform.gameObject.GetComponent<Animator>().Play("TriggerBG");
            }
        }

        public void UpdateSortingGroup(Element element, int sortId, int textId = -1)
        {
            if (element.transform.Find("Element"))
            {
                var elementTransform = element.transform.Find("Element");
                var elementSortingGroup = elementTransform.GetComponent<SortingGroup>();
                if (elementSortingGroup == null)
                {
                    elementSortingGroup =elementTransform.gameObject.AddComponent<SortingGroup>();
                }
                
                elementSortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                elementSortingGroup.sortingOrder = 200 + sortId;

                if (element.transform.Find("Text"))
                {
                    var textTransform = element.transform.Find("Text");
                    var textSortingGroup = textTransform.GetComponent<SortingGroup>();
                    if (textSortingGroup == null)
                    {
                        textSortingGroup = textTransform.gameObject.AddComponent<SortingGroup>();
                    }

                    textSortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                    
                    if(textId < 0)
                        textSortingGroup.sortingOrder = 220 + sortId;
                    else
                        textSortingGroup.sortingOrder = 220 + textId;
                }
            }
            else
            {
                if (!element.transform.GetComponent<SortingGroup>())
                {
                    var sortingGroup = element.transform.gameObject.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                    sortingGroup.sortingOrder = 200 + sortId;
                }
                else
                {
                    var sortingGroup = element.transform.gameObject.GetComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
                    sortingGroup.sortingOrder = 200 + sortId;
                }
            }

        }

        public async Task FlyToJackpot(uint positionId, uint jackpotId)
        {
            var x = positionId / 3;
            var y = positionId % 3;
            var element = GetElement((int) x, (int) y);

            if (element != null)
            {
                //获取jackpotid
                //   element.transform.gameObject.GetComponent<Animator>().Play("BeforeFly");
                //   await context.WaitSeconds(1.5f);


                //获取jackpotid
                UpdateSortingGroup(element, (int)positionId);
               
                element.transform.gameObject.GetComponent<Animator>().Play("Idle_bg");
                var addressJackPot = Constant11031.JackpotsGameObjectAddress[jackpotId - 1];
                var objFly = context.assetProvider.InstantiateGameObject(addressJackPot, true);
                objFly.transform.parent = context.transform;
                objFly.transform.position = element.transform.position;
                objFly.SetActive(true);

                if (!objFly.GetComponent<SortingGroup>())
                {
                    var sortingGroup = objFly.gameObject.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
                    sortingGroup.sortingOrder = 999;
                }

                var startPos = objFly.transform.position;
                AudioUtil.Instance.PlayAudioFx("Jackpot_Alarm2");
                var endPos = context.view.Get<JackpotPanel11031>().GetIntegralPos((int) jackpotId);
                await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f, Ease.Linear);
                await context.view.Get<JackpotPanel11031>().PlayFlyToJackPot(jackpotId);

                context.assetProvider.RecycleGameObject(addressJackPot, objFly);
            }
            else
            {
                XDebug.Log("NoElement Lock OnCurrent Position");
            }
        }

        public async Task PlayBeforeFlyToJackPot(
            List<ChilliFrenzyGameResultExtraInfo.Types.LinkData.Types.LinkItem> linkItems)
        {
            for (var i = 0; i < linkItems.Count; i++)
            {
                var x = linkItems[i].PositionId / 3;
                var y = linkItems[i].PositionId % 3;
                var element = GetElement((int) x, (int) y);

                if (element != null)
                {
                    UpdateSortingGroup(element, (int)linkItems[i].PositionId, linkItems.Count - i);
                    element.transform.gameObject.GetComponent<Animator>().Play("BeforeFly");
                }
            }

            if (linkItems.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFx("Jackpot_Alarm1");
                await context.WaitSeconds(1.5f);
            }
        }

        // public async Task FlyToJackPot()
        // {
        //     var extraState11031 = context.state.Get<ExtraState11031>();
        //     
        //     foreach (var child in newJackPotLockElementsList)
        //     {
        //         //获取jackpotid
        //         var jackPotId = child.Value;
        //         child.Key.transform.gameObject.GetComponent<Animator>().Play("BeforeFly");
        //         await context.WaitSeconds(1.5f);
        //         child.Key.transform.gameObject.GetComponent<Animator>().Play("Idle_bg");
        //         var addressJackPot = Constant11031.JackpotsGameObjectAddress[jackPotId - 1];
        //         var objFly = context.assetProvider.InstantiateGameObject(addressJackPot, true);
        //         objFly.transform.parent = context.transform;
        //         objFly.transform.position = child.Key.transform.position;
        //         objFly.SetActive(true);
        //         
        //         if (!objFly.GetComponent<SortingGroup>())
        //         {
        //             var sortingGroup = objFly.gameObject.AddComponent<SortingGroup>();
        //             sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
        //             sortingGroup.sortingOrder = 500;
        //         }
        //         
        //         var startPos = objFly.transform.position;
        //         var endPos = context.view.Get<JackpotPanel11031>().GetIntegralPos((int) jackPotId);
        //         await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f, Ease.Linear);
        //     //    await context.view.Get<JackpotPanel11031>().PlayFlyToJackPot(jackPotId);
        //         
        //         if (extraState11031.LinkLockElementIdsList.ContainsKey(wheel))
        //             context.assetProvider.RecycleGameObject(addressJackPot, objFly);
        //     }
        // }

        public void ClearNewJackPotLockElementsList()
        {
            newJackPotLockElementsList.Clear();
        }
    }
}