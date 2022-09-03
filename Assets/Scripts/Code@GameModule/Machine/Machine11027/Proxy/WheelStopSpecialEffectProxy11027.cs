using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11027 : WheelStopSpecialEffectProxy
    {
        private ExtraState11027 extraState;

        public WheelStopSpecialEffectProxy11027(MachineContext context)
            : base(context)
        {
            extraState = machineContext.state.Get<ExtraState11027>();
        }

        protected override async void HandleCustomLogic()
        {
            //wild的光飞到收集区
            await WildFlyCollect();
            base.HandleCustomLogic();
        }
        
        private async Task WildFlyCollect()
		{
			machineContext.AddWaitEvent(WaitEvent.WAIT_STOP_SPECIAL_EFFECT);
			await WildAllElements();
            await PLayWildShowAnimation();
            machineContext.RemoveWaitEvent(WaitEvent.WAIT_STOP_SPECIAL_EFFECT);
        }

        private async Task PLayWildShowAnimation()
        {
            Wheel wheel = machineContext.state.Get<WheelsActiveState11027>().GetRunningWheel()[0];
            int winLinesCount = wheel.wheelState.GetNormalWinLine().Count;
            if (winLinesCount <= 0)
            {
                return;
            }
            var winLines = wheel.wheelState.GetNormalWinLine();
            List<ElementContainer> twoColumnWildList = new List<ElementContainer>();
            List<ElementContainer> threeColumnWildList = new List<ElementContainer>();
            List<ElementContainer> fourColumnWildList = new List<ElementContainer>();
            for (var i = 0; i < 3; i++)
            {
                var container = wheel.GetRoll(2).GetVisibleContainer(i);
                if (Constant11027.ListWildElementIds.Contains(container.sequenceElement.config.id))
                {
                    twoColumnWildList.Add(container);
                }
            }

            for (var i = 0; i < 3; i++)
            {
                var container = wheel.GetRoll(3).GetVisibleContainer(i);
                if (Constant11027.ListWildElementIds.Contains(container.sequenceElement.config.id))
                {
                    threeColumnWildList.Add(container);
                }
            }

            for (var i = 0; i < 3; i++)
            {
                var container = wheel.GetRoll(4).GetVisibleContainer(i);
                if (Constant11027.ListWildElementIds.Contains(container.sequenceElement.config.id))
                {
                    fourColumnWildList.Add(container);
                }
            }
            
            for (int i = 0; i < winLines.Count; i++)
            {
                var count = winLines[i].Positions.Count;
                for (var c = 0; c < count; c++)
                {
                    var pos = winLines[i].Positions[c];
                    var rollIndex = (int) pos.X;
                    var rowIndex = (int) pos.Y;
                    var elementContainer = wheel.GetWinLineElementContainer((int) rollIndex, (int) rowIndex);
                    var element = elementContainer.GetElement();
                    if (twoColumnWildList.Contains(elementContainer) || threeColumnWildList.Contains(elementContainer) || fourColumnWildList.Contains(elementContainer))
                    {
                        elementContainer.ShiftSortOrder(true);
                        elementContainer.PlayElementAnimation("Show");
                    }
                }
            }
            if (twoColumnWildList.Count > 0 || threeColumnWildList.Count > 0 || fourColumnWildList.Count > 0)
            {
                await machineContext.WaitSeconds(2.0f);
            }
        }

        protected async Task WildAllElements()
        {
            Wheel wheel = machineContext.state.Get<WheelsActiveState11027>().GetRunningWheel()[0];
            List<ElementContainer> twoColumnWildList = new List<ElementContainer>();
            List<ElementContainer> threeColumnWildList = new List<ElementContainer>();
            List<ElementContainer> fourColumnWildList = new List<ElementContainer>();
            for (var i = 0; i < 3; i++)
            {
                var container = wheel.GetRoll(2).GetVisibleContainer(i);
                if (Constant11027.ListWildElementIds.Contains(container.sequenceElement.config.id))
                {
                    twoColumnWildList.Add(container);
                }
            }
            
            for (var i = 0; i < 3; i++)
            {
                var container = wheel.GetRoll(3).GetVisibleContainer(i);
                if (Constant11027.ListWildElementIds.Contains(container.sequenceElement.config.id))
                {
                    threeColumnWildList.Add(container);
                }
            }

            for (var i = 0; i < 3; i++)
            {
                var container = wheel.GetRoll(4).GetVisibleContainer(i);
                if (Constant11027.ListWildElementIds.Contains(container.sequenceElement.config.id))
                {
                    fourColumnWildList.Add(container);
                }
            }

            if (twoColumnWildList.Count > 0)
            {
                for (var i = 0; i < twoColumnWildList.Count; i++)
                {
                    // await FlyStar(twoColumnWildList[i]);
                    //拖尾，拖尾时间0.5秒
                    AudioUtil.Instance.PlayAudioFx("W01_Fly");
                    var endPos = machineContext.view.Get<CollectionGroup11027>().GetIntegralAnimationNodePos();
                    var objFly = machineContext.assetProvider.InstantiateGameObject("Fly", true);
                    objFly.transform.parent = machineContext.transform;
                    var startPos = twoColumnWildList[i].transform.position;
                    objFly.transform.position = startPos;
                    XUtility.Fly(objFly.transform, startPos, endPos, 0, 0.5f,null);
                    await machineContext.WaitSeconds(0.5f);
                    AudioUtil.Instance.PlayAudioFx("W01_FlyStop");
                    await machineContext.view.Get<CollectionGroup11027>().ShowCollectionGroup(true);
                    AudioUtil.Instance.PlayAudioFx("W01_FlyStopStock");
                    machineContext.assetProvider.RecycleGameObject("Fly", objFly);
                    await machineContext.WaitSeconds(1.0f);
                    machineContext.view.Get<CollectionGroup11027>().HideParticle();
                }
            }
            
            
            if (threeColumnWildList.Count > 0)
            {
                for (var i = 0; i < threeColumnWildList.Count; i++)
                {
                    // await FlyStar(twoColumnWildList[i]);
                    //拖尾，拖尾时间0.5秒
                    AudioUtil.Instance.PlayAudioFx("W01_Fly");
                    var endPos = machineContext.view.Get<CollectionGroup11027>().GetIntegralAnimationNodePos();
                    var objFly = machineContext.assetProvider.InstantiateGameObject("Fly", true);
                    objFly.transform.parent = machineContext.transform;
                    var startPos = threeColumnWildList[i].transform.position;
                    objFly.transform.position = startPos;
                    XUtility.Fly(objFly.transform, startPos, endPos, 0, 0.5f,null);
                    await machineContext.WaitSeconds(0.5f);
                    // await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f, Ease.Linear, machineContext);
                    AudioUtil.Instance.PlayAudioFx("W01_FlyStop");
                    await machineContext.view.Get<CollectionGroup11027>().ShowCollectionGroup(true);
                    AudioUtil.Instance.PlayAudioFx("W01_FlyStopStock");
                    machineContext.assetProvider.RecycleGameObject("Fly", objFly);
                    await machineContext.WaitSeconds(1.0f);
                    machineContext.view.Get<CollectionGroup11027>().HideParticle();
                }
            }
            
            
            if (fourColumnWildList.Count > 0)
            {
                for (var i = 0; i < fourColumnWildList.Count; i++)
                {
                    // await FlyStar(twoColumnWildList[i]);
                    //拖尾，拖尾时间0.5秒
                    AudioUtil.Instance.PlayAudioFx("W01_Fly");
                    var endPos = machineContext.view.Get<CollectionGroup11027>().GetIntegralAnimationNodePos();
                    var objFly = machineContext.assetProvider.InstantiateGameObject("Fly", true);
                    objFly.transform.parent = machineContext.transform;
                    var startPos = fourColumnWildList[i].transform.position;
                    objFly.transform.position = startPos;
                    XUtility.Fly(objFly.transform, startPos, endPos, 0, 0.5f,null);
                    await machineContext.WaitSeconds(0.5f);
                    // await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f, Ease.Linear, machineContext);
                    AudioUtil.Instance.PlayAudioFx("W01_FlyStop");
                    await machineContext.view.Get<CollectionGroup11027>().ShowCollectionGroup(true);
                    AudioUtil.Instance.PlayAudioFx("W01_FlyStopStock");
                    machineContext.assetProvider.RecycleGameObject("Fly", objFly);
                }
            }
        }
    }
}