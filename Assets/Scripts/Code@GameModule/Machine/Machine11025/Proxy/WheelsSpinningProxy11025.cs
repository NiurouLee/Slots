using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace GameModule
{
	public class WheelsSpinningProxy11025 : WheelsSpinningProxy
	{
		private ExtraState11025 _extraState;
		public ExtraState11025 extraState
		{
			get
			{
				if (_extraState == null)
				{
					_extraState =  machineContext.state.Get<ExtraState11025>();
				}
				return _extraState;
			}
		}
		private FreeSpinState _freeState;
		public FreeSpinState freeState
		{
			get
			{
				if (_freeState == null)
				{
					_freeState =  machineContext.state.Get<FreeSpinState>();
				}
				return _freeState;
			}
		}
		public WheelsSpinningProxy11025(MachineContext context)
		:base(context)
		{

	
		}
		public override void OnSpinResultReceived()
		{
			OnSpinResultReceivedAsync();
		}

		public async void OnSpinResultReceivedAsync()
		{
			var percent = 100;
			if (freeState.IsTriggerFreeSpin && Random.Range(0,100) < percent)
			{
				AudioUtil.Instance.PlayAudioFx("B01_Except");
				var freeWinTrans = machineContext.assetProvider.InstantiateGameObject("TransitionAnimation");
				// var sortingGroup = freeWinTrans.AddComponent<SortingGroup>();
				// sortingGroup.sortingLayerID = SortingLayer.NameToID("UI");
				// sortingGroup.sortingOrder = 5000;
				freeWinTrans.transform.SetParent(machineContext.transform);
				freeWinTrans.SetActive(true);
				await XUtility.PlayAnimationAsync(freeWinTrans.GetComponent<Animator>(), "TransitionAnimation");
				GameObject.Destroy(freeWinTrans);	
			}

			await machineContext.WaitSeconds(0.5f);
			base.OnSpinResultReceived();
		}
	}
}