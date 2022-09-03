

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Debug = UnityEngine.Debug;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

namespace GameModule
{
    public class WheelsActiveState11020 : WheelsActiveState
    {
        public bool baseWheelFromFree = false;

        SSpin currentSpinResult;

        LockedFramesView11020 lockedFramesView;

        public ulong CurTotalBet;
        public List<uint> ListNewIds; 

        public WheelsActiveState11020(MachineState machineState)
            : base(machineState)
        {
            ListNewIds = new List<uint>();
            spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;

            Constant11020.firstInitWheel = true;
        }

        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            baseWheelFromFree = false;

            Constant11020.firstInitWheel = true;

            base.UpdateStateOnRoomSetUp(gameEnterInfo);

            var wheel = GetRunningWheel()[0];
            
            lockedFramesView = machineState.machineContext.view.Get<LockedFramesView11020>();

            if (lockedFramesView != null)
            {
                lockedFramesView.StartWheel(wheel, false);
            }

            machineState.machineContext.view.Get<WheelView11020>().StartGame(wheel);
        }

        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            var freeSpinState = machineState.Get<FreeSpinState>();

            if (!freeSpinState.IsTriggerFreeSpin && !freeSpinState.IsOver)
            {
                bool isTriggered = machineState.Get<SuperBonusInfoState11020>().IsTriggered();

                if (isTriggered)
                {
                    UpdateRunningWheel(new List<string>() { Constant11020.superBonusWheelName }, true);
                }
                else
                {
                    UpdateRunningWheel(new List<string>() { Constant11020.freeWheelName }, true);
                }
                ShowRollsMasks(GetRunningWheel()[0]);
            }
            else
            {
                UpdateRunningWheel(new List<string>() { Constant11020.baseWheelName }, true);

                machineState.machineContext.view.Get<SuperBonusInfoView11020>().StartWheel();
            }

            var wheel = GetRunningWheel()[0];

            if (lockedFramesView != null && !baseWheelFromFree)
            {
                lockedFramesView.StartWheel(wheel, false);
            }

            if (Constant11020.firstInitWheel)
            {
                ForceWheelRandomElement(wheel);
            }

            machineState.machineContext.view.Get<WheelView11020>().StartGame(wheel);

