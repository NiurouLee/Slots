using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Rendering;
using GameModule;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11029 : WheelStopSpecialEffectProxy
    {
        ElementConfigSet elementConfigSet = null;
        private LockElementLayer11029 _layer;
        private ExtraState11029 extraState11029;
        private FreeSpinState freeSpinState;
        private LockElementLayer11029 _layerBonusFree;
        private List<RollShiftHelper> _rollShiftHelper = new List<RollShiftHelper>();

        public WheelStopSpecialEffectProxy11029(MachineContext context)
            : base(context)
        {
            elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
            extraState11029 = machineContext.state.Get<ExtraState11029>();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
        }

        public override void SetUp()
        {
            base.SetUp();

            for (int i = 0; i < 5; i++)
            {
                _rollShiftHelper.Add(new RollShiftHelper(machineContext));
            }

            var wheelBase = machineContext.view.Get<Wheel>(0);
            _layer = machineContext.view.Add<LockElementLayer11029>(wheelBase.transform);
            _layer.BindingWheel(wheelBase);
            _layer.SetSortingGroup("LocalFx", 400);

            var wheelFree = machineContext.view.Get<Wheel>(1);
            _layerBonusFree = machineContext.view.Add<LockElementLayer11029>(wheelFree.transform);
            _layerBonusFree.BindingWheel(wheelFree);
            _layerBonusFree.SetSortingGroup("Element", 9999);
        }

        protected override async void HandleCustomLogic()
        {
            PlayIdle();
            if (freeSpinState.IsInFreeSpin)
            {
                if (freeSpinState.freeSpinId == 0)
                {
                    var wheelsSpinningProxy11029 =
                        machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as WheelsSpinningProxy11029;
                    wheelsSpinningProxy11029._layer.RecyleRandomWild();
                    ShowRandomWildElement();
                }
                else if (freeSpinState.freeSpinId == 3)
                {
                    var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();
                    for (int w = 0; w < wheel.Count; w++)
                    {
                        machineContext.state.Get<WheelsActiveState11029>().FadeOutMapRollMask(wheel[w]);
                    }
                    var wheelsSpinningProxy11029 =
                        machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as WheelsSpinningProxy11029;
                    wheelsSpinningProxy11029._layerMapGameRandomWild1.RecyleMapGameRandomWild1();
                    wheelsSpinningProxy11029._layerMapGameRandomWild2.RecyleMapGameRandomWild2();
                    wheelsSpinningProxy11029._layerMapGameRandomWild3.RecyleMapGameRandomWild3();
                    wheelsSpinningProxy11029._layerMapGameRandomWild1.RecyleAllMapGameFire(1);
                    wheelsSpinningProxy11029._layerMapGameRandomWild2.RecyleAllMapGameFire(2);
                    wheelsSpinningProxy11029._layerMapGameRandomWild3.RecyleAllMapGameFire(3);
                    ShowMapGameRandomWildElement();
                }
                else if (freeSpinState.freeSpinId == 5)
                {
                    var wheelsSpinningProxy11029 =
                        machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as WheelsSpinningProxy11029;
                    wheelsSpinningProxy11029._layerMapGameRandomWild1.RecyleMapGameStickyWild1();
                    wheelsSpinningProxy11029._layerMapGameRandomWild2.RecyleMapGameStickyWild2();
                    wheelsSpinningProxy11029._layerMapGameRandomWild3.RecyleMapGameStickyWild3();
                    ShowMapGameStickyWildElement();
                }
                else if (freeSpinState.freeSpinId == 1)
                {
                    machineContext.view.Get<LockElementLayer11029>().ClearAllLayer(0, 1);
                    if (extraState11029.GetIsDrag())
                    {
                        await BonusFly();
                    }
                }
                else if (freeSpinState.IsInFreeSpin && (freeSpinState.freeSpinId == 2))
                {
                    AudioUtil.Instance.StopAudioFx("Map_Classic_Spin");
                }
                else if (freeSpinState.freeSpinId == 4)
                {
                    var wheelsSpinningProxy11029 =
                        machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as WheelsSpinningProxy11029;
                    wheelsSpinningProxy11029._layerMapGameRandomWild1.RecyleMapGameM0vingWild1();
                    wheelsSpinningProxy11029._layerMapGameRandomWild2.RecyleMapGameMovingWild2();
                    wheelsSpinningProxy11029._layerMapGameRandomWild3.RecyleMapGameMovingWild3();
                    ShowMapGameMovingWildElement();
                }
                else if (freeSpinState.freeSpinId == 6 || freeSpinState.freeSpinId == 7 ||
                         freeSpinState.freeSpinId == 8)
                {
                    await ShowMapGameMul1WildElement();
                }
            }
            else
            {
                if (extraState11029.GetIsDrag())
                {
                    await BonusFly();
                    await BlinkBonusLine();
                    if (machineContext.state.Get<BetState>().IsFeatureUnlocked(0) && !extraState11029.GetIsDrag())
                    { 
                        CollectHorse();
                    }
                }else if (extraState11029.GetIsWheel())
                {
                    await BlinkBonusLine();
                    if (machineContext.state.Get<BetState>().IsFeatureUnlocked(0) && !extraState11029.GetIsDrag())
                    {
                        await StartCollectHorse();
                    }
                }else if (freeSpinState.IsTriggerFreeSpin)
                {
                    await BlinkBonusLine();
                    await StartCollectHorse();
                }
                else
                {
                    await BlinkBonusLine();
                    if (machineContext.state.Get<BetState>().IsFeatureUnlocked(0) && !extraState11029.GetIsDrag())
                    { 
                        CollectHorse();
                    }
                }
            }
            base.HandleCustomLogic();
        }

        public void PlayIdle()
        {
            var wheels = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (Constant11029.ListJSymbolElementIds.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Idle");
                }
            }
        }

        private async void CollectHorse()
        {
            await StartCollectHorse();
        }


        private async Task StartCollectHorse()
        {
            bool audioPlayed = false;
            
            int currentPoint =  (int) machineContext.state.Get<ExtraState11029>().GetMapPoint();
            
            var wheels = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == Constant11029.HorseElementId)
                {
                    return true;
                }

                return false;
            });
            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    var horseIcon = machineContext.assetProvider.InstantiateGameObject("Active_S01");
                    horseIcon.transform.parent = machineContext.transform;
                    horseIcon.transform.localScale = machineContext.transform.Find("WheelFeature/Wheels").localScale;
                    var targetPosition = machineContext.view.Get<ProgressBar11029>().GetIntegralPos();
                    horseIcon.transform.position = reTriggerElementContainers[i].transform.position;
                    var sortingGroup = horseIcon.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
                    sortingGroup.sortingOrder = 400;
                    horseIcon.transform.gameObject.GetComponent<Animator>().Play("Blink");
                    machineContext.WaitSeconds(0.2f, () =>
                    {
                        if (!audioPlayed)
                            AudioUtil.Instance.PlayAudioFxOneShot("Map_Fly");
                        audioPlayed = true;
                        XUtility.Fly(horseIcon.transform, horseIcon.transform.position, targetPosition, 0, 0.6f,
                            () =>
                            {
                                GameObject.Destroy(horseIcon);
                            });
                    });
                }
                await machineContext.WaitSeconds(0.8f);
                await machineContext.view.Get<ProgressBar11029>().ChangeFill(true, true, currentPoint);
            }
        }
        
        private void ShowRandomWildElement()
        {
            var listWildPos = extraState11029.GetFreeRandomWildIds();
            if (listWildPos.Count > 0)
            {
                var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
                foreach (var wildPos in listWildPos)
                {
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int) wildPos.Y);
                    //更换wild
                    var elementConfig = elementConfigSet.GetElementConfig(Constant11029.WildElementId);
                    container.UpdateElement(new SequenceElement(elementConfig, machineContext));
                }
            }
        }

        private void ShowMapGameRandomWildElement()
        {
            var listWildPos = extraState11029.GetRandomWildIds();
            if (listWildPos.Count > 0)
            {
                for (var i = 0; i < listWildPos.Count; i++)
                {
                    if (listWildPos[i].Items.Count > 0)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[i];
                        foreach (var wildPos in listWildPos[i].Items)
                        {
                            var roll = wheel.GetRoll((int) wildPos.X);
                            var container = roll.GetVisibleContainer((int) wildPos.Y);
                            //更换wild
                            var elementConfig = elementConfigSet.GetElementConfig(Constant11029.WildElementId);
                            container.UpdateElement(new SequenceElement(elementConfig, machineContext));
                        }
                    }
                }
            }
        }

        public async Task BonusFly()
        {
            machineContext.view.Get<LightBaseView11029>().HideBaseLight();
            machineContext.view.Get<HighLightView11029>().HideHighLight();
            var wheelsSpinningProxy11029 =
                machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as
                    WheelsSpinningProxy11029;
            //美杜莎更换长图和轮盘拉升同时进行
            await machineContext.WaitSeconds(0.1f);
            if (freeSpinState.IsInFreeSpin)
            {
                wheelsSpinningProxy11029._layerBonusFree.PlayInBonusWin();
            }
            else
            {
                wheelsSpinningProxy11029._layerBase.PlayInWin();
            }
            AudioUtil.Instance.PlayAudioFx("BonusGame_Burst");
            await machineContext.WaitSeconds(1.0f-0.1f);
            var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            //创建J系列
            await ShowRandomGems();
            // await _layer.ShowRandomGems();
            //播放刷新
            // _layer.PlayTrigger();
            var endPos = machineContext.view.Get<ControlPanel>().GetWinTextRefWorldPosition(Vector3.zero);
            int rollCount = wheel.rollCount;
            for (int m = 0; m < rollCount; m++)
            {
                for (int n = 0; n < wheel.GetRoll(m).rowCount; n++)
                {
                    var container = wheel.GetRoll(m).GetVisibleContainer(n);
                    if (Constant11029.ListJSymbolElementIds.Contains(container.sequenceElement.config.id))
                    {
                        container.PlayElementAnimation("Trigger");
                    }
                }
            }
            AudioUtil.Instance.PlayAudioFx("BonusGame_Refresh");
            await machineContext.WaitSeconds(2.0f);
            for (int x = 0; x < rollCount; x++)
            {
                for (int y = 0; y < wheel.GetRoll(x).rowCount; y++)
                {
                    var container = wheel.GetRoll(x).GetVisibleContainer(y);
                    if (Constant11029.ListJSymbolElementIds.Contains(container.sequenceElement.config.id))
                    {
                        ulong winRate = container.sequenceElement.config.GetExtra<ulong>("winRate");
                        ulong chips = container.sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                        var objFly = machineContext.assetProvider.InstantiateGameObject("Fly", true);
                        objFly.transform.parent = machineContext.transform;
                        var startPos = container.transform.position;
                        objFly.transform.position = startPos;
                        objFly.transform.localScale = machineContext.transform.Find("WheelFeature/Wheels").localScale;
                        AudioUtil.Instance.PlayAudioFx("BonusGame_Fly");
                        container.PlayElementAnimation("Active_J02_Fly");
                        XUtility.Fly(objFly.transform, startPos, endPos, 0, 0.5f, null);
                        await machineContext.WaitSeconds(0.5f);
                        var objFinish = machineContext.assetProvider.InstantiateGameObject("WinEffetcs", true);
                        objFinish.transform.parent = machineContext.transform;
                        objFinish.transform.position = endPos;
                        objFinish.transform.gameObject.SetActive(true);
                        // XUtility.Fly(objFinish.transform, endPos, endPos, 0, 0.5f,null);
                        AudioUtil.Instance.PlayAudioFx("BonusGame_RaiseWin");
                        AddWinChipsToControlPanel(chips, 0.5f, false, false);
                        await machineContext.WaitSeconds(0.5f);
                        machineContext.assetProvider.RecycleGameObject("Fly", objFly);
                        machineContext.assetProvider.RecycleGameObject("WinEffetcs", objFinish);
                    }
                }
            }
        }

        public async Task ShowRandomGems()
        {
            var extraState = machineContext.state.Get<ExtraState11029>();
            var listWildPos = extraState.GetRandomGems();
            if (listWildPos.Count > 0)
            {
                var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                for (int i = 0; i < listWildPos.Count; i++)
                {
                    var roll = wheel.GetRoll((int) listWildPos[i].X);
                    var container = roll.GetVisibleContainer((int) listWildPos[i].Y);
                    var elementConfig = elementConfigSet.GetElementConfig(listWildPos[i].Id);
                    container.UpdateElement(new SequenceElement(elementConfig, machineContext));
                    container.ShiftSortOrder(true);
                    container.PlayElementAnimation("Exp");
                    AudioUtil.Instance.PlayAudioFx("BonusGame_J02_Burst");
                    if (i == listWildPos.Count)
                    {
                        await machineContext.WaitSeconds(0.83f);
                    }
                    else
                    {
                        await machineContext.WaitSeconds(0.6f);
                    }
                }
            }
        }

        protected async Task BlinkBonusLine()
        {
            var _extraState11029 = machineContext.state.Get<ExtraState11029>();
            var bagWinLines = new List<WinLine>();
            var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
            var bonusWinLines = wheel.wheelState.GetBonusWinLine();
            for (int i = 0; i < bonusWinLines.Count; i++)
            {
                if (bonusWinLines[i].BonusGameId == 4001)
                {
                    bagWinLines.Add(bonusWinLines[i]);
                }
            }

            if (bagWinLines.Count > 0)
            {
                for (int i = 0; i < bagWinLines.Count; i++)
                {
                    await _layer.ShowBigBagElement((int) bagWinLines[i].Positions[0].X);
                }
                await _layer.PlayWin();
                if (bagWinLines[0].Pay > 0)
                {
                    await machineContext.view.Get<MoneyBag11029>().ShowCollectionFiveGroup(true);
                    var chips = machineContext.state.Get<BetState>().GetPayWinChips(bagWinLines[0].Pay);
                    await machineContext.view.Get<MoneyBag11029>().ShowCollectionGroupZhen(chips);
                    if (!_extraState11029.NeedWheelSettle())
                    {
                        AddWinChipsToControlPanel((ulong) chips, 1, false, false);
                        await machineContext.WaitSeconds(1.0f);
                    }
                }
                else
                {
                    await machineContext.view.Get<MoneyBag11029>().ShowCollectionGroup(true);
                }
            }
        }

        private async Task ShowMapGameMul1WildElement()
        {
            for (int i = 0; i < 3; i++)
            {
                var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[i];
                var roll = wheel.GetRoll((int) 2);
                var container = roll.GetVisibleContainer(1);
                //更换wild
                var elementConfig = elementConfigSet.GetElementConfig(Constant11029.BigWildElementId);
                container.UpdateElement(new SequenceElement(elementConfig, machineContext),true);
                container.ShiftSortOrder(true);
                var sortingGroup = container.transform.GetComponent<SortingGroup>();
                sortingGroup.sortingOrder = 9999;
                roll.transform.gameObject.SetActive(true);
                for (int k = 0; k < roll.rowCount; k++)
                {
                    var elementContainer = roll.GetVisibleContainer(k);
                    elementContainer.transform.gameObject.SetActive(false);
                }
                container.transform.gameObject.SetActive(true);
                wheel.transform.Find("Wild").gameObject.SetActive(false);
                var extraState = machineContext.state.Get<ExtraState11029>();
                var listWildMul = extraState.GetRandomWildIdsGetMapMultipliers();
                var num = listWildMul[i];
                container.transform.GetChild(0).Find("BetText").gameObject
                    .GetComponent<TextMesh>().text = num.ToString() + "X";
                container.PlayElementAnimation("Open");
            }
            AudioUtil.Instance.PlayAudioFx("Map_Wild_Open");
            await machineContext.WaitSeconds(0.5f);
        }

        private void ShowMapGameStickyWildElement()
        {
            var listWildPos = extraState11029.GetStickyWildIds();
            if (listWildPos.Count > 0)
            {
                for (var i = 0; i < listWildPos.Count; i++)
                {
                    if (listWildPos[i].Items.Count > 0)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[i];
                        foreach (var wildPos in listWildPos[i].Items)
                        {
                            var roll = wheel.GetRoll((int) wildPos.X);
                            var container = roll.GetVisibleContainer((int) wildPos.Y);
                            //更换wild
                            var elementConfig = elementConfigSet.GetElementConfig(Constant11029.WildElementId);
                            container.UpdateElement(new SequenceElement(elementConfig, machineContext));
                        }
                    }
                }
            }
        }
        
        private void ShowMapGameMovingWildElement()
        {
            var listWildPos = extraState11029.GetMovingWildIds();
            if (listWildPos.Count > 0)
            {
                for (var i = 0; i < listWildPos.Count; i++)
                {
                    if (listWildPos[i].Items.Count > 0)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[i];
                        foreach (var wildPos in listWildPos[i].Items)
                        {
                            var roll = wheel.GetRoll((int) wildPos.X);
                            var container = roll.GetVisibleContainer((int) wildPos.Y);
                            //更换wild
                            var elementConfig = elementConfigSet.GetElementConfig(Constant11029.WildElementId);
                            container.UpdateElement(new SequenceElement(elementConfig, machineContext));
                        }
                    }
                }
            }
        }
    }
}
   