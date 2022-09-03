using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class ExtraState11005 : ExtraState<GoldMineGameResultExtraInfo>
	{
		public ExtraState11005(MachineState state)
		:base(state)
		{

	
		}
		
		public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
		{
			base.UpdateStateOnReceiveSpinResult(spinResult);
			isBonusGame = spinResult.GameResult.IsBonusGame;
		}

		public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
		{
			base.UpdateStateOnRoomSetUp(gameEnterInfo);
			isBonusGame = gameEnterInfo.GameResult.IsBonusGame;
		}
		
		public override bool HasSpecialBonus()
		{
			if (!extraInfo.Adventure.Rolled || !extraInfo.Adventure.StepDone)
			{
				return true;
			}

			return false;
		}


		public override bool HasSpecialEffectWhenWheelStop()
		{
			// //收集字母
			// var collection = extraInfo.Collection.Add;
			// for (int i = 0; i < collection.count; i++)
			// {
			// 	if (collection[i] > 0)
			// 	{
			// 		return true;
			// 	}
			// }
			//
			//
			//
			// //收集炸弹
			// if (extraInfo.LuckCountOld != extraInfo.LuckCount)
			// {
			// 	return true;
			// }
			//
			// return false;

			return true;
		}

		public GoldMineGameResultExtraInfo.Types.Collection GetCollection()
		{
			return extraInfo.Collection;
		}


		public bool IsAllLetterCollect()
		{
			var collection = extraInfo.Collection.Now;
			for (int i = 0; i < collection.count; i++)
			{
				if (collection[i] == 0)
				{
					return false;
				}
			}

			return true;
		}
		
		public int GetThisOneGetLetterNum()
		{
			int num = 0;
			var collection = extraInfo.Collection.Add;
			for (int i = 0; i < collection.count; i++)
			{
				num += (int)collection[i];
			}

			return num;
		}
		
		public uint GetLuckCount()
		{
			return extraInfo.LuckCount;
		}

		public uint GetLastLuckCount()
		{
			return extraInfo.LuckCountOld;
		}
		
		public GoldMineGameResultExtraInfo.Types.Adventure GetMapInfo()
		{
			return extraInfo.Adventure;
		}

		public uint GetMoreLetterCount()
		{
			var listNow = extraInfo.Collection.Now;
			uint num = 0;
			for (int i = 0; i < listNow.count; i++)
			{
				num += listNow[i];
			}

			return Math.Max(0,num - 10);
		}

	}
}