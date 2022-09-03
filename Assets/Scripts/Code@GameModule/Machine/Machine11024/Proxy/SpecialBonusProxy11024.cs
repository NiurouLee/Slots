using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class SpecialBonusProxy11024 : SpecialBonusProxy
	{
		private MapView11024 _map;
		public MapView11024 map
		{
			get
			{
				if (_map == null)
				{
					_map =  machineContext.view.Get<MapView11024>();
				}
				return _map;
			}
		}
		private WheelsActiveState11024 _activeWheelState;
		public WheelsActiveState11024 activeWheelState
		{
			get
			{
				if (_activeWheelState == null)
				{
					_activeWheelState =  machineContext.state.Get<WheelsActiveState11024>();
				}
				return _activeWheelState;
			}
		}
		private ExtraState11024 _extraState;
		public ExtraState11024 extraState
		{
			get
			{
				if (_extraState == null)
				{
					_extraState =  machineContext.state.Get<ExtraState11024>();
				}
				return _extraState;
			}
		}

		private Wheel _baseWheel;

		public Wheel baseWheel
		{
			get
			{
				if (_baseWheel == null)
				{
					_baseWheel = machineContext.view.Get<Wheel>("WheelBaseGame");
				}
				return _baseWheel;
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
		
		private PigGroupView11024 _pigGroup;

		public PigGroupView11024 pigGroup
		{
			get
			{
				if (_pigGroup == null)
				{
					_pigGroup = machineContext.view.Get<PigGroupView11024>();
				}
				return _pigGroup;
			}
		}
		public SpecialBonusProxy11024(MachineContext context)
		:base(context)
		{

	
		}

		protected override void HandleCustomLogic()
		{
			HandleCustomLogicAsync();
		}
		public async void HandleCustomLogicAsync()
		{
			if (extraState.HasBonus())
			{
				if (!map.isOpen)
				{
					await map.OpenMap();
				}
				await extraState.SendBonusProcess();
				collectBar.InitState();
				await machineContext.WaitSeconds(1);
				await map.MoveToNextLevel();
				await machineContext.WaitSeconds(1);
				if (Constant11024.IsBigPoint(map.nowLevel))
				{
				}
				else
				{
					var winCoins = extraState.GetMapData().TotalWinHistory[map.nowLevel-1];
					var audioName = "Symbol_SmallWin_" + machineContext.assetProvider.AssetsId;
					AddWinChipsToControlPanel(winCoins,0.5f,true,false,audioName);
					await machineContext.WaitSeconds(1.5f);
					await map.CloseMap();
					machineContext.view.Get<MachineSystemWidgetView>().Show();
					AudioUtil.Instance.UnPauseMusic();
					machineContext.view.Get<ControlPanel>().UpdateControlPanelState(false, false);
				}
			}
			Proceed();
		}
		protected override void RegisterInterestedWaitEvent()
		{
			waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
			waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
		}
	}
}