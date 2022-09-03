using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using GameModule;

namespace GameModule
{
	public class ExtraState11301 : ExtraState<CatInBootsGameResultExtraInfo>
	{
		public ExtraState11301(MachineState state)
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

		public CatInBootsGameResultExtraInfo.Types.LinkData GetLinkData()
		{
			return extraInfo.LinkData;
		}
		
		public CatInBootsGameResultExtraInfo.Types.LinkData.Types.Item GetLinkJackpotPay(int positionId)
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


		//------------商城----------------------
		/// <summary>
		/// 每次spin时，有可能出现代币。map key:位置1-15, value:数量
		/// </summary>
		/// <returns></returns>
		public MapField<uint, uint> GetAttachItems(){
			return extraInfo.AttachItems;
		}
		/// <summary>
		/// 获取当前剩余代币
		/// </summary>
		/// <returns></returns>
		public uint GetCollectItems(){
			return extraInfo.CollectItems;
		}
		/// <summary>
		/// 当前特殊玩法结束时，判断是否需要恢复商城
		/// </summary>
		/// <returns></returns>
		public bool IsMapFeature(){
			return extraInfo.IsMapFeature;
		}
		/// <summary>
		/// 格子是否全部打开
		/// </summary>
		/// <returns></returns>
		public bool IsAllOpened(){
			return extraInfo.AllOpened;
		}
		/// <summary>
		/// 获取平均下注信息，用于显示商城
		/// </summary>
		/// <returns></returns>
		public CatInBootsGameResultExtraInfo.Types.AvgBetData GetAvgBet(){
			return extraInfo.AvgBet;
		}
		// 获取当前商城信息
		public RepeatedField<CatInBootsGameResultExtraInfo.Types.RoleData> GetRolesData(){
			return extraInfo.Roles;
		}
		public RepeatedField<uint> SuperFreePattern;
		public async void ChooseBoxRequest(uint choose,uint roleId, System.Action<bool,SBonusProcess>requestCallback)
		{
			CatInBootsBonusGameRequest req = new CatInBootsBonusGameRequest();
			req.RoleId = roleId;
			req.Choose = choose;

			var sBonusProcess = await SendBonusProcess<CatInBootsBonusGameRequest>(req);

			var ok = sBonusProcess != null && sBonusProcess.GameResult != null;

			requestCallback?.Invoke(ok,sBonusProcess);
		}
	}
}