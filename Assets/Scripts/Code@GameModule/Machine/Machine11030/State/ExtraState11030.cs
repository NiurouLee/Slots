using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class ExtraState11030 : ExtraState<GoldRushTrainGameResultExtraInfo>
	{
		public ExtraState11030(MachineState state)
		:base(state)
		{
			
		}

		public bool IsInChoose()
		{
			return !extraInfo.Chosen;
		}
		
		public uint GetFreeSpinCount()
		{
			return extraInfo.FreeSpinCount;
		}
		public override bool IsBlinkFeatureTriggered(uint elementId)
		{
			if (Constant11030.ScatterList.Contains(elementId))
			{
				if (IsInChoose())
					return true;
				if (machineState.Get<FreeSpinState11030>().NewCount > 0)
					return true;
			}
			return false;
		}
		public List<int> GetStickyColumnsIndex()
		{
			var stickyColumnsIndexList = new List<int>();
			for (var i = 0; i < extraInfo.StickyColumns.Count; i++)
			{
				stickyColumnsIndexList.Add((int) extraInfo.StickyColumns[i].X);
			}
			return stickyColumnsIndexList;
		}

		public bool IsInTrain()
		{
			return extraInfo.IsTrain;
		}

		public bool IsTrainFromChoose()
		{
			return extraInfo.IsChosenTrain;
		}

		public bool IsSelectLinePanelNeedInit()
		{
			return !extraInfo.IsPanelReady;
		}

		public ulong GetNowCoinAndTrainBaseWinRate()
		{
			return extraInfo.WinLinePay;
		}
		
		public ulong GetNowTrainBaseWinRate()
		{
			return extraInfo.WinLinePay;
		}
		
		public GoldRushTrainGameResultExtraInfo.Types.Train GetNextRunningTrain()
		{
			var trains = extraInfo.Trains;
			for (var i = 0; i < trains.Count; i++)
			{
				var train = trains[i];
				if (!train.Over && train.Id != 14)
					return train;
			}
			return null;
		}
		public GoldRushTrainGameResultExtraInfo.Types.Train GetLastGoldTrain()
		{
			var trains = extraInfo.Trains;
			if (trains.Count > 0)
			{
				var train = trains.Last();
				if (!train.Over && train.Id == 14)
				{
					return train;
				}
			}
			return null;
		}

		public List<GoldRushTrainGameResultExtraInfo.Types.Train> GetCompleteRunningTrains()
		{
			var trains = extraInfo.Trains;
			var completeRunningTrains = new List<GoldRushTrainGameResultExtraInfo.Types.Train>();
			for (var i = 0; i < trains.Count; i++)
			{
				var train = trains[i];
				if (train.Over && train.Id != 14)
					completeRunningTrains.Add(train);
			}
			return completeRunningTrains;
		}
		public Panel GetChooseTriggeringPanel()
		{
			return extraInfo.ChooseTriggeringPanels[0];
		}
		
		public Panel GetLineTriggeringPanel()
		{
			return extraInfo.TrainTriggeringPanels[0];
		}
		
		public override bool HasBonusGame()
		{
			return IsInTrain()||IsInChoose();
		}
	}
}