using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using GameModule;

namespace GameModule
{
	public class ExtraState11012 : ExtraState<MoneyLinkGameResultExtraInfo>
	{
		public ExtraState11012(MachineState state)
		:base(state)
		{

	
		}
		
		
		public override bool HasSpecialEffectWhenWheelStop()
		{
			return true;
		}


		public RepeatedField<uint> GetFreeLockDoorIds()
		{
			return this.extraInfo.DoorPositionIds;
		}


		public bool IsBigDoor()
		{
			
			if (extraInfo.DoorPositionIds.Count >= 15)
			{
				return true;
			}
			
			return false;
		}
		
		
		public bool LastIsBigDoor()
		{
			
			if (extraInfo.LastDoorPositionIds.Count >= 15)
			{
				return true;
			}
			
			return false;
		}

		public bool IsLastLinkLock()
		{

			int num = 0;
			for (int i = 0; i < extraInfo.LinkData.Items.Count; i++)
			{
				if (extraInfo.LinkData.Items[i].SymbolId > 0)
				{
					num++;
				}
			}
			
			if (num == 14)
			{
				return true;
			}

			return false;
		}


		public int GetLastLinkLockPosition()
		{
			if (IsLastLinkLock())
			{
				for (int i = 0; i < extraInfo.LinkData.Items.Count; i++)
				{
					if (extraInfo.LinkData.Items[i].SymbolId <= 0)
					{
						return (int)extraInfo.LinkData.Items[i].PositionId;
					}
				}
			}

			return -1;
		}


		public MoneyLinkGameResultExtraInfo.Types.LinkData GetLinkData()
		{
			return extraInfo.LinkData;
		}
		
		
		
		
		
		public MoneyLinkGameResultExtraInfo.Types.LinkData.Types.Item GetLinkJackpotPay(int positionId)
		{

			if (extraInfo.LinkData.Items.Count > 0)
			{
				return extraInfo.LinkData.Items[positionId];
			}
			
			return null;
		}
		
		
		public long GetRespinTotalWin()
		{
			long winNum = 0;
			var linkData = GetLinkData();
			
			if (linkData.FullWinRate > 0)
			{
                
				var jackpotWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips(linkData.FullWinRate);
				winNum += (long)jackpotWin;
			}
                
			var dicLink = extraInfo.LinkData.Items;
			foreach (var linkKV in dicLink)
			{
				
				var itemLink = linkKV;

				
					long linkWin = 0;
					if (itemLink.JackpotId > 0)
					{
						linkWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips((long) itemLink.JackpotPay);
					}
					else
					{
						linkWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips((long)itemLink.WinRate);
					}

					winNum += linkWin;
				

			}

			return winNum;
		}


		

	}
}