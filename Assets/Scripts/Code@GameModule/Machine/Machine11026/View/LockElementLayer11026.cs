using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;


namespace GameModule
{
    public class LockElementLayer11026 : LockElementLayer
    {
        public List<DragonRisingGameResultExtraInfo.Types.Position> lastWildPosList;
        public List<DragonRisingGameResultExtraInfo.Types.Position> lastRandomWildPosList;

        public LockElementLayer11026(Transform transform)
            : base(transform)
        {
            lastRandomWildPosList = new List<DragonRisingGameResultExtraInfo.Types.Position>();
            lastWildPosList = new List<DragonRisingGameResultExtraInfo.Types.Position>();
        }

        //super时固定wild
        public void ShowStickyWildElement()
        {
            var extraState = context.state.Get<ExtraState11026>();
            var listWildPos = extraState.GetFreeStickyWildIds();
            if (listWildPos.Count > 0)
            {
                var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                foreach (var wildPos in listWildPos)
                {
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    //更换wild
                    LockElement(
                        context.machineConfig.GetSequenceElement(Constant11026.WildElementId, context),
                        (int) wildPos.X, (int) wildPos.Y, true);
                    var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
                    newWild.transform.position = endPos;
                    lastWildPosList.Add(wildPos);
                }
            }
        }

        //从link回来时固定随机wild
        public void ReShowRandomWildElement()
        {
            var extraState = context.state.Get<ExtraState11026>();
            var listReShowRandomWildPos = extraState.GetFreeRandomWildIds();
            if (listReShowRandomWildPos.Count > 0)
            {
                var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                foreach (var wildPos in listReShowRandomWildPos)
                {
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    //更换wild
                    LockElement(
                        context.machineConfig.GetSequenceElement(Constant11026.WildElementId, context),
                        (int) wildPos.X, (int) wildPos.Y, true);
                    var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
                    newWild.transform.position = endPos;
                }
            }
        }
        
        //回收产生的随机wild
        public void RecyleRandomWild()
        {
            var extraState = context.state.Get<ExtraState11026>();
            var listWildPos = extraState.GetFreeRandomWildIds();
            if (listWildPos.Count > 0)
            {
                foreach (var wildPos in listWildPos)
                {
                    // if (lastRandomWildPosList.Contains(wildPos))
                    // {
                        ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                        // listWildPos.Remove(wildPos);
                    // }
                }
            }
        }

        //回收产生的固定wild
        public void RecyleStickyWild()
        {
            var extraState = context.state.Get<ExtraState11026>();
            var listWildPos = extraState.GetFreeStickyWildIds();
            if (listWildPos.Count > 0)
            {
                foreach (var wildPos in listWildPos)
                {
                    ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                }
            }
        }

        //free过场动画完成后销毁wild
        public void FinishRecyleStickeyWild()
        {
            if (lastWildPosList.Count > 0)
            {
                foreach (var wildPos in lastWildPosList)
                {
                    ClearAllLayer((int) wildPos.X, (int) wildPos.Y);
                }
            }
        }

        private void ClearAllLayer(int row, int column)
        {
            ClearElement(row, column);
        }

        //随机投放wild
        public async Task ShowRandomWildElement()
        {
            var extraState = context.state.Get<ExtraState11026>();
            var listWildPos = extraState.GetFreeRandomWildIds();
            if (listWildPos.Count > 0)
            {
                var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                foreach (var wildPos in listWildPos)
                {
                    //喷火
                    context.view.Get<TransitionsView11026>().PlaySuperFreeTransition();
                    await context.WaitSeconds(0.36f);
                    AudioUtil.Instance.PlayAudioFx("FreeGame_MegaBonus");
                    var roll = wheel.GetRoll((int) wildPos.X);
                    var container = roll.GetVisibleContainer((int) wildPos.Y);
                    //拖尾
                    var objFly = context.assetProvider.InstantiateGameObject("ep_Active_J01_Super", true);
                    objFly.transform.parent = context.transform;
                    var startPos = context.view.Get<TransitionsView11026>().GetIntegralPos();
                    objFly.transform.position = startPos;
                    var endPos = wheel.GetRoll((int) wildPos.X).GetVisibleContainerPosition((int) wildPos.Y);
                    await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f, Ease.Linear, context);
                    //更换wild
                    LockElement(context.machineConfig.GetSequenceElement(Constant11026.WildElementId, context),
                        (int) wildPos.X, (int) wildPos.Y, true);
                    var newWild = GetElement((int) wildPos.X, (int) wildPos.Y);
                    newWild.transform.position = endPos;
                    newWild.transform.gameObject.GetComponent<Animator>().Play("Super");
                    lastRandomWildPosList.Add(wildPos);
                    await context.WaitSeconds(0.4f);
                    context.assetProvider.RecycleGameObject("ep_Active_J01_Super", objFly);
                    lastRandomWildPosList.Add(wildPos);
                }
            }
        }
    }
}