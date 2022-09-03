using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class WinLineAnimationController11029 : WinLineAnimationController
    {
        protected override bool NeedShowWinFrame(WinLine winLine)
        {
            if (wheel.GetContext().state.Get<FreeSpinState>().freeSpinId == 2 &&
                wheel.GetContext().state.Get<FreeSpinState>().IsInFreeSpin &&
                wheel.GetContext().state.Get<FreeSpinState>().TotalCount!= 0 )
            {
                return false;
            }
            return true;
        }
        
        public override void PlayElementWinAnimation(uint rollIndex, uint rowIndex, bool showWinFrame = true)
        {
            var freeSpinState = wheel.GetContext().state.Get<FreeSpinState>();
            if (freeSpinState.freeSpinId >= 3 && freeSpinState.IsInFreeSpin == true)
            {
                 winFrameLayer.Scale = 0.81f;
            }
            else
            {
                winFrameLayer.Scale = 0;
            }

            if (freeSpinState.freeSpinId >= 6 && freeSpinState.IsInFreeSpin)
            {
                var winFrameElementContainer = wheel.GetWinFrameElementContainer((int) rollIndex, (int) rowIndex);
                if (rollIndex == 2)
                {
                    rowIndex = 1;
                }
                var elementContainer = wheel.GetWinLineElementContainer((int) rollIndex, (int) rowIndex);
                if (showWinFrame)
                {
                    winFrameLayer?.ShowWinFrame(winFrameElementContainer);
                }

                var element = elementContainer.GetElement();
                if (element != null && !element.HasAnimState("Win"))
                {
                    return;
                }

                if (!elementContainer.doneTag)
                {
                    elementContainer.ShiftSortOrder(true);
                    elementContainer.PlayElementAnimation("Win");

                    elementContainer.doneTag = true;
                }
            }
            else
            {
                 base.PlayElementWinAnimation(rollIndex, rowIndex, showWinFrame);
            }
        }

        public override async Task BlinkWinLineAsync(WinLine winLine)
        {
            if (wheel.GetContext().state.Get<FreeSpinState>().freeSpinId == 2)
            {
                int index = 0;
                var winLines = wheel.wheelState.GetNormalWinLine();
                for (var i = 0; i < winLines.Count; i++)
                {
                    index = (int) winLines[i].PayRuleId;
                }

                if (index > 0)
                {
                    wheel.GetContext().view.Get<MiniGameView11029>().PlayWinFrame(index);
                }
            }
            await base.BlinkWinLineAsync(winLine);
        }

        public override void StopElementWinAnimation(uint rollIndex, uint rowIndex)
        {
            var container = wheel.GetWinLineElementContainer((int) rollIndex, (int) rowIndex);

            var element = container.GetElement();
            if (element != null && !element.HasAnimState("Win"))
            {
                return;
            }

            if (!container.doneTag)
            {
                if (wheel.GetContext().state.Get<FreeSpinState>().freeSpinId == 6 ||
                    wheel.GetContext().state.Get<FreeSpinState>().freeSpinId == 7 ||
                    wheel.GetContext().state.Get<FreeSpinState>().freeSpinId == 8)

                {
                    if (rollIndex == 2 && rowIndex == 1)
                    {
                        container.ShiftSortOrder(false);
                        //   panelView.ReelViews[col].UpdateContainerWinSortingOrder(symbolContainerView, false);
                        container.doneTag = true;
                    }
                    else
                    {
                        container.UpdateAnimationToStatic();
                        container.ShiftSortOrder(false);
                        //   panelView.ReelViews[col].UpdateContainerWinSortingOrder(symbolContainerView, false);
                        container.doneTag = true;
                    }
                }else
                {
                    container.UpdateAnimationToStatic();
                    container.ShiftSortOrder(false);
                    //   panelView.ReelViews[col].UpdateContainerWinSortingOrder(symbolContainerView, false);
                    container.doneTag = true;
                }
            }
        }
    }
}