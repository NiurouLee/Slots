using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class SubRoundStartProxy11024 : SubRoundStartProxy
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
		public SubRoundStartProxy11024(MachineContext context)
		:base(context)
		{

	
		}
		protected override void HandleCustomLogic()
		{
			HandleCustomLogicAsync();
		}

		public async void HandleCustomLogicAsync()
		{
			if (activeWheelState.gameType == GameType11024.Free)
			{
				var wheelList = activeWheelState.GetRunningWheel();
				for (var i = 0; i < wheelList.Count; i++)
				{
					((WheelFree11024) wheelList[i]).CloseWildCover();
				}
			}
			else if (activeWheelState.gameType == GameType11024.Link)
			{
				var wheelList = activeWheelState.GetRunningWheel();
				for (var i = 0; i < wheelList.Count; i++)
				{
					((WheelLink11024) wheelList[i]).ReduceLeftSpinTimes();
				}
			}
			Proceed();
		}
		
		protected override void PlayBgMusic()
		{
			if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
			{
				AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
			}
			else if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
			{
				AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
			}
			else
			{
				AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
			}
		}
	}
}