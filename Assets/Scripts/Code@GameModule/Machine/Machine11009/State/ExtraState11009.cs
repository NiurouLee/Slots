using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using GameModule;
using GameModule.Utility;

namespace GameModule
{
	public class ExtraState11009 : ExtraState<PharaohTreasureGameResultExtraInfo>
	{

		
		
		public ExtraState11009(MachineState state)
		:base(state)
		{

	
		}


		public override bool HasSpecialEffectWhenWheelStop()
		{
			return true;
		}


		/// <summary>
		/// 宝箱是否被激活
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool IsBoxActivated(int index)
		{
			return extraInfo.FeatureStates[index].Activated;
		}
		
		/// <summary>
		/// 宝箱上次是否被激活
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool IsLastBoxActivated(int index)
		{
			return lastExtraInfo?.FeatureStates[index].Activated ?? false;
		}
		

		/// <summary>
		/// 宝箱是否全满
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool IsBoxExaggerated(int index)
		{
			return extraInfo.FeatureStates[index].Exaggerated;
		}


		public bool IsAllBoxActivate()
		{
			for (int i = 0; i < 3; i++)
			{
				if (!IsBoxActivated(i))
				{
					return false;
				}
			}

			return true;
		}

		public bool HasBoxActivate()
		{
			for (int i = 0; i < 3; i++)
			{
				if (IsBoxActivated(i))
				{
					return true;
				}
			}

			return false;
		}


		public RepeatedField<bool> GetFreeGameType()
		{
			
			return  extraInfo.FreeGameType;
		}

		
		public bool GetFreeGreenState()
		{
			return extraInfo.FreeGameType[Constant11009.IndexGreen];
		}
		
		public bool GetFreeRedState()
		{
			return extraInfo.FreeGameType[Constant11009.IndexRed];
		}
		
		public bool GetFreePurpleState()
		{
			return extraInfo.FreeGameType[Constant11009.IndexPurple];
		}

		public PharaohTreasureGameResultExtraInfo.Types.JackpotWord GetJackptWord(int jackpotId)
		{
			return extraInfo.JackpotWords[jackpotId - 1];
		}


		public MapField<uint,int> GetWildMapInfo()
		{
			return extraInfo.GreenReelPositionMap;
		}

	}
}