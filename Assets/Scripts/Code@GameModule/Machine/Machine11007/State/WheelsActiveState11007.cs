//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-12 18:02
//  Ver : 1.0.0
//  Description : WheelsActiveState11007.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

namespace GameModule
{
    public class WheelsActiveState11007 : WheelsActiveState
    {
        //是否正在进行BonusSpin
        private bool _isBonusWheel;
        public bool IsBonusWheel
        {
            get => _isBonusWheel;
            set => _isBonusWheel = value;
        }
        private bool _isLinkWheel;
        public bool IsLinkWheel
        {
            get => _isLinkWheel;
            set => _isLinkWheel = value;
        }
        private bool _isNormalWheel;
        public bool IsNormalWheel
        {
            get => _isNormalWheel;
            set => _isNormalWheel = value;
        }
        public WheelsActiveState11007(MachineState machineState)
            : base(machineState)
        {

        }
        
        public override void UpdateRunningWheelState(GameResult gameResult)
        {
            var extraInfo = ProtocolUtils.GetAnyStruct<ColossalPigsGameResultExtraInfo>(gameResult.ExtraInfoPb);
            if (gameResult.IsReSpin || (!gameResult.IsReSpin && extraInfo.Items.Count>0 && !extraInfo.Finished))
            {
                UpdateLinkWheelState();
            }
            else
            {
                UpdateNormalWheelState();
            }
        }

        public void UpdateNormalWheelState()
        {
            IsBonusWheel  = false;
            IsLinkWheel   = false;
            IsNormalWheel = true;
            UpdateRunningWheel(new List<string>() {"WheelBaseGame"});
        }

        public void UpdateLinkWheelState()
        {
            IsBonusWheel  = false;
            IsLinkWheel   = true;
            IsNormalWheel = false;
            UpdateRunningWheel(new List<string>() {"WheelRespinLink"});
        }

        public void UpdateBonusWheelState()
        {
            IsBonusWheel  = true;
            IsLinkWheel   = false;
            IsNormalWheel = false;
            UpdateRunningWheel(new List<string>() {"WheelRespinBonus"});
        }
        
        
        
        public override string GetReelNameForWheel(Wheel wheel)
        {
            if (wheel.wheelName.Contains("Bonus"))
            {
                return string.Format("Bonus{0}Reels", machineState.Get<BonusWheelState11007>().CurrentBonusReelId);
            }
            if (wheel.wheelName.Contains("Link"))
            {
                return Constant11007.LinkReels;
            }
            return "Reels";
        }
    }
}