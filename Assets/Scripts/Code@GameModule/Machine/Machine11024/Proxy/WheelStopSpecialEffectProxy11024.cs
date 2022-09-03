using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using GameModule;
using UnityEngine.Rendering;

namespace GameModule
{
	public class WheelStopSpecialEffectProxy11024 : WheelStopSpecialEffectProxy
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

		// private float collectS01Time = 0.5f;
		private float collectPigCoinTime = Constant11024.CollectPigCoinTime;
		public WheelStopSpecialEffectProxy11024(MachineContext machineContext)
		:base(machineContext)
		{

	
		}

		protected override void HandleCustomLogic()
		{
			HandleCustomLogicAsync();
		}

		public async void HandleCustomLogicAsync()
		{
			if (activeWheelState.gameType == GameType11024.Base)
			{
				if (!collectBar.IsLocked())
				{
					await CollectS01();
				}
				await CollectPigCoin();
			}
			else if (activeWheelState.gameType == GameType11024.Free)
			{
				var taskList = new List<Task>();
				var wheelList = activeWheelState.GetRunningWheel();
				AudioUtil.Instance.PlayAudioFx("Map_WildOpen");
				for (var i = 0; i < wheelList.Count; i++)
				{
					taskList.Add(((WheelFree11024) wheelList[i]).OpenWildCover());
				}
				await Task.WhenAll(taskList);
			}
			else if (activeWheelState.gameType == GameType11024.Link)
			{
				await PerformBoost();
			}
			Proceed();
		}

		public async Task CollectS01()
		{
			var containerList = new List<ElementContainer>();
			for (var i = 0; i < baseWheel.rollCount; i++)
			{
				var roll = baseWheel.GetRoll(i);
				for (var i1 = 0; i1 < roll.rowCount; i1++)
				{
					var container = roll.GetVisibleContainer(i1);
					if (Constant11024.IsS01Id(container.sequenceElement.config.id))
					{
						containerList.Add(container);
					}
				}
			}

			if (containerList.Count > 0)
			{
				for (var i = 0; i < containerList.Count; i++)
				{
					var container = containerList[i];
					var flyObject = machineContext.assetProvider.InstantiateGameObject("Fly_S01", true);
					var sortingGroup = flyObject.GetComponent<SortingGroup>();
					sortingGroup.sortingLayerName = "LocalUI";
					sortingGroup.sortingOrder = 1;
					flyObject.transform.SetParent(baseWheel.transform,false);
					flyObject.transform.position = container.transform.position;
					var targetPosition = collectBar.GetCollectPosition();
					var targetLocalPosition = flyObject.transform.parent.InverseTransformPoint(targetPosition);
					flyObject.transform.DOLocalMove(targetLocalPosition, Constant11024.CollectS01Time).OnComplete(() =>
					{
						machineContext.assetProvider.RecycleGameObject("Fly_S01", flyObject);
					});
				}

				AudioUtil.Instance.PlayAudioFx("Map_S01Fly");
				await machineContext.WaitSeconds(Constant11024.CollectS01Time);
				AudioUtil.Instance.PlayAudioFx("Map_S01FlyStop");
				await collectBar.PerformBarAdd();
			}
		}

