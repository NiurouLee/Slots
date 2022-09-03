using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LockView11013 : TransformHolder
    {
        protected LockPrefabLayer lockElementLayer;

        public LockView11013(Transform inTransform) : base(inTransform)
        {
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            lockElementLayer = new LockPrefabLayer(context.view.Get<Wheel>(), context);
        }


        public async Task PlayElementAnim(int row, int column, string stateName)
        {
            var objElement = lockElementLayer.GetElement(row, column);
            if (objElement != null)
            {
                Animator animator = objElement.GetComponent<Animator>();
                await XUtility.PlayAnimationAsync(animator, stateName, context, true);
            }
        }


        protected async Task WildToElement(GameObject objLockWild, int x, int y)
        {
            if (objLockWild != null)
            {
                Animator animatorLockWild = null;
                animatorLockWild = objLockWild.GetComponent<Animator>();
                await XUtility.PlayAnimationAsync(animatorLockWild, "Bonus", context, true);
                lockElementLayer.Remove(y, x);
            }
        }

        public async Task TriggerFreeCount()
        {
            var freeSpinState = context.state.Get<FreeSpinState>();

            if (freeSpinState.IsTriggerFreeSpin || freeSpinState.NewCount > 0)
            {
                // if (freeSpinState.IsTriggerFreeSpin)
                // {
                AudioUtil.Instance.StopMusic();
                // }

                Wheel wheel = context.state.Get<WheelsActiveState11013>().GetRunningWheel()[0];


                int XCount = wheel.rollCount;
                int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

                var extraState = context.state.Get<ExtraState11013>();
                var listFreeSpinCount = extraState.GetFreeSpinCount();

                List<Task> listTask = new List<Task>();
                if (!freeSpinState.IsTriggerFreeSpin)
                {
                    for (int x = 0; x < XCount; x++)
                    {
                        for (int y = 0; y < YCount; y++)
                        {
                            var container = wheel.GetRoll(x).GetVisibleContainer(y);
                            if (container.sequenceElement.config.id == Constant11013.GoldenScatterElement ||
                                container.sequenceElement.config.id == Constant11013.PinkScatterElement)
                            {
                                int posId = x * YCount + y;
                                uint freeSpinCount = listFreeSpinCount[posId];
                                if (freeSpinCount > 0)
                                {
                                    //如果是金色，并且原来的地方有锁定，就先从wild变成金色贝壳
                                    if (container.sequenceElement.config.id == Constant11013.GoldenScatterElement)
                                    {
                                        var objLockWild = lockElementLayer.GetElement(y, x);
                                        listTask.Add(WildToElement(objLockWild, x, y));
                                    }
                                }
                            }
                        }
                    }
                }


                if (listTask.Count > 0)
                {
                    AudioUtil.Instance.PlayAudioFx("B02_Close");
                    await Task.WhenAll(listTask);
                }


                for (int x = 0; x < XCount; x++)
                {
                    for (int y = 0; y < YCount; y++)
                    {
                        var container = wheel.GetRoll(x).GetVisibleContainer(y);
                        if (container.sequenceElement.config.id == Constant11013.GoldenScatterElement ||
                            container.sequenceElement.config.id == Constant11013.PinkScatterElement)
                        {
                            int posId = x * YCount + y;
                            uint freeSpinCount = listFreeSpinCount[posId];
                            if (freeSpinCount > 0)
                            {
                                container.PlayElementAnimation("Spin");

                                var tranSpin = container.GetElement().transform
                                    .Find("OpenState/CountGroup/FreeSpinSprite");

                                var tranSpins = container.GetElement().transform
                                    .Find("OpenState/CountGroup/FreeSpinsSprite");


                                if (freeSpinCount == 1)
                                {
                                    tranSpin.gameObject.SetActive(true);
                                    tranSpins.gameObject.SetActive(false);
                                }
                                else
                                {
                                    tranSpin.gameObject.SetActive(false);
                                    tranSpins.gameObject.SetActive(true);
                                }


                                container.ShiftSortOrder(true);
                                TextMesh txtCount = container.GetElement().transform
                                    .Find("OpenState/CountGroup/CountText").GetComponent<TextMesh>();
                                txtCount.text = freeSpinCount.ToString();
                                AudioUtil.Instance.PlayAudioFx("B01_Open");
                                await context.WaitSeconds(0.833f);
                            }
                        }
                    }
                }
            }
        }


        public void ScatterToWildNoAnim()
        {
            var extraState = context.state.Get<ExtraState11013>();
            var listGolden = extraState.GetGoldenPosList();


            Wheel wheel = context.state.Get<WheelsActiveState11013>().GetRunningWheel()[0];
            int xCount = wheel.rollCount;
            int yCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

            for (int i = 0; i < listGolden.Count; i++)
            {
                int pos = (int) listGolden[i];
                int x = pos / yCount;
                int y = pos % yCount;

                var obj = lockElementLayer.ReplaceOrAttachToElement("W02Frame", y, x);

                SortingGroup sortingGroup = obj.GetComponent<SortingGroup>();
                sortingGroup.sortingOrder = 9000 + i;
            }
        }


        public void ClearWild()
        {
            lockElementLayer.Clear();
        }


        public async Task CloseAllPink()
        {
            var freeSpinState = context.state.Get<FreeSpinState>();

            if (freeSpinState.IsTriggerFreeSpin || freeSpinState.NewCount > 0)
            {
                Wheel wheel = context.state.Get<WheelsActiveState11013>().GetRunningWheel()[0];


                int XCount = wheel.rollCount;
                int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

                List<Task> listTask = new List<Task>();
                for (int x = 0; x < XCount; x++)
                {
                    for (int y = 0; y < YCount; y++)
                    {
                        var container = wheel.GetRoll(x).GetVisibleContainer(y);
                        if (container.sequenceElement.config.id == Constant11013.PinkScatterElement)
                        {
                            //listTask.Add(container.PlayElementAnimationAsync("Idle"));
                            container.ShiftSortOrder(false);
                            container.UpdateAnimationToStatic();
                        }
                    }
                }

                if (listTask.Count > 0)
                {
                    await Task.WhenAll(listTask);
                }
            }
        }

        public async Task ScatterToWild()
        {
            CloseAllPink();

            var extraState = context.state.Get<ExtraState11013>();
            var listGolden = extraState.GetGoldenPosList();
            Wheel wheel = context.state.Get<WheelsActiveState11013>().GetRunningWheel()[0];
            int xCount = wheel.rollCount;
            int yCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

            List<Task> listTask = new List<Task>();
            for (int i = 0; i < listGolden.Count; i++)
            {
                int pos = (int) listGolden[i];
                int x = pos / yCount;
                int y = pos % yCount;
                var container = wheel.GetRoll(x).GetVisibleContainer(y);

                listTask.Add(PlayChangeWild(container, y, x, i));
            }

            if (listTask.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFx("B02_Wild02");
                await Task.WhenAll(listTask);
            }
        }


        protected async Task PlayChangeWild(ElementContainer container, int row, int column, int index)
        {
            container.ShiftSortOrder(true);
            await container.PlayElementAnimationAsync("Wild");
            var obj = lockElementLayer.ReplaceOrAttachToElement("W02Frame", row, column);
            SortingGroup sortingGroup = obj.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 9000 + index;
        }
    }
}