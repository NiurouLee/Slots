// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/14:09
// Ver : 1.0.0
// Description : ExtraState11001.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class ExtraState11001 : ExtraState<BingoCloverGameResultExtraInfo>
    {
        private BetState _betState;

        private MapField<ulong, BingoCloverGameResultExtraInfo.Types.BingoData> _bingoData;

        public ExtraState11001(MachineState machineState)
            : base(machineState)
        {
        }

        public uint GetPotLevel()
        {
            return extraInfo.Pot.Level;
        }

        public override bool HasSpecialEffectWhenWheelStop()
        {
            return HasNewBingoItemFromPanel();
        }

        public override bool HasSpecialBonus()
        {
            if (IsBonusFreeSpin())
            {
                return extraInfo.SuperBingo.Lines != null
                       && extraInfo.SuperBingo.Lines.Count > 0
                       && extraInfo.SuperBingo.CurrentBonusLine <= extraInfo.SuperBingo.Lines.Count;
            }

            var bingoData = _bingoData[_betState.totalBet];

            return bingoData.Lines != null && bingoData.Lines.Count > 0 &&
                   bingoData.CurrentBonusLine <= bingoData.Lines.Count;
        }
        
        public override bool IsBlinkFeatureTriggered(uint elementId)
        {
            if (elementId == Constant11001.b01)
            {
                return machineState.Get<FreeSpinState>().IsTriggerFreeSpin;
            }
            return false;
        }

        public bool IsBonusFreeSpin()
        {
            return extraInfo.SuperBingo != null
                   && machineState.Get<FreeSpinState>().IsInFreeSpin &&
                   !machineState.Get<FreeSpinState>().IsOver;
        }

        public bool IsSuperBonus()
        {
            return IsBonusFreeSpin() && extraInfo.SuperBingo.IsBlock;
        }
        
        public bool IsMegaBonus()
        {
            return IsBonusFreeSpin() && extraInfo.SuperBingo.IsCross;
        }

        public bool NextIsSuperFreeSpin()
        {
            return extraInfo.SuperBingo != null && extraInfo.SuperBingo.IsOver == false;
        }

        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
            _betState = machineState.Get<BetState>();
            _bingoData = extraInfo.BingoMap;
        }

        public BingoCloverGameResultExtraInfo.Types.BingoData GetBingoData(bool forceSuperBingo = false)
        {
            if (IsBonusFreeSpin()  || forceSuperBingo)
            {
                XDebug.Log("IsSuperFreeSpin:");
               // if (!extraInfo.SuperBingo.IsOver)
                    return extraInfo.SuperBingo;
            }

            var totalBet = _betState.totalBet;
            if (!_bingoData.ContainsKey(totalBet))
            {
                _bingoData.Add(totalBet, CreateBingoData());
            }
            
            XDebug.Log("NormalBingo:");
            return _bingoData[totalBet];
        }

        public bool HasBingoDataForBet(ulong totalBet)
        {
            return _bingoData.ContainsKey(totalBet);
        }

        public BingoCloverGameResultExtraInfo.Types.BingoData GetNormalBingoData()
        {
            var totalBet = _betState.totalBet;
            if (!_bingoData.ContainsKey(totalBet))
            {
                _bingoData.Add(totalBet, CreateBingoData());
            }
            
            XDebug.Log("NormalBingo:");
            return _bingoData[totalBet];
        }

        public BingoCloverGameResultExtraInfo.Types.BingoData CreateBingoData()
        {
            return new BingoCloverGameResultExtraInfo.Types.BingoData();
        }

        public BingoCloverGameResultExtraInfo.Types.Wheel GetBingoJackpotWheelInfo()
        {
            return extraInfo.Wheel;
        }

        public bool HasNewBingoItemFromRandom()
        {
            if (IsBonusFreeSpin())
            {
                return extraInfo.SuperBingo.RandomIncrease.Count > 0;
            }

            return _bingoData[_betState.totalBet].RandomIncrease.Count > 0;
        }
 
        public RepeatedField<BingoCloverGameResultExtraInfo.Types.BingoHotLine> GetHotLines()
        {
            if (IsBonusFreeSpin())
            {
                return extraInfo.SuperBingo.HotLines;
            }

            return _bingoData[_betState.totalBet].HotLines;
        }

        public bool HasNewBingoItemFromPanel()
        {
            if (IsBonusFreeSpin())
            {
                return extraInfo.SuperBingo.PanelIncrease.Count > 0;
            }

            return _bingoData[_betState.totalBet].PanelIncrease.Count > 0;
        }

        public RepeatedField<BingoCloverGameResultExtraInfo.Types.BingoItem> GetNewBingoItemFromPanel()
        {
            if (IsBonusFreeSpin())
            {
                return extraInfo.SuperBingo.PanelIncrease;
            }

            return _bingoData[_betState.totalBet].PanelIncrease;
        }
        
        public override void UpdateStatePreRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStatePreRoomSetUp(gameEnterInfo);
            _bingoData = extraInfo.BingoMap;
        }
        
        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            SyncBingoDataToBingoMap();
        }

        public override void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            base.UpdateStateOnBonusProcess(sBonusProcess);
            SyncBingoDataToBingoMap();
        }
        
        public override void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            base.UpdateStateOnSettleProcess(settleProcess);
            SyncBingoDataToBingoMap();
        }

        protected void SyncBingoDataToBingoMap()
        {
            if (_bingoData != null && _bingoData.Keys.Count > 0)
            {
                var extraInfoBingoMap = extraInfo.BingoMap;
                foreach (var item in extraInfoBingoMap)
                {
                    if (_bingoData.ContainsKey(item.Key))
                    {
                        _bingoData[item.Key] = item.Value;
                    }
                }

                extraInfo.BingoMap.Clear();
            }
        }
    }
}