            baseWheelFromFree = false;
        }

        public void UpdateRunningWheelStateFreeSpin()
        {
            bool isTriggered = machineState.Get<SuperBonusInfoState11020>().IsTriggered();

            if (isTriggered)
            {
                UpdateRunningWheel(new List<string>() { Constant11020.superBonusWheelName }, true);
            }
            else
            {
                UpdateRunningWheel(new List<string>() { Constant11020.freeWheelName }, true);
            }

            //ShowRollsMasks(GetRunningWheel()[0]);

            var wheel = GetRunningWheel()[0];

            if (lockedFramesView != null)
            {
                lockedFramesView.StartWheel(wheel, false);
            }

            ForceWheelRandomElement(wheel);

            machineState.machineContext.view.Get<WheelView11020>().StartGame(wheel);
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            currentSpinResult = spinResult;

            base.UpdateStateOnReceiveSpinResult(spinResult);
        }

        public override string GetReelNameForWheel(Wheel wheel)
        {
            return Constant11020.GetReelName(wheel.wheelName);
        }

        public Panel GetPanel()
        {
            return currentSpinResult != null ? currentSpinResult.GameResult.Panels[0] : null;
        }

        public void ForceWheelRandomElement(Wheel wheel)
        {
            // Debug.Log("start ForceWheelRandomElement ======");
            var elementConfigSet = machineState.machineConfig.GetElementConfigSet();

            Roll roll = null;
            var i = 0;

            while (i < wheel.rollCount)
            {
                roll = wheel.GetRoll(i);
                if (roll != null)
                {
                    for (var k = 0; k < 10; ++k)
                    {
                        var elementContainer = roll.GetContainer(k);

                        // Debug.Log($"ForceWheelRandomElement: {i}, {k}");

                        if (elementContainer == null)
                        {
                            break;
                        }

                        if (Constant11020.lionElement != elementContainer.sequenceElement.config.id)
                        {
                            continue;
                        }

                        var eid = UnityEngine.Random.Range(1, 10);

                        elementContainer.UpdateElement(
                            new SequenceElement(elementConfigSet.GetElementConfig((uint)eid), 
                            machineState.machineContext));
                        elementContainer.UpdateElementMaskInteraction(false);

                        // Debug.Log($"ForceWheelRandomElement fireball: {i}, {k}");
                    }

                    UpdateHighElementSortingOrder(wheel, i);
                }
                else
                {
                    break;
                }
                ++i;
            }

            // Debug.Log("ForceWheelRandomElement ======");
        }

        public void RemoveWheelWildElement(Wheel wheel)
        {
            var elementConfigSet = machineState.machineConfig.GetElementConfigSet();

            Roll roll = null;
            var i = 0;

            while (i < wheel.rollCount)
            {
                roll = wheel.GetRoll(i);
                if (roll != null)
                {
                    for (var k = 0; k < 10; ++k)
                    {
                        var elementContainer = roll.GetContainer(k);

                        if (elementContainer == null)
                        {
                            break;
                        }

                        var id = elementContainer.sequenceElement.config.id;
                        if (Constant11020.wildElement != id || id == Constant11020.buleWildElement)
                        {
                            continue;
                        }

                        var eid = UnityEngine.Random.Range(1, 10);

                        elementContainer.UpdateElement(
                            new SequenceElement(elementConfigSet.GetElementConfig((uint)eid), 
                            machineState.machineContext));
                        elementContainer.UpdateElementMaskInteraction(false);
                    }

                    UpdateHighElementSortingOrder(wheel, i);
                }
                else
                {
                    break;
                }
                ++i;
            }

            // Debug.Log("ForceWheelRandomElement ======");
        }

        public void MakeFireBallElementsAnimation(string animationName, int rollId, bool sound)
        {
            var wheel = GetRunningWheel()[0];
            if (wheel != null)
            {
                Roll roll = null;
                var i = rollId < 0 ? 0 : rollId;

                while (i < wheel.rollCount)
                {
                    roll = wheel.GetRoll(i);
                    if (roll != null)
                    {
                        for (var k = 0; k < 10; ++k)
                        {
                            var row = k;
                            var elementContainer = roll.GetContainer(k);
                            if (elementContainer == null)
                            {
                                break;
                            }

                            var elem = elementContainer.GetElement();
                            if (elem != null && elem.isStaticElement 
                                && elem.sequenceElement.config.id == Constant11020.lionElement)
                            {
                                var animator = elem.transform.GetComponent<Animator>();
                                XUtility.PlayAnimation(animator, animationName, () =>
                                {
                                    var containerY = elementContainer.transform.position.y;
                                    var needShift = (rollId == 1 || rollId == 3) && containerY<-2;
                                    needShift = needShift || (rollId == 0 || rollId == 2 || rollId == 4) && containerY<-3;
                                    if (animator && needShift)
                                    {
                                        elem.transform.gameObject.SetActive(false);
                                    }
                                });
                                //OTDO
                                // if (sound)
                                // {
                                //     AudioUtil.Instance.PlayAudioFx("Fire_Ball_Stop");
                                // }
                            }
                        }

                        if (rollId >= 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    ++i;
                }
            }
        }

        public void MakeFireBallsDisappear()
        {
            var wheel = GetRunningWheel()[0];
            if (wheel == null)
            {
                return;
            }

            Roll roll = null;
            var i = 0;

            while (i < 5)
            {
                roll = wheel.GetRoll(i);
                if (roll != null)
                {
                    var num = (i == 1 || i ==3) ? 5 : 4;
                    for (var k = 0; k < num; ++k)
                    {
                        var elementContainer = roll.GetVisibleContainer(k);
                        if (elementContainer == null)
                        {
                            break;
                        }

                        var elem = elementContainer.GetElement();
                        if (elem != null && elem.sequenceElement.config.id == Constant11020.lionElement)
                        {
                            AddDisappearFireBall(elementContainer);
                        }
                    }
                }
                ++i;
            }
        }

        private async void AddDisappearFireBall(ElementContainer elementContainer)
        {
            var ball = machineState.machineContext.assetProvider.InstantiateGameObject("Static_fireball", true);

            ball.transform.SetParent(elementContainer.transform.parent.parent);
            ball.transform.position = elementContainer.transform.position;
            ball.transform.localScale = Vector3.one;
            ball.transform.DOKill();

            XUtility.PlayAnimation(ball.GetComponent<Animator>(), "Idle");

            ball.transform.DOScale(Vector3.zero, 0.3f).OnComplete(
                () =>
                {
                    ball.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    machineState.machineContext.assetProvider.RecycleGameObject("Static_fireball", ball);
                    // ball.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
            );

            var elementConfigSet = machineState.machineConfig.GetElementConfigSet();
            var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(0), machineState.machineContext);
            elementContainer.UpdateElement(seqElement, true);
        }

        public void ResetElementsExtraSortOder()
        {
            var wheel = GetRunningWheel()[0];
            if (wheel == null)
            {
                return;
            }

            Roll roll = null;
            var i = 0;

            while (i < 5)
            {
                roll = wheel.GetRoll(i);
                if (roll != null)
                {
                    // var num = (i == 1 || i ==3) ? 5 : 4;
                    for (var k = 0; k < 20; ++k)
                    {
                        var elementContainer = roll.GetContainer(k);
                        if (elementContainer == null)
                        {
                            break;
                        }

                        elementContainer.UpdateExtraSortingOrder(0);

                        var id = elementContainer.sequenceElement.config.id;
                        if (id == Constant11020.wildElement || id == Constant11020.buleWildElement)
                        {
                            elementContainer.UpdateAnimationToStatic();
                        }
                        elementContainer.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.VisibleInsideMask, true);
                    }
                }
                ++i;
            }
        }

        public void ShowRollsMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }

            for (var i = 0; i < 5; ++i)
            {
                var o = roll.Find("BlackMask" + i).gameObject;
                o.SetActive(true);

                var render = o.GetComponent<SpriteRenderer>();
                DOTween.Kill(render);
                render.DOFade(0f, 0f);
                render.DOFade(0.6f, 1.0f);
            }
        }

        public void FadeOutRollMask(Wheel wheel, int rollIndex)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }

            var render = roll.Find("BlackMask" + rollIndex).gameObject.GetComponent<SpriteRenderer>();
            DOTween.Kill(render);
            render.DOFade(0, 0.3f);
        }

        public void UpdateWheelHighElementSortingOrder()
        {
            var wheel = GetRunningWheel()[0];
            if (wheel == null)
            {
                return;
            }

            var i = 0;

            while (i < 5)
            {
                UpdateHighElementSortingOrder(wheel, i);

                ++i;
            }
        }

        public void UpdateHighElementSortingOrder(Wheel wheel, int rollIndex)
        {
            var roll = wheel.GetRoll(rollIndex);
            if (roll == null)
            {
                return;
            }

            var count = (rollIndex == 1 || rollIndex == 3) ? 5 : 4;

            uint elementId = 0;
            
            for (var i = 0; i < count; ++i)
            {
                var container = roll.GetVisibleContainer(i);

                elementId = container.GetElement().sequenceElement.config.id;

                if (
                    // elementId == Constant11020.wildElement || 
                    // elementId == Constant11020.buleWildElement || 
                    elementId == Constant11020.bonusElement ||
                    elementId == Constant11020.bonusWildElement
                    || elementId == Constant11020.lionElement)
                {
                    container.UpdateExtraSortingOrder(1850-container.GetBaseSortOrder()+i);
                   
                    container.UpdateElementMaskInteraction(true);
                } 
            }
        }
    }
}
