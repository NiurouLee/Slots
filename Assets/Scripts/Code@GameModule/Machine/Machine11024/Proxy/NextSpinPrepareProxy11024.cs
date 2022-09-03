using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
    public class NextSpinPrepareProxy11024 : NextSpinPrepareProxy
    {
        private BetState _betState;

        public BetState betState
        {
            get
            {
                if (_betState == null)
                {
                    _betState = machineContext.state.Get<BetState>();
                }

                return _betState;
            }
        }

        private MapView11024 _mapView;

        public MapView11024 mapView
        {
            get
            {
                if (_mapView == null)
                {
                    _mapView = machineContext.view.Get<MapView11024>();
                }

                return _mapView;
            }
        }

        private CollectBarView11024 _collectBar;

        public CollectBarView11024 collectBar
        {
            get
            {
                if (_collectBar == null)
                {
                    _collectBar = machineContext.view.Get<CollectBarView11024>();
                }

                return _collectBar;
            }
        }

        private WheelsActiveState11024 _activeWheelState;

        public WheelsActiveState11024 activeWheelState
        {
            get
            {
                if (_activeWheelState == null)
                {
                    _activeWheelState = machineContext.state.Get<WheelsActiveState11024>();
                }

                return _activeWheelState;
            }
        }

        public NextSpinPrepareProxy11024(MachineContext context)
            : base(context)
        {
        }

        protected override void OnUnlockBetFeatureConfigChanged()
        {
            collectBar.RefreshShopLockState();
        }

        protected override void OnBetChange()
        {
            base.OnBetChange();
            collectBar.RefreshShopLockState();
            var activeWheel = activeWheelState.GetRunningWheel()[0];
            for (var i = 0; i < 5; i++)
            {
                var roll = activeWheel.GetRoll(i);
                for (var i1 = 0; i1 < 3; i1++)
                {
                    var container = roll.GetVisibleContainer(i1);
                    if (Constant11024.IsValueId(container.sequenceElement.config.id))
                    {
                        var element = (ElementValue11024) container.GetElement();
                        element.UpdateElementContent();
                    }
                }
            }
        }

        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            base.OnMachineInternalEvent(internalEvent, args);
            if (internalEvent == MachineInternalEvent.EVENT_CUSTOM_CLICK &&
                !machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                var btnType = (BtnType11024) args[0];
                if (btnType == BtnType11024.CloseMap)
                {
                    DealAboutCloseMapBtn();
                }
                else if (btnType == BtnType11024.OpenMap)
                {
                    DealAboutOpenMapBtn();
                }
            }
        }

        public async void DealAboutCloseMapBtn()
        {
            if (mapView.isOpen)
            {
                await mapView.CloseMap();
                machineContext.view.Get<MachineSystemWidgetView>().Show();
                AudioUtil.Instance.UnPauseMusic();
                machineContext.view.Get<ControlPanel>().UpdateControlPanelState(false, false);
                machineContext.view.Get<ControlPanel>().ShowSpinButton(true);
            }
        }

        public async void DealAboutOpenMapBtn()
        {
            if (betState.IsFeatureUnlocked(0))
            {
                if (!mapView.isOpen)
                {
                    machineContext.view.Get<ControlPanel>().StopWinAnimation(false, 0);
                    mapView.InitState();
                    await mapView.OpenMap();
                    mapView.SetBtnEnable(true);
                }
            }
            else
            {
                machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, 0);
            }
        }
    }
}