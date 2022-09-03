using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
namespace GameModule{
    public class ExtraState11312 : ExtraState<CaptainPenguinGameResultExtraInfo>
    {
        public ExtraState11312(MachineState state) : base(state)
        {
        }
        // base--
        public RepeatedField<uint> RandomS01 => extraInfo.RandomS01;
        public MapField<uint, uint> RandomWild => extraInfo.RandomWild;
        public MapField<uint, uint> RandomScatter => extraInfo.RandomScatter;
        // Free---
        public MapField<uint, uint> PreLockedSymbols => extraInfo.FreeGameInfo.PreLockedSymbols;
        public MapField<uint, uint> NewLockedSymbols => extraInfo.FreeGameInfo.NewLockedSymbols;
        public MapField<uint, uint> AllLastLockedSymbols = new MapField<uint, uint>();

        // link
        // 是否是金币对应的卷轴
        public ulong AddedToReels => extraInfo.ReSpinInfo.AddedToReels;
        public MapField<uint,uint> StartCoins=>extraInfo.ReSpinInfo.StartCoins;
        public MapField<uint, global::DragonU3DSDK.Network.API.ILProtocol.CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.LinkItem> LinkItems => extraInfo.ReSpinInfo.LinkItems;
        //剩下的框
        public RepeatedField<uint> ValidFrames => extraInfo.ReSpinInfo.ValidFrames;
        //已经用掉的框
        public RepeatedField<uint> UsedFrames => extraInfo.ReSpinInfo.UsedFrames;
        //新出现的框
        public RepeatedField<uint> NewFrames => extraInfo.ReSpinInfo.NewFrames;
        public RepeatedField<CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.CoinInFrame>  CoinInFrame => extraInfo.ReSpinInfo.CoinInFrame;
        //小轮盘，转到随机翻倍时用
        public uint RandomPickIndex => extraInfo.ReSpinInfo.RandomPickIndex;
        //轮盘高度
        public uint PanelHeight => extraInfo.ReSpinInfo.PanelHeight;
        //jackpotInfo
        public CaptainPenguinGameResultExtraInfo.Types.ReSpinInfo.Types.JackpotInfo JackpotInfo => extraInfo.ReSpinInfo.JackpotInfo;
        // 当前面板上所有金币的合
        public ulong YellowCoinTotalWinRate=>extraInfo.ReSpinInfo.YellowCoinTotalWinRate;

        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
            RefreshFreeLockedData();
        }
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            RefreshFreeLockedData();
        }
        public void RefreshFreeLockedData(){
            AllLastLockedSymbols = new MapField<uint, uint>();
            // free--刷新数据
            if(PreLockedSymbols!=null && PreLockedSymbols.Count!=0){
                foreach (var item in PreLockedSymbols)
                {
                    if(!AllLastLockedSymbols.ContainsKey(item.Key))
                        AllLastLockedSymbols.Add(item.Key, item.Value);
                }
            }
            if(NewLockedSymbols!=null && NewLockedSymbols.Count!=0){
                foreach (var item in NewLockedSymbols)
                {
                    if(!AllLastLockedSymbols.ContainsKey(item.Key))
                        AllLastLockedSymbols.Add(item.Key, item.Value);
                }
            } 
        } 

        public long GetRespinTotalWin()
		{
			long winNum = 0;
			foreach (var item in LinkItems)
			{	
                var value = item.Value;
                long linkWin = 0;
                if (value.JackpotId > 0)
                {
                    ulong winPay = value.JackpotPay;
                    linkWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips((long)winPay);
                    if(value.WinRate>0){
                        var addExtraWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips((long)value.WinRate);
                        linkWin += addExtraWin;
                    }
                        
                }
                else
                {
                    linkWin = this.machineState.machineContext.state.Get<BetState>().GetPayWinChips((long)value.WinRate);
                }

                winNum += linkWin;
				

			}

			return winNum;
		}      
    }
}

