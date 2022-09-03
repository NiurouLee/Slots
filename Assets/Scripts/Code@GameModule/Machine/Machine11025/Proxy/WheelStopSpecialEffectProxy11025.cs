using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
	public class WheelStopSpecialEffectProxy11025 : WheelStopSpecialEffectProxy
	{
		public WheelStopSpecialEffectProxy11025(MachineContext machineContext)
		:base(machineContext)
		{

	
		}
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
		private WheelBase11025 _baseWheel;
		public WheelBase11025 baseWheel
		{
			get
			{
				if (_baseWheel == null)
				{
					_baseWheel =  machineContext.view.Get<WheelBase11025>();
				}
				return _baseWheel;
			}
		}
		private WheelsActiveState11025 _wheelsActiveState;
		public WheelsActiveState11025 wheelsActiveState
		{
			get
			{
				if (_wheelsActiveState == null)
				{
					_wheelsActiveState =  machineContext.state.Get<WheelsActiveState11025>();
				}
				return _wheelsActiveState;
			}
		}

		protected override void HandleCustomLogic()
		{
			HandleCustomLogicAsync();
		}

		public async void HandleCustomLogicAsync()
		{
			var runningWheel = (Wheel11025)wheelsActiveState.GetRunningWheel()[0];
			if (runningWheel.wheelName == Constant11025.WheelBaseGameName)
			{
				if (extraState.GetFlowerList().Count > 0)
				{
					await ((WheelBase11025)runningWheel).FlyAllFlowerOnWheel();
				}
				var normalData = extraState.GetNowNormalData();
				var cleanList = normalData.FailedIndexes;
				var fullList = normalData.FullIndexes;
				//收集花
				var waitFlag = ((WheelBase11025)runningWheel).UpdateChameleonState(normalData.StickyColumnReSpinCounts);
				for (var i = 0; i < fullList.Count; i++)
				{
					var rollIndex = (int) fullList[i];
					baseWheel.CleanFrameRoll(rollIndex);
				}
				if (waitFlag || fullList.Count > 0)
				{
					await machineContext.WaitSeconds(0.5f);
				}
				//更新respin条并清掉集满列的线框
				var taskList = new List<Task>();
				taskList.Add( ((WheelBase11025)runningWheel).UpdateNormalDataPanel(normalData.StickyItems,null,cleanList,false,true));
				taskList.Add(PerformBaseCollect(fullList));
				await Task.WhenAll(taskList);
				//更新stickyItem
				//表演集满列的收集
				//集满列的分数收集到地上
			}
			else if (runningWheel.wheelName == Constant11025.WheelFreeGameName)
			{
				var freeData = extraState.GetFreeData();
				var fullList = freeData.NewFullIndexes;
				await ((WheelFree11025)runningWheel).UpdateNormalDataPanel(freeData.StickyItems);
				if (fullList.Count > 0)
				{
					await machineContext.WaitSeconds(1f);
				}
				for (var i = 0; i < fullList.Count; i++)
				{
					var rollIndex = (int) fullList[i];
					await PerformFreeMultiRoll(rollIndex,(int)freeData.Multipliers[rollIndex]);
				}
			}
			
			base.HandleCustomLogic();
		}

		public async Task PerformFreeMultiRoll(int rollIndex,int result)
		{
			var multiWheel = machineContext.view.Get<FreeMultiWheel11025>();
			var rollHeight = Constant11025.RollHeightList[rollIndex];
			var freeWheel = machineContext.view.Get<WheelFree11025>();
			var elementList = new List<StickyElementContainer11025>();
			var stickyDictionary = freeWheel.StickyMap[rollIndex];
			for (var i = 0; i < rollHeight; i++)
			{
				elementList.Add(stickyDictionary[(uint)i]);
			}
			await multiWheel.StartSpin(elementList, result);
			var stickyFrame = freeWheel.FrameDataMap[rollIndex][0];
			stickyFrame.SetLength(stickyFrame.length,true);
		}

		public async Task PerformBaseCollect(RepeatedField<uint> fullList)
		{
			for (var i = 0; i < fullList.Count; i++)
			{
				var rollIndex = (int) fullList[i];
				await PerformBaseCollectRoll(rollIndex);
			}
			for (var i = 0; i < fullList.Count; i++)
			{
				var rollIndex = (int) fullList[i];
				await CollectToFloor(rollIndex);
			}
		}
		public async Task PerformBaseCollectRoll(int rollIndex)
        {
            var nowNormalData = extraState.GetNowNormalData();
            var rollItemList = new Dictionary<int,ChameleonGameResultExtraInfo.Types.StickyItem>();
            for (var i = 0; i < nowNormalData.StickyItems.Count; i++)
            {
                var item = nowNormalData.StickyItems[i];
                if (item.X == rollIndex)
                {
                    rollItemList[(int) item.Y] = item;
                }
            }

            await baseWheel.PrepareCollectRoll(rollIndex);
            await machineContext.WaitSeconds(0.5f);
            var rollHeight = Constant11025.RollHeightList[rollIndex];
            bool openMouthFlag = true;
            var collectCount = 0;
            for (var i = rollHeight - 1; i >= 0; i--)
            {
                if (rollItemList[i].WinRate > 0)
                {
	                if (openMouthFlag)
	                {
		                openMouthFlag = false;
		                baseWheel.ChameleonPrepareCollectCoin(rollIndex);
	                }
	                collectCount++;
	                var chips = (long)machineContext.state.Get<BetState>().GetPayWinChips(rollItemList[i].WinRate);
                    baseWheel.ChameleonCollectCoin(rollIndex,i,chips,collectCount);
                    await machineContext.WaitSeconds(Constant11025.EatCoinInterval);
                }
            }
            await machineContext.WaitSeconds(0.5f);
            for (var i = rollHeight - 1; i >= 0; i--)
            {
                if (rollItemList[i].JackpotPay > 0)
                {
	                collectCount++;
	                var chips = (long)machineContext.state.Get<BetState>().GetPayWinChips(rollItemList[i].JackpotPay);
	                await baseWheel.ChameleonCollectJackpot(rollIndex, i, chips,collectCount);
	                await machineContext.WaitSeconds(0.5f);
	                await ShowJackpotPopup<UIJackpotBase11025>((int) rollItemList[i].JackpotId, chips);
                }
            }

            await machineContext.WaitSeconds(0.5f);
            await baseWheel.FinishCollectRoll(rollIndex);
        }

		public TaskCompletionSource<bool> niceWinTask;
		public async Task CollectToFloor(int rollIndex)
		{
			var collectValue = baseWheel.GetCollectValue(rollIndex);
			machineContext.state.Get<WinState>().wheelWin -= (ulong)collectValue;
			await baseWheel.CollectToFloor(rollIndex);
			var winLevel = AddWinChipsToControlPanel((ulong) collectValue, 0, true, true);
			
			var effectDuration = 0.5f;

			if (winLevel != WinLevel.NoWin)
			{
				if (winLevel == WinLevel.SmallWin)
				{
					effectDuration = 1.0f;
				}
				else if (winLevel == WinLevel.Win)
				{
					effectDuration = 2.0f;
				}
				else if (winLevel == WinLevel.NiceWin)
				{
					machineContext.view.Get<ControlPanel>().ShowStopButton(true);
					effectDuration = 6;
				}
			}
			niceWinTask = GetWaitTask();
			await niceWinTask.Task;
			machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
		}
		protected virtual async Task ShowJackpotPopup<T>(int jpType, long chips) where T : UIJackpotBase11025
		{
			string address = "UIJackpot" + Constant11025.JackpotName[jpType] + machineContext.assetProvider.AssetsId;

			if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
			{
				XDebug.LogError($"ShowJackmpotPopUp:{address} is Not Exist");
				return;
			}

			TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
			machineContext.AddWaitTask(waitTask, null);
			AudioUtil.Instance.PlayAudioFx("Jackpot");
			var startPopUp = PopUpManager.Instance.ShowPopUp<T>(address);
			startPopUp.SetJackpotWinNum((ulong) chips);
			startPopUp.SetPopUpCloseAction(() =>
			{
				machineContext.RemoveTask(waitTask);
				waitTask.SetResult(true);
			});

			await waitTask.Task;
		}
		
		public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
		{
			switch (internalEvent)
			{
				case MachineInternalEvent.EVENT_CONTROL_STOP:
					machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
					machineContext.view.Get<ControlPanel>().StopWinAnimation();
					break;
				case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
				{
					var waitEvent = (WaitEvent) args[0];
					if (waitEvent == WaitEvent.WAIT_WIN_NUM_ANIMTION)
					{
						machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
						niceWinTask.SetResult(true);
						niceWinTask = null;
					}
					break;
				}
			}
			base.OnMachineInternalEvent(internalEvent, args);
		}
	}
}