		public async Task CollectPigCoin()
		{
			var hasCollect = false;
			var taskList = new List<Task>();
			for (var pigType = 0; pigType < 3; pigType++)
			{
				var containerList = new List<ElementContainer>();
				for (var i = 0; i < baseWheel.rollCount; i++)
				{
					var roll = baseWheel.GetRoll(i);
					for (var i1 = 0; i1 < roll.rowCount; i1++)
					{
						var container = roll.GetVisibleContainer(i1);
						if (Constant11024.IsPigId(container.sequenceElement.config.id, pigType))
						{
							containerList.Add(container);
						}
					}
				}

				if (containerList.Count > 0)
				{
					hasCollect = true;
					var localPigType = pigType;
					for (var i = 0; i < containerList.Count; i++)
					{
						var container = containerList[i];
						var nameList = new List<string>
						{
							"Purple",
							"Red",
							"Green",
						};
						var assetName = "ActiveFly" + nameList[localPigType];
						container.PlayElementAnimation("Fly");
						machineContext.WaitSeconds(Constant11024.ElementFlyBeforeCollectPigCoinTime, () =>
						{
							var flyObject = machineContext.assetProvider.InstantiateGameObject(assetName, true);
							flyObject.SetActive(false);
							flyObject.SetActive(true);
							var sortingGroup = flyObject.GetComponent<SortingGroup>();
							sortingGroup.sortingLayerName = "LocalUI";
							sortingGroup.sortingOrder = 1;
							flyObject.transform.SetParent(baseWheel.transform,false);
							flyObject.transform.position = container.transform.position;
							var startLocalPosition = flyObject.transform.localPosition;
							var targetPosition = pigGroup.GetCollectPosition(localPigType);
							var targetLocalPosition = flyObject.transform.parent.InverseTransformPoint(targetPosition);
							var midLocalPos = (startLocalPosition + targetLocalPosition) / 2;
							midLocalPos.y += (targetLocalPosition.y - startLocalPosition.y);
							// Vector3[] wayPoints = new[] {startLocalPosition, midLocalPos, targetLocalPosition};
							// flyObject.transform.DOLocalPath(wayPoints, collectPigCoinTime, PathType.CatmullRom,
							// 		PathMode.Full3D, 10)
							// 	.SetEase(Ease.InOutQuad).OnComplete(() =>
							// 	{
							// 		machineContext.assetProvider.RecycleGameObject(assetName, flyObject);
							// 	});

							Vector3 yProgress = Vector3.zero;
							Vector3 lowYProgress = Vector3.zero;
							Vector3 highYProgress = new Vector3(0, (targetLocalPosition.y - startLocalPosition.y)/2, 0);
							Vector3 lineProgress = startLocalPosition;
							var sequence = DOTween.Sequence();
							sequence.Append(DOTween.To(() => lowYProgress,
								setter =>
								{
									yProgress = setter;
								}, highYProgress, collectPigCoinTime / 2).SetEase(
								Ease.OutSine//OutQuad
								));
							sequence.Append(DOTween.To(() => highYProgress,
								setter =>
								{
									yProgress = setter;
								}, lowYProgress, collectPigCoinTime / 2).SetEase(
								Ease.InSine//Linear
								));
							sequence.Insert(0, DOTween.To(() => startLocalPosition, setter =>
							{
								lineProgress = setter;
								var nowLocalPosition = lineProgress + yProgress;
								flyObject.transform.localPosition = nowLocalPosition;
							}, targetLocalPosition, collectPigCoinTime).SetEase(Ease.Linear));
							sequence.AppendCallback(() =>
							{
								machineContext.assetProvider.RecycleGameObject(assetName, flyObject);
							});
							sequence.target = machineContext.transform;
						});
					}
					var task = CollectCoinTask(localPigType);
					// var targetType = (int) extraState.GetPigCollectLevel(localPigType);
					// if (targetType == 3)
						taskList.Add(task);
				}	
			}
			
			if (hasCollect)
			{
				var glowFlag = false;
				for (var i = 0; i < 3; i++)
				{
					if (pigGroup.NeedGlow(i))
					{
						glowFlag = true;
						break;
					}
				}
				AudioUtil.Instance.PlayAudioFx("J0123_Fly");
				await machineContext.WaitSeconds(collectPigCoinTime);
				AudioUtil.Instance.PlayAudioFx("J0123_FlyStop");
				if (glowFlag)
				{
					await machineContext.WaitSeconds(0.533f);
					AudioUtil.Instance.PlayAudioFx("J0123_FlyStopBigger");
				}
				await Task.WhenAll(taskList);
			}
		}
		async Task CollectCoinTask(int localPigType)
		{
			await machineContext.WaitSeconds(Constant11024.ElementFlyBeforeCollectPigCoinTime);
			await machineContext.WaitSeconds(collectPigCoinTime);
			await pigGroup.CollectPigCoin(localPigType);
		}
		public async Task PerformBoost()
		{
			bool darkCoverShow = false;
			var activeWheelList = activeWheelState.GetRunningWheel();
			for (var i = 0; i < activeWheelList.Count; i++)
			{
				var linkWheel = (WheelLink11024)activeWheelList[i];
				var boostData = extraState.GetReSpinBoostData(linkWheel.linkWheelIndex);
				if (boostData.Count > 0)
				{
					darkCoverShow = true;
					break;
				}
			}

			if (darkCoverShow)
			{
				await machineContext.WaitSeconds(0.2f);
				AudioUtil.Instance.PlayAudioFx("LinkGame_Boost");
				for (var i = 0; i < activeWheelList.Count; i++)
				{
					var linkWheel = (WheelLink11024) activeWheelList[i];
					linkWheel.ShowWheelCover(true);
					if (extraState.GetReSpinBoostData(linkWheel.linkWheelIndex).Count > 0)
					{
						linkWheel.ShowBoostCover(true);
					}
				}
				await machineContext.WaitSeconds(2.433f);
				// AudioUtil.Instance.PlayAudioFx("LinkGame_BoostChecked");
				// for (var i = 0; i < activeWheelList.Count; i++)
				// {
				// 	var linkWheel = (WheelLink11024) activeWheelList[i];
				// 	if (extraState.GetReSpinBoostData(linkWheel.linkWheelIndex).Count > 0)
				// 	{
				// 		linkWheel.ShowBoostCover(false);
				// 	}
				// }
				for (var i = 0; i < activeWheelList.Count; i++)
				{
					var linkWheel = (WheelLink11024) activeWheelList[i];
					var boostData = extraState.GetReSpinBoostData(linkWheel.linkWheelIndex);
					for (var i1 = 0; i1 < boostData.Count; i1++)
					{
						var posX = (int) boostData[i1].X;
						var posY = (int) boostData[i1].Y;
						// var rollIndex = boostData[i1].X * 3 + boostData[i1].Y;
						linkWheel.stickyElementList[posX][posY].SetSortingOrderBoost(posX, posY);
					}
				}
				await machineContext.WaitSeconds(1f);
				for (var i = 0; i < activeWheelList.Count; i++)
				{
					var linkWheel = (WheelLink11024)activeWheelList[i];
					var boostData = extraState.GetReSpinBoostData(linkWheel.linkWheelIndex);
					for (var i1 = 0; i1 < boostData.Count; i1++)
					{
						var posX = (int)boostData[i1].X;
						var posY = (int)boostData[i1].Y;
						var rollIndex = posX * 3 + posY;
						var addWinRate = boostData[i1].WinRate;
						var stickyElement = linkWheel.stickyElementList[posX][posY];
						var itemData = linkWheel.GetItemData(rollIndex);
						stickyElement.SetStickyDataWithoutRefresh(itemData);
						await stickyElement.ShowAddAnimation(addWinRate);
					}
				}
				for (var i = 0; i < activeWheelList.Count; i++)
				{
					var linkWheel = (WheelLink11024) activeWheelList[i];
					linkWheel.ShowWheelCover(false);
					var boostData = extraState.GetReSpinBoostData(linkWheel.linkWheelIndex);
					for (var i1 = 0; i1 < boostData.Count; i1++)
					{
						var posX = (int)boostData[i1].X;
						var posY = (int)boostData[i1].Y;
						// var rollIndex = boostData[i1].X * 3 + boostData[i1].Y;
						linkWheel.stickyElementList[posX][posY].SetSortingOrder(posX,posY);
					}
				}
			}
		}
	}
}