using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class SubRoundStartProxy11301 : SubRoundStartProxy
	{
		public SubRoundStartProxy11301(MachineContext context)
		:base(context)
		{

		}
		/// <summary>
		/// 为了避免每次respin重置已经锁定link的图标
		/// </summary>
		protected override void StopWinCycle(bool force = false)
        {
            var activeState = machineContext.state.Get<WheelsActiveState>();
            var runningWheel = activeState.GetRunningWheel();
			// 踩坑---respin下不需要打断已经锁定的link图标的动画
			var respinState = machineContext.state.Get<ReSpinState>();
			if(respinState.NextIsReSpin) return;
            for (var i = 0; i < runningWheel.Count; i++){
				runningWheel[i].winLineAnimationController.StopAllElementAnimation();
			}
                
        }
		protected override void HandleCustomLogic()
		{
			Constant11301.IsShowMapFeature = false;
			machineContext.view.Get<DoorView11301>().RollingStartLockFreeDoor();
			machineContext.view.Get<DoorView11301>().OpenLinkAnticipation();
			machineContext.view.Get<LinkLockView11301>().RefreshReSpinCount(true);

			base.HandleCustomLogic();
		}
	}
}