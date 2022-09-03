//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-07 14:55
//  Ver : 1.0.0
//  Description : ExtraState11028.cs
//  ChangeLog :
//  **********************************************



#region ************************************Extra Aztec************************************
/*注意这个机器左右都能算赢钱线；
从base进入转盘的时候；scatter的winLine freeSpinCount是0但是bonusGameId>0
free里面reTrigger的scatter的winLine freeSpinCount>0
message AztecGameResultExtraInfo {
    message Wheel {
        bool to_bonus_wheel = 1; // 去bonus转盘
        uint32 rapid_count = 2; // rapid数量
        uint32 free_spin_count = 3; // freeSpin数
        bool is_over = 4; // 直接结束
        uint64 win_rate = 5; // win rate奖励； 当rapid = 3 4
        uint32 jackpot_id = 6; // jackpotId; 当rapid = 5 6 7 8 9
        uint64 jackpot_pay = 7; // jackpot_pay; 当rapid = 5 6 7 8 9, 选中才有值，没中是0
    }

    uint64 bet = 1; // 进入bonus时的bet
    bool is_chosen = 2; // 进入bonus后，是否选完白天黑夜
    bool is_night = 3; // 是不是黑夜
    uint32 normal_wheel_index = 4; // 普通转盘停留index
    repeated Wheel normal_wheel = 5; // 普通转盘信息
    uint32 bonus_wheel_index = 6; // bonus转盘停留index
    repeated uint32 bonus_wheel = 7; // bonus转盘信息，倍率
    bool is_bonus_wheel = 8; // 当前是否是bonus转盘
    bool is_playing = 9; // 是否进入bonus
    bool is_over = 10; // bonus是否可以结算；调用settle api
    uint64 total_win = 11; // bonus总赢钱，包括进来时候的panel线奖
    repeated Panel triggering_panels = 12; // 触发bonus时的panels
}*/
#endregion

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;

namespace GameModule
{
    public class ExtraState11028 : ExtraState<AztecGameResultExtraInfo>
    {
        protected List<uint> rapidHitPayRuleIds = new List<uint>() {3, 4, 19, 20};
        public ExtraState11028(MachineState state) 
            : base(state)
        {

        }
        
        public override bool HasSpecialEffectWhenWheelStop()
        {
            return machineState.Get<WheelsActiveState>().GetRunningWheel()[0].wheelState.GetJackpotWinLines().Count>0;
        }
        
        public override bool HasBonusGame()
        {
            return IsChooseStep() || NeedSettle() || IsWheelStep();
        }

        //选择白天黑夜
        public bool IsChooseStep()
        {
            return !extraInfo.IsChosen && IsWheelStep();
        }
        
        //转盘玩法
        public bool IsWheelStep()
        {
            return extraInfo.IsPlaying;
        }

        //下一个转盘
        public bool IsMultiplierWheel => NormalHit.ToBonusWheel;

        public bool NeedSettle()
        {
            return extraInfo.IsOver && IsWheelStep();
        }
        
        public bool IsNight => extraInfo.IsNight;
        public uint NormalWheelIndex => extraInfo.NormalWheelIndex;
        public bool HasWheelWin => NormalHit.WinRate > 0 || NormalHit.JackpotPay > 0;
        public DragonU3DSDK.Network.API.ILProtocol.AztecGameResultExtraInfo.Types.Wheel NormalHit => extraInfo.NormalWheel[(int)extraInfo.NormalWheelIndex];
        public List<uint> GetNormalWheelData()
        {
            List<uint> dataList = new List<uint>();
            var wheel = extraInfo.NormalWheel;
            for (int i = 0; i < wheel.Count; i++)
            {
                var wheelPan = wheel[i];
                if (wheelPan.FreeSpinCount>0)
                {
                    dataList.Add(wheelPan.FreeSpinCount);
                }
                else
                {
                    dataList.Add(wheelPan.RapidCount);    
                }
            }
            return dataList;
        }

        public uint MultiplierWheelHit => extraInfo.BonusWheel[(int)MultiplierWheelIndex];
        public uint MultiplierWheelIndex => extraInfo.BonusWheelIndex;
        public List<uint> GetMultiplierWheelData()
        {
            List<uint> dataList = new List<uint>();
            var wheel = extraInfo.BonusWheel;
            for (int i = 0; i < wheel.Count; i++)
            {
                var wheelPan = wheel[i];
                dataList.Add(wheelPan);
            }
            return dataList;
        }

        public uint GetJackpotCount()
        {
            return NormalHit.RapidCount;
        }
        
        public uint GetJackpotCount(WinLine jackpotWinLine)
        {
            if (jackpotWinLine.JackpotId >=1 && jackpotWinLine.JackpotId <= 5)
            {
                return jackpotWinLine.JackpotId + 4;
            }
            return jackpotWinLine.JackpotId - 1;
        }
        
        public bool IsJackpotNight(WinLine jackpotWinLine)
        {
            if (jackpotWinLine.JackpotId >=1 && jackpotWinLine.JackpotId <= 5)
            {
                return false;
            }
            return true;
        }
    }
}