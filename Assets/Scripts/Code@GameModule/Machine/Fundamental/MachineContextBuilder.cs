// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:45 AM
// Ver : 1.0.0
// Description : MachineBuilder.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public abstract class MachineContextBuilder
    {
        public string machineId;

        public VerticalMachineLayoutHelper layoutHelper;
        public MachineContextBuilder(string inMachineId)
        {
            machineId = inMachineId;
        }

        public void BindingMachineView(MachineContext machineContext)
        {
            BindingBackgroundView(machineContext);
            BindingWheelView(machineContext);
            BindingJackpotView(machineContext);
            BindingExtraView(machineContext);
        }


        public virtual Vector2 GetPadMinXScaler()
        {
            return new Vector2(0.9f, 0.9f);
        }

        public virtual void AdaptMachineView(MachineContext machineContext)
        {
            if (!Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
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

                wheels.localScale = lerpScalerVec;

                if (tranJackpotPanel != null)
                {
                    tranJackpotPanel.parent = parentJackpot;
                }
            }
        }

        public VerticalMachineLayoutHelper GetLayOutHelper()
        {
            return layoutHelper;
        }

        public void AttachMachineView(MachineContext machineContext)
        {
            AttachTopPanel(machineContext);
            AttachControlPanel(machineContext);
            AttachFiveOfKindView(machineContext);
            AttachExtraView(machineContext);
        }

        public virtual MachineConfig SetUpMachineConfig(MachineContext machineContext)
        {
            TextAsset textAsset = machineContext.assetProvider.GetAsset<TextAsset>($"Machine{machineId}Config");

            if (textAsset != null)
            {
                MachineConfig machineConfig = LitJson.JsonMapper.ToObject<MachineConfig>(textAsset.text);

                machineConfig.audioConfig = SetUpCommonAudioConfig(machineContext);

                return machineConfig;
            }

            return null;
        }

        public virtual IMachineAudioConfig SetUpCommonAudioConfig(MachineContext machineContext)
        {
            var audioConfig = new MachineAudioConfig(machineId);
            return audioConfig;
        }

        public virtual IElementExtraInfoProvider GetElementExtraInfoProvider()
        {
            return new ElementExtraInfoProvider();
        }

        public virtual ISequenceElementConstructor GetSequenceElementConstructor(MachineContext machineContext)
        {
            return new SequenceElementConstructor(machineContext);
        }

        public virtual void SetUpCommonMachineState(MachineState machineState)
        {
            machineState.Add<BetState>();
            machineState.Add<AdStrategyState>();
            machineState.Add<FreeSpinState>();
            machineState.Add<AutoSpinState>();
            machineState.Add<ReSpinState>();
            machineState.Add<WinState>();
            machineState.Add<JackpotInfoState>();

            SetUpWheelActiveState(machineState);
        }

        public virtual void AddMachineFeatures(MachineFeature machineFeature)
        {
            
        }

        public virtual void SetUpWheelActiveState(MachineState machineState)
        {
            machineState.Add<WheelsActiveState>();
        }

        public abstract void SetUpMachineState(MachineState machineState);


        public virtual void AttachExtraView(MachineContext machineContext)
        {

        }

        public virtual void BindingBackgroundView(MachineContext machineContext)
        {

        }

        public virtual void BindingWheelView(MachineContext machineContext)
        {
        }

        public virtual void BindingJackpotView(MachineContext machineContext)
        {

        }

        public virtual void BindingExtraView(MachineContext machineContext)
        {

        }

        public virtual void AttachControlPanel(MachineContext machineContext)
        {
            var address = "ControlPanel";

            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
                address += "V";

            var controlPanelGameObject = machineContext.assetProvider.InstantiateGameObject(address);
            controlPanelGameObject.transform.SetParent(machineContext.MachineUICanvasTransform, false);
            machineContext.view.Add<ControlPanel>(controlPanelGameObject.transform);
        }

        public virtual void AttachTopPanel(MachineContext machineContext)
        {
            var address = "TopPanel";

            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
                address += "V";

            var topPanelGameObject = machineContext.assetProvider.InstantiateGameObject(address);
            topPanelGameObject.transform.SetParent(machineContext.MachineUICanvasTransform, false);
            machineContext.view.Add<TopPanelView>(topPanelGameObject.transform);
        }

        public virtual void AttachFiveOfKindView(MachineContext machineContext)
        {
            var address = "UISlot5OfAKindNoticeH";
            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
                address = "UISlot5OfAKindNoticeV";

            var fiveOfKindGameObject = machineContext.assetProvider.InstantiateGameObject(address);
            fiveOfKindGameObject.transform.SetParent(machineContext.MachineUICanvasTransform, false);
            fiveOfKindGameObject.transform.SetAsFirstSibling();

            machineContext.view.Add<FiveOfKindView>(fiveOfKindGameObject.transform);
        }


        protected virtual float CalculateMachineScaleFactor(float designAspectRatio, float currentAspectRatio)
        {
            float scale = 1.0f;
            if (currentAspectRatio < designAspectRatio)
            {
                scale = currentAspectRatio / designAspectRatio;
            }
            return scale;
        }

        public async Task AttachSystemWidget(MachineContext machineContext)
        {
            var widgetGameObject = new GameObject("SystemWidget");
            var rectTransform = widgetGameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);

            if (Client.Get<MachineLogicController>().IsPortraitMachine(machineContext.assetProvider.AssetsId))
                rectTransform.sizeDelta = ViewResolution.referenceResolutionPortrait;
            else
            {
                rectTransform.sizeDelta = ViewResolution.referenceResolutionLandscape;
            }

            widgetGameObject.transform.SetParent(machineContext.MachineUICanvasTransform, false);
            widgetGameObject.transform.SetAsFirstSibling();

            //Guide
            var guideView = machineContext.view.Add<GuideMachineWidgetView>(widgetGameObject.transform);
            await guideView.LoadGuideWidgetView();

            var widgetView = machineContext.view.Add<MachineSystemWidgetView>(widgetGameObject.transform);
            await widgetView.LoadContainerView();

            // Activity
            // var activityController = Client.Get<ActivityController>();
            // await activityController.OnAttachSystemWidgets(machineContext, widgetGameObject);
        }

        #region LogicControl
        protected virtual Dictionary<LogicStepType, Type> SetUpLogicStepProxy()
        {
            return new Dictionary<LogicStepType, Type>()
            {
                {LogicStepType.STEP_MACHINE_SETUP, typeof(MachineSetUpProxy)},
                {LogicStepType.STEP_NEXT_SPIN_PREPARE, typeof(NextSpinPrepareProxy)},
                {LogicStepType.STEP_ROUND_START, typeof(RoundStartProxy)},
                {LogicStepType.STEP_SUBROUND_START, typeof(SubRoundStartProxy)},
                {LogicStepType.STEP_WHEEL_SPINNING, typeof(WheelsSpinningProxy)},
                {LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT, typeof(WheelStopSpecialEffectProxy)},
                {LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT, typeof(EarlyHighLevelWinEffectProxy)},
                {LogicStepType.STEP_WIN_LINE_BLINK, typeof(WinLineBlinkProxy)},
                {LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE, typeof(ControlPanelWinUpdateProxy)},
                {LogicStepType.STEP_SPECIAL_BONUS, typeof(SpecialBonusProxy)},
                {LogicStepType.STEP_SPECIAL_BONUS_WIN_NUM_INTERRUPT, typeof(WinNumInterruptProxy)},
                {LogicStepType.STEP_LATE_HIGH_LEVEL_WIN_EFFECT, typeof(LateHighLevelWinEffectProxy)},
                {LogicStepType.STEP_BONUS, typeof(BonusProxy)},
                {LogicStepType.STEP_BONUS_WIN_NUM_INTERRUPT, typeof(WinNumInterruptProxy)},
                {LogicStepType.STEP_RE_SPIN, typeof(ReSpinLogicProxy)},
                {LogicStepType.STEP_PRE_ROUND_END_WIN_NUM_INTERRUPT, typeof(PreRoundEndNumInterruptProxy)},
                {LogicStepType.STEP_SUBROUND_FINISH, typeof(SubRoundFinishProxy)},
                {LogicStepType.STEP_FREE_GAME, typeof(FreeGameProxy)},
                {LogicStepType.STEP_ROUND_FINISH, typeof(RoundFinishProxy)},
            };
        }

        public Dictionary<LogicStepType, LogicStepProxy> InstantiateLogicProxy(MachineContext context)
        {
            var proxyTypeDict = SetUpLogicStepProxy();

            var proxyInstanceDict = new Dictionary<LogicStepType, LogicStepProxy>();

            foreach (var proxyType in proxyTypeDict)
            {
                var constructor = proxyType.Value.GetConstructor(new[] { typeof(MachineContext) });
                if (constructor != null)
                {
                    proxyInstanceDict.Add(proxyType.Key, (LogicStepProxy)constructor.Invoke(new object[] { context }));
                }
            }

            return proxyInstanceDict;
        }

        public FlowController BuildLogicStepFlow(IFlowControlContext context)
        {
            return new ChainFlowController().Add(
                new LogicStep(context, LogicStepType.STEP_MACHINE_SETUP),
                new LoopFlowController().Add(
                    new LogicStep(context, LogicStepType.STEP_NEXT_SPIN_PREPARE),
                    new LogicStep(context, LogicStepType.STEP_ROUND_START),
                    new LoopFlowController().Add(
                        new LoopFlowController().Add(
                            new LogicStep(context, LogicStepType.STEP_SUBROUND_START),
                            new LogicStep(context, LogicStepType.STEP_WHEEL_SPINNING),
                            new LogicStep(context, LogicStepType.STEP_WHEEL_STOP_SPECIAL_EFFECT),
                            new LogicStep(context, LogicStepType.STEP_EARLY_HIGH_LEVEL_WIN_EFFECT),
                            new LogicStep(context, LogicStepType.STEP_WIN_LINE_BLINK),
                            new LogicStep(context, LogicStepType.STEP_CONTROL_PANEL_WIN_UPDATE),
                            new LogicStep(context, LogicStepType.STEP_SPECIAL_BONUS),
                            new LogicStep(context, LogicStepType.STEP_SPECIAL_BONUS_WIN_NUM_INTERRUPT),
                            new LogicStep(context, LogicStepType.STEP_LATE_HIGH_LEVEL_WIN_EFFECT),
                            new LogicStep(context, LogicStepType.STEP_BONUS),
                            new LogicStep(context, LogicStepType.STEP_BONUS_WIN_NUM_INTERRUPT),
                            new LogicStep(context, LogicStepType.STEP_RE_SPIN),
                            new LogicStep(context, LogicStepType.STEP_PRE_ROUND_END_WIN_NUM_INTERRUPT),
                            new LogicStep(context, LogicStepType.STEP_SUBROUND_FINISH)
                        ),
                        new LogicStep(context, LogicStepType.STEP_FREE_GAME)
                    ),
                    new LogicStep(context, LogicStepType.STEP_ROUND_FINISH)
                )
            );
        }

        #endregion
    }
}