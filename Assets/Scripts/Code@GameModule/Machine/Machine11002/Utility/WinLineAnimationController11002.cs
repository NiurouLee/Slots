using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WinLineAnimationController11002 : WinLineAnimationController
    {
        private ChillView GetCurrentChillView()
        {
            ChillView chill;
            if (wheel.wheelName == "Wheel0")
            {
                chill = wheel.GetContext().view.Get<ChillView>();
            }
            else
            {
                chill = wheel.GetContext().view.Get<ChillView>(1);
            }

            return chill;
        }

        public override async Task BlinkWinLineAsync(WinLine winLine)
        {
            if (winLine.Positions == null || winLine.Positions.Count == 0)
                return;

            var wheelConfig = wheel.wheelState.GetWheelConfig();
            var count = winLine.Positions.Count;

            var chill = GetCurrentChillView();

            for (var i = 0; i < count; i++)
            {
                var pos = winLine.Positions[i];
                var rollIndex = (int)pos.X;
                if (wheelConfig.isIndependentWheel)
                {
                    rollIndex = (int)(pos.X / wheelConfig.rollRowCount);
                }
                if (((winLine.Mask >> rollIndex) & 1) > 0 || NeedPlayWinAnimation(winLine))
                {
                    if (chill.IsFullWild(i))
                    {
                        chill.PlayFullWildWin(i);
                        var elementContainer = wheel.GetWinLineElementContainer((int)pos.X, (int)pos.Y);
                        winFrameLayer?.ShowWinFrame(elementContainer);
                    }
                    else
                    {
                        PlayElementWinAnimation(pos.X, pos.Y, NeedShowWinFrame(winLine));
                    }
                }
                else
                {
                    chill.PlayFullWildIdle(i);
                }
            }
            payLineLayer?.ShowPayLine(winLine);

            wheel.ResetDoneTag();

            int actionId = repeatOperationId;

            await wheel.GetContext().WaitSeconds(wheel.wheelState.GetWheelConfig().winLineBlinkDuration);

            if (actionId != repeatOperationId)
                return;

            //如果只有一条时间线循环播放，那么不使用stop方法，stop方法内部处理精灵遮罩会造成层级跳跃
            //TODO:如果所有图标都在一条线上，但并不是只有一条，这里会发生跳帧，考虑一下优化
            for (var i = 0; i < count; i++)
            {
                var pos = winLine.Positions[i];
                var rollIndex = (int)pos.X;
                if (wheelConfig.isIndependentWheel)
                {
                    rollIndex = (int)(pos.X / wheelConfig.rollRowCount);
                }

                if (((winLine.Mask >> rollIndex) & 1) > 0 || NeedPlayWinAnimation(winLine))
                {
                    StopElementWinAnimation(pos.X, pos.Y);
                }
            }

            winFrameLayer?.HideAllWinFrame();
            payLineLayer?.HideAllPayLines();

            wheel.ResetDoneTag();
        }
    }
}