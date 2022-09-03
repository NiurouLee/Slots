using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class MachineContextBuilder11301 : MachineContextBuilder
	{
		public MachineContextBuilder11301(string inMachineId)
		:base(inMachineId)
		{

	
		}
		public override void SetUpCommonMachineState(MachineState machineState)
        {
            machineState.Add<BetState11301>();
            machineState.Add<AdStrategyState>();
            machineState.Add<FreeSpinState11301>();
            machineState.Add<AutoSpinState>();
            machineState.Add<ReSpinState>();
            machineState.Add<WinState>();
            machineState.Add<JackpotInfoState>();

            SetUpWheelActiveState(machineState);
        }

		public override IElementExtraInfoProvider GetElementExtraInfoProvider()
		{
			return new ElementExtraInfoProvider11301();
		}
		public override ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
		{
			return base.GetSequenceElementConstructor(machineContext);
		}
		
		public override void SetUpWheelActiveState(MachineState machineState)
		{
			machineState.Add<WheelsActiveState11301>();
		}
		public override void SetUpMachineState(MachineState machineState)
		{
			//Base
			machineState.Add<WheelState>();
			
			//Link
			machineState.Add<WheelState>();
			
			//Free
			machineState.Add<WheelState>();
			
			
            
			machineState.Add<ExtraState11301>();
			machineState.Replace<ReSpinState, ReSpinState11301>();

		}
		public override void BindingWheelView(MachineContext machineContext)
		{
			var WheelBaseGameTrans = machineContext.transform.Find("Wheels/WheelBaseGame");
			var WheelBaseGameWheel = machineContext.view.Add<Wheel>(WheelBaseGameTrans);
			WheelBaseGameWheel.BuildWheel<Roll, ElementSupplier, WheelSpinningController<WheelAnimationController11301>>(machineContext.state.Get<WheelState>());
			WheelBaseGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

			var WheelLinkGameTrans = machineContext.transform.Find("Wheels/WheelLinkGame");
			var WheelLinkGameWheel = machineContext.view.Add<IndependentWheel>(WheelLinkGameTrans);
			WheelLinkGameWheel.BuildWheel<SoloRoll, ElementSupplier, IndependentSpinningController<IndependentWheelAnimationController>>(machineContext.state.Get<WheelState>(1));
			WheelLinkGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();
			WheelLinkGameWheel.wheelMask.gameObject.SetActive(true);
            WheelLinkGameWheel.wheelMask.GetComponent<SpriteMask>().enabled = false;

			var WheelFreeGameTrans = machineContext.transform.Find("Wheels/WheelFreeGame");
			var WheelFreeGameWheel = machineContext.view.Add<Wheel>(WheelFreeGameTrans);
			WheelFreeGameWheel.BuildWheel<Roll11301, ElementSupplier, WheelSpinningController<WheelAnimationController11301>>(machineContext.state.Get<WheelState>(2));
			WheelFreeGameWheel.SetUpWinLineAnimationController<WinLineAnimationController>();

		}
		public override void BindingJackpotView(MachineContext machineContext)
		{
			var jackpotTran = machineContext.transform.Find("JackpotPanel");
			machineContext.view.Add<JackPotPanel>(jackpotTran);
		}
		public override void BindingExtraView(MachineContext machineContext)
		{
			base.BindingExtraView(machineContext);
			machineContext.view.Add<DoorView11301>(null);
			
			var tranTransitionView = machineContext.transform.Find("Wheels/TransitionView");
			machineContext.view.Add<TransitionView11301>(tranTransitionView);
			
			
			var LinkWinView = machineContext.transform.Find("Wheels/WheelLinkGame/WinGroup");
			machineContext.view.Add<BonusWinView11301>(LinkWinView);
			
			
			var linkLockView = machineContext.transform.Find("Wheels/WheelLinkGame/SpinRemainingGroup");
			machineContext.view.Add<LinkLockView11301>(linkLockView);

			var ShopEntranceView = machineContext.transform.Find("Wheels/WheelBaseGame/ShopEntrance");
			machineContext.view.Add<ShopEntranceView11301>(ShopEntranceView);

		}
		public override void AttachTopPanel(MachineContext machineContext)
		{
			base.AttachTopPanel(machineContext);
		}
		protected override Dictionary<LogicStepType,Type>  SetUpLogicStepProxy()
		{
			var logicProxy = base.SetUpLogicStepProxy();
			logicProxy[LogicStepType.STEP_MACHINE_SETUP] = typeof(MachineSetUpProxy11301);
			logicProxy[LogicStepType.STEP_NEXT_SPIN_PREPARE] = typeof(NextSpinPrepareProxy11301);
			logicProxy[LogicStepType.STEP_SUBROUND_START] = typeof(SubRoundStartProxy11301);
			logicProxy[LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT] = typeof(WheelStopSpecialEffectProxy11301);
			logicProxy[LogicStepType.STEP_FREE_GAME] = typeof(FreeGameProxy11301);
			logicProxy[LogicStepType.STEP_RE_SPIN] = typeof(LinkLogicProxy11301);
			logicProxy[LogicStepType.STEP_SUBROUND_FINISH] = typeof(SubRoundFinishProxy11301);

			return logicProxy;
		}


		public override Vector2 GetPadMinXScaler()
		{
			return new Vector2(0.83f, 0.83f);
		}

		public override void AdaptMachineView(MachineContext machineContext)
        {
            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
            {
                // var aspectRatio = ViewResolution.referenceResolutionPortrait.y / ViewResolution.referenceResolutionPortrait.x;
                // var designAspectRatio = GetDesignedAspectRatio();
                // var machineScaleFactor = CaculateMachineScaleFactor(designAspectRatio, aspectRatio);
                // machineContext.MachineScaleFactor = machineScaleFactor;
                // machineContext.transform.localScale = new Vector3(machineScaleFactor, machineScaleFactor, machineScaleFactor);
				
            }
            else
            {
                float x = Mathf.Clamp(ViewResolution.referenceResolutionLandscape.x, 1024, ViewResolution.designSize.x);
                float lerp = (x - 1024f) / (ViewResolution.designSize.x - 1024f);
                Vector2 padScaler = GetPadMinXScaler();
                float lerpScalerX = Mathf.Lerp(padScaler.x, 1, lerp);
                float lerpScalerY = Mathf.Lerp(padScaler.y, 1, lerp);

                var wheels = machineContext.transform.GetComponentInChildren<SimpleRollUpdaterEasingScriptable>()?.transform;
                Vector3 lerpScalerVec = new Vector3(lerpScalerX, lerpScalerY, 1);

                if (wheels == null)
                {
                    wheels = machineContext.transform.Find("Wheels");
                }
                if (wheels == null)
                {
                    wheels = machineContext.transform.Find("ZhenpingAnim/Wheels");
                }

				if(lerpScalerX<=0.84f){
                    machineContext.transform.Find("Wheels").transform.localPosition = new Vector3(0,-0.2f,0);
					machineContext.transform.Find("Background/Title").gameObject.SetActive(true);
                }


                var tranJackpotPanel = wheels.transform.parent.Find("JackpotPanel");
                if (tranJackpotPanel == null)
                {
                    tranJackpotPanel = wheels.transform.parent.Find("ZhenpingAnim/JackpotPanel");
                }

                Transform parentJackpot = null;
                if (tranJackpotPanel != null)
                {
                    parentJackpot = tranJackpotPanel.parent;
                    tranJackpotPanel.parent = wheels;
                }

				var doorView = wheels.transform.parent.Find("LockElementLayer");
				if(doorView !=null){
					doorView.parent = wheels;
					
				}

                wheels.localScale = lerpScalerVec;

                if (tranJackpotPanel != null)
                {
                    tranJackpotPanel.parent = parentJackpot;
                }
				if(doorView!=null){
					doorView.parent = parentJackpot;
				}

            }
        }
	}
}