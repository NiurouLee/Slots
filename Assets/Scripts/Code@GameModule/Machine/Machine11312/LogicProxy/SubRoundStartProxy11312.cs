using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule
{
    public class SubRoundStartProxy11312 : SubRoundStartProxy
    {
        public SubRoundStartProxy11312(MachineContext context) : base(context)
        {
        }

        /// <summary>
		/// 为了避免每次respin重置已经锁定link的图标
		/// </summary>
		protected override void StopWinCycle(bool force = false)
        {
            // 踩坑---respin下不需要打断已经锁定的link图标的动画
            var respinState = machineContext.state.Get<ReSpinState>();
            if (respinState.NextIsReSpin) return;
            base.StopWinCycle();

        }
    }
}

