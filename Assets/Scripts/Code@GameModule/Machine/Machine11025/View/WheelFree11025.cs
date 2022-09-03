using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelFree11025:Wheel11025
    {
        [ComponentBinder("CollectLightGroup")] 
        public Transform collectLightGroup;

        public List<Transform> collectLightList;
        public WheelFree11025(Transform transform) : base(transform)
        {
            collectLightList = new List<Transform>();
            ComponentBinder.BindingComponent(this,transform);
        }

        public Transform GetCollectLight(int index)
        {
            return collectLightList[index];
        }

        public void SetCollectLightNormal(int index)
        {
            collectLightList[index] = collectLightGroup.Find("YellowAnimation" + index);
        }

        public void SetCollectLightShort(int index)
        {
            collectLightList[index] = collectLightGroup.Find("YellowAnimation" + index+"_short");
        }

        public override void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState,
            string inWheelElementSortingLayerName = "Element")
        {
            base.BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(inWheelState, inWheelElementSortingLayerName);
            for (var i = 0; i <= rollCount; i++)
            {
                collectLightList.Add(collectLightGroup.Find("YellowAnimation"+i));
                GetCollectLight(i).gameObject.SetActive(false);
            }
        }

        public void ResetFreeRollMaskOrder()
        {
            rollMaskGroup.GetComponent<SortingGroup>().sortingOrder = 100;
        }
        public async Task PrepareCollectAll(List<int> rollCountList)
        {
            rollMaskGroup.GetComponent<SortingGroup>().sortingOrder = 500;
            for (var i = 0; i < rollCount; i++)
            {
                SetRollMaskColor(i, RollMaskOpacityLevel11025.Level3,0.2f);
                CleanFrameRoll(i);
            }

            var lightShowState = new List<bool>() {false, false, false, false, false, false};
            if (rollCountList[2] == 0)
            {
                SetCollectLightShort(2);
            }
            else
            {
                SetCollectLightNormal(2);
            }
            if (rollCountList[4] == 0)
            {
                SetCollectLightShort(4);
            }
            else
            {
                SetCollectLightNormal(4);
            }

            for (var i = 0; i < rollCountList.Count; i++)
            {
                if (rollCountList[i] > 0)
                {
                    lightShowState[i] = true;
                    lightShowState[i + 1] = true;
                }
            }
            for (var i = 0; i < lightShowState.Count; i++)
            {
                if (lightShowState[i])
                {
                    GetCollectLight(i).gameObject.SetActive(true);
                }
            }

            AudioUtil.Instance.PlayAudioFx("J01_Orange");
            await context.WaitSeconds(0.5f);
        }


        public async Task PrepareCollectRoll(int rollIndex)
        {
            var chameleon = chameleonList[rollIndex];
            var openMouthTask = chameleon.OpenMouth();
            var rollHeight = Constant11025.RollHeightList[rollIndex];
            await context.WaitSeconds(0.1f);
            var stickyElementList = StickyMap[rollIndex];
            var roll = GetRoll(rollIndex);
            for (var i = rollHeight - 1; i >= 0; i--)
            {
                var elementContainer = stickyElementList[(uint) i];
                if (elementContainer.HasContainer())
                {
                    var realElementContainer = roll.GetVisibleContainer(i);
                    if (Constant11025.ValueList.Contains(realElementContainer.sequenceElement.config.id))
                    {
                        ((ElementValue11025)realElementContainer.GetElement()).SetWinRate((long) elementContainer.nowStickyElementData.WinRate);
                    }
                    elementContainer.PerformReadyToCollect();
                    await context.WaitSeconds(0.1f);   
                }
            }
            await openMouthTask;
        }
        public async Task FinishCollectRoll(int rollIndex,bool closeRightLight)
        {
            var rollHeight = Constant11025.RollHeightList[rollIndex];
            GetCollectLight(rollIndex).gameObject.SetActive(false);
            if (closeRightLight)
            {
                GetCollectLight(rollIndex+1).gameObject.SetActive(false);
            }
            var chameleon = chameleonList[rollIndex];
            // chameleon.CloseMouth();
            CleanRoll(rollIndex);
            await context.WaitSeconds(0.3f);
        }
    }
}