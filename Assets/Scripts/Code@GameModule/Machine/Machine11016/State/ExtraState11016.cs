//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 11:05
//  Ver : 1.0.0
//  Description : ExtraState11016.cs
//  ChangeLog :
//  **********************************************


// message LittleImpGameResultExtraInfo {
//     message BonusGame {
//         repeated Panel triggering_panels = 1; // 触发panels
//         repeated Panel mini_panels = 2; // 小slot
//         uint32 mini_progress = 3; // bonus进度  mini_progress >= mini_panels.length时 bonus完毕
//         repeated uint32 mini_game_ids = 4; //小slot编号
//         uint64 bet = 5; // 进入bonus的bet
//         uint64 pre_win = 6; // 进入bonus的panel win
//         uint64 total_win = 7; // bonus总赢钱
//     }
//
//     BonusGame bonusGame = 1; // bonus数据
//
//     message FreeGame {
//         uint32 starting_panel_count = 1; // 开启freeGame时的起始panel数
//         uint32 current_panel_count = 2; // 当前panel数
//         uint32 last_bomb_count = 3; // 上一次spin炸弹数量
//         uint32 bomb_count = 4; // 当前炸弹数量
//         uint32 bomb_left_next_level = 5; // 到下一级还缺少的炸弹数量
//         uint32 level = 6; // freeGame等级 4, 9是superFree, avgBet 19是megaFree, avgBet
//     }
//     FreeGame freeGame = 2; // freeSpin数据
// }

using System;
using Dlugin;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11016:ExtraState<LittleImpGameResultExtraInfo>
    {
        uint[] averateLevel={5,10,20}; 
        public ExtraState11016(MachineState state) : base(state)
        {
            
        }

        private LittleImpGameResultExtraInfo.Types.BonusGame BonusGame => extraInfo.BonusGame;
        private LittleImpGameResultExtraInfo.Types.FreeGame FreeGame => extraInfo.FreeGame;
        public int FreeGameLevel => (int)FreeGame.Level;
        public int CurrentPanelCount => (int)FreeGame.CurrentPanelCount;
        public int MiniProgress => (int)extraInfo.BonusGame.MiniProgress;
        public int BombLeftNextLevel => (int)FreeGame.BombLeftNextLevel;
        public int MaxFreePanelCount => 9;
        public bool IsBonusFinish => MiniProgress >= BonusGame.MiniGameIds.Count;
        public Google.ilruntime.Protobuf.Collections.RepeatedField<uint> MiniGameIds =>BonusGame.MiniGameIds;
        public Google.ilruntime.Protobuf.Collections.RepeatedField<Panel> MiniPanels => BonusGame.MiniPanels;
        public bool IsAverateBet => Array.IndexOf(averateLevel, FreeGame.Level)>=0;
        public Panel CurrentGamePanel => BonusGame.MiniPanels[MiniProgress-1];
        public int CurrentGameId => (int)extraInfo.BonusGame.MiniGameIds[MiniProgress];

        public override bool HasBonusGame()
        {
            return MiniProgress <= BonusGame.MiniGameIds.Count && BonusGame.MiniGameIds.Count>0;
        }
        
        public Google.ilruntime.Protobuf.Collections.RepeatedField<Position> GetBonusWinLinePositions()
        {
            var panel = BonusGame.TriggeringPanels[0];
            for (int i = 0; i < panel.WinLines.Count; i++)
            {
                var winLine = panel.WinLines[i];
                if (winLine.BonusGameId>0)
                {
                    return winLine.Positions;
                }
            }
            return null;
        }

        public Position GetBonusWinLinePosition(int index)
        {
            var panel = BonusGame.TriggeringPanels[0];
            for (int i = 0; i < panel.WinLines.Count; i++)
            {
                var winLine = panel.WinLines[i];
                if (winLine.BonusGameId>0)
                {
                    return winLine.Positions[index];
                }
            }
            return null;
        }
        
        
        public int GetNextPanelCount(int curPanelCount)
        {
            var listPanelCount = new []{1, 2, 3, 4, 6, 9};
            for (int i = 0; i < listPanelCount.Length; i++)
            {
                var panelCount = listPanelCount[i];
                if (curPanelCount == listPanelCount[i] && i < listPanelCount.Length-1)
                {
                    return listPanelCount[i + 1];
                }
            }
            return 0;
        }

        public override bool HasSpecialEffectWhenWheelStop()
        {
            return machineState.Get<FreeSpinState>().IsInFreeSpin;
        }
    }
}