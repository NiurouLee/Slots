using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;

namespace GameModule
{
	public class ExtraState11019 : ExtraState<FireLinkByTheBayGameResultExtraInfo>
	{
		public ExtraState11019(MachineState state)
		:base(state)
		{

	
		}
		
		
		public override bool HasSpecialEffectWhenWheelStop()
		{
			return true;
		}



		public FireLinkByTheBayGameResultExtraInfo.Types.LinkData GetLinkData()
		{
			return extraInfo.LinkData;
		}


		public FireLinkByTheBayGameResultExtraInfo.Types.LinkData.Types.Item GetLinkJackpotPay(int positionId)
		{

			if (extraInfo.LinkData.Items.Count > 0)
			{
				return extraInfo.LinkData.Items[positionId];
			}
			
			return null;
		}

		public bool LinkIsUnLock(uint positionId)
		{
			// foreach (var linkDataItem in extraInfo.LinkData.Items)
			// {
			// 	if (linkDataItem.PositionId == positionId)
			// 	{
			// 		
			// 		return true;
			// 	}
			// }
			if (8-positionId%8 <= extraInfo.LinkHeight)
			{
				return true;
			}


			return false;
		}
		
		


		public long GetRespinTotalWin()
		{
			long winNum = 0;
			var linkData = GetLinkData();
                
			var dicLink = extraInfo.LinkData.Items;
			foreach (var linkKV in dicLink)
			{
				
				var itemLink = linkKV;

				if (LinkIsUnLock(itemLink.PositionId))
				{
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

			}

			return winNum;
		}


		public int GetPepperCount()
		{
			return (int)extraInfo.PepperCount;
		}

		public int GetLastPepperCount()
		{
			return lastExtraInfo!=null ? (int)lastExtraInfo.PepperCount : -1;
		}

	}
}