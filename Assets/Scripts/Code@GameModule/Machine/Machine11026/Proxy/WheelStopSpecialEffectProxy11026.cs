using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
	public class WheelStopSpecialEffectProxy11026 : WheelStopSpecialEffectProxy
	{
		ElementConfigSet elementConfigSet = null;
		private LockElementLayer11026 _layer;
		private ExtraState11026 extraState;

		public WheelStopSpecialEffectProxy11026(MachineContext context)
			: base(context)
		{
			elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
			extraState = machineContext.state.Get<ExtraState11026>();
		}

		protected override async void HandleCustomLogic()
		{
			if (!machineContext.state.Get<ReSpinState>().IsInRespin)
			{
				if (extraState.GetIsMega())
				{
					machineContext.view.Get<LockElementLayer11026>().RecyleRandomWild();
					ShowRandomWildElement();
				}

				if (extraState.GetIsSuper())
				{
					machineContext.view.Get<LockElementLayer11026>().RecyleStickyWild();
					ShowStickryWildElement();
				}
			}

			await machineContext.view.Get<LinkLockView11026>().ShowReSpinCount();
			base.HandleCustomLogic();
		}

		private void ShowRandomWildElement()
		{
			var listWildPos = extraState.GetFreeRandomWildIds();
			if (listWildPos.Count > 0)
			{
				var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
				foreach (var wildPos in listWildPos)
				{
					var roll = wheel.GetRoll((int) wildPos.X);
					var container = roll.GetVisibleContainer((int) wildPos.Y);
					//更换wild
					var elementConfig = elementConfigSet.GetElementConfig(Constant11026.WildElementId);
					container.UpdateElement(new SequenceElement(elementConfig, machineContext));
				}
			}
		}


		private void ShowStickryWildElement()
		{
			var listWildPos = extraState.GetFreeStickyWildIds();
			if (listWildPos.Count > 0)
			{
				var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
				foreach (var wildPos in listWildPos)
				{
					var roll = wheel.GetRoll((int) wildPos.X);
					var container = roll.GetVisibleContainer((int) wildPos.Y);
					var endPos = container.transform.position;
					//更换wild
					var elementConfig =
						elementConfigSet.GetElementConfig(Constant11026.WildElementId);
					container.UpdateElement(new SequenceElement(elementConfig, machineContext));
				}
			}
		}
	}
}